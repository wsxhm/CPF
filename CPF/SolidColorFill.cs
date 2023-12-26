using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    /// <summary>
    /// 纯色填充
    /// </summary>
    public class SolidColorFill : ViewFill
    {
        /// <summary>
        /// 填充颜色
        /// </summary>
        [PropertyMetadata(typeof(Color), "0,0,0")]
        public Color Color
        {
            get { return (Color)GetValue(); }
            set { SetValue(value); }
        }

        public override Brush CreateBrush(in Rect rect, in float renderScaling)
        {
            //return new SolidColorBrush(Color);
            return SolidColorBrush.Create(Color);
        }

        public override string ToString()
        {
            return Color.ToString();
        }

        public override string GetCreationCode()
        {
            return "\"" + Color.ToString() + "\"";
        }

        /// <summary>
        /// 颜色格式字符串
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator SolidColorFill(string n)
        {
            return new SolidColorFill { Color = Color.Parse(n) };
        }
    }
}
