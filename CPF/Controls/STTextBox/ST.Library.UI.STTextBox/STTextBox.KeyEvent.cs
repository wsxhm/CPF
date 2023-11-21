using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ST.Library.UI.STTextBox
{
    public partial class STTextBox
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.Back:
                    this.ProcessBackSpaceKey();
                    return true;
                case Keys.Tab:
                    this.ProcessTabKey();
                    return true;
                case Keys.Enter:
                    this.ProcessEnter();
                    return true;
                //========================================================
                case Keys.Left:
                    this.ProcessLeftKey(false);
                    return true;
                case Keys.Right:
                    this.ProcessRightKey(false);
                    return true;
                case Keys.Up:
                    this.ProcessUpKey(false);
                    return true;
                case Keys.Down:
                    this.ProcessDownKey(false);
                    return true;
                case Keys.Home:
                    this.ProcessHomeKey(false);
                    return true;
                case Keys.End:
                    this.ProcessEndKey(false);
                    return true;
                case Keys.PageUp:
                    this.ProcessPageUpKey(false);
                    return true;
                case Keys.PageDown:
                    this.ProcessPageDownKey(false);
                    return true;
                //========================================================
                case Keys.Shift | Keys.Tab:
                    this.ProcessShiftTabKey();
                    return true;
                case Keys.Shift | Keys.Up:
                    this.ProcessUpKey(true);
                    return true;
                case Keys.Shift | Keys.Down:
                    this.ProcessDownKey(true);
                    return true;
                case Keys.Shift | Keys.Left:
                    this.ProcessLeftKey(true);
                    return true;
                case Keys.Shift | Keys.Right:
                    this.ProcessRightKey(true);
                    return true;
                case Keys.Shift | Keys.Home:
                    this.ProcessHomeKey(true);
                    return true;
                case Keys.Shift | Keys.End:
                    this.ProcessEndKey(true);
                    return true;
                case Keys.Shift | Keys.PageUp:
                    this.ProcessPageUpKey(true);
                    return true;
                case Keys.Shift | Keys.PageDown:
                    this.ProcessPageDownKey(true);
                    return true;
                //========================================================
                case Keys.Control | Keys.A:
                    this.SelectAll();
                    return true;
                case Keys.Control | Keys.C:
                    this.Copy();
                    return true;
                case Keys.Control | Keys.X:
                    this.Cut();
                    return true;
                case Keys.Control | Keys.V:
                    this.Paste();
                    return true;
                case Keys.Control | Keys.Z:
                    this.Undo();
                    return true;
                case Keys.Control | Keys.Y:
                    this.Redo();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if (e.KeyChar < 32/* || e.KeyChar > 126*/) {
                return;
            }
            this.EnterText(e.KeyChar.ToString());
        }
        //==================================================================================
        private void ProcessEnter() {
            var c = m_core;
            string strText = "\r\n";
            if (this._AutoIndent) {
                strText += Regex.Match(
                    c.TextManager[c.TextManager.GetLineIndexFromCharIndex(c.Caret.IndexOfChar)].RawString,
                    @"^[ \t]+"  //Space,TAB
                    ).Value;
            }
            this.EnterText(strText);
        }

        private void ProcessTabKey() {
            var c = m_core;
            int nIndexStartLine = 0, nIndexEndLine = 0;
            if (!c.Selection.IsEmptySelection) {
                nIndexStartLine = c.TextManager.GetLineIndexFromCharIndex(c.Selection.StartIndex);
                nIndexEndLine = c.TextManager.GetLineIndexFromCharIndex(c.Selection.EndIndex);
            }
            if (nIndexStartLine == nIndexEndLine) {
                if (!this._TabToSpace) {
                    this.EnterText("\t");
                } else {
                    float fSpaceCount = c.ITextBoxRender.GetTabSpaceCount(c.ITextView.GetCurrentCharOffset(), this._TabSize);
                    this.EnterText("".PadLeft((int)Math.Ceiling(fSpaceCount)));
                }
            } else {
                string strInsert = this._TabToSpace ? "".PadLeft(this._TabSize) : "\t";
                var histories = c.TextManager.InsertAtStart(nIndexStartLine, nIndexEndLine - nIndexStartLine + 1, strInsert);
                c.ITextHistory.SetHistory(histories);
                var lineStart = c.TextManager[nIndexStartLine];
                var lineEnd = c.TextManager[nIndexEndLine];
                c.Selection.SetSelection(lineStart.IndexOfFirstChar, lineEnd.IndexOfFirstChar + lineEnd.GetLengthWithoutNewline());
                c.Caret.IndexOfChar = c.Selection.EndIndex;
                c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            }
        }

        private void ProcessShiftTabKey() {
            var c = m_core;
            int nIndexStartLine = 0, nIndexEndLine = 0;
            nIndexStartLine = c.TextManager.GetLineIndexFromCharIndex(c.Selection.StartIndex);
            nIndexEndLine = c.TextManager.GetLineIndexFromCharIndex(c.Selection.EndIndex);
            if (nIndexStartLine == nIndexEndLine) {
                var line = c.TextManager[nIndexStartLine];
                for (int i = 0; i < line.RawString.Length; i++) {
                    switch (line.RawString[i]) {
                        case ' ':
                        case '\t':
                            continue;
                        default:
                            break;
                    }
                    if (i == 0 || i + line.IndexOfFirstChar < c.Selection.StartIndex) {
                        return;
                    }
                }
            }
            var histories = c.TextManager.TrimStartTab(nIndexStartLine, nIndexEndLine - nIndexStartLine + 1, this._TabSize);
            c.ITextHistory.SetHistory(histories.ToArray());
            var lineStart = c.TextManager[nIndexStartLine];
            var lineEnd = c.TextManager[nIndexEndLine];
            c.Selection.SetSelection(lineStart.IndexOfFirstChar, lineEnd.IndexOfFirstChar + lineEnd.GetLengthWithoutNewline());
            c.Caret.IndexOfChar = c.Selection.EndIndex;
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
        }

        private void ProcessBackSpaceKey() {
            var c = m_core;
            if (c.Caret.IndexOfChar == 0 && c.Selection.IsEmptySelection) {
                return;
            }
            if (c.Selection.IsEmptySelection) {
                //c.Selection.SetIndex(c.Caret.IndexOfChar);
                int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(c.Caret.IndexOfChar);
                TextLine stLine = c.TextManager[nLineIndex];
                if (c.Caret.IndexOfChar == stLine.IndexOfFirstChar) {//if the index is the line start index
                    stLine = c.TextManager[nLineIndex - 1];
                    //c.Selection.StartIndex -= stLine.RawString.Length - stLine.GetLengthWithoutNewline();
                    c.Selection.SetSelection(c.Caret.IndexOfChar, c.Caret.IndexOfChar - (stLine.RawString.Length - stLine.GetLengthWithoutNewline()));
                } else {
                    int nIndex = stLine.IndexOfFirstChar;
                    c.IGraphemeSplitter.Each(stLine.RawString, (str, nStart, nLen) => {
                        if (nIndex + nLen >= c.Caret.IndexOfChar) {
                            return false;
                        }
                        nIndex += nLen;
                        return true;
                    });
                    //c.Selection.StartIndex = nIndex;
                    c.Selection.SetSelection(nIndex, c.Caret.IndexOfChar);
                }
            }
            this.EnterText("");
        }

        private void ProcessUpKey(bool bShiftDown) {
            var c = m_core;
            if (c.Scroll.YValue == 0 && c.Caret.Y - m_core.ViewRectangle.Y < m_core.LineHeight) {
                return;
            }
            c.Caret.Y -= c.LineHeight;// c.ITextRender.GetFontHeight();
            var fi = c.ITextView.FindFromPoint(c.Caret.Location);
            if (!c.Caret.CopyFromFindInfo(fi)) {
                return;
            }
            bool bRedraw = false;
            if (bShiftDown) {
                c.Selection.SetSelection(c.Caret.IndexOfChar);
                bRedraw = true;
            } else {
                if (!c.Selection.IsEmptySelection) {
                    bRedraw = true;
                    c.Caret.IndexOfChar = c.Selection.StartIndex;
                }
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            if (c.ITextView.ScrollToCaret() || bRedraw) {
                this.Invalidate();
            }
        }

        private void ProcessDownKey(bool bShiftDown) {
            var c = m_core;
            if (c.Caret.IndexOfLine >= c.TextManager.LineCount - 1) {
                //return;
            }
            c.Caret.Y += c.LineHeight;// c.ITextRender.GetFontHeight();
            var fi = c.ITextView.FindFromPoint(c.Caret.Location);
            if (!c.Caret.CopyFromFindInfo(fi)) {
                return;
            }
            bool bRedraw = false;
            if (bShiftDown) {
                c.Selection.SetSelection(c.Caret.IndexOfChar);
                bRedraw = true;
            } else {
                if (!c.Selection.IsEmptySelection) {
                    bRedraw = true;
                    c.Caret.IndexOfChar = c.Selection.EndIndex;
                }
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            if (c.ITextView.ScrollToCaret() || bRedraw) {
                this.Invalidate();
            }
        }

        private void ProcessLeftKey(bool bShiftDown) {
            var c = m_core;
            bool bRedraw = false;
            if (bShiftDown) {
                var nIndex = c.TextBox.FindLeftIndex(c.Caret.IndexOfChar);
                if (nIndex == c.Caret.IndexOfChar) {
                    return;
                }
                c.Caret.IndexOfChar = nIndex;
                c.Selection.SetSelection(c.Caret.IndexOfChar);
                bRedraw = true;
            } else {
                if (!c.Selection.IsEmptySelection) {
                    bRedraw = true;
                    c.Caret.IndexOfChar = c.Selection.StartIndex;
                } else {
                    var nIndex = c.TextBox.FindLeftIndex(c.Caret.IndexOfChar);
                    if (nIndex == c.Caret.IndexOfChar) {
                        return;
                    }
                    c.Caret.IndexOfChar = nIndex;
                }
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            if (c.ITextView.ScrollToCaret() || bRedraw) {
                this.Invalidate();
            }
        }

        private void ProcessRightKey(bool bShiftDown) {
            var c = m_core;
            bool bRedraw = false;
            if (bShiftDown) {
                var nIndex = c.TextBox.FindRightIndex(c.Caret.IndexOfChar);
                if (nIndex == c.Caret.IndexOfChar) {
                    return;
                }
                c.Caret.IndexOfChar = nIndex;
                c.Selection.SetSelection(nIndex);
                bRedraw = true;
            } else {
                if (!c.Selection.IsEmptySelection) {
                    bRedraw = true;
                    c.Caret.IndexOfChar = c.Selection.EndIndex;
                } else {
                    var nIndex = c.TextBox.FindRightIndex(c.Caret.IndexOfChar);
                    if (nIndex == c.Caret.IndexOfChar) {
                        return;
                    }
                    c.Caret.IndexOfChar = nIndex;
                }
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            if (c.ITextView.ScrollToCaret() || bRedraw) {
                this.Invalidate();
            }
        }

        private void ProcessHomeKey(bool bShiftDown) {
            var c = m_core;
            bool bRedraw = false;
            var stLine = c.TextManager.GetLineFromCharIndex(c.Caret.IndexOfChar);
            c.Caret.IndexOfChar = stLine.IndexOfFirstChar;
            if (bShiftDown) {
                c.Selection.SetSelection(c.Caret.IndexOfChar);
                bRedraw = true;
            } else {
                if (!c.Selection.IsEmptySelection) {
                    bRedraw = true;
                }
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            if (c.ITextView.ScrollToCaret() || bRedraw) {
                this.Invalidate();
            }
        }

        private void ProcessEndKey(bool bShiftDown) {
            var c = m_core;
            bool bRedraw = false;
            var stLine = c.TextManager.GetLineFromCharIndex(c.Caret.IndexOfChar);
            //c.Caret.IndexOfChar = stLine.IndexOfFirstChar + stLine.RawString.Length;
            c.Caret.IndexOfChar = stLine.IndexOfFirstChar + stLine.GetLengthWithoutNewline();
            if (bShiftDown) {
                c.Selection.SetSelection(c.Caret.IndexOfChar);
                bRedraw = true;
            } else {
                if (!c.Selection.IsEmptySelection) {
                    bRedraw = true;
                }
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            if (c.ITextView.ScrollToCaret() || bRedraw) {
                this.Invalidate();
            }
        }

        private void ProcessPageUpKey(bool bShiftDown) {
            var c = m_core;
            if (c.Scroll.YValue == 0) {
                return;
            }
            int nCurrentLineCount = this.Height / c.LineHeight;
            c.Scroll.YValue -= nCurrentLineCount;
            if (c.Scroll.YValue < 0) {
                c.Scroll.YValue = 0;
                c.Caret.Y = 0;
            }
            var fi = c.ITextView.SetCaretPostion(c.Caret.Location);// c.ITextView.FindFromPoint(c.Caret.Location);
            if (!fi.Find) {
                return;
            }
            //c.Caret.CopyFromFindInfo(fi);
            if (bShiftDown) {
                c.Selection.SetSelection(c.Caret.IndexOfChar);
            } else {
                if (!c.Selection.IsEmptySelection) {
                    c.Selection.SetIndex(c.Caret.IndexOfChar);
                    this.Invalidate();
                } else {
                    c.Selection.SetIndex(c.Caret.IndexOfChar);
                }
            }
            //c.ITextView.SetCaretPostion(c.Caret.X, c.Caret.Y);
            this.Invalidate();
        }

        private void ProcessPageDownKey(bool bShiftDown) {
            var c = m_core;
            if (c.Scroll.YValue == c.Scroll.MaxYValue) {
                return;
            }
            int nCurrentLineCount = this.Height / c.LineHeight;
            c.Scroll.YValue += nCurrentLineCount;
            if (c.Scroll.YValue > c.Scroll.MaxYValue) {
                c.Scroll.YValue = c.Scroll.MaxYValue;
            }
            var fi = c.ITextView.SetCaretPostion(c.Caret.Location);// c.ITextView.FindFromPoint(c.Caret.Location);
            if (!fi.Find) {
                return;
            }
            //c.Caret.CopyFromFindInfo(fi);
            if (bShiftDown) {
                c.Selection.SetSelection(c.Caret.IndexOfChar);
            } else {
                if (!c.Selection.IsEmptySelection) {
                    c.Selection.SetIndex(c.Caret.IndexOfChar);
                    this.Invalidate();
                } else {
                    c.Selection.SetIndex(c.Caret.IndexOfChar);
                }
            }
            //c.ITextView.SetCaretPostion(c.Caret.X, c.Caret.Y);
            this.Invalidate();
        }

        private void EnterText(string strText) {
            var c = m_core;
            var history = c.TextManager.SetText(c.Selection.StartIndex, c.Selection.Length, strText);
            if (history != TextHistoryRecord.Empty) {
                c.ITextHistory.SetHistory(new TextHistoryRecord[] { history });
            }
            c.Caret.IndexOfChar = c.Selection.StartIndex + strText.Length;
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            c.Selection.SetIndex(c.Caret.IndexOfChar);
            c.ITextView.ScrollToCaret();
            //this.Invalidate();
        }
        //==================================================
        private int FindRightIndex(int nIndex) {
            var c = m_core;
            if (nIndex >= c.TextManager.TextLength) {
                return c.TextManager.TextLength;
            }
            int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(nIndex);
            var line = c.TextManager[nLineIndex];
            c.IGraphemeSplitter.Each(line.RawString, nIndex - line.IndexOfFirstChar, (str, nStart, nLen) => {
                nIndex += nLen;
                return false;
            });
            return nIndex;
        }
        //private FindInfo FindRightChar(int nIndex) {
        //    //TODO: add location for return;
        //    var c = m_core;
        //    if (nIndex >= c.TextManager.TextLength) {
        //        return FindInfo.Empty;
        //    }
        //    int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(nIndex);
        //    var line = c.TextManager[nLineIndex];
        //    c.IGraphemeSplitter.Each(line.RawString, nIndex - line.IndexOfFirstChar, (str, nStart, nLen) => {
        //        nIndex += nLen;
        //        return false;
        //    });
        //    return new FindInfo() {
        //        Find = true,
        //        Line = line,
        //        IndexOfCharInLine = nIndex - line.IndexOfFirstChar,
        //        IndexOfLine = nLineIndex,
        //    };
        //}

        public int FindLeftIndex(int nIndex) {
            var c = m_core;
            if (nIndex <= 1) return 0;
            int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(nIndex);
            TextLine line = c.TextManager[nLineIndex];
            if (nIndex == line.IndexOfFirstChar) {
                line = c.TextManager[--nLineIndex];
                if (line.RawString.Length > 1 && line.RawString[line.RawString.Length - 2] == '\r') {
                    nIndex -= 2;
                } else {
                    nIndex -= 1;
                }
            } else {
                int nIndexRet = line.IndexOfFirstChar;
                c.IGraphemeSplitter.Each(line.RawString, (str, nStart, nLen) => {
                    if (nIndexRet + nLen >= nIndex) {
                        return false;
                    }
                    nIndexRet += nLen;
                    return true;
                });
                nIndex = nIndexRet;
            }
            return nIndex;
        }

        //private FindInfo FindLeftChar(int nIndex) {
        //    //TODO: add location for return;
        //    var c = m_core;
        //    if (nIndex <= 1) return new FindInfo() {
        //        Find = true,
        //        Line = c.TextManager[0]
        //    };
        //    int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(nIndex);
        //    TextLine line = c.TextManager[nLineIndex];
        //    if (nIndex == line.IndexOfFirstChar) {
        //        line = c.TextManager[--nLineIndex];
        //        if (line.RawString.Length > 1 && line.RawString[line.RawString.Length - 2] == '\r') {
        //            nIndex -= 2;
        //        } else {
        //            nIndex -= 1;
        //        }
        //    } else {
        //        int nIndexRet = line.IndexOfFirstChar;
        //        c.IGraphemeSplitter.Each(line.RawString, (str, nStart, nLen) => {
        //            if (nIndexRet + nLen >= nIndex) {
        //                return false;
        //            }
        //            nIndexRet += nLen;
        //            return true;
        //        });
        //        nIndex = nIndexRet;
        //    }
        //    return new FindInfo() {
        //        Find = true,
        //        Line = line,
        //        IndexOfCharInLine = nIndex - line.IndexOfFirstChar,
        //        IndexOfLine = nLineIndex
        //    };
        //}
    }
}
