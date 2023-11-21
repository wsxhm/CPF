using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Effects
{
    /// <summary>
    /// 浮雕
    /// </summary>
    public class ReliefEffect : Effect
    {
        /// <summary>
        /// 浮雕
        /// </summary>
        public ReliefEffect() { }
        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            //确定图像的宽和高
            int height = bitmap.Height;
            int width = bitmap.Width;

            //LockBits将Bitmap锁定到内存中
            using (var lk = bitmap.Lock())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int r = 0, g = 0, b = 0;
                        lk.GetPixel(x, y, out byte aa, out byte rr, out byte gg, out byte bb);
                        if (x < width - 1 && y < height - 1)
                        {
                            //r = Math.Abs(p[0] - p[3 * (width + 1)] + 128);
                            //g = Math.Abs(p[1] - p[3 * (width + 1) + 1] + 128);
                            //b = Math.Abs(p[2] - p[3 * (width + 1) + 2] + 128);
                            lk.GetPixel(x + 1, y + 1, out byte aaa, out byte rrr, out byte ggg, out byte bbb);
                            b = Math.Abs(rr - rrr + 128);
                            g = Math.Abs(gg - ggg + 128);
                            r = Math.Abs(bb - bbb + 128);
                        }
                        else
                        {
                            r = 128;
                            g = 128;
                            b = 128;
                        }

                        if (r > 255) r = 255;
                        if (r < 0) r = 0;
                        if (g > 255) g = 255;
                        if (g < 0) g = 0;
                        if (b > 255) b = 255;
                        if (b < 0) b = 0;
                        //p[0] = (byte)r;
                        //p[1] = (byte)g;
                        //p[2] = (byte)b;
                        lk.SetPixel(x, y, aa, (byte)r, (byte)g, (byte)b);
                    }
                }
            }

            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            dc.DrawImage(bitmap, rect, rect);
        }
    }
}
