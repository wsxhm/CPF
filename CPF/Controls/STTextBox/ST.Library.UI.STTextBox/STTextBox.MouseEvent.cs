using CPF;
using CPF.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    partial class STTextBox
    {
        private struct ScrollIncrement
        {
            public int Time;
            public int Value;

            public ScrollIncrement(int nTime, int nValue) {
                this.Time = nTime;
                this.Value = nValue;
            }
        }

        private DateTime m_dt_last_scroll_h = DateTime.Now;
        private DateTime m_dt_last_scroll_v = DateTime.Now;
        private ScrollIncrement[] m_arr_si = new ScrollIncrement[]{
            new ScrollIncrement(20, 6),
            new ScrollIncrement(30, 5),
            new ScrollIncrement(40, 4),
            new ScrollIncrement(50, 3),
            new ScrollIncrement(60, 2),
        };

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            var c = m_core;
            this.Focus();//设置焦点
            if (c.Scroll.HoverScrollBar != STTextBoxScrollInfo.ScrollBarType.None) {
                if (c.Scroll.HBackRect.Contains(e.Location)) {
                    c.Scroll.DownScrollBar = STTextBoxScrollInfo.ScrollBarType.H;
                    c.ITextView.ScrollXToMuosePoint(e.Location.X);
                    this.Invalidate();
                } else if (c.Scroll.VBackRect.Contains(e.Location)) {
                    c.Scroll.DownScrollBar = STTextBoxScrollInfo.ScrollBarType.V;
                    c.ITextView.ScrollYToMousePoint(e.Location.Y);
                    this.Invalidate();
                }
                return;
            }

            var fi = c.ITextView.SetCaretPostion(e.Location);// c.ITextView.FindFromPoint(e.Location);
            //c.Caret.CopyFromFindInfo(fi);
            var key = Root.InputManager.KeyboardDevice.Modifiers;
            if (key != InputModifiers.Shift) {
                c.Selection.SetIndex(c.Caret.IndexOfChar);
            } else {
                //if (c.Caret.IndexOfChar < c.Selection.AnchorIndex) {
                //    //c.Selection.StartIndex = c.Caret.IndexOfChar;
                //    //c.Selection.EndIndex = c.Selection.AnchorIndex;
                //    c.Selection.SetSelection(c.Selection.AnchorIndex, c.Caret.IndexOfChar);
                //} else {
                //    c.Selection.StartIndex = c.Selection.AnchorIndex;
                //    c.Selection.EndIndex = c.Caret.IndexOfChar;
                //}
                c.Selection.SetSelection(c.Selection.AnchorIndex, c.Caret.IndexOfChar);
            }
            //c.ITextView.SetCaretPostion(c.Caret.X, c.Caret.Y);
            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            var c = m_core;
            switch (c.Scroll.DownScrollBar) {
                case STTextBoxScrollInfo.ScrollBarType.V:
                    c.ITextView.ScrollYToMousePoint(e.Location.Y);
                    this.Invalidate();
                    return;
                case STTextBoxScrollInfo.ScrollBarType.H:
                    c.ITextView.ScrollXToMuosePoint(e.Location.X);
                    this.Invalidate();
                    return;
            }
            if (e.LeftButton == MouseButtonState.Pressed) {
                //var sw = new System.Diagnostics.Stopwatch();
                //sw.Start();
                var fi = c.ITextView.FindFromPoint(e.Location);
                if (!fi.Find) return;
                c.Caret.CopyFromFindInfo(fi);
                int nIndex = c.Caret.IndexOfChar;
                //if (nIndex > c.Selection.AnchorIndex) {
                //    c.Selection.SetSelection(c.Selection.AnchorIndex, nIndex);
                //    //c.Selection.StartIndex = c.Selection.AnchorIndex;
                //    //c.Selection.EndIndex = nIndex;
                //} else {
                //    c.Selection.SetSelection(nIndex, c.Selection.AnchorIndex);
                //    //c.Selection.StartIndex = nIndex;
                //    //c.Selection.EndIndex = c.Selection.AnchorIndex;
                //}
                c.Selection.SetSelection(c.Selection.AnchorIndex, nIndex);
                c.ITextView.SetCaretPostion(fi.IndexOfChar);
                c.ITextView.ScrollToCaret();
                //sw.Stop();
                //Console.WriteLine("CheckSelection: - " + sw.ElapsedMilliseconds);
                this.Invalidate();
                return;
            }
            //没有鼠标点击
            if (e.LeftButton == MouseButtonState.Released&& e.RightButton == MouseButtonState.Released && c.Scroll.CountDown != 0) {
                if (c.Scroll.VBackRect.Contains(e.Location)) {
                    c.Scroll.HoverScrollBar = STTextBoxScrollInfo.ScrollBarType.V;
                } else if (c.Scroll.HBackRect.Contains(e.Location)) {
                    c.Scroll.HoverScrollBar = STTextBoxScrollInfo.ScrollBarType.H;
                } else {
                    c.Scroll.HoverScrollBar = STTextBoxScrollInfo.ScrollBarType.None;
                }
                if (c.Scroll.HoverScrollBar != STTextBoxScrollInfo.ScrollBarType.None) {
                    this.ShowScrollBar(c.Scroll.DisplayTime);
                    if (this.SetCursor(Cursors.Arrow)) {
                        this.Invalidate();
                    }
                } else {
                    if (this.SetCursor(Cursors.Ibeam)) {
                        this.Invalidate();
                    }
                }
            } else {
                c.ITextView.OnSetCursor(e);
            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            var c = m_core;
            c.Scroll.DownScrollBar = STTextBoxScrollInfo.ScrollBarType.None;
            this.Invalidate();
        }
        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
            var c = m_core;
            c.Scroll.HoverScrollBar = STTextBoxScrollInfo.ScrollBarType.None;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            DateTime dt_now = DateTime.Now;
            //Console.WriteLine("Scroll: --------------------- " + dt_now.Subtract(m_dt_last_scroll_v).TotalMilliseconds);
            int nIncrement = 1;
            int nTemp = (int)dt_now.Subtract(m_dt_last_scroll_v).TotalMilliseconds;
            foreach (var v in m_arr_si) {
                if (nTemp < v.Time) {
                    nIncrement = v.Value;
                    break;
                }
            }
            m_dt_last_scroll_v = dt_now;
            var c = m_core;
            if (e.Delta.Length > 0) {//应该是滚动距离判断
                if (c.Scroll.YValue <= 0) return;
                if (c.Scroll.YValue - nIncrement < 0) {
                    nIncrement = c.Scroll.YValue;
                }
                c.Scroll.YValue -= nIncrement;
                c.Caret.Y += nIncrement * c.LineHeight;// this.Font.Height;
            } else {
                if (c.Scroll.YValue >= c.Scroll.MaxYValue) {
                    return;
                }
                if (c.Scroll.YValue + nIncrement > c.Scroll.MaxYValue) {
                    nIncrement = c.Scroll.MaxYValue - c.Scroll.YValue;
                }
                c.Scroll.YValue += nIncrement;
                c.Caret.Y -= c.LineHeight * nIncrement;// c.ITextRender.GetFontHeight();
            }
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            //c.ITextView.SetCaretPostion(c.Caret.X, c.Caret.Y);
            this.ShowScrollBar(c.Scroll.DisplayTime);
            this.Invalidate();
            //c.Scroll.Timer = 5;
        }
        /*protected virtual void OnMouseHWheel(MouseEventArgs e) {
            DateTime dt_now = DateTime.Now;
            //Console.WriteLine("Scroll: --------------------- " + dt_now.Subtract(m_dt_last_scroll_h).TotalMilliseconds);
            int nIncrement = 1;
            int nTemp = (int)dt_now.Subtract(m_dt_last_scroll_h).TotalMilliseconds;
            foreach (var v in m_arr_si) {
                if (nTemp < v.Time) {
                    nIncrement = v.Value;
                    break;
                }
            }
            m_dt_last_scroll_h = dt_now;
            var c = m_core;
            if (e.Delta < 0) {
                if (c.Scroll.XValue <= 0) return;
                if (c.Scroll.XValue - nIncrement < 0) {
                    nIncrement = c.Scroll.XValue;
                }
                c.Scroll.XValue -= nIncrement;
                c.Caret.X += c.Scroll.XIncrement * nIncrement;
            } else {
                if (c.Scroll.XValue >= c.Scroll.MaxXValue) {
                    return;
                }
                if (c.Scroll.XValue + nIncrement > c.Scroll.MaxXValue) {
                    nIncrement = c.Scroll.MaxXValue - c.Scroll.XValue;
                }
                c.Scroll.XValue += nIncrement;
                c.Caret.X -= c.Scroll.XIncrement * nIncrement;
            }
            //c.ITextView.SetCaretPostion(c.Caret.X, c.Caret.Y);
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            this.ShowScrollBar(c.Scroll.DisplayTime);
            this.Invalidate();
            //c.Scroll.Timer = 5;
        }*/
        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            var c = m_core;
            var fi = c.ITextView.FindFromPoint(e.Location);
            int nIndex = fi.IndexOfChar - fi.Line.IndexOfFirstChar;
            int nSelectionStart = fi.IndexOfChar, nSelectionLen = 0;
            c.IWordSplitter.Each(fi.Line.RawString, (str, nStart, nLen) => {
                if (nIndex >= nStart && nIndex < nStart + nLen)
                {
                    switch (str[nStart])
                    {
                        case '\r':
                        case '\n':
                            return false;
                    }
                    nSelectionStart = fi.Line.IndexOfFirstChar + nStart;
                    nSelectionLen = nLen;
                    return false;
                }
                nSelectionStart = fi.Line.IndexOfFirstChar + nStart;
                nSelectionLen = nLen;
                return true;
            });
            c.Selection.SetSelection(nSelectionStart, nSelectionStart + nSelectionLen);
            c.Caret.IndexOfChar = c.Selection.EndIndex;
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            this.Invalidate();
        }
    }
}
