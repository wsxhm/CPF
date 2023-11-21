using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 定义Grid的列
    /// </summary>
    public class ColumnDefinition : DefinitionBase
    {
        /// <summary>
        /// 权重*或者数值
        /// </summary>
        [PropertyMetadata(typeof(GridLength), "*")]
        public GridLength Width
        {
            get { return (GridLength)GetValue(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(typeof(FloatField), "100%")]
        public FloatField MaxWidth
        {
            get { return (FloatField)GetValue(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(typeof(FloatField), "0")]
        public FloatField MinWidth
        {
            get { return (FloatField)GetValue(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public float ActualWidth
        {
            get; internal set;
        }

        internal override GridLength UserSizeValueCache => Width;

        internal List<UIElement> UIElements;

        internal float offset;
    }
}
