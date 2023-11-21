using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    public class STTextBoxSelectionInfo
    {
        public event EventHandler SelectionChanged;

        private int _StartIndex;
        public int StartIndex {
            get { return _StartIndex; }
            private set {
                if (value == _StartIndex) return;
                _StartIndex = value;
                //this.OnSelectionChanged(EventArgs.Empty);
            }
        }
        private int _EndIndex;

        public int EndIndex {
            get { return _EndIndex; }
            private set {
                if (value == _EndIndex) return;
                _EndIndex = value;
                //this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public bool IsBlock { get; set; }
        public int AnchorIndex { get; set; }
        public int Length { get { return EndIndex - StartIndex; } }
        public bool IsEmptySelection { get { return EndIndex == StartIndex; } }

        public void SetIndex(int nIndex) { this.SetIndex(nIndex, false); }
        public void SetSelection(int nCurrentIndex) { this.SetSelection(nCurrentIndex, false); }
        public void SetSelection(int nIndexStart, int nIndexEnd) { this.SetSelection(nIndexStart, nIndexEnd, false); }

        public void SetIndex(int nIndex, bool isBlock) {
            this.IsBlock = isBlock;
            this.AnchorIndex = nIndex;
            this.StartIndex = nIndex;
            this.EndIndex = nIndex;
            this.OnSelectionChanged(EventArgs.Empty);
        }

        public void SetSelection(int nCurrentIndex, bool isBlock) {
            this.IsBlock = isBlock;
            if (nCurrentIndex <= this.AnchorIndex) {
                this.StartIndex = nCurrentIndex;
                this.EndIndex = this.AnchorIndex;
            } else {
                this.StartIndex = this.AnchorIndex;
                this.EndIndex = nCurrentIndex;
            }
            this.OnSelectionChanged(EventArgs.Empty);
        }

        public void SetSelection(int nIndexStart, int nIndexEnd, bool isBlock) {
            this.IsBlock = isBlock;
            this.AnchorIndex = nIndexStart;
            if (nIndexStart < nIndexEnd) {
                this.StartIndex = nIndexStart;
                this.EndIndex = nIndexEnd;
            } else {
                this.StartIndex = nIndexEnd;
                this.EndIndex = nIndexStart;
            }
            this.OnSelectionChanged(EventArgs.Empty);
        }

        public void Clear() {
            this.StartIndex = 0;
            this.EndIndex = 0;
        }

        protected virtual void OnSelectionChanged(EventArgs e) {
            if (this.SelectionChanged != null) {
                this.SelectionChanged(this, e);
            }
        }
    }
}
