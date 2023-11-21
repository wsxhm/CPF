using CPF.Controls;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 使用 UIElement 绘制区域。
    /// </summary>
    public class VisualFill : TextureFill
    {
        public VisualFill()
        {

        }
        public VisualFill(UIElement element)
        {
            Element = element;
        }

        public UIElement Element
        {
            get { return GetValue<UIElement>(); }
            set { SetValue(value); }
        }

        [PropertyMetadata((uint)10)]
        public uint Width
        {
            get { return GetValue<uint>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata((uint)10)]
        public uint Height
        {
            get { return GetValue<uint>(); }
            set { SetValue(value); }
        }
        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Width) || propertyName == nameof(Height))
            {
                value = Math.Max(1, (uint)value);
            }
            return base.OnSetValue(propertyName, ref value);
        }
        //View root;
        Bitmap bitmap;

        public override Brush CreateBrush(in Rect rect, in float renderScaling)
        {
            var ele = Element;
            if (ele != null)
            {
                if (ele.Root == null)
                {

                }
                else
                {
                    var w = Width * renderScaling;
                    var h = Height * renderScaling;
                    if (bitmap == null || w != bitmap.Width || h != bitmap.Height)
                    {
                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                        }
                        bitmap = new Bitmap((int)Math.Ceiling(w), (int)Math.Ceiling(h));
                    }
                    using (DrawingContext dc = DrawingContext.FromBitmap(bitmap))
                    {
                        dc.Clear(Color.Transparent);
                        Matrix m = dc.Transform;
                        m.ScalePrepend(renderScaling, renderScaling);
                        dc.Transform = m;
                        ele.Render(dc);
                    }
                    Image = bitmap;
                }
            }
            return base.CreateBrush(rect, renderScaling);
        }

        protected override void OverrideMatrix(ref Matrix matrix, in float renderScaling)
        {
            matrix.Scale(1 / renderScaling, 1 / renderScaling);
        }

        /// <summary>
        /// 不要设置
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image Image { get => base.Image; set => base.Image = value; }

        protected override void Dispose(bool disposing)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            base.Dispose(disposing);
        }
    }
}
