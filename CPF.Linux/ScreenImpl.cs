using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Platform;
using System.Globalization;
using System.Linq;
using static CPF.Linux.XLib;
using System.Runtime.InteropServices;

namespace CPF.Linux
{
    public class ScreenImpl : Screen
    {
        public ScreenImpl(Rect bounds, Rect workingArea, bool primary,
            string name, Size? physicalSize, double? pixelDensity) : base(bounds, workingArea, primary)
        {
            XWorkingArea = workingArea;
            Name = name;
            if (physicalSize == null && pixelDensity == null)
            {
                PixelDensity = 1;
            }
            else if (pixelDensity == null)
            {
                PixelDensity = GuessPixelDensity(bounds.Width, physicalSize.Value.Width);
            }
            else
            {
                PixelDensity = pixelDensity.Value;
                PhysicalSize = physicalSize;
            }
        }
        private const int FullHDWidth = 1920;
        public string Name { get; set; }
        public Size? PhysicalSize { get; set; }
        public double PixelDensity { get; set; }

        public Rect XWorkingArea { get; set; }

        public override Bitmap Screenshot()
        {
            var bounds = Bounds;
            var image = XGetImage(LinuxPlatform.Platform.Display, LinuxPlatform.Platform.Info.RootWindow, (int)bounds.X, (int)bounds.Y, (int)bounds.Width,
    (int)bounds.Height, ~0, 2 /* ZPixmap*/);
            if (image == IntPtr.Zero)
            {
                string s = String.Format("XGetImage returned NULL when asked to for a {0}x{1} region block",
                    bounds.Width, bounds.Height);
                throw new InvalidOperationException(s);
            }

            Bitmap bmp = new Bitmap((int)bounds.Width, (int)bounds.Height);
            var visual = LinuxPlatform.Platform.Info.TransparentVisualInfo;
            int red, blue, green;
            int red_mask = (int)visual.red_mask;
            int blue_mask = (int)visual.blue_mask;
            int green_mask = (int)visual.green_mask;
            using (var b = bmp.Lock())
            {
                for (int y = 0; y < bounds.Height; y++)
                {
                    for (int x = 0; x < bounds.Width; x++)
                    {
                        var pixel = XGetPixel(image, x, y);

                        switch (visual.depth)
                        {
                            case 16: /* 16bbp pixel transformation */
                                red = (int)((pixel & red_mask) >> 8) & 0xff;
                                green = (int)(((pixel & green_mask) >> 3)) & 0xff;
                                blue = (int)((pixel & blue_mask) << 3) & 0xff;
                                break;
                            case 24:
                            case 32:
                                red = (int)((pixel & red_mask) >> 16) & 0xff;
                                green = (int)(((pixel & green_mask) >> 8)) & 0xff;
                                blue = (int)((pixel & blue_mask)) & 0xff;
                                break;
                            default:
                                string text = string.Format("{0}bbp depth not supported.", visual.depth);
                                throw new NotImplementedException(text);
                        }

                        b.SetPixel(x, y, 255, (byte)red, (byte)green, (byte)blue);
                    }
                }
            }

            XDestroyImage(image);
            return bmp;
        }

        public static double GuessPixelDensity(double pixelWidth, double mmWidth)
            => pixelWidth <= FullHDWidth ? 1 : Math.Max(1, Math.Round(pixelWidth / mmWidth * 25.4 / 96));


        //private static X11ScreensUserSettings _settings;
        private static ScreenImpl[] _cache;
        private static X11Info _x11;
        private static XWindow _window;
        //用来解决虚拟机里调整屏幕分辨率之后缓存读取到屏幕尺寸不对的问题
        static byte isScreenChange;
        static ScreenImpl()
        {
            //_settings = X11ScreensUserSettings.Detect();
            _x11 = LinuxPlatform.Platform.Info;
            _window = new XWindow
            {
                EventAction = (ref XEvent ev) =>
                {
                    // Invalidate cache on RRScreenChangeNotify
                    if ((int)ev.type == _x11.RandrEventBase + (int)RandrEvent.RRScreenChangeNotify)
                    {
                        _cache = null;
                        isScreenChange = 3;
                    }
                }
            };
            XRRSelectInput(_x11.Display, _window.Handle, RandrEventMask.RRScreenChangeNotify);
        }


