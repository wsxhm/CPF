using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CPF.Toolkit.Controls
{
    internal class MdiWindowRect
    {
        public MdiWindowRect()
        {

        }
        public MdiWindowRect(float left, float top, float width, float height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public WindowState OldState { get; set; }

        public override string ToString()
        {
            return $"left:{this.Left}  top:{this.Top}  width:{this.Width}  height:{this.Height}";
        }
    }
}
