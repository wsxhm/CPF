using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public interface ITextHistory
    {
        void SetHistory(TextHistoryRecord[] histories);
        TextHistoryRecord[] GetUndo();
        TextHistoryRecord[] GetRedo();
        void Clear();
    }
}
