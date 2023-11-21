using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Documents
{
    /// <summary>
    /// 定义一个多Char的字符对象，比如：Unicode 里的表情：😁
    /// </summary>
    public class UTF32Text : IFlowElement, ICanSelectElement
    {
        /// <summary>
        /// 定义一个文档字符
        /// </summary>
        /// <param name="c"></param>
        public UTF32Text(string c)
        {
            //if (c.Length != 2)
            //{
            //    throw new Exception("无效字符，必须长度为2的UTF32字符");
            //}
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
        string _char;

        public short StyleId
        {
            get;
            set;
        }

        public string Text
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

        public virtual void Arrange(in Font font, in Size availableSize)
        {
            if (!IsMeasureValid)
            {
                IsMeasureValid = true;
                //var styleId = StyleId;
                var size = Platform.Application.GetDrawingFactory().MeasureString(_char, font);
                w = size.Width;
                h = size.Height;
            }
        }

        float w; float h;
        /// <summary>
        /// 布局之后的尺寸，包含margin
        /// </summary>
        public float Width { get { return w; } }
        /// <summary>
        /// 布局之后的尺寸，包含margin
        /// </summary>
        public float Height { get { return h; } }

        public override string ToString()
        {
            return _char;
        }

        public float Right => 0;

        public float Bottom => 0;

        public bool IsMeasureValid { get; set; }

        public virtual FlowDirection FlowDirection
        {
            get
            {
                return FlowDirection.Auto;
                //if (char.GetUnicodeCategory(_char[0]) != System.Globalization.UnicodeCategory.Surrogate)
                //{
                //    return FlowDirection.LeftToRight;
                //}
                //int c = char.ConvertToUtf32(_char[0], _char[1]);
                //if (c < 0x1E800 || c > 0x1EFFF)//1E800-1EFFF 从右向左书写的文字 RTL scripts
                //{
                //    return FlowDirection.LeftToRight;
                //}
                //return FlowDirection.RightToLeft;
            }
        }
    }
}
