using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExoGif
{
    public class ScreenRecording
    {
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

        public void Save(string outputFileName, int fps, int timeSeconds, int X, int Y, int W, int H)
        {
            int delay = Convert.ToInt32(1000 / fps);
            int frames = timeSeconds * fps;

            using (var gif = AnimatedGif.AnimatedGif.Create(outputFileName, delay))
            {
                for (int i = 0; i < frames; i++)
                {
                    Image img = CaptureWindow(X, Y, W, H);
                    System.Threading.Thread.Sleep(delay);
                    gif.AddFrame(img);
                    img.Dispose();
                }
            }
        }
    }
}
