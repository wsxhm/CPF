using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class MouseWheelEventArgs : MouseEventArgs
    {
        public MouseWheelEventArgs(UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, MouseDevice mouseDevice, Vector delta, bool isTouch = false) : base(mouseDevice, source, LeftButtonDown, RightButtonDown, MiddleButtonDown, location, isTouch)
        {
            //IsTouch = isTouch;
            Delta = delta;
        }

        //public bool IsTouch { get; private set; }
        public Vector Delta { get; private set; }
    }
}
