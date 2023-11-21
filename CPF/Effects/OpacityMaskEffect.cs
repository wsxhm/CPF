using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Effects
{
    /// <summary>
    /// 透明遮罩
    /// </summary>
    public class OpacityMaskEffect : Effect
    {
        /// <summary>
        /// 透明遮罩
        /// </summary>
        public OpacityMaskEffect(Bitmap mask)
        {
            MaskImage = mask;
        }
        /// <summary>
        /// 遮罩的图片
        /// </summary>
        public Bitmap MaskImage
        {
            get { return GetValue<Bitmap>(); }
            set { SetValue(value); }
        }
        [PropertyChanged(nameof(MaskImage))]
        void OnMaskImage(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (newValue == null)
            {
                throw new Exception("MaskImage不能为null");
            }
        }

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            var img = MaskImage;
            using (var l = bitmap.Lock())
            {
                using (var mask = img.Lock())
                {
                    var w = bitmap.Width;
                    var h = bitmap.Height;

                    for (int x = 0; x < w; x++)
                    {
                        for (int y = 0; y < h; y++)
                        {

                        }
                    }
                }
            }
            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            dc.DrawImage(bitmap, rect, rect);
        }
    }
}
