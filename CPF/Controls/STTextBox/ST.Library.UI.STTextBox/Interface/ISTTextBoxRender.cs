using CPF.Controls;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ST.Library.UI.STTextBox
{
    /// <summary>
    /// All drawing of STTextBox comes from this interface.
    /// Calling rules: 
    ///     STTextBox guarantees that OnBeginPaint(Graphics) or BeginPaint() 
    ///     must be called before calling all drawing functions in this interface. 
    ///     And call OnEndPaint(Graphics) or EndPaint() at the end of drawing.
    /// During the OnPaint of the control, OnBegin/EndPaint is called for initialization.
    /// Otherwise call Begin/EndPaint.
    /// 
    /// * However, if you rewrite some functions in TextView or ITextView. 
    /// * It needs to be called by the developer himself.
    /// 
    /// The STTextBox release uses GDI+ for rendering by default.
    /// </summary>
    public interface ISTTextBoxRender
    {
        /// <summary>
        /// Bind the control
        /// </summary>
        /// <param name="ctrl">the STTextBox control</param>
        void BindControl(Control ctrl);
        void UnbindControl();
        /// <summary>
        /// This function will be called at the start of the STTextBox.Paint event.
        /// </summary>
        /// <param name="g">PaintEventArgs.Graphics</param>
        void OnBeginPaint(Graphics g);
        /// <summary>
        /// This function will be called at the end of the STTextBox.Paint event.
        /// * The destruction of [g] should not be done in this function, it should be handled by STTextBox.Paint.
        /// </summary>
        /// <param name="g">PaintEventArgs.Graphics</param>
        void OnEndPaint(Graphics g);
        /// <summary>
        /// Developers should call this method instead of OnBegin(Graphics) when initializing drawing is required.
        /// </summary>
        void BeginPaint();
        /// <summary>
        /// This function corresponds to BeginPaint().
        /// </summary>
        void EndPaint();
        /// <summary>
        /// Set how many spaces are required for a tab.
        /// </summary>
        /// <param name="nSize">Number of spaces</param>
        /// <returns>The old size</returns>
        int SetTabSize(int nSize);
        /// <summary>
        /// Get how many spaces are required for a tab.
        /// </summary>
        /// <returns>Number of spaces</returns>
        int GetSpaceWidth();
        int GetFontHeight();
        int GetTabSize();
        /// <summary>
        /// Get how much width the tab character needs at the current position.
        /// </summary>
        /// <param name="nLeftWidth">The position relative to the start of the text</param>
        /// <returns>Width</returns>
        int GetTabWidth(int nLeftWidth);
        /// <summary>
        /// How many spaces are needed to get tabs.
        /// </summary>
        /// <param name="nLeftWidth">The position relative to the start of the text</param>
        /// <param name="nTabSize">How many spaces are required for a tab</param>
        /// <returns>Space count</returns>
        float GetTabSpaceCount(int nLeftWidth, int nTabSize);
        /// <summary>
        /// Get the string width
        /// </summary>
        /// <param name="strText">text</param>
        /// <param name="style">Text style</param>
        /// <param name="nLeftWidth">The position relative to the start of the text</param>
        /// <returns>Width</returns>
        int GetStringWidth(string strText, TextStyle style, int nLeftWidth);

        void SetClip(Rect rect);
        void ResetClip();
        void DrawImage(Image img, Rect rect);
        void DrawString(string strText, Font ft, CPF.Drawing.Color color, Rect rect, StringFormat sf);
        void DrawString(string strText, TextStyle style, Rect rect, TextDecorationLocation Underline = TextDecorationLocation.None);
        void FillRectangle(CPF.Drawing.Color backColor, Rect rect);
        void FillRectangle(CPF.Drawing.Color backColor, float nX, float nY, float nWidth, float nHeight);
    }
}
