using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF
{
    [Description("表示四周的厚度，可以用百分比，格式：all或者left,top,right,bottom")]
    public struct ThicknessField
    {
        /// <summary>
        /// 表示四周的厚度 This constructur builds a Thickness with a specified value on every side.
        /// </summary>
        /// <param name="uniformLength">The specified uniform length.</param>
        public ThicknessField(in FloatField uniformLength)
        {
            Left = Top = Right = Bottom = uniformLength;
        }

        /// <summary>
        /// This constructor builds a Thickness with the specified number of pixels on each side.
        /// 表示四周的厚度
        /// </summary>
        /// <param name="left">The thickness for the left side.</param>
        /// <param name="top">The thickness for the top side.</param>
        /// <param name="right">The thickness for the right side.</param>
        /// <param name="bottom">The thickness for the bottom side.</param>
        public ThicknessField(in FloatField left, in FloatField top, in FloatField right, in FloatField bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public FloatField Top { get; set; }
        public FloatField Bottom { get; set; }

        public FloatField Left { get; set; }
        public FloatField Right { get; set; }

        public static implicit operator ThicknessField(string n)
        {
            if (string.IsNullOrWhiteSpace(n))
            {
                throw new Exception("Thickness字符转换格式不对");
            }
            if (n.IndexOf(',') >= 0)
            {
                var temp = n.Split(',');
                if (temp.Length < 4)
                {
                    throw new Exception("Thickness字符转换格式不对");
                }
                try
                {
                    return new ThicknessField(temp[0].Trim(), temp[1].Trim(), temp[2].Trim(), temp[3].Trim());
                }
                catch (Exception)
                {
                    throw new Exception("Thickness字符转换格式不对");
                }
            }
            else
            {
                try
                {
                    return new ThicknessField(n);
                }
                catch (Exception)
                {
                    throw new Exception("Thickness字符转换格式不对");
                }
            }
        }

        public override string ToString()
        {
            return $"{Left},{Top},{Right},{Bottom}";
        }
    }
}
