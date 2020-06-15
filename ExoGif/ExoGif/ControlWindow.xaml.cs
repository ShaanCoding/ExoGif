using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace ExoGif
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        //Cancel, Start-STOP, Submit
        public bool isPause = false;
        public bool isSubmit = false;

        ScreenRecording screenRecording;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public ControlWindow(string outputFileName, int X, int Y, int W, int H)
        {
            InitializeComponent();
            screenRecording = new ScreenRecording(outputFileName, X, Y, W, H);
            //Means we have a progress / timer bar of max limit
            worker.WorkerReportsProgress = true;

            worker.WorkerSupportsCancellation = true;
            worker.DoWork += BackgroundWorkerRecording;
            worker.ProgressChanged += BackgroundWorkerRecording_ProgressChanged;
            worker.RunWorkerCompleted += BackgroundWorkerRecording_RunWorkerCompleted;

            //Starts worker
            worker.RunWorkerAsync();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPause)
            {
                //If is paused we unpause
                isPause = false;

                Uri resourceUri = new Uri("Assets/Buttons/pause.png", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush();
                brush.ImageSource = temp;
                StartStopButton.Background = brush;
            }
            else
            {
                isPause = true;

                Uri resourceUri = new Uri("Assets/Buttons/recording.png", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush();
                brush.ImageSource = temp;
                StartStopButton.Background = brush;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            isSubmit = true;
        }

        private void BackgroundWorkerRecording(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            //Do recording in here
            int frames = Properties.Settings.Default.recordingLength * Properties.Settings.Default.framesPerSecond;
            int delay = Convert.ToInt32(1000 / Properties.Settings.Default.framesPerSecond);

            for (int i = 0; i < frames; i++)
            {
                screenRecording.SaveFrame();
                System.Threading.Thread.Sleep(delay);

                //pause
                while(isPause)
                {
                    System.Threading.Thread.Sleep(10);
                    //if program is set to complete whilst paused or is cancelled
                    if(isSubmit == true || worker.CancellationPending)
                    {
                        break;
                    }
                }

                //Cancels
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                //Finishes early
                if (isSubmit == true)
                {
                    break;
                }

                //Return progressbar%
                int returnProgressPercent = (int)Math.Ceiling((decimal)(i) / frames * 100);
                worker.ReportProgress(returnProgressPercent);
            }

            //Opens my recordings
            if (Properties.Settings.Default.openFileCapture && e.Cancel == false)
            {
                Process.Start("explorer.exe", ExoGif.Properties.Settings.Default.saveDirectory);
            }
        }

        private void BackgroundWorkerRecording_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                //Means canceleed
                //Set stop variables i.e change button to start, progress bar 100% etc
                screenRecording.Close();

                //Delete file
                screenRecording.Delete();

            }
            else if(e.Error != null)
            {
                //Means error occured
                screenRecording.Close();
            }
            else
            {
                //This means it is done finishing successfully
                screenRecording.Close();
            }

            this.Close();
        }

        private void BackgroundWorkerRecording_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Updates UI-elements on update
            TimeRecordedBar.Value = e.ProgressPercentage;
            //Add text based timer of remaining time
        }

    }
}
