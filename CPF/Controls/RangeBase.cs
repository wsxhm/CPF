using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示具有特定范围内的值的元素。
    /// </summary>
    [Description("表示具有特定范围内的值的元素。"), Browsable(false)]
    [DefaultProperty(nameof(Value))]
    public abstract class RangeBase : Control
    {
        /// <summary>
        /// 最小值
        /// </summary>
        [Description("最小值")]
        public double Minimum
        {
            get { return (double)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        [Description("最大值")]
        [PropertyMetadata(1d)]
        public double Maximum
        {
            get { return (double)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 当前值
        /// </summary>
        [Description("当前值")]
        public double Value
        {
            get { return (double)GetValue(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(.1d)]
        public double LargeChange
        {
            get
            {
                return (double)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        [PropertyMetadata(.05d)]
        public double SmallChange
        {
            get
            {
                return (double)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Value))
            {
                var v = (double)Convert.ChangeType(value, typeof(double));
                var min = Minimum;
                if (v < min)
                {
                    value = min;
                }
                else
                {
                    var max = Maximum;
                    if (v > max)
                    {
                        value = max;
                    }
                }
            }
            else if (propertyName == nameof(Maximum))
            {
                var max = (double)Convert.ChangeType(value, typeof(double));
                if (max < Minimum)
                {
                    throw new Exception("Maximum不能小于Minimum");
                }
                if (Value > max)
                {
                    Value = max;
                }
            }
            else if (propertyName == nameof(Minimum))
            {
                var min = (double)Convert.ChangeType(value, typeof(double));
                if (min > Maximum)
                {
                    throw new Exception("Maximum不能小于Minimum");
                }
                if (Value < min)
                {
                    Value = min;
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
            if (propertyName == nameof(Value))
            {
                RaiseEvent(EventArgs.Empty, nameof(ValueChanged));
            }
        }

        public event EventHandler ValueChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
    }
}
