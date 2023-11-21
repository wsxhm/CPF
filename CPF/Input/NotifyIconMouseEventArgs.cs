using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class NotifyIconMouseEventArgs : EventArgs
    {
        public NotifyIconMouseEventArgs(MouseButton button)
        {
            Button = button;
        }
        public MouseButton Button { get;private set; }
    }
}
