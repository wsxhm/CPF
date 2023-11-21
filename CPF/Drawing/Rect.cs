using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    [Serializable]
    public struct Rect : IFormattable
    {
        /// <summary>
        /// rect.Left * scale, rect.Top * scale, rect.Width * scale, rect.Height * scale
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Rect operator *(Rect rect1, float scale)
        {
            return new Rect(rect1.Left * scale, rect1.Top * scale, rect1.Width * scale, rect1.Height * scale);
        }
        public static bool operator ==(Rect rect1, Rect rect2)
        {
            return rect1.X.Equal(rect2.X) &&
                   rect1.Y.Equal(rect2.Y) &&
                   rect1.Width.Equal(rect2.Width) &&
                   rect1.Height.Equal(rect2.Height);
        }

        /// <summary>
        /// Compares two Rect instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Rect instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first Rect to compare</param>
        /// <param name='rect2'>The second Rect to compare</param>
        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return !(rect1 == rect2);
        }
        /// <summary>
        /// Compares two Rect instances for object equality.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Rect instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first Rect to compare</param>
        /// <param name='rect2'>The second Rect to compare</param>
        public static bool Equals(Rect rect1, Rect rect2)
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
        /// Equals - compares this Rect with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Rect and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Rect))
            {
                return false;
            }

            Rect value = (Rect)o;
            return Rect.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Rect with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Rect to compare to "this"</param>
        public bool Equals(Rect value)
        {
            return Rect.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Rect
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Rect
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
        ///// <param name="source"> string with Rect data </param>
        ///// </summary>
        //public static Rect Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Rect value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Empty")
        //    {
        //        value = Empty;
        //    }
        //    else
        //    {
        //        value = new Rect(
        //            Convert.Tofloat(firstToken, formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider));
        //    }

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}
        public static implicit operator Rect(string n)
        {
            if (n.Trim().ToLower() == "empty")
            {
                return Empty;
            }
            try
            {
                var temp = n.Split(',');
                return new Rect(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), float.Parse(temp[3]));
            }
            catch (Exception e)
            {
                throw new Exception("无法将字符串转换成Rect " + n, e);
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


        internal float _x;
        internal float _y;
        internal float _width;
        internal float _height;




        #endregion Internal Fields
        #region Constructors

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters
        /// </summary>
        public Rect(in Point location,
                   in Size size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                _x = location._x;
                _y = location._y;
                _width = size._width;
                _height = size._height;
            }
        }

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters.
        /// Width and Height must be non-negative
        /// </summary>
        public Rect(in float x,
                    in float y,
                    in float width,
                    in float height)
        {
            if (width < 0 || height < 0)
            {
                //throw new ArgumentException($"尺寸不能小于0:width:{width},height:{height}");
                Console.WriteLine($"尺寸不能小于0:width:{width},height:{height}");
                System.Diagnostics.Debug.WriteLine($"尺寸不能小于0:width:{width},height:{height}");
            }

            _x = x;
            _y = y;
            _width = Math.Max(0, width);
            _height = Math.Max(0, height);
        }

        /// <summary>
        /// Constructor which sets the initial values to bound the two points provided.
        /// </summary>
        public Rect(Point point1,
                    Point point2)
        {
            _x = Math.Min(point1._x, point2._x);
            _y = Math.Min(point1._y, point2._y);

            //  Max with 0 to prevent float weirdness from causing us to be (-epsilon..0)
            _width = Math.Max(Math.Max(point1._x, point2._x) - _x, 0);
            _height = Math.Max(Math.Max(point1._y, point2._y) - _y, 0);
        }

        /// <summary>
        /// Constructor which sets the initial values to bound the point provided and the point
        /// which results from point + vector.
        /// </summary>
        public Rect(Point point,
                    Vector vector)
            : this(point, point + vector)
        {
        }

        /// <summary>
        /// Constructor which sets the initial values to bound the (0,0) point and the point 
        /// that results from (0,0) + size. 
        /// </summary>
        public Rect(Size size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                _x = _y = 0;
                _width = size.Width;
                _height = size.Height;
            }
        }

        #endregion Constructors

        #region Statics

        /// <summary>
        /// Empty - a static property which provides an Empty rectangle.  X and Y are positive-infinity
        /// and Width and Height are negative infinity.  This is the only situation where Width or
        /// Height can be negative.
        /// </summary>
        public static Rect Empty
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
        public Point Location
        {
            get
            {
                return new Point(_x, _y);
            }
            set
            {
                //if (IsEmpty)
                //{
                //    throw new System.InvalidOperationException("宽度不能小于0");
                //}

                _x = value._x;
                _y = value._y;
            }
        }

        /// <summary>
        /// Size - The Size representing the area of the Rectangle
        /// </summary>
        public Size Size
        {
            get
            {
                if (IsEmpty)
                    return Size.Empty;
                return new Size(_width, _height);
            }
            set
            {
                if (value.IsEmpty)
                {
                    this = s_empty;
                }
                else
                {
                    //if (IsEmpty)
                    //{
                    //    throw new System.InvalidOperationException("宽度不能小于0");
                    //}

                    _width = value._width;
                    _height = value._height;
                }
            }
        }

        /// <summary>
        /// X - The X coordinate of the Location.
        /// If this is the empty rectangle, the value will be positive infinity.
        /// If this rect is Empty, setting this property is illegal.
        /// </summary>
        public float X
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
        public float Y
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
        public float Left
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
        public float Top
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
        public float Right
        {
            get
            {
                if (IsEmpty)
                {
                    return float.NegativeInfinity;
                }

                return _x + _width;
            }
        }

        /// <summary>
        /// Bottom Property - This is a read-only alias for Y + Height
        /// If this is the empty rectangle, the value will be negative infinity.
        /// </summary>
        public float Bottom
        {
            get
            {
                if (IsEmpty)
                {
                    return float.NegativeInfinity;
                }

                return _y + _height;
            }
        }

        /// <summary>
        /// TopLeft Property - This is a read-only alias for the Point which is at X, Y
        /// If this is the empty rectangle, the value will be positive infinity, positive infinity.
        /// </summary>
        public Point TopLeft
        {
            get
            {
                return new Point(Left, Top);
            }
        }

        /// <summary>
        /// TopRight Property - This is a read-only alias for the Point which is at X + Width, Y
        /// If this is the empty rectangle, the value will be negative infinity, positive infinity.
        /// </summary>
        public Point TopRight
        {
            get
            {
                return new Point(Right, Top);
            }
        }

        /// <summary>
        /// BottomLeft Property - This is a read-only alias for the Point which is at X, Y + Height
        /// If this is the empty rectangle, the value will be positive infinity, negative infinity.
        /// </summary>
        public Point BottomLeft
        {
            get
            {
                return new Point(Left, Bottom);
            }
        }

        /// <summary>
        /// BottomRight Property - This is a read-only alias for the Point which is at X + Width, Y + Height
        /// If this is the empty rectangle, the value will be negative infinity, negative infinity.
        /// </summary>
        public Point BottomRight
        {
            get
            {
                return new Point(Right, Bottom);
            }
        }
        /// <summary>
        /// Gets the center point of the rectangle.
        /// </summary>
        public Point Center { get { return new Point(_x + (_width / 2), _y + (_height / 2)); } }
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
        public bool Contains(in Point point)
        {
            return Contains(point._x, point._y);
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
        public bool Contains(in float x, in float y)
        {
            if (IsEmpty)
            {
                return false;
            }

            return ContainsInternal(x, y);
        }

        /// <summary>
        /// Contains - Returns true if the Rect non-Empty and is entirely contained within the
        /// rectangle, inclusive of the edges.
        /// Returns false otherwise
        /// </summary>
        public bool Contains(in Rect rect)
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
        /// IntersectsWith - Returns true if the Rect intersects with this rectangle
        /// Returns false otherwise.
        /// Note that if one edge is coincident, this is considered an intersection.
        /// </summary>
        /// <returns>
        /// Returns true if the Rect intersects with this rectangle
        /// Returns false otherwise.
        /// or Height
        /// </returns>
        /// <param name="rect"> Rect </param>
        public bool IntersectsWith(in Rect rect)
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
        /// 更新为交集
        /// </summary>
        /// <param name="rect"> The rect to intersect with this </param>
        public void Intersect(in Rect rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                float left = Math.Max(Left, rect.Left);
                float top = Math.Max(Top, rect.Top);

                //  Max with 0 to prevent float weirdness from causing us to be (-epsilon..0)
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
        public static Rect Intersect(Rect rect1, Rect rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        /// <summary>
        /// Union - Update this rectangle to be the union of this and rect.
        /// </summary>
        public void Union(in Rect rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                float left = Math.Min(Left, rect.Left);
                float top = Math.Min(Top, rect.Top);


                // We need this check so that the math does not result in NaN
                if ((rect.Width == float.PositiveInfinity) || (Width == float.PositiveInfinity))
                {
                    _width = float.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent float weirdness from causing us to be (-epsilon..0)                    
                    float maxRight = Math.Max(Right, rect.Right);
                    _width = Math.Max(maxRight - left, 0);
                }

                // We need this check so that the math does not result in NaN
                if ((rect.Height == float.PositiveInfinity) || (Height == float.PositiveInfinity))
                {
                    _height = float.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent float weirdness from causing us to be (-epsilon..0)
                    float maxBottom = Math.Max(Bottom, rect.Bottom);
                    _height = Math.Max(maxBottom - top, 0);
                }

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Union - Return the result of the union of rect1 and rect2.
        /// </summary>
        public static Rect Union(Rect rect1, Rect rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        /// <summary>
        /// Union - Update this rectangle to be the union of this and point.
        /// </summary>
        public void Union(Point point)
        {
            Union(new Rect(point, point));
        }

        /// <summary>
        /// Union - Return the result of the union of rect and point.
        /// </summary>
        public static Rect Union(Rect rect, Point point)
        {
            rect.Union(new Rect(point, point));
            return rect;
        }

        /// <summary>
        /// Offset - translate the Location by the offset provided.
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Offset(Vector offsetVector)
        {
            _x += offsetVector._x;
            _y += offsetVector._y;
        }

        /// <summary>
        /// Offset - translate the Location by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Offset(float offsetX, float offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Rect Offset(Rect rect, Vector offsetVector)
        {
            rect.Offset(offsetVector.X, offsetVector.Y);
            return rect;
        }

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Rect Offset(Rect rect, float offsetX, float offsetY)
        {
            rect.Offset(offsetX, offsetY);
            return rect;
        }

        /// <summary>
        /// Inflate - inflate the bounds by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Inflate(Size size)
        {
            Inflate(size._width, size._height);
        }

        /// <summary>
        /// Inflate - inflate the bounds by the size provided, in all directions.
        /// If -width is > Width / 2 or -height is > Height / 2, this Rect becomes Empty
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Inflate(float width, float height)
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
            // maintains the invariant that either the Rect is Empty or _width and _height are
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
        public static Rect Inflate(Rect rect, Size size)
        {
            rect.Inflate(size._width, size._height);
            return rect;
        }

        /// <summary>
        /// Inflate - return the result of inflating rect by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Rect Inflate(Rect rect, float width, float height)
        {
            rect.Inflate(width, height);
            return rect;
        }

        /// <summary>
        /// Returns the bounds of the transformed rectangle.
        /// The Empty Rect is not affected by this call.
        /// </summary>
        /// <returns>
        /// The rect which results from the transformation.
        /// </returns>
        /// <param name="rect"> The Rect to transform. </param>
        /// <param name="matrix"> The Matrix by which to transform. </param>
        public static Rect Transform(Rect rect, Matrix matrix)
        {
            MatrixUtil.TransformRect(ref rect, ref matrix);
            return rect;
        }

        /// <summary>
        /// Updates rectangle to be the bounds of the original value transformed
        /// by the matrix.
        /// The Empty Rect is not affected by this call.        
        /// </summary>
        /// <param name="matrix"> Matrix </param>
        public void Transform(Matrix matrix)
        {
            MatrixUtil.TransformRect(ref this, ref matrix);
        }

        /// <summary>
        /// Scale the rectangle in the X and Y directions
        /// </summary>
        /// <param name="scaleX"> The scale in X </param>
        /// <param name="scaleY"> The scale in Y </param>
        public void Scale(float scaleX, float scaleY)
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
        private bool ContainsInternal(in float x, in float y)
        {
            // We include points on the edge as "contained".
            // We do "x - _width <= _x" instead of "x <= _x + _width"
            // so that this check works when _width is PositiveInfinity
            // and _x is NegativeInfinity.
            return ((x >= _x) && (x - _width <= _x) &&
                    (y >= _y) && (y - _height <= _y));
        }

        static private Rect CreateEmptyRect()
        {
            Rect rect = new Rect();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            rect._x = float.PositiveInfinity;
            rect._y = float.PositiveInfinity;
            rect._width = float.NegativeInfinity;
            rect._height = float.NegativeInfinity;
            return rect;
        }

        #endregion Private Methods

        #region Private Fields

        private readonly static Rect s_empty = CreateEmptyRect();

        #endregion Private Fields
    }
}
