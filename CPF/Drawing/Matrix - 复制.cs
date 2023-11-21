using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CPF.Drawing
{
    /// <summary>
    /// 3*3 2D变换矩阵，一般不要直接用无参数构造函数，直接用Matrix.Identity
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(MatrixConverter))]
    public struct Matrix : IFormattable
    {
        // the transform is identity by default
        // Actually fill in the fields - some (internal) code uses the fields directly for perf.
        private static Matrix s_identity = CreateIdentity();

        #region Constructor

        /// <summary>
        /// Creates a matrix of the form
        ///             / m11 scalex, m12 skewx,  0 p0 \
        ///             | m21 skewy,  m22 scaley, 0 p1 |
        ///             \ m31 offsetX,m32 offsetY,1 p2 /
        /// </summary>
        public Matrix(float m11, float m12,
                      float m21, float m22,
                      float offsetX, float offsetY)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            _padding = 0;

            p2 = 1;
            p1 = 0;
            p0 = 0;
            // We will detect EXACT identity, scale, translation or
            // scale+translation and use special case algorithms.
            DeriveMatrixType();
        }

        #endregion Constructor

        #region Identity

        /// <summary>
        /// Identity
        /// </summary>
        public static Matrix Identity
        {
            get
            {
                return s_identity;
            }
        }

        ///// <summary>
        ///// Sets the matrix to identity.
        ///// </summary>
        //public void SetIdentity()
        //{
        //    _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
        //}

        /// <summary>
        /// Tests whether or not a given transform is an identity transform
        /// 测试给定的变换是否是恒等变换
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return (_type == MatrixTypes.TRANSFORM_IS_IDENTITY ||
                        (_m11 == 1 && _m12 == 0 && _m21 == 0 && _m22 == 1 && _offsetX == 0 && _offsetY == 0 && Persp2 == 1 && Persp1 == 0 && Persp0 == 0));
            }
        }

        #endregion Identity

        #region Operators
        /// <summary>
        /// Multiplies two transformations.
        /// </summary>
        public static Matrix operator *(Matrix trans1, Matrix trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            //trans1.Debug_CheckType();
            return trans1;
        }

        /// <summary>
        /// Multiply
        /// </summary>
        public static Matrix Multiply(Matrix trans1, Matrix trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            //trans1.Debug_CheckType();
            return trans1;
        }

        #endregion Operators

        #region Combine Methods

        /// <summary>
        /// Append - "this" becomes this * matrix, the same as this *= matrix.
        /// </summary>
        /// <param name="matrix"> The Matrix to append to this Matrix </param>
        public void Append(Matrix matrix)
        {
            this *= matrix;
        }

        /// <summary>
        /// Prepend - "this" becomes matrix * this, the same as this = matrix * this.
        /// </summary>
        /// <param name="matrix"> The Matrix to prepend to this Matrix </param>
        public void Prepend(Matrix matrix)
        {
            this = matrix * this;
        }

        /// <summary>
        /// Rotates this matrix about the origin
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in degrees</param>
        public void Rotate(float angle)
        {
            angle %= 360f; // Doing the modulo before converting to radians reduces total error
            this *= CreateRotationRadians((float)(angle * (Math.PI / 180.0)));
        }

        /// <summary>
        /// Prepends a rotation about the origin to "this"
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in degrees</param>
        public void RotatePrepend(float angle)
        {
            angle %= 360f; // Doing the modulo before converting to radians reduces total error
            this = CreateRotationRadians((float)(angle * (Math.PI / 180.0))) * this;
        }

        /// <summary>
        /// Rotates this matrix about the given point
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in degrees</param>
        /// <param name='centerX'>The centerX of rotation</param>
        /// <param name='centerY'>The centerY of rotation</param>
        public void RotateAt(float angle, float centerX, float centerY)
        {
            angle %= 360f; // Doing the modulo before converting to radians reduces total error
            this *= CreateRotationRadians((float)(angle * (Math.PI / 180.0)), centerX, centerY);
        }

        /// <summary>
        /// Prepends a rotation about the given point to "this"
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in degrees</param>
        /// <param name='centerX'>The centerX of rotation</param>
        /// <param name='centerY'>The centerY of rotation</param>
        public void RotateAtPrepend(float angle, float centerX, float centerY)
        {
            angle %= 360f; // Doing the modulo before converting to radians reduces total error
            this = CreateRotationRadians((float)(angle * (Math.PI / 180.0)), centerX, centerY) * this;
        }

        /// <summary>
        /// Scales this matrix around the origin
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        public void Scale(float scaleX, float scaleY)
        {
            this *= CreateScaling(scaleX, scaleY);
        }

        /// <summary>
        /// Prepends a scale around the origin to "this"
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        public void ScalePrepend(float scaleX, float scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        /// <summary>
        /// Scales this matrix around the center provided
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        /// <param name="centerX">The centerX about which to scale</param>
        /// <param name="centerY">The centerY about which to scale</param>
        public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
        {
            this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        /// <summary>
        /// Prepends a scale around the center provided to "this"
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        /// <param name="centerX">The centerX about which to scale</param>
        /// <param name="centerY">The centerY about which to scale</param>
        public void ScaleAtPrepend(float scaleX, float scaleY, float centerX, float centerY)
        {
            this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
        }

        /// <summary>
        /// Skews this matrix
        /// </summary>
        /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
        /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
        public void Skew(float skewX, float skewY)
        {
            skewX %= 360;
            skewY %= 360;
            this *= CreateSkewRadians((float)(skewX * (Math.PI / 180.0)),
                                      (float)(skewY * (Math.PI / 180.0)));
        }

        /// <summary>
        /// Prepends a skew to this matrix
        /// </summary>
        /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
        /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
        public void SkewPrepend(float skewX, float skewY)
        {
            skewX %= 360;
            skewY %= 360;
            this = CreateSkewRadians((float)(skewX * (Math.PI / 180.0)),
                                     (float)(skewY * (Math.PI / 180.0))) * this;
        }

        /// <summary>
        /// Translates this matrix
        /// </summary>
        /// <param name='offsetX'>The offset in the x dimension</param>
        /// <param name='offsetY'>The offset in the y dimension</param>
        public void Translate(float offsetX, float offsetY)
        {
            //
            // / a b 0 \   / 1 0 0 \    / a      b       0 \
            // | c d 0 | * | 0 1 0 | = |  c      d       0 |
            // \ e f 1 /   \ x y 1 /    \ e+x    f+y     1 /
            //
            // (where e = _offsetX and f == _offsetY)
            //

            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                // Values would be incorrect if matrix was created using default constructor.
                // or if SetIdentity was called on a matrix which had values.
                //
                SetMatrix(1, 0,
                          0, 1,
                          offsetX, offsetY,
                          MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else if (_type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
            }
            else
            {
                _offsetX += offsetX;
                _offsetY += offsetY;

                // If matrix wasn't unknown we added a translation
                _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }

            //Debug_CheckType();
        }

        /// <summary>
        /// Prepends a translation to this matrix
        /// </summary>
        /// <param name='offsetX'>The offset in the x dimension</param>
        /// <param name='offsetY'>The offset in the y dimension</param>
        public void TranslatePrepend(float offsetX, float offsetY)
        {
            this = CreateTranslation(offsetX, offsetY) * this;
        }

        #endregion Set Methods

        #region Transformation Services

        /// <summary>
        /// Transform - returns the result of transforming the point by this matrix
        /// </summary>
        /// <returns>
        /// The transformed point
        /// </returns>
        /// <param name="point"> The Point to transform </param>
        public Point Transform(Point point)
        {
            Point newPoint = point;
            MultiplyPoint(ref newPoint._x, ref newPoint._y);
            return newPoint;
        }

        /// <summary>
        /// Transform - Transforms each point in the array by this matrix
        /// </summary>
        /// <param name="points"> The Point array to transform </param>
        public void Transform(Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    MultiplyPoint(ref points[i]._x, ref points[i]._y);
                }
            }
        }

        /// <summary>
        /// Transform - returns the result of transforming the Vector by this matrix.
        /// </summary>
        /// <returns>
        /// The transformed vector
        /// </returns>
        /// <param name="vector"> The Vector to transform </param>
        public Vector Transform(Vector vector)
        {
            Vector newVector = vector;
            MultiplyVector(ref newVector._x, ref newVector._y);
            return newVector;
        }

        /// <summary>
        /// Transform - Transforms each Vector in the array by this matrix.
        /// </summary>
        /// <param name="vectors"> The Vector array to transform </param>
        public void Transform(Vector[] vectors)
        {
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length; i++)
                {
                    MultiplyVector(ref vectors[i]._x, ref vectors[i]._y);
                }
            }
        }

        #endregion Transformation Services

        #region Inversion

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public float Determinant
        {
            get
            {
                switch (_type)
                {
                    case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return 1f;
                    case MatrixTypes.TRANSFORM_IS_SCALING:
                    case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return (_m11 * _m22);
                    default:
                        return (_m11 * _m22) - (_m12 * _m21);
                }
            }
        }
        float p0;
        float p1;
        float p2;
        /// <summary>
        /// 3D效果用的，支持不完善
        /// </summary>
        public float Persp0
        {
            get { return p0; }
            set
            {
                p0 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }

        /// <summary>
        /// 3D效果用的，支持不完善
        /// </summary>
        public float Persp1
        {
            get { return p1; }
            set
            {
                p1 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }

        /// <summary>
        /// 3D效果用的，支持不完善
        /// </summary>
        public float Persp2
        {
            get { return p2; }
            set
            {
                p2 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }

        /// <summary>
        /// HasInverse Property - returns true if this matrix is invertable, false otherwise.
        /// </summary>
        public bool HasInverse
        {
            get
            {
                return !FloatUtil.IsZero(Determinant);
            }
        }

        /// <summary>
        /// Replaces matrix with the inverse of the transformation.  This will throw an InvalidOperationException
        /// if !HasInverse
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This will throw an InvalidOperationException if the matrix is non-invertable
        /// </exception>
        public void Invert()
        {
            float determinant = Determinant;

            if (FloatUtil.IsZero(determinant))
            {
                throw new System.InvalidOperationException("IsZero - Returns whether or not the float is 'close' to 0.  Same as AreClose(float, 0),");
            }

            // Inversion does not change the type of a matrix.
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    break;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    {
                        _m11 = 1f / _m11;
                        _m22 = 1f / _m22;
                    }
                    break;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    _offsetX = -_offsetX;
                    _offsetY = -_offsetY;
                    break;
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    {
                        _m11 = 1f / _m11;
                        _m22 = 1f / _m22;
                        _offsetX = -_offsetX * _m11;
                        _offsetY = -_offsetY * _m22;
                    }
                    break;
                default:
                    {
                        if (p0 != 0 || p1 != 0 || p2 != 1)
                        {
                            var a00 = _m11;
                            var a01 = _m12;
                            var a02 = p0;
                            var a10 = _m21;
                            var a11 = _m22;
                            var a12 = p1;
                            var a20 = _offsetX;
                            var a21 = _offsetY;
                            var a22 = p2;

                            var b01 = a22 * a11 - a12 * a21;
                            var b11 = -a22 * a10 + a12 * a20;
                            var b21 = a21 * a10 - a11 * a20;

                            // Calculate the inverse determinant
                            var det = a00 * b01 + a01 * b11 + a02 * b21;
                            var invdet = 1f / det;

                            // If det is zero, we want to return false. However, we also want to return false if 1/det
                            // overflows to infinity (i.e. det is denormalized). All of this is subsumed by our final check
                            // at the bottom (that all 9 scalar matrix entries are finite).
                            _m11 = b01 * invdet;
                            _m12 = (-a22 * a01 + a02 * a21) * invdet;
                            p0 = (a12 * a01 - a02 * a11) * invdet;
                            _m21 = b11 * invdet;
                            _m22 = (a22 * a00 - a02 * a20) * invdet;
                            p1 = (-a12 * a00 + a02 * a10) * invdet;
                            _offsetX = b21 * invdet;
                            _offsetY = (-a21 * a00 + a01 * a20) * invdet;
                            p2 = (a11 * a00 - a01 * a10) * invdet;
                        }
                        else
                        {
                            float invdet = 1f / determinant;
                            SetMatrix(_m22 * invdet,
                                      -_m12 * invdet,
                                      -_m21 * invdet,
                                      _m11 * invdet,
                                      (_m21 * _offsetY - _offsetX * _m22) * invdet,
                                      (_offsetX * _m12 - _m11 * _offsetY) * invdet,
                                      MatrixTypes.TRANSFORM_IS_UNKNOWN);
                        }
                    }
                    break;
            }
        }

        #endregion Inversion

        #region Public Properties

        /// <summary>
        /// M11 scalex
        /// </summary>
        public float M11
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1f;
                }
                else
                {
                    return _m11;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(value, 0,
                              0, 1,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_SCALING);
                }
                else
                {
                    _m11 = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }

        /// <summary>
        /// M12 skewy
        /// </summary>
        public float M12
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _m12;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, value,
                              0, 1,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    _m12 = value;
                    _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
            }
        }

        /// <summary>
        /// M22 skewx
        /// </summary>
        public float M21
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _m21;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              value, 1,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    _m21 = value;
                    _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
            }
        }

        /// <summary>
        /// M22 scaley
        /// </summary>
        public float M22
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1f;
                }
                else
                {
                    return _m22;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              0, value,
                              0, 0,
                              MatrixTypes.TRANSFORM_IS_SCALING);
                }
                else
                {
                    _m22 = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }

        /// <summary>
        /// OffsetX
        /// </summary>
        public float OffsetX
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _offsetX;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              0, 1,
                              value, 0,
                              MatrixTypes.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    _offsetX = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }

        /// <summary>
        /// OffsetY
        /// </summary>
        public float OffsetY
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0;
                }
                else
                {
                    return _offsetY;
                }
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1, 0,
                              0, 1,
                              0, value,
                              MatrixTypes.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    _offsetY = value;
                    if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }

        #endregion Public Properties

        #region Internal Methods
        /// <summary>
        /// MultiplyVector
        /// </summary>
        internal void MultiplyVector(ref float x, ref float y)
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x *= _m11;
                    y *= _m22;
                    break;
                default:
                    float xadd = y * _m21;
                    float yadd = x * _m12;
                    x *= _m11;
                    x += xadd;
                    y *= _m22;
                    y += yadd;
                    break;
            }
        }

        /// <summary>
        /// MultiplyPoint
        /// </summary>
        internal void MultiplyPoint(ref float x, ref float y)
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    return;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x += _offsetX;
                    y += _offsetY;
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    x *= _m11;
                    y *= _m22;
                    return;
                case MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x *= _m11;
                    x += _offsetX;
                    y *= _m22;
                    y += _offsetY;
                    break;
                default:
                    if (p0 != 0 || p1 != 0 || p2 != 1)
                    {
                        var x1 = sdot(x, _m11, y, _m21) + _offsetX;
                        var y1 = sdot(x, _m12, y, _m22) + _offsetY;
                        var z1 = sdot(x, p0, y, p1) + p2;
                        if (z1 != 0)
                        {
                            z1 = 1 / z1;
                        }
                        x = x1 * z1;
                        y = y1 * z1;
                    }
                    else
                    {
                        float xadd = y * _m21 + _offsetX;
                        float yadd = x * _m12 + _offsetY;
                        x *= _m11;
                        x += xadd;
                        y *= _m22;
                        y += yadd;
                    }
                    break;
            }
        }
        static float sdot(float a, float b, float c, float d)
        {
            return a * b + c * d;
        }
        /// <summary>
        /// Creates a rotation transformation about the given point
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in radians</param>
        internal static Matrix CreateRotationRadians(float angle)
        {
            return CreateRotationRadians(angle, /* centerX = */ 0, /* centerY = */ 0);
        }

        /// <summary>
        /// Creates a rotation transformation about the given point
        /// </summary>
        /// <param name='angle'>The angle to rotate specifed in radians</param>
        /// <param name='centerX'>The centerX of rotation</param>
        /// <param name='centerY'>The centerY of rotation</param>
        internal static Matrix CreateRotationRadians(float angle, float centerX, float centerY)
        {
            Matrix matrix = new Matrix();
            matrix.p2 = 1;

            float sin = (float)(Math.Sin(angle));
            float cos = (float)(Math.Cos(angle));
            float dx = (float)((centerX * (1.0 - cos))) + (centerY * sin);
            float dy = (float)((centerY * (1.0 - cos))) - (centerX * sin);

            matrix.SetMatrix(cos, sin,
                              -sin, cos,
                              dx, dy,
                              MatrixTypes.TRANSFORM_IS_UNKNOWN);

            return matrix;
        }

        /// <summary>
        /// Creates a scaling transform around the given point
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        /// <param name='centerX'>The centerX of scaling</param>
        /// <param name='centerY'>The centerY of scaling</param>
        internal static Matrix CreateScaling(float scaleX, float scaleY, float centerX, float centerY)
        {
            Matrix matrix = new Matrix();
            matrix.p2 = 1;

            matrix.SetMatrix(scaleX, 0,
                             0, scaleY,
                             centerX - scaleX * centerX, centerY - scaleY * centerY,
                             MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION);

            return matrix;
        }

        /// <summary>
        /// Creates a scaling transform around the origin
        /// </summary>
        /// <param name='scaleX'>The scale factor in the x dimension</param>
        /// <param name='scaleY'>The scale factor in the y dimension</param>
        internal static Matrix CreateScaling(float scaleX, float scaleY)
        {
            Matrix matrix = new Matrix();
            matrix.p2 = 1;
            matrix.SetMatrix(scaleX, 0,
                             0, scaleY,
                             0, 0,
                             MatrixTypes.TRANSFORM_IS_SCALING);
            return matrix;
        }

        /// <summary>
        /// Creates a skew transform
        /// </summary>
        /// <param name='skewX'>The skew angle in the x dimension in degrees</param>
        /// <param name='skewY'>The skew angle in the y dimension in degrees</param>
        internal static Matrix CreateSkewRadians(float skewX, float skewY)
        {
            Matrix matrix = new Matrix();
            matrix.p2 = 1;

            matrix.SetMatrix(1f, (float)(Math.Tan(skewY)),
                            (float)(Math.Tan(skewX)), 1f,
                             0f, 0f,
                             MatrixTypes.TRANSFORM_IS_UNKNOWN);

            return matrix;
        }

        /// <summary>
        /// Sets the transformation to the given translation specified by the offset vector.
        /// </summary>
        /// <param name='offsetX'>The offset in X</param>
        /// <param name='offsetY'>The offset in Y</param>
        internal static Matrix CreateTranslation(float offsetX, float offsetY)
        {
            Matrix matrix = new Matrix();
            matrix.p2 = 1;

            matrix.SetMatrix(1, 0,
                             0, 1,
                             offsetX, offsetY,
                             MatrixTypes.TRANSFORM_IS_TRANSLATION);

            return matrix;
        }
        /// <summary>
        /// Creates a translation matrix from the given vector.
        /// </summary>
        /// <param name="position">The translation position.</param>
        /// <returns>A translation matrix.</returns>
        public static Matrix CreateTranslation(Vector position)
        {
            return CreateTranslation(position.X, position.Y);
        }
        #endregion Internal Methods

        #region Private Methods
        /// <summary>
        /// Sets the transformation to the identity.
        /// </summary>
        private static Matrix CreateIdentity()
        {
            Matrix matrix = new Matrix();
            matrix.p2 = 1;
            matrix.SetMatrix(1, 0,
                             0, 1,
                             0, 0,
                             MatrixTypes.TRANSFORM_IS_IDENTITY);
            return matrix;
        }

        ///<summary>
        /// Sets the transform to
        ///             / m11, m12, 0 \
        ///             | m21, m22, 0 |
        ///             \ offsetX, offsetY, 1 /
        /// where offsetX, offsetY is the translation.
        ///</summary>
        private void SetMatrix(float m11, float m12,
                               float m21, float m22,
                               float offsetX, float offsetY,
                               MatrixTypes type)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = type;
        }

        /// <summary>
        /// Set the type of the matrix based on its current contents
        /// </summary>
        private void DeriveMatrixType()
        {
            _type = 0;

            // Now classify our matrix.
            if (!(_m21 == 0 && _m12 == 0))
            {
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                return;
            }

            if (!(_m11 == 1 && _m22 == 1))
            {
                _type = MatrixTypes.TRANSFORM_IS_SCALING;
            }

            if (!(_offsetX == 0 && _offsetY == 0))
            {
                _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }

            if (0 == (_type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)))
            {
                // We have an identity matrix.
                _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
            }
            return;
        }

        ///// <summary>
        ///// Asserts that the matrix tag is one of the valid options and
        ///// that coefficients are correct.   
        ///// </summary>
        //[Conditional("DEBUG")]
        //private void Debug_CheckType()
        //{
        //    switch(_type)
        //    {
        //    case MatrixTypes.TRANSFORM_IS_IDENTITY:
        //        return;
        //    case MatrixTypes.TRANSFORM_IS_UNKNOWN:
        //        return;
        //    case MatrixTypes.TRANSFORM_IS_SCALING:
        //        Debug.Assert(_m21 == 0);
        //        Debug.Assert(_m12 == 0);
        //        Debug.Assert(_offsetX == 0);
        //        Debug.Assert(_offsetY == 0);
        //        return;
        //    case MatrixTypes.TRANSFORM_IS_TRANSLATION:
        //        Debug.Assert(_m21 == 0);
        //        Debug.Assert(_m12 == 0);
        //        Debug.Assert(_m11 == 1);
        //        Debug.Assert(_m22 == 1);
        //        return;
        //    case MatrixTypes.TRANSFORM_IS_SCALING|MatrixTypes.TRANSFORM_IS_TRANSLATION:
        //        Debug.Assert(_m21 == 0);
        //        Debug.Assert(_m12 == 0);
        //        return;
        //    default:
        //        Debug.Assert(false);
        //        return;
        //    }
        //}

        #endregion Private Methods

        #region Private Properties and Fields

        /// <summary>
        /// Efficient but conservative test for identity.  Returns
        /// true if the the matrix is identity.  If it returns false
        /// the matrix may still be identity.
        /// </summary>
        private bool IsDistinguishedIdentity
        {
            get
            {
                return _type == MatrixTypes.TRANSFORM_IS_IDENTITY;
            }
        }

        // The hash code for a matrix is the xor of its element's hashes.
        // Since the identity matrix has 2 1's and 4 0's its hash is 0.
        private const int c_identityHashCode = 0;

        #endregion Private Properties and Fields

        internal float _m11;
        internal float _m12;
        internal float _m21;
        internal float _m22;
        internal float _offsetX;
        internal float _offsetY;
        internal MatrixTypes _type;

        // This field is only used by unmanaged code which isn't detected by the compiler.
