using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace ExoGif
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //THIS APP SHOULD OPEN AS A BG PROCESS IN AUTO-STARTUP
            if(e.Args.Count() == 1)
            {
                //Should be a switch statement
                switch(e.Args[0])
                {
                    case "/captureGIF":
                        CaptureWindow captureWindow = new CaptureWindow();
                        captureWindow.Show();
                        break;
                    case "/myRecordings":
                        //Open my recordings
                        Process.Start("explorer.exe", ExoGif.Properties.Settings.Default.saveDirectory);
                        break;
                    case "/settings":
                        SettingsWindow settingsWindow = new SettingsWindow();
                        settingsWindow.Show();
                        break;
                    case "/help":
                        Process.Start("https://github.com/ShaanCoding/ExoGif");
                        break;
                }
            }
            else
            {
                //If opened normally
                CaptureWindow captureWindow = new CaptureWindow();
                captureWindow.Show();
            }

            //Capture Gif, My Recordings, Settings, Help
            JumpTask captureGIFTask = new JumpTask
            {
                Title = "Capture GIF",
                Arguments = "/captureGIF",
                Description = "Records the desktop as a GIF",
                CustomCategory = "Tasks",
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };

            JumpTask myRecordingsTask = new JumpTask
            {
                Title = "My Recordings",
                Arguments = "/myRecordings",
                Description = "Takes you to your GIF recordings",
                CustomCategory = "⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯",
                //ICON IS TEMP
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };

            JumpTask settingsTask = new JumpTask
            {
                Title = "Settings",
                Arguments = "/settings",
                Description = "Takes you to the settings menu",
                CustomCategory = "⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯",
                //ICON IS TEMP
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };

            JumpTask helpTask = new JumpTask
            {
                Title = "Help",
                Arguments = "/help",
                Description = "Takes you to the help menu",
                CustomCategory = "⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯",
                //Icon is temp
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };

            JumpList jumpList = new JumpList();
            jumpList.JumpItems.Add(captureGIFTask);
            jumpList.JumpItems.Add(myRecordingsTask);
            jumpList.JumpItems.Add(settingsTask);
            jumpList.JumpItems.Add(helpTask);
            jumpList.ShowFrequentCategory = false;
            jumpList.ShowRecentCategory = false;
            JumpList.SetJumpList(Application.Current, jumpList);
        }
    }
}
