using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    public class SingleTextView : NoWrapTextView
    {
        public SingleTextView() {
            base.ShowLineNumber = false;
        }

        public override void OnTextStartChange(ISTTextBoxRender render, TextManagerTextEventArgs e) {
            base.OnTextStartChange(render, e);
            for (int i = 0; i < e.TextHistoryRecord.Count; i++) {
                var r = e.TextHistoryRecord[i];
                r.NewText = r.NewText.Replace("\r", "").Replace("\n", "");
                e.TextHistoryRecord[i] = r;
            }
        }

        public override void OnCalcTextRectangle() {
            base.OnCalcTextRectangle();
            Rectangle rect_text = base.TextRectangle;
            Rectangle rect_head = base.HeadRectangle;
            rect_text.Height = base.Core.LineHeight + base.Core.TextBox.LineSpacing;
            rect_text.Y = (base.Core.TextBox.Height - rect_text.Height) / 2;
            rect_head.Height = rect_text.Height;
            rect_head.Y = rect_text.Y;
            base.TextRectangle = rect_text;
            base.HeadRectangle = rect_head;
        }

        public override void OnCalcScroll(STTextBoxScrollInfo scrollInfo) {
            base.OnCalcScroll(scrollInfo);
            var c = base.Core;
            c.Scroll.MaxYValue = c.TextManager.LineCount - base.TextRectangle.Height / c.LineHeight;
            if (c.Scroll.MaxYValue < 0) {
                c.Scroll.MaxYValue = 0;
            }
        }
    }
}
