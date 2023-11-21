using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CPF.Design;
namespace CPF
{
    /// <summary>
    /// 单精度浮点带单位
    /// </summary>
    //[TypeConverter(typeof(FloatValueTypeConverter))]
    //[Editor(typeof(FloatValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Description("格式：数字或者百分比数字，100、100%")]
    public struct FloatField : ISerializerCode
    {
        /// <summary>
        /// 单精度浮点带单位
        /// </summary>
        /// <param name="value">如果value等于 float.NaN 一般相当于Auto</param>
        /// <param name="unit"></param>
        public FloatField(in float value, in Unit unit)
        {
            this.value = value;
            this.unit = unit;
        }
        float value;
        /// <summary>
        /// 如果value等于 float.NaN 一般相当于Auto
        /// </summary>
        public float Value { get { return value; } set { this.value = value; } }
        Unit unit;
        /// <summary>
        /// 单位
        /// </summary>
        public Unit Unit { get { return unit; } set { this.unit = value; } }

        public static FloatField operator *(FloatField f1, int n)
        {
            return new FloatField(f1.value * n, f1.unit);
        }
        public static FloatField operator *(FloatField f1, float n)
        {
            return new FloatField(f1.value * n, f1.unit);
        }
        public static FloatField operator *(FloatField f1, double n)
        {
            return new FloatField((float)(f1.value * n), f1.unit);
        }

        public static FloatField operator +(FloatField pointField, float value)
        {
            return new FloatField(pointField.value + value, pointField.unit);
        }
        public static FloatField operator -(FloatField pointField, float value)
        {
            return new FloatField(pointField.value - value, pointField.unit);
        }

        public static implicit operator FloatField(int n)
        {
            return new FloatField(n, Unit.Default);
        }

        public static implicit operator FloatField(float n)
        {
            return new FloatField(n, Unit.Default);
        }
        /// <summary>
        /// 计算实际值
        /// </summary>
        /// <param name="value">单位为百分比的时候用的计算值</param>
        /// <returns></returns>
        public float GetActualValue(in float value)
        {
            if (IsAuto || IsZero)
            {
                return 0;
            }
            if (unit == Unit.Default)
            {
                return this.value;
            }
            return this.value * value;
        }
        ///// <summary>
        ///// 只返回单位为Default情况下的值，其他情况都是0，由于百分比是相对可变的
        ///// </summary>
        ///// <returns></returns>
        //public float GetFixedValue()
        //{
        //    if (Unit == Unit.Default && !IsAuto)
        //    {
        //        return Value;
        //    }
        //    return 0;
        //}

        /// <summary>
        /// 双精度转化为单精度，单位默认
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator FloatField(double n)
        {
            return new FloatField((float)n, Unit.Default);
        }
        /// <summary>
        /// 数字%或者直接数字或者auto
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator FloatField(string n)
        {
            return FloatField.Parse(n);
        }

        public static bool operator ==(FloatField f1, FloatField f2)
        {
            if ((f1.IsAuto && f2.IsAuto) || (f1.IsZero && f2.IsZero))
            {
                return true;
            }
            return f1.value == f2.value &&
                   f1.unit == f2.unit;
        }

        public static bool operator !=(FloatField f1, FloatField f2)
        {
            return !(f1 == f2);
        }

        public static bool Equals(FloatField f1, FloatField f2)
        {
            return f1.value.Equals(f2.value) &&
                   f1.unit.Equals(f2.unit);
        }

        public override bool Equals(object o)
        {
            if ((null == o) || !(o is FloatField))
            {
                return false;
            }

            return FloatField.Equals(this, (FloatField)o);
        }

        public bool Equals(FloatField value)
        {
            return FloatField.Equals(this, value);
        }


        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return value.GetHashCode() ^
                   unit.GetHashCode();
        }

        public static FloatField Parse(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return FloatField.Auto;
            }
            try
            {
                if (s.ToLower() == "auto")
                {
                    return FloatField.Auto;
                }
                //if (s.ToLower() == "zero")
                //{
                //    return FloatField.Zero;
                //}
                if (s[s.Length - 1] != '%')
                {
                    return new FloatField(float.Parse(s), Unit.Default);
                }
                else
                {
                    return new FloatField(float.Parse(s.TrimEnd('%')) / 100, Unit.Percent);
                }
            }
            catch (Exception)
            {
                throw new InvalidCastException("FloatValue字符串格式不对");
            }
        }

        public static FloatField Auto
        {
            get { return new FloatField(float.NaN, Unit.Default); }
        }

        public static FloatField Zero
        {
            get { return new FloatField(0f, Unit.Default); }
        }

        public bool IsAuto
        {
            get { return float.IsNaN(value); }
        }
        public bool IsZero
        {
            get { return value == 0f; }
        }

        public override string ToString()
        {
            if (IsZero)
            {
                return "0";
            }
            else if (IsAuto)
            {
                return "Auto";
            }
            else
            {
                if (this.unit == Unit.Default)
                {
                    return this.value.ToString();
                }
                else
                {
                    return this.value * 100 + "%";
                }
            }
        }

        public string GetCreationCode()
        {
            if (unit == Unit.Default && !IsAuto)
            {
                return value.ToString("0.#f");
            }
            return "\"" + this.ToString() + "\"";
            //return $"new FloatField({value}f,Unit.{unit})";
        }
    }

    public enum Unit : byte
    {
        /// <summary>
        /// 默认，96DPI中1像素
        /// </summary>
        Default,
        /// <summary>
        /// 百分比 %
        /// </summary>
        Percent,
    }
}
