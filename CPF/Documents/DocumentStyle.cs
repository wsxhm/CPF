using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Documents
{
    /// <summary>
    /// 文档元素样式
    /// </summary>
    public class DocumentStyle : CpfObject, IDocumentStyle
    {
        /// <summary>
        /// 文档元素样式
        /// </summary>
        public DocumentStyle()
        {

        }
        /// <summary>
        /// 背景填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Background
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 字体名称
        /// </summary>
        [UIPropertyMetadata("宋体", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public string FontFamily
        {
            get { return (string)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 字体尺寸
        /// </summary>
        [UIPropertyMetadata(12f, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public float FontSize
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        [UIPropertyMetadata(FontStyles.Regular, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public FontStyles FontStyle
        {
            get { return (FontStyles)GetValue(); }
            set { SetValue(value); }
        }

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
        public IDocumentStyle Parent { get; internal set; }

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            //CpfObject p;
            //if (pm.PropertyName != nameof(Parent) && (pm is UIPropertyMetadataAttribute) && ((UIPropertyMetadataAttribute)pm).Inherits && (p = Parent as CpfObject) != null && p.HasProperty(pm.PropertyName))
            //{
            //    return p.GetValue(pm.PropertyName);
            //}
            if (Parent != null)
            {
                if (pm.PropertyName == nameof(Parent.FontFamily))
                {
                    return Parent.FontFamily;
                }
                if (pm.PropertyName == nameof(Parent.FontSize))
                {
                    return Parent.FontSize;
                }
                if (pm.PropertyName == nameof(Parent.FontStyle))
                {
                    return Parent.FontStyle;
                }
                if (pm.PropertyName == nameof(Parent.Foreground))
                {
                    return Parent.Foreground;
                }
                if (pm.PropertyName == nameof(Parent.Background))
                {
                    return Parent.Background;
                }
                if (pm.PropertyName == nameof(Parent.TextDecoration))
                {
                    return Parent.TextDecoration;
                }
            }
            return base.OnGetDefaultValue(pm);
        }
    }
}
