using CPF.Controls;
using CPF.Drawing;
using CPF.Mac.AppKit;
using CPF.Mac.CoreGraphics;
using CPF.Mac.ObjCRuntime;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Mac
{
    public class NativeHost : NSView, INativeImpl
    {
        public NativeHost()
        {
            //WantsLayer = true;
            //Layer.MasksToBounds = true;
        }

        Color backcolor;
        public void SetBackColor(Color color)
        {
            //WantsLayer = true;
            backcolor = color;
            SetNeedsDisplayInRect(new CGRect(0, 0, Frame.Width, Frame.Height));
            //base.Layer.BackgroundColor = new CGColor(backcolor.R / 255.0, backcolor.G / 255.0, backcolor.B / 255.0, backcolor.A / 255.0);
        }

        public override void DrawRect(CGRect dirtyRect)
        {
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SetFillColor(new CGColor(backcolor.R / 255.0, backcolor.G / 255.0, backcolor.B / 255.0, backcolor.A / 255.0));
            //if (clipRect.HasValue && parent != null)
            //{
            //var f = parent.Frame;
            //var rect = new Rect((float)dirtyRect.X, (float)(f.Height - dirtyRect.Y), (float)dirtyRect.Width, (float)dirtyRect.Height);
            //rect.Intersect(clipRect.Value);
            context.FillRect(new CGRect(0, 0, Frame.Width, Frame.Height));
            //}
        }

        public override bool WantsDefaultClipping => true;

        object INativeImpl.Handle => Handle;

        Rect? clipRect;
        long? rectId;
        public void SetBounds(Rect renderRect, Rect clip, bool visible)
        {
            if (visible)
            {
                //WantsLayer = true;
                //Layer.MasksToBounds = true;
                var bounds = new CGRect((renderRect.Left * parent.LayoutScaling), ((parent.root.ActualSize.Height - renderRect.Bottom) * parent.LayoutScaling), (renderRect.Width * parent.LayoutScaling), (renderRect.Height * parent.LayoutScaling));
                if (!float.IsInfinity(clip.Width) && !float.IsInfinity(clip.Height) && clipRect != clip)
                {
                    clipRect = clip;
                    //var rgn = CreateRectRgn((int)(rect.X * parent.RenderScaling), (int)(rect.Y * parent.RenderScaling), (int)(rect.Right * parent.RenderScaling), (int)(rect.Bottom * parent.RenderScaling));

                    //base.DisplayRect(new CGRect((clip.X * parent.LayoutScaling), ((parent.root.ActualSize.Height - clip.Bottom) * parent.LayoutScaling), (clip.Width * parent.LayoutScaling), (clip.Height * parent.LayoutScaling)));
                    //if (rectId.HasValue)
                    //{
                    //    base.RemoveTrackingRect(rectId.Value);
                    //}
                    //rectId = base.AddTrackingRect(bounds, this, IntPtr.Zero, true);
                }
                base.Hidden = false;
                //bounds = new CGRect(0, 0, 300, 300);
                base.Frame = bounds;
                //Console.WriteLine(Frame + "    " + renderRect + " -- -- " + clip + "     " + parent.LayoutScaling);
            }
            else
            {
                base.Hidden = true;
            }
        }

        object contentHandle;
        public void SetContent(object content)
        {
            if (content != null)
            {
                if (content is IntPtr intPtr)
                {
                    if (intPtr != IntPtr.Zero)
                    {
                        IntPtr selAddSubview_Handle = (IntPtr)typeof(NSView).GetField("selAddSubview_Handle", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
                        if (IsDirectBinding)
                        {
                            Messaging.void_objc_msgSend_IntPtr(base.Handle, selAddSubview_Handle, intPtr);
                        }
                        else
                        {
                            Messaging.void_objc_msgSendSuper_IntPtr(base.SuperHandle, selAddSubview_Handle, intPtr);
                        }
                    }
                }
                else if (content is NSView view)
                {
                    AddSubview(view);
                    //base.DocumentView = view;
                }
                else
                {
                    throw new Exception("必须是控件句柄IntPtr或者NSView类型");
                }
            }
            else
            {
                if (contentHandle is IntPtr intPtr)
                {
                    IntPtr selWillRemoveSubview_Handle = (IntPtr)typeof(NSView).GetField("selWillRemoveSubview_Handle", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
                    if (IsDirectBinding)
                    {
                        Messaging.void_objc_msgSend_IntPtr(base.Handle, selWillRemoveSubview_Handle, intPtr);
                    }
                    else
                    {
                        Messaging.void_objc_msgSendSuper_IntPtr(base.SuperHandle, selWillRemoveSubview_Handle, intPtr);
                    }
                }
                else if (contentHandle is NSView view)
                {
                    WillRemoveSubview(view);
                    //base.DocumentView = null;
                }
            }
            contentHandle = content;
        }
        UIElement owner;
        public void SetOwner(NativeElement owner)
        {
            this.owner = owner;
        }

        WindowImpl parent;
        public void SetParent(IViewImpl parent)
        {
            if (parent is WindowImpl window)
            {
                window.view.AddSubview(this);
            }
            else
            {
                if (this.parent != null)
                {
                    this.parent.view.WillRemoveSubview(this);
                }
            }
            this.parent = parent as WindowImpl;
        }
    }
}
