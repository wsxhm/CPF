using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public interface IEmojiRender
    {
        bool IsEmoji(string strChar);
        void DrawEmoji(ISTTextBoxRender dt, string strChar, int nX, int nY, int nWidth, bool bSelected);
    }
}
