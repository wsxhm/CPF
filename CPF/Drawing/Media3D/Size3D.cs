using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing.Media3D
{
    /// <summary>
    /// Size3D - A value type which defined a size in terms of non-negative width, 
    /// length, and height.
    /// </summary>
    public partial struct Size3D
    {
        #region Constructors

        /// <summary>
        /// Constructor which sets the size's initial values.  Values must be non-negative.
        /// </summary>
        /// <param name="x">X dimension of the new size.</param>
        /// <param name="y">Y dimension of the new size.</param>
        /// <param name="z">Z dimension of the new size.</param>
        public Size3D(float x, float y, float z)
        {
            if (x < 0 || y < 0 || z < 0)
            {
                throw new System.ArgumentException("不能小于0");
            }


            _x = x;
            _y = y;
            _z = z;
        }

        #endregion Constructors

        #region Statics

        /// <summary>
        /// Empty - a static property which provides an Empty size.  X, Y, and Z are 
        /// negative-infinity.  This is the only situation
        /// where size can be negative.
        /// </summary>
        public static Size3D Empty
        {
            get
            {
                return s_empty;
            }
        }

        #endregion Statics

        #region Public Methods and Properties

        /// <summary>
        /// IsEmpty - this returns true if this size is the Empty size.
        /// Note: If size is 0 this Size3D still contains a 0, 1, or 2 dimensional set
        /// of points, so this method should not be used to check for 0 volume.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _x < 0;
            }
        }

        /// <summary>
        /// Size in X dimension. Default is 0, must be non-negative.
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
                    throw new System.InvalidOperationException("值无效！");
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("不能小于0");
                }

                _x = value;
            }
        }

        /// <summary>
        /// Size in Y dimension. Default is 0, must be non-negative.
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
                    throw new System.InvalidOperationException("值无效！");
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("不能小于0");
                }

                _y = value;
            }
        }


        /// <summary>
        /// Size in Z dimension. Default is 0, must be non-negative.
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
                    throw new System.InvalidOperationException("值无效！");
                }

                if (value < 0)
                {
                    throw new System.ArgumentException("不能小于0");
                }

                _z = value;
            }
        }

        #endregion Public Methods

        #region Public Operators

        /// <summary>
        /// Explicit conversion to Vector.
        /// </summary>
        /// <param name="size">The size to convert to a vector.</param>
        /// <returns>A vector equal to this size.</returns>
        public static explicit operator Vector3D(Size3D size)
        {
            return new Vector3D(size._x, size._y, size._z);
        }

        /// <summary>
        /// Explicit conversion to point.
        /// </summary>
        /// <param name="size">The size to convert to a point.</param>
        /// <returns>A point equal to this size.</returns>
        public static explicit operator Point3D(Size3D size)
        {
            return new Point3D(size._x, size._y, size._z);
        }

        #endregion Public Operators

        #region Private Methods

        private static Size3D CreateEmptySize3D()
        {
            Size3D empty = new Size3D();
            // Can't use setters because they throw on negative values
            empty._x = float.NegativeInfinity;
            empty._y = float.NegativeInfinity;
            empty._z = float.NegativeInfinity;
            return empty;
        }

        #endregion Private Methods

        #region Private Fields

        private readonly static Size3D s_empty = CreateEmptySize3D();

        #endregion Private Fields

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods




        /// <summary>
        /// Compares two Size3D instances for exact equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Size3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='size1'>The first Size3D to compare</param>
        /// <param name='size2'>The second Size3D to compare</param>
        public static bool operator ==(Size3D size1, Size3D size2)
        {
            return size1.X == size2.X &&
                   size1.Y == size2.Y &&
                   size1.Z == size2.Z;
        }

        /// <summary>
        /// Compares two Size3D instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Size3D instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='size1'>The first Size3D to compare</param>
        /// <param name='size2'>The second Size3D to compare</param>
        public static bool operator !=(Size3D size1, Size3D size2)
        {
            return !(size1 == size2);
        }
        /// <summary>
        /// Compares two Size3D instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Size3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='size1'>The first Size3D to compare</param>
        /// <param name='size2'>The second Size3D to compare</param>
        public static bool Equals(Size3D size1, Size3D size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            else
            {
                return size1.X.Equals(size2.X) &&
                       size1.Y.Equals(size2.Y) &&
                       size1.Z.Equals(size2.Z);
            }
        }

        /// <summary>
        /// Equals - compares this Size3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Size3D and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Size3D))
            {
                return false;
            }

            Size3D value = (Size3D)o;
            return Size3D.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Size3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Size3D to compare to "this"</param>
        public bool Equals(Size3D value)
        {
            return Size3D.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Size3D
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Size3D
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
                       Z.GetHashCode();
            }
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with Size3D data </param>
        ///// </summary>
        //public static Size3D Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Size3D value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Empty")
        //    {
        //        value = Empty;
        //    }
        //    else
        //    {
        //        value = new Size3D(
        //            Convert.ToDouble(firstToken, formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider));
        //    }

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------




        #region Public Properties



        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods





        #endregion ProtectedMethods

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods









        #endregion Internal Methods

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
            //return ConvertToString(null /* format string */, null /* format provider */);
            return string.Format("X:{0},Y:{1},Z:{2}", X, Y, Z);
        }

        ///// <summary>
        ///// Creates a string representation of this object based on the IFormatProvider
        ///// passed in.  If the provider is null, the CurrentCulture is used.
        ///// </summary>
        ///// <returns>
        ///// A string representation of this object.
        ///// </returns>
        //public string ToString(IFormatProvider provider)
        //{

        //    // Delegate to the internal method which implements all ToString calls.
        //    return ConvertToString(null /* format string */, provider);
        //}

        ///// <summary>
        ///// Creates a string representation of this object based on the format string
        ///// and IFormatProvider passed in.
        ///// If the provider is null, the CurrentCulture is used.
        ///// See the documentation for IFormattable for more information.
        ///// </summary>
        ///// <returns>
        ///// A string representation of this object.
        ///// </returns>
        //string IFormattable.ToString(string format, IFormatProvider provider)
        //{

        //    // Delegate to the internal method which implements all ToString calls.
        //    return ConvertToString(format, provider);
        //}

        ///// <summary>
        ///// Creates a string representation of this object based on the format string
        ///// and IFormatProvider passed in.
        ///// If the provider is null, the CurrentCulture is used.
        ///// See the documentation for IFormattable for more information.
        ///// </summary>
        ///// <returns>
        ///// A string representation of this object.
        ///// </returns>
        //internal string ConvertToString(string format, IFormatProvider provider)
        //{
        //    if (IsEmpty)
        //    {
        //        return "Empty";
        //    }

        //    // Helper to get the numeric list separator for a given culture.
        //    char separator = MS.Internal.TokenizerHelper.GetNumericListSeparator(provider);
        //    return String.Format(provider,
        //                         "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}",
        //                         separator,
        //                         _x,
        //                         _y,
        //                         _z);
        //}



        #endregion Internal Properties

        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties



        #endregion Dependency Properties

        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields


        internal float _x;
        internal float _y;
        internal float _z;




        #endregion Internal Fields
    }
}
