using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing.Media3D
{
    /// <summary>
    /// Vector3D - 3D vector representation.
    /// </summary>
    public struct Vector3D
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor that sets vector's initial values.
        /// </summary>
        /// <param name="x">Value of the X coordinate of the new vector.</param>
        /// <param name="y">Value of the Y coordinate of the new vector.</param>
        /// <param name="z">Value of the Z coordinate of the new vector.</param>
        public Vector3D(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        #endregion Constructors


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Length of the vector.
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
            }
        }

        /// <summary>
        /// Length of the vector squared.
        /// </summary>
        public float LengthSquared
        {
            get
            {
                return _x * _x + _y * _y + _z * _z;
            }
        }

        /// <summary>
        /// Updates the vector to maintain its direction, but to have a length
        /// of 1. Equivalent to dividing the vector by its Length.
        /// Returns NaN if length is zero.
        /// </summary>
        public void Normalize()
        {
            // Computation of length can overflow easily because it
            // first computes squared length, so we first divide by
            // the largest coefficient.
            float m = Math.Abs(_x);
            float absy = Math.Abs(_y);
            float absz = Math.Abs(_z);
            if (absy > m)
            {
                m = absy;
            }
            if (absz > m)
            {
                m = absz;
            }

            _x /= m;
            _y /= m;
            _z /= m;

            float length = (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
            this /= length;
        }

        /// <summary>
        /// Computes the angle between two vectors.
        /// </summary>
        /// <param name="vector1">First vector.</param>
        /// <param name="vector2">Second vector.</param>
        /// <returns>
        /// Returns the angle required to rotate vector1 into vector2 in degrees.
        /// This will return a value between [0, 180] degrees.
        /// (Note that this is slightly different from the Vector member
        /// function of the same name.  Signed angles do not extend to 3D.)
        /// </returns>
        public static float AngleBetween(Vector3D vector1, Vector3D vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

            float ratio = DotProduct(vector1, vector2);

            // The "straight forward" method of acos(u.v) has large precision
            // issues when the dot product is near +/-1.  This is due to the
            // steep slope of the acos function as we approach +/- 1.  Slight
            // precision errors in the dot product calculation cause large
            // variation in the output value.
            //
            //        |                   |
            //         \__                |
            //            ---___          |
            //                  ---___    |
            //                        ---_|_
            //                            | ---___
            //                            |       ---___
            //                            |             ---__
            //                            |                  \
            //                            |                   |
            //       -|-------------------+-------------------|-
            //       -1                   0                   1
            //
            //                         acos(x)
            //
            // To avoid this we use an alternative method which finds the
            // angle bisector by (u-v)/2:
            //
            //                            _>
            //                       u  _-  \ (u-v)/2
            //                        _-  __-v
            //                      _=__--      
            //                    .=----------->
            //                            v
            //
            // Because u and v and unit vectors, (u-v)/2 forms a right angle
            // with the angle bisector.  The hypotenuse is 1, therefore
            // 2*asin(|u-v|/2) gives us the angle between u and v.
            //
            // The largest possible value of |u-v| occurs with perpendicular
            // vectors and is sqrt(2)/2 which is well away from extreme slope
            // at +/-1.
            //
            // (See Windows OS 

            float theta;

            if (ratio < 0)
            {
                theta = (float)(Math.PI - 2.0 * Math.Asin((-vector1 - vector2).Length / 2.0));
            }
            else
            {
                theta = (float)(2.0 * Math.Asin((vector1 - vector2).Length / 2.0));
            }

            return M3DUtil.RadiansToDegrees(theta);
        }

        /// <summary>
        /// Operator -Vector (unary negation).
        /// </summary>
        /// <param name="vector">Vector being negated.</param>
        /// <returns>Negation of the given vector.</returns>
        public static Vector3D operator -(Vector3D vector)
        {
            return new Vector3D(-vector._x, -vector._y, -vector._z);
        }

        /// <summary>
        /// Negates the values of X, Y, and Z on this Vector3D
        /// </summary>
        public void Negate()
        {
            _x = -_x;
            _y = -_y;
            _z = -_z;
        }

        /// <summary>
        /// Vector addition.
        /// </summary>
        /// <param name="vector1">First vector being added.</param>
        /// <param name="vector2">Second vector being added.</param>
        /// <returns>Result of addition.</returns>
        public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        /// <summary>
        /// Vector addition.
        /// </summary>
        /// <param name="vector1">First vector being added.</param>
        /// <param name="vector2">Second vector being added.</param>
        /// <returns>Result of addition.</returns>
        public static Vector3D Add(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x + vector2._x,
                                vector1._y + vector2._y,
                                vector1._z + vector2._z);
        }

        /// <summary>
        /// Vector subtraction.
        /// </summary>
        /// <param name="vector1">Vector that is subtracted from.</param>
        /// <param name="vector2">Vector being subtracted.</param>
        /// <returns>Result of subtraction.</returns>
        public static Vector3D operator -(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        /// <summary>
        /// Vector subtraction.
        /// </summary>
        /// <param name="vector1">Vector that is subtracted from.</param>
        /// <param name="vector2">Vector being subtracted.</param>
        /// <returns>Result of subtraction.</returns>
        public static Vector3D Subtract(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1._x - vector2._x,
                                vector1._y - vector2._y,
                                vector1._z - vector2._z);
        }

        /// <summary>
        /// Vector3D + Point3D addition.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of addition.</returns>
        public static Point3D operator +(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x + point._x,
                               vector._y + point._y,
                               vector._z + point._z);
        }

        /// <summary>
        /// Vector3D + Point3D addition.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of addition.</returns>
        public static Point3D Add(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x + point._x,
                               vector._y + point._y,
                               vector._z + point._z);
        }

        /// <summary>
        /// Vector3D - Point3D subtraction.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of subtraction.</returns>
        public static Point3D operator -(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x - point._x,
                               vector._y - point._y,
                               vector._z - point._z);
        }

        /// <summary>
        /// Vector3D - Point3D subtraction.
        /// </summary>
        /// <param name="vector">Vector by which we offset the point.</param>
        /// <param name="point">Point being offset by the given vector.</param>
        /// <returns>Result of subtraction.</returns>
        public static Point3D Subtract(Vector3D vector, Point3D point)
        {
            return new Point3D(vector._x - point._x,
                               vector._y - point._y,
                               vector._z - point._z);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="vector">Vector being multiplied.</param>
        /// <param name="scalar">Scalar value by which the vector is multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3D operator *(Vector3D vector, float scalar)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="vector">Vector being multiplied.</param>
        /// <param name="scalar">Scalar value by which the vector is multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3D Multiply(Vector3D vector, float scalar)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="scalar">Scalar value by which the vector is multiplied</param>
        /// <param name="vector">Vector being multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3D operator *(float scalar, Vector3D vector)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="scalar">Scalar value by which the vector is multiplied</param>
        /// <param name="vector">Vector being multiplied.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3D Multiply(float scalar, Vector3D vector)
        {
            return new Vector3D(vector._x * scalar,
                                vector._y * scalar,
                                vector._z * scalar);
        }

        /// <summary>
        /// Scalar division.
        /// </summary>
        /// <param name="vector">Vector being divided.</param>
        /// <param name="scalar">Scalar value by which we divide the vector.</param>
        /// <returns>Result of division.</returns>
        public static Vector3D operator /(Vector3D vector, float scalar)
        {
            return vector * (1f / scalar);
        }

        /// <summary>
        /// Scalar division.
        /// </summary>
        /// <param name="vector">Vector being divided.</param>
        /// <param name="scalar">Scalar value by which we divide the vector.</param>
        /// <returns>Result of division.</returns>
        public static Vector3D Divide(Vector3D vector, float scalar)
        {
            return vector * (1f / scalar);
        }

        /// <summary>
        /// Vector3D * Matrix3D multiplication
        /// </summary>
        /// <param name="vector">Vector being tranformed.</param>
        /// <param name="matrix">Transformation matrix applied to the vector.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3D operator *(Vector3D vector, Matrix3D matrix)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Vector3D * Matrix3D multiplication
        /// </summary>
        /// <param name="vector">Vector being tranformed.</param>
        /// <param name="matrix">Transformation matrix applied to the vector.</param>
        /// <returns>Result of multiplication.</returns>
        public static Vector3D Multiply(Vector3D vector, Matrix3D matrix)
        {
            return matrix.Transform(vector);
        }

        /// <summary>
        /// Vector dot product.
        /// </summary>
        /// <param name="vector1">First vector.</param>
        /// <param name="vector2">Second vector.</param>
        /// <returns>Dot product of two vectors.</returns>
        public static float DotProduct(Vector3D vector1, Vector3D vector2)
        {
            return DotProduct(ref vector1, ref vector2);
        }

        /// <summary>
        /// Faster internal version of DotProduct that avoids copies
        ///
        /// vector1 and vector2 to a passed by ref for perf and ARE NOT MODIFIED
        /// </summary>
        internal static float DotProduct(ref Vector3D vector1, ref Vector3D vector2)
        {
            return vector1._x * vector2._x +
                   vector1._y * vector2._y +
                   vector1._z * vector2._z;
        }

        /// <summary>
        /// Vector cross product.
        /// </summary>
        /// <param name="vector1">First vector.</param>
        /// <param name="vector2">Second vector.</param>
        /// <returns>Cross product of two vectors.</returns>
        public static Vector3D CrossProduct(Vector3D vector1, Vector3D vector2)
        {
            Vector3D result;
            CrossProduct(ref vector1, ref vector2, out result);
            return result;
        }

        /// <summary>
        /// Faster internal version of CrossProduct that avoids copies
        ///
        /// vector1 and vector2 to a passed by ref for perf and ARE NOT MODIFIED
        /// </summary>
        internal static void CrossProduct(ref Vector3D vector1, ref Vector3D vector2, out Vector3D result)
        {
            result._x = vector1._y * vector2._z - vector1._z * vector2._y;
            result._y = vector1._z * vector2._x - vector1._x * vector2._z;
            result._z = vector1._x * vector2._y - vector1._y * vector2._x;
        }

        /// <summary>
        /// Vector3D to Point3D conversion.
        /// </summary>
        /// <param name="vector">Vector being converted.</param>
        /// <returns>Point representing the given vector.</returns>
        public static explicit operator Point3D(Vector3D vector)
        {
            return new Point3D(vector._x, vector._y, vector._z);
        }

        /// <summary>
        /// Explicit conversion to Size3D.  Note that since Size3D cannot contain negative values,
        /// the resulting size will contains the absolute values of X, Y, and Z.
        /// </summary>
        /// <param name="vector">The vector to convert to a size.</param>
        /// <returns>A size equal to this vector.</returns>
        public static explicit operator Size3D(Vector3D vector)
        {
            return new Size3D(Math.Abs(vector._x), Math.Abs(vector._y), Math.Abs(vector._z));
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods




        /// <summary>
        /// Compares two Vector3D instances for exact equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector3D to compare</param>
        /// <param name='vector2'>The second Vector3D to compare</param>
        public static bool operator ==(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y &&
                   vector1.Z == vector2.Z;
        }

        /// <summary>
        /// Compares two Vector3D instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector3D instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector3D to compare</param>
        /// <param name='vector2'>The second Vector3D to compare</param>
        public static bool operator !=(Vector3D vector1, Vector3D vector2)
        {
            return !(vector1 == vector2);
        }
        /// <summary>
        /// Compares two Vector3D instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector3D instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector3D to compare</param>
        /// <param name='vector2'>The second Vector3D to compare</param>
        public static bool Equals(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y) &&
                   vector1.Z.Equals(vector2.Z);
        }

        /// <summary>
        /// Equals - compares this Vector3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Vector3D and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Vector3D))
            {
                return false;
            }

            Vector3D value = (Vector3D)o;
            return Vector3D.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Vector3D with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Vector3D to compare to "this"</param>
        public bool Equals(Vector3D value)
        {
            return Vector3D.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Vector3D
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Vector3D
        /// </returns>
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode() ^
                   Z.GetHashCode();
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with Vector3D data </param>
        ///// </summary>
        //public static Vector3D Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Vector3D value;

        //    String firstToken = th.NextTokenRequired();

        //    value = new Vector3D(
        //        Convert.ToDouble(firstToken, formatProvider),
        //        Convert.ToDouble(th.NextTokenRequired(), formatProvider),
        //        Convert.ToDouble(th.NextTokenRequired(), formatProvider));

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

        /// <summary>
        ///     X - float.  Default value is 0.
        /// </summary>
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
        ///     Y - float.  Default value is 0.
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

        /// <summary>
        ///     Z - float.  Default value is 0.
        /// </summary>
        public float Z
        {
            get
            {
                return _z;
            }

            set
            {
                _z = value;
            }

        }

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

            //// Delegate to the internal method which implements all ToString calls.
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
