using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Effects
{
    /// <summary>
    /// 灰度图
    /// </summary>
    public class GrayScaleEffect : Effect
    {
        /// <summary>
        /// 灰度图
        /// </summary>
        public GrayScaleEffect() { }

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            GrayScale(bitmap);
            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            dc.DrawImage(bitmap, rect, rect);
        }

        /// <summary>
        /// 将图片转换成黑白色效果
        /// </summary>
        /// <param name="bmp">原图</param>
        public static unsafe void GrayScale(Bitmap bmp)
        {
            //确定图像的宽和高
            int height = bmp.Height;
            int width = bmp.Width;

            using (var l = bmp.Lock())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        l.GetPixel(x, y, out byte a, out byte r, out byte g, out byte b);
                        var p = (byte)Math.Min(255, 0.7 * r + (0.2 * g) + (0.1 * b));
                        l.SetPixel(x, y, a, p, p, p);
                    } // x
                } // y
            }
        }
    }
}
