using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using CPF.Input;
using static CPF.Linux.XLib;
using CPF.OpenGL;

namespace CPF.Linux
{
    public class XWindow : IDisposable
    {
        internal static object XlibLock = new object();
        public XWindow()
        {
            OnCreateWindw();
            LinuxPlatform.Platform.windows.Add(Handle, this);
        }

        protected virtual void OnCreateWindw()
        {
            Handle = XCreateSimpleWindow(LinuxPlatform.Platform.Display, LinuxPlatform.Platform.Info.DefaultRootWindow,
                0, 0, 1, 1, 0, IntPtr.Zero, IntPtr.Zero);
        }

        public IntPtr Handle { get; protected set; }

        public OnEventHandler EventAction { get; set; }
        public virtual void OnEvent(ref XEvent e)
        {
            if (EventAction != null)
            {
                EventAction(ref e);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                if (Handle != IntPtr.Zero)
                {
                    XDestroyWindow(LinuxPlatform.Platform.Display, Handle);
                    LinuxPlatform.Platform.windows.Remove(Handle);
                    Handle = IntPtr.Zero;
                }

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~XWindow()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public delegate void OnEventHandler(ref XEvent xEvent);
    public unsafe class X11Window : XWindow, IWindowImpl
    {
        internal static IntPtr mouseDownWindow;
        public static X11Window main;
        protected override void OnCreateWindw()
        {
            if (main == null)
            {
                //Thread.Sleep(10000);
                main = this;
            }
            x11info = LinuxPlatform.Platform.Info;
            XSetWindowAttributes attr = new XSetWindowAttributes();
            var valueMask = default(SetWindowValuemask);

            attr.background_pixel = IntPtr.Zero;
            attr.border_pixel = IntPtr.Zero;
            attr.backing_store = 1;
            attr.bit_gravity = Gravity.NorthWestGravity;
            attr.win_gravity = Gravity.NorthWestGravity;
            attr.override_redirect = this is PopWindow;
            valueMask |= SetWindowValuemask.BackPixel | SetWindowValuemask.BorderPixel | SetWindowValuemask.BackingStore | SetWindowValuemask.OverrideRedirect
                         //| SetWindowValuemask.BackPixmap 
                         | SetWindowValuemask.BitGravity | SetWindowValuemask.WinGravity;
            var depth = 32;// (int)x11info.TransparentVisualInfo.depth;
            attr.colormap = XCreateColormap(x11info.Display, x11info.RootWindow, x11info.TransparentVisualInfo.visual, 0);
            valueMask |= SetWindowValuemask.ColorMap;
            size = new PixelSize(300, 200);
            position = new PixelPoint(10, 10);

            Handle = XCreateWindow(x11info.Display, x11info.RootWindow, 10, 10, size.Width, size.Height, 0, depth, (int)CreateWindowArgs.InputOutput, x11info.TransparentVisualInfo.visual, new UIntPtr((uint)valueMask), ref attr);

            //      var attributes = new XSetWindowAttributes();
            //      attributes.background_pixel = XWhitePixel(x11info.Display, x11info.DefaultScreen);
            //      Handle = XCreateWindow(x11info.Display, x11info.RootWindow,
            //0, 0, 100, 100, 0, depth,
            //(int)CreateWindowArgs.InputOutput, XDefaultVisual(x11info.Display, 0)
            //, ((ulong)SetWindowValuemask.BackPixel), ref attributes);

            //Handle = XCreateSimpleWindow(x11info.Display, x11info.RootWindow, 10, 10, 100, 100, 0, XWhitePixel(x11info.Display, x11info.DefaultScreen), XWhitePixel(x11info.Display, x11info.DefaultScreen));

            //      var attributes = new XSetWindowAttributes();
            //      attributes.background_pixel = XWhitePixel(x11info.Display, x11info.DefaultScreen);
            //      Handle = XCreateWindow(x11info.Display, x11info.RootWindow,
            //0, 0, 100, 100, 0, depth,
            //(int)CreateWindowArgs.InputOutput, XDefaultVisual(x11info.Display, 0)
            //, ((ulong)SetWindowValuemask.BackPixel), ref attributes);


            var protocols = new[]
            {
                x11info.Atoms.WM_DELETE_WINDOW
            };
            XSetWMProtocols(x11info.Display, Handle, protocols, protocols.Length);
            XChangeProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_WINDOW_TYPE, x11info.Atoms.XA_ATOM,
                32, PropertyMode.Replace, new[] { x11info.Atoms._NET_WM_WINDOW_TYPE_NORMAL }, 1);

            SetWmClass("CPFApplication");

            UpdateMotifHints();

            var def = IntPtr.Zero;
            var fontset = XCreateFontSet(x11info.Display, "-*-*-*-*-*-*-14-*-*-*-*-*-*-*", out var missing_charset_list_return, out var missing_charset_count_return, ref def);

            //Console.WriteLine("Display:" + x11info.Display + " fontset:" + fontset + " missing_charset_count_return:" + missing_charset_count_return);

            //var list = XVaCreateNestedList(0, XNames.XNFontSet, fontset, IntPtr.Zero);
            XPoint spot = new XPoint();
            var pSL = Marshal.StringToHGlobalAnsi(XNames.XNSpotLocation);
            var pFS = Marshal.StringToHGlobalAnsi(XNames.XNFontSet);
            var list = XVaCreateNestedList(0, pSL, spot, pFS, fontset, IntPtr.Zero);

            //Console.WriteLine("list:" + list);

            //Console.WriteLine(string.Join(" - ", GetSupportedInputStyles(x11info.Xim)));
            //UpdateSizeHints(null);
            xic = XCreateIC(x11info.Xim, XNames.XNInputStyle, styleOverTheSpot,
                XNames.XNClientWindow, Handle, XNames.XNPreeditAttributes, list, IntPtr.Zero);
            //xic = LinuxPlatform.CreateIC(x11info.Xim, Handle);
            //Console.WriteLine("xic:" + xic);

            if (xic == IntPtr.Zero)
            {
                //ximStyle = styleRoot;
                xic = XCreateIC(x11info.Xim,
                    XNames.XNInputStyle, styleRoot,
                    XNames.XNClientWindow, Handle,
                    XNames.XNFocusWindow, Handle,
                    IntPtr.Zero);
                Console.WriteLine("xic:" + xic);
            }
            if (pSL != IntPtr.Zero)
                Marshal.FreeHGlobal(pSL);
            if (pFS != IntPtr.Zero)
                Marshal.FreeHGlobal(pFS);
            XFree(list);
            //long im_event_mask = 0;
            ////XIMStyles xIMStyles = new XIMStyles();
            //var icv = XGetICValues(_xic, XNames.XNFilterEvents, ref im_event_mask, IntPtr.Zero);
            //Console.WriteLine(icv);

            XEventMask ignoredMask = XEventMask.SubstructureRedirectMask
                         | XEventMask.ResizeRedirectMask
                         | XEventMask.PointerMotionHintMask;
            if (LinuxPlatform.Platform.XI2 != null)
                LinuxPlatform.Platform.XI2.AddWindow(Handle);

            var mask = new IntPtr(0xffffff ^ (int)ignoredMask);
            //lock (XlibLock)
            //{
            XSelectInput(x11info.Display, Handle, mask);
            //}

            //启用拖拽
            IntPtr[] XdndVersion = new IntPtr[] { new IntPtr(4) };
            int[] atoms;
            atoms = new int[XdndVersion.Length];
            for (int i = 0; i < XdndVersion.Length; i++)
            {
                atoms[i] = XdndVersion[i].ToInt32();
            }
            XChangeProperty(x11info.Display, Handle, x11info.Atoms.XdndAware,
                    (IntPtr)Atom.XA_ATOM, 32,
                    PropertyMode.Replace, atoms, 1);

            ScreenImpl.GetDpi();

            XFlush(x11info.Display);
            //Thread.Sleep(10000);
            //var att = GetXWindowAttributes();
            //Console.WriteLine(att);
            if (Application.GetDrawingFactory().UseGPU)
            {
                //Thread.Sleep(5000);
                context = new OpenGL.GlxContext(this);
                if (context.Context == IntPtr.Zero)
                {
                    Application.GetDrawingFactory().UseGPU = false;
                    context = null;
                }
            }
        }
        CPF.Linux.OpenGL.GlxContext context;

        //private IEnumerable GetMatchingStylesInPreferredOrder(IntPtr xim)
        //{
        //    XIMProperties[] supportedStyles = GetSupportedInputStyles(xim);
        //    foreach (XIMProperties p in GetPreferredStyles())
        //        if (Array.IndexOf(supportedStyles, p) >= 0)
        //            yield return p;
        //}
        public static XIMProperties[] GetSupportedInputStyles(IntPtr xim)
        {
            //Thread.Sleep(10000);
            IntPtr stylesPtr;
            string ret = XGetIMValues(xim, XNames.XNQueryInputStyle, out stylesPtr, IntPtr.Zero);
            if (ret != null || stylesPtr == IntPtr.Zero)
                return new XIMProperties[0];
            XIMStyles styles = (XIMStyles)Marshal.PtrToStructure(stylesPtr, typeof(XIMStyles));
            XIMProperties[] supportedStyles = new XIMProperties[styles.count_styles];
            var p = (long*)styles.supported_styles;
            for (int i = 0; i < styles.count_styles; i++)
            {
                //supportedStyles[i] = (XIMProperties)Marshal.PtrToStructure(new IntPtr((long)styles.supported_styles + i * Marshal.SizeOf (typeof (IntPtr)))), typeof(XIMProperties));
                supportedStyles[i] = (XIMProperties)p[i];
            }
            XFree(stylesPtr);
            return supportedStyles;
        }

        const XIMProperties styleRoot = XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing;
        const XIMProperties styleOverTheSpot = XIMProperties.XIMPreeditPosition | XIMProperties.XIMStatusNothing;
        const XIMProperties styleOnTheSpot = XIMProperties.XIMPreeditCallbacks | XIMProperties.XIMStatusNothing;
        //private XIMProperties[] GetPreferredStyles()
        //{
        //    var env = "over-the-spot";
        //    string[] list = env.Split(' ');
        //    XIMProperties[] ret = new XIMProperties[list.Length];
        //    for (int i = 0; i < list.Length; i++)
        //    {
        //        string s = list[i];
        //        switch (s)
        //        {
        //            case "over-the-spot":
        //                ret[i] = styleOverTheSpot;
        //                break;
        //            case "on-the-spot":
        //                ret[i] = styleOnTheSpot;
        //                break;
        //            case "root":
        //                ret[i] = styleRoot;
        //                break;
        //        }
        //    }
        //    return ret;
        //}


        private void SetWmClass(string wmClass)
        {
            var data = Encoding.ASCII.GetBytes(wmClass);
            fixed (void* pdata = data)
            {
                XChangeProperty(x11info.Display, Handle, x11info.Atoms.XA_WM_CLASS, x11info.Atoms.XA_STRING, 8,
                    PropertyMode.Replace, pdata, data.Length);
            }
        }

        X11Info x11info;
        IntPtr xic;
        //X11Window parent;
        internal View root;
        PixelSize size;
        PixelPoint position;
        //private bool have_Xutf8ResetIC = true;
        public PixelSize Size
        {
            get { return size; }
        }
        void UpdateMotifHints()
        {
            var functions = MotifFunctions.Move | MotifFunctions.Close | MotifFunctions.Resize |
                            MotifFunctions.Minimize | MotifFunctions.Maximize; ;// 
            //var decorations = MotifDecorations.Menu | MotifDecorations.Title | MotifDecorations.Border |
            //                  MotifDecorations.Maximize | MotifDecorations.Minimize | MotifDecorations.ResizeH;

            //if (_popup || _systemDecorations == SystemDecorations.None)
            //{
            //    decorations = 0;
            //}
            //else if (_systemDecorations == SystemDecorations.BorderOnly)
            //{
            //    decorations = MotifDecorations.Border;
            //}

            //if (!_canResize || _systemDecorations == SystemDecorations.BorderOnly)
            //{
            //    functions &= ~(MotifFunctions.Resize | MotifFunctions.Maximize);
            //    decorations &= ~(MotifDecorations.Maximize | MotifDecorations.ResizeH);
            //}

            var hints = new MotifWmHints
            {
                flags = new IntPtr((int)(MotifFlags.Decorations | MotifFlags.Functions)),//
                //decorations = new IntPtr((int)decorations),
                functions = new IntPtr((int)functions)
            };

            XChangeProperty(x11info.Display, Handle,
                x11info.Atoms._MOTIF_WM_HINTS, x11info.Atoms._MOTIF_WM_HINTS, 32,
                PropertyMode.Replace, ref hints, 5);
        }

        void UpdateSizeHints(PixelSize? preResize)
        {
            var size = Screen.WorkingArea.Size;
            var min = new PixelSize(1, 1); //_minMaxSize.minSize;
            var max = new PixelSize((int)size.Width, (int)size.Height);//_minMaxSize.maxSize;

            //if (!_canResize || _systemDecorations == SystemDecorations.BorderOnly)
            //    max = min = _realSize;

            if (preResize.HasValue)
            {
                var desired = preResize.Value;
                max = new PixelSize(Math.Max(desired.Width, max.Width), Math.Max(desired.Height, max.Height));
                min = new PixelSize(Math.Min(desired.Width, min.Width), Math.Min(desired.Height, min.Height));
            }

            var hints = new XSizeHints
            {
                min_width = min.Width,
                min_height = min.Height
            };
            hints.height_inc = hints.width_inc = 1;
            var flags = XSizeHintsFlags.PMinSize | XSizeHintsFlags.PResizeInc;
            // People might be passing double.MaxValue
            if (max.Width < 100000 && max.Height < 100000)
            {
                hints.max_width = max.Width;
                hints.max_height = max.Height;
                flags |= XSizeHintsFlags.PMaxSize;
            }

            hints.flags = (IntPtr)flags;

            XSetWMNormalHints(x11info.Display, Handle, ref hints);
        }

        XWindowAttributes GetXWindowAttributes()
        {
            XWindowAttributes attributes = new XWindowAttributes();
            XGetWindowAttributes(x11info.Display, Handle, ref attributes);
            return attributes;
        }

        //bool _mapped;
        Bitmap bitmap;
        public override void OnEvent(ref XEvent e)
        {
            //Console.WriteLine(e.type);
            var ev = e;
            //if (ev.AnyEvent.type == XEventName.KeyPress ||
            //    ev.AnyEvent.type == XEventName.KeyRelease)
            //{
            //    // PreFilter() handles "shift key state updates.
            //    //Keyboard.PreFilter(xevent);
            //    if (XFilterEvent(ref ev, Handle))
            //    {
            //        // probably here we could raise WM_IME_KEYDOWN and
            //        // WM_IME_KEYUP, but I'm not sure it is worthy.
            //        return;
            //    }
            //}
            //else if (XFilterEvent(ref ev, IntPtr.Zero))
            //{ return; }
            if (ev.type == XEventName.MapNotify)
            {
                //_mapped = true;
                //if (_useRenderWindow)
                //    XMapWindow(_x11.Display, _renderHandle);
            }
            else if (ev.type == XEventName.UnmapNotify)
            {   //_mapped = false;
            }
            else
            if (ev.type == XEventName.Expose)
            {
                var rect = new Rect(ev.ExposeEvent.x, ev.ExposeEvent.y, ev.ExposeEvent.width <= 0 ? size.Width : ev.ExposeEvent.width, ev.ExposeEvent.height <= 0 ? size.Height : ev.ExposeEvent.height);
                if (!firstLayout)
                {
                    Paint(rect, size);
                }
                //if (!_triggeredExpose)
                //{
                //    _triggeredExpose = true;
                //    Dispatcher.UIThread.Post(() =>
                //    {
                //        _triggeredExpose = false;
                //        DoPaint();
                //    }, DispatcherPriority.Render);
                //}
            }
            else if (ev.type == XEventName.FocusIn)
            {
                activate = true;
                //if (ActivateTransientChildIfNeeded())
                //    return;
                Activated?.Invoke();
                //Console.WriteLine("Activated");
                //XSetICFocus(xic);
            }
            else if (ev.type == XEventName.FocusOut)
            {
                activate = false;
                //Console.WriteLine("Deactivated");
                Deactivated?.Invoke();
                XUnsetICFocus(xic);
                if (xic != IntPtr.Zero)
                {
                    //if (have_Xutf8ResetIC)
                    //{
                    //    try
                    //    {
                    //        Xutf8ResetIC(xic);
                    //    }
                    //    catch (EntryPointNotFoundException)
                    //    {
                    //        have_Xutf8ResetIC = false;
                    //    }
                    //}
                    //XUnsetICFocus(xic);
                }
            }
            else if (ev.type == XEventName.MotionNotify)
            {
                modifiers = TranslateModifiers(ev.MotionEvent.state);
                MouseEvent(EventType.MouseMove, new Point(ev.MotionEvent.x, ev.MotionEvent.y), modifiers, new Vector(), MouseButton.None);
            }
            else if (ev.type == XEventName.LeaveNotify)
            {
                modifiers = TranslateModifiers(ev.CrossingEvent.state);
                MouseEvent(EventType.MouseLeave, new Point(ev.MotionEvent.x, ev.MotionEvent.y), modifiers, new Vector(), MouseButton.None);
            }
            else if (ev.type == XEventName.PropertyNotify)
            {
                OnPropertyChange(ev.PropertyEvent.atom, ev.PropertyEvent.state == 0);
            }
            else
            if (ev.type == XEventName.ButtonPress)
            {
                //Console.WriteLine("ButtonPress");
                if (ActivateTransientChildIfNeeded())
                    return;
                if (ev.ButtonEvent.button < 4 || ev.ButtonEvent.button == 8 || ev.ButtonEvent.button == 9)
                {
                    MouseButton mouseButton = MouseButton.None;
                    switch (ev.ButtonEvent.button)
                    {
                        case 1:
                            mouseButton = MouseButton.Left;
                            break;
                        case 2:
                            mouseButton = MouseButton.Middle;
                            break;
                        case 3:
                            mouseButton = MouseButton.Right;
                            break;
                        case 8:
                            mouseButton = MouseButton.XButton1;
                            break;
                        case 9:
                            mouseButton = MouseButton.XButton2;
                            break;
                    }
                    modifiers = TranslateModifiers(ev.ButtonEvent.state);
                    MouseEvent(EventType.MouseDown, new Point(ev.ButtonEvent.x, ev.ButtonEvent.y), modifiers, new Vector(), mouseButton);

                }
                else
                {
                    modifiers = TranslateModifiers(ev.ButtonEvent.state);
                    //var delta = ev.ButtonEvent.button == 4
                    //    ? new Vector(0, 1)
                    //    : ev.ButtonEvent.button == 5
                    //        ? new Vector(0, -1)
                    //        : ev.ButtonEvent.button == 6
                    //            ? new Vector(1, 0)
                    //            : new Vector(-1, 0);
                    var delta = ev.ButtonEvent.button == 4
                        ? new Vector(0, 120)
                        : ev.ButtonEvent.button == 5
                            ? new Vector(0, -120)
                            : ev.ButtonEvent.button == 6
                                ? new Vector(120, 0)
                                : new Vector(-120, 0);
                    MouseEvent(EventType.MouseWheel, new Point(ev.MotionEvent.x, ev.MotionEvent.y), modifiers, delta, MouseButton.None);
                }

            }
            else if (ev.type == XEventName.ButtonRelease)
            {
                if (ActivateTransientChildIfNeeded())
                    return;
                //Console.WriteLine("ButtonRelease");
                if (ev.ButtonEvent.button < 4 || ev.ButtonEvent.button == 8 || ev.ButtonEvent.button == 9)
                {
                    MouseButton mouseButton = MouseButton.None;
                    switch (ev.ButtonEvent.button)
                    {
                        case 1:
                            mouseButton = MouseButton.Left;
                            break;
                        case 2:
                            mouseButton = MouseButton.Middle;
                            break;
                        case 3:
                            mouseButton = MouseButton.Right;
                            break;
                        case 8:
                            mouseButton = MouseButton.XButton1;
                            break;
                        case 9:
                            mouseButton = MouseButton.XButton2;
                            break;
                    }
                    modifiers = TranslateModifiers(ev.ButtonEvent.state);
                    MouseEvent(EventType.MouseUp, new Point(ev.ButtonEvent.x, ev.ButtonEvent.y), modifiers, new Vector(), mouseButton);
                }
            }
            else
            if (ev.type == XEventName.ConfigureNotify)
            {
                if (ev.ConfigureEvent.window != Handle)
                    return;
                ev.ConfigureEvent.override_redirect = true;
                //var needEnqueue = (_configure == null);
                //_configure = ev.ConfigureEvent;
                PixelPoint _configurePoint;
                if (ev.ConfigureEvent.override_redirect || ev.ConfigureEvent.send_event)
                    _configurePoint = new PixelPoint(ev.ConfigureEvent.x, ev.ConfigureEvent.y);
                else
                {
                    XTranslateCoordinates(x11info.Display, Handle, x11info.RootWindow,
                        0, 0,
                        out var tx, out var ty, out _);
                    _configurePoint = new PixelPoint(tx, ty);
                }
                //if (needEnqueue)
                //    Dispatcher.UIThread.Post(() =>
                //    {
                //if (_configure == null)
                //    return;
                var cev = ev.ConfigureEvent;
                var npos = _configurePoint;

                var nsize = new PixelSize(cev.width, cev.height);
                var changedSize = size != nsize;
                var changedPos = npos != position;
                size = nsize;
                position = npos;
                bool updatedSizeViaScaling = false;
                //if (changedPos)
                {
                    PositionChanged?.Invoke(npos);
                    //updatedSizeViaScaling = UpdateScaling();
                }

                if (changedSize && !updatedSizeViaScaling)
                {
                    Resized?.Invoke(ClientSize);
                    //Console.WriteLine(position + " , " + size);
                }

                if (changedSize && !firstLayout)
                {
                    Paint(new Rect(0, 0, size.Width, size.Height), nsize);
                }
                //Console.WriteLine("firstLayout");
                if (firstLayout)
                {
                    root.Invalidate();
                }
                firstLayout = false;
                //    Dispatcher.UIThread.RunJobs(DispatcherPriority.Layout);
                //}, DispatcherPriority.Layout);
                //if (_useRenderWindow)
                //    XConfigureResizeWindow(x11info.Info.Display, _renderHandle, ev.ConfigureEvent.width,
                //        ev.ConfigureEvent.height);
            }
            else if (ev.type == XEventName.ConfigureRequest)
            {
                var v = ev.ConfigureRequestEvent;
            }
            else if (ev.type == XEventName.DestroyNotify && ev.AnyEvent.window == Handle)
            {
                //Cleanup();
                Close();
            }
            else if (ev.type == XEventName.ClientMessage)
            {
                //Console.WriteLine(ev.ClientMessageEvent.message_type + " " + GetAtomName(x11info.Display, ev.ClientMessageEvent.ptr1) + ev.ClientMessageEvent.ptr1 + " " + ev.ClientMessageEvent.ptr2);
                if (ev.ClientMessageEvent.message_type == x11info.Atoms.WM_PROTOCOLS)
                {
                    if (ev.ClientMessageEvent.ptr1 == x11info.Atoms.WM_DELETE_WINDOW)
                    {
                        Close();
                    }

                }
                //else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndSelection)
                //{
                //    Console.WriteLine("XdndSelection");
                //}
                //else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndPosition)
                //{
                //    var pos_x = (int)ev.ClientMessageEvent.ptr3 >> 16;
                //    var pos_y = (int)ev.ClientMessageEvent.ptr3 & 0xFFFF;
                //    Console.WriteLine(pos_x + "," + pos_y);
                //}
                else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndEnter)
                {
                    //Console.WriteLine("dragenter");
                    var mp = MouseDevice.Location;
                    dataObject.AllowedEffects = dataObject.EffectFromAction(ev.ClientMessageEvent.ptr5);
                    dataObject.SetDragDropEffects(root.InputManager.DragDropDevice.DragEnter(new DragEventArgs(dataObject, new Point(mp.X - position.X, mp.Y - position.Y), root) { DragEffects = dataObject.AllowedEffects }, root.LayoutManager.VisibleUIElements));
                    //查询格式，判断是否可以接受数据
                    dataObject.DragEnter(ref ev, (s, ee) =>
                    {
                        var mmp = MouseDevice.Location;
                        dataObject.SetDragDropEffects(root.InputManager.DragDropDevice.DragOver(new DragEventArgs(dataObject, new Point(mmp.X - position.X, mmp.Y - position.Y), root) { DragEffects = dataObject.AllowedEffects }, root.LayoutManager.VisibleUIElements));
                    });

                }
                else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndDrop)
                {
                    var mp = MouseDevice.Location;
                    root.InputManager.DragDropDevice.Drop(new DragEventArgs(dataObject, new Point(mp.X - position.X, mp.Y - position.Y), root) { DragEffects = dataObject.DropEffects }, root.LayoutManager.VisibleUIElements);
                    dataObject.StopTimer();
                    dataObject.SendFinished();
                }
                else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndLeave)
                {
                    root.InputManager.DragDropDevice.DragLeave(root.LayoutManager.VisibleUIElements);
                    dataObject.StopTimer();
                }
                //else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndStatus)
                //{
                //    Console.WriteLine("XdndStatus");
                //}
                //else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndFinished)
                //{
                //    Console.WriteLine("XdndFinished");
                //}
                //else if (ev.ClientMessageEvent.message_type == x11info.Atoms.XdndAware)
                //{
                //    Console.WriteLine("XdndAware");
                //}
            }
            //else if (ev.type == XEventName.SelectionNotify)
            //{
            //    Console.WriteLine("SelectionNotify:" + dataObject.GetText(ref ev));
            //}
            else if (ev.type == XEventName.KeyPress || ev.type == XEventName.KeyRelease)
            {
                if (ActivateTransientChildIfNeeded())
                    return;

                var index = ev.KeyEvent.state.HasFlag(XModifierMask.ShiftMask);

                // We need the latin key, since it's mainly used for hotkeys, we use a different API for text anyway
                var key = (X11Key)XKeycodeToKeysym(x11info.Display, ev.KeyEvent.keycode, index ? 1 : 0).ToInt32();

                // Manually switch the Shift index for the keypad,
                // there should be a proper way to do this
                if (ev.KeyEvent.state.HasFlag(XModifierMask.Mod2Mask)
                    && key > X11Key.Num_Lock && key <= X11Key.KP_9)
                { key = (X11Key)XKeycodeToKeysym(x11info.Display, ev.KeyEvent.keycode, index ? 0 : 1).ToInt32(); }


                modifiers = TranslateModifiers(ev.KeyEvent.state);
                root.InputManager.KeyboardDevice.Modifiers = modifiers;

                if (ev.type == XEventName.KeyPress)
                {
                    root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, X11KeyConvert.ConvertKey(key), ev.KeyEvent.keycode, modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyDown);
                    var len = LookupString(ref ev, 24, out var xKeySym, out var status);
                    //Console.WriteLine(lookup_buffer.ToString());

                    //var buffer = stackalloc byte[1024];
                    //var len = Xutf8LookupString(_xic, ref ev, buffer, 1024, out var x11Key, out var status);
                    //Console.WriteLine(len);
                    //var len = XwcLookupString(_xic, ref ev, buffer, 1024, out _, out var status);
                    //var str = new StringBuilder(40);
                    //X11Key x11Key= X11Key.a;
                    //LookupStatus status = LookupStatus.XBufferOverflow;
                    //var len = XmbLookupString(_xic, ref ev.KeyEvent, buffer, 1024, out var x11Key, out var status);
                    //Console.WriteLine(status);
                    if (status == XLookupStatus.XLookupChars || status == XLookupStatus.XLookupBoth)
                    {
                        var text = lookup_buffer.ToString();
                        //var text = Encoding.UTF8.GetString(buffer, len);
                        if ((text.Length == 1 && !(text[0] < ' ' || text[0] == 0x7f)) || text.Length > 1)
                        {
                            root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(root, root.InputManager.KeyboardDevice, text), KeyEventType.TextInput);
                        }
                    }
                }
                else
                {
                    root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(root, X11KeyConvert.ConvertKey(key), ev.KeyEvent.keycode, modifiers, root.InputManager.KeyboardDevice), KeyEventType.KeyUp);
                }

                //x11info.LastActivityTimestamp = ev.ButtonEvent.time;
            }


            base.OnEvent(ref e);
        }

        DataObject dataObject = new DataObject();

        private bool have_Xutf8LookupString = true;

        private byte[] lookup_byte_buffer = new byte[100];
        private StringBuilder lookup_buffer = new StringBuilder(24);
        private int LookupString(ref XEvent xevent, int len, out XKeySym keysym, out XLookupStatus status)
        {
            IntPtr keysym_res;
            int res;

            status = XLookupStatus.XLookupNone;
            if (xic != IntPtr.Zero && have_Xutf8LookupString && xevent.type == XEventName.KeyPress)
            {
                do
                {
                    try
                    {
                        res = Xutf8LookupString(xic, ref xevent, lookup_byte_buffer, lookup_byte_buffer.Length, out keysym_res, out status);
                    }
                    catch (EntryPointNotFoundException)
                    {
                        have_Xutf8LookupString = false;

                        // call again, this time we'll go through the non-xic clause
                        return LookupString(ref xevent, len, out keysym, out status);
                    }
                    if (status != XLookupStatus.XBufferOverflow)
                        break;
                    lookup_byte_buffer = new byte[lookup_byte_buffer.Length << 1];
                } while (true);
                lookup_buffer.Length = 0;
                string s = Encoding.UTF8.GetString(lookup_byte_buffer, 0, res);
                lookup_buffer.Append(s);
                keysym = (XKeySym)keysym_res.ToInt32();
                return s.Length;
            }
            else
            {
                IntPtr statusPtr = IntPtr.Zero;
                res = XLookupString(ref xevent, lookup_byte_buffer, len, out keysym_res, out statusPtr);
                lookup_buffer.Length = 0;
                string s = Encoding.ASCII.GetString(lookup_byte_buffer, 0, res);
                lookup_buffer.Append(s);
                keysym = (XKeySym)keysym_res.ToInt32();
                return res;
            }
        }


        private void Paint(Rect rect, PixelSize size)
        {
            root.LayoutManager.ExecuteLayoutPass();
            if (context != null)
            {
                context.MakeCurrent();
                if (root.LayoutManager.VisibleUIElements != null)
                {
                    context.GetFramebufferInfo(out var fb, out var sam, out var sten);
                    var rt = new OpenGlRenderTarget(context, size.Width, (int)size.Height, fb, sam, sten);
                    using (DrawingContext dc = DrawingContext.FromRenderTarget(rt))
                    {
                        root.RenderView(dc, rect);
                    }
                }
                context.SwapBuffers();
            }
            else
            {
                if (bitmap == null || bitmap.Width != size.Width || bitmap.Height != size.Height)
                {
                    if (bitmap != null)
                    {
                        bitmap.Dispose();
                    }
                    bitmap = new Bitmap(size.Width, size.Height);
                }
                if (root != null && root.LayoutManager.VisibleUIElements != null)
                {
                    using (DrawingContext dc = DrawingContext.FromBitmap(bitmap))
                    {
                        root.RenderView(dc, rect);
                    }
                }

                using (var l = bitmap.Lock())
                {
                    var gc = XCreateGC(x11info.Display, Handle, 0, IntPtr.Zero);
                    //XLockDisplay(x11info.Display);
                    var img = new XImage();
                    int bitsPerPixel = 32;
                    img.width = bitmap.Width;
                    img.height = bitmap.Height;
                    img.format = 2; //ZPixmap;
                    img.data = l.DataPointer;
                    img.byte_order = 0;// LSBFirst;
                    img.bitmap_unit = bitsPerPixel;
                    img.bitmap_bit_order = 0;// LSBFirst;
                    img.bitmap_pad = bitsPerPixel;
                    img.depth = 32;
                    img.bytes_per_line = bitmap.Width * 4;
                    img.bits_per_pixel = bitsPerPixel;
                    XInitImage(ref img);

                    XPutImage(x11info.Display, Handle, gc, ref img, 0, 0, 0, 0, (uint)size.Width, (uint)size.Height);
                    //XSync(x11info.Display, false);
                    //XUnlockDisplay(x11info.Display);
                    XFreeGC(x11info.Display, gc);
                    //XFlush(x11info.Display);
                    //Console.WriteLine("Expose");
                }
            }
        }

        WindowState _lastWindowState = WindowState.Normal;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="atom"></param>
        /// <param name="hasValue"></param>
        /// <param name="windowState">部分Linux里会丢失状态消息</param>
        private void OnPropertyChange(IntPtr atom, bool hasValue, WindowState? windowState = null)
        {
            //Console.WriteLine(GetAtomName(x11info.Display, atom));
            if (atom == x11info.Atoms._NET_WM_STATE)
            {
                WindowState state = WindowState.Normal;
                if (hasValue)
                {
                    if (windowState.HasValue)
                    {
                        state = windowState.Value;
                    }
                    else
                    {
                        XGetWindowProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_STATE, IntPtr.Zero, new IntPtr(256),
                            false, (IntPtr)Atom.XA_ATOM, out _, out _, out var nitems, out _,
                            out var prop);
                        int maximized = 0;
                        var pitems = (IntPtr*)prop.ToPointer();
                        if (nitems.ToInt32() == 0)
                        {
                            return;
                        }
                        for (var c = 0; c < nitems.ToInt32(); c++)
                        {
                            if (pitems[c] == x11info.Atoms._NET_WM_STATE_HIDDEN)
                            {
                                state = WindowState.Minimized;
                                break;
                            }
                            if (pitems[c] == x11info.Atoms._NET_WM_STATE_FULLSCREEN)
                            {
                                state = WindowState.FullScreen;
                                break;
                            }
                            if (pitems[c] == x11info.Atoms._NET_WM_STATE_MAXIMIZED_HORZ ||
                                pitems[c] == x11info.Atoms._NET_WM_STATE_MAXIMIZED_VERT)
                            {
                                maximized++;
                                if (maximized == 2)
                                {
                                    state = WindowState.Maximized;
                                    break;
                                }
                            }
                        }
                        XFree(prop);
                    }
                }
                if (_lastWindowState != state)
                {
                    var old = _lastWindowState;
                    _lastWindowState = state;
                    WindowStateChanged?.Invoke();
                    if (state == WindowState.Maximized)
                    {
                        var sc = Screen;
                        lastRect = new Rect(position.X, position.Y, size.Width, size.Height);
                        //Console.WriteLine(sc.WorkingArea);
                        if (sc.Primary)
                        {
                            Position = new PixelPoint((int)sc.WorkingArea.Left, (int)sc.WorkingArea.Top);
                            //root.MarginLeft = sc.WorkingArea.Left / LayoutScaling;
                            //root.MarginTop = sc.WorkingArea.Top / LayoutScaling;
                            PositionChanged(position);
                            Resize(sc.WorkingArea.Size / RenderScaling, true);
                        }
                        else
                        {
                            //Console.WriteLine(sc.Bounds);
                            //Position = new PixelPoint((int)sc.Bounds.Left, (int)sc.Bounds.Top);
                            //PositionChanged(position);
                            //Resize(sc.Bounds.Size, true);
                            root.BeginInvoke(() =>
                            {
                                (root as Window).WindowState = WindowState.FullScreen;
                            });
                        }

                        root.Delay(TimeSpan.FromMilliseconds(100), () =>
                        {
                            root.Invalidate();//Centos里开启GPU后最大化有时候会丢失图像，这里强制刷新
                        });
                    }
                    else if (state == WindowState.Normal && old == WindowState.Maximized)
                    {
                        Resize(lastRect.Size / RenderScaling, true);
                        root.BeginInvoke(() =>
                        {
                            if (!(root is Window window) || !window.IsDragMove)
                            {
                                Position = new PixelPoint((int)lastRect.X, (int)lastRect.Y);
                            }

                            root.Delay(TimeSpan.FromMilliseconds(100), () =>
                            {
                                root.Invalidate();//Centos里开启GPU后最大化还原有时候会丢失图像，这里强制刷新
                            });
                        });
                    }

                    //Console.WriteLine(lastRect + " " + state + " " + old);
                }
                //else if (fullscreen && _lastWindowState != state && state != WindowState.Normal)
                //{
                //    _lastWindowState = state;
                //    WindowStateChanged?.Invoke();
                //}
            }

        }
        Rect lastRect;

        InputModifiers modifiers;
        public void MouseEvent(EventType type, Point pos, InputModifiers modifiers, Vector delta, MouseButton mouseButton, bool isTouch = false)
        {
            this.modifiers = modifiers;
            if (type == EventType.MouseMove && root is Window window && window.IsDragMove)
            {
                if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen)
                {
                    var me = pos / RenderScaling;
                    var left = me.X;
                    var t = me.Y;
                    var w = window.ActualSize.Width;
                    var percent = left / w;
                    var ml = MouseDevice.Location;
                    WindowState = WindowState.Normal;
                    root.BeginInvoke(() =>
                    {
                        var atr = GetXWindowAttributes();
                        var scr = window.Screen;
                        var tt = (ml.Y - scr.WorkingArea.Y) / window.RenderScaling - t;
                        window.MarginTop = tt;
                        window.MarginLeft = left - atr.width / window.RenderScaling * percent;
                        //Console.WriteLine(left + "-" + window.ActualSize.Width + "-" + percent + "--" + tt + "," + window.MarginLeft);
                        DragMove();
                    });
                }
                else
                {
                    DragMove();
                }

                return;
            }
            if (type == EventType.MouseDown)
            {
                mouseDownWindow = Handle;
            }
            //modifiers = TranslateModifiers(mods);
            root.InputManager.KeyboardDevice.Modifiers = modifiers;
            if (type == EventType.MouseLeave || type == EventType.MouseMove)
            {
                root.InputManager.MouseDevice.ProcessEvent(new MouseEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), pos / RenderScaling, root.InputManager.MouseDevice, isTouch), root.LayoutManager.VisibleUIElements, type);
                //Console.WriteLine("mousemove：" + ev.MotionEvent.x + "," + ev.MotionEvent.y + "===" + new Point(ev.MotionEvent.x, ev.MotionEvent.y) / LayoutScaling);
            }
            else if (type == EventType.MouseWheel)
            {
                root.InputManager.MouseDevice.ProcessEvent(new MouseWheelEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), pos / RenderScaling, root.InputManager.MouseDevice, delta), root.LayoutManager.VisibleUIElements, type);
            }
            else if (type == EventType.MouseDown || type == EventType.MouseUp)
            {
                if (type == EventType.MouseDown)
                {
                    switch (mouseButton)
                    {
                        case MouseButton.Left:
                            modifiers |= InputModifiers.LeftMouseButton;
                            break;
                        case MouseButton.Middle:
                            modifiers |= InputModifiers.MiddleMouseButton;
                            break;
                        case MouseButton.Right:
                            modifiers |= InputModifiers.RightMouseButton;
                            break;
                    }
                }
                else
                {
                    switch (mouseButton)
                    {
                        case MouseButton.Left:
                            modifiers ^= InputModifiers.LeftMouseButton;
                            break;
                        case MouseButton.Middle:
                            modifiers ^= InputModifiers.MiddleMouseButton;
                            break;
                        case MouseButton.Right:
                            modifiers ^= InputModifiers.RightMouseButton;
                            break;
                    }
                }
                root.InputManager.KeyboardDevice.Modifiers = modifiers;
                root.InputManager.MouseDevice.ProcessEvent(new MouseButtonEventArgs(root, modifiers.HasFlag(InputModifiers.LeftMouseButton), modifiers.HasFlag(InputModifiers.RightMouseButton), modifiers.HasFlag(InputModifiers.MiddleMouseButton), pos / RenderScaling, root.InputManager.MouseDevice, mouseButton, isTouch), root.LayoutManager.VisibleUIElements, type);
            }
            if (type == EventType.MouseUp)
            {
                if (xic != IntPtr.Zero && !(root.InputManager.KeyboardDevice.FocusedElement is IEditor))
                {
                    XUnsetICFocus(xic);
                }
                else if (xic != IntPtr.Zero && root.InputManager.KeyboardDevice.FocusedElement is IEditor editor && editor.IsInputMethodEnabled && CanActivate)
                {
                    XSetICFocus(xic);
                }
                mouseDownWindow = IntPtr.Zero;
            }
            //x11info.LastActivityTimestamp = ev.ButtonEvent.time;
        }

        private void DragMove()
        {
            root.BeginInvoke(() =>
            {
                lock (XlibLock)
                {
                    XEvent xEvent = new XEvent();
                    var p = MouseDevice.Location;
                    var display = x11info.Display;
                    xEvent.ClientMessageEvent.type = XEventName.ClientMessage;
                    xEvent.ClientMessageEvent.message_type = x11info.Atoms._NET_WM_MOVERESIZE;
                    xEvent.ClientMessageEvent.display = display;
                    xEvent.ClientMessageEvent.window = Handle;
                    xEvent.ClientMessageEvent.format = 32;
                    xEvent.ClientMessageEvent.ptr1 = (IntPtr)p.X;
                    xEvent.ClientMessageEvent.ptr2 = (IntPtr)p.Y;
                    xEvent.ClientMessageEvent.ptr3 = (IntPtr)8;
                    xEvent.ClientMessageEvent.ptr4 = (IntPtr)1;
                    XUngrabPointer(display, IntPtr.Zero);
                    XSendEvent(display, x11info.RootWindow, false, (IntPtr)(EventMask.SubstructureNotifyMask | EventMask.SubstructureRedirectMask), ref xEvent);
                    XFlush(display);


                    //p = MouseDevice.Location;
                    XEvent xevent = new XEvent();
                    xevent.type = XEventName.ButtonRelease;
                    xevent.ButtonEvent.button = 1;
                    xevent.ButtonEvent.window = Handle;
                    xevent.ButtonEvent.x = p.X - position.X;
                    xevent.ButtonEvent.y = p.Y - position.Y;
                    xevent.ButtonEvent.x_root = p.X;
                    xevent.ButtonEvent.y_root = p.X;
                    xevent.ButtonEvent.display = display;
                    xevent.ButtonEvent.state = XModifierMask.Button1Mask;

                    XSendEvent(display, Handle, false, (IntPtr)EventMask.ButtonReleaseMask, ref xevent);
                    XFlush(display);
                }
            });
        }

        InputModifiers TranslateModifiers(XModifierMask state)
        {
            var rv = default(InputModifiers);
            if (state.HasFlag(XModifierMask.Button1Mask))
                rv |= InputModifiers.LeftMouseButton;
            if (state.HasFlag(XModifierMask.Button2Mask))
                rv |= InputModifiers.MiddleMouseButton;
            if (state.HasFlag(XModifierMask.Button3Mask))
                rv |= InputModifiers.RightMouseButton;
            //if (state.HasFlag(XModifierMask.Button4Mask))
            //    rv |= InputModifiers.XButton1MouseButton;
            //if (state.HasFlag(XModifierMask.Button5Mask))
            //    rv |= InputModifiers.XButton2MouseButton;
            if (state.HasFlag(XModifierMask.ShiftMask))
                rv |= InputModifiers.Shift;
            if (state.HasFlag(XModifierMask.ControlMask))
                rv |= InputModifiers.Control;
            if (state.HasFlag(XModifierMask.Mod1Mask))
                rv |= InputModifiers.Alt;
            //if (state.HasFlag(XModifierMask.Mod4Mask))
            //    rv |= InputModifiers.Meta;
            return rv;
        }

        void Resize(Size clientSize, bool force)
        {
            if (!force && clientSize == ClientSize)
                return;

            var needImmediatePopupResize = clientSize != ClientSize;

            var pixelSize = ToPixelSize(clientSize);
            //Paint(new Rect(new Point(), new Size(pixelSize.Width, pixelSize.Height)), pixelSize);
            //UpdateSizeHints(pixelSize);
            XConfigureResizeWindow(x11info.Display, Handle, pixelSize);
            XFlush(x11info.Display);

            if (force)//|| (_popup && needImmediatePopupResize))
            {
                size = pixelSize;
                Resized?.Invoke(ClientSize);
            }
        }
        PixelSize ToPixelSize(Size size) => new PixelSize((int)Math.Ceiling(size.Width * RenderScaling), (int)Math.Ceiling(size.Height * RenderScaling));
        public void UpdateScaling()
        {
            //lock (XlibLock)
            {
                //float newScaling;
                //var pos = new Point(position.X, position.Y);
                //var monitor = ScreenImpl.Screens.OrderBy(x => x.PixelDensity)
                //    .FirstOrDefault(m => m.Bounds.Contains(pos));

                //if (RenderScaling != newScaling * Application.BaseScale)
                {
                    //var oldScaledSize = ClientSize;
                    //RenderScaling = newScaling * Application.BaseScale;
                    ScalingChanged?.Invoke();
                    //SetMinMaxSize(_scaledMinMaxSize.minSize, _scaledMinMaxSize.maxSize);
                    Resized(ClientSize);
                    //return true;
                }

                //return false;
            }
        }

        public Func<bool> Closing { get; set; }
        public Action Closed { get; set; }
        public WindowState WindowState
        {
            get { return _lastWindowState; }
            set
            {
                if (_lastWindowState == value)
                    return;
                if (value == WindowState.Minimized)
                {
                    XIconifyWindow(x11info.Display, Handle, x11info.DefaultScreen);
                }
                else if (value == WindowState.Maximized)
                {
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_HIDDEN);
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_FULLSCREEN);
                    ChangeWMAtoms(true, x11info.Atoms._NET_WM_STATE_MAXIMIZED_VERT,
                        x11info.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
                    lastRect = new Rect(position.X, position.Y, size.Width, size.Height);
                }
                else if (value == WindowState.FullScreen)
                {
                    //lastRect = new Rect(position.X, position.Y, size.Width, size.Height);
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_HIDDEN);
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_MAXIMIZED_VERT,
                        x11info.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
                    ChangeWMAtoms(true, x11info.Atoms._NET_WM_STATE_FULLSCREEN);
                }
                else
                {
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_HIDDEN);
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_FULLSCREEN);
                    ChangeWMAtoms(false, x11info.Atoms._NET_WM_STATE_MAXIMIZED_VERT,
                        x11info.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
                    SendNetWMMessage(x11info.Atoms._NET_ACTIVE_WINDOW, (IntPtr)1);
                }
                //if (fullscreen && value == WindowState.Normal)
                //{
                //    SetFullscreen(false);
                //}
                //else if (fullscreen && value == WindowState.Maximized)
                //{

                //}
                XFlush(x11info.Display);
                OnPropertyChange(x11info.Atoms._NET_WM_STATE, true, value);
                //_lastWindowState = value;
            }
        }
        public Action WindowStateChanged { get; set; }

        public Screen Screen
        {
            get
            {
                //XWindowAttributes attributes = new XWindowAttributes();
                //XGetWindowAttributes(x11info.Display, Handle, ref attributes);
                //var sc = Marshal.PtrToStructure<XScreen>(attributes.screen);
                var pos = position;
                var rect = new Rect(pos.X, pos.Y, size.Width, size.Height);
                var screen = Screen.AllScreens.Select(a =>
                {
                    if (rect.IntersectsWith(a.WorkingArea) || rect == a.WorkingArea || a.WorkingArea.Contains(rect) || rect.Contains(a.WorkingArea))
                    {
                        var r = rect;
                        r.Intersect(a.WorkingArea);
                        return new { sc = a, v = r.Width * r.Height };
                    }
                    else
                    {
                        return new { sc = a, v = 0f };
                    }
                }).OrderByDescending(a => a.v).Select(a => a.sc).FirstOrDefault();
                if (screen == null)
                {
                    screen = Screen.AllScreens[0];
                }
                return screen;
            }
        }
        bool ismain;
        public bool IsMain
        {
            get
            {
                return ismain;
            }

            set
            {
                ismain = value;
                if (value)
                {
                    main = this;
                }
            }
        }
        public Size ClientSize => new Size(size.Width / RenderScaling, size.Height / RenderScaling);

        public float RenderScaling { get { return Application.BaseScale * ScreenImpl.DpiScale; } }

        public float LayoutScaling { get { return RenderScaling; } }

        public Action ScalingChanged { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Activated { get; set; }
        public Action Deactivated { get; set; }
        public bool CanActivate { get; set; } = true;
        public PixelPoint Position
        {
            get { return position; }
            set
            {
                //if (!(this is PopWindow))
                //{
                //    Console.WriteLine(value);
                //}
                //if (this is PopWindow)
                //{
                position = value;
                XMoveWindow(x11info.Display, Handle, value.X, value.Y);
                return;
                //}
                //
                //XMoveResizeWindow(x11info.Display, Handle, value.X, value.Y,size.Width,size.Height);

            }
        }

        bool activate;
        public void Activate()
        {
            if (activate)
            {
                return;
            }
            //if (x11info.Atoms._NET_ACTIVE_WINDOW != IntPtr.Zero)
            //{//这个鬼东西会有个系统消息提示
            //    SendNetWMMessage(x11info.Atoms._NET_ACTIVE_WINDOW, (IntPtr)1, x11info.LastActivityTimestamp,
            //        IntPtr.Zero);
            //}
            //else
            {
                XRaiseWindow(x11info.Display, Handle);
                XSetInputFocus(x11info.Display, Handle, 0, IntPtr.Zero);
            }
        }
        void SendNetWMMessage(IntPtr message_type, IntPtr l1,
         IntPtr? l2 = null, IntPtr? l3 = null, IntPtr? l4 = null, IntPtr? l5 = null)
        {
            var xev = new XEvent
            {
                ClientMessageEvent =
                {
                    type = XEventName.ClientMessage,
                    send_event = true,
                    window = Handle,
                    message_type = message_type,
                    format = 32,
                    //ptr1 = l1,
                    //ptr2 = l2 ?? IntPtr.Zero,
                    //ptr3 = l3 ?? IntPtr.Zero,
                    //ptr4 = l4 ?? IntPtr.Zero,
                    //ptr5 = l5 ?? IntPtr.Zero
                }
            };
            xev.ClientMessageEvent.ptr1 = l1;
            xev.ClientMessageEvent.ptr2 = l2 ?? IntPtr.Zero;
            xev.ClientMessageEvent.ptr3 = l3 ?? IntPtr.Zero;
            xev.ClientMessageEvent.ptr4 = l4 ?? IntPtr.Zero;
            xev.ClientMessageEvent.ptr5 = l5 ?? IntPtr.Zero;
            lock (XlibLock)
            {
                XSendEvent(x11info.Display, x11info.RootWindow, false,
                new IntPtr((int)(EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask)), ref xev);
            }
            XFlush(x11info.Display);
        }

        public void Capture()
        {
            //throw new NotImplementedException();
        }

        public void Close()
        {
            if (Closing.Invoke() != true)
            {
                Closed.Invoke();
                Dispose();
            }
        }


        bool paint = false;
        Rect invalidateRect;
        public void Invalidate(in Rect rect)
        {
            Rect all = new Rect(new Point(), root.ActualSize);//控件区域
            if ((all.Contains(rect) || rect.IntersectsWith(all) || all == rect || rect.Contains(all)) && !rect.IsEmpty && !all.IsEmpty)
            {
                //Debug.WriteLine(invalidateRect);
                if (invalidateRect.IsEmpty || rect.Contains(invalidateRect) || rect == invalidateRect)//如果更新区域大于原有失效区域，则当前失效区域设为更新区域
                {
                    invalidateRect = rect;
                }
                else if (invalidateRect.Contains(rect))//如果更新区域小于原来的失效区域，则失效区域不变
                {

                }
                else if (invalidateRect.IsEmpty)//如果原来的失效区域为空
                {
                    invalidateRect = rect;
                }
                else
                {//如果两个区域没有关联或者相交
                    var minX = invalidateRect.X < rect.X ? invalidateRect.X : rect.X;//确定包含这两个矩形的最小矩形
                    var minY = invalidateRect.Y < rect.Y ? invalidateRect.Y : rect.Y;
                    var maxW = (invalidateRect.Width + invalidateRect.X - minX) > (rect.Width + rect.X - minX) ? (invalidateRect.Width + invalidateRect.X - minX) : (rect.Width + rect.X - minX);
                    var maxH = (invalidateRect.Height + invalidateRect.Y - minY) > (rect.Height + rect.Y - minY) ? (invalidateRect.Height + invalidateRect.Y - minY) : (rect.Height + rect.Y - minY);
                    Rect min = new Rect(minX, minY, maxW, maxH);

                    invalidateRect = min;
                }
                invalidateRect.Intersect(all);//最后失效区域为在控件区域里面的相交区域
            }
            if (!paint)
            {
                paint = true;
                Threading.Dispatcher.MainThread.BeginInvoke(() =>
               {
                   root.LayoutManager.ExecuteLayoutPass();
                   //OnPaint(IntPtr.Zero, new Rect(((invalidateRect.X - 1) * scaling), ((invalidateRect.Y - 1) * scaling), ((invalidateRect.Width + 2) * scaling), ((invalidateRect.Height + 2) * scaling)));
                   var r = new Rect(((invalidateRect.X - 1) * RenderScaling), ((invalidateRect.Y - 1) * RenderScaling), ((invalidateRect.Width + 2) * RenderScaling), ((invalidateRect.Height + 2) * RenderScaling));
                   r.X = Math.Max(0, r.X);
                   r.Y = Math.Max(0, r.Y);
                   var xev = new XEvent
                   {
                       ExposeEvent =
                       {
                        type = XEventName.Expose,
                        send_event = true,
                        window = Handle,
                        count=1,
                        display=x11info.Display,
                        height=(int)r.Height+1,
                        width= (int)r.Width+1,
                        x=(int)r.X,
                        y= (int)r.Y
                       }
                   };
                   //Console.WriteLine("Invalidate:" + r);
                   lock (XlibLock)
                   {
                       XSendEvent(x11info.Display, Handle, false,
                           new IntPtr((int)(EventMask.ExposureMask)), ref xev);
                   }
                   invalidateRect = new Rect();
                   paint = false;
                   XFlush(x11info.Display);
                   //Console.WriteLine("Invalidate");
               });
            }

            //RECT r = new RECT((int)((rect.X - 1) * scaling), (int)((rect.Y - 1) * scaling), (int)((rect.Right + 2) * scaling), (int)((rect.Bottom + 2) * scaling));
            //InvalidateRect(handle, ref r, false);
        }
        public Point PointToClient(Point point)
        {
            return new Point((point.X - Position.X) / RenderScaling, (point.Y - Position.Y) / RenderScaling);
        }

        public Point PointToScreen(Point point)
        {
            return new Point(
            (int)(point.X * RenderScaling + Position.X),
            (int)(point.Y * RenderScaling + Position.Y));
        }

        public void ReleaseCapture()
        {
            //throw new NotImplementedException();
        }

        public void SetCursor(Cursor cursor)
        {
            XDefineCursor(x11info.Display, Handle, (IntPtr)cursor.PlatformCursor);
        }

        //bool fullscreen;
        //public void SetFullscreen(bool fullscreen)
        //{
        //    this.fullscreen = fullscreen;
        //    //XSetWindowAttributes attr = new XSetWindowAttributes();
        //    //var valueMask = SetWindowValuemask.OverrideRedirect;
        //    //attr.override_redirect = fullscreen;
        //    //XChangeWindowAttributes(x11info.Display, Handle, (ulong)valueMask, ref attr);
        //    //XMapWindow(x11info.Display, Handle);
        //    //XFlush(x11info.Display);
        //    ChangeWMAtoms(fullscreen, x11info.Atoms._NET_WM_STATE_FULLSCREEN);
        //    if (fullscreen)
        //    {
        //        lastRect = new Rect(position.X, position.Y, size.Width, size.Height);
        //        //var sc = Screen;
        //        //Resize(sc.Bounds.Size, true);
        //        //Position = new PixelPoint();
        //        _lastWindowState = WindowState.Maximized;
        //        WindowStateChanged?.Invoke();
        //    }
        //    else
        //    {
        //        if (_lastWindowState == WindowState.Maximized)
        //        {
        //            _lastWindowState = WindowState.Normal;
        //            WindowStateChanged?.Invoke();
        //        }
        //    }
        //}
        public void SetIcon(Image image)
        {
            if (image == null)
            {
                XDeleteProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_ICON);
                return;
            }
            var _width = Math.Min(image.Width, 128);
            var _height = Math.Min(image.Height, 128);
            var _bdata = new uint[_width * _height];
            fixed (void* ptr = _bdata)
            {
                var iptr = (int*)ptr;
                iptr[0] = _width;
                iptr[1] = _height;
            }

            var h = GCHandle.Alloc(_bdata, GCHandleType.Pinned);
            using (Bitmap bmp = new Bitmap(_width, _height, _width * 4, PixelFormat.Bgra, h.AddrOfPinnedObject()))
            {
                using (var dc = DrawingContext.FromBitmap(bmp))
                {
                    dc.DrawImage(image, new Rect(0, 0, _width, _height), new Rect(0, 0, image.Width, image.Height));
                }
            }
            h.Free();


            var data = new UIntPtr[_width * _height + 2];
            data[0] = new UIntPtr((uint)_width);
            data[1] = new UIntPtr((uint)_height);
            for (var y = 0; y < _height; y++)
            {
                var r = y * _width;
                for (var x = 0; x < _width; x++)
                    data[r + x] = new UIntPtr(_bdata[r + x]);
            }

            fixed (void* pdata = data)
                XChangeProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_ICON,
                    new IntPtr((int)Atom.XA_CARDINAL), 32, PropertyMode.Replace,
                    pdata, data.Length);
        }

        public void SetIMEEnable(bool enable)
        {
            //throw new NotImplementedException();
            if (xic != IntPtr.Zero)
            {
                //if (!enable)
                //{
                //    XUnsetICFocus(xic);
                //}
                //else
                //{
                XSetICFocus(xic);
                //}
            }
        }

        public void SetIMEPosition(Point point)
        {
            XPoint spot = new XPoint();
            spot.X = (short)point.X;
            spot.Y = (short)(point.Y + 20);

            IntPtr pSL = IntPtr.Zero;
            try
            {
                pSL = Marshal.StringToHGlobalAnsi(XNames.XNSpotLocation);
                IntPtr preedit = XVaCreateNestedList(0, pSL, spot, IntPtr.Zero);
                XSetICValues(xic, XNames.XNPreeditAttributes, preedit, IntPtr.Zero);
            }
            finally
            {
                if (pSL != IntPtr.Zero)
                    Marshal.FreeHGlobal(pSL);
            }
            //throw new NotImplementedException();
        }

        public void SetRoot(View view)
        {
            root = view;
            root.LayoutUpdated += Root_LayoutUpdated;
            //root.PropertyChanged += Root_PropertyChanged;
        }

        //private void Root_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(Window.IsDragMove))
        //    {
        //        lock (XlibLock)
        //        {
        //            XEvent xEvent = new XEvent();
        //            var p = MouseDevice.Location;
        //            var display = x11info.Display;
        //            xEvent.ClientMessageEvent.type = XEventName.ClientMessage;
        //            xEvent.ClientMessageEvent.message_type = x11info.Atoms._NET_WM_MOVERESIZE;
        //            xEvent.ClientMessageEvent.display = display;
        //            xEvent.ClientMessageEvent.window = Handle;
        //            xEvent.ClientMessageEvent.format = 32;
        //            xEvent.ClientMessageEvent.ptr1 = (IntPtr)p.X;
        //            xEvent.ClientMessageEvent.ptr2 = (IntPtr)p.Y;
        //            xEvent.ClientMessageEvent.ptr3 = (IntPtr)8;
        //            xEvent.ClientMessageEvent.ptr4 = (IntPtr)1;
        //            XUngrabPointer(display, IntPtr.Zero);
        //            XSendEvent(display, x11info.RootWindow, false, (IntPtr)(EventMask.SubstructureNotifyMask | EventMask.SubstructureRedirectMask), ref xEvent);


        //            //p = MouseDevice.Location;
        //            XEvent xevent = new XEvent();
        //            xevent.type = XEventName.ButtonRelease;
        //            xevent.ButtonEvent.button = 1;
        //            xevent.ButtonEvent.window = Handle;
        //            xevent.ButtonEvent.x = p.X - position.X;
        //            xevent.ButtonEvent.y = p.Y - position.Y;
        //            xevent.ButtonEvent.x_root = p.X;
        //            xevent.ButtonEvent.y_root = p.X;
        //            xevent.ButtonEvent.display = display;
        //            xevent.ButtonEvent.state = XModifierMask.Button1Mask;

        //            XSendEvent(display, Handle, false, (IntPtr)EventMask.ButtonReleaseMask, ref xevent);
        //            XFlush(display);
        //        }
        //    }
        //}

        bool firstLayout = true;
        private void Root_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            var b = position;
            var s = root.ActualSize;
            var l = root.ActualOffset;
            var src = root.Screen;
            if (size.Width != (int)Math.Ceiling(s.Width * RenderScaling) || size.Height != (int)Math.Ceiling(s.Height * RenderScaling) || b.X != (int)(l.X * RenderScaling + src.WorkingArea.X) || b.Y != (int)(l.Y * RenderScaling + src.WorkingArea.Y))
            {
                //Bounds = new Rect(l.X * RenderScaling, l.Y * RenderScaling, s.Width * RenderScaling, s.Height * RenderScaling);
                Position = new PixelPoint((int)(l.X * RenderScaling + src.WorkingArea.X), (int)(l.Y * RenderScaling + src.WorkingArea.Y));
                Resize(s, false);
            }
            //Console.WriteLine("Root_LayoutUpdated" + l.ToString() + "," + s.ToString() + "----" + position);
        }

        public void SetTitle(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            fixed (void* pdata = data)
            {
                XChangeProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_NAME, x11info.Atoms.UTF8_STRING, 8,
                    PropertyMode.Replace, pdata, data.Length);
                XStoreName(x11info.Display, Handle, text);

                //XSetIconName(x11info.Display, Handle, text);
                //XTextProperty xTextProperty = new XTextProperty { encoding = x11info.Atoms.UTF8_STRING, format = 8, value = (IntPtr)pdata, nitems = (IntPtr)data.Length };
                //XSetWMIconName(x11info.Display, Handle, ref xTextProperty);
                //XSetWMName(x11info.Display, Handle, ref xTextProperty);
                //Console.WriteLine(r);
            }
        }

        public void SetVisible(bool visible)
        {
            if (visible)
            {
                XMapWindow(x11info.Display, Handle);
                root.LayoutManager.ExecuteLayoutPass();
                root.Visibility = Visibility.Visible;
                if (CanActivate)
                {
                    Activate();
                }
                else
                {
                    XRaiseWindow(x11info.Display, Handle);
                }
                Position = position;
                //XFlush(x11info.Display);
                //Console.WriteLine("显示后：" + position);
            }
            else
            {
                activate = false;
                //position = new PixelPoint();
                XUnmapWindow(x11info.Display, Handle);
            }
        }
        //private HashSet<X11Window> _transientChildren = new HashSet<X11Window>();
        public bool ActivateTransientChildIfNeeded()
        {
            return _disabled;
            //if (_disabled)
            //{
            //    return true;
            //}
            //if (_transientChildren.Count == 0)
            //    return false;
            //var child = _transientChildren.First();
            //if (!child.ActivateTransientChildIfNeeded())
            //    child.Activate();
            //return true;
        }
        void SetTransientParent(X11Window window, bool informServer = true)
        {
            //parent?._transientChildren.Remove(this);
            //parent = window;
            //parent?._transientChildren.Add(this);
            if (informServer)
                XSetTransientForHint(x11info.Display, Handle, window?.Handle ?? IntPtr.Zero);
        }
        public void ShowDialog(Window window)
        {
            SetTransientParent((X11Window)window.ViewImpl);
            XMapWindow(x11info.Display, Handle);
            XFlush(x11info.Display);
            root.LayoutManager.ExecuteLayoutPass();
            root.Visibility = Visibility.Visible;
        }

        public void ShowInTaskbar(bool value)
        {
            ChangeWMAtoms(!value, x11info.Atoms._NET_WM_STATE_SKIP_TASKBAR);
        }

        public void TopMost(bool value)
        {
            ChangeWMAtoms(value, x11info.Atoms._NET_WM_STATE_ABOVE);
        }

        void ChangeWMAtoms(bool enable, params IntPtr[] atoms)
        {
            if (atoms.Length < 1 || atoms.Length > 4)
                throw new ArgumentException();

            //if (!_mapped)
            {
                XGetWindowProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_STATE, IntPtr.Zero, new IntPtr(256),
                    false, (IntPtr)Atom.XA_ATOM, out _, out _, out var nitems, out _,
                    out var prop);
                var ptr = (IntPtr*)prop.ToPointer();
                var newAtoms = new HashSet<IntPtr>();
                for (var c = 0; c < nitems.ToInt64(); c++)
                    newAtoms.Add(*ptr);
                XFree(prop);
                foreach (var atom in atoms)
                    if (enable)
                        newAtoms.Add(atom);
                    else
                        newAtoms.Remove(atom);

                XChangeProperty(x11info.Display, Handle, x11info.Atoms._NET_WM_STATE, (IntPtr)Atom.XA_ATOM, 32,
                    PropertyMode.Replace, newAtoms.ToArray(), newAtoms.Count);
            }

            SendNetWMMessage(x11info.Atoms._NET_WM_STATE,
                (IntPtr)(enable ? 1 : 0),
                atoms[0],
                atoms.Length > 1 ? atoms[1] : IntPtr.Zero,
                atoms.Length > 2 ? atoms[2] : IntPtr.Zero,
                atoms.Length > 3 ? atoms[3] : IntPtr.Zero
            );
        }

        protected override void Dispose(bool disposing)
        {
            context?.Dispose();
            context = null;
            SetTransientParent(null, false);
            if (xic != IntPtr.Zero)
            {
                XDestroyIC(xic);
                xic = IntPtr.Zero;
            }
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            base.Dispose(disposing);
            if (ismain)
            {
                LinuxPlatform.Platform.run = false;
            }
        }

        bool _disabled;
        public void SetEnable(bool enable)
        {
            _disabled = !enable;
        }
    }

    public class PopWindow : X11Window, IPopupImpl
    {

    }
}
