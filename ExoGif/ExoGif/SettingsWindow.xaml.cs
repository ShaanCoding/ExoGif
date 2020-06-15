using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow1_ContentRendered(object sender, EventArgs e)
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            if(Properties.Settings.Default.saveDirectory == null || Properties.Settings.Default.saveDirectory == "")
            {
                Properties.Settings.Default.saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                Properties.Settings.Default.Save();
            }

            URLDirectoryTextBox.Text = Properties.Settings.Default.saveDirectory;
            ExoGifSoundOnCaptureCheckBox.IsChecked = Properties.Settings.Default.playSoundCapture;
            ExoGifFolderAfterCaptureCheckBox.IsChecked = Properties.Settings.Default.openFileCapture;
            LengthTextBox.Text = Properties.Settings.Default.recordingLength.ToString();
            FramesPerSecondTextBox.Text = Properties.Settings.Default.framesPerSecond.ToString();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.saveDirectory = URLDirectoryTextBox.Text;
            Properties.Settings.Default.playSoundCapture = (bool)ExoGifSoundOnCaptureCheckBox.IsChecked;
            Properties.Settings.Default.openFileCapture = (bool)ExoGifFolderAfterCaptureCheckBox.IsChecked;

            int recordingLength, framesPerSecond;
            bool successfulyParsedOne = Int32.TryParse(LengthTextBox.Text, out recordingLength);
            bool sucessfulyParsedTwo = Int32.TryParse(FramesPerSecondTextBox.Text, out framesPerSecond);

            if(successfulyParsedOne && sucessfulyParsedTwo)
            {
                Properties.Settings.Default.recordingLength = recordingLength;
                Properties.Settings.Default.framesPerSecond = framesPerSecond;

                //Applys save
                Properties.Settings.Default.Save();
                this.Close();
            }
            else
            {
                MessageBox.Show("Error: Frames per second or recording length are not ints");
            }
        }

        private void URLDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            using(CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog())
            {
                openFileDialog.IsFolderPicker = true;
                if(openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    URLDirectoryTextBox.Text = openFileDialog.FileName;
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            //Save function
            SaveSettings();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
