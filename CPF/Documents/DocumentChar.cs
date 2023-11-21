using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Globalization;

namespace CPF.Documents
{
    /// <summary>
    /// 定义一个文档字符
    /// </summary>
    public class DocumentChar : IFlowElement, ICanSelectElement
    {
        /// <summary>
        /// 定义一个文档字符
        /// </summary>
        /// <param name="c"></param>
        public DocumentChar(char c)
        {
            _char = c;
            StyleId = -1;
            IsMeasureValid = false;
        }
        public float Left
        {
            get;
            set;
        }

        public float Top
        {
            get;
            set;
        }
        char _char;

        public short StyleId
        {
            get;
            set;
        }

        public char Char
        {
            get
            {
                return _char;
            }
        }
        /// <summary>
        /// 是否可以被选中
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSelect { get { return true; } }

        static Dictionary<Font, FontSize> fontSize = new Dictionary<Font, FontSize>();

        public virtual void Arrange(in Font font, in Size availableSize)
        {
            if (!IsMeasureValid)
            {
                IsMeasureValid = true;

                var s = GetCharSize(font, _char);
                w = s.Width;
                h = s.Height;
            }
        }

        public static Size GetCharSize(in Font font, char _char)
        {
            if (!fontSize.TryGetValue(font, out var fontsize))
            {
                fontsize = new FontSize();
                fontsize.ChineseSize = Platform.Application.GetDrawingFactory().MeasureString("大", font);
                fontsize.NumSize = Platform.Application.GetDrawingFactory().MeasureString("5", font);
                fontsize.WhiteSize = Platform.Application.GetDrawingFactory().MeasureString(" ", font);
                fontsize.TabSize = new Size(fontsize.ChineseSize.Width * 4, fontsize.ChineseSize.Height);
                fontsize.WrapSize = new Size(1, fontsize.ChineseSize.Height);
                var length = 'z' - 'a' + 1;
                for (var i = 'a'; i < 'a' + length; i++)
                {
                    fontsize.LetterSize.Add(i, Platform.Application.GetDrawingFactory().MeasureString(i.ToString(), font));
                }
                length = 'Z' - 'A' + 1;
                for (var i = 'A'; i < 'A' + length; i++)
                {
                    fontsize.LetterSize.Add(i, Platform.Application.GetDrawingFactory().MeasureString(i.ToString(), font));
                }
                fontSize.Add(font, fontsize);
            }


            //var styleId = StyleId;
            if (_char > '\u4e00' && _char < '\u9fa5')
            {
                //w = fontsize.ChineseSize.Width;
                //h = fontsize.ChineseSize.Height;
                return fontsize.ChineseSize;
            }
            if ((_char >= 'a' && _char <= 'z') || (_char >= 'A' && _char <= 'Z'))
            {
                var s = fontsize.LetterSize[_char];
                //w = s.Width;
                //h = s.Height;
                return s;
            }
            if (_char >= '0' && _char <= '9')
            {
                //w = fontsize.NumSize.Width;
                //h = fontsize.NumSize.Height;
                return fontsize.NumSize;
            }
            if (_char == '\t')
            {
                //w = fontsize.TabSize.Width;
                //h = fontsize.TabSize.Height;
                return fontsize.TabSize;
            }
            if (_char == ' ')
            {
                //w = fontsize.WhiteSize.Width;
                //h = fontsize.WhiteSize.Height;
                return fontsize.WhiteSize;
            }
            if (_char == '\n')
            {
                //w = fontsize.WrapSize.Width;
                //h = fontsize.WrapSize.Height;
                return fontsize.WrapSize;
            }
            var size = Platform.Application.GetDrawingFactory().MeasureString(_char.ToString(), font);
            //w = size.Width;
            //h = size.Height;
            return size;
        }

        float w; float h;
        /// <summary>
        /// 布局之后的尺寸
        /// </summary>
        public float Width { get { return w; } }
        /// <summary>
        /// 布局之后的尺寸
        /// </summary>
        public float Height { get { return h; } }

        public override string ToString()
        {
            return _char.ToString();
        }


        public bool IsMeasureValid { get; set; }

        public virtual FlowDirection FlowDirection
        {
            get
            {
                var cate = char.GetUnicodeCategory(_char);
                if (cate == UnicodeCategory.SpaceSeparator || cate == UnicodeCategory.MathSymbol || cate == UnicodeCategory.OtherPunctuation)
                {
                    return FlowDirection.Auto;
                }
                if (_char < '\u0530' || _char > '\u07BF')//0530-058F：亚美尼亚语 (Armenian)  到 0780-07BF：马尔代夫语 (Thaana)
                {
                    return FlowDirection.LeftToRight;
                }
                return FlowDirection.RightToLeft;
            }
        }

        public float Right => 0;

        public float Bottom => 0;
    }

    class FontSize
    {
        public Size ChineseSize;
        public Size WhiteSize;
        public Size TabSize;
        public Size NumSize;
        public Size WrapSize;
        public Dictionary<char, Size> LetterSize = new Dictionary<char, Size>();
    }
}
