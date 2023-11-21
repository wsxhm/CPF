using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using System.Linq;
using System.Runtime.InteropServices;
using CPF.Drawing;

namespace CPF.Linux
{
    class DataObject : IDataObject
    {
        public static IntPtr textAtom = XLib.XInternAtom(LinuxPlatform.Platform.Display, "text/plain", false);
        public static IntPtr textUtf8Atom = XLib.XInternAtom(LinuxPlatform.Platform.Display, "text/plain;charset=utf-8", false);
        public static IntPtr htmlAtom = XLib.XInternAtom(LinuxPlatform.Platform.Display, "text/html", false);
        public static IntPtr fileNamesAtom = XLib.XInternAtom(LinuxPlatform.Platform.Display, "text/uri-list", false);
        public static List<(IntPtr, IntPtr, DataFormat)> atomNonProtocols = new List<(IntPtr, IntPtr, DataFormat)>();
        static DataObject()
        {
            var atoms = LinuxPlatform.Platform.Info.Atoms;
            //atomNonProtocols.Add((textAtom, XLib.XInternAtom(LinuxPlatform.Platform.Display,
            //            String.Concat("MWFNonP+", "text/plain"), false), DataFormat.Text));
            //atomNonProtocols.Add((htmlAtom, XLib.XInternAtom(LinuxPlatform.Platform.Display,
            //            String.Concat("MWFNonP+", "text/html"), false), DataFormat.Html));
            //atomNonProtocols.Add((fileNamesAtom, XLib.XInternAtom(LinuxPlatform.Platform.Display,
            //            String.Concat("MWFNonP+", "text/uri-list"), false), DataFormat.FileNames));
            atomNonProtocols.Add((atoms.UTF8_STRING, atoms.UTF8_STRING, DataFormat.Text));
            atomNonProtocols.Add((atoms.COMPOUND_TEXT, atoms.COMPOUND_TEXT, DataFormat.Text));
            atomNonProtocols.Add((atoms.XA_STRING, atoms.XA_STRING, DataFormat.Text));
            atomNonProtocols.Add((atoms.STRING, atoms.STRING, DataFormat.Text));
            atomNonProtocols.Add((atoms.TEXT, atoms.TEXT, DataFormat.Text));
        }

        string text;
        string html;
        List<string> fileNames;
        X11Info x11Info;
        public DataObject()
        {
            x11Info = LinuxPlatform.Platform.Info;
        }


        public bool Contains(DataFormat dataFormat)
        {
            switch (dataFormat)
            {
                case DataFormat.Text:
                    return text != null;
                case DataFormat.Html:
                    return html != null;
                case DataFormat.FileNames:
                    return fileNames != null;
                default:
                    return false;
            }
        }

        public object GetData(DataFormat dataFormat)
        {
            switch (dataFormat)
            {
                case DataFormat.Text:
                    return text;
                case DataFormat.Html:
                    return html;
                case DataFormat.FileNames:
                    return fileNames;
                default:
                    return null;
            }
        }

        public DragDropEffects EffectFromAction(IntPtr action)
        {
            if (action == x11Info.Atoms.XdndActionCopy)
                return DragDropEffects.Copy;
            else if (action == x11Info.Atoms.XdndActionMove)
                return DragDropEffects.Move;
            if (action == x11Info.Atoms.XdndActionLink)
                return DragDropEffects.Link;

            return DragDropEffects.None;
        }

        private IntPtr ActionFromEffect(DragDropEffects effect)
        {
            IntPtr action = IntPtr.Zero;

            // We can't OR together actions on XDND so sadly the primary
            // is the only one shown here
            if ((effect & DragDropEffects.Copy) != 0)
                action = x11Info.Atoms.XdndActionCopy;
            else if ((effect & DragDropEffects.Move) != 0)
                action = x11Info.Atoms.XdndActionMove;
            else if ((effect & DragDropEffects.Link) != 0)
                action = x11Info.Atoms.XdndActionLink;
            return action;
        }

