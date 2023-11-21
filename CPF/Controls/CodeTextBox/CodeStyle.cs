using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public class CodeStyle : CpfObject
    {
        /// <summary>
        /// 背景填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Background
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }

        ///// <summary>
        ///// 字体样式
        ///// </summary>
        //[UIPropertyMetadata(FontStyles.Regular, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        //public FontStyles FontStyle
        //{
        //    get { return (FontStyles)GetValue(); }
        //    set { SetValue(value); }
        //}

        /// <summary>
        /// 前景色
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender)]
        public ViewFill Foreground
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// </summary>
        [UIPropertyMetadata(typeof(TextDecoration), "none", UIPropertyOptions.AffectsRender)]
        public TextDecoration TextDecoration
        {
            get { return (TextDecoration)GetValue(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public UIElement Owner
        {
            get;set;
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
            if (Owner)
            {
                Owner.Invalidate();
            }
        }
    }
}
