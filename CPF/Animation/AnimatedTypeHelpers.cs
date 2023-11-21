using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;

namespace CPF.Animation
{
    public static class AnimatedTypeHelpers
    {
        #region Interpolation Methods
        public static Thickness InterpolateThickness(Thickness from, Thickness to, double progress)
        {
            return new Thickness(
                (float)InterpolateDouble(from.Left, to.Left, progress),
                (float)InterpolateDouble(from.Top, to.Top, progress),
                (float)InterpolateDouble(from.Right, to.Right, progress),
                (float)InterpolateDouble(from.Bottom, to.Bottom, progress));
        }

        public static Byte InterpolateByte(Byte from, Byte to, Double progress)
        {
            return (Byte)((Int32)from + (Int32)((((Double)(to - from)) + (Double)0.5) * progress));
        }

        public static Color InterpolateColor(Color from, Color to, Double progress)
        {
            return from + ((to - from) * (Single)progress);
        }

        public static Decimal InterpolateDecimal(Decimal from, Decimal to, Double progress)
        {
            return from + ((to - from) * (Decimal)progress);
        }

        public static Double InterpolateDouble(Double from, Double to, Double progress)
        {
            return from + ((to - from) * progress);
        }

        public static Int16 InterpolateInt16(Int16 from, Int16 to, Double progress)
        {
            if (progress == 0.0)
            {
                return from;
            }
            else if (progress == 1.0)
            {
                return to;
            }
            else
            {
                Double addend = (Double)(to - from);
                addend *= progress;
                addend += (addend > 0.0) ? 0.5 : -0.5;

                return (Int16)(from + (Int16)addend);
            }
        }

        public static Int32 InterpolateInt32(Int32 from, Int32 to, Double progress)
        {
            if (progress == 0.0)
            {
                return from;
            }
            else if (progress == 1.0)
            {
                return to;
            }
            else
            {
                Double addend = (Double)(to - from);
                addend *= progress;
                addend += (addend > 0.0) ? 0.5 : -0.5;

                return from + (Int32)addend;
            }
        }

        public static Int64 InterpolateInt64(Int64 from, Int64 to, Double progress)
        {
            if (progress == 0.0)
            {
                return from;
            }
            else if (progress == 1.0)
            {
                return to;
            }
            else
            {
                Double addend = (Double)(to - from);
                addend *= progress;
                addend += (addend > 0.0) ? 0.5 : -0.5;

                return from + (Int64)addend;
            }
        }

        public static Point InterpolatePoint(Point from, Point to, float progress)
        {
            return from + ((to - from) * progress);
        }

        //public static Point3D InterpolatePoint3D(Point3D from, Point3D to, Double progress)
        //{
        //    return from + ((to - from) * progress);
        //}

        //public static Quaternion InterpolateQuaternion(Quaternion from, Quaternion to, Double progress, bool useShortestPath)
        //{
        //    return Quaternion.Slerp(from, to, progress, useShortestPath);
        //}

        public static Rect InterpolateRect(Rect from, Rect to, float progress)
        {
            Rect temp = new Rect();

            // from + ((from - to) * progress)
            temp.Location = new Point(
                from.Location.X + ((to.Location.X - from.Location.X) * progress),
                from.Location.Y + ((to.Location.Y - from.Location.Y) * progress));
            temp.Size = new Size(
                from.Size.Width + ((to.Size.Width - from.Size.Width) * progress),
                from.Size.Height + ((to.Size.Height - from.Size.Height) * progress));

            return temp;
        }

        //public static Rotation3D InterpolateRotation3D(Rotation3D from, Rotation3D to, Double progress)
        //{
        //    return new QuaternionRotation3D(InterpolateQuaternion(from.publicQuaternion, to.publicQuaternion, progress, /* useShortestPath = */ true));
        //}

        public static Single InterpolateSingle(Single from, Single to, Double progress)
        {
            return from + (Single)((to - from) * progress);
        }

        public static Size InterpolateSize(Size from, Size to, float progress)
        {
            return (Size)InterpolateVector((Vector)from, (Vector)to, progress);
        }

        public static Vector InterpolateVector(Vector from, Vector to, float progress)
        {
            return from + ((to - from) * progress);
        }

        //public static Vector3D InterpolateVector3D(Vector3D from, Vector3D to, Double progress)
        //{
        //    return from + ((to - from) * progress);
        //}


