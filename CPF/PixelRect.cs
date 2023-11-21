using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public struct PixelRect : IFormattable
    {
        public static bool operator ==(PixelRect rect1, PixelRect rect2)
        {
            return rect1.X.Equal(rect2.X) &&
                   rect1.Y.Equal(rect2.Y) &&
                   rect1.Width.Equal(rect2.Width) &&
                   rect1.Height.Equal(rect2.Height);
        }

        /// <summary>
        /// Compares two PixelRect instances for exact inequality.
        /// Note that int values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, int.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two PixelRect instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first PixelRect to compare</param>
        /// <param name='rect2'>The second PixelRect to compare</param>
        public static bool operator !=(PixelRect rect1, PixelRect rect2)
        {
            return !(rect1 == rect2);
        }
        /// <summary>
        /// Compares two PixelRect instances for object equality.  In this equality
        /// int.NaN is equal to itself, unlike in numeric equality.
        /// Note that int values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two PixelRect instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first PixelRect to compare</param>
        /// <param name='rect2'>The second PixelRect to compare</param>
        public static bool Equals(PixelRect rect1, PixelRect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            else
            {
                return rect1.X.Equals(rect2.X) &&
                       rect1.Y.Equals(rect2.Y) &&
                       rect1.Width.Equals(rect2.Width) &&
                       rect1.Height.Equals(rect2.Height);
            }
        }

        /// <summary>
        /// Equals - compares this PixelRect with the passed in object.  In this equality
        /// int.NaN is equal to itself, unlike in numeric equality.
        /// Note that int values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of PixelRect and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is PixelRect))
            {
                return false;
            }

            PixelRect value = (PixelRect)o;
            return PixelRect.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this PixelRect with the passed in object.  In this equality
        /// int.NaN is equal to itself, unlike in numeric equality.
        /// Note that int values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The PixelRect to compare to "this"</param>
        public bool Equals(PixelRect value)
        {
            return PixelRect.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this PixelRect
        /// </summary>
        /// <returns>
        /// int - the HashCode for this PixelRect
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
                return X.GetHashCode() ^
                       Y.GetHashCode() ^
                       Width.GetHashCode() ^
                       Height.GetHashCode();
            }
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with PixelRect data </param>
        ///// </summary>
        //public static PixelRect Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    PixelRect value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Empty")
        //    {
        //        value = Empty;
        //    }
        //    else
        //    {
        //        value = new PixelRect(
        //            Convert.Tofloat(firstToken, formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider));
        //    }

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}
        public static implicit operator PixelRect(string n)
        {
            if (n.Trim().ToLower() == "empty")
            {
                return Empty;
            }
            try
            {
                var temp = n.Split(',');
                return new PixelRect(int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]));
            }
            catch (Exception e)
            {
                throw new Exception("无法将字符串转换成PixelRect " + n, e);
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
                                 "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}",
                                 separator,
                                 _x,
                                 _y,
                                 _width,
                                 _height);
        }


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields


        internal int _x;
        internal int _y;
        internal int _width;
        internal int _height;




        #endregion Internal Fields
        #region Constructors

        ///// <summary>
        ///// Constructor which sets the initial values to the values of the parameters
        ///// </summary>
        //public PixelRect(in Point location,
        //           in Size size)
        //{
        //    if (size.IsEmpty)
        //    {
        //        this = s_empty;
        //    }
        //    else
        //    {
        //        _x = location._x;
        //        _y = location._y;
        //        _width = size._width;
        //        _height = size._height;
        //    }
        //}

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters.
        /// Width and Height must be non-negative
        /// </summary>
        public PixelRect(in int x,
                    in int y,
                    in int width,
                    in int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException("尺寸不能小于0");
            }

            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        #endregion Constructors

        #region Statics

        /// <summary>
        /// Empty - a static property which provides an Empty rectangle.  X and Y are positive-infinity
        /// and Width and Height are negative infinity.  This is the only situation where Width or
        /// Height can be negative.
        /// </summary>
        public static PixelRect Empty
        {
            get
            {
                return s_empty;
            }
        }

        #endregion Statics

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return _width <= 0 && _height <= 0 && _x == 0 && _y == 0;
            }
        }

        /// <summary>
        /// Location - The Point representing the origin of the Rectangle
        /// </summary>
        public PixelPoint Location
        {
            get
            {
                return new PixelPoint(_x, _y);
            }
            set
            {
                //if (IsEmpty)
                //{
                //    throw new System.InvalidOperationException("宽度不能小于0");
                //}

                _x = value.X;
                _y = value.Y;
            }
        }

        /// <summary>
        /// Size - The Size representing the area of the Rectangle
        /// </summary>
        public PixelSize Size
        {
            get
            {
                if (IsEmpty)
                    return PixelSize.Empty;
                return new PixelSize(_width, _height);
            }
            set
            {
                _width = value.Width;
                _height = value.Height;
            }
        }

        /// <summary>
        /// X - The X coordinate of the Location.
        /// If this is the empty rectangle, the value will be positive infinity.
        /// If this rect is Empty, setting this property is illegal.
        /// </summary>
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                //if (IsEmpty)
                //{
                //    throw new System.InvalidOperationException("宽度不能小于0");
                //}

                _x = value;
            }

        }

        /// <summary>
        /// Y - The Y coordinate of the Location
        /// If this is the empty rectangle, the value will be positive infinity.
        /// If this rect is Empty, setting this property is illegal.
        /// </summary>
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                //if (IsEmpty)
                //{
                //    throw new System.InvalidOperationException("宽度不能小于0");
                //}

                _y = value;
            }
        }

        /// <summary>
        /// Width - The Width component of the Size.  This cannot be set to negative, and will only
        /// be negative if this is the empty rectangle, in which case it will be negative infinity.
        /// If this rect is Empty, setting this property is illegal.
        /// </summary>
        public int Width
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

                if (value < 0)
                {
                    throw new System.ArgumentException("宽度不能小于0");
                }

                _width = value;
            }
        }

        /// <summary>
        /// Height - The Height component of the Size.  This cannot be set to negative, and will only
        /// be negative if this is the empty rectangle, in which case it will be negative infinity.
        /// If this rect is Empty, setting this property is illegal.
        /// </summary>
        public int Height
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

                if (value < 0)
                {
                    throw new System.ArgumentException("高度不能小于0");
                }

                _height = value;
            }
        }

        /// <summary>
        /// Left Property - This is a read-only alias for X
        /// If this is the empty rectangle, the value will be positive infinity.
        /// </summary>
        public int Left
        {
            get
            {
                return _x;
            }
        }

        /// <summary>
        /// Top Property - This is a read-only alias for Y
        /// If this is the empty rectangle, the value will be positive infinity.
        /// </summary>
        public int Top
        {
            get
            {
                return _y;
            }
        }

        /// <summary>
        /// Right Property - This is a read-only alias for X + Width
        /// If this is the empty rectangle, the value will be negative infinity.
        /// </summary>
        public int Right
        {
            get
            {
                //if (IsEmpty)
                //{
                //    return int.NegativeInfinity;
                //}

                return _x + _width;
            }
        }

        /// <summary>
        /// Bottom Property - This is a read-only alias for Y + Height
        /// If this is the empty rectangle, the value will be negative infinity.
        /// </summary>
        public int Bottom
        {
            get
            {
                //if (IsEmpty)
                //{
                //    return int.NegativeInfinity;
                //}

                return _y + _height;
            }
        }

        /// <summary>
        /// TopLeft Property - This is a read-only alias for the Point which is at X, Y
        /// If this is the empty rectangle, the value will be positive infinity, positive infinity.
        /// </summary>
        public PixelPoint TopLeft
        {
            get
            {
                return new PixelPoint(Left, Top);
            }
        }

        /// <summary>
        /// TopRight Property - This is a read-only alias for the Point which is at X + Width, Y
        /// If this is the empty rectangle, the value will be negative infinity, positive infinity.
        /// </summary>
        public PixelPoint TopRight
        {
            get
            {
                return new PixelPoint(Right, Top);
            }
        }

        /// <summary>
        /// BottomLeft Property - This is a read-only alias for the Point which is at X, Y + Height
        /// If this is the empty rectangle, the value will be positive infinity, negative infinity.
        /// </summary>
        public PixelPoint BottomLeft
        {
            get
            {
                return new PixelPoint(Left, Bottom);
            }
        }

        /// <summary>
        /// BottomRight Property - This is a read-only alias for the Point which is at X + Width, Y + Height
        /// If this is the empty rectangle, the value will be negative infinity, negative infinity.
        /// </summary>
        public PixelPoint BottomRight
        {
            get
            {
                return new PixelPoint(Right, Bottom);
            }
        }
        /// <summary>
        /// Gets the center point of the rectangle.
        /// </summary>
        public PixelPoint Center { get { return new PixelPoint(_x + (_width / 2), _y + (_height / 2)); } }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Contains - Returns true if the Point is within the rectangle, inclusive of the edges.
        /// Returns false otherwise.
        /// </summary>
        /// <param name="point"> The point which is being tested </param>
        /// <returns>
        /// Returns true if the Point is within the rectangle.
        /// Returns false otherwise
        /// </returns>
        public bool Contains(in PixelPoint point)
        {
            return Contains(point.X, point.Y);
        }

        /// <summary>
        /// Contains - Returns true if the Point represented by x,y is within the rectangle inclusive of the edges.
        /// Returns false otherwise.
        /// </summary>
        /// <param name="x"> X coordinate of the point which is being tested </param>
        /// <param name="y"> Y coordinate of the point which is being tested </param>
        /// <returns>
        /// Returns true if the Point represented by x,y is within the rectangle.
        /// Returns false otherwise.
        /// </returns>
        public bool Contains(in int x, in int y)
        {
            if (IsEmpty)
            {
                return false;
            }

            return ContainsInternal(x, y);
        }

        /// <summary>
        /// Contains - Returns true if the PixelRect non-Empty and is entirely contained within the
        /// rectangle, inclusive of the edges.
        /// Returns false otherwise
        /// </summary>
        public bool Contains(in PixelRect rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (_x <= rect._x &&
                    _y <= rect._y &&
                    _x + _width >= rect._x + rect._width &&
                    _y + _height >= rect._y + rect._height);
        }

        /// <summary>
        /// IntersectsWith - Returns true if the PixelRect intersects with this rectangle
        /// Returns false otherwise.
        /// Note that if one edge is coincident, this is considered an intersection.
        /// </summary>
        /// <returns>
        /// Returns true if the PixelRect intersects with this rectangle
        /// Returns false otherwise.
        /// or Height
        /// </returns>
        /// <param name="rect"> PixelRect </param>
        public bool IntersectsWith(in PixelRect rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (rect.Left <= Right) &&
                   (rect.Right >= Left) &&
                   (rect.Top <= Bottom) &&
                   (rect.Bottom >= Top);
        }

        /// <summary>
        /// Intersect - Update this rectangle to be the intersection of this and rect
        /// If either this or rect are Empty, the result is Empty as well.
        /// </summary>
        /// <param name="rect"> The rect to intersect with this </param>
        public void Intersect(in PixelRect rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                int left = Math.Max(Left, rect.Left);
                int top = Math.Max(Top, rect.Top);

                //  Max with 0 to prevent int weirdness from causing us to be (-epsilon..0)
                _width = Math.Max(Math.Min(Right, rect.Right) - left, 0);
                _height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0);

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Intersect - Return the result of the intersection of rect1 and rect2.
        /// If either this or rect are Empty, the result is Empty as well.
        /// </summary>
        public static PixelRect Intersect(PixelRect rect1, PixelRect rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        /// <summary>
        /// Union - Update this rectangle to be the union of this and rect.
        /// </summary>
        public void Union(in PixelRect rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                int left = Math.Min(Left, rect.Left);
                int top = Math.Min(Top, rect.Top);


                // We need this check so that the math does not result in NaN
                //if ((rect.Width == int.PositiveInfinity) || (Width == int.PositiveInfinity))
                //{
                //    _width = int.PositiveInfinity;
                //}
                //else
                //{
                    //  Max with 0 to prevent int weirdness from causing us to be (-epsilon..0)                    
                    int maxRight = Math.Max(Right, rect.Right);
                    _width = Math.Max(maxRight - left, 0);
                //}

                // We need this check so that the math does not result in NaN
                //if ((rect.Height == int.PositiveInfinity) || (Height == int.PositiveInfinity))
                //{
                //    _height = int.PositiveInfinity;
                //}
                //else
                //{
                    //  Max with 0 to prevent int weirdness from causing us to be (-epsilon..0)
                    int maxBottom = Math.Max(Bottom, rect.Bottom);
                    _height = Math.Max(maxBottom - top, 0);
                //}

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Union - Return the result of the union of rect1 and rect2.
        /// </summary>
        public static PixelRect Union(PixelRect rect1, PixelRect rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        ///// <summary>
        ///// Union - Update this rectangle to be the union of this and point.
        ///// </summary>
        //public void Union(PixelPoint point)
        //{
        //    Union(new PixelRect(point, point));
        //}

        ///// <summary>
        ///// Union - Return the result of the union of rect and point.
        ///// </summary>
        //public static PixelRect Union(PixelRect rect, PixelPoint point)
        //{
        //    rect.Union(new PixelRect(point, point));
        //    return rect;
        //}

        ///// <summary>
        ///// Offset - translate the Location by the offset provided.
        ///// If this is Empty, this method is illegal.
        ///// </summary>
        //public void Offset(Vector offsetVector)
        //{
        //    _x += offsetVector._x;
        //    _y += offsetVector._y;
        //}

        /// <summary>
        /// Offset - translate the Location by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Offset(int offsetX, int offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }

        ///// <summary>
        ///// Offset - return the result of offsetting rect by the offset provided
        ///// If this is Empty, this method is illegal.
        ///// </summary>
        //public static PixelRect Offset(PixelRect rect, Vector offsetVector)
        //{
        //    rect.Offset(offsetVector.X, offsetVector.Y);
        //    return rect;
        //}

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static PixelRect Offset(PixelRect rect, int offsetX, int offsetY)
        {
            rect.Offset(offsetX, offsetY);
            return rect;
        }

        /// <summary>
        /// Inflate - inflate the bounds by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Inflate(PixelSize size)
        {
            Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// Inflate - inflate the bounds by the size provided, in all directions.
        /// If -width is > Width / 2 or -height is > Height / 2, this PixelRect becomes Empty
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Inflate(int width, int height)
        {
            _x -= width;
            _y -= height;

            // Do two additions rather than multiplication by 2 to avoid spurious overflow
            // That is: (A + 2 * B) != ((A + B) + B) if 2*B overflows.
            // Note that multiplication by 2 might work in this case because A should start
            // positive & be "clamped" to positive after, but consider A = Inf & B = -MAX.
            _width += width;
            _width += width;
            _height += height;
            _height += height;

            // We catch the case of inflation by less than -width/2 or -height/2 here.  This also
            // maintains the invariant that either the PixelRect is Empty or _width and _height are
            // non-negative, even if the user parameters were NaN, though this isn't strictly maintained
            // by other methods.
            if (!(_width >= 0 && _height >= 0))
            {
                this = s_empty;
            }
        }

        /// <summary>
        /// Inflate - return the result of inflating rect by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static PixelRect Inflate(PixelRect rect, PixelSize size)
        {
            rect.Inflate(size.Width, size.Height);
            return rect;
        }

        /// <summary>
        /// Inflate - return the result of inflating rect by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static PixelRect Inflate(PixelRect rect, int width, int height)
        {
            rect.Inflate(width, height);
            return rect;
        }

        ///// <summary>
        ///// Returns the bounds of the transformed rectangle.
        ///// The Empty PixelRect is not affected by this call.
        ///// </summary>
        ///// <returns>
        ///// The rect which results from the transformation.
        ///// </returns>
        ///// <param name="rect"> The PixelRect to transform. </param>
        ///// <param name="matrix"> The Matrix by which to transform. </param>
        //public static PixelRect Transform(PixelRect rect, Matrix matrix)
        //{
        //    MatrixUtil.TransformRect(ref rect, ref matrix);
        //    return rect;
        //}

        ///// <summary>
        ///// Updates rectangle to be the bounds of the original value transformed
        ///// by the matrix.
        ///// The Empty PixelRect is not affected by this call.        
        ///// </summary>
        ///// <param name="matrix"> Matrix </param>
        //public void Transform(Matrix matrix)
        //{
        //    MatrixUtil.TransformRect(ref this, ref matrix);
        //}

        /// <summary>
        /// Scale the rectangle in the X and Y directions
        /// </summary>
        /// <param name="scaleX"> The scale in X </param>
        /// <param name="scaleY"> The scale in Y </param>
        public void Scale(int scaleX, int scaleY)
        {
            if (IsEmpty)
            {
                return;
            }

            _x *= scaleX;
            _y *= scaleY;
            _width *= scaleX;
            _height *= scaleY;

            // If the scale in the X dimension is negative, we need to normalize X and Width
            if (scaleX < 0)
            {
                // Make X the left-most edge again
                _x += _width;

                // and make Width positive
                _width *= -1;
            }

            // Do the same for the Y dimension
            if (scaleY < 0)
            {
                // Make Y the top-most edge again
                _y += _height;

                // and make Height positive
                _height *= -1;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// ContainsInternal - Performs just the "point inside" logic
        /// </summary>
        /// <returns>
        /// bool - true if the point is inside the rect
        /// </returns>
        /// <param name="x"> The x-coord of the point to test </param>
        /// <param name="y"> The y-coord of the point to test </param>
        private bool ContainsInternal(in int x, in int y)
        {
            // We include points on the edge as "contained".
            // We do "x - _width <= _x" instead of "x <= _x + _width"
            // so that this check works when _width is PositiveInfinity
            // and _x is NegativeInfinity.
            return ((x >= _x) && (x - _width <= _x) &&
                    (y >= _y) && (y - _height <= _y));
        }

        static private PixelRect CreateEmptyRect()
        {
            PixelRect rect = new PixelRect();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            //rect._x = int.PositiveInfinity;
            //rect._y = int.PositiveInfinity;
            //rect._width = int.NegativeInfinity;
            //rect._height = int.NegativeInfinity;
            return rect;
        }

        #endregion Private Methods

        #region Private Fields

        private readonly static PixelRect s_empty = CreateEmptyRect();

        #endregion Private Fields
    }
}