        public static unsafe ScreenImpl[] Screens
        {
            get
            {
                if (_cache != null && isScreenChange > 0)
                {
                    isScreenChange--;
                    _cache = null;
                }
                if (_cache != null)
                    return _cache;
                //Console.WriteLine("Screens");
                var monitors = XRRGetMonitors(_x11.Display, _window.Handle, true, out var count);

                //Console.WriteLine("Screens:" + count);
                if (count > 0)
                {
                    var screens = new ScreenImpl[count];
                    for (var c = 0; c < count; c++)
                    {
                        var mon = monitors[c];
                        var namePtr = XGetAtomName(_x11.Display, mon.Name);
                        var name = Marshal.PtrToStringAnsi(namePtr);
                        XFree(namePtr);

                        var density = 1d;
                        //if (_settings.NamedScaleFactors?.TryGetValue(name, out density) != true)
                        {
                            if (mon.MWidth == 0)
                                density = 1;
                            else
                                density = ScreenImpl.GuessPixelDensity(mon.Width, mon.MWidth);
                        }
                        //for (int o = 0; o < mon.NOutput; o++)
                        //{
                        //    Console.WriteLine(GetPhysicalMonitorSizeFromEDID(mon.Outputs[o]));
                        //}
                        //density *= _settings.GlobalScaleFactor;

                        var bounds = new Rect(mon.X, mon.Y, mon.Width, mon.Height);
                        screens[c] = new ScreenImpl(bounds, bounds,
                            mon.Primary != 0,
                            name,
                            (mon.MWidth == 0 || mon.MHeight == 0) ? (Size?)null : new Size(mon.MWidth, mon.MHeight),
                            density);
                    }

                    _cache = UpdateWorkArea(_x11, screens);
                }
                else
                {
                    Console.WriteLine("可能无法显示界面，请检查显卡驱动以及开启桌面混合");
                    _cache = new ScreenImpl[1] { new ScreenImpl(new Rect(0, 0, 1024, 768), new Rect(0, 0, 1024, 768), true, "默认", null, null) { } };
                }

                GetDpi();
                XFree(new IntPtr(monitors));
                return _cache;
            }
        }

