using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using CPF.Drawing;

namespace CPF.Input
{
    public class MouseEventArgs : InputEventArgs
    {
        public MouseEventArgs(InputDevice device, UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, bool isTouch = false) : base(device)
        {
            this.OriginalSource = source;
            //this.Type = type;
            this.leftButton = LeftButtonDown ? MouseButtonState.Pressed : MouseButtonState.Released;
            this.rightButton = RightButtonDown ? MouseButtonState.Pressed : MouseButtonState.Released;
            this.middleButton = MiddleButtonDown ? MouseButtonState.Pressed : MouseButtonState.Released;
            //this.xButton1 = xButton1;
            //this.xButton2 = xButton2;
            this.location = location;
            IsTouch = isTouch;
        }
        public MouseEventArgs(InputDevice device, UIElement source, MouseButtonState LeftButtonDown, MouseButtonState RightButtonDown, MouseButtonState MiddleButtonDown, Point location, bool isTouch = false) : base(device)
        {
            this.OriginalSource = source;
            //this.Type = type;
            this.leftButton = LeftButtonDown;
            this.rightButton = RightButtonDown;
            this.middleButton = MiddleButtonDown;
            //this.xButton1 = xButton1;
            //this.xButton2 = xButton2;
            this.location = location;
            IsTouch = isTouch;
        }
        public MouseEventArgs(UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, MouseDevice mouseDevice, bool isTouch = false) : this(mouseDevice, source, LeftButtonDown, RightButtonDown, MiddleButtonDown, location, isTouch)
        {

        }
        /// <summary>
        /// 是否来自屏幕触摸
        /// </summary>
        public bool IsTouch { get; private set; }

        MouseButtonState leftButton;
        MouseButtonState rightButton;
        MouseButtonState middleButton;
        //MouseButtonState xButton1;
        //MouseButtonState xButton2;
        Point location;

        public MouseButtonState LeftButton
        {
            get
            {
                return leftButton;
            }
        }

        public MouseButtonState RightButton
        {
            get
            {
                return rightButton;
            }
        }

        public MouseButtonState MiddleButton
        {
            get
            {
                return middleButton;
            }
        }

        public MouseDevice MouseDevice
        {
            get { return (MouseDevice)this.Device; }
        }

        //public MouseButtonState XButton1
        //{
        //    get
        //    {
        //        return xButton1;
        //    }
        //}

        //public MouseButtonState XButton2
        //{
        //    get
        //    {
        //        return xButton2;
        //    }
        //}

        public Point Location
        {
            get
            {
                return location;
            }
            internal set { location = value; }
        }

        //protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        //{
        //    EventHandler<MouseEventArgs> h = genericHandler as EventHandler<MouseEventArgs>;
        //    if (h != null)
        //    {
        //        h(genericTarget, this);
        //    }
        //    else
        //    {
        //        base.InvokeEventHandler(genericHandler, genericTarget);
        //    }
        //}
    }
}
