using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CPF.Drawing.Media3D
{
    [Serializable]
    public struct Rect3D : IFormattable
    {
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods




        /// <summary>
        /// Compares two Rect3D instances for exact equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Rect3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first Rect3D to compare</param>
        /// <param name='rect2'>The second Rect3D to compare</param>
        public static bool operator ==(Rect3D rect1, Rect3D rect2)
        {
            return rect1.X == rect2.X &&
                   rect1.Y == rect2.Y &&
                   rect1.Z == rect2.Z &&
                   rect1.SizeX == rect2.SizeX &&
                   rect1.SizeY == rect2.SizeY &&
                   rect1.SizeZ == rect2.SizeZ;
        }

        /// <summary>
        /// Compares two Rect3D instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Rect3D instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first Rect3D to compare</param>
        /// <param name='rect2'>The second Rect3D to compare</param>
        public static bool operator !=(Rect3D rect1, Rect3D rect2)
        {
            return !(rect1 == rect2);
        }
        /// <summary>
        /// Compares two Rect3D instances for object equality.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Rect3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='rect1'>The first Rect3D to compare</param>
        /// <param name='rect2'>The second Rect3D to compare</param>
        public static bool Equals(Rect3D rect1, Rect3D rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            else
            {
                return rect1.X.Equals(rect2.X) &&
                       rect1.Y.Equals(rect2.Y) &&
                       rect1.Z.Equals(rect2.Z) &&
                       rect1.SizeX.Equals(rect2.SizeX) &&
                       rect1.SizeY.Equals(rect2.SizeY) &&
                       rect1.SizeZ.Equals(rect2.SizeZ);
            }
        }

        /// <summary>
        /// Equals - compares this Rect3D with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Rect3D and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Rect3D))
            {
                return false;
            }

            Rect3D value = (Rect3D)o;
            return Rect3D.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Rect3D with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Rect3D to compare to "this"</param>
        public bool Equals(Rect3D value)
        {
            return Rect3D.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Rect3D
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Rect3D
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
                       Z.GetHashCode() ^
                       SizeX.GetHashCode() ^
                       SizeY.GetHashCode() ^
                       SizeZ.GetHashCode();
            }
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with Rect3D data </param>
        ///// </summary>
        //public static Rect3D Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Rect3D value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Empty")
        //    {
        //        value = Empty;
        //    }
        //    else
        //    {
        //        value = new Rect3D(
        //            Convert.ToDouble(firstToken, formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider));
        //    }

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}

   

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties


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
            //char separator = MS.Internal.TokenizerHelper.GetNumericListSeparator(provider);
            char separator = ',';
            return String.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}",
                                 separator,
                                 _x,
                                 _y,
                                 _z,
                                 _sizeX,
                                 _sizeY,
                                 _sizeZ);
        }



        #endregion Internal Properties


        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields


        internal float _x;
        internal float _y;
        internal float _z;
        internal float _sizeX;
        internal float _sizeY;
        internal float _sizeZ;




        #endregion Internal Fields
        #region Constructors

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters.
        /// </summary>
        /// <param name="location">Location of the new rectangle.</param>
        /// <param name="size">Size of the new rectangle.</param>
        public Rect3D(Point3D location, Size3D size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                _x = location._x;
                _y = location._y;
                _z = location._z;
                _sizeX = size._x;
                _sizeY = size._y;
                _sizeZ = size._z;
            }
            Debug.Assert(size.IsEmpty == IsEmpty);
        }

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters.
        /// SizeX, sizeY, sizeZ must be non-negative.
        /// </summary>
        /// <param name="x">Value of the X location coordinate of the new rectangle.</param>
        /// <param name="y">Value of the X location coordinate of the new rectangle.</param>
        /// <param name="z">Value of the X location coordinate of the new rectangle.</param>
        /// <param name="sizeX">Size of the new rectangle in X dimension.</param>
        /// <param name="sizeY">Size of the new rectangle in Y dimension.</param>
        /// <param name="sizeZ">Size of the new rectangle in Z dimension.</param>
        public Rect3D(float x, float y, float z, float sizeX, float sizeY, float sizeZ)
        {
            if (sizeX < 0 || sizeY < 0 || sizeZ < 0)
            {
                throw new System.ArgumentException("不能小于0");
            }

            _x = x;
            _y = y;
            _z = z;
            _sizeX = sizeX;
            _sizeY = sizeY;
            _sizeZ = sizeZ;
        }

        /// <summary>
        /// Constructor which sets the initial values to bound the two points provided.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        internal Rect3D(Point3D point1, Point3D point2)
        {
            _x = Math.Min(point1._x, point2._x);
            _y = Math.Min(point1._y, point2._y);
            _z = Math.Min(point1._z, point2._z);
            _sizeX = Math.Max(point1._x, point2._x) - _x;
            _sizeY = Math.Max(point1._y, point2._y) - _y;
            _sizeZ = Math.Max(point1._z, point2._z) - _z;
        }

        /// <summary>
        /// Constructor which sets the initial values to bound the point provided and the point
        /// which results from point + vector.
        /// </summary>
        /// <param name="point">Location of the rectangle.</param>
        /// <param name="vector">Vector extending the rectangle from the location.</param>
        internal Rect3D(Point3D point, Vector3D vector) : this(point, point + vector)
        {

        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// Empty - a static property which provides an Empty rectangle.  X, Y, and Z are 
        /// positive-infinity and sizes are negative infinity.  This is the only situation
        /// where size can be negative.
        /// </summary>
        public static Rect3D Empty
        {
            get
            {
                return s_empty;
            }
        }

        /// <summary>
        /// IsEmpty - this returns true if this rect is the Empty rectangle.
        /// Note: If size is 0 this Rectangle still contains a 0 or 1 dimensional set
        /// of points, so this method should not be used to check for 0 area.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _sizeX < 0;
            }
        }

        /// <summary>
        /// The point representing the origin of the rectangle.
        /// </summary>
        public Point3D Location
        {
            get
            {
                return new Point3D(_x, _y, _z);
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _x = value._x;
                _y = value._y;
                _z = value._z;
            }
        }

        /// <summary>
        /// The size representing the area of the rectangle.
        /// </summary>
        public Size3D Size
        {
            get
            {
                if (IsEmpty)
                    return Size3D.Empty;
                else
                    return new Size3D(_sizeX, _sizeY, _sizeZ);
            }
            set
            {
                if (value.IsEmpty)
                {
                    this = s_empty;
                }
                else
                {
                    if (IsEmpty)
                    {
                        throw new System.InvalidOperationException("Size不能小于0");
                    }

                    _sizeX = value._x;
                    _sizeY = value._y;
                    _sizeZ = value._z;
                }
            }
        }

        /// <summary>
        /// Size of the rectangle in the X dimension.
        /// </summary>
        public float SizeX
        {
            get
            {
                return _sizeX;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                if (value < 0)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _sizeX = value;
            }
        }

        /// <summary>
        /// Size of the rectangle in the Y dimension.
        /// </summary>
        public float SizeY
        {
            get
            {
                return _sizeY;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                if (value < 0)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _sizeY = value;
            }
        }

        /// <summary>
        /// Size of the rectangle in the Z dimension.
        /// </summary>
        public float SizeZ
        {
            get
            {
                return _sizeZ;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                if (value < 0)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _sizeZ = value;
            }
        }

        /// <summary>
        /// Value of the X coordinate of the rectangle.
        /// </summary>
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _x = value;
            }
        }

        /// <summary>
        /// Value of the Y coordinate of the rectangle.
        /// </summary>
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _y = value;
            }
        }

        /// <summary>
        /// Value of the Z coordinate of the rectangle.
        /// </summary>
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException("Size不能小于0");
                }

                _z = value;
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Returns true if the point is within the rectangle, inclusive of the edges.
        /// Returns false otherwise.
        /// </summary>
        /// <param name="point">The point which is being tested.</param>
        /// <returns>True if the point is within the rectangle. False otherwise</returns>
        public bool Contains(Point3D point)
        {
            return Contains(point._x, point._y, point._z);
        }

        /// <summary>
        /// Contains - Returns true if the Point represented by x,y,z is within the rectangle 
        /// inclusive of the edges. Returns false otherwise.
        /// </summary>
        /// <param name="x">X coordinate of the point which is being tested.</param>
        /// <param name="y">Y coordinate of the point which is being tested.</param>
        /// <param name="z">Y coordinate of the point which is being tested.</param>
        /// <returns> True if the Point represented by x,y is within the rectangle.
        /// False otherwise. </returns>
        public bool Contains(float x, float y, float z)
        {
            if (IsEmpty)
            {
                return false;
            }

            return ContainsInternal(x, y, z);
        }

        /// <summary>
        /// Returns true if the rectangle is non-Empty and is entirely contained within the
        /// rectangle, inclusive of the edges. Returns false otherwise.
        /// </summary>
        /// <param name="rect">Rectangle being tested.</param>
        /// <returns>Returns true if the rectangle is non-Empty and is entirely contained within the
        /// rectangle, inclusive of the edges. Returns false otherwise.</returns>
        public bool Contains(Rect3D rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (_x <= rect._x &&
                    _y <= rect._y &&
                    _z <= rect._z &&
                    _x + _sizeX >= rect._x + rect._sizeX &&
                    _y + _sizeY >= rect._y + rect._sizeY &&
                    _z + _sizeZ >= rect._z + rect._sizeZ);
        }

        /// <summary>
        /// Returns true if the rectangle intersects with this rectangle. 
        /// Returns false otherwise. Note that if one edge is coincident, this is considered 
        /// an intersection.
        /// </summary>
        /// <param name="rect">Rectangle being tested.</param>
        /// <returns>True if the rectangle intersects with this rectangle. 
        /// False otherwise.</returns>
        public bool IntersectsWith(Rect3D rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (rect._x <= (_x + _sizeX)) &&
                   ((rect._x + rect._sizeX) >= _x) &&
                   (rect._y <= (_y + _sizeY)) &&
                   ((rect._y + rect._sizeY) >= _y) &&
                   (rect._z <= (_z + _sizeZ)) &&
                   ((rect._z + rect._sizeZ) >= _z);
        }

        /// <summary>
        /// Intersect - Update this rectangle to be the intersection of this and rect
        /// If either this or rect are Empty, the result is Empty as well.
        /// </summary>
        /// <param name="rect"> The rect to intersect with this </param>
        public void Intersect(Rect3D rect)
        {
            if (IsEmpty || rect.IsEmpty || !this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                float x = Math.Max(_x, rect._x);
                float y = Math.Max(_y, rect._y);
                float z = Math.Max(_z, rect._z);
                _sizeX = Math.Min(_x + _sizeX, rect._x + rect._sizeX) - x;
                _sizeY = Math.Min(_y + _sizeY, rect._y + rect._sizeY) - y;
                _sizeZ = Math.Min(_z + _sizeZ, rect._z + rect._sizeZ) - z;

                _x = x;
                _y = y;
                _z = z;
            }
        }

        /// <summary>
        /// Return the result of the intersection of rect1 and rect2.
        /// If either this or rect are Empty, the result is Empty as well.
        /// </summary>
        /// <param name="rect1">First rectangle.</param>
        /// <param name="rect2">Second rectangle.</param>
        /// <returns>The result of the intersection of rect1 and rect2.</returns>
        public static Rect3D Intersect(Rect3D rect1, Rect3D rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        /// <summary>
        /// Update this rectangle to be the union of this and rect.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        public void Union(Rect3D rect)
        {
            if (IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                float x = Math.Min(_x, rect._x);
                float y = Math.Min(_y, rect._y);
                float z = Math.Min(_z, rect._z);
                _sizeX = Math.Max(_x + _sizeX, rect._x + rect._sizeX) - x;
                _sizeY = Math.Max(_y + _sizeY, rect._y + rect._sizeY) - y;
                _sizeZ = Math.Max(_z + _sizeZ, rect._z + rect._sizeZ) - z;
                _x = x;
                _y = y;
                _z = z;
            }
        }

        /// <summary>
        /// Return the result of the union of rect1 and rect2.
        /// </summary>
        /// <param name="rect1">First rectangle.</param>
        /// <param name="rect2">Second rectangle.</param>
        /// <returns>The result of the union of the two rectangles.</returns>
        public static Rect3D Union(Rect3D rect1, Rect3D rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        /// <summary>
        /// Update this rectangle to be the union of this and point.
        /// </summary>
        /// <param name="point">Point.</param>
        public void Union(Point3D point)
        {
            Union(new Rect3D(point, point));
        }

        /// <summary>
        /// Return the result of the union of rect and point.
        /// </summary>
        /// <param name="rect">Rectangle.</param>
        /// <param name="point">Point.</param>
        /// <returns>The result of the union of rect and point.</returns>
        public static Rect3D Union(Rect3D rect, Point3D point)
        {
            rect.Union(new Rect3D(point, point));
            return rect;
        }

        /// <summary>
        /// Translate the Location by the offset provided.
        /// If this is Empty, this method is illegal.
        /// </summary>
        /// <param name="offsetVector"></param>
        public void Offset(Vector3D offsetVector)
        {
            Offset(offsetVector._x, offsetVector._y, offsetVector._z);
        }

        /// <summary>
        /// Offset - translate the Location by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetZ"></param>
        public void Offset(float offsetX, float offsetY, float offsetZ)
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Size不能小于0");
            }

            _x += offsetX;
            _y += offsetY;
            _z += offsetZ;
        }

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="offsetVector"></param>
        /// <returns></returns>
        public static Rect3D Offset(Rect3D rect, Vector3D offsetVector)
        {
            rect.Offset(offsetVector._x, offsetVector._y, offsetVector._z);
            return rect;
        }

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetZ"></param>
        /// <returns></returns>
        public static Rect3D Offset(Rect3D rect, float offsetX, float offsetY, float offsetZ)
        {
            rect.Offset(offsetX, offsetY, offsetZ);
            return rect;
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal readonly static Rect3D Infinite = CreateInfiniteRect3D();

        #endregion Internal Fields
        

        /// <summary>
        /// ContainsInternal - Performs just the "point inside" logic.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>bool - true if the point is inside the rect</returns>
        private bool ContainsInternal(float x, float y, float z)
        {
            // We include points on the edge as "contained"
            return ((x >= _x) && (x <= _x + _sizeX) &&
                    (y >= _y) && (y <= _y + _sizeY) &&
                    (z >= _z) && (z <= _z + _sizeZ));
        }

        private static Rect3D CreateEmptyRect3D()
        {
            Rect3D empty = new Rect3D();
            empty._x = float.PositiveInfinity;
            empty._y = float.PositiveInfinity;
            empty._z = float.PositiveInfinity;
            // Can't use setters because they throw on negative values
            empty._sizeX = float.NegativeInfinity;
            empty._sizeY = float.NegativeInfinity;
            empty._sizeZ = float.NegativeInfinity;
            return empty;
        }

        private static Rect3D CreateInfiniteRect3D()
        {
            // NTRAID#Longhorn-1044943-2005/11/15-Microsoft - Robustness with infinities
            //
            //   Once the issue with Rect robustness with infinities is addressed we
            //   should change the values below to make this rectangle truely extend
            //   from -Infinite to +Infinity.
            //
            //   Until then we use a Rect from -float.MaxValue to +float.MaxValue.
            //   Because this rect is used only as a conservative bounding box for
            //   ScreenSpaceLines3D this span should be sufficient for the following
            //   reasons:
            //
            //     1.  Our meshes and transforms are reprensented in single precision
            //         at render time.  If it's not in this range it will not be
            //         rendered.
            //
            //     2.  SSLines3Ds are constructed as simple quads at render time.
            //         We will hit the guard band on the GPU at a limit far less than
            //         +/- float.MaxValue.
            //
            //     3.  We do our managed math in float precision so this still
            //         leaves us ample space to account for transforms, etc.
            //

            Rect3D infinite = new Rect3D();
            infinite._x = -float.MaxValue;
            infinite._y = -float.MaxValue;
            infinite._z = -float.MaxValue;
            infinite._sizeX = float.MaxValue * 2f;
            infinite._sizeY = float.MaxValue * 2f;
            infinite._sizeZ = float.MaxValue * 2f;
            return infinite;
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly static Rect3D s_empty = CreateEmptyRect3D();

        #endregion Private Fields
    }
}
