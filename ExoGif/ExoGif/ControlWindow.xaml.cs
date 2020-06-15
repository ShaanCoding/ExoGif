﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public int fps;
        public int timeSeconds;

        ScreenRecording screenRecording;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public ControlWindow(string outputFileName, int fps, int timeSeconds, int X, int Y, int W, int H)
        {
            InitializeComponent();
            this.fps = fps;
            this.timeSeconds = timeSeconds;
            screenRecording = new ScreenRecording(outputFileName, fps, X, Y, W, H);
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
                StartStopButton.Content = "Stop";
            }
            else
            {
                isPause = true;
                StartStopButton.Content = "Start";
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
            int frames = timeSeconds * fps;
            int delay = Convert.ToInt32(1000 / fps);

            for (int i = 0; i < frames; i++)
            {
                screenRecording.SaveFrame();
                System.Threading.Thread.Sleep(delay);

                //pause
                while(isPause)
                {
                    System.Threading.Thread.Sleep(10);
                    //if program is set to complete whilst paused
                    if(isSubmit == true)
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
