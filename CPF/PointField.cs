using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 支持单位百分比的Point，格式：10,10  10%,10%
    /// </summary>
    public struct PointField
    {
        public PointField(FloatField x, FloatField y)
        {
            X = x;
            Y = y;
        }

        public FloatField X { get; set; }

        public FloatField Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PointField)
            {
                var o = (PointField)obj;
                return o.X.Equals(X) && o.Y.Equals(Y);
            }
            return false;
        }

        public static implicit operator PointField(string n)
        {
            if (n.IndexOf(',') < 0)
            {
                throw new Exception("PointValue 字符串格式错误 :" + n);
            }
            else
            {
                var tem = n.Split(',');
                try
                {
                    return new PointField { X = tem[0], Y = tem[1] };
                }
                catch (Exception)
                {
                    throw new Exception("PointValue 字符串格式错误 :" + n);
                }
            }
        }
        public static implicit operator PointField(Point point)
        {
            return new PointField(point.X, point.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
    }
}
