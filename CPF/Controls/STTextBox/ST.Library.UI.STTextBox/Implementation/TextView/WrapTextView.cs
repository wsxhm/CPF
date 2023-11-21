using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ST.Library.UI.STTextBox
{
    public class WrapTextView : TextView
    {
        public Color LineNumberGuideColor { get; set; }
        public bool ShowLineNumberGuide { get; set; }

        public enum Method { Char, Word };
        public enum Alignment { Left, Center, Right/*, Justify*/ }
        private enum SubLineStatus
        {
            Start,
            Middle,
            End,
            None
        }
        private class WrapLineInfo
        {
            public int ScreenLineOffset { get; set; }
            public List<SubLineInfo> SubLines { get; set; }

            public override string ToString() {
                return "(" /*+ this.IndexOfLine + "," +*/ + this.ScreenLineOffset + "," + this.SubLines.Count + ")";
            }
        }

        private struct WordInfo
        {
            public int Width;
            public int CharCount;
        }

        private struct SubLineInfo
        {
            public int Index;           // The first char index of sub-line 
            public int Length;          // The text length
            public int CharCount;       // Grapheme count
            public int Width;           // Sub-Line width
            public int X;               // The X position to draw
            //public float ExtraWidth;    // The char space width
        }

        public Method WrapMethod { get; private set; }
        public Alignment WrapAlignment { get; private set; }

        private int m_n_line_count_start_change = 0;
        private HashSet<TextLine> m_hs_changed_temp = new HashSet<TextLine>();
        private StringFormat m_sf = new StringFormat(StringFormat.GenericTypographic) {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center
        };

        public WrapTextView() : this(Method.Word, Alignment.Left) { }
        public WrapTextView(Method method, Alignment alignment) {
            this.WrapMethod = method;
            this.WrapAlignment = alignment;
            this.ShowLineNumberGuide = true;
            this.LineNumberGuideColor = Color.Magenta;
        }

        public void SetWrap(Method method, Alignment alignment) {
            this.WrapMethod = method;
            this.WrapAlignment = alignment;
            if (this.Core == null) return;
            this.OnInitAllText();
        }

        public override int GetCurrentCharOffset() {
            var c = base.Core;
            TextLine line = c.Caret.Line;
            var tag = _Tag(line);
            int nSubIndex = 0;
            int nCharIndex = c.Caret.IndexOfChar - line.IndexOfFirstChar;
            foreach (var sub in tag.SubLines) {
                if (nCharIndex >= sub.Index && nCharIndex <= sub.Index + sub.Length) {
                    break;
                }
                nSubIndex++;
            }
            var subLine = tag.SubLines[nSubIndex];
            return c.Caret.X - base.TextRectangle.X - subLine.X;
        }

        public override void OnInitAllText() {
            var c = base.Core;
            c.ITextBoxRender.BeginPaint();
            foreach (var line in base.Core.TextManager) {
                this.CacheLine(line, base.Core.ITextBoxRender);
            }
            c.ITextBoxRender.EndPaint();
            c.TextBox.Invalidate();
        }

        public override void OnLineAdded(ISTTextBoxRender render, TextManagerLineEventArgs e) {
            this.CacheLine(e.Line, render);
            m_hs_changed_temp.Add(e.Line);
        }

        private WrapLineInfo _Tag(TextLine line) {
            WrapLineInfo ret = null;
            if (line.Tag == null || !(line.Tag is WrapLineInfo)) {
                line.Tag = ret = new WrapLineInfo();
                return ret;
            }
            return (WrapLineInfo)line.Tag;
        }

        private void CacheLine(TextLine line, ISTTextBoxRender render) {
            var tag = _Tag(line);
            tag.SubLines = this.GetCache(line, render);
        }

        public override void OnLineRemoved(ISTTextBoxRender render, TextManagerLineEventArgs e) {
            //m_hs_changed.Remove(e.Line);
            m_hs_changed_temp.Remove(e.Line);
        }

        public override void OnLineChanged(ISTTextBoxRender render, TextManagerLineEventArgs e) {
            //m_hs_changed.Add(e.Line);
            m_hs_changed_temp.Add(e.Line);
        }

        private List<SubLineInfo> GetCache(TextLine line, ISTTextBoxRender render) {
            var c = base.Core;
            List<SubLineInfo> lst = null;
            if (this.WrapMethod == Method.Char) {
                lst = new List<SubLineInfo>();
                this.GetCacheByChar(line, render, lst, 0, 0);
            } else {
                lst = this.GetCacheByWord(line, render);
            }
            for (int i = 0; i < lst.Count; i++) {
                var sub = lst[i];
                switch (this.WrapAlignment) {
                    case Alignment.Center:
                        sub.X = (base.TextRectangle.Width - sub.Width) >> 1;
                        if (sub.Length > 0) {
                            switch (line.RawString[sub.Index + sub.Length - 1]) {
                                case '\r':
                                case '\n':
                                    sub.X += c.ITextBoxRender.GetSpaceWidth() / 2;
                                    break;
                            }
                        }
                        break;
                    case Alignment.Right:
                        sub.X = base.TextRectangle.Width - sub.Width - c.ITextBoxRender.GetSpaceWidth();
                        if (sub.Length > 0) {
                            switch (line.RawString[sub.Index]) {
                                case '\r':
                                case '\n':
                                    if (i == 0) break;
                                    var temp = lst[i - 1];
                                    temp.Width += sub.Width;
                                    temp.Length += sub.Length;
                                    temp.CharCount += 1;
                                    lst[i - 1] = temp;
                                    lst.RemoveAt(i);
                                    return lst;

                            }
                            switch (line.RawString[sub.Index + sub.Length - 1]) {
                                case '\r':
                                case '\n':
                                    sub.X += c.ITextBoxRender.GetSpaceWidth();
                                    break;
                            }
                            break;
                        }
                        break;
                }
                lst[i] = sub;
            }
            return lst;
        }

        private int GetCacheByChar(TextLine line, ISTTextBoxRender render, List<SubLineInfo> lst, int nStartIndex, int nWidth) {
            var c = base.Core;
            int nLineWidth = nWidth, nCharWidth = 0;
            int nSubStrLen = 0;
            int nCharCount = 0;
            int nTextViewLineWidth = base.TextRectangle.Width;
            if (this.WrapAlignment != Alignment.Left) {
                nTextViewLineWidth -= c.ITextBoxRender.GetSpaceWidth();
            }
            c.IGraphemeSplitter.Each(line.RawString, nStartIndex, (str, nStart, nLen) => {
                string strChar = str.Substring(nStart, nLen);
                if (c.IsEmoji(strChar)) {
                    nCharWidth = c.FontHeight;
                } else {
                    nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), nLineWidth);
                }
                if (nLineWidth + nCharWidth >= nTextViewLineWidth) {
                    lst.Add(new SubLineInfo() {
                        Index = nStartIndex,
                        Length = nSubStrLen,
                        CharCount = nCharCount,
                        Width = nLineWidth
                    });
                    nCharCount = 1;
                    nStartIndex += nSubStrLen;
                    nSubStrLen = nLen;
                    nLineWidth = nCharWidth;
                } else {
                    nSubStrLen += nLen;
                    nLineWidth += nCharWidth;
                    nCharCount++;
                }
                return true;
            });
            lst.Add(new SubLineInfo() {
                Index = nStartIndex,
                Length = nSubStrLen,
                CharCount = nCharCount,
                Width = nLineWidth
            });
            return nLineWidth;
        }

        private List<SubLineInfo> GetCacheByWord(TextLine line, ISTTextBoxRender render) {
            var c = base.Core;
            int nCharCount = 0, nCharCountTemp = 0;
            int nLineWidth = 0;
            int nSubStrLen = 0, nIndexStart = 0;
            List<SubLineInfo> lst = new List<SubLineInfo>();
            int nTextViewLineWidth = base.TextRectangle.Width;
            if (this.WrapAlignment != Alignment.Left) {
                nTextViewLineWidth -= c.ITextBoxRender.GetSpaceWidth();
            }
            //if (this.Alignment == UI.STTextBox.Alignment.Justify) {
            //    nTextViewLineWidth -= c.ITextRender.GetSpaceWidth();
            //}
            c.IWordSplitter.Each(line.RawString, (str, nStart, nLen) => {
                string strWord = str.Substring(nStart, nLen);
                nCharCountTemp = 0;
                var wi = this.GetWordInfo(line, render, strWord, nStart, nLineWidth);
                if (wi.Width >= nTextViewLineWidth) {
                    int nCharWidth = 0;
                    c.IGraphemeSplitter.Each(strWord, (s, ns, nl) => {
                        string strChar = s.Substring(ns, nl);
                        if (c.IsEmoji(strChar)) {
                            nCharWidth = c.FontHeight;
                        } else {
                            nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), nLineWidth);
                        }
                        nCharCountTemp++;
                        if (nLineWidth + nCharWidth >= nTextViewLineWidth) {
                            lst.Add(new SubLineInfo() {
                                Index = nIndexStart,
                                Length = nSubStrLen,
                                CharCount = nCharCount,
                                Width = nLineWidth
                            });
                            nIndexStart += nSubStrLen;
                            nSubStrLen = nl;
                            nLineWidth = nCharWidth;
                            nCharCount = nCharCountTemp;
                        } else {
                            nSubStrLen += nl;
                            nLineWidth += nCharWidth;
                            nCharCount += nCharCountTemp;
                        }
                    });
                    return true;
                }
                nCharCountTemp += wi.CharCount;
                if (nLineWidth + wi.Width >= nTextViewLineWidth) {
                    lst.Add(new SubLineInfo() {
                        Index = nIndexStart,
                        Length = nSubStrLen,
                        CharCount = nCharCount,
                        Width = nLineWidth
                    });
                    nIndexStart += nSubStrLen;
                    nSubStrLen = nLen;
                    nLineWidth = wi.Width;
                    nCharCount = nCharCountTemp;
                } else {
                    nSubStrLen += nLen;
                    nLineWidth += wi.Width;
                    nCharCount += nCharCountTemp;
                }
                return true;
            });
            lst.Add(new SubLineInfo() {
                Index = nIndexStart,
                Length = nSubStrLen,
                CharCount = nCharCount,
                Width = nLineWidth
            });
            return lst;
        }

        private WordInfo GetWordInfo(TextLine line, ISTTextBoxRender render, string strWord, int nIndex, int nLeftWidth) {
            var c = base.Core;
            int nWordWidth = 0;
            int nCharCount = 0;
            c.IGraphemeSplitter.Each(strWord, (s, ns, nl) => {
                string strChar = s.Substring(ns, nl);
                if (c.IsEmoji(strChar)) {
                    nWordWidth += c.FontHeight;
                } else {
                    nWordWidth += c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nIndex), nLeftWidth + nWordWidth);
                }
                nCharCount++;
            });
            return new WordInfo() { Width = nWordWidth, CharCount = nCharCount };
        }

        public override void OnTextChanged(ISTTextBoxRender render, TextManagerTextEventArgs e) {
            base.OnTextChanged(render, e);
            var c = base.Core;
            var m = c.TextManager;
            int nIndexMin = int.MaxValue, nLineIndex = 0;
            if (m.LineCount.ToString().Length != m_n_line_count_start_change.ToString().Length) {
                foreach (var line in m) this.CacheLine(line, render);
                m_hs_changed_temp.Clear();
                nIndexMin = 0;
            } else {
                foreach (var v in e.TextHistoryRecord) {
                    if (v.Index < nIndexMin) nIndexMin = v.Index;
                    nLineIndex = m.GetLineIndexFromCharIndex(v.Index);
                    var line = m[nLineIndex];
                    if (m_hs_changed_temp.Contains(line)) {
                        this.CacheLine(line, render);
                        m_hs_changed_temp.Remove(line);
                    }
                }
            }
            foreach (var line in m_hs_changed_temp) this.CacheLine(line, render);
            nLineIndex = m.GetLineIndexFromCharIndex(nIndexMin);
            for (int i = nLineIndex + 1; i < m.LineCount; i++) {
                var tag_prev = _Tag(m[i - 1]);
                _Tag(m[i]).ScreenLineOffset = tag_prev.ScreenLineOffset + tag_prev.SubLines.Count;
            }
        }

        public override void OnTextStartChange(ISTTextBoxRender render, TextManagerTextEventArgs e) {
            m_hs_changed_temp.Clear();
            m_n_line_count_start_change = base.Core.TextManager.LineCount;
        }

        public override FindInfo FindFromPoint(Point pt) {
            var c = base.Core;
            FindInfo fi = new FindInfo();
            int nXStart = base.TextRectangle.X;
            float nX = 0;// nXStart - c.Scroll.XOffset;
            int nIndexChar = 0, nCharWidth = 0;
            int nYOffset = ((pt.Y - base.TextRectangle.Y) / c.LineHeight);
            int nYValue = c.Scroll.YValue + nYOffset;
            if (nYValue < 0) nYValue = 0;
            int nIndexLine = this.GetLineIndexFromYScroll(nYValue);
            TextLine line = c.TextManager[nIndexLine];
            var tag = _Tag(line);
            //SubLineInfo subLine = tag.SubLines[0];
            if (nYValue >= tag.ScreenLineOffset + tag.SubLines.Count) {
                nYOffset -= nYValue - tag.ScreenLineOffset - tag.SubLines.Count + 1;
            }
            //if (tag.ScreenLineOffset < nYValue) {
            var temp = nYValue - tag.ScreenLineOffset;
            if (temp >= tag.SubLines.Count) {
                temp = tag.SubLines.Count - 1;
            }
            var subLine = tag.SubLines[temp];
            //}
            //nX += subLine.X;
            fi.Line = line;
            fi.IndexOfLine = nIndexLine;
            //var nXXX = subLine.ExtraWidth * 2;
            c.ITextBoxRender.BeginPaint();
            c.IGraphemeSplitter.Each(line.RawString.Substring(subLine.Index, subLine.Length), nIndexChar, (str, nStart, nLen) => {
                if (str[nStart] == '\r') return false;
                if (str[nStart] == '\n') return false;
                string strChar = str.Substring(nStart, nLen);
                nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), (int)nX/* - base.TextRectangle.X + c.Scroll.XOffset*/);
                //nCharWidth += (int)nXXX;
                if (nX + subLine.X + base.TextRectangle.X + nCharWidth > pt.X) {
                    fi.Find = true;
                    if (pt.X - base.TextRectangle.X - subLine.X - nX >= nCharWidth / 2) {
                        fi.IndexOfCharInLine = subLine.Index + nIndexChar + nLen;
                        fi.Location = new Point((int)nX + nCharWidth + base.TextRectangle.X + subLine.X, base.TextRectangle.Y + nYOffset * c.LineHeight);
                    } else {
                        fi.IndexOfCharInLine = subLine.Index + nIndexChar;
                        fi.Location = new Point((int)nX + base.TextRectangle.X + subLine.X, base.TextRectangle.Y + nYOffset * c.LineHeight);
                    }
                    return false;
                }
                nX += nCharWidth;
                nIndexChar += nLen;
                return true;
            });
            c.ITextBoxRender.EndPaint();
            if (!fi.Find) {
                fi.IndexOfCharInLine = subLine.Index + nIndexChar;
                fi.Location = new Point((int)nX + base.TextRectangle.X + subLine.X, base.TextRectangle.Y + nYOffset * c.LineHeight);
            }
            fi.Find = true;
            //Console.WriteLine("->>>>>>>>>>>>>>>>>>>>>" + line.RawString.Substring(subLine.Index, subLine.Length) + " -- " + fi.IndexOfChar);
            return fi;
        }

        public override FindInfo FindFromCharIndex(int nIndex) {
            var c = base.Core;
            int nIndexOfLine = c.TextManager.GetLineIndexFromCharIndex(nIndex);
            TextLine stLine = c.TextManager[nIndexOfLine];
            if (stLine == null) {// TODO: stLine will not be null
                return new FindInfo();
            }
            return this.FindFromCharIndex(nIndexOfLine, nIndex - stLine.IndexOfFirstChar);
        }

        private FindInfo FindFromCharIndex(int nIndexOfLine, int nIndex) {
            var c = base.Core;
            return this.FindFromCharIndex(c.ITextBoxRender, nIndexOfLine, nIndex);
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
            int nX = base.TextRectangle.X;
            int nCharWidth = 0;
            var tag = _Tag(line);
            int nSubIndex = 0;
            foreach (var sub in tag.SubLines) {
                if (nIndex >= sub.Index && nIndex < sub.Index + sub.Length) {
                    break;
                }
                nSubIndex++;
            }
            var subLine = tag.SubLines[nSubIndex];
            c.IGraphemeSplitter.Each(line.RawString.Substring(subLine.Index, subLine.Length), (str, nStart, nLen) => {
                if (subLine.Index + nStart >= nIndex) {
                    return false;
                }
                string strChar = str.Substring(nStart, nLen);
                nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + subLine.Index + nStart), nX - base.TextRectangle.X + c.Scroll.XOffset);
                nX += nCharWidth;
                return true;
            });
            return new FindInfo() {
                Line = line,
                IndexOfLine = nIndexOfLine,
                IndexOfCharInLine = nIndex,
                Location = new Point((int)nX + subLine.X, base.TextRectangle.Y + (tag.ScreenLineOffset + nSubIndex - c.Scroll.YValue) * c.LineHeight)
            };
        }

        protected override int OnDrawLine(ISTTextBoxRender render, TextLine line, int nX, int nY) {
            var c = base.Core;
            int nRet = 0;
            int nLineSpacing = c.TextBox.GetIntYSize(c.TextBox.LineSpacing) / 2;
            //nY += c.TextBox.GetIntYSize(c.TextBox.LineSpacing) / 2;
            float nXStart = 0;
            int nIndexEach = 0, nCharWidth;
            if (string.IsNullOrEmpty(line.RawString)) {
                return c.LineHeight;
            }
            int nCharSpacing = c.TextBox.GetIntXSize(c.TextBox.CharSpacing) / 2;
            int nCurrentCharIndex = line.IndexOfFirstChar + nIndexEach;
            Rectangle rect_char = new Rectangle(0, nY, 0, c.LineHeight);
            var tag = _Tag(line);
            var temp = tag.ScreenLineOffset < c.Scroll.YValue ? c.Scroll.YValue - tag.ScreenLineOffset : 0;
            for (int i = temp; i < tag.SubLines.Count; i++) {
                var subLine = tag.SubLines[i];
                nXStart = nX + subLine.X;
                rect_char.Y = nY;
                rect_char.X = nX + subLine.X;
                bool bIsEmoji = false;
                TextStyleRange tsi = TextStyleRange.Empty;
                c.IGraphemeSplitter.Each(line.RawString.Substring(subLine.Index, subLine.Length), nIndexEach, (str, nStart, nLen) => {
                    nCurrentCharIndex = line.IndexOfFirstChar + subLine.Index + nStart;
                    string strChar = str.Substring(nStart, nLen);
                    if (c.IsEmoji(strChar)) {
                        bIsEmoji = true;
                        nCharWidth = c.FontHeight;
                    } else {
                        if (nLen > 1 && strChar[nLen - 1] >= '\uFE00' && strChar[nLen - 1] <= '\uFE0F') {
                            strChar = str.Substring(nStart, nLen - 1);
                        }
                        bIsEmoji = false;
                        nCharWidth = c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), rect_char.X - subLine.X - base.TextRectangle.X) + nCharSpacing;
                    }
                    if (nXStart + nCharWidth <= base.TextRectangle.Left) {
                        nXStart += nCharWidth;
                        return true;
                    }
                    rect_char.Width = nCharWidth;
                    if (bIsEmoji) {
                        bool bSelected = nCurrentCharIndex >= c.Selection.StartIndex && nCurrentCharIndex < c.Selection.EndIndex;
                        c.IEmojiRender.DrawEmoji(render, strChar, rect_char.X + (nCharSpacing >> 1), nY + nLineSpacing, c.FontHeight, bSelected);
                    } else {
                        c.ITextBoxRender.DrawString(strChar, c.GetStyleFromCharIndex(nCurrentCharIndex), rect_char);
                    }
                    rect_char.X += rect_char.Width;
                    nXStart += nCharWidth;
                    if (rect_char.X >= base.TextRectangle.Right) {
                        return false;
                    }
                    return true;
                });
                nY += c.LineHeight;
                nRet += c.LineHeight;
                if (nY >= base.TextRectangle.Bottom) break;
            }
            return nRet;
        }

        protected override void OnDrawHead(ISTTextBoxRender render, int nStartLineIndex) {
            var c = base.Core;
            TextLine line = c.TextManager[nStartLineIndex];
            var tag = _Tag(line);
            var temp = tag.ScreenLineOffset < c.Scroll.YValue ? c.Scroll.YValue - tag.ScreenLineOffset : 0;

            render.FillRectangle(base.HeadBackgroundColor, base.HeadRectangle);

            var clr_temp = Color.FromArgb(base.LineNumberColor.A / 3, base.LineNumberColor);
            int nY = base.HeadRectangle.Y;
            this.DrawHeadLine(render, line, this.GetSubLineStatus(tag.SubLines.Count, temp), nStartLineIndex + 1, base.LineNumberColor, nY);
            nY += c.LineHeight;
            for (int i = temp + 1; i < tag.SubLines.Count; i++) {
                if (nY >= base.HeadRectangle.Bottom) return;
                this.DrawHeadLine(render, line, this.GetSubLineStatus(tag.SubLines.Count, i), nStartLineIndex + 1, clr_temp, nY);
                nY += c.LineHeight;
            }

            for (int i = nStartLineIndex + 1; i < c.TextManager.LineCount; i++) {
                line = c.TextManager[i];
                tag = _Tag(line);
                this.DrawHeadLine(render, line, this.GetSubLineStatus(tag.SubLines.Count, 0), i + 1, base.LineNumberColor, nY);
                nY += c.LineHeight;
                for (int j = 1; j < tag.SubLines.Count; j++) {
                    if (nY >= base.HeadRectangle.Bottom) return;
                    this.DrawHeadLine(render, line, this.GetSubLineStatus(tag.SubLines.Count, j), i + 1, clr_temp, nY);
                    nY += c.LineHeight;
                }
            }
        }

        private void DrawHeadLine(ISTTextBoxRender render, TextLine line, SubLineStatus status, int nShowNumber, Color clr, int nY) {
            var c = base.Core;
            //int nStatusWidth = c.TextBox.GetIntXSize(base.LineStatusWidth);
            Rectangle rect_linenumber = new Rectangle(base.HeadRectangle.X, nY, base.LineNumberWidth - c.TextBox.GetIntXSize(10), c.LineHeight);
            //Rectangle rect_status = new Rectangle(base.HeadRectangle.Right - nStatusWidth, nY, nStatusWidth, c.LineHeight);
            Rectangle rect_temp = new Rectangle(base.HeadRectangle.X + c.TextBox.GetIntXSize(5), nY, c.TextBox.GetIntXSize(1), c.LineHeight);
            if (this.ShowLineNumberGuide) {
                var clr_temp = this.LineNumberGuideColor;
                switch (status) {
                    case SubLineStatus.Start:
                        rect_temp.Y += c.LineHeight / 2;
                        rect_temp.Height -= rect_temp.Y - nY;
                        render.FillRectangle(clr_temp, rect_temp.Right, rect_temp.Y, rect_temp.Width * 3, rect_temp.Width);
                        break;
                    //case SubLineStatus.Middle:
                    case SubLineStatus.End:
                        rect_temp.Height -= c.LineHeight / 2;
                        render.FillRectangle(clr_temp, rect_temp.Right, rect_temp.Bottom - rect_temp.Width, rect_temp.Width * 3, rect_temp.Width);
                        break;
                    case SubLineStatus.None:
                        rect_temp.Y += rect_temp.Width;
                        rect_temp.Height -= rect_temp.Width * 2;
                        rect_temp.Width = c.TextBox.GetIntXSize(4);
                        clr_temp = Color.FromArgb(clr_temp.A / 3, clr_temp);
                        break;
                }
                render.FillRectangle(clr_temp, rect_temp);
            }
            render.DrawString(nShowNumber.ToString(), c.TextBox.Font, clr, rect_linenumber, m_sf);
            //if (base.ShowLineStatus) {
            //    if (m_hs_changed.Contains(line)) {
            //        render.FillRectangle(base.LineStatusColor, rect_status);
            //    }
            //}
        }

        private SubLineStatus GetSubLineStatus(int nSubLineCount, int nIndex) {
            if (nSubLineCount == 1) {
                return SubLineStatus.None;
            } else if (nIndex == 0) {
                return SubLineStatus.Start;
            } else if (nIndex == nSubLineCount - 1) {
                return SubLineStatus.End;
            } else {
                return SubLineStatus.Middle;
            }
        }

        public override int GetLineIndexFromYScroll(int nYValue) {
            var m = base.Core.TextManager;
            int nLeft = 0, nRight = m.LineCount - 1, nMid = 0;
            while (nLeft <= nRight) {
                nMid = (nLeft + nRight) >> 1;
                var tag = _Tag(m[nMid]);
                if (nYValue < tag.ScreenLineOffset) {
                    nRight = nMid - 1;
                } else if (nYValue >= tag.ScreenLineOffset + tag.SubLines.Count) {
                    nLeft = nMid + 1;
                } else {
                    return nMid;
                }
            }
            if (nMid < 0) return 0;
            if (nMid >= m.LineCount) return m.LineCount - 1;
            return nMid;
        }

        public override void SetCaretPostion(int nCharIndex) {
            var c = this.Core;
            int nLineIndex = c.TextManager.GetLineIndexFromCharIndex(nCharIndex);
            var line = c.TextManager[nLineIndex];
            int nWidth = 0, nTempCharIndex = 0, nSubLineIndex = 0;
            var tag = _Tag(line);

            foreach (var v in tag.SubLines) {
                nTempCharIndex = line.IndexOfFirstChar + v.Index;
                if (nCharIndex >= nTempCharIndex && nCharIndex < nTempCharIndex + v.Length) {
                    break;
                }
                nSubLineIndex++;
            }
            if (nSubLineIndex >= tag.SubLines.Count) {
                nSubLineIndex = tag.SubLines.Count - 1;
            }
            nTempCharIndex = nCharIndex;
            var subLine = tag.SubLines[nSubLineIndex];
            c.ITextBoxRender.BeginPaint();
            c.IGraphemeSplitter.Each(line.RawString, subLine.Index, (str, nStart, nLen) => {
                if (nStart >= nCharIndex - line.IndexOfFirstChar) return false;
                string strChar = str.Substring(nStart, nLen);
                nWidth += c.GetStringWidth(strChar, c.GetStyleFromCharIndex(line.IndexOfFirstChar + nStart), nWidth);
                nTempCharIndex = line.IndexOfFirstChar + nStart + nLen;
                return true;
            });
            c.ITextBoxRender.EndPaint();
            c.Caret.Line = line;
            c.Caret.IndexOfLine = nLineIndex;
            c.Caret.IndexOfChar = nTempCharIndex;
            this.SetCaretPostion((int)nWidth + this.TextRectangle.X + subLine.X, this.TextRectangle.Y + (tag.ScreenLineOffset + nSubLineIndex - c.Scroll.YValue) * c.LineHeight);
        }

        protected override void OnDrawLineSelectionBackground(ISTTextBoxRender render, TextLine line, int nLineIndex, STTextBoxSelectionInfo selection, int nY) {
            var c = base.Core;
            int nRet = 0;
            //nY += c.TextBox.GetIntYSize(c.TextBox.LineSpacing) / 2;
            if (string.IsNullOrEmpty(line.RawString)) {
                return;
            }
            int nCharSpacing = c.TextBox.GetIntXSize(c.TextBox.CharSpacing) / 2;
            var tag = _Tag(line);
            var temp = tag.ScreenLineOffset < c.Scroll.YValue ? c.Scroll.YValue - tag.ScreenLineOffset : 0;
            for (int i = temp; i < tag.SubLines.Count; i++) {
                var subLine = tag.SubLines[i];
                if (line.IndexOfFirstChar + subLine.Index >= selection.EndIndex) {
                    return;
                }
                if (line.IndexOfFirstChar + subLine.Index + subLine.Length <= selection.StartIndex) {
                    if (nY >= base.TextRectangle.Bottom) break;
                    nY += c.LineHeight;
                    nRet += c.LineHeight;
                    continue;
                }
                int nLeft = base.TextRectangle.X + subLine.X, nWidth = subLine.Width;
                if (selection.StartIndex > line.IndexOfFirstChar + subLine.Index) {
                    nLeft = this.FindFromCharIndex(render, nLineIndex, selection.StartIndex - line.IndexOfFirstChar).Location.X;
                    nWidth -= nLeft - base.TextRectangle.X - subLine.X;
                }
                if (selection.EndIndex < line.IndexOfFirstChar + subLine.Index + subLine.Length) {
                    int nCurrentIndex = line.IndexOfFirstChar + subLine.Index;
                    nWidth = this.FindFromCharIndex(render, nLineIndex, selection.EndIndex - line.IndexOfFirstChar).Location.X;
                    nWidth -= nLeft;
                }
                render.FillRectangle(c.TextBox.SelectionColor, nLeft, nY, nWidth, c.LineHeight);
                nY += c.LineHeight;
                nRet += c.LineHeight;
                if (nY >= base.TextRectangle.Bottom) break;
            }
        }

        public override void OnResize(EventArgs e) {
            base.OnResize(e);
            var c = base.Core;
            int nLineOffset = 0;
            c.ITextBoxRender.BeginPaint();
            foreach (TextLine line in c.TextManager) {
                var tag = _Tag(line);
                tag.ScreenLineOffset = nLineOffset;
                tag.SubLines = this.GetCache(line, c.ITextBoxRender);
                nLineOffset += tag.SubLines.Count;
            }
            c.ITextBoxRender.EndPaint();
            //this.ResizeScroll();
            //this.SetCaretPostion(c.Caret.IndexOfChar);
        }

        public override void OnCalcScroll(STTextBoxScrollInfo scrollInfo) {
            var c = base.Core;
            var m = c.TextManager;
            var tag = _Tag(m[m.LineCount - 1]);
            c.Scroll.MaxYValue = tag.ScreenLineOffset + tag.SubLines.Count - base.TextRectangle.Height / c.LineHeight + 2;
            if (c.Scroll.MaxYValue < 0) {
                c.Scroll.MaxYValue = 0;
            }
            if (c.Scroll.YValue > c.Scroll.MaxYValue) {
                c.Scroll.YValue = c.Scroll.MaxYValue;
            }
            //this.SetCaretPostion(c.Caret.IndexOfChar);
        }
    }
}
