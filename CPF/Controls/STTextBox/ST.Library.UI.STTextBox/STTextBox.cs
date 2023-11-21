using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CPF.Controls;
using CPF.Drawing;
using System.Timers;
using CPF;
using CPF.Input;

namespace ST.Library.UI.STTextBox
{
    public partial class STTextBox : Control
    {
        //var dpi = Root.LayoutScaling;
        public Font Font { 
            set { 
                this.FontFamily = value.FontFamily;
                this.FontSize = value.FontSize;
                this.FontStyle = value.FontStyle;
            }
            get {
                return new Font(this.FontFamily, this.FontSize, this.FontStyle);
            }
        }
        public float DPIZoom { get { return Root.RenderScaling; } }

        private int _BorderWidth = 1;

        public int BorderWidth {
            get { return _BorderWidth; }
            set { _BorderWidth = value; }
        }

        private int _LineSpacing = 0;

        public int LineSpacing {
            get { return _LineSpacing; }
            set {
                if (value < 0) {
                    throw new ArgumentException("The value must be more than zero");
                }
                _LineSpacing = value;
                this.Invalidate();
            }
        }

        private int _CharSpacing = 0;

        public int CharSpacing {
            get { return _CharSpacing; }
            set {
                if (value < 0) {
                    throw new ArgumentException("The value must be more than zero");
                }
                _CharSpacing = value;
                this.Invalidate();
            }
        }

        private Color _BorderColor = Color.FromArgb(125, 0, 0, 0);

        public Color BorderColor {
            get { return _BorderColor; }
            set {
                if (_BorderColor == value) {
                    return;
                }
                _BorderColor = value;
                this.Invalidate();
            }
        }

        private Color _SelectionColor = Color.FromArgb(125, Color.DarkGray.R, Color.DarkGray.G, Color.DarkGray.B);

        public Color SelectionColor {
            get { return _SelectionColor; }
            set {
                if (value == _SelectionColor) {
                    return;
                }
                _SelectionColor = value;
                if (!m_core.Selection.IsEmptySelection) {
                    this.Invalidate();
                }
            }
        }

        private bool _AllowScrollBar = true;

        public bool AllowScrollBar {
            get { return _AllowScrollBar; }
            set { _AllowScrollBar = value; }
        }

        private Color _ScrollbarBackColor = Color.FromArgb(40, Color.Gray.R, Color.Gray.G, Color.Gray.B);

        public Color ScrollbarBackColor {
            get { return _ScrollbarBackColor; }
            set { _ScrollbarBackColor = value; }
        }

        private Color _ScrollBarCornerColor =Color.FromArgb(80, Color.Gray.R, Color.Gray.G, Color.Gray.B);

        public Color ScrollBarCornerColor {
            get { return _ScrollBarCornerColor; }
            set { _ScrollBarCornerColor = value; }
        }

        private Color _ScrollbarThumbHoverColor = Color.FromArgb(80, Color.Gray.R, Color.Gray.G, Color.Gray.B);

        public Color ScrollbarThumbHoverColor {
            get { return _ScrollbarThumbHoverColor; }
            set { _ScrollbarThumbHoverColor = value; }
        }

        private Color _ScrollbarThumbBackColor = Color.FromArgb(80, Color.Black.R, Color.Black.G, Color.Black.B);

        public Color ScrollbarThumbBackColor {
            get { return _ScrollbarThumbBackColor; }
            set { _ScrollbarThumbBackColor = value; }
        }

        private int _TabSize = 4;

        private bool _AutoIndent = true;

        private bool _TabToSpace = false;

        private const int WM_MOUSEHWHEEL = 0x020E;
        private object m_obj_sync = new object();
        private Timer m_timer;
        private IntPtr m_hIMC;
        protected Core m_core;


        public STTextBox() {
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Background = Color.White;
            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Elapsed += m_timer_Tick;
            m_core = new Core(this);
        }

        #region override

        

        protected override void OnRender(DrawingContext e) {
            base.OnRender(e);
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            var render = m_core.ITextBoxRender;
            //try {
            render.OnBeginPaint(new Graphics(e));
            m_core.ITextView.OnDrawView(render);
            if (this._AllowScrollBar && m_core.Scroll.CountDown != 0) {
                this.OnCalcScrollRectangle(m_core.Scroll);
                bool bFlag = true;
                if (m_core.Scroll.HBackRect != m_core.Scroll.HThumbRect) {
                    this.OnDrawHScrollBar(render, m_core.Scroll);
                } else { bFlag = false; }
                if (m_core.Scroll.VBackRect != m_core.Scroll.VThumbRect) {
                    this.OnDrawVScrollBar(render, m_core.Scroll);
                } else { bFlag = false; }
                if (bFlag) this.OnDrawScrollBarCorner(render, m_core.Scroll);
            }
            this.OnDrawBorder(render);
            render.OnEndPaint(new Graphics(e));
            //} catch (Exception ex) {
            //    MessageBox.Show(ex.Message + "\r\n\r\n" + ex.StackTrace);
            //}
            //sw.Stop();
            //Console.WriteLine("OnPaint - " + sw.ElapsedMilliseconds + "ms");
        }