        //public static double GetRotationAngleFromMatrix(Matrix matrix)
        //{
        //    double s11 = Math.Sign(matrix.M11);
        //    double s22 = Math.Sign(matrix.M22);

        //    double quadrantCorrectionFactor = s11 < 0 && s22 < 0 ? -180.0 : 0;

        //    double atan = Math.Atan(matrix.M12 / matrix.M22);

        //    double degrees = ToDegrees(atan);

        //    degrees = degrees + quadrantCorrectionFactor;

        //    degrees = degrees < 0 ? degrees + 360.0 : degrees;

        //    return degrees;
        //}
        ///// <summary>
        ///// Converts an angle expressed in radians to an angle expressed in degrees
        ///// </summary>
        ///// <param name="radians">The angle in radians</param>
        ///// <returns>The angle expressed in degrees</returns>
        //public static double ToDegrees(double radians)
        //{
        //    double angle = (radians * 180 / Math.PI) % 360;

        //    angle = angle < 0 ? angle + 360.0 : angle;

        //    return angle;
        //}
        //public static Matrix InterpolateMatrix(Matrix from, Matrix to, double progress)
        //{
        //    double angleFrom = GetRotationAngleFromMatrix(from);
        //    double angleTo = GetRotationAngleFromMatrix(to);

        //    double xFrom = from.OffsetX;
        //    double xTo = to.OffsetX;

        //    double yFrom = from.OffsetY;
        //    double yTo = to.OffsetY;

        //    double angleInterpolated = angleFrom + ((angleTo - angleFrom) * progress);
        //    double xInterpolated = xFrom + ((xTo - xFrom) * progress);
        //    double yInterpolated = yFrom + ((yTo - yFrom) * progress);

        //    Matrix interpolated = Matrix.Identity;

        //    interpolated.Rotate((float)angleInterpolated);
        //    interpolated.Translate((float)xInterpolated, (float)yInterpolated);

        //    return interpolated;
        //}
        public static Matrix InterpolateMatrix(Matrix from, Matrix to, double progress)
        {
            float normalizedTime = (float)progress;
            var newMatrix = new Matrix(
                     ((to.M11 - from.M11) * normalizedTime) + from.M11,
                     ((to.M12 - from.M12) * normalizedTime) + from.M12,
                     ((to.M21 - from.M21) * normalizedTime) + from.M21,
                     ((to.M22 - from.M22) * normalizedTime) + from.M22,
                     ((to.OffsetX - from.OffsetX) * normalizedTime) + from.OffsetX,
                     ((to.OffsetY - from.OffsetY) * normalizedTime) + from.OffsetY);

            return newMatrix;
        }
        #endregion

        #region Add Methods

        public static Byte AddByte(Byte value1, Byte value2)
        {
            return (Byte)(value1 + value2);
        }

        public static Color AddColor(Color value1, Color value2)
        {
            return value1 + value2;
        }

        public static Decimal AddDecimal(Decimal value1, Decimal value2)
        {
            return value1 + value2;
        }

        public static Double AddDouble(Double value1, Double value2)
        {
            return value1 + value2;
        }

        public static Int16 AddInt16(Int16 value1, Int16 value2)
        {
            return (Int16)(value1 + value2);
        }

        public static Int32 AddInt32(Int32 value1, Int32 value2)
        {
            return value1 + value2;
        }

        public static Int64 AddInt64(Int64 value1, Int64 value2)
        {
            return value1 + value2;
        }

        public static Point AddPoint(Point value1, Point value2)
        {
            return new Point(
                value1.X + value2.X,
                value1.Y + value2.Y);
        }

        //public static Point3D AddPoint3D(Point3D value1, Point3D value2)
        //{
        //    return new Point3D(
        //        value1.X + value2.X,
        //        value1.Y + value2.Y,
        //        value1.Z + value2.Z);
        //}

        //public static Quaternion AddQuaternion(Quaternion value1, Quaternion value2)
        //{
        //    return value1 * value2;
        //}

        public static Single AddSingle(Single value1, Single value2)
        {
            return value1 + value2;
        }

        public static Size AddSize(Size value1, Size value2)
        {
            return new Size(
                value1.Width + value2.Width,
                value1.Height + value2.Height);
        }

        public static Vector AddVector(Vector value1, Vector value2)
        {
            return value1 + value2;
        }

        //public static Vector3D AddVector3D(Vector3D value1, Vector3D value2)
        //{
        //    return value1 + value2;
        //}

