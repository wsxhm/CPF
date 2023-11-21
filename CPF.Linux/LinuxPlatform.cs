using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using CPF;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using System.Threading.Tasks;
using CPF.Controls;
using System.Linq;
using System.Diagnostics;
using static CPF.Linux.XLib;
using static CPF.Linux.Glib;
using static CPF.Linux.Gtk;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace CPF.Linux
{
    public class LinuxPlatform : RuntimePlatform
    {
        public static LinuxPlatform Platform;

        ////[DllImport("libXlibDemo.so")]
        ////public static extern IntPtr OpenIM(IntPtr dpy);
        ////[DllImport("libXlibDemo.so")]
        ////public static extern IntPtr CreateIC(IntPtr xim, IntPtr win);
        //[DllImport("libXlibDemo.so")]
        //public static extern void Move(IntPtr win, IntPtr dpy, ref XEvent xEvent);

        ///// <summary>
        ///// https://blog.csdn.net/qq_32768743/article/details/90605212
        ///// </summary>
        //public static void Test()
        //{
        //    Console.WriteLine(setlocale(6, ""));
        //    var dpy = XOpenDisplay(IntPtr.Zero);
        //    XSupportsLocale();
        //    Console.WriteLine(XSetLocaleModifiers(""));

        //    IntPtr default_string = IntPtr.Zero;
        //    var fontset = XCreateFontSet(dpy, "-*-*-medium-r-normal-*-*-120-*-*-*-*", out var missing_charsets, out var num_missing_charsets, ref default_string);

        //    Console.WriteLine("default_string:" + Marshal.PtrToStringUni(default_string));
        //    //Thread.Sleep(10000);
        //    var screen = XDefaultScreen(dpy);
        //    var win = XCreateSimpleWindow(dpy, XRootWindow(dpy, screen), 0, 0, 500, 200,
        //                              2, XBlackPixel(dpy, screen), XWhitePixel(dpy, screen));
        //    var gc = XCreateGC(dpy, win, 0, IntPtr.Zero);
        //    XSetForeground(dpy, gc, XWhitePixel(dpy, screen));
        //    XSetBackground(dpy, gc, XBlackPixel(dpy, screen));
        //    var im = XOpenIM(dpy, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        //    var list = XVaCreateNestedList(0, XNames.XNFontSet, fontset, IntPtr.Zero);
        //    var best_style = XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing;
        //    Console.WriteLine(string.Join("-", X11Window.GetSupportedInputStyles(im)));
        //    var ic = XCreateIC(im,
        //                   XNames.XNInputStyle, best_style,
        //                   XNames.XNClientWindow, win,
        //                   IntPtr.Zero);
        //    XFree(list);

        //    long im_event_mask = 0;
        //    XGetICValues(ic, XNames.XNFilterEvents, ref im_event_mask, IntPtr.Zero);
        //    XSelectInput(dpy, win, (IntPtr)((long)(XEventMask.ExposureMask | XEventMask.KeyPressMask
        //                           | XEventMask.StructureNotifyMask) | im_event_mask));
        //    XSetICFocus(ic);
        //    XMapWindow(dpy, win);
        //    //XRectangle preedit_area = new XRectangle();
        //    //XRectangle status_area = new XRectangle();

        //    while (true)
        //    {
        //        XNextEvent(dpy, out var xEvent);
        //        if (XFilterEvent(ref xEvent, IntPtr.Zero))
        //            continue;

        //        //switch (xEvent.type)
        //        //{
        //        //    case XEventName.KeyPress:

        //        //        break;
        //        //    case XEventName.ConfigureNotify:

        //        //        if ((best_style & XIMProperties.XIMPreeditArea) != 0)
        //        //        {
        //        //            preedit_area.width = (ushort)(xEvent.ConfigureEvent.width * 4 / 5);
        //        //            preedit_area.height = 0;
        //        //            GetPreferredGeometry(ic, XNames.XNPreeditAttributes, ref preedit_area);
        //        //            preedit_area.x = (short)(xEvent.ConfigureEvent.width - preedit_area.width);
        //        //            preedit_area.y = (short)(xEvent.ConfigureEvent.height - preedit_area.height);
        //        //            SetGeometry(ic, XNames.XNPreeditAttributes, preedit_area);
        //        //        }
        //        //        if ((best_style & XIMProperties.XIMStatusArea) != 0)
        //        //        {
        //        //            status_area.width = (ushort)(xEvent.ConfigureEvent.width / 5);
        //        //            status_area.height = 0;
        //        //            GetPreferredGeometry(ic, XNames.XNStatusAttributes, ref status_area);
        //        //            status_area.x = 0;
        //        //            status_area.y = (short)(xEvent.ConfigureEvent.height - status_area.height);
        //        //            SetGeometry(ic, XNames.XNStatusAttributes, status_area);
        //        //        }
        //        //        break;
        //        //    default:
        //        //        break;
        //        //}

        //    }


        //}

        //static void GetPreferredGeometry(IntPtr ic, string name, ref XRectangle area)
        ////XIC ic;
        ////char *name; /* XNPreEditAttributes or XNStatusAttributes */
        ////XRectangle *area; /* the constraints on the area */
        //{
        //    var list = XVaCreateNestedList(0, "areaNeeded", ref area, IntPtr.Zero);
        //    /* set the constraints */
        //    XSetICValues(ic, name, list, IntPtr.Zero);
        //    /* Now query the preferred size */
        //    /* The Xsi input method, Xwnmo, seems to ignore the constraints, */
        //    /* but we’re not going to try to enforce them here. */
        //    XGetICValues(ic, name, list, IntPtr.Zero);
        //    XFree(list);
        //}
        ////XIC ic;
        ////char *name; /* XNPreEditAttributes or XNStatusAttributes */
        ////XRectangle *area; /* the actual area to set */
        //static unsafe void SetGeometry(IntPtr ic, string name, XRectangle area)
        //{
        //    var list = XVaCreateNestedList(0, "area", ref area, IntPtr.Zero);
        //    XSetICValues(ic, name, list, IntPtr.Zero);
        //    XFree(list);
        //}


        enum EventCodes
        {
            X11 = 1,
            Signal = 2
        }

        //private int _sigread, _sigwrite;
        //private int _epoll;
        private object _lock = new object();
        //private bool _signaled;

        static Pollfd[] pollfds;        // For watching the X11 socket
        static Socket listen;           //
        static Socket wake;         //
        static Socket wake_receive;     //
        static byte[] network_buffer;		//
        /// <summary>
        /// 启用触摸事件，之所有加上这个，因为部分Linux对XI2支持有问题
        /// </summary>
        public bool EnabledTouch { get; set; }

        public unsafe LinuxPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine($"系统架构：{RuntimeInformation.OSArchitecture}");
                Console.WriteLine($"系统名称：{RuntimeInformation.OSDescription}");
                Console.WriteLine($"进程架构：{RuntimeInformation.ProcessArchitecture}");
                Console.WriteLine($"是否64位操作系统：{Environment.Is64BitOperatingSystem}");
                Console.WriteLine("CPU CORE:" + Environment.ProcessorCount);
                Console.WriteLine("HostName:" + Environment.MachineName);
                Console.WriteLine("Version:" + Environment.OSVersion);
                Console.WriteLine("CPF Version:" + typeof(LinuxPlatform).Assembly.GetName().Version);

                Platform = this;
                XInitThreads();
                Display = XOpenDisplay(IntPtr.Zero);
                //Console.WriteLine(setlocale(6, ""));
                //DeferredDisplay = XOpenDisplay(IntPtr.Zero);
                if (Display == IntPtr.Zero)
                    throw new Exception("XOpenDisplay failed");
                XError.Init();
                Info = new X11Info(Display, DeferredDisplay);

                Thread.CurrentThread.Name = "主线程";
                _cursors = Enum.GetValues(typeof(CursorFontShape)).Cast<CursorFontShape>()
                    .ToDictionary(id => id, id => XLib.XCreateFontCursor(Display, id));


                if (Info.XInputVersion != null && EnabledTouch)
                {
                    var xi2 = new XI2Manager();
                    if (xi2.Init(this))
                        XI2 = xi2;
                }

                var fd = XLib.XConnectionNumber(Display);
                //var ev = new epoll_event()
                //{
                //    events = EPOLLIN,
                //    data = { u32 = (int)EventCodes.X11 }
                //};
                //_epoll = epoll_create1(0);
                //if (_epoll == -1)
                //    throw new Exception("epoll_create1 failed");

                //if (epoll_ctl(_epoll, EPOLL_CTL_ADD, fd, ref ev) == -1)
                //    throw new Exception("Unable to attach X11 connection handle to epoll");

                //var fds = stackalloc int[2];
                ////pipe2(fds, O_NONBLOCK);
                //if (pipe2(fds, O_NONBLOCK) == -1)
                //    throw new Exception("Unable to create X11 pipe");

                //_sigread = fds[0];
                //_sigwrite = fds[1];

                //ev = new epoll_event
                //{
                //    events = EPOLLIN,
                //    data = { u32 = (int)EventCodes.Signal }
                //};
                //if (epoll_ctl(_epoll, EPOLL_CTL_ADD, _sigread, ref ev) == -1)
                //    throw new Exception("Unable to attach signal pipe to epoll");


                // For sleeping on the X11 socket
                listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 0);
                listen.Bind(ep);
                listen.Listen(1);

                // To wake up when a timer is ready
                network_buffer = new byte[10];

                wake = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                wake.Connect(listen.LocalEndPoint);

                // Make this non-blocking, so it doesn't
                // deadlock if too many wakes are sent
                // before the wake_receive end is polled
                wake.Blocking = false;

                wake_receive = listen.Accept();

                pollfds = new Pollfd[2];
                pollfds[0] = new Pollfd();
                pollfds[0].fd = fd;
                pollfds[0].events = PollEvents.POLLIN;

                pollfds[1] = new Pollfd();
                pollfds[1].fd = wake_receive.Handle.ToInt32();
                pollfds[1].events = PollEvents.POLLIN;


                XrmInitialize(); /* Need to initialize the DB before calling Xrm* functions */

            }
        }
        public XI2Manager XI2;
        public X11Info Info { get; private set; }
        public IntPtr DeferredDisplay { get; set; }
        public IntPtr Display { get; set; }

        public override PixelPoint MousePosition
        {
            get
            {
                var po = GetCursorPos(Info);
                //Console.WriteLine(po);
                return new PixelPoint(po.x, po.y);
            }
        }

        public override TimeSpan DoubleClickTime
        {
            get
            {
                return TimeSpan.FromSeconds(0.4);
            }
        }


        public override IPopupImpl CreatePopup()
        {
            return new PopWindow();
        }

        public override IWindowImpl CreateWindow()
        {
            return new X11Window();
        }

        DataObject dataObject;
        public override DragDropEffects DoDragDrop(DragDropEffects allowedEffects, params (DataFormat, object)[] data)
        {
            dataObject = new DataObject();
            dataObject.StartDrag(allowedEffects, data);


            return allowedEffects;
        }

        public override IReadOnlyList<Screen> GetAllScreen()
        {
            return ScreenImpl.Screens;
        }

        public override IClipboard GetClipboard()
        {
            return new X11Clipboard();
        }



        private Dictionary<CursorFontShape, IntPtr> _cursors;
        private static readonly Dictionary<Cursors, CursorFontShape> s_mapping =
            new Dictionary<Cursors, CursorFontShape>
            {
                {Cursors.Arrow, CursorFontShape.XC_top_left_arrow},
                {Cursors.Cross, CursorFontShape.XC_cross},
                {Cursors.Hand, CursorFontShape.XC_hand1},
                {Cursors.Help, CursorFontShape.XC_question_arrow},
                {Cursors.Ibeam, CursorFontShape.XC_xterm},
                {Cursors.No, CursorFontShape.XC_X_cursor},
                {Cursors.Wait, CursorFontShape.XC_watch},
                {Cursors.AppStarting, CursorFontShape.XC_watch},
                {Cursors.BottomSide, CursorFontShape.XC_bottom_side},
                {Cursors.DragCopy, CursorFontShape.XC_center_ptr},
                {Cursors.DragLink, CursorFontShape.XC_fleur},
                {Cursors.DragMove, CursorFontShape.XC_diamond_cross},
                {Cursors.LeftSide, CursorFontShape.XC_left_side},
                {Cursors.RightSide, CursorFontShape.XC_right_side},
                {Cursors.SizeAll, CursorFontShape.XC_sizing},
                {Cursors.TopSide, CursorFontShape.XC_top_side},
                {Cursors.UpArrow, CursorFontShape.XC_sb_up_arrow},
                {Cursors.BottomLeftCorner, CursorFontShape.XC_bottom_left_corner},
                {Cursors.BottomRightCorner, CursorFontShape.XC_bottom_right_corner},
                {Cursors.SizeNorthSouth, CursorFontShape.XC_sb_v_double_arrow},
                {Cursors.SizeWestEast, CursorFontShape.XC_sb_h_double_arrow},
                {Cursors.TopLeftCorner, CursorFontShape.XC_top_left_corner},
                {Cursors.TopRightCorner, CursorFontShape.XC_top_right_corner},
            };

        public override object GetCursor(Cursors cursorType)
        {
            IntPtr handle;
            //if (cursorType == Cursors.None)
            //{
            //    handle = _nullCursor;
            //}
            //else
            //{
            handle = s_mapping.TryGetValue(cursorType, out var shape)
            ? _cursors[shape]
            : _cursors[CursorFontShape.XC_top_left_arrow];
            //}
            return handle;
        }

        public override SynchronizationContext GetSynchronizationContext()
        {
            return new LinuxSynchronizationContext();
        }

        static Dictionary<KeyGesture, PlatformHotkey> keyValuePairs = new Dictionary<KeyGesture, PlatformHotkey>() {
            { new KeyGesture(Keys.C,InputModifiers.Control),PlatformHotkey.Copy},
            { new KeyGesture(Keys.X,InputModifiers.Control),PlatformHotkey.Cut},
            { new KeyGesture(Keys.V,InputModifiers.Control),PlatformHotkey.Paste},
            { new KeyGesture(Keys.Y,InputModifiers.Control),PlatformHotkey.Redo},
            { new KeyGesture(Keys.A,InputModifiers.Control),PlatformHotkey.SelectAll},
            { new KeyGesture(Keys.Z,InputModifiers.Control),PlatformHotkey.Undo},
        };
        public override PlatformHotkey Hotkey(KeyGesture keyGesture)
        {
            keyValuePairs.TryGetValue(keyGesture, out PlatformHotkey platformHotkey);
            return platformHotkey;
        }

        internal Dictionary<IntPtr, XWindow> windows = new Dictionary<IntPtr, XWindow>();
        internal bool run = true;
        public unsafe override void Run()
        {
            //X11Window x11Window = new X11Window();
            //x11Window.SetVisible(true);
            DoTask();
            while (run)
            {
                XSync(Display, false);
                while (XPending(Display) != 0)
                {
                    XNextEvent(Display, out var xev);
                    if (XFilterEvent(ref xev, IntPtr.Zero))
                    {
                        continue;
                    }
                    OnEvent(ref xev, null);
                }
                //Invoke();
                //Thread.Sleep(1);
                //lock (_lock)
                //{
                //    _signaled = false;
                //    int buf = 0;
                //    while (read(_sigread, &buf, new IntPtr(4)).ToInt64() > 0)
                //    {
                //    }
                //}
                DoTask();
                Wait();
            }
            XCloseDisplay(Display);
        }

        public override void Run(CancellationToken cancellation)
        {
            //Console.WriteLine("开始循环");
            while (!cancellation.IsCancellationRequested && run)
            {
                XSync(Display, false);
                while (XPending(Display) != 0)
                {
                    XNextEvent(Display, out var xev);
                    if (XFilterEvent(ref xev, IntPtr.Zero))
                    {
                        continue;
                    }
                    OnEvent(ref xev, null);
                }
                DoTask();
                if (cancellation.IsCancellationRequested)
                {
                    break;
                }
                Wait();
            }
            //Console.WriteLine("结束循环" + cancellation.IsCancellationRequested + run);
        }
        private unsafe void Wait()
        {
            XFlush(Display);
            if (XPending(Display) == 0)
            {
                //if (run)
                //{
                //epoll_event ev;
                //    epoll_wait(_epoll, &ev, 1, -1);//-1永久阻塞，直到有信号,
                //}

                poll(pollfds, (uint)pollfds.Length, -1);
                // Clean out buffer, so we're not busy-looping on the same data
                //if (length == pollfds.Length)
                {
                    if (pollfds[1].revents != 0)
                        wake_receive.Receive(network_buffer, 0, 1, SocketFlags.None);

                }
            }
            CPF.Threading.DispatcherTimer.SetTimeTick();
        }

        HashSet<XEventHandler> handlers = new HashSet<XEventHandler>();
        public unsafe void RunMainLoop(CancelHandle cancelHandle, XEventHandler action)
        {
            handlers.Add(action);
            while (!cancelHandle.Cancel && run)
            {
                XSync(Display, false);
                while (XPending(Display) != 0)
                {
                    XNextEvent(Display, out var xev);
                    if (XFilterEvent(ref xev, IntPtr.Zero))
                    {
                        continue;
                    }
                    if (!action(ref xev))
                    {
                        OnEvent(ref xev, action);
                    }
                }
                //Invoke();
                //Thread.Sleep(1);
                //lock (_lock)
                //{
                //    _signaled = false;
                //    int buf = 0;
                //    while (read(_sigread, &buf, new IntPtr(4)).ToInt64() > 0)
                //    {
                //    }
                //}

                DoTask();
                Wait();
            }
            handlers.Remove(action);
        }

        internal unsafe void SetFlag()
        {
            //lock (_lock)
            //{
            //    if (_signaled)
            //        return;
            //    _signaled = true;
            //    int buf = 0;
            //    write(_sigwrite, &buf, new IntPtr(1));
            //}
            try
            {
                wake.Send(new byte[] { 0xFF });
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.WouldBlock)
                {
                    throw;
                }
            }
        }

        void Invoke()
        {
            if (LinuxSynchronizationContext.asyncQueue.TryDequeue(out SendOrPostData postData))
            {
                postData.SendOrPostCallback(postData.Data);
            }
            if (LinuxSynchronizationContext.invokeQueue.TryDequeue(out SendOrPostData result))
            {
                result.SendOrPostCallback(result.Data);
                result.ManualResetEvent.Set();
            }
        }
        unsafe void OnEvent(ref XEvent xev, XEventHandler action)
        {
            foreach (var item in handlers.Reverse())
            {
                if (action != item)
                {
                    if (item(ref xev))
                    {
                        return;
                    }
                }
            }

            //if (xev.type == XEventName.GenericEvent)
            //    fixed (void* data = &xev.GenericEventCookie)
            //    {
            //        XGetEventData(Display, data);
            //    }
            if (windows.TryGetValue(xev.AnyEvent.window, out var window))
            {
                lock (XWindow.XlibLock)
                {
                    window.OnEvent(ref xev);
                }
            }
            else if (xev.type == XEventName.GenericEvent && LinuxPlatform.Platform.XI2 != null)
            {
                fixed (void* data = &xev.GenericEventCookie)
                {
                    XGetEventData(Info.Display, data);
                    try
                    {
                        if (Info.XInputOpcode ==
                            xev.GenericEventCookie.extension)
                        {
                            //Console.WriteLine((IntPtr)ev.GenericEventCookie.data);
                            var ev = (XIEvent*)xev.GenericEventCookie.data;
                            if (windows.TryGetValue(((XIDeviceEvent*)ev)->EventWindow, out window))
                            {
                                Platform.XI2.OnEvent(ev, (X11Window)window);
                            }
                        }
                    }
                    finally
                    {
                        if (xev.GenericEventCookie.data != null)
                            XFreeEventData(Info.Display, data);
                    }
                }
            }
            //}
            //finally
            //{
            //    if (xev.type == XEventName.GenericEvent && xev.GenericEventCookie.data != null)
            //        XFreeEventData(Display, &xev.GenericEventCookie);
            //}
        }

        private static unsafe void DoTask()
        {
            if (LinuxSynchronizationContext.invokeQueue.Count > 0)
            {
                while (LinuxSynchronizationContext.invokeQueue.TryDequeue(out var result))
                {
                    result.SendOrPostCallback(result.Data);
                    result.ManualResetEvent.Set();
                }
            }
            if (LinuxSynchronizationContext.asyncQueue.Count > 0)
            {
                while (LinuxSynchronizationContext.asyncQueue.TryDequeue(out SendOrPostData data))
                {
                    data.SendOrPostCallback(data.Data);
                }
            }
        }

        void UpdateParent(IntPtr chooser, IWindowImpl parentWindow)
        {
            var xid = ((X11Window)parentWindow).Handle;
            gtk_widget_realize(chooser);
            var window = gtk_widget_get_window(chooser);
            var parent = GetForeignWindow(xid);
            if (window != IntPtr.Zero && parent != IntPtr.Zero)
                gdk_window_set_transient_for(window, parent);
        }

        async Task EnsureInitialized()
        {
            if (_initialized == null) _initialized = StartGtk();

            if (!(await _initialized))
                throw new Exception("Unable to initialize GTK on separate thread");
        }
        private Task<bool> _initialized;
        private unsafe Task<string[]> ShowDialog(string title, IWindowImpl parent, GtkFileChooserAction action,
            bool multiSelect, string initialFileName, IEnumerable<FileDialogFilter> filters)
        {
            IntPtr dlg;
            using (var name = new Utf8Buffer(title))
                dlg = gtk_file_chooser_dialog_new(name, IntPtr.Zero, action, IntPtr.Zero);
            UpdateParent(dlg, parent);
            if (multiSelect)
                gtk_file_chooser_set_select_multiple(dlg, true);

            gtk_window_set_modal(dlg, true);
            var tcs = new TaskCompletionSource<string[]>();
            List<IDisposable> disposables = null;


            if (filters != null)
                foreach (var f in filters.Where(a => !string.IsNullOrWhiteSpace(a.Extensions)))
                {
                    var filter = gtk_file_filter_new();
                    using (var b = new Utf8Buffer(f.Name))
                        gtk_file_filter_set_name(filter, b);

                    foreach (var e in f.Extensions.Split(',').Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()))
                        using (var b = new Utf8Buffer("*." + e))
                            gtk_file_filter_add_pattern(filter, b);

                    gtk_file_chooser_add_filter(dlg, filter);
                }

            disposables = new List<IDisposable>
            {
                ConnectSignal<signal_generic>(dlg, "close", delegate
                {
                    tcs.TrySetResult(null);
                    foreach (var d in disposables) d.Dispose();
                    disposables.Clear();
                    return false;
                }),
                ConnectSignal<signal_dialog_response>(dlg, "response", (_, resp, __) =>
                {
                    string[] result = null;
                    if (resp == GtkResponseType.Accept)
                    {
                        var resultList = new List<string>();
                        var gs = gtk_file_chooser_get_filenames(dlg);
                        var cgs = gs;
                        while (cgs != null)
                        {
                            if (cgs->Data != IntPtr.Zero)
                                resultList.Add(Utf8Buffer.StringFromPtr(cgs->Data));
                            cgs = cgs->Next;
                        }
                        g_slist_free(gs);
                        result = resultList.ToArray();
                    }

                    gtk_widget_hide(dlg);
                    foreach (var d in disposables) d.Dispose();
                    disposables.Clear();
                    tcs.TrySetResult(result);
                    return false;
                })
            };
            using (var open = new Utf8Buffer(
                action == GtkFileChooserAction.Save ? "Save"
                : action == GtkFileChooserAction.SelectFolder ? "Select"
                : "Open"))
                gtk_dialog_add_button(dlg, open, GtkResponseType.Accept);
            using (var open = new Utf8Buffer("Cancel"))
                gtk_dialog_add_button(dlg, open, GtkResponseType.Cancel);
            if (initialFileName != null)
                using (var fn = new Utf8Buffer(initialFileName))
                {
                    if (action == GtkFileChooserAction.Save)
                        gtk_file_chooser_set_current_name(dlg, fn);
                    else
                        gtk_file_chooser_set_filename(dlg, fn);
                }

            gtk_window_present(dlg);
            return tcs.Task;
        }

        string NameWithExtension(string path, string defaultExtension, FileDialogFilter filter)
        {
            var name = Path.GetFileName(path);
            if (name != null && !name.Contains("."))
            {
                if (!string.IsNullOrWhiteSpace(filter?.Extensions))
                {
                    if (defaultExtension != null
                        && filter.Extensions.Contains(defaultExtension))
                        return path + "." + defaultExtension.TrimStart('.');

                    var ext = filter.Extensions.Split(',').FirstOrDefault(x => x != "*");
                    if (ext != null)
                        return path + "." + ext.TrimStart('.');
                }

                if (defaultExtension != null)
                    path += "." + defaultExtension.TrimStart('.');
            }

            return path;
        }

        public override async Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent)
        {
            await EnsureInitialized();
            return await await RunOnGlibThread(
                () => ShowDialog(dialog.Title, parent,
                    dialog is OpenFileDialog ? GtkFileChooserAction.Open : GtkFileChooserAction.Save,
                    (dialog as OpenFileDialog)?.AllowMultiple ?? false,
                    System.IO.Path.Combine(string.IsNullOrEmpty(dialog.Directory) ? "" : dialog.Directory,
                        string.IsNullOrEmpty(dialog.InitialFileName) ? "" : dialog.InitialFileName), dialog.Filters));
        }

        public override async Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent)
        {
            await EnsureInitialized();
            return await await RunOnGlibThread(async () =>
            {
                var res = await ShowDialog(dialog.Title, parent,
                    GtkFileChooserAction.SelectFolder, false, dialog.Directory, null);
                return res?.FirstOrDefault();
            });
        }

        public override INativeImpl CreateNative()
        {
            return new NativeHost();
        }

        public override INotifyIconImpl CreateNotifyIcon()
        {
            return new NotifyIcon();
        }

    }
    /// <summary>
    /// 处理事件，返回值为true的时候不调用默认处理方法
    /// </summary>
    /// <param name="xEvent"></param>
    /// <returns></returns>
    public delegate bool XEventHandler(ref XEvent xEvent);

    public class CancelHandle
    {
        public bool Cancel { get; set; }

        public object Data { get; set; }
        /// <summary>
        /// 设置数据，同时设置Cancel=true
        /// </summary>
        /// <param name="data"></param>
        public void SetResult(object data)
        {
            Data = data;
            Cancel = true;
        }
    }
}
