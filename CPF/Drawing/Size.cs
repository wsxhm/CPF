using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF;
using System.ComponentModel;

namespace CPF.Drawing
{
    [TypeConverter(typeof(SizeConverter))]
    [Serializable]
    public struct Size : IFormattable
    {
        #region Constructors

        /// <summary>
        /// Constructor which sets the size's initial values.
        /// </summary>
        /// <param name="width"> float - The initial Width </param>
        /// <param name="height"> float - THe initial Height </param>
        public Size(in float width, in float height)
        {
            //if (width < 0 || height < 0)
            //{
            //    throw new System.ArgumentException("不能小于0");
            //}

            _width = width;
            _height = height;
        }

        #endregion Constructors

        #region Statics

        /// <summary>
        /// Empty - a static property which provides an Empty size.  Width and Height are 
        /// negative-infinity.  This is the only situation
        /// where size can be negative.
        /// </summary>
        public static Size Empty
        {
            get
            {
                return s_empty;
            }
        }

        #endregion Statics

        #region Public Methods and Properties
        /// <summary>
        /// Returns a new <see cref="Size"/> with the same width and the specified height.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The new <see cref="Size"/>.</returns>
        public Size WithHeight(float height)
        {
            return new Size(_width, height);
        }
        /// <summary>
        /// Returns a new <see cref="Size"/> with the same height and the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns>The new <see cref="Size"/>.</returns>
        public Size WithWidth(float width)
        {
            return new Size(width, _height);
        }
        /// <summary>
        /// Inflates the size by a <see cref="Thickness"/>. 放大
        /// </summary>
        /// <param name="thickness">The thickness.</param>
        /// <returns>The inflated size.</returns>
        public Size Inflate(Thickness thickness)
        {
            return new Size(
                Math.Max(0, _width + thickness.Left + thickness.Right),
                Math.Max(0, _height + thickness.Top + thickness.Bottom));
        }
        /// <summary>
        /// IsEmpty - this returns true if this size is the Empty size.
        /// Note: If size is 0 this Size still contains a 0 or 1 dimensional set
        /// of points, so this method should not be used to check for 0 area.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _width < 0 || _height < 0;
            }
        }

        /// <summary>
        /// Width - Default is 0
        /// </summary>
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                //if (IsEmpty)
                //{
                //    throw new System.InvalidOperationException("宽度不能小于0");
                //}

                //if (value < 0)
                //{
                //    throw new System.ArgumentException("宽度不能小于0");
                //}

                _width = value;
            }
        }

        /// <summary>
        /// Height - Default is 0
        /// </summary>
        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                //if (IsEmpty)
                //{
                //    throw new System.InvalidOperationException("宽度不能小于0");
                //}

                //if (value < 0)
                //{
                //    throw new System.ArgumentException("高度不能小于0");
                //}

                _height = value;
            }
        }

        /// <summary>
        /// Constrains the size. 最小尺寸
        /// </summary>
        /// <param name="constraint">The size to constrain to.</param>
        /// <returns>The constrained size.</returns>
        public Size Constrain(Size constraint)
        {
            return new Size(
                Math.Min(_width, constraint._width),
                Math.Min(_height, constraint._height));
        }

        /// <summary>
        /// Deflates the size by a <see cref="Thickness"/>. 缩小
        /// </summary>
        /// <param name="thickness">The thickness.</param>
        /// <returns>The deflated size.</returns>
        /// <remarks>The deflated size cannot be less than 0.</remarks>
        public Size Deflate(Thickness thickness)
        {
            return new Size(
                Math.Max(0, _width - thickness.Left - thickness.Right),
                Math.Max(0, _height - thickness.Top - thickness.Bottom));
        }

        public static Size Ceiling(Size value)
        {
            return new Size((float)Math.Ceiling(value.Width), (float)Math.Ceiling(value.Height));
        }

        public static Size Round(Size value)
        {
            return new Size((float)Math.Round(value.Width), (float)Math.Round(value.Height));
        }
        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
        #endregion Public Methods

        #region Public Operators
        public static Size operator +(Size sz1, Size sz2)
        {
            return Add(sz1, sz2);
        }
        public static Size operator -(Size sz1, Size sz2)
        {
            return Subtract(sz1, sz2);
        }
        /// <summary>
        /// Explicit conversion to Vector.
        /// </summary>
        /// <returns>
        /// Vector - A Vector equal to this Size
        /// </returns>
        /// <param name="size"> Size - the Size to convert to a Vector </param>
        public static explicit operator Vector(Size size)
        {
            return new Vector(size._width, size._height);
        }

        /// <summary>
        /// Explicit conversion to Point
        /// </summary>
        /// <returns>
        /// Point - A Point equal to this Size
        /// </returns>
        /// <param name="size"> Size - the Size to convert to a Point </param>
        public static explicit operator Point(Size size)
        {
            return new Point(size._width, size._height);
        }

        /// <summary>
        /// Scales a size.
        /// </summary>
        /// <param name="size">The size</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The scaled size.</returns>
        public static Size operator *(Size size, float scale)
        {
            return new Size(size._width * scale, size._height * scale);
        }

        /// <summary>
        /// Scales a size.
        /// </summary>
        /// <param name="size">The size</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The scaled size.</returns>
        public static Size operator /(Size size, float scale)
        {
            return new Size(size._width / scale, size._height / scale);
        }

        #endregion Public Operators

        #region Private Methods

        static private Size CreateEmptySize()
        {
            Size size = new Size();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            size._width = float.NegativeInfinity;
            size._height = float.NegativeInfinity;
            return size;
        }

        #endregion Private Methods

        #region Private Fields

        private readonly static Size s_empty = CreateEmptySize();

        #endregion Private Fields

        /// <summary>
        /// Compares two Size instances for exact equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Size instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='size1'>The first Size to compare</param>
        /// <param name='size2'>The second Size to compare</param>
        public static bool operator ==(Size size1, Size size2)
        {
            //return Equals(size1, size2);
            return size1._width.Equal(size2._width) && size1._height.Equal(size2._height);
        }

        /// <summary>
        /// Compares two Size instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Size instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='size1'>The first Size to compare</param>
        /// <param name='size2'>The second Size to compare</param>
        public static bool operator !=(in Size size1, in Size size2)
        {
            return !(size1 == size2);
        }
        /// <summary>
        /// 相等和 == 不完全一致的，==是大约
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        /// <returns></returns>
        public static bool Equals(in Size size1, in Size size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            else
            {
                //return Math.Abs(size1.Width - size2.Width) < 0.0001 &&
                //       Math.Abs(size1.Height - size2.Height) < 0.0001;
                return size1._width.Equals(size2._width) && size1._height.Equals(size2._height);
            }
        }

        /// <summary>
        /// Equals - compares this Size with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Size and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Size))
            {
                return false;
            }

            Size value = (Size)o;
            return Size.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Size with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Size to compare to "this"</param>
        public bool Equals(Size value)
        {
            return Size.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Size
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Size
        /// </returns>
        public override int GetHashCode()
        {
            if (IsEmpty)
            {
                return 0;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return Width.GetHashCode() ^
                       Height.GetHashCode();
            }
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with Size data </param>
        ///// </summary>
        //public static Size Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Size value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Empty")
        //    {
        //        value = Empty;
        //    }
        //    else
        //    {
        //        value = new Size(
        //            Convert.Tofloat(firstToken, formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider));
        //    }

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}
        /// <summary>
        /// w,h
        /// </summary>
        /// <param name="point"></param>
        public static explicit operator Size(string point)
        {
            try
            {
                var temp = point.Split(',');
                return new Size(float.Parse(temp[0]), float.Parse(temp[1]));
            }
            catch (Exception e)
            {
                throw new Exception("Size字符串格式不对", e);
            }
        }

        /// <summary>
        /// Creates a string representation of this object based on the current culture.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public override string ToString()
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, null /* format provider */);
        }

        /// <summary>
        /// Creates a string representation of this object based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(format, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            if (IsEmpty)
            {
                return "Empty";
            }

            // Helper to get the numeric list separator for a given culture.
            char separator = ',';
            return String.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}",
                                 separator,
                                 _width,
                                 _height);
        }


        internal float _width;
        internal float _height;

        public static Size Parse(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            if (parts.Count == 2)
            {
                return new Size(float.Parse(parts[0]), float.Parse(parts[1]));
            }
            else
            {
                throw new FormatException("Invalid Size.");
            }
        }
    }
}
