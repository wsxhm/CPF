using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Linux
{
    public class NativeHost : XWindow, INativeImpl
    {
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
            valueMask |= SetWindowValuemask.BackPixel | SetWindowValuemask.BorderPixel | SetWindowValuemask.BackingStore | SetWindowValuemask.OverrideRedirect
                         //| SetWindowValuemask.BackPixmap 
                         | SetWindowValuemask.BitGravity | SetWindowValuemask.WinGravity;
            attr.colormap = XLib.XCreateColormap(x11info.Info.Display, x11info.Info.RootWindow, x11info.Info.TransparentVisualInfo.visual, 0);
            valueMask |= SetWindowValuemask.ColorMap;

            Handle = XLib.XCreateWindow(x11info.Display, x11info.Info.DefaultRootWindow, 0, 0, 100, 100, 0, 32, (int)CreateWindowArgs.InputOutput, x11info.Info.TransparentVisualInfo.visual, new UIntPtr((uint)valueMask), ref attr);

            XEventMask ignoredMask = XEventMask.SubstructureRedirectMask | XEventMask.ResizeRedirectMask | XEventMask.PointerMotionHintMask;
            var mask = new IntPtr(0xffffff ^ (int)ignoredMask);
            XLib.XSelectInput(x11info.Display, Handle, mask);
            EventAction = OnXEvent;
        }
        PixelSize size;
        void OnXEvent(ref XEvent ev)
        {
            if (ev.type == XEventName.Expose)
            {
                var gc = XLib.XCreateGC(x11info.Display, Handle, 0, IntPtr.Zero);
                XColor xcolor = new XColor();

                xcolor.red = (ushort)(backcolor.R * 257);
                xcolor.green = (ushort)(backcolor.G * 257);
                xcolor.blue = (ushort)(backcolor.B * 257);
                XLib.XAllocColor(x11info.Display, attr.colormap, ref xcolor);
                XLib.XSetBackground(x11info.Display, gc, xcolor.pixel);
                XLib.XSetForeground(x11info.Display, gc, xcolor.pixel);
                XLib.XFillRectangle(x11info.Display, Handle, gc, 0, 0, size.Width, size.Height);
                XLib.XFreeGC(x11info.Display, gc);
            }
            else if (ev.type == XEventName.ConfigureNotify)
            {
                size = new PixelSize(ev.ConfigureEvent.width, ev.ConfigureEvent.height);
            }
        }
        Color backcolor;
        public void SetBackColor(Color color)
        {
            backcolor = color;
            Invalidate();
            //var gc = XLib.XCreateGC(x11info.Display, Handle, 0, IntPtr.Zero);
            //XColor xcolor = new XColor();

            //xcolor.red = (ushort)(color.R * 257);
            //xcolor.green = (ushort)(color.G * 257);
            //xcolor.blue = (ushort)(color.B * 257);
            //XLib.XAllocColor(x11info.Display, x11info.Info.DefaultColormap, ref xcolor);
            //XLib.XSetBackground(x11info.Display, gc, xcolor.pixel);
            //XLib.XFreeGC(x11info.Display, gc);
        }

        void Invalidate()
        {
            lock (XlibLock)
            {
                var xev = new XEvent
                {
                    ExposeEvent =
                    {
                        type = XEventName.Expose,
                        send_event = true,
                        window = Handle,
                        count=1,
                        display=x11info.Display,
                        height=size.Height,
                        width= size.Width,
                        x=0,
                        y= 0
                    }
                };
                XLib.XSendEvent(x11info.Display, Handle, false,
                    new IntPtr((int)(EventMask.ExposureMask)), ref xev);
            }
        }

        Rect? clipRect;
        Rect? renderRect;
        bool visible = false;
        public void SetBounds(Rect renderRect, Rect rect, bool visible)
        {
            if (visible)
            {
                if (!float.IsInfinity(rect.Width) && !float.IsInfinity(rect.Height) && clipRect != rect)
                {
                    clipRect = rect;
                    var rects = new XRectangle[] { new XRectangle { x = (short)(rect.X * parent.RenderScaling), y = (short)(rect.Y * parent.RenderScaling), width = (ushort)(rect.Width * parent.RenderScaling), height = (ushort)(rect.Height * parent.RenderScaling) } };
                    //Console.WriteLine(rect);
                    XLib.XShapeCombineRectangles(x11info.Display, Handle, XShapeKind.ShapeBounding, 0, 0, rects, rects.Length, XShapeOperation.ShapeSet, XOrdering.Unsorted);
                }
                if (this.renderRect != renderRect)
                {
                    this.renderRect = renderRect;
                    XLib.XMoveResizeWindow(x11info.Display, Handle, (int)(renderRect.X * parent.RenderScaling), (int)(renderRect.Y * parent.RenderScaling), (int)(renderRect.Width * parent.RenderScaling), (int)(renderRect.Height * parent.RenderScaling));
                }
                var gc = XLib.XCreateGC(x11info.Display, Handle, 0, IntPtr.Zero);
                XColor xcolor = new XColor();
                xcolor.red = (ushort)(backcolor.R * 257);
                xcolor.green = (ushort)(backcolor.G * 257);
                xcolor.blue = (ushort)(backcolor.B * 257);
                XLib.XAllocColor(x11info.Display, attr.colormap, ref xcolor);
                XLib.XSetBackground(x11info.Display, gc, xcolor.pixel);
                XLib.XSetForeground(x11info.Display, gc, xcolor.pixel);
                XLib.XFillRectangle(x11info.Display, Handle, gc, 0, 0, size.Width, size.Height);
                XLib.XFreeGC(x11info.Display, gc);
                //Invalidate();
                if (this.visible != visible)
                {
                    XLib.XMapWindow(x11info.Display, Handle);
                }
            }
            else
            {
                XLib.XUnmapWindow(x11info.Display, Handle);
            }
            this.visible = visible;
        }

        IntPtr contentHandle;
        public void SetContent(object content)
        {
            if (content != null)
            {
                if (content is IntPtr intPtr)
                {
                    contentHandle = intPtr;
                    if (contentHandle != IntPtr.Zero)
                    {
                        XLib.XReparentWindow(x11info.Display, intPtr, Handle, 0, 0);
                    }
                }
                else
                {
                    throw new Exception("必须是控件句柄IntPtr类型");
                }
            }
            else
            {
                if (contentHandle != IntPtr.Zero)
                {
                    XLib.XReparentWindow(x11info.Display, contentHandle, x11info.Info.DefaultRootWindow, 0, 0);
                }
                contentHandle = IntPtr.Zero;
            }
        }

        UIElement owner;
        public void SetOwner(NativeElement owner)
        {
            this.owner = owner;
        }
        X11Window parent;

        object INativeImpl.Handle => Handle;

        public void SetParent(IViewImpl parent)
        {
            if (parent is X11Window window)
            {
                XLib.XReparentWindow(x11info.Display, Handle, window.Handle, 0, 0);
            }
            else
            {
                XLib.XReparentWindow(x11info.Display, Handle, x11info.Info.DefaultRootWindow, 0, 0);
            }
            this.parent = parent as X11Window;
        }
    }
}