        //private IntPtr target;
        //private IntPtr source;
        //private IntPtr toplevel;
        IntPtr[] SupportedTypes;
        Point point;
        CancelHandle cancelHandle;
        IntPtr LastWindow;
        IntPtr TargetWindow;
        IntPtr SourceWindow;
        DragState State;
        IntPtr Action;
        public DragDropEffects AllowedEffects;
        //private bool dropped = false;
        (DataFormat, object)[] data;

        public DragDropEffects StartDrag(DragDropEffects allowedEffects, params (DataFormat, object)[] data)
        {
            if (X11Window.mouseDownWindow == IntPtr.Zero)
            {
                throw new Exception("必须是鼠标按下的时候调用");
            }
            SourceWindow = X11Window.mouseDownWindow;
            State = DragState.Beginning;
            //MouseState = XplatUIX11.MouseState;
            AllowedEffects = allowedEffects;
            Action = ActionFromEffect(allowedEffects);
            this.data = data;

            var suc = XLib.XSetSelectionOwner(LinuxPlatform.Platform.Display, x11Info.Atoms.XdndSelection, X11Window.mouseDownWindow, IntPtr.Zero);

            List<IntPtr> types = new List<IntPtr>();
            foreach (var item in data)
            {
                switch (item.Item1)
                {
                    case DataFormat.Text:
                        types.Add(textAtom);
                        types.Add(x11Info.Atoms.UTF8_STRING);
                        types.Add(x11Info.Atoms.STRING);
                        types.Add(x11Info.Atoms.TEXT);
                        types.Add(x11Info.Atoms.COMPOUND_TEXT);
                        types.Add(textUtf8Atom);
                        break;
                    case DataFormat.Html:
                        types.Add(htmlAtom);
                        break;
                    case DataFormat.FileNames:
                        types.Add(fileNamesAtom);
                        break;
                    default:
                        break;
                }
            }
            SupportedTypes = types.ToArray();

            if (suc == 0)
            {
                Console.Error.WriteLine("Could not take ownership of XdndSelection aborting drag.");
                //drag_data.Reset();
                return DragDropEffects.None;
            }
            //source = toplevel = target = X11Window.mouseDownWindow;
            State = DragState.Dragging;
            //dropped = false;

            TargetWindow = X11Window.mouseDownWindow;
            SendEnter(X11Window.mouseDownWindow, X11Window.mouseDownWindow, SupportedTypes);

            if (cancelHandle != null)
            {
                cancelHandle.Cancel = true;
            }
            cancelHandle = new CancelHandle();
            LinuxPlatform.Platform.RunMainLoop(cancelHandle, OnEvent);
            //if (!dropped)
            return EffectFromAction(Action);

        }
        bool WillAccept;
        // This version seems to be the most common
        private static readonly IntPtr[] XdndVersion = new IntPtr[] { new IntPtr(4) };
        private unsafe void SendEnter(IntPtr handle, IntPtr from, IntPtr[] supported)
        {
            XEvent xevent = new XEvent();

            xevent.AnyEvent.type = XEventName.ClientMessage;
            xevent.AnyEvent.display = LinuxPlatform.Platform.Display;
            xevent.ClientMessageEvent.window = handle;
            xevent.ClientMessageEvent.message_type = x11Info.Atoms.XdndEnter;
            xevent.ClientMessageEvent.format = 32;
            xevent.ClientMessageEvent.ptr1 = from;
            xevent.ClientMessageEvent.ptr5 = Action;

            // (int) xevent.ClientMessageEvent.ptr2 & 0x1)
            // int ptr2 = 0x1;
            // xevent.ClientMessageEvent.ptr2 = (IntPtr) ptr2;
            // (e)->xclient.data.l[1] = ((e)->xclient.data.l[1] & ~(0xFF << 24)) | ((v) << 24)

            //xevent.ClientMessageEvent.ptr2 = (IntPtr)((long)XdndVersion[0] << 24);
            xevent.ClientMessageEvent.ptr2 = (IntPtr)67108865;

            //if (supported.Length > 0)
            //    xevent.ClientMessageEvent.ptr3 = supported[0];
            //if (supported.Length > 1)
            //    xevent.ClientMessageEvent.ptr4 = supported[1];
            //if (supported.Length > 2)
            //    xevent.ClientMessageEvent.ptr5 = supported[2];

            //var list = supported.Select(a => (int)a).ToArray();
            //fixed (void* data = list)
            //{
            XLib.XChangeProperty(x11Info.Display, from, x11Info.Atoms.XdndTypeList, (IntPtr)Atom.XA_ATOM, 32, PropertyMode.Replace, supported, supported.Length);
            //}

            Console.WriteLine("SendEnter" + handle);
            XLib.XSendEvent(LinuxPlatform.Platform.Display, handle, false, IntPtr.Zero, ref xevent);
        }

