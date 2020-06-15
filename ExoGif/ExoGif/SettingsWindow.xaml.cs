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
            if(Properties.Settings.Default.saveDirectory == null)
            {
                Properties.Settings.Default.saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                Properties.Settings.Default.Save();
            }

            URLDirectoryTextBox.Text = Properties.Settings.Default.saveDirectory;
            ExoGifBackgroundProcessCheckBox.IsChecked = Properties.Settings.Default.backgroundProcess;
            ExoGifSoundOnCaptureCheckBox.IsChecked = Properties.Settings.Default.playSoundCapture;
            ExoGifFolderAfterCaptureCheckBox.IsChecked = Properties.Settings.Default.openFileCapture;

            /*
            //Also need hotkey one
            //String.split it
            string[] hotkeySplit = Properties.Settings.Default.hotkeySettings.Split(' ');
            HotKeyListBoxOne.item
            */
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.saveDirectory = URLDirectoryTextBox.Text;
            Properties.Settings.Default.backgroundProcess = (bool)ExoGifBackgroundProcessCheckBox.IsChecked;
            Properties.Settings.Default.playSoundCapture = (bool)ExoGifSoundOnCaptureCheckBox.IsChecked;
            Properties.Settings.Default.openFileCapture = (bool)ExoGifFolderAfterCaptureCheckBox.IsChecked;

            //Need hotkey one to be implemented later

            //Applys save
            Properties.Settings.Default.Save();
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
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