        protected virtual void OnDrawBorder(ISTTextBoxRender render) {
            if (this._BorderWidth <= 0) {
                return;
            }
            int nWidth = this.GetIntXSize(this._BorderWidth);
            int nHeight = this.GetIntYSize(this._BorderWidth);
            if (nWidth == 0) nWidth = 1;
            if (nHeight == 0) nHeight = 1;

            render.FillRectangle(this._BorderColor, 0, 0, nWidth, this.Height.Value);
            render.FillRectangle(this._BorderColor, this.Width.Value - nWidth, 0, nWidth, this.Height.Value);
            render.FillRectangle(this._BorderColor, nWidth, 0, this.Width.Value - nWidth * 2, nHeight);
            render.FillRectangle(this._BorderColor, nWidth, this.Height.Value - nHeight, this.Width.Value - nWidth * 2, nHeight);
        }

        /*protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            int nX = this.GetIntXSize(this._BorderWidth);
            int nY = this.GetIntYSize(this._BorderWidth);
            var rect = new Rect(nX, nY, this.Width.Value - nX * 2, this.Height.Value - nY * 2);
            if (rect.Height < 1 || rect.Width < 1 || rect == m_core.ViewRectangle) {
                return;
            }
            m_core.ViewRectangle = rect;
            m_core.ITextView.OnCalcTextRectangle();
            m_core.ITextView.OnResize(e);
            m_core.ITextView.OnCalcScroll(m_core.Scroll);
        }*/
        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            int nX = this.GetIntXSize(this._BorderWidth);
            int nY = this.GetIntYSize(this._BorderWidth);
            var rect = new Rect(nX, nY, this.Width.Value - nX * 2, this.Height.Value - nY * 2);
            if (rect.Height < 1 || rect.Width < 1 || rect == m_core.ViewRectangle)
            {
                return;
            }
            m_core.ViewRectangle = rect;
            m_core.ITextView.OnCalcTextRectangle();
            m_core.ITextView.OnResize(null);//hmbb
            m_core.ITextView.OnCalcScroll(m_core.Scroll);
        }
        //hmbb 光标 暂时先不用
        /*protected override void OnGotFocus(GotFocusEventArgs e) {
            base.OnGotFocus(e);
            Win32.CreateCaret(this.Handle, IntPtr.Zero, this.GetIntXSize(1), m_core.LineHeight);
            Win32.ShowCaret(this.Handle);
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            Win32.HideCaret(this.Handle);
            Win32.DestroyCaret();
        }*/

        #endregion

        #region public

        public float GetFloatXSize(int nSize) { return nSize * this.DPIZoom; }
        public float GetFloatXSize(float fSize) { return fSize * this.DPIZoom; }
        public int GetIntXSize(int nSize) { return (int)Math.Round(nSize * this.DPIZoom); }
        public int GetIntXSize(float fSize) { return (int)Math.Round(fSize * this.DPIZoom); }

        public float GetFloatYSize(int nSize) { return nSize * this.DPIZoom; }
        public float GetFloatYSize(float fSize) { return fSize * this.DPIZoom; }
        public int GetIntYSize(int nSize) { return (int)Math.Round(nSize * this.DPIZoom); }
        public int GetIntYSize(float fSize) { return (int)Math.Round(fSize * this.DPIZoom); }

        public void ShowScrollBar(int nSecond) {
            lock (m_obj_sync) {
                m_core.Scroll.CountDown = nSecond;
            }
            m_timer.Start();
        }

        public TextHistoryRecord SetText(string strText) {
            return m_core.TextManager.SetText(strText);
        }

        public TextHistoryRecord SetText(int nIndex, string strText) {
            return m_core.TextManager.SetText(nIndex, strText);
        }

        public TextHistoryRecord SetText(int nIndex, int nLen, string strText) {
            return m_core.TextManager.SetText(nIndex, nLen, strText);
        }

        public void SelectAll() {
            m_core.Selection.SetSelection(0, m_core.TextManager.TextLength);
            m_core.Caret.IndexOfChar = m_core.Selection.EndIndex;
            m_core.ITextView.SetCaretPostion(m_core.Caret.IndexOfChar);
            this.Invalidate();
        }

