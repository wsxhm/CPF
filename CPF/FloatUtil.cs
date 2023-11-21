using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public static class FloatUtil
    {
        internal static float FLT_EPSILON = 1.192092896e-07F;
        internal static float FLT_MAX_PRECISION = 0xffffff;
        internal static float INVERSE_FLT_MAX_PRECISION = 1.0F / FLT_MAX_PRECISION;
        internal static bool IsFloatFinite(object o)
        {
            float d = (float)o;
            return !(float.IsInfinity(d) || FloatUtil.IsNaN(d));
        }
        internal static bool IsFloatFiniteOrNaN(object o)
        {
            float d = (float)o;
            return !(float.IsInfinity(d));
        }
        /// <summary>
        /// GreaterThan - Returns whether or not the first float is greater than the second float.
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        /// <param name="value1"> The first Float to compare. </param>
        /// <param name="value2"> The second Float to compare. </param>
        public static bool GreaterThan(float value1, float value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        public static bool LessThan(float value1, float value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }
        public static bool GreaterThanOrClose(float value1, float value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }
        public static bool LessThanOrClose(float value1, float value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }
        /// <summary>
        /// AreClose
        /// </summary>
        public static bool AreClose(float a, float b)
        {
            if (a == b) return true;
            // This computes (|a-b| / (|a| + |b| + 10.0f)) < FLT_EPSILON
            float eps = (Math.Abs(a) + Math.Abs(b) + 10.0f) * FLT_EPSILON;
            float delta = a - b;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// IsOne
        /// </summary>
        public static bool IsOne(float a)
        {
            return (float)Math.Abs(a - 1.0f) < 10.0f * FLT_EPSILON;
        }

        /// <summary>
        /// IsZero
        /// </summary>
        public static bool IsZero(float a)
        {
            return (float)Math.Abs(a) < 10.0f * FLT_EPSILON;
        }

        /// <summary>
        /// IsCloseToDivideByZero
        /// </summary>
        public static bool IsCloseToDivideByZero(float numerator, float denominator)
        {
            // When updating this, please also update code in Arithmetic.h
            return Math.Abs(denominator) <= Math.Abs(numerator) * INVERSE_FLT_MAX_PRECISION;
        }

        public static bool IsNaN(float f)
        {
            return float.IsNaN(f);
        }
    }
}