        bool OnEvent(ref XEvent xEvent)
        {
            //Console.WriteLine(xEvent.type);
            switch (xEvent.type)
            {
                case XEventName.ButtonRelease:
                    SetResult();
                    return true;
                case XEventName.MotionNotify:
                    point = new Point(xEvent.MotionEvent.x, xEvent.MotionEvent.y);
                    HandleMouseOver();
                    return true;
                case XEventName.GenericEvent:
                    unsafe
                    {
                        fixed (void* data = &xEvent.GenericEventCookie)
                        {
                            XLib.XGetEventData(LinuxPlatform.Platform.Info.Display, data);
                            try
                            {
                                if (LinuxPlatform.Platform.Info.XInputOpcode ==
                                    xEvent.GenericEventCookie.extension)
                                {
                                    var xev = (XIEvent*)xEvent.GenericEventCookie.data;
                                    if (LinuxPlatform.Platform.windows.TryGetValue(((XIDeviceEvent*)xev)->EventWindow, out var window))
                                    {
                                        if (xev->evtype == XiEventType.XI_ButtonRelease)
                                        {
                                            SetResult();
                                        }
                                        else if (xev->evtype == XiEventType.XI_Motion)
                                        {
                                            var ev = (XIDeviceEvent*)xev;
                                            point = new Point((float)ev->event_x, (float)ev->event_y);
                                            HandleMouseOver();
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                if (xEvent.GenericEventCookie.data != null)
                                    XLib.XFreeEventData(LinuxPlatform.Platform.Info.Display, data);
                            }
                        }
                        return true;
                    }
                case XEventName.ClientMessage:
                    if (xEvent.ClientMessageEvent.message_type == x11Info.Atoms.XdndStatus)
                    {
                        WillAccept = ((int)xEvent.ClientMessageEvent.ptr2 & 0x1) != 0;
                        GiveFeedback(xEvent.ClientMessageEvent.ptr5);
                        //Console.WriteLine("WillAccept" + WillAccept);
                        return true;
                    }
                    else if (xEvent.ClientMessageEvent.message_type == x11Info.Atoms.XdndFinished)
                    {
                        cancelHandle.Cancel = true;
                        return true;
                    }
                    break;
                case XEventName.SelectionRequest:
                    if (xEvent.SelectionRequestEvent.selection != x11Info.Atoms.XdndSelection)
                        break;
                    //Console.WriteLine("requestor:" + xEvent.SelectionRequestEvent.requestor);

                    //foreach (var item in data)
                    //{
                    var type = xEvent.SelectionRequestEvent.target;
                    var item = data.FirstOrDefault();
                    if (type == textAtom || type == textUtf8Atom || type == x11Info.Atoms.STRING || type == x11Info.Atoms.TEXT)
                    {
                        item = data.FirstOrDefault(a => a.Item1 == DataFormat.Text);
                    }
                    else if (type == htmlAtom)
                    {
                        item = data.FirstOrDefault(a => a.Item1 == DataFormat.Html);
                    }
                    else if (type == fileNamesAtom)
                    {
                        item = data.FirstOrDefault(a => a.Item1 == DataFormat.FileNames);
                    }
                    if (item.Item2 != null)
                    {
                        switch (item.Item1)
                        {
                            case DataFormat.Text:
                                SetTextData(item.Item2, ref xEvent);
                                break;
                            case DataFormat.Html:
                                SetHtmlData(item.Item2, ref xEvent);
                                break;
                            case DataFormat.FileNames:
                                SetFileNameData(item.Item2, ref xEvent);
                                break;
                        }
                        //}
                        XLib.XFlush(x11Info.Display);
                    }
                    return true;
            }
            return false;
        }

        private void SetResult()
        {
            if (State == DragState.Beginning)
            {
                //state = State.Accepting;
            }
            else if (State != DragState.None)
            {
                if (WillAccept)
                {
                    SendDrop(TargetWindow, SourceWindow, IntPtr.Zero);
                    SendLeave(TargetWindow, SourceWindow);
                    XLib.XChangeActivePointerGrab(x11Info.Display,
EventMask.ButtonMotionMask |
EventMask.PointerMotionMask |
EventMask.ButtonPressMask |
EventMask.ButtonReleaseMask,
(IntPtr)((Cursor)Cursors.Arrow).PlatformCursor, IntPtr.Zero);
                    //if (QueryContinue(false, DragAction.Drop))
                    //    return;
                }
                else
                {

                    cancelHandle.Cancel = true;
                    SendLeave(TargetWindow, SourceWindow);
                    //if (QueryContinue(false, DragAction.Cancel))
                    //    return;

                    // fallback if no movement was detected, as .net does.
                    //if (motion_poll == -1)
                    //    DefaultEnterLeave(drag_data.Data);
                }

                State = DragState.None;
                // WE can't reset the drag data yet as it is still
                // most likely going to be used by the SelectionRequest
                // handlers
            }
        }

        DataFormat dataFormat;
        DragDropEffects dragDropEffects;
        public DragDropEffects DropEffects
        {
            get { return dragDropEffects; }
        }

        public void SetDragDropEffects(DragDropEffects dragDropEffects)
        {
            if (dragDropEffects != this.dragDropEffects)
            {
                SendStatus(TargetWindow, dragDropEffects);//发送消息，确定可以接受数据
            }
            this.dragDropEffects = dragDropEffects;
        }

        Threading.DispatcherTimer timer;
        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            if (cancelHandle != null)
            {
                cancelHandle.Cancel = true;
                cancelHandle = null;
            }

        }
        public void DragEnter(ref XEvent xEvent, EventHandler eventHandler)
        {
            dataFormat = DataFormat.Unknown;
            text = null;
            html = null;
            fileNames = null;
            TargetWindow = xEvent.AnyEvent.window;
            SourceWindow = xEvent.ClientMessageEvent.ptr1;
            SendStatus(SourceWindow, DropEffects);//发送消息，确定可以接受数据

            //确认可以接受数据的格式
            foreach (IntPtr atom in SourceSupportedList(ref xEvent))
            {
                //Console.WriteLine("格式：" + XLib.GetAtomName(x11Info.Display, atom));
                var f = atomNonProtocols.FirstOrDefault(a => a.Item1 == atom);
                Console.WriteLine(XLib.GetAtomName(LinuxPlatform.Platform.Display, atom));
                if (f.Item1 == IntPtr.Zero || dataFormat.HasFlag(f.Item3))
                {
                    continue;
                }
                dataFormat |= f.Item3;
                XLib.XConvertSelection(x11Info.Display, x11Info.Atoms.XdndSelection, atom,
                   f.Item2, TargetWindow, IntPtr.Zero /* CurrentTime */);
            }

            if (cancelHandle != null)
            {
                cancelHandle.Cancel = true;
            }
            cancelHandle = new CancelHandle();
            LinuxPlatform.Platform.RunMainLoop(cancelHandle, OnAcceptEvent);
            if (cancelHandle != null)
            {
                if (timer == null)
                {
                    timer = new Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                    timer.Tick += eventHandler;
                }
                timer.Start();
            }
        }

        public bool OnAcceptEvent(ref XEvent xEvent)
        {
            if (xEvent.type == XEventName.SelectionNotify)
            {
                var df = dataFormat;
                while (true)
                {
                    if (df == DataFormat.Unknown)
                    {
                        break;
                    }
                    if (df.HasFlag(DataFormat.Text))
                    {
                        df ^= DataFormat.Text;
                        text = GetText(ref xEvent);
                    }
                    if (df.HasFlag(DataFormat.Html))
                    {
                        df ^= DataFormat.Html;
                        html = GetText(ref xEvent);
                    }
                    if (df.HasFlag(DataFormat.FileNames))
                    {
                        if (text == null)
                        {
                            text = GetText(ref xEvent);
                        }
                        df ^= DataFormat.FileNames;
                        List<string> uri_list = new List<string>();
                        string[] lines = text.Split(new char[] { '\r', '\n' });
                        foreach (string line in lines)
                        {
                            // # is a comment line (see RFC 2483)
                            if (line.StartsWith("#"))
                                continue;
                            try
                            {
                                Uri uri = new Uri(line);
                                uri_list.Add(uri.LocalPath);
                            }
                            catch { }
                        }

                        fileNames = uri_list;
                        //string[] l = (string[])uri_list.ToArray(typeof(string));
                    }
                }
                if (cancelHandle != null)
                {
                    cancelHandle.Cancel = true;
                }
                return true;
            }
            return false;
        }

        public unsafe string GetText(ref XEvent xevent)
        {
            int nread = 0;
            IntPtr nitems;
            IntPtr bytes_after;

            StringBuilder builder = new StringBuilder();
            do
            {
                IntPtr actual_type;
                int actual_fmt;
                IntPtr data = IntPtr.Zero;

                if (0 != XLib.XGetWindowProperty(x11Info.Display,
                            xevent.AnyEvent.window,
                            (IntPtr)xevent.SelectionEvent.property,
                            IntPtr.Zero, new IntPtr(0xffffff), false,
                            (IntPtr)Atom.AnyPropertyType, out actual_type,
                            out actual_fmt, out nitems, out bytes_after,
                            out data))
                {
                    XLib.XFree(data);
                    break;
                }

                //if (unicode)
                //    builder.Append(Marshal.PtrToStringUni(data));
                //else
                //    builder.Append(Marshal.PtrToStringAnsi(data));

                Encoding textEnc = GetStringEncoding(xevent.SelectionEvent.property);
                if (textEnc != null)
                {
                    var text = textEnc.GetString((byte*)data.ToPointer(), nitems.ToInt32());
                    builder.Append(text);
                }
                else
                {
                    builder.Append(Marshal.PtrToStringAnsi(data));
                }

                nread += nitems.ToInt32();

                XLib.XFree(data);
            } while (bytes_after.ToInt32() > 0);
            if (nread == 0)
                return null;
            return builder.ToString();
        }
        Encoding GetStringEncoding(IntPtr atom)
        {
            return (atom == x11Info.Atoms.XA_STRING || atom == x11Info.Atoms.OEMTEXT)
                ? Encoding.ASCII
                : (atom == x11Info.Atoms.UTF8_STRING || atom == textUtf8Atom)
                    ? Encoding.UTF8
                    : (atom == x11Info.Atoms.UTF16_STRING || atom == x11Info.Atoms.UNICODETEXT || atomNonProtocols.First(a => a.Item1 == htmlAtom).Item2 == atom)
                        ? Encoding.Unicode
                        : null;
        }

        public void SendStatus(IntPtr source, DragDropEffects effect)
        {
            XEvent xevent = new XEvent();

            xevent.AnyEvent.type = XEventName.ClientMessage;
            xevent.AnyEvent.display = x11Info.Display;
            xevent.ClientMessageEvent.window = source;
            xevent.ClientMessageEvent.message_type = x11Info.Atoms.XdndStatus;
            xevent.ClientMessageEvent.format = 32;
            xevent.ClientMessageEvent.ptr1 = TargetWindow;
            if (effect != DragDropEffects.None)
                xevent.ClientMessageEvent.ptr2 = (IntPtr)1;

            xevent.ClientMessageEvent.ptr5 = ActionFromEffect(effect);
            XLib.XSendEvent(x11Info.Display, source, false, IntPtr.Zero, ref xevent);
            //XLib.XSync(x11Info.Display,false);
            Console.WriteLine("state:" + effect);
        }

        private void GiveFeedback(IntPtr action)
        {
            //GiveFeedbackEventArgs gfe = new GiveFeedbackEventArgs(EffectFromAction(drag_data.Action), true);

            //Control c = MwfWindow(source);
            //c.DndFeedback(gfe);

            //if (gfe.UseDefaultCursors)
            {
                Cursor cursor = Cursors.No;
                if (WillAccept)
                {
                    // Same order as on MS
                    if (action == x11Info.Atoms.XdndActionCopy)
                        cursor = Cursors.DragCopy;
                    else if (action == x11Info.Atoms.XdndActionLink)
                        cursor = Cursors.DragLink;
                    else if (action == x11Info.Atoms.XdndActionMove)
                        cursor = Cursors.DragMove;
                }
                // TODO: Try not to set the cursor so much
                //if (cursor.Handle != CurrentCursorHandle) {
                XLib.XChangeActivePointerGrab(x11Info.Display,
                        EventMask.ButtonMotionMask |
                        EventMask.PointerMotionMask |
                        EventMask.ButtonPressMask |
                        EventMask.ButtonReleaseMask,
                        (IntPtr)cursor.PlatformCursor, IntPtr.Zero);
                //CurrentCursorHandle = cursor.Handle;
                //}
            }
        }

        public IntPtr[] SourceSupportedList(ref XEvent xevent)
        {
            IntPtr[] res;


            if (((int)xevent.ClientMessageEvent.ptr2 & 0x1) == 0)
            {
                res = new IntPtr[3];
                res[0] = xevent.ClientMessageEvent.ptr3;
                res[1] = xevent.ClientMessageEvent.ptr4;
                res[2] = xevent.ClientMessageEvent.ptr5;
            }
            else
            {
                IntPtr type;
                int format;
                IntPtr count;
                IntPtr remaining;
                IntPtr data;
                XLib.XGetWindowProperty(x11Info.Display, SourceWindow, x11Info.Atoms.XdndTypeList,
                        IntPtr.Zero, new IntPtr(32), false, (IntPtr)Atom.XA_ATOM,
                        out type, out format, out count,
                        out remaining, out data);

                res = new IntPtr[count.ToInt32()];
                for (int i = 0; i < count.ToInt32(); i++)
                {
                    res[i] = (IntPtr)Marshal.ReadInt32(data, i *
                            Marshal.SizeOf(typeof(IntPtr)));
                }

                XLib.XFree(data);
            }

            return res;
        }

        public void SetTextData(object data, ref XEvent xevent)
        {
            IntPtr buffer;
            int len;
            string str = data as string;

            //if (str == null)
            //{
            //    IDataObject dobj = data as IDataObject;
            //    if (dobj == null)
            //        return;
            //    str = (string)dobj.GetData("System.String", true);
            //}

            if (xevent.SelectionRequestEvent.target == (IntPtr)Atom.XA_STRING)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(str);
                buffer = Marshal.AllocHGlobal(bytes.Length);
                len = bytes.Length;
                for (int i = 0; i < len; i++)
                    Marshal.WriteByte(buffer, i, bytes[i]);
            }
            else if (xevent.SelectionRequestEvent.target == x11Info.Atoms.UTF8_STRING || xevent.SelectionRequestEvent.target == textUtf8Atom)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                buffer = Marshal.AllocHGlobal(bytes.Length);
                len = bytes.Length;
                for (int i = 0; i < len; i++)
                    Marshal.WriteByte(buffer, i, bytes[i]);
            }
            else
            {
                buffer = Marshal.StringToHGlobalAnsi(str);
                len = 0;
                while (Marshal.ReadByte(buffer, len) != 0)
                    len++;
            }

            SetProperty(ref xevent, buffer, len);

            Marshal.FreeHGlobal(buffer);
        }
        public void SetHtmlData(object data, ref XEvent xevent)
        {
            IntPtr buffer;
            int len;
            string str = data as string;

            if (str == null)
                return;

            if (xevent.SelectionRequestEvent.target == (IntPtr)Atom.XA_STRING)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(str);
                buffer = Marshal.AllocHGlobal(bytes.Length);
                len = bytes.Length;
                for (int i = 0; i < len; i++)
                    Marshal.WriteByte(buffer, i, bytes[i]);
            }
            else
            {
                buffer = Marshal.StringToHGlobalAnsi(str);
                len = 0;
                while (Marshal.ReadByte(buffer, len) != 0)
                    len++;
            }

