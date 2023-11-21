using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    public class NoWrapTextView : TextView
    {
        private struct NoWrapLineCache
        {
            public int Index;
            public int Offset;
            public override string ToString() {
                return "[" + this.Index + "," + this.Offset + "]";
            }
        }

        private int m_nMaxLineWidth;
        //private HashSet<TextLine> m_hs_changed = new HashSet<TextLine>();
        private Dictionary<TextLine, List<NoWrapLineCache>> m_dic_nowrap_line = new Dictionary<TextLine, List<NoWrapLineCache>>();
        private StringFormat m_sf = new StringFormat(StringFormat.GenericTypographic) {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center
        };

        public override void OnInitAllText() {
            //m_hs_changed.Clear();
            m_dic_nowrap_line.Clear();
            var c = base.Core;
            foreach (var line in c.TextManager) {
                this.AddLine(line, c.ITextBoxRender);
            }
        }

        public override void OnLineAdded(ISTTextBoxRender render, TextManagerLineEventArgs e) {
            this.AddLine(e.Line, render);
        }

        public override void OnLineRemoved(ISTTextBoxRender render, TextManagerLineEventArgs e) {
            //m_hs_changed.Remove(e.Line);
            m_dic_nowrap_line.Remove(e.Line);
        }

        public override void OnLineChanged(ISTTextBoxRender render, TextManagerLineEventArgs e) {
            //m_hs_changed.Add(e.Line);
            var c = base.Core;
            int nElementIndex = 0;
            var li = this.GetLineCacheFromIndex(e.Line, e.History.Index - e.Line.IndexOfFirstChar, out nElementIndex);
            int nEachIndex = li.Index;
            e.Line.Tag = li.Offset;
            int nCounter = 0;
            List<NoWrapLineCache> lst = null;
            if (m_dic_nowrap_line.ContainsKey(e.Line)) {
                lst = m_dic_nowrap_line[e.Line];
                lst.RemoveRange(nElementIndex, lst.Count - nElementIndex);
            } else {
                lst = new List<NoWrapLineCache>();
            }
            c.IGraphemeSplitter.Each(e.Line.RawString, nEachIndex, (str, nStart, nLen) => {
                string strChar = str.Substring(nStart, nLen);
                if (nCounter++ % 500 == 0) {
                    lst.Add(new NoWrapLineCache() {
                        Offset = _LineWidth(e.Line),
                        Index = nStart
                    });
                }
                e.Line.Tag = _LineWidth(e.Line) + c.GetStringWidth(strChar, c.GetStyleFromCharIndex(e.Line.IndexOfFirstChar + nStart), _LineWidth(e.Line));
                if (_LineWidth(e.Line) > m_nMaxLineWidth) {
                    m_nMaxLineWidth = _LineWidth(e.Line);
                }
                return true;
            });
            if (lst.Count > 1) {
                if (!m_dic_nowrap_line.ContainsKey(e.Line)) {
                    m_dic_nowrap_line.Add(e.Line, lst);
                }
            }
        }

        public override int GetCurrentCharOffset() {
            var c = base.Core;
            return c.Caret.X - base.TextRectangle.X + c.Scroll.XOffset;
            //throw new NotImplementedException();
        }

        public override FindInfo FindFromPoint(Point pt) {
            var c = base.Core;
            FindInfo fi = new FindInfo();
            int nXStart = base.TextRectangle.X;
            int nX = nXStart - c.Scroll.XOffset, nIndexChar = 0, nCharWidth = 0;

            int nIndexLine = c.Scroll.YValue + ((pt.Y - base.TextRectangle.Y) / c.LineHeight);// c.ITextRender.GetFontHeight());
            if (nIndexLine >= c.TextManager.LineCount) {//Note: Count = 0;outofrange...
                nIndexLine = c.TextManager.LineCount - 1;
            } else if (nIndexLine < 0) {
                nIndexLine = 0;
            }

            TextLine line = c.TextManager[nIndexLine];
            fi.Line = line;
            fi.IndexOfLine = nIndexLine;
            int nElementIndex = 0;
            var li = this.GetLineCacheFromOffset(line, c.Scroll.XOffset + (pt.X - base.TextRectangle.Left), out nElementIndex);
            nIndexChar = li.Index;
            nX += li.Offset;
            //using (var dt = c.GetDrawingTools()) {
            c.ITextBoxRender.BeginPaint();
            c.IGraphemeSplitter.Each(line.RawString, nIndexChar, (str, nStart, nLen) => {
                if (str[nStart] == '\r') return false;
                if (str[nStart] == '\n') return false;
                string strChar = str.Substring(nStart, nLen);
                nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), nX - base.TextRectangle.X + c.Scroll.XOffset);
                if (nX + nCharWidth > pt.X) {
                    fi.Find = true;
                    if (pt.X - nX >= nCharWidth / 2) {
                        fi.IndexOfCharInLine = nIndexChar + nLen;
                        fi.Location = new Point(nX + nCharWidth, base.TextRectangle.Y + (nIndexLine - c.Scroll.YValue) * c.LineHeight);
                    } else {
                        fi.IndexOfCharInLine = nIndexChar;
                        fi.Location = new Point(nX, base.TextRectangle.Y + (nIndexLine - c.Scroll.YValue) * c.LineHeight);
                    }
                    return false;
                }
                nX += nCharWidth;
                nIndexChar += nLen;
                return true;
            });
            c.ITextBoxRender.EndPaint();
            //}
            if (!fi.Find) {
                fi.IndexOfCharInLine = nIndexChar;
                fi.Location = new Point(nX, base.TextRectangle.Y + (nIndexLine - c.Scroll.YValue) * c.LineHeight);
            }
            fi.Find = true;
            return fi;
        }

        public override FindInfo FindFromCharIndex(int nIndex) {
            var c = base.Core;
            int nIndexOfLine = c.TextManager.GetLineIndexFromCharIndex(nIndex);
            TextLine stLine = c.TextManager[nIndexOfLine];
            if (stLine == null) {
                return new FindInfo();
            }
            return this.FindFromCharIndex(nIndexOfLine, nIndex - stLine.IndexOfFirstChar);
        }

        private FindInfo FindFromCharIndex(int nIndexOfLine, int nIndex) {
            var c = base.Core;
            //using (var dt = base.Core.GetDrawingTools()) {
            return this.FindFromCharIndex(c.ITextBoxRender, nIndexOfLine, nIndex);
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="stLine"></param>
        /// <param name="nIndex">Index of stLine</param>
        /// <returns></returns>
        private FindInfo FindFromCharIndex(ISTTextBoxRender render, int nIndexOfLine, int nIndex) {
            var c = base.Core;
            TextLine line = c.TextManager[nIndexOfLine];
            int nX = 0, nIndexEach = 0, nCharWidth;
            int nXStart = base.TextRectangle.X;
            nX = nXStart - c.Scroll.XOffset;
            int nElementIndex = 0;
            var li = this.GetLineCacheFromIndex(line, nIndex, out nElementIndex);
            nIndexEach = li.Index;
            nX += li.Offset;
            c.IGraphemeSplitter.Each(line.RawString, nIndexEach, (str, nStart, nLen) => {
                if (nStart >= nIndex) {
                    return false;
                }
                string strChar = str.Substring(nStart, nLen);
                nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), nX - base.TextRectangle.X + c.Scroll.XOffset);
                nX += nCharWidth;
                return true;
            });
            return new FindInfo() {
                Line = line,
                IndexOfLine = nIndexOfLine,
                IndexOfCharInLine = nIndex,
                Location = new Point(nX, base.TextRectangle.Y + (nIndexOfLine - c.Scroll.YValue) * c.LineHeight)// c.ITextRender.GetFontHeight())
            };
        }

        protected override void OnDrawLineSelectionBackground(ISTTextBoxRender render, TextLine line, int nLineIndex, STTextBoxSelectionInfo selection, int nY) {
            var c = base.Core;
            if (line.IndexOfFirstChar >= selection.EndIndex) {
                return;
            }
            if (line.IndexOfFirstChar + line.RawString.Length <= selection.StartIndex) {
                return;
            }
            int nLeft = base.TextRectangle.Left, nWidth = _LineWidth(line) - c.Scroll.XOffset;
            if (line.IndexOfFirstChar < selection.StartIndex) {
                nLeft = this.FindFromCharIndex(render, nLineIndex, c.Selection.StartIndex - line.IndexOfFirstChar).Location.X;
                nWidth -= nLeft - base.TextRectangle.X;
            }
            if (selection.EndIndex <= line.IndexOfFirstChar + line.RawString.Length) {
                nWidth = this.FindFromCharIndex(render, nLineIndex, c.Selection.EndIndex - line.IndexOfFirstChar).Location.X;
                nWidth -= nLeft;
            }
            if (nLeft + nWidth > base.TextRectangle.Right) {
                nWidth -= nLeft + nWidth - base.TextRectangle.Right;
            }
            render.FillRectangle(c.TextBox.SelectionColor, nLeft, nY, nWidth, c.LineHeight);
        }

        protected override int OnDrawLine(ISTTextBoxRender render, TextLine line, int nX, int nY) {
            var c = base.Core;
            int nLineSpacing = c.TextBox.GetIntYSize(c.TextBox.LineSpacing) / 2;
            int nIndexEach = 0;
            Rectangle rect_char = new Rectangle(nX - c.Scroll.XOffset, nY, 0, c.LineHeight);
            if (string.IsNullOrEmpty(line.RawString)) {
                return c.LineHeight;
            }
            int nElementIndex = 0, nCharSpacing = c.TextBox.GetIntXSize(c.TextBox.CharSpacing);// / 2;
            var li = this.GetLineCacheFromOffset(line, c.Scroll.XOffset, out nElementIndex);
            nIndexEach = li.Index;
            rect_char.X += li.Offset;
            int nCurrentCharIndex = line.IndexOfFirstChar + nIndexEach;

            bool bIsEmoji = false;
            c.IGraphemeSplitter.Each(line.RawString, nIndexEach, (str, nStart, nLen) => {
                string strChar = str.Substring(nStart, nLen);
                if (c.IsEmoji(strChar)) {
                    bIsEmoji = true;
                    rect_char.Width = c.FontHeight;
                } else {
                    if (nLen > 1 && strChar[nLen - 1] >= '\uFE00' && strChar[nLen - 1] <= '\uFE0F') {
                        strChar = str.Substring(nStart, nLen - 1);
                    }
                    bIsEmoji = false;
                    rect_char.Width = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), rect_char.X - base.TextRectangle.X + c.Scroll.XOffset) + nCharSpacing;
                }
                nCurrentCharIndex = line.IndexOfFirstChar + nStart;
                if (rect_char.Right <= base.TextRectangle.Left) {
                    rect_char.X += rect_char.Width;
                    return true;
                }
                if (bIsEmoji) {
                    bool bSelected = nCurrentCharIndex >= c.Selection.StartIndex && nCurrentCharIndex < c.Selection.EndIndex;
                    c.IEmojiRender.DrawEmoji(render, strChar, rect_char.X + (nCharSpacing >> 1), nY + nLineSpacing, c.FontHeight, bSelected);
                } else {
                    c.ITextBoxRender.DrawString(strChar, c.GetStyleFromCharIndex(nCurrentCharIndex), rect_char);
                }
                if (rect_char.Right > base.TextRectangle.Right) {
                    return false;
                }
                rect_char.X += rect_char.Width;
                return true;
            });
            return c.LineHeight;
        }

        protected override void OnDrawHead(ISTTextBoxRender render, int nStartLineIndex) {
            render.FillRectangle(base.HeadBackgroundColor, base.HeadRectangle);
            var c = base.Core;
            Rectangle rect_linenumber = new Rectangle(base.HeadRectangle.X, base.HeadRectangle.Y, base.LineNumberWidth - c.TextBox.GetIntXSize(10), c.LineHeight);
            //Rectangle rect_status = new Rectangle(base.HeadRectangle.Right - c.TextBox.GetIntXSize(base.LineStatusWidth), rect_linenumber.Y, c.TextBox.GetIntXSize(base.LineStatusWidth), c.LineHeight);
            for (int i = nStartLineIndex; i < c.TextManager.LineCount; i++) {
                if (base.ShowLineNumber) {
                    render.DrawString((i + 1).ToString(), c.TextBox.Font, base.LineNumberColor, rect_linenumber, m_sf);
                }
                //if (base.ShowLineStatus) {
                //    if (m_hs_changed.Contains(c.TextManager[i])) {
                //        render.FillRectangle(base.LineStatusColor, rect_status);
                //    }
                //}
                rect_linenumber.Y += c.LineHeight;
                //rect_status.Y += c.LineHeight;
                if (rect_linenumber.Y >= base.HeadRectangle.Bottom) break;
            }
        }

        public override int GetLineIndexFromYScroll(int nYValue) {
            //if (nYValue > base.Core.Scroll.MaxYValue) {
            //    return base.Core.Scroll.MaxYValue;
            //}
            //if (nYValue < 0) {
            //    return 0;
            //}
            return nYValue;
        }

        private int _LineWidth(TextLine line) {
            if (line.Tag == null) {
                return 0;
            }
            if (line.Tag is int) {
                return (int)line.Tag;
            }
            return 0;
        }

        private void AddLine(TextLine line, ISTTextBoxRender render) {
            //m_hs_changed.Add(line);
            var c = base.Core;
            int nCounter = 0;
            List<NoWrapLineCache> lst = new List<NoWrapLineCache>();
            c.IGraphemeSplitter.Each(line.RawString, (str, nStart, nLen) => {
                string strChar = str.Substring(nStart, nLen);
                if (nCounter++ % 500 == 0) {
                    lst.Add(new NoWrapLineCache() {
                        Offset = _LineWidth(line),
                        Index = nStart
                    });
                }
                line.Tag = _LineWidth(line) + c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), _LineWidth(line));
                if (_LineWidth(line) > m_nMaxLineWidth) {
                    m_nMaxLineWidth = _LineWidth(line);
                }
                return true;
            });
            if (lst.Count > 1) {
                m_dic_nowrap_line.Add(line, lst);
            }
        }

        private NoWrapLineCache GetLineCacheFromOffset(TextLine line, int nOffset) {
            int n = 0;
            return this.GetLineCacheFromOffset(line, nOffset, out n);
        }

        private NoWrapLineCache GetLineCacheFromOffset(TextLine line, int nOffset, out int nElementIndex) {
            nElementIndex = 0;
            if (!m_dic_nowrap_line.ContainsKey(line)) {
                return new NoWrapLineCache();
            }
            var lst = m_dic_nowrap_line[line];
            int nLeft = 0, nRight = lst.Count - 1;
            while (nLeft <= nRight) {
                nElementIndex = (nLeft + nRight) >> 1;
                if (nOffset >= lst[nElementIndex].Offset) {
                    if (nElementIndex + 1 >= lst.Count || nOffset < lst[nElementIndex + 1].Offset) {
                        return lst[nElementIndex];
                    }
                    nLeft = nElementIndex + 1;
                } else {
                    nRight = nElementIndex - 1;
                }
            }
            return lst[nElementIndex];
        }

        private NoWrapLineCache GetLineCacheFromIndex(TextLine line, int nIndex) {
            int n = 0;
            return this.GetLineCacheFromIndex(line, nIndex, out n);
        }

        private NoWrapLineCache GetLineCacheFromIndex(TextLine line, int nIndex, out int nElementIndex) {
            nElementIndex = 0;
            if (!m_dic_nowrap_line.ContainsKey(line)) {
                return new NoWrapLineCache();
            }
            var lst = m_dic_nowrap_line[line];
            int nLeft = 0, nRight = lst.Count - 1;
            while (nLeft <= nRight) {
                nElementIndex = (nLeft + nRight) >> 1;
                if (nIndex >= lst[nElementIndex].Index) {
                    if (nElementIndex + 1 >= lst.Count || nIndex < lst[nElementIndex + 1].Index) {
                        return lst[nElementIndex];
                    }
                    nLeft = nElementIndex + 1;
                } else {
                    nRight = nElementIndex - 1;
                }
            }
            return lst[nElementIndex];
        }

        public override void SetCaretPostion(int nCharIndex) {
            var c = this.Core;
            int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(nCharIndex);
            var line = c.TextManager[nLineIndex];
            var cache = this.GetLineCacheFromIndex(line, nCharIndex - line.IndexOfFirstChar);
            int nWidth = cache.Offset, nIndexTemp = line.IndexOfFirstChar + cache.Index;
            c.ITextBoxRender.BeginPaint();
            c.IGraphemeSplitter.Each(line.RawString, cache.Index, (str, nStart, nLen) => {
                if (nStart >= nCharIndex - line.IndexOfFirstChar) return false;
                string strChar = str.Substring(nStart, nLen);
                nWidth += c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), nWidth);
                nIndexTemp = line.IndexOfFirstChar + nStart + nLen;
                return true;
            });
            c.ITextBoxRender.EndPaint();
            c.Caret.IndexOfLine = nLineIndex;
            c.Caret.IndexOfChar = nIndexTemp;
            this.SetCaretPostion(nWidth - c.Scroll.XOffset + this.TextRectangle.X, this.TextRectangle.Y + (nLineIndex - c.Scroll.YValue) * c.LineHeight);
        }

        public override void OnCalcScroll(STTextBoxScrollInfo scrollInfo) {
            var c = base.Core;
            c.Scroll.MaxYValue = c.TextManager.LineCount - base.TextRectangle.Height / c.LineHeight + 2;
            if (c.Scroll.MaxYValue < 0) {
                c.Scroll.MaxYValue = 0;
            }
            c.Scroll.MaxXValue = (m_nMaxLineWidth - base.TextRectangle.Width) / c.Scroll.XIncrement + 2;
            if (c.Scroll.MaxXValue < 0) {
                c.Scroll.MaxXValue = 0;
            }
        }
    }
}
