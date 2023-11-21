using CPF.Drawing;
using CPF.Input;
using CPF.Mac.AppKit;
using CPF.Mac.CoreGraphics;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Mac.Foundation;
using CPF.Controls;

namespace CPF.Mac
{
    public class NotifyIconImpl : NSObject, INotifyIconImpl
    {
        NSStatusItem item;
        public NotifyIconImpl()
        {
            item = NSStatusBar.SystemStatusBar.CreateStatusItem((double)NSStatusItemLength.Variable);
            item.Visible = false;
            item.Target = this;
            item.Action = new ObjCRuntime.Selector("onAction:");
            item.DoubleAction = new ObjCRuntime.Selector("onDoubleClick:");
            item.SendActionOn(NSTouchPhase.Any);
            //https://www.javaroad.cn/questions/147062
        }

        MouseButton mousebutton = MouseButton.None;
        [Export("onAction:")]
        protected void OnAction(NSObject sender)
        {
            var button = NSEvent.CurrentPressedMouseButtons;
            if (button == 1)
            {
                if (mousebutton == MouseButton.None)
                {
                    mousebutton = MouseButton.Left;
                    MouseDown?.Invoke(this, new NotifyIconMouseEventArgs(mousebutton));
                }
            }
            else if (button == 2)
            {
                if (mousebutton == MouseButton.None)
                {
                    mousebutton = MouseButton.Right;
                    MouseDown?.Invoke(this, new NotifyIconMouseEventArgs(mousebutton));
                }
            }
            else if (button == 0)
            {
                if (mousebutton != MouseButton.None)
                {//激活程序才能使用右键菜单
                    MacPlatform.Application.ActivateIgnoringOtherApps(true);
                    MouseUp?.Invoke(this, new NotifyIconMouseEventArgs(mousebutton));
                    mousebutton = MouseButton.None;
                }
            }
        }

        [Export("onDoubleClick:")]
        protected void onDoubleClick(NSObject sender)
        {
            DoubleClick?.Invoke(this, EventArgs.Empty);
        }

        private string text = "";
        public string Text
        {
            get => text; set
            {
                text = value;
                item.ToolTip = text;
            }
        }
        Image icon;
        public Image Icon
        {
            get => icon; set
            {
                icon = value;
                if (value != null)
                {
                    CGSize size;
                    var originalSize = new Size(icon.Width, icon.Height);
                    size.Height = NSFont.MenuFontOfSize(0).PointSize * 1.333333;

                    var scaleFactor = size.Height / originalSize.Height;
                    size.Width = originalSize.Width * scaleFactor;
                    var stream = icon.SaveToStream(ImageFormat.Png);
                    stream.Position = 0;
                    var img = NSImage.FromStream(stream);
                    img.Size = size;
                    item.Image = img;
                }
                else
                {
                    item.Image = null;
                }

            }
        }
        bool visible;
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                item.Visible = value;
            }
        }

        public event EventHandler Click;
        public event EventHandler DoubleClick;
        public event EventHandler<NotifyIconMouseEventArgs> MouseDown;
        public event EventHandler<NotifyIconMouseEventArgs> MouseUp;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (item != null)
            {
                NSStatusBar.SystemStatusBar.RemoveStatusItem(item);
                item.Dispose();
                item = null;
            }
        }
    }

}