            SetProperty(ref xevent, buffer, len);

            Marshal.FreeHGlobal(buffer);
        }

        public void SetFileNameData(object data, ref XEvent xevent)
        {
            var uri_list = data as IEnumerable<string>;

            //if (uri_list == null)
            //{
            //    IDataObject dobj = data as IDataObject;
            //    if (dobj == null)
            //        return;
            //    uri_list = dobj.GetData(DataFormats.FileDrop, true) as string[];
            //}

            if (uri_list == null)
                return;

            StringBuilder res = new StringBuilder();
            foreach (string uri_str in uri_list)
            {
                Uri uri = new Uri(uri_str);
                res.Append(uri.ToString());
                res.Append("\r\n");
            }

            IntPtr buffer = Marshal.StringToHGlobalAnsi((string)res.ToString());
            int len = 0;
            while (Marshal.ReadByte(buffer, len) != 0)
                len++;

            SetProperty(ref xevent, buffer, len);
        }
        private void SetProperty(ref XEvent xevent, IntPtr data, int length)
        {
            XEvent sel = new XEvent();
            sel.SelectionEvent.type = XEventName.SelectionNotify;
            sel.SelectionEvent.send_event = true;
            sel.SelectionEvent.display = x11Info.Display;
            sel.SelectionEvent.selection = xevent.SelectionRequestEvent.selection;
            sel.SelectionEvent.target = xevent.SelectionRequestEvent.target;
            sel.SelectionEvent.requestor = xevent.SelectionRequestEvent.requestor;
            sel.SelectionEvent.time = xevent.SelectionRequestEvent.time;
            sel.SelectionEvent.property = xevent.SelectionRequestEvent.property;

            XLib.XChangeProperty(x11Info.Display, xevent.SelectionRequestEvent.requestor,
                    xevent.SelectionRequestEvent.property,
                    xevent.SelectionRequestEvent.target,
                    8, PropertyMode.Replace, data, length);

            XLib.XSendEvent(x11Info.Display, xevent.SelectionRequestEvent.requestor, true,
                    (IntPtr)EventMask.NoEventMask, ref sel);
            return;
        }

