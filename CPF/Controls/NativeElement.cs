using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 用于内嵌原生控件，一般来说原生控件无法使用渲染变换，无法调整ZIndex，只能在最前端，可能无法透明。一般尽可能少用该控件
    /// </summary>
    [Description("用于内嵌原生控件，一般来说原生控件无法使用渲染变换，无法调整ZIndex，只能在最前端，可能无法透明。一般尽可能少用该控件")]
    public class NativeElement : UIElement
    {
        /// <summary>
        /// 用于内嵌原生控件，一般来说原生控件无法使用渲染变换，无法调整ZIndex，只能在最前端，可能无法透明。一般尽可能少用该控件
        /// </summary>
        public NativeElement()
        { }
        INativeImpl nativeImpl;
        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(RenderTransform) || propertyName == nameof(Effect) || propertyName == nameof(ZIndex))
            {
                throw new NotSupportedException("不支持" + propertyName);
            }
            return base.OnSetValue(propertyName, ref value);
        }
        /// <summary>
        /// 设置对应平台的控件句柄
        /// </summary>
        [Description("设置对应平台的控件句柄")]
        public object Content
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 背景色，有些平台可能无法透明
        /// </summary>
        [Description("背景色，有些平台可能无法透明"), UIPropertyMetadata(typeof(Color), "#fff", UIPropertyOptions.AffectsRender)]
        public Color BackColor
        {
            get { return (Color)GetValue(); }
            set { SetValue(value); }
        }
        [Browsable(false)]
        public INativeImpl NativeImpl { get => nativeImpl; }

        [PropertyChanged(nameof(Content))]
        void OnContentChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            nativeImpl?.SetContent(newValue);
        }

        [PropertyChanged(nameof(BackColor))]
        void OnBackColorChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            nativeImpl?.SetBackColor((Color)newValue);
        }

        protected override void OnAttachedToVisualTree()
        {
            if (!DesignMode)
            {
                nativeImpl = Application.GetRuntimePlatform().CreateNative();
                nativeImpl.SetOwner(this);
                nativeImpl.SetContent(Content);
                nativeImpl.SetBackColor(BackColor);
                nativeImpl.SetParent(Root.ViewImpl);
                Root.LayoutManager.nativeElements.Add(this);
            }
            base.OnAttachedToVisualTree();
        }

        protected override void OnDetachedFromVisualTree()
        {
            nativeImpl?.SetParent(null);
            Root.LayoutManager.nativeElements.Remove(this);
            base.OnDetachedFromVisualTree();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (DesignMode)
            {
                using (var sb = new SolidColorBrush(BackColor))
                {
                    dc.FillRectangle(sb, new Rect(0, 0, ActualSize.Width, ActualSize.Height));
                }
            }
            base.OnRender(dc);
        }

        protected override void Dispose(bool disposing)
        {
            nativeImpl?.Dispose();
            base.Dispose(disposing);
        }
    }
}
