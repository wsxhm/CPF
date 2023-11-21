using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;
using CPF.Input;

namespace ST.Library.UI.STTextBox
{
    public interface ITextView
    {
        void Init(STTextBox.Core textBoxCore);

        void OnTextStartChange(ISTTextBoxRender render, TextManagerTextEventArgs e);
        void OnLineChanged(ISTTextBoxRender render, TextManagerLineEventArgs e);
        void OnLineRemoved(ISTTextBoxRender render, TextManagerLineEventArgs e);
        void OnLineAdded(ISTTextBoxRender render, TextManagerLineEventArgs e);
        void OnLineCountChanged(ISTTextBoxRender render, EventArgs e);
        void OnTextChanged(ISTTextBoxRender render, TextManagerTextEventArgs e);

        void OnSetCursor(MouseEventArgs e);
        void OnResize(EventArgs e);
        void OnDrawView(ISTTextBoxRender render);

        void OnCalcTextRectangle();
        void OnCalcScroll(STTextBoxScrollInfo scrollInfo);

        int GetCurrentCharOffset();

        Point ControlToView(Point pt);
        Point ViewToControl(Point pt);
        FindInfo FindFromPoint(Point pt);
        FindInfo FindFromCharIndex(int nIndex);

        void SetCaretPostion(int nCharIndex);
        FindInfo SetCaretPostion(Point pt);
        void ScrollXToMuosePoint( float nX);
        void ScrollYToMousePoint(float nY);
        bool ScrollToCaret();
    }
}