        public bool HandleMouseOver()
        {
            IntPtr toplevel, window;
            int x_root, y_root;

            GetWindowsUnderPointer(out window, out toplevel, out x_root, out y_root);

            if (window != LastWindow && State == DragState.Entered)
            {
                State = DragState.Dragging;

                // TODO: Send a Leave if this is an MWF window

                if (toplevel != TargetWindow)
                    SendLeave(TargetWindow, toplevel);
            }

            State = DragState.Entered;
            if (toplevel != TargetWindow)
            {
                if (cancelHandle == null || cancelHandle.Cancel)
                {
                    return true;
                }
                // Entering a new toplevel window
                SendEnter(toplevel, SourceWindow, SupportedTypes);
            }
            else
            {
                // Already in a toplevel window, so send a position
                SendPosition(toplevel, SourceWindow,
                        Action,
                        x_root, y_root,
                        IntPtr.Zero);
            }

            TargetWindow = toplevel;
            LastWindow = window;
            return true;
        }
        void GetWindowsUnderPointer(out IntPtr window, out IntPtr toplevel, out int x_root, out int y_root)
        {
            toplevel = IntPtr.Zero;
            window = x11Info.RootWindow;

            IntPtr root, child;
            bool dnd_aware = false;
            int x_temp, y_temp;
            int mask_return;
            int x = x_root = (int)point.X;
            int y = y_root = (int)point.Y;

            while (XLib.XQueryPointer(x11Info.Display, window, out root, out child,
                           out x_temp, out y_temp, out x, out y, out mask_return))
            {

                if (!dnd_aware)
                {
                    dnd_aware = IsWindowDndAware(window);
                    if (dnd_aware)
                    {
                        toplevel = window;
                        x_root = x_temp;
                        y_root = y_temp;
                    }
                }

                if (child == IntPtr.Zero)
                    break;

                window = child;
            }
        }


