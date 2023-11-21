using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Input
{
    public class MouseButtonEventArgs : MouseEventArgs
    {
        public MouseButtonEventArgs(UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, MouseDevice mouseDevice, MouseButton mouseButton, bool isTouch = false) : base(source, LeftButtonDown, RightButtonDown, MiddleButtonDown, location, mouseDevice, isTouch)
        {
            MouseButton = mouseButton;
        }

        internal long timestamp;

        /// <summary>
        /// 当前触发事件的按键
        /// </summary>
        public MouseButton MouseButton { get; private set; }

        /// <summary>
        /// Read-only access to the button state.
        /// </summary>
        public MouseButtonState ButtonState
        {
            get
            {
                MouseButtonState state = MouseButtonState.Released;

                switch (MouseButton)
                {
                    case MouseButton.Left:
                        state = this.LeftButton;
                        break;

                    case MouseButton.Right:
                        state = this.RightButton;
                        break;

                    case MouseButton.Middle:
                        state = this.MiddleButton;
                        break;

                        //case MouseButton.XButton1:
                        //    state = this.XButton1;
                        //    break;

                        //case MouseButton.XButton2:
                        //    state = this.XButton2;
                        //    break;
                }

                return state;
            }
        }
    }

    public enum MouseButton : byte
    {
        None,
        /// <summary>
        ///    The left mouse button.
        /// </summary>
        Left,

        /// <summary>
        ///    The middle mouse button.
        /// </summary>
        Middle,

        /// <summary>
        ///    The right mouse button.
        /// </summary>
        Right,

        /// <summary>
        ///    The fourth mouse button.
        /// </summary>
        XButton1,

        /// <summary>
        ///    The fifth mouse button.
        /// </summary>
        XButton2
    }
}
