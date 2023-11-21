using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class TouchEventArgs : InputEventArgs
    {
        public TouchEventArgs(TouchPoint position, TouchDevice touchDevice, UIElement source) : base(touchDevice)
        {
            this.OriginalSource = source;
            this.Position = position;
        }

        public TouchDevice TouchDevice { get { return (TouchDevice)Device; } }

        public TouchPoint Position { get; internal set; }

    }

    public class TouchMoveEventArgs : TouchEventArgs
    {
        public TouchMoveEventArgs(TouchPoint position, TouchDevice touchDevice, UIElement source, ManipulationDelta delta) : base(position, touchDevice, source)
        {
            DeltaManipulation = delta;
        }
        /// <summary>
        /// 多点触控转换的变换数据
        /// </summary>
        public ManipulationDelta DeltaManipulation { get; private set; }

    }

    public struct TouchPoint
    {
        public Point Position { get; set; }

        public int Id { get; set; }
        public override string ToString()
        {
            return Id + ":" + Position.ToString();
        }
        public static bool operator ==(TouchPoint f1, TouchPoint f2)
        {
            return Equals(f1, f2);
        }

        public static bool operator !=(TouchPoint f1, TouchPoint f2)
        {
            return !Equals(f1, f2);
        }

        public override bool Equals(object obj)
        {
            if (obj is TouchPoint touchPoint && touchPoint.Id == Id && touchPoint.Position == Position)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Position.GetHashCode();
        }
    }
}