        public static Rect AddRect(Rect value1, Rect value2)
        {
            return new Rect(
                AddPoint(value1.Location, value2.Location),
                AddSize(value1.Size, value2.Size));
        }

        //public static Rotation3D AddRotation3D(Rotation3D value1, Rotation3D value2)
        //{
        //    if (value1 == null)
        //    {
        //        value1 = Rotation3D.Identity;
        //    }
        //    if (value2 == null)
        //    {
        //        value2 = Rotation3D.Identity;
        //    }

        //    return new QuaternionRotation3D(AddQuaternion(value1.publicQuaternion, value2.publicQuaternion));
        //}

        #endregion

        #region Subtract Methods

        public static Byte SubtractByte(Byte value1, Byte value2)
        {
            return (Byte)(value1 - value2);
        }

        public static Color SubtractColor(Color value1, Color value2)
        {
            return value1 - value2;
        }

        public static Decimal SubtractDecimal(Decimal value1, Decimal value2)
        {
            return value1 - value2;
        }

        public static Double SubtractDouble(Double value1, Double value2)
        {
            return value1 - value2;
        }

        public static Int16 SubtractInt16(Int16 value1, Int16 value2)
        {
            return (Int16)(value1 - value2);
        }

        public static Int32 SubtractInt32(Int32 value1, Int32 value2)
        {
            return value1 - value2;
        }

        public static Int64 SubtractInt64(Int64 value1, Int64 value2)
        {
            return value1 - value2;
        }

        public static Point SubtractPoint(Point value1, Point value2)
        {
            return new Point(
                value1.X - value2.X,
                value1.Y - value2.Y);
        }

        //public static Point3D SubtractPoint3D(Point3D value1, Point3D value2)
        //{
        //    return new Point3D(
        //        value1.X - value2.X,
        //        value1.Y - value2.Y,
        //        value1.Z - value2.Z);
        //}

        //public static Quaternion SubtractQuaternion(Quaternion value1, Quaternion value2)
        //{
        //    value2.Invert();

        //    return value1 * value2;
        //}

        public static Single SubtractSingle(Single value1, Single value2)
        {
            return value1 - value2;
        }

        public static Size SubtractSize(Size value1, Size value2)
        {
            return new Size(
                value1.Width - value2.Width,
                value1.Height - value2.Height);
        }

        public static Vector SubtractVector(Vector value1, Vector value2)
        {
            return value1 - value2;
        }

        //public static Vector3D SubtractVector3D(Vector3D value1, Vector3D value2)
        //{
        //    return value1 - value2;
        //}

        public static Rect SubtractRect(Rect value1, Rect value2)
        {
            return new Rect(
                SubtractPoint(value1.Location, value2.Location),
                SubtractSize(value1.Size, value2.Size));
        }

        //public static Rotation3D SubtractRotation3D(Rotation3D value1, Rotation3D value2)
        //{
        //    return new QuaternionRotation3D(SubtractQuaternion(value1.publicQuaternion, value2.publicQuaternion));
        //}

        #endregion

        #region GetSegmentLength Methods

