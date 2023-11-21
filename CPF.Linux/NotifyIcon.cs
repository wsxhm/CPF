using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using static CPF.Linux.XLib;

namespace CPF.Linux
{
    class NotifyIcon : XWindow, INotifyIconImpl
    {
        public NotifyIcon()
        {
            DisplayHandle = LinuxPlatform.Platform.Display;
        }
        LinuxPlatform x11info;

        XSetWindowAttributes attr = new XSetWindowAttributes();
        protected override void OnCreateWindw()
        {
            x11info = LinuxPlatform.Platform;
            var valueMask = default(SetWindowValuemask);
            attr.background_pixel = IntPtr.Zero;
            attr.border_pixel = IntPtr.Zero;
            attr.backing_store = 1;
            attr.bit_gravity = Gravity.NorthWestGravity;
            attr.win_gravity = Gravity.NorthWestGravity;
            attr.override_redirect = true;
            valueMask |= SetWindowValuemask.BackPixel | SetWindowValuemask.BorderPixel | SetWindowValuemask.BackingStore | SetWindowValuemask.OverrideRedirect
                         //| SetWindowValuemask.BackPixmap 
                         | SetWindowValuemask.BitGravity | SetWindowValuemask.WinGravity;
            valueMask |= SetWindowValuemask.SaveUnder;

            attr.colormap = XLib.XCreateColormap(x11info.Info.Display, x11info.Info.RootWindow, x11info.Info.TransparentVisualInfo.visual, 0);
            valueMask |= SetWindowValuemask.ColorMap;

            Handle = XLib.XCreateWindow(x11info.Display, x11info.Info.DefaultRootWindow, 0, 0, 16, 16, 0, 32, (int)CreateWindowArgs.InputOutput, x11info.Info.TransparentVisualInfo.visual, new UIntPtr((uint)valueMask), ref attr);
            //Handle= XCreateSimpleWindow(x11info.Display, x11info.Info.DefaultRootWindow, 0, 0, 16, 16, 0, IntPtr.Zero, XWhitePixel(x11info.Display,x11info.Info.DefaultScreen)); 

            XEventMask ignoredMask = XEventMask.SubstructureRedirectMask | XEventMask.ResizeRedirectMask | XEventMask.PointerMotionHintMask;
            var mask = new IntPtr(0xffffff ^ (int)ignoredMask);
            XLib.XSelectInput(x11info.Display, Handle, mask);
            EventAction = OnXEvent;
            popup.Children.Add(textBlock);
        }
        Bitmap bitmap;
        void OnXEvent(ref XEvent ev)
        {
            if (ev.type == XEventName.Expose)
            {
                XWindowAttributes Attr = new XWindowAttributes();
                XGetWindowAttributes(DisplayHandle, Handle, ref Attr);

                if (icon != null)
                {
                    if (bitmap == null || bitmap.Width != Attr.width || bitmap.Height != Attr.height)
                    {
                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                        }
                        bitmap = new Bitmap(Attr.width, Attr.height);
                    }
                    using (var dc = DrawingContext.FromBitmap(bitmap))
                    {
                        dc.AntialiasMode = AntialiasMode.AntiAlias;
                        dc.Clear(Color.Transparent);
                        dc.DrawImage(icon, new Rect(0, 0, bitmap.Width, bitmap.Height), new Rect(0, 0, icon.Width, icon.Height));
                    }
                }
                if (bitmap != null)
                {
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

                        XPutImage(x11info.Display, Handle, gc, ref img, 0, 0, 0, 0, (uint)Attr.width, (uint)Attr.height);
                        //XSync(x11info.Display, false);
                        //XUnlockDisplay(x11info.Display);
                        XFreeGC(x11info.Display, gc);
                        //XFlush(x11info.Display);
                        //Console.WriteLine(Attr.width + "," + Attr.height);
                    }
                }
            }
            else if (ev.type == XEventName.ButtonPress)
            {
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
                    if (MouseDown != null)
                    {
                        MouseDown(this, new NotifyIconMouseEventArgs(mouseButton));
                    }
                    if (mouseButton == MouseButton.Left)
                    {
                        var time = DateTime.Now;
                        if (mouseDownTime.HasValue)
                        {
                            if (time - mouseDownTime <= LinuxPlatform.Platform.DoubleClickTime)
                            {
                                if (DoubleClick != null)
                                {
                                    DoubleClick(this, EventArgs.Empty);
                                }
                                mouseDownTime = null;
                            }
                            else
                            {
                                mouseDownTime = time;
                            }
                        }
                        else
                        {
                            mouseDownTime = time;
                        }
                    }
                }
            }
            else if (ev.type == XEventName.ButtonRelease)
            {
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
                    if (MouseUp != null)
                    {
                        MouseUp(this, new NotifyIconMouseEventArgs(mouseButton));
                    }
                    if (mouseButton == MouseButton.Left)
                    {
                        if (Click != null)
                        {
                            Click(this, EventArgs.Empty);
                        }
                    }
                }
            }
            else if (ev.type == XEventName.MotionNotify)
            {
                if (!isMouseEnter)
                {
                    isMouseEnter = true;
                    if (!string.IsNullOrWhiteSpace(Text))
                    {
                        textBlock.Text = Text;
                        popup.Placement = PlacementMode.Absolute;
                        var p = MouseDevice.Location;
                        popup.MarginLeft = p.X / popup.LayoutScaling + 10;
                        if (p.X < 100)
                        {
                            popup.MarginTop = p.Y / popup.LayoutScaling + 10;
                        }
                        else
                        {
                            popup.MarginTop = p.Y / popup.LayoutScaling - 30;
                        }
                        popup.LoadStyle(Window.Windows.FirstOrDefault(a => a.IsMain));
                        popup.Show();
                    }
                }
            }
            else if (ev.type == XEventName.LeaveNotify)
            {
                isMouseEnter = false;
                if (popup.Visibility == Visibility.Visible)
                {
                    popup.Width = "auto";
                    popup.Height = "auto";
                    popup.Hide();
                }
            }
        }
        TextBlock textBlock = new TextBlock { Margin = "2" };
        Popup popup = new Popup
        {
            CanActivate = false,
            StaysOpen = false,
            BorderFill = "#aaa",
            Background = "#fff",
            BorderStroke = "1",
        };
        bool isMouseEnter;
        IntPtr DisplayHandle;
        IntPtr SystrayMgrWindow;

        public string Text { get; set; }
        Image icon;
        public Image Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                Invalidate();
            }
        }

        private void Invalidate()
        {
            XWindowAttributes Attr = new XWindowAttributes();
            XGetWindowAttributes(DisplayHandle, Handle, ref Attr);
            var xev = new XEvent
            {
                ExposeEvent =
                    {
                        type = XEventName.Expose,
                        send_event = true,
                        window = Handle,
                        count=1,
                        display=x11info.Display,
                        height=Attr.height,
                        width= Attr.width,
                    }
            };
            lock (XlibLock)
            {
                XSendEvent(x11info.Display, Handle, false,
                    new IntPtr((int)(EventMask.ExposureMask)), ref xev);
            }
        }

        //public ContextMenu ContextMenu { get; set; }

        bool visible;
        DateTime? mouseDownTime;
        public event EventHandler Click;
        public event EventHandler DoubleClick;
        public event EventHandler<NotifyIconMouseEventArgs> MouseDown;
        public event EventHandler<NotifyIconMouseEventArgs> MouseUp;

        bool maped;
        public bool Visible
        {
            get { return visible; }
            set
            {
                //System.Threading.Thread.Sleep(10000);
                visible = value;
                if (visible)
                {
                    XGrabServer(DisplayHandle);
                    SystrayMgrWindow = XGetSelectionOwner(DisplayHandle, LinuxPlatform.Platform.Info.Atoms._NET_SYSTEM_TRAY_S);
                    XUngrabServer(DisplayHandle);
                    XFlush(DisplayHandle);


                    if (SystrayMgrWindow != IntPtr.Zero)
                    {
                        //XSelectInput(DisplayHandle, SystrayMgrWindow, (IntPtr)XEventMask.StructureNotifyMask);

                        XSizeHints size_hints;

                        // We are going to be directly mapped by the system tray, so mark as mapped
                        // so we can later properly unmap it.
                        //XWindowAttributes attributes = new XWindowAttributes();
                        //XGetWindowAttributes(x11info.Display, XDefaultRootWindow(x11info.Display), ref attributes);
                        //Console.WriteLine(attributes.width + "," + attributes.height);

                        size_hints = new XSizeHints();

                        size_hints.flags = (IntPtr)(XSizeHintsFlags.PMinSize | XSizeHintsFlags.PMaxSize | XSizeHintsFlags.PBaseSize);

                        size_hints.min_width = 16;
                        size_hints.min_height = 16;
                        size_hints.max_width = 16;
                        size_hints.max_height = 16;
                        size_hints.base_width = 16;
                        size_hints.base_height = 16;

                        XSetWMNormalHints(DisplayHandle, Handle, ref size_hints);

                        IntPtr[] atoms = new IntPtr[2];
                        atoms[0] = (IntPtr)1;           // Version 1
                        atoms[1] = (IntPtr)1;           // we want to be mapped

                        XChangeProperty(DisplayHandle, Handle, LinuxPlatform.Platform.Info.Atoms._XEMBED_INFO, LinuxPlatform.Platform.Info.Atoms._XEMBED_INFO, 32, PropertyMode.Replace, atoms, 2);

                        SendNetClientMessage(SystrayMgrWindow, LinuxPlatform.Platform.Info.Atoms._NET_SYSTEM_TRAY_OPCODE, IntPtr.Zero, (IntPtr)SystrayRequest.SYSTEM_TRAY_REQUEST_DOCK, Handle);
                        //XReparentWindow(DisplayHandle, Handle, SystrayMgrWindow, 0, 0);
                        Invalidate();
                        if (maped)
                        {
                            XMapWindow(x11info.Display, Handle);
                        }
                        maped = true;
                    }
                }
                else
                {
                    XUnmapWindow(DisplayHandle, Handle);
                }
            }
        }

        void SendNetClientMessage(IntPtr window, IntPtr message_type, IntPtr l0, IntPtr l1, IntPtr l2)
        {
            XEvent xev;

            xev = new XEvent();
            xev.ClientMessageEvent.type = XEventName.ClientMessage;
            xev.ClientMessageEvent.send_event = true;
            xev.ClientMessageEvent.window = window;
            xev.ClientMessageEvent.message_type = message_type;
            xev.ClientMessageEvent.format = 32;
            xev.ClientMessageEvent.ptr1 = l0;
            xev.ClientMessageEvent.ptr2 = l1;
            xev.ClientMessageEvent.ptr3 = l2;
            XSendEvent(DisplayHandle, window, false, new IntPtr((int)EventMask.NoEventMask), ref xev);
            //XSync(DisplayHandle, false);
        }

        protected override void Dispose(bool disposing)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            base.Dispose(disposing);
        }

    }
}
