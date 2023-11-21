using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public delegate void TextManagerTextEventHandler(object sender, TextManagerTextEventArgs e);
    public delegate void TextManagerLineEventHandler(object sender, TextManagerLineEventArgs e);


    public class TextManagerTextEventArgs : EventArgs
    {
        public List<TextHistoryRecord> TextHistoryRecord { get; private set; }

        public TextManagerTextEventArgs(List<TextHistoryRecord> thrs) {
            this.TextHistoryRecord = thrs;
        }
    }

    public class TextManagerLineEventArgs : EventArgs
    {
        public int IndexOfLine { get; private set; }
        public TextLine Line { get; private set; }
        public TextHistoryRecord History { get; private set; }

        public TextManagerLineEventArgs(int nLineIndex, TextLine stLine, TextHistoryRecord history) {
            this.IndexOfLine = nLineIndex;
            this.History = history;
            this.Line = stLine;
        }
    }
}
