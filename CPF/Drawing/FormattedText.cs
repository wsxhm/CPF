using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 绘制的文本格式
    /// </summary>
    public struct FormattedText
    {
        public FormattedText(string text, Font font)
        {
            Text = text;
            this.font = font;
            maxWidth = float.MaxValue;
        }

        public FormattedText(string text, Font font, float maxWidth)
        {
            Text = text;
            this.font = font;
            this.maxWidth = maxWidth;
        }

        ///// <summary>
        ///// 文本对齐方式
        ///// </summary>
        //public TextAlignment TextAlignment { get; set; }

        //TextWrapping textWrapping;

        float maxWidth;
        //public float MaxHeight { get; set; } = float.MaxValue;

        //Size size = Size.Empty;
        ///// <summary>
        ///// 文字占用尺寸计算结果
        ///// </summary>
        //public Size Size
        //{
        //    get
        //    {
        //        if (size == Size.Empty)
        //        {
        //            size = CPF.Platform.Application.GetDrawingFactory().MeasureString(Text, Font);
        //        }
        //        return size;
        //    }
        //}

        Font font;

        public string Text { get; }
        ///// <summary>
        ///// 是否换行
        ///// </summary>
        //public TextWrapping TextWrapping
        //{
        //    get
        //    {
        //        return textWrapping;
        //    }

        //    set
        //    {
        //        if (textWrapping != value)
        //        {
        //            textWrapping = value;
        //            size = Size.Empty;
        //        }
        //    }
        //}

        public Font Font
        {
            get
            {
                return font;
            }

            set
            {
                //if (font != value)
                //{
                    font = value;
                    //size = Size.Empty;
                //}
            }
        }
        /// <summary>
        /// 限制文本布局的最大宽度
        /// </summary>
        public float MaxWidth
        {
            get
            {
                return maxWidth;
            }

            set
            {
                if (maxWidth != value)
                {
                    maxWidth = value;
                    //size = Size.Empty;
                }
            }
        }
    }
}
