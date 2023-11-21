using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Documents;
using CPF.Input;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 文本显示
    /// </summary>
    [Description("文本显示")]
    [DefaultProperty(nameof(TextBlock.Text))]
    public class TextBlock : UIElement
    {
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        [UIPropertyMetadata(TextAlignment.Left, UIPropertyOptions.AffectsRender)]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(51); }
            set { SetValue(value, 51); }
        }

        /// <summary>
        /// 背景填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Background
        {
            get { return (ViewFill)GetValue(45); }
            set { SetValue(value, 45); }
        }
        /// <summary>
        /// 字体名称
        /// </summary>
        [UIPropertyMetadata("宋体", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public string FontFamily
        {
            get { return GetValue<string>(46); }
            set { SetValue(value, 46); }
        }

        /// <summary>
        /// 字体尺寸
        /// </summary>
        [UIPropertyMetadata(12f, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public float FontSize
        {
            get { return GetValue<float>(47); }
            set { SetValue(value, 47); }
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        [UIPropertyMetadata(FontStyles.Regular, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits)]
        public FontStyles FontStyle
        {
            get { return GetValue<FontStyles>(48); }
            set { SetValue(value, 48); }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public ViewFill Foreground
        {
            get { return (ViewFill)GetValue(49); }
            set { SetValue(value, 49); }
        }
        /// <summary>
        /// 文字描边
        /// </summary>
        [Description("文字描边")]
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill TextStrokeFill
        {
            get { return (ViewFill)GetValue(55); }
            set { SetValue(value, 55); }
        }

        /// <summary>
        /// 文字描边
        /// </summary>
        [Description("文字描边")]
        [UIPropertyMetadata(typeof(Stroke), "0", UIPropertyOptions.AffectsRender)]
        public Stroke TextStroke
        {
            get { return (Stroke)GetValue(54); }
            set { SetValue(value, 54); }
        }
        /// <summary>
        /// 文本裁剪
        /// </summary>
        [Description("文本裁剪")]
        [UIPropertyMetadata(TextTrimming.None, UIPropertyOptions.AffectsRender)]
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(56); }
            set { SetValue(value, 56); }
        }
        /// <summary>
        /// 文本在垂直方向的对齐方式
        /// </summary>
        [Description("文本在垂直方向的对齐方式")]
        [UIPropertyMetadata(VerticalAlignment.Top, UIPropertyOptions.AffectsRender)]
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(57); }
            set { SetValue(value, 57); }
        }

        [UIPropertyMetadata("", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public string Text
        {
            get { return (string)GetValue(50); }
            set { SetValue(value, 50); }
        }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// </summary>
        [UIPropertyMetadata(typeof(TextDecoration), "", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        public TextDecoration TextDecoration
        {
            get { return (TextDecoration)GetValue(52); }
            set { SetValue(value, 52); }
        }

        [NotCpfProperty]
        public bool UseEllipsisToolTip { get; set; }
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(FontSize))
        //    {

        //    }
        //}



        protected override void OnRender(DrawingContext dc)
        {
            var back = Background;
            var size = ActualSize;
            //var bs = BorderStroke;
            //var dpi = Root.Scaling;
            //bs.Width = (float)Math.Round(dpi * bs.Width) / dpi;
            if (back != null && size.Width > 0 && size.Height > 0)
            {
                using (var b = back.CreateBrush(new Rect(new Point(), size), Root.RenderScaling))
                {
                    var w = size.Width;// - bs.Width - bs.Width;
                    var h = size.Height;// - bs.Width - bs.Width;
                    if (w > 0 && h > 0)
                    {
                        dc.FillRectangle(b, new Rect(0, 0, w, h));
                    }
                }
            }
            base.OnRender(dc);
            var fore = Foreground;
            var text = Text;
            if (fore != null && !string.IsNullOrEmpty(text))
            {
                var textStrokeFill = TextStrokeFill;
                Brush textStrokeBrush = null;
                if (textStrokeFill != null)
                {
                    textStrokeBrush = textStrokeFill.CreateBrush(new Rect(new Point(), size), Root.RenderScaling);
                }
                using (var b = fore.CreateBrush(new Rect(new Point(), size), Root.RenderScaling))
                {
                    var y = 0f;
                    var font = new Font(FontFamily, FontSize, FontStyle);
                    Size s;
                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            s = DrawingFactory.Default.MeasureString(text, font, size.Width);
                            if (TextTrimming == TextTrimming.CharacterEllipsis)
                            {
                                var fh = font.DefaultLineHeight;
                                if (size.Height > fh && s.Height > size.Height && size.Height % fh > 0.001)
                                {
                                    s.Height = (int)(size.Height / fh) * fh;
                                }
                            }
                            y = Math.Max(0, (size.Height - s.Height) / 2);
                            break;
                        case VerticalAlignment.Bottom:
                            s = DrawingFactory.Default.MeasureString(text, font, size.Width);
                            if (TextTrimming == TextTrimming.CharacterEllipsis)
                            {
                                var fh = font.DefaultLineHeight;
                                if (size.Height > fh && s.Height > size.Height && size.Height % fh > 0.001)
                                {
                                    s.Height = (int)(size.Height / fh) * fh;
                                }
                            }
                            y = Math.Max(0, size.Height - s.Height);
                            break;
                    }
                    //dc.DrawString(new Point(0, y), b, text, font, TextAlignment, size.Width, TextDecoration, size.Height, TextTrimming, TextStroke, textStrokeBrush);
                    bool ellipsis = false;
                    dc.DrawString(out ellipsis, new Point(0, y), b, text, font, TextAlignment, size.Width, TextDecoration, size.Height, TextTrimming, TextStroke, textStrokeBrush);
                    if (UseEllipsisToolTip)
                    {
                        if (ellipsis)
                            ToolTip = text;
                        else
                            ToolTip = null;
                    }
                }
                textStrokeBrush?.Dispose();
            }

        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}
        protected override Size MeasureOverride(in Size availableSize)
        {
            var text = Text;
            var maxW = MaxWidth;
            if (!string.IsNullOrEmpty(text))
            {
                using (var f = new Font(FontFamily, FontSize, FontStyle))
                {
                    Size textSize;
                    if (!float.IsNaN(availableSize.Width))
                    {
                        if (!maxW.IsAuto)
                        {
                            textSize = Platform.Application.GetDrawingFactory().MeasureString(text, f, Math.Max(Math.Min(maxW.GetActualValue(availableSize.Width), availableSize.Width), 1));
                        }
                        else
                        {
                            textSize = Platform.Application.GetDrawingFactory().MeasureString(text, f, Math.Max(availableSize.Width, 1));
                        }
                    }
                    else if (!maxW.IsAuto && maxW.Unit == Unit.Default)
                    {
                        textSize = Platform.Application.GetDrawingFactory().MeasureString(text, f, Math.Max(maxW.Value, 1));
                    }
                    else
                    {
                        textSize = Platform.Application.GetDrawingFactory().MeasureString(text, f);
                    }
                    return textSize;
                }
            }
            return base.MeasureOverride(availableSize);
        }

        Size textSize;
        /// <summary>
        /// 布局后的文本尺寸
        /// </summary>
        public Size TextSize
        {
            get { return textSize; }
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var text = Text;
            if (!string.IsNullOrEmpty(text))
            {
                using (var f = new Font(FontFamily, FontSize, FontStyle))
                {
                    textSize = Platform.Application.GetDrawingFactory().MeasureString(text, f, Math.Max(finalSize.Width, 1));
                    return textSize;
                }
            }
            else
            {
                return base.ArrangeOverride(finalSize);
            }
        }


        //protected override void OnLayoutUpdated()
        //{
        //    base.OnLayoutUpdated();
        //    var aS = ActualSize;
        //    if (textSize.Width > aS.Width || textSize.Height > aS.Height)
        //    {
        //        ToolTip = Text;
        //    }
        //    else
        //    {
        //        ToolTip = null;
        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
        }
    }
    /// <summary>
    /// 描述内容在垂直反向的对齐方式
    /// </summary>
    public enum VerticalAlignment : byte
    {
        Top,
        Center,
        Bottom
    }
}
