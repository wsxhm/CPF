using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public class TextLine
    {
        public int IndexOfFirstChar { get; internal set; }
        public string RawString { get; internal set; }
        public object Tag { get; set; }

        internal TextLine() {
            this.RawString = string.Empty;
        }

        public int GetLengthWithoutNewline() {
            for (int i = this.RawString.Length - 1; i >= 0; i--) {
                switch (this.RawString[i]) {
                    case '\r':
                    case '\n':
                        continue;
                    default:
                        return i + 1;
                }
            }
            return 0;
        }

        public override string ToString() {
            return "[" + IndexOfFirstChar + "]" + this.RawString;
        }
    }
}