        public static Double GetSegmentLengthBoolean(Boolean from, Boolean to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static Double GetSegmentLengthByte(Byte from, Byte to)
        {
            return Math.Abs((Int32)to - (Int32)from);
        }

        public static Double GetSegmentLengthChar(Char from, Char to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static Double GetSegmentLengthColor(Color from, Color to)
        {
            return Math.Abs(to.ScA - from.ScA)
                 + Math.Abs(to.ScR - from.ScR)
                 + Math.Abs(to.ScG - from.ScG)
                 + Math.Abs(to.ScB - from.ScB);
        }

        public static Double GetSegmentLengthDecimal(Decimal from, Decimal to)
        {
            // We may lose precision here, but it's not likely going to be a big deal
            // for the purposes of this method.  The relative lengths of Decimal
            // segments will still be adequately represented.
            return (Double)Math.Abs(to - from);
        }

        public static Double GetSegmentLengthDouble(Double from, Double to)
        {
            return Math.Abs(to - from);
        }

        public static Double GetSegmentLengthInt16(Int16 from, Int16 to)
        {
            return Math.Abs(to - from);
        }

        public static Double GetSegmentLengthInt32(Int32 from, Int32 to)
        {
            return Math.Abs(to - from);
        }

        public static Double GetSegmentLengthInt64(Int64 from, Int64 to)
        {
            return Math.Abs(to - from);
        }

        public static Double GetSegmentLengthMatrix(Matrix from, Matrix to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static Double GetSegmentLengthObject(Object from, Object to)
        {
            return 1.0;
        }

        public static Double GetSegmentLengthPoint(Point from, Point to)
        {
            return Math.Abs((to - from).Length);
        }

        //public static Double GetSegmentLengthPoint3D(Point3D from, Point3D to)
        //{
        //    return Math.Abs((to - from).Length);
        //}

        //public static Double GetSegmentLengthQuaternion(Quaternion from, Quaternion to)
        //{
        //    from.Invert();

        //    return (to * from).Angle;
        //}

        public static Double GetSegmentLengthRect(Rect from, Rect to)
        {
            // This seems to me to be the most logical way to define the
            // distance between two rects.  Lots of sqrt, but since paced
            // rectangle animations are such a rare thing, we may as well do
            // them right since the user obviously knows what they want.
            Double a = GetSegmentLengthPoint(from.Location, to.Location);
            Double b = GetSegmentLengthSize(from.Size, to.Size);

            // Return c.
            return Math.Sqrt((a * a) + (b * b));
        }

        //public static Double GetSegmentLengthRotation3D(Rotation3D from, Rotation3D to)
        //{
        //    return GetSegmentLengthQuaternion(from.publicQuaternion, to.publicQuaternion);
        //}

        public static Double GetSegmentLengthSingle(Single from, Single to)
        {
            return Math.Abs(to - from);
        }

        public static Double GetSegmentLengthSize(Size from, Size to)
        {
            return Math.Abs(((Vector)to - (Vector)from).Length);
        }

        public static Double GetSegmentLengthString(String from, String to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static Double GetSegmentLengthVector(Vector from, Vector to)
        {
            return Math.Abs((to - from).Length);
        }

        //public static Double GetSegmentLengthVector3D(Vector3D from, Vector3D to)
        //{
        //    return Math.Abs((to - from).Length);
        //}

        #endregion

        #region Scale Methods

        public static Byte ScaleByte(Byte value, Double factor)
        {
            return (Byte)((Double)value * factor);
        }

        public static Color ScaleColor(Color value, Double factor)
        {
            return value * (Single)factor;
        }

        public static Decimal ScaleDecimal(Decimal value, float factor)
        {
            return value * (Decimal)factor;
        }

        public static Double ScaleDouble(Double value, float factor)
        {
            return value * factor;
        }

        public static Int16 ScaleInt16(Int16 value, float factor)
        {
            return (Int16)((Double)value * factor);
        }

        public static Int32 ScaleInt32(Int32 value, float factor)
        {
            return (Int32)((Double)value * factor);
        }

        public static Int64 ScaleInt64(Int64 value, float factor)
        {
            return (Int64)((Double)value * factor);
        }

        public static Point ScalePoint(Point value, float factor)
        {
            return new Point(
                value.X * factor,
                value.Y * factor);
        }

        //public static Point3D ScalePoint3D(Point3D value, Double factor)
        //{
        //    return new Point3D(
        //        value.X * factor,
        //        value.Y * factor,
        //        value.Z * factor);
        //}

        //public static Quaternion ScaleQuaternion(Quaternion value, Double factor)
        //{
        //    return new Quaternion(value.Axis, value.Angle * factor);
        //}

        public static Rect ScaleRect(Rect value, float factor)
        {
            Rect temp = new Rect();

            temp.Location = new Point(
                value.Location.X * factor,
                value.Location.Y * factor);
            temp.Size = new Size(
                value.Size.Width * factor,
                value.Size.Height * factor);

            return temp;
        }

        //public static Rotation3D ScaleRotation3D(Rotation3D value, Double factor)
        //{
        //    return new QuaternionRotation3D(ScaleQuaternion(value.publicQuaternion, factor));
        //}

        public static Single ScaleSingle(Single value, Double factor)
        {
            return (Single)((Double)value * factor);
        }

        public static Size ScaleSize(Size value, float factor)
        {
            return (Size)((Vector)value * factor);
        }

        public static Vector ScaleVector(Vector value, float factor)
        {
            return value * factor;
        }

        //public static Vector3D ScaleVector3D(Vector3D value, Double factor)
        //{
        //    return value * factor;
        //}

        #endregion

        #region EnsureValidAnimationValue Methods

        public static bool IsValidAnimationValueBoolean(Boolean value)
        {
            return true;
        }

        public static bool IsValidAnimationValueByte(Byte value)
        {
            return true;
        }

        public static bool IsValidAnimationValueChar(Char value)
        {
            return true;
        }

        public static bool IsValidAnimationValueColor(Color value)
        {
            return true;
        }

        public static bool IsValidAnimationValueDecimal(Decimal value)
        {
            return true;
        }

        public static bool IsValidAnimationValueDouble(Double value)
        {
            if (IsInvalidDouble(value))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidAnimationValueInt16(Int16 value)
        {
            return true;
        }

        public static bool IsValidAnimationValueInt32(Int32 value)
        {
            return true;
        }

        public static bool IsValidAnimationValueInt64(Int64 value)
        {
            return true;
        }

        public static bool IsValidAnimationValueMatrix(Matrix value)
        {
            return true;
        }

        public static bool IsValidAnimationValuePoint(Point value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y))
            {
                return false;
            }

            return true;
        }

        //public static bool IsValidAnimationValuePoint3D(Point3D value)
        //{
        //    if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y) || IsInvalidDouble(value.Z))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public static bool IsValidAnimationValueQuaternion(Quaternion value)
        //{
        //    if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y)
        //        || IsInvalidDouble(value.Z) || IsInvalidDouble(value.W))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public static bool IsValidAnimationValueRect(Rect value)
        {
            if (IsInvalidDouble(value.Location.X) || IsInvalidDouble(value.Location.Y)
                || IsInvalidDouble(value.Size.Width) || IsInvalidDouble(value.Size.Height)
                || value.IsEmpty)
            {
                return false;
            }

            return true;
        }

        //public static bool IsValidAnimationValueRotation3D(Rotation3D value)
        //{
        //    return IsValidAnimationValueQuaternion(value.publicQuaternion);
        //}

        public static bool IsValidAnimationValueSingle(Single value)
        {
            if (IsInvalidDouble(value))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidAnimationValueSize(Size value)
        {
            if (IsInvalidDouble(value.Width) || IsInvalidDouble(value.Height))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidAnimationValueString(String value)
        {
            return true;
        }

        public static bool IsValidAnimationValueVector(Vector value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y))
            {
                return false;
            }

            return true;
        }

        //public static bool IsValidAnimationValueVector3D(Vector3D value)
        //{
        //    if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y) || IsInvalidDouble(value.Z))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        #endregion

        #region GetZeroValueMethods

        public static Byte GetZeroValueByte(Byte baseValue)
        {
            return 0;
        }

        public static Color GetZeroValueColor(Color baseValue)
        {
            return Color.FromScRgb(0.0F, 0.0F, 0.0F, 0.0F);
        }

        public static Decimal GetZeroValueDecimal(Decimal baseValue)
        {
            return Decimal.Zero;
        }

        public static Double GetZeroValueDouble(Double baseValue)
        {
            return 0.0;
        }

        public static Int16 GetZeroValueInt16(Int16 baseValue)
        {
            return 0;
        }

        public static Int32 GetZeroValueInt32(Int32 baseValue)
        {
            return 0;
        }

        public static Int64 GetZeroValueInt64(Int64 baseValue)
        {
            return 0;
        }

        public static Point GetZeroValuePoint(Point baseValue)
        {
            return new Point();
        }

        //public static Point3D GetZeroValuePoint3D(Point3D baseValue)
        //{
        //    return new Point3D();
        //}

        //public static Quaternion GetZeroValueQuaternion(Quaternion baseValue)
        //{
        //    return Quaternion.Identity;
        //}

        public static Single GetZeroValueSingle(Single baseValue)
        {
            return 0.0F;
        }

        public static Size GetZeroValueSize(Size baseValue)
        {
            return new Size();
        }

        public static Vector GetZeroValueVector(Vector baseValue)
        {
            return new Vector();
        }

        //public static Vector3D GetZeroValueVector3D(Vector3D baseValue)
        //{
        //    return new Vector3D();
        //}

        public static Rect GetZeroValueRect(Rect baseValue)
        {
            return new Rect(new Point(), new Vector());
        }

        //public static Rotation3D GetZeroValueRotation3D(Rotation3D baseValue)
        //{
        //    return Rotation3D.Identity;
        //}

        #endregion

        #region Helpers

        private static Boolean IsInvalidDouble(Double value)
        {
            return Double.IsInfinity(value)
                || DoubleUtil.IsNaN(value);
        }

        #endregion
    }
}