        private void SendDrop(IntPtr handle, IntPtr from, IntPtr time)
        {
            XEvent xevent = new XEvent();

            xevent.AnyEvent.type = XEventName.ClientMessage;
            xevent.AnyEvent.display = x11Info.Display;
            xevent.ClientMessageEvent.window = handle;
            xevent.ClientMessageEvent.message_type = x11Info.Atoms.XdndDrop;
            xevent.ClientMessageEvent.format = 32;
            xevent.ClientMessageEvent.ptr1 = from;
            xevent.ClientMessageEvent.ptr3 = time;

            XLib.XSendEvent(x11Info.Display, handle, false, IntPtr.Zero, ref xevent);
            //dropped = true;
        }

        private void SendPosition(IntPtr handle, IntPtr from, IntPtr action, int x, int y, IntPtr time)
        {
            XEvent xevent = new XEvent();

            xevent.AnyEvent.type = XEventName.ClientMessage;
            xevent.AnyEvent.display = x11Info.Display;
            xevent.ClientMessageEvent.window = handle;
            xevent.ClientMessageEvent.message_type = x11Info.Atoms.XdndPosition;
            xevent.ClientMessageEvent.format = 32;
            xevent.ClientMessageEvent.ptr1 = from;
            xevent.ClientMessageEvent.ptr3 = (IntPtr)((x << 16) | (y & 0xFFFF));
            xevent.ClientMessageEvent.ptr4 = time;
            xevent.ClientMessageEvent.ptr5 = action;

            XLib.XSendEvent(x11Info.Display, handle, false, IntPtr.Zero, ref xevent);
        }