#pragma warning disable 0414
        // Matrix in blt'd to unmanaged code, so this is padding 
        // to align structure.
        //
        // ToDo: [....], Validate that this blt will work on 64-bit
        //
        internal Int32 _padding;
#pragma warning restore 0414

        /// <summary>
        /// Compares two Matrix instances for exact equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11 == matrix2.M11 &&
                       matrix1.M12 == matrix2.M12 &&
                       matrix1.M21 == matrix2.M21 &&
                       matrix1.M22 == matrix2.M22 &&
                       matrix1.OffsetX == matrix2.OffsetX &&
                       matrix1.OffsetY == matrix2.OffsetY &&
                       matrix1.Persp0 == matrix2.Persp0 &&
                       matrix1.Persp1 == matrix2.Persp1 &&
                       matrix1.Persp2 == matrix2.Persp2;
            }
        }

        /// <summary>
        /// Compares two Matrix instances for exact inequality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, float.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !(matrix1 == matrix2);
        }
        /// <summary>
        /// Compares two Matrix instances for object equality.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Matrix instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='matrix1'>The first Matrix to compare</param>
        /// <param name='matrix2'>The second Matrix to compare</param>
        public static bool Equals(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }
            else
            {
                return matrix1.M11.Equals(matrix2.M11) &&
                       matrix1.M12.Equals(matrix2.M12) &&
                       matrix1.M21.Equals(matrix2.M21) &&
                       matrix1.M22.Equals(matrix2.M22) &&
                       matrix1.OffsetX.Equals(matrix2.OffsetX) &&
                       matrix1.OffsetY.Equals(matrix2.OffsetY) &&
                       matrix1.Persp0.Equals(matrix2.Persp0) &&
                       matrix1.Persp1.Equals(matrix2.Persp1) &&
                       matrix1.Persp2.Equals(matrix2.Persp2);
            }
        }

        /// <summary>
        /// Equals - compares this Matrix with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Matrix and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Matrix))
            {
                return false;
            }

            Matrix value = (Matrix)o;
            return Matrix.Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Matrix with the passed in object.  In this equality
        /// float.NaN is equal to itself, unlike in numeric equality.
        /// Note that float values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Matrix to compare to "this"</param>
        public bool Equals(Matrix value)
        {
            return Matrix.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Matrix
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Matrix
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
                return M11.GetHashCode() ^
                       M12.GetHashCode() ^
                       M21.GetHashCode() ^
                       M22.GetHashCode() ^
                       OffsetX.GetHashCode() ^
                       OffsetY.GetHashCode() ^
                       Persp0.GetHashCode() ^
                       Persp1.GetHashCode() ^
                       Persp2.GetHashCode();
            }
        }

        ///// <summary>
        ///// Parse - returns an instance converted from the provided string using
        ///// the culture "en-US"
        ///// <param name="source"> string with Matrix data </param>
        ///// </summary>
        //public static Matrix Parse(string source)
        //{
        //    IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

        //    TokenizerHelper th = new TokenizerHelper(source, formatProvider);

        //    Matrix value;

        //    String firstToken = th.NextTokenRequired();

        //    // The token will already have had whitespace trimmed so we can do a
        //    // simple string compare.
        //    if (firstToken == "Identity")
        //    {
        //        value = Identity;
        //    }
        //    else
        //    {
        //        value = new Matrix(
        //            Convert.Tofloat(firstToken, formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider),
        //            Convert.Tofloat(th.NextTokenRequired(), formatProvider));
        //    }

        //    // There should be no more tokens in this string.
        //    th.LastTokenRequired();

        //    return value;
        //}


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
            if (IsIdentity)
            {
                return "Identity";
            }

            // Helper to get the numeric list separator for a given culture.
            char separator = ',';
            return String.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}",
                                 separator,
                                 _m11,
                                 _m12,
                                 _m21,
                                 _m22,
                                 _offsetX,
                                 _offsetY);
        }

    }

    [System.Flags]
    internal enum MatrixTypes
    {
        TRANSFORM_IS_IDENTITY = 0,
        TRANSFORM_IS_TRANSLATION = 1,
        TRANSFORM_IS_SCALING = 2,
        TRANSFORM_IS_UNKNOWN = 4
    }


    internal static class MatrixUtil
    {
        /// <summary>
        /// TransformRect - Internal helper for perf
        /// </summary>
        /// <param name="rect"> The Rect to transform. </param>
        /// <param name="matrix"> The Matrix with which to transform the Rect. </param>
        internal static void TransformRect(ref Rect rect, ref Matrix matrix)
        {
            if (rect.IsEmpty)
            {
                return;
            }

            MatrixTypes matrixType = matrix._type;

            // If the matrix is identity, don't worry.
            if (matrixType == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // Scaling
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_SCALING))
            {
                rect._x *= matrix._m11;
                rect._y *= matrix._m22;
                rect._width *= matrix._m11;
                rect._height *= matrix._m22;

                // Ensure the width is always positive.  For example, if there was a reflection about the
                // y axis followed by a translation into the visual area, the width could be negative.
                if (rect._width < 0.0)
                {
                    rect._x += rect._width;
                    rect._width = -rect._width;
                }

                // Ensure the height is always positive.  For example, if there was a reflection about the
                // x axis followed by a translation into the visual area, the height could be negative.
                if (rect._height < 0.0)
                {
                    rect._y += rect._height;
                    rect._height = -rect._height;
                }
            }

            // Translation
            if (0 != (matrixType & MatrixTypes.TRANSFORM_IS_TRANSLATION))
            {
                // X
                rect._x += matrix._offsetX;

                // Y
                rect._y += matrix._offsetY;
            }

            if (matrixType == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                // Al Bunny implementation.
                Point point0 = matrix.Transform(rect.TopLeft);
                Point point1 = matrix.Transform(rect.TopRight);
                Point point2 = matrix.Transform(rect.BottomRight);
                Point point3 = matrix.Transform(rect.BottomLeft);

                // Width and height is always positive here.
                rect._x = Math.Min(Math.Min(point0.X, point1.X), Math.Min(point2.X, point3.X));
                rect._y = Math.Min(Math.Min(point0.Y, point1.Y), Math.Min(point2.Y, point3.Y));

                rect._width = Math.Max(Math.Max(point0.X, point1.X), Math.Max(point2.X, point3.X)) - rect._x;
                rect._height = Math.Max(Math.Max(point0.Y, point1.Y), Math.Max(point2.Y, point3.Y)) - rect._y;
            }
        }

        /// <summary>
        /// Multiplies two transformations, where the behavior is matrix1 *= matrix2.
        /// This code exists so that we can efficient combine matrices without copying
        /// the data around, since each matrix is 52 bytes.
        /// To reduce duplication and to ensure consistent behavior, this is the
        /// method which is used to implement Matrix * Matrix as well.
        /// </summary>
        internal static void MultiplyMatrix(ref Matrix matrix1, ref Matrix matrix2)
        {
            MatrixTypes type1 = matrix1._type;
            MatrixTypes type2 = matrix2._type;

            // Check for idents

            // If the second is ident, we can just return
            if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                return;
            }

            // If the first is ident, we can just copy the memory across.
            if (type1 == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix1 = matrix2;
                return;
            }

            // Optimize for translate case, where the second is a translate
            if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // 2 additions
                matrix1._offsetX += matrix2._offsetX;
                matrix1._offsetY += matrix2._offsetY;

                // If matrix 1 wasn't unknown we added a translation
                if (type1 != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }

                return;
            }

            // Check for the first value being a translate
            if (type1 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
            {
                // Save off the old offsets
                float offsetX = matrix1._offsetX;
                float offsetY = matrix1._offsetY;

                // Copy the matrix
                matrix1 = matrix2;

                matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
                matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;

                if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
                else
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
                return;
            }

            // The following code combines the type of the transformations so that the high nibble
            // is "this"'s type, and the low nibble is mat's type.  This allows for a switch rather
            // than nested switches.

            // trans1._type |  trans2._type
            //  7  6  5  4   |  3  2  1  0
            int combinedType = ((int)type1 << 4) | (int)type2;

            switch (combinedType)
            {
                case 34:  // S * S
                    // 2 multiplications
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    return;

                case 35:  // S * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._offsetX;
                    matrix1._offsetY = matrix2._offsetY;

                    // Transform set to Translate and Scale
                    matrix1._type = MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING;
                    return;

                case 50: // S|T * S
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX *= matrix2._m11;
                    matrix1._offsetY *= matrix2._m22;
                    return;

                case 51: // S|T * S|T
                    matrix1._m11 *= matrix2._m11;
                    matrix1._m22 *= matrix2._m22;
                    matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                    matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                    return;
                case 36: // S * U
                case 52: // S|T * U
                case 66: // U * S
                case 67: // U * S|T
                case 68: // U * U
                    matrix1 = new Matrix(
                        matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
                        matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,

                        matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
                        matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,

                        matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX,
                        matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                    return;
                    //#if DEBUG
                    //            default:
                    //                Debug.Fail("Matrix multiply hit an invalid case: " + combinedType);
                    //                break;
                    //#endif
            }
        }

        /// <summary>
        /// Applies an offset to the specified matrix in place.
        /// </summary>
        internal static void PrependOffset(
            ref Matrix matrix,
            float offsetX,
            float offsetY)
        {
            if (matrix._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix = new Matrix(1, 0, 0, 1, offsetX, offsetY);
                matrix._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }
            else
            {
                // 
                //  / 1   0   0 \       / m11   m12   0 \
                //  | 0   1   0 |   *   | m21   m22   0 |
                //  \ tx  ty  1 /       \  ox    oy   1 /
                //
                //       /   m11                  m12                     0 \
                //  =    |   m21                  m22                     0 |
                //       \   m11*tx+m21*ty+ox     m12*tx + m22*ty + oy    1 /
                //

                matrix._offsetX += matrix._m11 * offsetX + matrix._m21 * offsetY;
                matrix._offsetY += matrix._m12 * offsetX + matrix._m22 * offsetY;

                if (matrix._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }
    }


}