        public void Copy() {
            this.Copy(false);
        }

        public void Cut() {
            this.Copy(true);
        }

        private void Copy(bool bCut) {
            var c = m_core;
            if (c.Selection.IsEmptySelection) {
                return;
            }
            var data = c.TextManager.GetText(c.Selection.StartIndex, c.Selection.Length);
            Clipboard.SetData((DataFormat.Text, data));
            if (bCut) {
                this.EnterText("");
            }
        }

        public void Paste() {
            string strText = Clipboard.GetData(DataFormat.Text).ToString();
            if (string.IsNullOrEmpty(strText)) {
                return;
            }
            this.EnterText(strText);
        }

        public void Undo() {
            this.RunHistory(true);
        }

        public void Redo() {
            this.RunHistory(false);
        }

        private void RunHistory(bool isUndo) {
            var c = m_core;
            if (c.ITextHistory == null) {
                return;
            }
            var histories = isUndo ? c.ITextHistory.GetUndo() : c.ITextHistory.GetRedo();
            if (histories == null || histories.Length == 0) {
                return;
            }
            if (isUndo) {
                var temp = new TextHistoryRecord[histories.Length];
                for (int i = 0; i < histories.Length; i++) {
                    temp[i] = histories[i];
                    temp[i].NewText = histories[i].OldText;
                    temp[i].OldText = histories[i].NewText;
                }
                histories = temp;
            }
            histories = c.TextManager.RunHistory(histories);
            var last = histories[histories.Length - 1];
            c.Selection.SetSelection(last.Index, last.Index + last.NewText.Length);
            c.Caret.IndexOfChar = m_core.Selection.EndIndex;
            c.ITextView.SetCaretPostion(c.Caret.IndexOfChar);
            c.ITextView.ScrollToCaret();
            this.Invalidate();
        }

        #endregion

        #region protected

        protected virtual void OnImeStart(IntPtr hIMC) { }
        protected virtual void OnImeResultStr(IntPtr hIMC, string strResult) { this.EnterText(strResult); }
        protected virtual void OnImeCompStr(IntPtr hIMC, string strComp) { }
        protected virtual void OnImeEnd(IntPtr hIMC) { }

        protected virtual void OnCalcScrollRectangle(STTextBoxScrollInfo scroll) {
            int nSW = this.GetIntXSize(scroll.Size);
            int nSH = this.GetIntYSize(scroll.Size);
            var c = m_core;
            Rect rectV = c.ViewRectangle;
            scroll.HBackRect = new Rect(rectV.X, rectV.Bottom - nSH, rectV.Width - nSW, nSH);
            scroll.VBackRect = new Rect(rectV.Right - nSW, rectV.Y, nSW, rectV.Height - nSH);
            Rect rect_h_thumb = scroll.HBackRect;
            Rect rect_v_thumb = scroll.VBackRect;

            float fScale = ((float)rectV.Width / scroll.XIncrement) / (rectV.Width / scroll.XIncrement + scroll.MaxXValue);
            rect_h_thumb.Width = (int)(rectV.Width * fScale);
            if (rect_h_thumb.Width > scroll.HBackRect.Width) {
                rect_h_thumb.Width = scroll.HBackRect.Width;
            } else if (rect_h_thumb.Width < this.GetIntXSize(4)) {
                rect_h_thumb.Width = this.GetIntXSize(4);
            }

            fScale = ((float)rectV.Height / c.LineHeight) / (rectV.Height / c.LineHeight + scroll.MaxYValue);
            rect_v_thumb.Height = (int)(rectV.Height * fScale);
            if (rect_v_thumb.Height > scroll.VBackRect.Height) {
                rect_v_thumb.Height = scroll.VBackRect.Height;
            } else if (rect_v_thumb.Height < this.GetIntYSize(4)) {
                rect_v_thumb.Height = this.GetIntYSize(4);
            }

            rect_h_thumb.X = rectV.X;
            if (scroll.MaxXValue > 0) {
                fScale = (float)scroll.XValue / scroll.MaxXValue;
                rect_h_thumb.X += (int)((scroll.HBackRect.Width - rect_h_thumb.Width) * fScale);
            }

            rect_v_thumb.Y = rectV.Y;
            if (scroll.MaxYValue > 0) {
                fScale = (float)scroll.YValue / scroll.MaxYValue;
                rect_v_thumb.Y += (int)((scroll.VBackRect.Height - rect_v_thumb.Height) * fScale);
            }
            scroll.HThumbRect = rect_h_thumb;
            scroll.VThumbRect = rect_v_thumb;
            if (scroll.VBackRect == rect_v_thumb) {
                rect_h_thumb.Width += nSW;
                scroll.HThumbRect = rect_h_thumb;
                var temp = scroll.HBackRect;
                temp.Width += nSW;
                scroll.HBackRect = temp;
            } else if (scroll.HBackRect == rect_h_thumb) {
                rect_v_thumb.Height += nSH;
                scroll.VThumbRect = rect_v_thumb;
                var temp = scroll.VBackRect;
                temp.Height += nSH;
                scroll.VBackRect = temp;
            } else {
            }
            //scroll.HThumbRect = rect_h_thumb;
            //scroll.VThumbRect = rect_v_thumb;
        }

