﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExoGif
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private Point Start;
        private Point Current;
        private bool isDrawing;
        private readonly double X, Y, W, H;

        private MP3Player startPlayer;
        private MP3Player endPlayer;

        public CaptureWindow()
        {
            InitializeComponent();
            startPlayer = new MP3Player(Environment.CurrentDirectory + @"\Assets\Sounds\startSound.mp3", "startSound", false);
            startPlayer.Volume("startSound", 1000);
            endPlayer = new MP3Player(Environment.CurrentDirectory + @"\Assets\Sounds\endSound.mp3", "endSound", false);
            endPlayer.Volume("endSound", 1000);
        }

        private void Grid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            Start = Mouse.GetPosition(Canvas1);

            Canvas.SetLeft(Rect, Start.X);
            Canvas.SetTop(Rect, Start.Y);
        }

        private void Grid1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;

            Grid1.Background = new SolidColorBrush(Colors.Black) { Opacity = 0.0 };
            Canvas1.Background = new SolidColorBrush(Colors.Black) { Opacity = 0.0 };
            Rect.Fill = Brushes.Transparent;
            Rect.Opacity = 1;

            //Allows click through
            var hwnd = new WindowInteropHelper(this).Handle;
            NativeMethods.SetWindowExTransparent(hwnd);

            if(Properties.Settings.Default.playSoundCapture)
            {
                //Recording start sound
                startPlayer.Play("startSound");
            }

            // Calculate rectangle cords/size
            Point revampedPoint = PointToScreen(new Point(X, Y));
            string fileName = Properties.Settings.Default.saveDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".gif";
            ControlWindow controlWindow = new ControlWindow(fileName, (int)revampedPoint.X, (int)revampedPoint.Y, (int)W, (int)H);
            controlWindow.ShowDialog();

            if (Properties.Settings.Default.playSoundCapture)
            {
                endPlayer.Play("endSound");
            }

            this.Close();
        }

        private void Grid1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Get new position
                Current = Mouse.GetPosition(Canvas1);

                // Calculate rectangle cords/size
                X = Math.Min(Current.X, Start.X);
                Y = Math.Min(Current.Y, Start.Y);
                W = Math.Max(Current.X, Start.X) - X;
                H = Math.Max(Current.Y, Start.Y) - Y;

                Canvas.SetLeft(Rect, X);
                Canvas.SetTop(Rect, Y);

                // Update rectangle
                Rect.Width = W;
                Rect.Height = H;
                Rect.SetValue(Canvas.LeftProperty, X);
                Rect.SetValue(Canvas.TopProperty, Y);

                // Toogle visibility
                if (Rect.Visibility != Visibility.Visible)
                {
                    Rect.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
