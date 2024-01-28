using CPF.Documents;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    [Browsable(false)]
    public abstract class ITextBoxView : UIElement, IDocumentStyle
    {
        private void Error() {
            throw new Exception("你看到此错误说明没有被重写，禁止调用,请使用override重写");
        }
        public virtual string FontFamily { get; set; }
        public virtual float FontSize { get; set; }
        public virtual FontStyles FontStyle { get; set; }
        public virtual ViewFill Foreground { get; set; }
        public virtual ViewFill Background { get; set; }
        public virtual TextDecoration TextDecoration { get; set; }
        public virtual Document Document { get; }
        public virtual void _ShowCaret() { Error(); }
        public virtual Point GetPostion(IList<uint> index, out float height) {
            Error();
            height = 0;
            return null;
        }
        public virtual void HitTest(Point mosPos, IList<uint> index) {
            Error();
        }
        public virtual int Comparer(IList<uint> index1, IList<uint> index2) { Error(); return 0; }

        public virtual void UpdateCaretPosition() { Error(); }

        public virtual IDocumentElement HitTestElement(Point mosPos) { Error(); return null; }
    }
}
