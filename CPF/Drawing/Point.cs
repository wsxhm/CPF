using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.ComponentModel;

namespace CPF.Drawing
{
    [TypeConverter(typeof(PointConverter))]
    [Serializable]
    public struct Point
    {
        public static bool operator ==(Point point1, Point point2)
        {
            return point1.X.Equal(point2.X) &&
                   point1.Y.Equal(point2.Y);
        }

        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        public static bool Equals(Point point1, Point point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Point))
            {
                return false;
            }

            Point value = (Point)o;
            return Point.Equals(this, value);
        }

        public bool Equals(Point value)
        {
            return Point.Equals(this, value);
        }


        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode();
        }

        public static readonly Point Empty;

        /// <summary>
        /// 是否为0,0
        /// </summary>
        public bool IsEmpty => this == Empty;
        public float X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }

        }

        /// <summary>
        ///     Y - double.  Default value is 0.
        /// </summary>
        public float Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }

        }
        //public static Point Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Point value;

        //    String firstToken = th.NextTokenRequired();

        //    value = new Point(
        //        Convert.ToDouble(firstToken, formatProvider),
        //        Convert.ToDouble(th.NextTokenRequired(), formatProvider));

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}

        public static Point Parse(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            if (parts.Count == 2)
            {
                return new Point(float.Parse(parts[0]), float.Parse(parts[1]));
            }
            else
            {
                throw new FormatException("Invalid Point.");
            }
        }

        string ConvertToString(string format, IFormatProvider provider)
        {
            return String.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}",
                                 ',',
                                 _x,
                                 _y);
        }

        public override string ToString()
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, null /* format provider */);
        }
        internal float _x;
        internal float _y;

        public Point(in float x, in float y)
        {
            _x = x;
            _y = y;
        }

        public void Offset(float offsetX, float offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }
        public static Point operator +(Point point, Vector vector)
        {
            return new Point(point._x + vector._x, point._y + vector._y);
        }
        public static Point Add(Point point, Vector vector)
        {
            return new Point(point._x + vector._x, point._y + vector._y);
        }

        public static Point operator -(Point point, Vector vector)
        {
            return new Point(point._x - vector._x, point._y - vector._y);
        }
        public static Point Subtract(Point point, Vector vector)
        {
            return new Point(point._x - vector._x, point._y - vector._y);
        }

        public static Vector operator -(Point point1, Point point2)
        {
            return new Vector(point1._x - point2._x, point1._y - point2._y);
        }

        public static Vector Subtract(Point point1, Point point2)
        {
            return new Vector(point1._x - point2._x, point1._y - point2._y);
        }

        public static Point operator *(Point point, Matrix matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Multiplies a point by a factor coordinate-wise
        /// </summary>
        /// <param name="p">Point to multiply</param>
        /// <param name="k">Factor</param>
        /// <returns>Points having its coordinates multiplied</returns>
        public static Point operator *(Point p, float k) => new Point(p.X * k, p.Y * k);

        /// <summary>
        /// Multiplies a point by a factor coordinate-wise
        /// </summary>
        /// <param name="p">Point to multiply</param>
        /// <param name="k">Factor</param>
        /// <returns>Points having its coordinates multiplied</returns>
        public static Point operator *(float k, Point p) => new Point(p.X * k, p.Y * k);

        /// <summary>
        /// Divides a point by a factor coordinate-wise
        /// </summary>
        /// <param name="p">Point to divide by</param>
        /// <param name="k">Factor</param>
        /// <returns>Points having its coordinates divided</returns>
        public static Point operator /(Point p, float k) => new Point(p.X / k, p.Y / k);

        public static Point Multiply(Point point, Matrix matrix)
        {
            return matrix.Transform(point);
        }

        public static explicit operator Size(Point point)
        {
            return new Size(Math.Abs(point._x), Math.Abs(point._y));
        }
        public static implicit operator Vector(Point point)
        {
            return new Vector(point._x, point._y);
        }
        /// <summary>
        /// x,y
        /// </summary>
        /// <param name="point"></param>
        public static implicit operator Point(string point)
        {
            try
            {
                var temp = point.Split(',');
                return new Point(float.Parse(temp[0]), float.Parse(temp[1]));
            }
            catch (Exception e)
            {
                throw new Exception("Point字符串格式不对", e);
            }
        }
        /// <summary>
        /// Returns a new point with the specified X coordinate.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <returns>The new point.</returns>
        public Point WithX(float x)
        {
            return new Point(x, _y);
        }

        /// <summary>
        /// Returns a new point with the specified Y coordinate.
        /// </summary>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The new point.</returns>
        public Point WithY(float y)
        {
            return new Point(_x, y);
        }

    }
}
