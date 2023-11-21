using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CPF.Drawing.Media3D
{
    /// <summary>
    /// Quaternions.
    /// Quaternions are distinctly 3D entities that represent rotation in three dimensions.
    /// Their power comes in being able to interpolate (and thus animate) between 
    /// quaternions to achieve a smooth, reliable interpolation.
    /// The default quaternion is the identity.
    /// 四元数。
    /// 四元数是明显 3D 实体表示在三维空间中的旋转。
    /// 他们的力量是能够之间内插 （并因此进行动画处理）
    /// 四元数来实现一种平稳、 可靠的插值方法。
    /// 默认四元数是恒等变换
    /// </summary>
    public struct Quaternion
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor that sets quaternion's initial values.
        /// </summary>
        /// <param name="x">Value of the X coordinate of the new quaternion.</param>
        /// <param name="y">Value of the Y coordinate of the new quaternion.</param>
        /// <param name="z">Value of the Z coordinate of the new quaternion.</param>
        /// <param name="w">Value of the W coordinate of the new quaternion.</param>
        public Quaternion(float x, float y, float z, float w)
        {
            _x = x;
            _y = y;
            _z = z;
            _w = w;
            _isNotDistinguishedIdentity = true;
        }

        /// <summary>
        /// Constructs a quaternion via specified axis of rotation and an angle.
        /// Throws an InvalidOperationException if given (0,0,0) as axis vector.
        /// </summary>
        /// <param name="axisOfRotation">Vector representing axis of rotation.</param>
        /// <param name="angleInDegrees">Angle to turn around the given axis (in degrees).</param>
        public Quaternion(Vector3D axisOfRotation, float angleInDegrees)
        {
            angleInDegrees %= 360f; // Doing the modulo before converting to radians reduces total error
            float angleInRadians = (float)(angleInDegrees * (Math.PI / 180.0));
            float length = axisOfRotation.Length;
            if (length == 0)
                throw new System.InvalidOperationException("length为0");
            Vector3D v = (axisOfRotation / length) * (float)Math.Sin(0.5 * angleInRadians);
            _x = v.X;
            _y = v.Y;
            _z = v.Z;
            _w = (float)Math.Cos(0.5 * angleInRadians);
            _isNotDistinguishedIdentity = true;
        }

        #endregion Constructors


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods
        /// <summary>
        ///     Identity quaternion
        /// </summary>
        public static Quaternion Identity
        {
            get
            {
                return s_identity;
            }
        }

        /// <summary>
        /// Retrieves quaternion's axis.
        /// </summary>
        public Vector3D Axis
        {
            // q = M [cos(Q/2), sin(Q /2)v]
            // axis = sin(Q/2)v
            // angle = cos(Q/2)
            // M is magnitude
            get
            {
                // Handle identity (where axis is indeterminate) by
                // returning arbitrary axis.
                if (IsDistinguishedIdentity || (_x == 0 && _y == 0 && _z == 0))
                {
                    return new Vector3D(0, 1, 0);
                }
                else
                {
                    Vector3D v = new Vector3D(_x, _y, _z);
                    v.Normalize();
                    return v;
                }
            }
        }

        /// <summary>
        /// Retrieves quaternion's angle.
        /// </summary>
        public float Angle
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 0;
                }

                // Magnitude of quaternion times sine and cosine
                float msin = (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
                float mcos = _w;

                if (!(msin <= Double.MaxValue))
                {
                    // Overflowed probably in squaring, so let's scale
                    // the values.  We don't need to include _w in the
                    // scale factor because we're not going to square
                    // it.
                    float maxcoeff = Math.Max(Math.Abs(_x), Math.Max(Math.Abs(_y), Math.Abs(_z)));
                    float x = _x / maxcoeff;
                    float y = _y / maxcoeff;
                    float z = _z / maxcoeff;
                    msin = (float)Math.Sqrt(x * x + y * y + z * z);
                    // Scale mcos too.
                    mcos = _w / maxcoeff;
                }

                // Atan2 is better than acos.  (More precise and more efficient.)
                return (float)(Math.Atan2(msin, mcos) * (360.0 / Math.PI));
            }
        }

        /// <summary>
        /// Returns whether the quaternion is normalized (i.e. has a magnitude of 1).
        /// </summary>
        public bool IsNormalized
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return true;
                }
                float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
                return DoubleUtil.IsOne(norm2);
            }
        }

        /// <summary>
        /// Tests whether or not a given quaternion is an identity quaternion.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return IsDistinguishedIdentity || (_x == 0 && _y == 0 && _z == 0 && _w == 1);
            }
        }

        /// <summary>
        /// Relaces quaternion with its conjugate
        /// </summary>
        public void Conjugate()
        {
            if (IsDistinguishedIdentity)
            {
                return;
            }

            // Conjugate([x,y,z,w]) = [-x,-y,-z,w]
            _x = -_x;
            _y = -_y;
            _z = -_z;
        }

        /// <summary>
        /// Replaces quaternion with its inverse
        /// </summary>
        public void Invert()
        {
            if (IsDistinguishedIdentity)
            {
                return;
            }

            // Inverse = Conjugate / Norm Squared
            Conjugate();
            float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            _x /= norm2;
            _y /= norm2;
            _z /= norm2;
            _w /= norm2;
        }

        /// <summary>
        /// Normalizes this quaternion.
        /// </summary>
        public void Normalize()
        {
            if (IsDistinguishedIdentity)
            {
                return;
            }

            float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            if (norm2 > Double.MaxValue)
            {
                // Handle overflow in computation of norm2
                float rmax = 1f / Max(Math.Abs(_x),
                                      Math.Abs(_y),
                                      Math.Abs(_z),
                                      Math.Abs(_w));

                _x *= rmax;
                _y *= rmax;
                _z *= rmax;
                _w *= rmax;
                norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            }
            float normInverse = (float)(1.0 / Math.Sqrt(norm2));
            _x *= normInverse;
            _y *= normInverse;
            _z *= normInverse;
            _w *= normInverse;
        }

        /// <summary>
        /// Quaternion addition.
        /// </summary>
        /// <param name="left">First quaternion being added.</param>
        /// <param name="right">Second quaternion being added.</param>
        /// <returns>Result of addition.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new Quaternion(0, 0, 0, 2);
                }
                else
                {
                    // We know left is not distinguished identity here.                    
                    left._w += 1;
                    return left;
                }
            }
            else if (left.IsDistinguishedIdentity)
            {
                // We know right is not distinguished identity here.
                right._w += 1;
                return right;
            }
            else
            {
                return new Quaternion(left._x + right._x,
                                      left._y + right._y,
                                      left._z + right._z,
                                      left._w + right._w);
            }
        }

        /// <summary>
        /// Quaternion addition.
        /// </summary>
        /// <param name="left">First quaternion being added.</param>
        /// <param name="right">Second quaternion being added.</param>
        /// <returns>Result of addition.</returns>
        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return (left + right);
        }

        /// <summary>
        /// Quaternion subtraction.
        /// </summary>
        /// <param name="left">Quaternion to subtract from.</param>
        /// <param name="right">Quaternion to subtract from the first quaternion.</param>
        /// <returns>Result of subtraction.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            if (right.IsDistinguishedIdentity)
            {
                if (left.IsDistinguishedIdentity)
                {
                    return new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    // We know left is not distinguished identity here.
                    left._w -= 1;
                    return left;
                }
            }
            else if (left.IsDistinguishedIdentity)
            {
                // We know right is not distinguished identity here.
                return new Quaternion(-right._x, -right._y, -right._z, 1 - right._w);
            }
            else
            {
                return new Quaternion(left._x - right._x,
                                      left._y - right._y,
                                      left._z - right._z,
                                      left._w - right._w);
            }
        }

        /// <summary>
        /// Quaternion subtraction.
        /// </summary>
        /// <param name="left">Quaternion to subtract from.</param>
        /// <param name="right">Quaternion to subtract from the first quaternion.</param>
        /// <returns>Result of subtraction.</returns>
        public static Quaternion Subtract(Quaternion left, Quaternion right)
        {
            return (left - right);
        }

        /// <summary>
        /// Quaternion multiplication.
        /// </summary>
        /// <param name="left">First quaternion.</param>
        /// <param name="right">Second quaternion.</param>
        /// <returns>Result of multiplication.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            if (left.IsDistinguishedIdentity)
            {
                return right;
            }
            if (right.IsDistinguishedIdentity)
            {
                return left;
            }

            float x = left._w * right._x + left._x * right._w + left._y * right._z - left._z * right._y;
            float y = left._w * right._y + left._y * right._w + left._z * right._x - left._x * right._z;
            float z = left._w * right._z + left._z * right._w + left._x * right._y - left._y * right._x;
            float w = left._w * right._w - left._x * right._x - left._y * right._y - left._z * right._z;
            Quaternion result = new Quaternion(x, y, z, w);
            return result;

        }

        /// <summary>
        /// Quaternion multiplication.
        /// </summary>
        /// <param name="left">First quaternion.</param>
        /// <param name="right">Second quaternion.</param>
        /// <returns>Result of multiplication.</returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            return left * right;
        }

        /// <summary>
        /// Scale this quaternion by a scalar.
        /// </summary>
        /// <param name="scale">Value to scale by.</param>            
        private void Scale(float scale)
        {
            if (IsDistinguishedIdentity)
            {
                _w = scale;
                IsDistinguishedIdentity = false;
                return;
            }
            _x *= scale;
            _y *= scale;
            _z *= scale;
            _w *= scale;
        }

        /// <summary>
        /// Return length of quaternion.
        /// </summary>
        private float Length()
        {
            if (IsDistinguishedIdentity)
            {
                return 1;
            }

            float norm2 = _x * _x + _y * _y + _z * _z + _w * _w;
            if (!(norm2 <= Double.MaxValue))
            {
                // Do this the slow way to avoid squaring large
                // numbers since the length of many quaternions is
                // representable even if the squared length isn't.  Of
                // course some lengths aren't representable because
                // the length can be up to twice as big as the largest
                // coefficient.

                float max = Math.Max(Math.Max(Math.Abs(_x), Math.Abs(_y)),
                                      Math.Max(Math.Abs(_z), Math.Abs(_w)));

                float x = _x / max;
                float y = _y / max;
                float z = _z / max;
                float w = _w / max;

                float smallLength = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
                // Return length of this smaller vector times the scale we applied originally.
                return smallLength * max;
            }
            return (float)Math.Sqrt(norm2);
        }

        /// <summary>
        /// Smoothly interpolate between the two given quaternions using Spherical 
        /// Linear Interpolation (SLERP).
        /// </summary>
        /// <param name="from">First quaternion for interpolation.</param>
        /// <param name="to">Second quaternion for interpolation.</param>
        /// <param name="t">Interpolation coefficient.</param>
        /// <returns>SLERP-interpolated quaternion between the two given quaternions.</returns>
        public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
        {
            return Slerp(from, to, t, /* useShortestPath = */ true);
        }

        /// <summary>
        /// Smoothly interpolate between the two given quaternions using Spherical 
        /// Linear Interpolation (SLERP).
        /// </summary>
        /// <param name="from">First quaternion for interpolation.</param>
        /// <param name="to">Second quaternion for interpolation.</param>
        /// <param name="t">Interpolation coefficient.</param>
        /// <param name="useShortestPath">If true, Slerp will automatically flip the sign of
        ///     the destination Quaternion to ensure the shortest path is taken.</param>
        /// <returns>SLERP-interpolated quaternion between the two given quaternions.</returns>
        public static Quaternion Slerp(Quaternion from, Quaternion to, float t, bool useShortestPath)
        {
            if (from.IsDistinguishedIdentity)
            {
                from._w = 1;
            }
            if (to.IsDistinguishedIdentity)
            {
                to._w = 1;
            }

            float cosOmega;
            float scaleFrom, scaleTo;

            // Normalize inputs and stash their lengths
            float lengthFrom = from.Length();
            float lengthTo = to.Length();
            from.Scale(1 / lengthFrom);
            to.Scale(1 / lengthTo);

            // Calculate cos of omega.
            cosOmega = from._x * to._x + from._y * to._y + from._z * to._z + from._w * to._w;

            if (useShortestPath)
            {
                // If we are taking the shortest path we flip the signs to ensure that
                // cosOmega will be positive.
                if (cosOmega < 0.0)
                {
                    cosOmega = -cosOmega;
                    to._x = -to._x;
                    to._y = -to._y;
                    to._z = -to._z;
                    to._w = -to._w;
                }
            }
            else
            {
                // If we are not taking the UseShortestPath we clamp cosOmega to
                // -1 to stay in the domain of Math.Acos below.
                if (cosOmega < -1f)
                {
                    cosOmega = -1f;
                }
            }

            // Clamp cosOmega to [-1,1] to stay in the domain of Math.Acos below.
            // The logic above has either flipped the sign of cosOmega to ensure it
            // is positive or clamped to -1 aready.  We only need to worry about the
            // upper limit here.
            if (cosOmega > 1.0)
            {
                cosOmega = 1f;
            }

            Debug.Assert(!(cosOmega < -1.0) && !(cosOmega > 1.0),
                "cosOmega should be clamped to [-1,1]");

            // The mainline algorithm doesn't work for extreme
            // cosine values.  For large cosine we have a better
            // fallback hence the asymmetric limits.
            const float maxCosine = (float)(1.0 - 1e-6);
            const float minCosine = (float)(1e-10 - 1.0);

            // Calculate scaling coefficients.
            if (cosOmega > maxCosine)
            {
                // Quaternions are too close - use linear interpolation.
                scaleFrom = 1f - t;
                scaleTo = t;
            }
            else if (cosOmega < minCosine)
            {
                // Quaternions are nearly opposite, so we will pretend to 
                // is exactly -from.
                // First assign arbitrary perpendicular to "to".
                to = new Quaternion(-from.Y, from.X, -from.W, from.Z);

                float theta = (float)(t * Math.PI);

                scaleFrom = (float)Math.Cos(theta);
                scaleTo = (float)Math.Sin(theta);
            }
            else
            {
                // Standard case - use SLERP interpolation.
                float omega = (float)Math.Acos(cosOmega);
                float sinOmega = (float)Math.Sqrt(1.0 - cosOmega * cosOmega);
                scaleFrom = (float)Math.Sin((1.0 - t) * omega) / sinOmega;
                scaleTo = (float)Math.Sin(t * omega) / sinOmega;
            }

            // We want the magnitude of the output quaternion to be
            // multiplicatively interpolated between the input
            // magnitudes, i.e. lengthOut = lengthFrom * (lengthTo/lengthFrom)^t
            //                            = lengthFrom ^ (1-t) * lengthTo ^ t

            float lengthOut = lengthFrom * (float)Math.Pow(lengthTo / lengthFrom, t);
            scaleFrom *= lengthOut;
            scaleTo *= lengthOut;

            return new Quaternion(scaleFrom * from._x + scaleTo * to._x,
                                  scaleFrom * from._y + scaleTo * to._y,
                                  scaleFrom * from._z + scaleTo * to._z,
                                  scaleFrom * from._w + scaleTo * to._w);
        }

        #endregion Public Methods

        #region Private Methods

        static private float Max(float a, float b, float c, float d)
        {
            if (b > a)
                a = b;
            if (c > a)
                a = c;
            if (d > a)
                a = d;
            return a;
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// X - Default value is 0.
        /// </summary>
        public float X
        {
            get
            {
                return _x;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _x = value;
            }
        }

        /// <summary>
        /// Y - Default value is 0.
        /// </summary>
        public float Y
        {
            get
            {
                return _y;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _y = value;
            }
        }

        /// <summary>
        /// Z - Default value is 0.
        /// </summary>
        public float Z
        {
            get
            {
                return _z;
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _z = value;
            }
        }

        /// <summary>
        /// W - Default value is 1.
        /// </summary>
        public float W
        {
            get
            {
                if (IsDistinguishedIdentity)
                {
                    return 1f;
                }
                else
                {
                    return _w;
                }
            }

            set
            {
                if (IsDistinguishedIdentity)
                {
                    this = s_identity;
                    IsDistinguishedIdentity = false;
                }
                _w = value;
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields

        internal float _x;
        internal float _y;
        internal float _z;
        internal float _w;

        #endregion Internal Fields

        #region Private Fields and Properties

        // If this bool is false then we are a default quaternion with
        // all doubles equal to zero, but should be treated as
        // identity.
        private bool _isNotDistinguishedIdentity;

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !_isNotDistinguishedIdentity;
            }
            set
            {
                _isNotDistinguishedIdentity = !value;
            }
        }

        private static int GetIdentityHashCode()
        {
            // This code is called only once.
            float zero = 0;
            float one = 1;
            // return zero.GetHashCode() ^ zero.GetHashCode() ^ zero.GetHashCode() ^ one.GetHashCode();
            // But this expression can be simplified because the first two hash codes cancel.
            return zero.GetHashCode() ^ one.GetHashCode();
        }

        private static Quaternion GetIdentity()
        {
            // This code is called only once.
            Quaternion q = new Quaternion(0, 0, 0, 1);
            q.IsDistinguishedIdentity = true;
            return q;
        }


        // Hash code for identity.
        private static int c_identityHashCode = GetIdentityHashCode();

        // Default identity
        private static Quaternion s_identity = GetIdentity();

        #endregion Private Fields and Properties

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods




        /// <summary>
        /// Compares two Quaternion instances for exact equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Quaternion instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='quaternion1'>The first Quaternion to compare</param>
        /// <param name='quaternion2'>The second Quaternion to compare</param>
        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2)
        {
            if (quaternion1.IsDistinguishedIdentity || quaternion2.IsDistinguishedIdentity)
            {
                return quaternion1.IsIdentity == quaternion2.IsIdentity;
            }
            else
            {
                return quaternion1.X == quaternion2.X &&
                       quaternion1.Y == quaternion2.Y &&
                       quaternion1.Z == quaternion2.Z &&
                       quaternion1.W == quaternion2.W;
            }
        }

        /// <summary>
        /// Compares two Quaternion instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Quaternion instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='quaternion1'>The first Quaternion to compare</param>
        /// <param name='quaternion2'>The second Quaternion to compare</param>
        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2)
        {
            return !(quaternion1 == quaternion2);
        }
        /// <summary>
        /// Compares two Quaternion instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Quaternion instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='quaternion1'>The first Quaternion to compare</param>
        /// <param name='quaternion2'>The second Quaternion to compare</param>
        public static bool Equals(Quaternion quaternion1, Quaternion quaternion2)
        {
            if (quaternion1.IsDistinguishedIdentity || quaternion2.IsDistinguishedIdentity)
            {
                return quaternion1.IsIdentity == quaternion2.IsIdentity;
            }
            else
            {
                return quaternion1.X.Equals(quaternion2.X) &&
                       quaternion1.Y.Equals(quaternion2.Y) &&
                       quaternion1.Z.Equals(quaternion2.Z) &&
                       quaternion1.W.Equals(quaternion2.W);
            }
        }

        /// <summary>
        /// Equals - compares this Quaternion with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Quaternion and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Quaternion))
            {
                return false;
            }

            Quaternion value = (Quaternion)o;
            return Quaternion.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Quaternion with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Quaternion to compare to "this"</param>
        public bool Equals(Quaternion value)
        {
            return Quaternion.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Quaternion
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Quaternion
        /// </returns>
        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
            {
                return c_identityHashCode;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return X.GetHashCode() ^
                       Y.GetHashCode() ^
                       Z.GetHashCode() ^
                       W.GetHashCode();
            }
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with Quaternion data </param>
        ///// </summary>
        //public static Quaternion Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Quaternion value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Identity")
        //    {
        //        value = Identity;
        //    }
        //    else
        //    {
        //        value = new Quaternion(
        //            Convert.ToDouble(firstToken, formatProvider),
        //            Convert.ToDouble(th.NextTokenRequired(), formatProvider),
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
            return string.Format("X:{0},Y:{1},Z:{2},W:{3}", X, Y, Z, W);
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
        //    if (IsIdentity)
        //    {
        //        return "Identity";
        //    }

        //    // Helper to get the numeric list separator for a given culture.
        //    char separator = MS.Internal.TokenizerHelper.GetNumericListSeparator(provider);
        //    return String.Format(provider,
        //                         "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}",
        //                         separator,
        //                         _x,
        //                         _y,
        //                         _z,
        //                         _w);
        //}



        #endregion Internal Properties
    }
}
