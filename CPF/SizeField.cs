using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 支持单位百分比的Size
    /// </summary>
    public struct SizeField
    {
        public SizeField(FloatField w, FloatField h)
        {
            Width = w;
            Height = h;
        }

        public FloatField Width { get; set; }

        public FloatField Height { get; set; }
        /// <summary>
        /// 100%,100%
        /// </summary>
        public static SizeField Fill { get { return new SizeField("100%", "100%"); } }

        public override bool Equals(object obj)
        {
            if (obj is SizeField)
            {
                var o = (SizeField)obj;
                return o.Width.Equals(Width) && o.Height.Equals(Height);
            }
            return false;
        }
        public static implicit operator SizeField(string n)
        {
            if (n.IndexOf(',') < 0)
            {
                throw new Exception("SizeValue 字符串格式错误 :" + n);
            }
            else
            {
                var tem = n.Split(',');
                try
                {
                    return new SizeField(tem[0], tem[1]);
                }
                catch (Exception)
                {
                    throw new Exception("SizeValue 字符串格式错误 :" + n);
                }
            }
        }
        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString()
        {
            return Width.ToString() + "," + Height.ToString();
        }
    }
}