        private void SendLeave(IntPtr handle, IntPtr from)
        {
            XEvent xevent = new XEvent();

            xevent.AnyEvent.type = XEventName.ClientMessage;
            xevent.AnyEvent.display = x11Info.Display;
            xevent.ClientMessageEvent.window = handle;
            xevent.ClientMessageEvent.message_type = x11Info.Atoms.XdndLeave;
            xevent.ClientMessageEvent.format = 32;
            xevent.ClientMessageEvent.ptr1 = from;

            XLib.XSendEvent(x11Info.Display, handle, false, IntPtr.Zero, ref xevent);
        }

        public void SendFinished()
        {
            XEvent xevent = new XEvent();

            xevent.AnyEvent.type = XEventName.ClientMessage;
            xevent.AnyEvent.display = x11Info.Display;
            xevent.ClientMessageEvent.window = SourceWindow;
            xevent.ClientMessageEvent.message_type = x11Info.Atoms.XdndFinished;
            xevent.ClientMessageEvent.format = 32;
            xevent.ClientMessageEvent.ptr1 = TargetWindow;

            XLib.XSendEvent(x11Info.Display, SourceWindow, false, IntPtr.Zero, ref xevent);
            XLib.XFlush(x11Info.Display);
        }

        private bool IsWindowDndAware(IntPtr handle)
        {
            bool res = true;
            // Check the version number, we need greater than 3
            IntPtr actual;
            int format;
            IntPtr count;
            IntPtr remaining;
            IntPtr data = IntPtr.Zero;

            XLib.XGetWindowProperty(x11Info.Display, handle, x11Info.Atoms.XdndAware, IntPtr.Zero, new IntPtr(0x8000000), false,
                    (IntPtr)Atom.XA_ATOM, out actual, out format,
                    out count, out remaining, out data);

            if (actual != (IntPtr)Atom.XA_ATOM || format != 32 ||
                    count.ToInt32() == 0 || data == IntPtr.Zero)
            {
                if (data != IntPtr.Zero)
                    XLib.XFree(data);
                return false;
            }

            int version = Marshal.ReadInt32(data, 0);

            if (version < 3)
            {
                Console.Error.WriteLine("XDND Version too old (" + version + ").");
                XLib.XFree(data);
                return false;
            }

            // First type is actually the XDND version
            if (count.ToInt32() > 1)
            {
                res = false;
                for (int i = 1; i < count.ToInt32(); i++)
                {
                    IntPtr type = (IntPtr)Marshal.ReadInt32(data, i *
                            Marshal.SizeOf(typeof(int)));
                    for (int j = 0; j < SupportedTypes.Length; j++)
                    {
                        if (SupportedTypes[j] == type)
                        {
                            res = true;
                            break;
                        }
                    }
                }
            }

            XLib.XFree(data);
            return res;
        }
        private enum DragState
        {
            None,
            Beginning,
            Dragging,
            Entered
        }

    }
}
