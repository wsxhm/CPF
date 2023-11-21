using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public class STTextBoxCaretInfo
    {
        private int _X;
        public int X {
            get { return _X; }
            internal set {
                _X = value;
            }
        }

        private int _Y;
        public int Y {
            get { return _Y; }
            internal set {
                _Y = value;
            }
        }

        private int _Width = 1;
        public int Width {
            get { return _Width; }
            set { _Width = value; }
        }

        public bool Visable;
        public int IndexOfLine;
        public int IndexOfChar;
        public TextLine Line;
        public Point Location {
            get { return new Point(X, Y); }
        }

        public bool CopyFromFindInfo(FindInfo fi) {
            if (!fi.Find) {
                return false;
            }
            this._X = fi.Location.X;
            this._Y = fi.Location.Y;
            this.IndexOfChar = fi.IndexOfChar;
            this.IndexOfLine = fi.IndexOfLine;
            this.Line = fi.Line;
            return true;
        }
    }
}
