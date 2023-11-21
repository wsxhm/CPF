using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 线条样式
    /// </summary>
    [Description("线条样式，格式：width[,Solid/Dash/Dot/DashDot/DashDotDot]")]
    public struct Stroke
    {

        public Stroke(float width)
            : this(width, DashStyles.Solid)
        { }

        public Stroke(float width, DashStyles ds)
            : this(width, ds, 1, null, CapStyles.Flat)
        {
        }
        public Stroke(float width, DashStyles ds, CapStyles cap)
            : this(width, ds, 1, null, cap)
        {
        }

        public Stroke(float width, DashStyles ds, float dos, float[] df, CapStyles start)
        {
            this.width = width;
            dashStyle = ds;
            dashOffset = dos;
            dashPattern = df;
            startCap = start;
            lineJoin = LineJoins.Miter;
            //endCap = end;
        }
        public Stroke(float width, DashStyles ds, float dos, float[] df, CapStyles start, LineJoins lineJoin)
        {
            this.width = width;
            dashStyle = ds;
            dashOffset = dos;
            dashPattern = df;
            startCap = start;
            this.lineJoin = lineJoin;
            //endCap = end;
        }

        LineJoins lineJoin;
        /// <summary>
        /// 线条连接处形状
        /// </summary>
        public LineJoins LineJoin
        {
            get { return lineJoin; }
            set { lineJoin = value; }
        }

        DashStyles dashStyle;
        /// <summary>
        /// 线条类型
        /// </summary>
        public DashStyles DashStyle
        {
            get { return dashStyle; }
            set { dashStyle = value; }
        }

        float width;
        /// <summary>
        /// 线条宽度
        /// </summary>
        public float Width
        {
            get { return width; }
            set { width = value; }
        }
        float dashOffset;
        /// <summary>
        /// 获取或设置直线的起点到短划线图案起始处的距离。
        /// </summary>
        public float DashOffset
        {
            get { return dashOffset; }
            set { dashOffset = value; }
        }
        float[] dashPattern;

        /// <summary>
        /// 获取或设置自定义的短划线和空白区域的数组。只能偶数个
        /// </summary>
        public float[] DashPattern
        {
            get { return dashPattern; }
            set { dashPattern = value; }
        }
        /// <summary>
        /// 线条两端样式
        /// </summary>
        public CapStyles StrokeCap
        {
            get
            {
                return startCap;
            }
            set
            {
                startCap = value;
            }
        }
        ///// <summary>
        ///// 末端线条样式
        ///// </summary>
        //public CapStyles EndCap
        //{
        //    get
        //    {
        //        return endCap;
        //    }

        //    set
        //    {
        //        endCap = value;
        //    }
        //}

        CapStyles startCap;
        //CapStyles endCap;

        public static bool operator !=(Stroke pen1, Stroke pen2)
        {
            if (pen1.DashOffset != pen2.DashOffset)
            {
                return true;
            }
            if (pen1.DashStyle != pen2.DashStyle)
            {
                return true;
            }
            if (pen1.Width != pen2.Width)
            {
                return true;
            }
            if (pen1.startCap != pen2.startCap)
            {
                return true;
            }
            if (pen1.lineJoin != pen2.lineJoin)
            {
                return true;
            }
            if ((pen1.dashPattern == null && pen2.dashPattern != null) || (pen2.dashPattern == null && pen1.dashPattern != null))
            {
                return true;
            }
            if (pen1.DashPattern != null && pen2.DashPattern != null)
            {
                if (pen1.dashPattern.Length != pen2.dashPattern.Length)
                {
                    return true;
                }
                for (int i = 0; i < pen1.dashPattern.Length; i++)
                {
                    if (pen1.dashPattern[i] != pen2.dashPattern[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool operator ==(Stroke pen1, Stroke pen2)
        {
            return !(pen1 != pen2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Stroke))
            {
                return false;
            }
            return this == (Stroke)obj;
        }
        public override int GetHashCode()
        {
            int hash = 0;
            if (this.DashPattern != null)
            {
                foreach (var item in dashPattern)
                {
                    hash = (hash ^ item.GetHashCode());
                }
            }
            return this.Width.GetHashCode() ^
                   this.DashOffset.GetHashCode() ^
                   this.DashStyle.GetHashCode() ^
                   this.startCap.GetHashCode() ^
                   this.lineJoin.GetHashCode() ^
                   hash;

        }
        /// <summary>
        /// width[,Solid/Dash/Dot/DashDot/DashDotDot]
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator Stroke(string n)
        {
            if (string.IsNullOrEmpty(n))
            {
                return default;
            }
            try
            {
                var tem = n.Split(' ');
                n = tem[0];
                Stroke stroke;
                if (n.IndexOf(',') > 0)
                {
                    var temp = n.Split(',');
                    var width = float.Parse(temp[0]);
                    var s = temp[1].ToLower().Trim();
                    var ds = (DashStyles)Enum.Parse(typeof(DashStyles), s, true);
                    stroke = new Stroke(width, ds);
                    if (temp.Length > 2)
                    {
                        var cap = (CapStyles)Enum.Parse(typeof(CapStyles), temp[2].ToLower().Trim(), true);
                        //stroke = new Stroke(width, ds, cap);
                        stroke.startCap = cap;
                    }
                    if (temp.Length > 3)
                    {
                        stroke.lineJoin = (LineJoins)Enum.Parse(typeof(LineJoins), temp[3].ToLower().Trim(), true);
                    }
                }
                else
                {
                    stroke = new Stroke(float.Parse(n));
                }

                if (tem.Length > 1)
                {
                    var ns = tem[1].Split(',');
                    var fs = new float[ns.Length];
                    for (int i = 0; i < fs.Length; i++)
                    {
                        fs[i] = float.Parse(ns[i]);
                    }
                    //n = tem[0];
                    //var temp = n.Split(',');
                    //var width = float.Parse(temp[0]);
                    //var s = temp[1].ToLower().Trim();
                    //var ds = (DashStyles)Enum.Parse(typeof(DashStyles), s, true);
                    //return new Stroke(width, ds) { DashPattern = fs };
                    stroke.dashPattern = fs;
                }
                return stroke;
            }
            catch (Exception)
            {
                throw new InvalidCastException("Stroke字符串格式不对");
            }
        }

        public float[] GetDashPattern()
        {
            switch (dashStyle)
            {
                case DashStyles.Solid:
                    break;
                case DashStyles.Dash:
                    return new float[] { 3, 3 };
                case DashStyles.Dot:
                    return new float[] { 1, 1 };
                case DashStyles.DashDot:
                    return new float[] { 3, 3, 1, 3 };
                case DashStyles.DashDotDot:
                    return new float[] { 3, 3, 1, 3, 1, 3 };
                case DashStyles.Custom:
                    return DashPattern;
                default:
                    break;
            }
            return null;
        }

        public override string ToString()
        {
            var str = width + "," + dashStyle.ToString();
            if (startCap != CapStyles.Flat || lineJoin != LineJoins.Miter)
            {
                str += "," + startCap.ToString();
            }
            if (lineJoin != LineJoins.Miter)
            {
                str += "," + lineJoin.ToString();
            }
            if (dashStyle == DashStyles.Custom && dashPattern != null)
            {
                str += " " + string.Join(",", dashPattern);
            }
            return str;

        }
    }

    public enum DashStyles : byte
    {
        Solid = 0,
        /// <summary>
        /// 短线 3, 3
        /// </summary>
        Dash = 1,
        /// <summary>
        /// 点 1,1
        /// </summary>
        Dot = 2,
        /// <summary>
        /// 3, 3, 1, 3
        /// </summary>
        DashDot = 3,
        /// <summary>
        ///  3, 3, 1, 3, 1, 3
        /// </summary>
        DashDotDot = 4,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 5,
    }
    public enum CapStyles : byte
    {
        Flat = 0,
        Square = 1,
        Round = 2,
        Triangle = 3,
    }
    public enum LineJoins : byte
    {
        Miter = 0,
        Bevel = 1,
        Round = 2,
        //MiterOrBevel = 3,
    }
}