        protected virtual void OnDrawVScrollBar(ISTTextBoxRender render, STTextBoxScrollInfo scroll) {
            var clr = scroll.HoverScrollBar == STTextBoxScrollInfo.ScrollBarType.V ? this._ScrollbarThumbHoverColor : this._ScrollbarBackColor;
            render.FillRectangle(clr, scroll.VBackRect);
            render.FillRectangle(clr, scroll.VBackRect.X, scroll.VBackRect.Y, this.GetIntXSize(1), scroll.VBackRect.Height);
            render.FillRectangle(this._ScrollbarThumbBackColor, scroll.VThumbRect);
        }

        protected virtual void OnDrawHScrollBar(ISTTextBoxRender render, STTextBoxScrollInfo scroll) {
            var clr = scroll.HoverScrollBar == STTextBoxScrollInfo.ScrollBarType.H ? this._ScrollbarThumbHoverColor : this._ScrollbarBackColor;
            render.FillRectangle(clr, scroll.HBackRect);
            render.FillRectangle(clr, scroll.HBackRect.X, scroll.HBackRect.Y, scroll.HBackRect.Width, this.GetIntYSize(1));
            render.FillRectangle(this._ScrollbarThumbBackColor, scroll.HThumbRect);
        }

        protected virtual void OnDrawScrollBarCorner(ISTTextBoxRender render, STTextBoxScrollInfo scroll) {
            int nWidth = this.GetIntXSize(scroll.Size);
            int nHeight = this.GetIntYSize(scroll.Size);
            render.FillRectangle(
                this._ScrollBarCornerColor, m_core.ViewRectangle.Right - nWidth,
                m_core.ViewRectangle.Bottom - nHeight,
                nWidth,
                nHeight);
        }

        #endregion

        #region private

        private bool SetCursor(Cursor c) {
            if (this.Cursor == c) {
                return false;
            }
            this.Cursor = c;
            return true;
        }
        //hmbb  输入法获取输入
        /*private void OnImeStartPrivate(IntPtr hIMC) {
            var CandidateForm = new Win32.CANDIDATEFORM() {
                dwStyle = Win32.CFS_CANDIDATEPOS,
                ptCurrentPos = m_core.Caret.Location
            };
            Win32.ImmSetCandidateWindow(hIMC, ref CandidateForm);
            var CompositionForm = new Win32.COMPOSITIONFORM() {
                dwStyle = Win32.CFS_FORCE_POSITION,
                ptCurrentPos = m_core.Caret.Location
            };
            Win32.ImmSetCompositionWindow(hIMC, ref CompositionForm);
            var logFont = new Win32.LOGFONT() {
                lfHeight = m_core.FontHeight,
                lfFaceName = this.Font.FontFamily + "\0"
            };
            Win32.ImmSetCompositionFont(hIMC, ref logFont);
            this.OnImeStart(hIMC);
        }

        private void OnImeEndPrivate(IntPtr hIMC) {
            Win32.ImmReleaseContext(this.Handle, hIMC);
            this.OnImeEnd(hIMC);
        }

        private void OnImeResultStrPrivate(IntPtr hIMC, string strResult) {
            var CompositionForm = new Win32.COMPOSITIONFORM() {
                dwStyle = Win32.CFS_FORCE_POSITION,
                ptCurrentPos = m_core.Caret.Location
            };
            Win32.ImmSetCompositionWindow(hIMC, ref CompositionForm);
            this.OnImeResultStr(hIMC, strResult);
        }*/

        private void m_timer_Tick(object sender, EventArgs e) {
            if (m_core.Scroll.HoverScrollBar != STTextBoxScrollInfo.ScrollBarType.None) {
                return;
            }
            lock (m_obj_sync) {
                m_core.Scroll.CountDown--;
            }
            if (m_core.Scroll.CountDown <= 0) {
                m_timer.Stop();
                this.Invalidate();
            }
            //设置输入法位置
            var point = m_core.Caret.Location;
            var p = this.PointToView(point);
            Root?.ViewImpl.SetIMEPosition(p);
        }

        #endregion
    }
}
