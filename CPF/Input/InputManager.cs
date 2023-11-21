using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;

namespace CPF.Input
{
    public class InputManager
    {
        public InputManager(View host)
        {
            this.view = host;
        }

        View view;

        KeyboardDevice keyDevice;
        MouseDevice device;
        DragDropDevice dropDevice;
        /// <summary>
        ///  Read-only access to the mouse device associated with this event.
        /// </summary>
        public MouseDevice MouseDevice
        {
            get
            {
                if (device == null)
                {
                    device = new MouseDevice(this);
                }
                return device;
            }
        }

        public KeyboardDevice KeyboardDevice
        {
            get
            {
                if (keyDevice == null)
                {
                    keyDevice = new KeyboardDevice(this);
                }
                return keyDevice;
            }
        }

        public DragDropDevice DragDropDevice
        {
            get
            {
                if (dropDevice == null)
                {
                    dropDevice = new DragDropDevice(this);
                }
                return dropDevice;
            }
        }
        TouchDevice touchDevice;
        public TouchDevice TouchDevice
        {
            get
            {
                if (touchDevice == null)
                {
                    touchDevice = new TouchDevice(this);
                }
                return touchDevice;
            }
        }

        public View Root
        {
            get
            {
                return view;
            }
        }
    }
}
