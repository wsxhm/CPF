using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 内容模板
    /// </summary>
    [Description("内容模板"), Browsable(false)]
    public class ContentTemplate : Decorator
    {
        [PropertyMetadata(null)]
        public object Content
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        public string ContentStringFormat
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        protected override void InitializeComponent()
        {
            Action<object, object> action = (content, old) =>
             {
                 if (old != null && content != null && old.GetType() == content.GetType())
                 {//如果类型不变就不用重新创建元素
                     return;
                 }
                 if (content != null && !(content is UIElement) && !(content is Image))
                 {
                     Child = new TextBlock
                     {
                         Bindings =
                         {
                            {
                                nameof(TextBlock.Text),
                                nameof(Content),
                                1,
                                BindingMode.OneWay,
                                Convert
                            }
                         }
                     };
                 }
                 else if (content is Image image)
                 {
                     Child = new Picture
                     {
                         MaxHeight = "100%",
                         MaxWidth = "100%",
                         StretchDirection = StretchDirection.DownOnly,
                         Stretch = Stretch.Uniform,
                         Bindings =
                         {
                            {
                                nameof(Picture.Source),
                                nameof(Content),
                                1,
                                BindingMode.OneWay
                            }
                         }
                     };
                 }
             };
            if (Content != null)
            {
                action(Content, null);
            }
            Commands.Add(nameof(Content), (s, e) => action(((CPFPropertyChangedEventArgs)e).NewValue, ((CPFPropertyChangedEventArgs)e).OldValue));
        }

        protected object Convert(object a)
        {
            var v = a == null ? "" : (string.IsNullOrWhiteSpace(ContentStringFormat) ? a.ToString() : string.Format(ContentStringFormat, a.ToString()));
            return v;
        }
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Width), new UIPropertyMetadataAttribute(typeof(FloatField), "100%", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute(typeof(FloatField), "100%", UIPropertyOptions.AffectsMeasure));
        }
        //protected override Size MeasureOverride(in Size availableSize)
        //{
        //    return base.MeasureOverride(availableSize);
        //}
    }
}
