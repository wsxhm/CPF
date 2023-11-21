using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public interface ITextStyleMonitor
    {
        void Init(string strText);
        void OnSelectionChanged(TextManager textManager, int nStart, int nLen);
        void OnTextChanged(TextManager textManager, List<TextHistoryRecord> thrs);
        TextStyleRange GetStyleFromCharIndex(int nIndex);
    }
}
