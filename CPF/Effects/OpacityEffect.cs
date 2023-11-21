using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Effects
{
    /// <summary>
    /// 调整控件透明度
    /// </summary>
    public class OpacityEffect : Effect
    {
        /// <summary>
        /// 不透明度0-1
        /// </summary>
        [PropertyMetadata(1f)]
        public float Opacity { get { return GetValue<float>(); } set { SetValue(value); } }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Opacity))
            {
                var o = (float)value;
                o = Math.Min(o, 1);
                o = Math.Max(0, o);
                value = o;
            }
            return base.OnSetValue(propertyName, ref value);
        }

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            var o = Opacity;
            //using (var l = bitmap.Lock())
            //{
            //    var w = Math.Min(rect.X + rect.Width, bitmap.Width);
            //    var h = Math.Min(rect.Y + rect.Height, bitmap.Height);

            //    for (int x = (int)rect.X; x < w; x++)
            //    {
            //        for (int y = (int)rect.Y; y < h; y++)
            //        {
            //            l.GetPixel(x, y,out byte a,out byte r,out byte g,out byte b);
            //            a = (byte)(a * o);
            //            l.SetPixel(x, y, ref a,ref r,ref g,ref b);
            //        }
            //    }
            //}
            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            dc.DrawImage(bitmap, rect, rect, o);
        }

    }
}