        public static unsafe void GetDpi()
        {
            var resourceString = XResourceManagerString(LinuxPlatform.Platform.Display);
            IntPtr db;
            XrmValue value = new XrmValue();
            IntPtr type = IntPtr.Zero;
            double dpi = 96;


            if (resourceString != IntPtr.Zero)
            {
                db = XrmGetStringDatabase(resourceString);
                //Console.WriteLine(resourceString);
                if (XrmGetResource(db, "Xft.dpi", "String", ref type, ref value))
                {
                    if (value.addr != null)
                    {
                        dpi = atof(value.addr);
                        Console.WriteLine("DPI:" + dpi);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("XResourceManagerString读取失败");
            }
            var scale = (float)dpi / 96;
            if (scale != DpiScale)
            {
                DpiScale = scale;

                foreach (var item in LinuxPlatform.Platform.windows)
                {
                    if (item.Value is X11Window window)
                    {
                        window.UpdateScaling();
                    }
                }
            }

        }

        public static float DpiScale = 1;

        //const int EDIDStructureLength = 32; // Length of a EDID-Block-Length(128 bytes), XRRGetOutputProperty multiplies offset and length by 4
        //private static unsafe Size? GetPhysicalMonitorSizeFromEDID(IntPtr rrOutput)
        //{
        //    if (rrOutput == IntPtr.Zero)
        //        return null;
        //    var properties = XLib.XRRListOutputProperties(_x11.Display, rrOutput, out int propertyCount);
        //    var hasEDID = false;
        //    for (var pc = 0; pc < propertyCount; pc++)
        //    {
        //        if (properties[pc] == _x11.Atoms.EDID)
        //            hasEDID = true;
        //    }
        //    if (!hasEDID)
        //        return null;
        //    XLib.XRRGetOutputProperty(_x11.Display, rrOutput, _x11.Atoms.EDID, 0, EDIDStructureLength, false, false, _x11.Atoms.AnyPropertyType, out IntPtr actualType, out int actualFormat, out int bytesAfter, out _, out IntPtr prop);
        //    if (actualType != _x11.Atoms.XA_INTEGER)
        //        return null;
        //    if (actualFormat != 8) // Expecting an byte array
        //        return null;

        //    var edid = new byte[bytesAfter];
        //    Marshal.Copy(prop, edid, 0, bytesAfter);
        //    XFree(prop);
        //    XFree(new IntPtr(properties));
        //    if (edid.Length < 22)
        //        return null;
        //    var width = edid[21]; // 0x15 1 Max. Horizontal Image Size cm. 
        //    var height = edid[22]; // 0x16 1 Max. Vertical Image Size cm. 
        //    if (width == 0 && height == 0)
        //        return null;
        //    return new Size(width * 10, height * 10);
        //}

        static unsafe ScreenImpl[] UpdateWorkArea(X11Info info, ScreenImpl[] screens)
        {
            var rect = default(Rect);
            foreach (var s in screens)
            {
                rect.Union(s.Bounds);
                //Fallback value
                s.XWorkingArea = s.Bounds;
            }

            var res = XGetWindowProperty(info.Display,
                info.RootWindow,
                info.Atoms._NET_WORKAREA,
                IntPtr.Zero,
                new IntPtr(128),
                false,
                info.Atoms.AnyPropertyType,
                out var type,
                out var format,
                out var count,
                out var bytesAfter,
                out var prop);

            if (res != (int)Status.Success || type == IntPtr.Zero ||
                format == 0 || bytesAfter.ToInt64() != 0 || count.ToInt64() % 4 != 0)
            { return screens.Select(a => new ScreenImpl(a.Bounds, a.XWorkingArea, a.Primary, a.Name, a.PhysicalSize, a.PixelDensity)).ToArray(); }

            var pwa = (IntPtr*)prop;
            var wa = new Rect(pwa[0].ToInt32(), pwa[1].ToInt32(), pwa[2].ToInt32(), pwa[3].ToInt32());


            foreach (var s in screens)
            {
                var b = s.Bounds;
                b.Intersect(wa);
                s.XWorkingArea = b;
            }

            XFree(prop);
            return screens.Select(a => new ScreenImpl(a.Bounds, a.XWorkingArea, a.Primary, a.Name, a.PhysicalSize, a.PixelDensity)).ToArray();
        }
    }

    //class X11ScreensUserSettings
    //{
    //    public double GlobalScaleFactor { get; set; } = 1;
    //    public Dictionary<string, double> NamedScaleFactors { get; set; }

    //    static double? TryParse(string s)
    //    {
    //        if (s == null)
    //            return null;
    //        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var rv))
    //            return rv;
    //        return null;
    //    }


    //    public static X11ScreensUserSettings DetectEnvironment()
    //    {
    //        var globalFactor = Environment.GetEnvironmentVariable("CPF_GLOBAL_SCALE_FACTOR");
    //        var screenFactors = Environment.GetEnvironmentVariable("CPF_SCREEN_SCALE_FACTORS");
    //        if (globalFactor == null && screenFactors == null)
    //            return null;

    //        var rv = new X11ScreensUserSettings
    //        {
    //            GlobalScaleFactor = TryParse(globalFactor) ?? 1
    //        };

    //        try
    //        {
    //            if (!string.IsNullOrWhiteSpace(screenFactors))
    //            {
    //                rv.NamedScaleFactors = screenFactors.Split(';').Where(x => !string.IsNullOrWhiteSpace(x))
    //                    .Select(x => x.Split('=')).ToDictionary(x => x[0],
    //                        x => double.Parse(x[1], CultureInfo.InvariantCulture));
    //            }
    //        }
    //        catch
    //        {
    //            //Ignore
    //        }

    //        return rv;
    //    }


    //    public static X11ScreensUserSettings Detect()
    //    {
    //        return DetectEnvironment() ?? new X11ScreensUserSettings();
    //    }
    //}
}
