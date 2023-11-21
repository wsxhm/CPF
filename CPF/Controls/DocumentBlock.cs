using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Documents;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 简单的文档控件，支持图片字体控件等元素布局，支持每个字符设置样式
    /// </summary>
    [Description("简单的文档控件，支持图片字符控件等元素布局，支持每个字符设置样式")]
    public class DocumentBlock : UIElement, IDocumentStyle
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
        /// <summary>
        /// 字体名称
        /// </summary>
        [UIPropertyMetadata("宋体", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public string FontFamily
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 字体尺寸
        /// </summary>
        [UIPropertyMetadata(12f, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public float FontSize
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        [UIPropertyMetadata(FontStyles.Regular, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public FontStyles FontStyle
        {
            get { return GetValue<FontStyles>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
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
        /// <summary>
        /// 获取或设置显示的文本，如果设置Document里的内容了，Text属性值不会自动变化
        /// </summary>
        [UIPropertyMetadata("", UIPropertyOptions.AffectsMeasure)]
        public string Text
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        Document document;
        public Document Document
        {
            get
            {
                if (document == null)
                {
                    document = new Document(this);
                }
                return document;
            }
        }

        /// <summary>
        /// 样式
        /// </summary>
        [NotCpfProperty]
        public Collection<DocumentStyle> Styles
        {
            get { return Document.Styles; }
        }

        [PropertyChanged(nameof(Text))]
        void RegisterText(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            Document.Children.Clear();
            if (newValue != null)
            {
                Document.Add(newValue.ToString());
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(Text))
        //    {
        //        Document.Children.Clear();
        //        if (newValue != null)
        //        {
        //            Document.Add(newValue.ToString());
        //        }
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        protected override Size MeasureOverride(in Size availableSize)
        {
            //var size = base.MeasureOverride(availableSize);
            if (document != null)
            {
                using (var font = new Font(FontFamily, FontSize, FontStyle))
                {
                    //var w = Width;
                    //if (!w.IsAuto && w.Unit == Unit.Default && MaxWidth.IsAuto && MinWidth.IsAuto)
                    //{
                    //    document.Arrange(font, availableSize);
                    //    return document.Size;
                    //}
                    document.Arrange(font, availableSize);
                    //if (document.WordWarp)
                    //{
                    //    document.IsMeasureValid = false;
                    //}
                    return document.Size;
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            if (document != null)
            {
                //var rect = GetArrangeRect(finalSize);
                using (var font = new Font(FontFamily, FontSize, FontStyle))
                {
                    //document.Arrange(font, finalSize);
                    foreach (var child in document.UIContainers)
                    {
                        child.UIElement.Arrange(new Rect(child.ActualPositon, new Size(child.Width, child.Height)));
                    }
                    //if (document.WordWarp)
                    //{
                    //    if (Height.IsAuto && (MarginTop.IsAuto || MarginBottom.IsAuto))
                    //    {
                    //        return new Size(finalSize.Width, document.Size.Height);
                    //    }
                    //}
                    return finalSize;

                }
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            //using (SolidColorBrush sb = new SolidColorBrush(Color.Black))
            //{
            //    dc.DrawRectangle(sb, new Stroke(1), new Rect(new Point(1, 1), new Size(ActualSize.Width - 3, ActualSize.Height - 3)));
            //}
            var back = Background;
            var size = ActualSize;
            if (back != null && size.Width > 0 && size.Height > 0)
            {
                using (var b = back.CreateBrush(new Rect(new Point(), size), Root.RenderScaling))
                {
                    dc.FillRectangle(b, new Rect(0, 0, size.Width, size.Height));
                }
            }
            //base.OnRender(dc);
            if (document != null)
            {
                document.Render(dc, new Rect(new Point(), size));
            }
        }

        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
        //    base.OnMouseEnter(e);
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ClipToBounds), new PropertyMetadataAttribute(true)); overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
        }
    }
}
