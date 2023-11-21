using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF
{
    //[TypeConverter(typeof(TransformConverter))]
    /// <summary>
    /// 字符串格式和CSS的差不多，但是没有单位，旋转没有xyz，其他变换没有z，比如：定义单个或者多个rotate(20) skew(15,3) translate(100,200) 或者单独一个 matrix(m11, m12, m21, m22, offsetX, offsetY) 默认值是Identity
    /// </summary>
    [Description("字符串格式和CSS的差不多，但是没有单位，旋转没有xyz，其他变换没有z，比如：定义单个或者多个rotate(20) skew(15,3) translate(100,200) 或者单独一个 matrix(m11, m12, m21, m22, offsetX, offsetY) 默认值是Identity")]
    public abstract class Transform : CpfObject
    {
        static readonly Transform s_identity = new MatrixTransform(Matrix.Identity);
        ///<summary>
        /// 恒等变换
        ///</summary>
        public static Transform Identity
        {
            get
            {
                //if (s_identity == null)
                //{
                //    s_identity =;
                //}
                return s_identity;
            }
        }

        public override string ToString()
        {
            return "matrix(" + Value.ToString() + ")";
        }

        /// <summary>
        /// 对Point执行变换
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point DoTransform(Point point)
        {
            return Value.Transform(point);
        }
        /// <summary>
        /// 变换矩阵
        /// </summary>
        public abstract Matrix Value { get; }

        /// <summary>
        /// 返回一个逆变换，如果不可逆则返回null
        /// </summary>
        [NotCpfProperty]
        public Transform Inverse
        {
            get
            {
                if (Value.HasInverse)
                {
                    Matrix m = Value;
                    m.Invert();
                    return new MatrixTransform(m);
                }
                return null;
            }
        }
        /// <summary>
        /// 字符串格式和CSS的差不多，但是没有单位，旋转没有xyz，比如：定义单个或者多个rotate(20) skew(15,3) translate(100,200)
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator Transform(string n)
        {
            return Parse(n);
        }
        /// <summary>
        /// 字符串格式和CSS的差不多，但是没有单位，旋转没有xyz，其他变换没有z，比如：定义单个或者多个rotate(20) skew(15,3) translate(100,200)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Transform Parse(string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim() == "matrix(identity)" || value.ToLower().Trim() == "identity" || value == "null" || value == "none")
                {
                    return Transform.Identity;
                }
                value = value.ToLower().Trim();

                if (value.StartsWith("matrix("))
                {
                    var end = value.IndexOf(')');
                    var v = value.Substring(7, end - 7);
                    var temp = v.Split(',');
                    if (temp.Length == 6)
                    {
                        return new MatrixTransform(new Matrix(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), float.Parse(temp[3]), float.Parse(temp[4]), float.Parse(temp[5])));
                    }
                }
                else
                {
                    var g = new GeneralTransform();

                    var text = value;
                    while (true)
                    {
                        text = parse(text, g);
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            break;
                        }
                    }

                    return g;
                }

                throw new Exception("Transform格式不对");
            }
            catch (Exception e)
            {
                throw new Exception("Transform格式不对:" + value, e);
            }
        }

        static (float x, float y, string txt) parse(string text, string start)
        {
            float x = 0;
            float y = 0;
            var end = text.IndexOf(')');
            var temp = text.Substring(start.Length + 1, end - start.Length - 1);
            if (temp.Contains(","))
            {
                var ts = temp.Split(',');
                x = float.Parse(ts[0]);
                y = float.Parse(ts[1]);
            }
            else
            {
                x = y = float.Parse(temp);
            }
            return (x, y, text.Substring(end + 1).Trim());
        }

        static string parse(string text, GeneralTransform transform)
        {
            if (text.StartsWith("scale"))
            {
                var value = parse(text, "scale");
                transform.ScaleX = value.x;
                transform.ScaleY = value.y;
                return value.txt;
            }
            if (text.StartsWith("scaleX"))
            {
                var value = parse(text, "scaleX");
                transform.ScaleX = value.x;
                return value.txt;
            }
            if (text.StartsWith("scaleY"))
            {
                var value = parse(text, "scaleY");
                transform.ScaleY = value.y;
                return value.txt;
            }
            else if (text.StartsWith("translate"))
            {
                var value = parse(text, "translate");
                transform.OffsetX = value.x;
                transform.OffsetY = value.y;
                return value.txt;
            }
            else if (text.StartsWith("translateX"))
            {
                var value = parse(text, "translateX");
                transform.OffsetX = value.x;
                return value.txt;
            }
            else if (text.StartsWith("translateY"))
            {
                var value = parse(text, "translateY");
                transform.OffsetY = value.y;
                return value.txt;
            }
            else if (text.StartsWith("rotate"))
            {
                var value = parse(text, "rotate");
                transform.Angle = value.x;
                return value.txt;
            }
            else if (text.StartsWith("skew"))
            {
                var value = parse(text, "skew");
                transform.SkewX = value.x;
                transform.SkewY = value.y;
                return value.txt;
            }
            else if (text.StartsWith("skewX"))
            {
                var value = parse(text, "skewX");
                transform.SkewX = value.x;
                return value.txt;
            }
            else if (text.StartsWith("skewY"))
            {
                var value = parse(text, "skewY");
                transform.SkewY = value.y;
                return value.txt;
            }
            return "";
        }
    }
}
