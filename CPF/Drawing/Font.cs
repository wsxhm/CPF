using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    public struct Font : IDisposable
    {
        /// <summary>
        /// 初始化字体
        /// </summary>
        /// <param name="fontfamily"></param>
        /// <param name="size">像素为单位px</param>
        /// <param name="fontstyle"></param>
        public Font(string fontfamily, float size, FontStyles fontstyle = FontStyles.Regular)
        {
            if (string.IsNullOrWhiteSpace(fontfamily))
            {
                throw new Exception("字体名称不能为空");
            }
            font = null;
            this.FontFamily = fontfamily;
            this.FontSize = size;
            this.FontStyle = fontstyle;
            ascent = 0;
            descent = 0;
            lineHeight = 0;
        }

        //public Font(IDisposable font, string fontfamily, float size, FontStyles fontstyle)
        //{
        //    this.font = font;
        //    this.FontFamily = fontfamily;
        //    this.FontSize = size;
        //    this.FontStyle = fontstyle;
        //}

        IDisposable font;
        public IDisposable AdapterFont
        {
            get
            {
                if (font == null)
                {
                    font = Platform.Application.GetDrawingFactory().CreateFont(FontFamily, FontSize, FontStyle);
                }
                return font;
            }
        }

        public float FontSize { get; }

        public string FontFamily { get; }

        public FontStyles FontStyle { get; }

        public override bool Equals(object obj)
        {
            if (obj is Font font)
            {
                return font.FontSize == FontSize && font.FontFamily == FontFamily && font.FontStyle == FontStyle;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int s = FontFamily == null ? 0 : FontFamily.GetHashCode();
            return FontStyle.GetHashCode() ^ FontSize.GetHashCode() ^ s;
            //return base.GetHashCode();
        }

        public void Dispose()
        {
            if (font != null)
            {
                font.Dispose();
                font = null;
            }
            //GC.SuppressFinalize(this);
        }
        float lineHeight;
        /// <summary>
        /// 字体默认行高
        /// </summary>
        public float LineHeight
        {
            get
            {
                if (lineHeight == 0)
                {
                    lineHeight = Platform.Application.GetDrawingFactory().GetLineHeight(this);
                }
                return lineHeight;
            }
        }

        float ascent;
        /// <summary>
        /// 获取从字样的基线到英语大写字母顶部的距离。
        /// </summary>
        public float Ascent
        {
            get
            {
                if (ascent == 0)
                {
                    ascent = Platform.Application.GetDrawingFactory().GetAscent(this);
                }
                return ascent;
            }
        }

        float descent;
        /// <summary>
        /// 获取从字样的基线到行底部的距离
        /// </summary>
        public float Descent
        {
            get
            {
                if (descent == 0)
                {
                    descent = Platform.Application.GetDrawingFactory().GetDescent(this);
                }
                return descent;
            }
        }

        //~Font()
        //{
        //    Dispose();
        //}
    }

    [Flags]
    public enum FontStyles : byte
    {
        /// <summary>
        /// 普通文本
        /// </summary>
        Regular = 0,
        /// <summary>
        /// 加粗文本
        /// </summary>
        Bold = 1,
        /// <summary>
        /// 倾斜文本
        /// </summary>
        Italic = 2,
        ///// <summary>
        ///// 带下划线的文本
        ///// </summary>
        //Underline = 4,
        ///// <summary>
        ///// 中间有直线通过的文本
        ///// </summary>
        //Strikeout = 8,
    }
    ///// <summary>
    ///// 段落对齐方式
    ///// </summary>
    //public enum ParagraphAlignment
    //{
    //    Near = 0,
    //    Far = 1,
    //    Center = 2,
    //}
    /// <summary>
    /// 是否换行
    /// </summary>
    public enum TextWrapping : byte
    {
        /// <summary>
        /// Text should not wrap.
        /// </summary>
        NoWrap,

        /// <summary>
        /// Text can wrap.
        /// </summary>
        Wrap
    }
    ///// <summary>
    ///// 文字对齐方式
    ///// </summary>
    //public enum TextAlignment : byte
    //{
    //    /// <summary>
    //    /// The text is left-aligned.
    //    /// </summary>
    //    Left,

    //    /// <summary>
    //    /// The text is centered.
    //    /// </summary>
    //    Center,

    //    /// <summary>
    //    /// The text is right-aligned.
    //    /// </summary>
    //    Right,
    //}

}
