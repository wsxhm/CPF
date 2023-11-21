using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 定义Grid的行
    /// </summary>
    public class RowDefinition : DefinitionBase
    {
        /// <summary>
        /// 权重*或者数值
        /// </summary>
        [PropertyMetadata(typeof(GridLength), "*")]
        public GridLength Height
        {
            get { return (GridLength)GetValue(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(typeof(FloatField), "Auto")]
        public FloatField MaxHeight
        {
            get { return (FloatField)GetValue(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(typeof(FloatField), "0")]
        public FloatField MinHeight
        {
            get { return (FloatField)GetValue(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public float ActualHeight
        {
            get; internal set;
        }

        internal override GridLength UserSizeValueCache => Height;

        internal List<UIElement> UIElements;
        internal float offset;
    }
}
