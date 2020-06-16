using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ExoGif
{
    public class ScreenRecording
    {
        private AnimatedGif.AnimatedGifCreator gif;
        private string outputFileName;
        private readonly int X, Y, W, H;

        public ScreenRecording(string outputFileName, int X, int Y, int W, int H)
        {
            int delay = Convert.ToInt32(1000 / Properties.Settings.Default.framesPerSecond);
            gif = AnimatedGif.AnimatedGif.Create(outputFileName, delay);
            this.outputFileName = outputFileName;
            this.X = X;
            this.Y = Y;
            this.W = W;
            this.H = H;
        }

        public Image CaptureWindow(int Left, int Top, int Width, int Height)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = NativeMethods.GetWindowDC(NativeMethods.GetDesktopWindow());
            // create a device context we can copy to
            IntPtr hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, Width, Height);
            // select the bitmap object
            IntPtr hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            // bitblt over
            NativeMethods.BitBlt(hdcDest, 0, 0, Width, Height, hdcSrc, Left, Top, NativeMethods.SRCCOPY);
            // restore selection
            NativeMethods.SelectObject(hdcDest, hOld);
            // clean up
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(NativeMethods.GetDesktopWindow(), hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            NativeMethods.DeleteObject(hBitmap);
            return img;
        }

        //Instead class structure
        //Save - creates file with delays
        //Add frame function
        //Dispose / close function

        public void SaveFrame()
        {
            Image img = CaptureWindow(X, Y, W, H);
            gif.AddFrame(img);
            img.Dispose();
        }

        public void Close()
        {
            gif.Dispose();
        }

        public void Delete()
        {
            try
            {
                if(File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                }
                else
                {
                    MessageBox.Show("File could not be found");
                }
            }
            catch(IOException ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }
    }
}
