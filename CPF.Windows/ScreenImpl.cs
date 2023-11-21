using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CPF.Drawing;
using CPF.Platform;
using static CPF.Windows.UnmanagedMethods;

namespace CPF.Windows
{
    public class ScreenImpl : Screen
    {
        private readonly IntPtr _hMonitor;
        public ScreenImpl(Rect bounds, Rect workingArea, bool primary, IntPtr hMonitor) : base(bounds, workingArea, primary)
        {
            this._hMonitor = hMonitor;
        }

        //public IReadOnlyList<Screen> GetAllScreens()
        //{
        //    var ScreenCount = GetSystemMetrics(SystemMetric.SM_CMONITORS);
        //    List<Screen> screens = new List<Screen>();
        //    EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
        //        (IntPtr monitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr data) =>
        //        {
        //            screens.Add(FromMonitor(monitor, hdcMonitor));
        //            return true;
        //        }, IntPtr.Zero);
        //    return screens;
        //}

        public static Screen FromMonitor(IntPtr monitor, IntPtr hdc)
        {
            MONITORINFO monitorInfo = MONITORINFO.Create();
            if (GetMonitorInfo(monitor, ref monitorInfo))
            {
                RECT bounds = monitorInfo.rcMonitor;
                RECT workingArea = monitorInfo.rcWork;
                Rect cBounds = new Rect(bounds.left, bounds.top, bounds.right - bounds.left,
                    bounds.bottom - bounds.top);
                Rect cWorkArea =
                    new Rect(workingArea.left, workingArea.top, workingArea.right - workingArea.left,
                        workingArea.bottom - workingArea.top);

                return new ScreenImpl(cBounds, cWorkArea, monitorInfo.dwFlags == 1,
                     monitor);
            }
            return null;
        }

        public override int GetHashCode()
        {
            return (int)_hMonitor;
        }

        public override bool Equals(object obj)
        {
            return (obj is ScreenImpl screen) ? this._hMonitor == screen._hMonitor : base.Equals(obj);
        }

        public override Bitmap Screenshot()
        {
            var srcDC = GetDC(IntPtr.Zero);
            var bounds = Bounds;

            IntPtr memDc = CreateCompatibleDC(srcDC);
            BITMAPINFOHEADER info = new BITMAPINFOHEADER();
            info.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            info.biBitCount = 32;
            info.biHeight = -(int)bounds.Height;
            info.biWidth = (int)bounds.Width;
            info.biPlanes = 1;
            var hBitmap = CreateDIBSection(memDc, ref info, 0, out var ppvBits, IntPtr.Zero, 0);
            var oldBits = SelectObject(memDc, hBitmap);

            BitBlt(memDc, 0, 0, (int)bounds.Width,
                    (int)bounds.Height, srcDC, (int)bounds.X, (int)bounds.Y, TernaryRasterOperations.SRCCOPY);
            Bitmap temp = new Bitmap((int)bounds.Width, (int)bounds.Height, (int)bounds.Width * 4, PixelFormat.Bgra, ppvBits);

            Bitmap bitmap = (Bitmap)temp.Clone();
            temp.Dispose();

            SelectObject(memDc, oldBits);
            DeleteObject(hBitmap);
            DeleteDC(memDc);

            ReleaseDC(IntPtr.Zero, srcDC);
            return bitmap;
        }
    }
}
