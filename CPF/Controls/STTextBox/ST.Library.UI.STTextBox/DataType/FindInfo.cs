using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    public struct FindInfo
    {
        public bool Find;
        public int IndexOfLine;
        public int IndexOfCharInLine;
        public int IndexOfChar { get { return Line.IndexOfFirstChar + IndexOfCharInLine; } }
        public TextLine Line;
        public Point Location;

        public static FindInfo Empty;
    }
}
