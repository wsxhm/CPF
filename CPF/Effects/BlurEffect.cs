using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Effects
{
    /// <summary>
    /// 模糊特效
    /// </summary>
    public class BlurEffect : Effect
    {
        /// <summary>
        /// 模糊特效
        /// </summary>
        public BlurEffect()
        { }

        /// <summary>
        /// 模糊半径
        /// </summary>
        [PropertyMetadata((uint)2)]
        public uint BlurRadius
        {
            get { return GetValue<uint>(); }
            set { SetValue(value); }
        }

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            //var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            //using (Bitmap bmp = GaussianBlur(bitmap, (int)BlurRadius, rect))
            //using (Bitmap bmp = Convolute(bitmap, KernelGaussianBlur5x5))
            //using (Bitmap bmp = GaussianBlurShadow(bitmap, (int)BlurRadius, Color.BurlyWood, rect))
            //using (Bitmap bmp = StackBlur.Process(bitmap, (int)BlurRadius))
            //{
            //    StackBlur.ProcessOwner(bitmap, (int)BlurRadius);
            //    dc.DrawImage(bitmap, rect, rect);
            //}
            var radius = BlurRadius;
            var multiple = 1;
            if (radius >= 4 && (bitmap.Width > 100 || bitmap.Height > 100))
            {
                multiple = 2;
                if (radius >= 8 && (bitmap.Width > 400 || bitmap.Height > 400))
                {
                    multiple = 4;
                }
                if (radius >= 16 && (bitmap.Width > 800 || bitmap.Height > 800))
                {
                    multiple = 8;
                }
            }
            if (multiple == 1)
            {
                var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
                StackBlur.ProcessOwner(bitmap, (int)radius);
                dc.DrawImage(bitmap, rect, rect);
            }
            else
            {
                var rect = new Rect(0, 0, Math.Max(1, bitmap.Width / multiple), Math.Max(1, bitmap.Height / multiple));
                using (Bitmap source = new Bitmap((int)rect.Width, (int)rect.Height))
                {
                    using (var dcc = DrawingContext.FromBitmap(source))
                    {
                        dcc.Clear(Color.Transparent);
                        dcc.AntialiasMode = AntialiasMode.AntiAlias;
                        dcc.DrawImage(bitmap, rect, new Rect(0, 0, bitmap.Width, bitmap.Height));
                    }
                    StackBlur.ProcessOwner(source, (int)Math.Ceiling((double)(BlurRadius / multiple)));
                    var old = dc.AntialiasMode;
                    dc.AntialiasMode = AntialiasMode.AntiAlias;
                    dc.DrawImage(source, new Rect(0, 0, bitmap.Width, bitmap.Height), rect);
                    dc.AntialiasMode = old;
                }
            }
        }

        //public override Rect OverrideRenderRect(Rect rect)
        //{
        //    var br = BlurRadius;
        //    return new Rect(rect.X - br, rect.Y - br, rect.Width + br * 2, rect.Height + br * 2);
        //}

        private static int[] CreateGaussianBlurRow(int amount)
        {
            int size = 1 + (amount * 2);
            int[] weights = new int[size];

            for (int i = 0; i <= amount; ++i)
            {
                // 1 + aa - aa + 2ai - ii
                weights[i] = 16 * (i + 1);
                weights[weights.Length - i - 1] = weights[i];
            }

            return weights;
        }
        /// <summary>
        /// 高斯模糊，指针实现
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="amount">模糊半径</param>
        /// <param name="rect">处理区域</param>
        /// <returns></returns>
        public static unsafe Bitmap GaussianBlur(Bitmap bmp, int amount, Rect rect)
        {
            Bitmap des = new Bitmap(bmp.Width, bmp.Height);
            int[] w = CreateGaussianBlurRow(amount);
            int wlen = w.Length;
            int top = (int)rect.Top;
            int left = (int)rect.Left;
            int width = bmp.Width;
            int height = bmp.Height;
            if (rect.Height >= 1 && rect.Width >= 1)
            {
                using (var s = bmp.Lock())
                {
                    using (var d = des.Lock())
                    {
                        for (int y = top; y < rect.Bottom; ++y)
                        {
                            long[] waSums = new long[wlen];
                            long[] wcSums = new long[wlen];
                            long[] aSums = new long[wlen];
                            long[] bSums = new long[wlen];
                            long[] gSums = new long[wlen];
                            long[] rSums = new long[wlen];
                            long waSum = 0;
                            long wcSum = 0;
                            long aSum = 0;
                            long bSum = 0;
                            long gSum = 0;
                            long rSum = 0;

                            //byte* dstPtr =
                            d.GetPixel(left, y, out byte a, out byte r, out byte g, out byte b);
                            int yy = y;
                            int xx = left;

                            for (int wx = 0; wx < wlen; ++wx)
                            {
                                int srcX = left + wx - amount;
                                waSums[wx] = 0;
                                wcSums[wx] = 0;
                                aSums[wx] = 0;
                                bSums[wx] = 0;
                                gSums[wx] = 0;
                                rSums[wx] = 0;

                                if (srcX >= 0 && srcX < width)
                                {
                                    for (int wy = 0; wy < wlen; ++wy)
                                    {
                                        int srcY = y + wy - amount;

                                        if (srcY >= 0 && srcY < height)
                                        {
                                            //byte* c = s[srcX, srcY];
                                            s.GetPixel(srcX, srcY, out byte aa, out byte rr, out byte gg, out byte bb);
                                            int wp = w[wy];

                                            waSums[wx] += wp;
                                            wp *= aa + (aa >> 7);
                                            wcSums[wx] += wp;
                                            wp >>= 8;

                                            aSums[wx] += wp * aa;
                                            bSums[wx] += wp * bb;
                                            gSums[wx] += wp * gg;
                                            rSums[wx] += wp * rr;
                                        }
                                    }

                                    int wwx = w[wx];
                                    waSum += wwx * waSums[wx];
                                    wcSum += wwx * wcSums[wx];
                                    aSum += wwx * aSums[wx];
                                    bSum += wwx * bSums[wx];
                                    gSum += wwx * gSums[wx];
                                    rSum += wwx * rSums[wx];
                                }
                            }

                            wcSum >>= 8;

                            if (waSum == 0 || wcSum == 0)
                            {
                                a = 0;
                                r = 0;
                                g = 0;
                                b = 0;
                            }
                            else
                            {
                                a = (byte)(aSum / waSum);
                                b = (byte)(bSum / wcSum);
                                g = (byte)(gSum / wcSum);
                                r = (byte)(rSum / wcSum);

                                //b = blue;
                                //g = green;
                                //r = red;
                                //a = alpha;
                            }
                            d.SetPixel(xx, yy, a, r, g, b);
                            if (xx >= width)
                            {
                                yy += 1;
                                xx = 0;
                            }
                            else
                            {
                                xx += 1;
                            }
                            //dstPtr += 4;

                            for (int x = left + 1; x < rect.Right; ++x)
                            {
                                for (int i = 0; i < wlen - 1; ++i)
                                {
                                    waSums[i] = waSums[i + 1];
                                    wcSums[i] = wcSums[i + 1];
                                    aSums[i] = aSums[i + 1];
                                    bSums[i] = bSums[i + 1];
                                    gSums[i] = gSums[i + 1];
                                    rSums[i] = rSums[i + 1];
                                }

                                waSum = 0;
                                wcSum = 0;
                                aSum = 0;
                                bSum = 0;
                                gSum = 0;
                                rSum = 0;

                                int wx;
                                for (wx = 0; wx < wlen - 1; ++wx)
                                {
                                    long wwx = w[wx];
                                    waSum += wwx * waSums[wx];
                                    wcSum += wwx * wcSums[wx];
                                    aSum += wwx * aSums[wx];
                                    bSum += wwx * bSums[wx];
                                    gSum += wwx * gSums[wx];
                                    rSum += wwx * rSums[wx];
                                }

                                wx = wlen - 1;

                                waSums[wx] = 0;
                                wcSums[wx] = 0;
                                aSums[wx] = 0;
                                bSums[wx] = 0;
                                gSums[wx] = 0;
                                rSums[wx] = 0;

                                int srcX = x + wx - amount;

                                if (srcX >= 0 && srcX < width)
                                {
                                    for (int wy = 0; wy < wlen; ++wy)
                                    {
                                        int srcY = y + wy - amount;

                                        if (srcY >= 0 && srcY < height)
                                        {
                                            //byte* c = s[srcX, srcY];
                                            s.GetPixel(srcX, srcY, out byte aa, out byte rr, out byte gg, out byte bb);
                                            int wp = w[wy];

                                            waSums[wx] += wp;
                                            wp *= aa + (aa >> 7);
                                            wcSums[wx] += wp;
                                            wp >>= 8;

                                            aSums[wx] += wp * (long)aa;
                                            bSums[wx] += wp * (long)bb;
                                            gSums[wx] += wp * (long)gg;
                                            rSums[wx] += wp * (long)rr;
                                        }
                                    }

                                    int wr = w[wx];
                                    waSum += wr * waSums[wx];
                                    wcSum += wr * wcSums[wx];
                                    aSum += wr * aSums[wx];
                                    bSum += wr * bSums[wx];
                                    gSum += wr * gSums[wx];
                                    rSum += wr * rSums[wx];
                                }

                                wcSum >>= 8;

                                if (waSum == 0 || wcSum == 0)
                                {
                                    a = 0;
                                    r = 0;
                                    g = 0;
                                    b = 0;
                                }
                                else
                                {
                                    a = (byte)(aSum / waSum);
                                    b = (byte)(bSum / wcSum);
                                    g = (byte)(gSum / wcSum);
                                    r = (byte)(rSum / wcSum);

                                    //b = blue;
                                    //g = green;
                                    //r = red;
                                    //a = alpha;
                                }
                                d.SetPixel(xx, yy, a, r, g, b);
                                if (xx >= width)
                                {
                                    yy += 1;
                                    xx = 0;
                                }
                                else
                                {
                                    xx += 1;
                                }
                                //dstPtr += 4;
                            }
                        }
                    }
                }
            }
            return des;
        }
        /// <summary>
        /// 高斯模糊阴影，不透明度来区分边界
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="amount"></param>
        /// <param name="color"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static unsafe Bitmap GaussianBlurShadow(Bitmap bmp, int amount, Color color, Rect rect)
        {
            Bitmap des = new Bitmap(bmp.Width, bmp.Height);
            using (var dc = DrawingContext.FromBitmap(des))
            {
                dc.PushClip(rect);
                dc.Clear(color);
                dc.PopClip();
            }
            int[] w = CreateGaussianBlurRow(amount);
            int wlen = w.Length;
            int top = (int)rect.Top;
            int left = (int)rect.Left;
            int width = bmp.Width;
            int height = bmp.Height;
            if (rect.Height >= 1 && rect.Width >= 1)
            {
                using (var s = bmp.Lock())
                {
                    using (var d = des.Lock())
                    {
                        long[] waSums = new long[wlen];
                        long[] aSums = new long[wlen];
                        for (int y = top; y < rect.Bottom; ++y)
                        {
                            long waSum = 0;
                            long aSum = 0;

                            d.GetAlpha(left, y, out byte a);
                            int yy = y;
                            int xx = left;

                            for (int wx = 0; wx < wlen; ++wx)
                            {
                                int srcX = left + wx - amount;
                                waSums[wx] = 0;
                                aSums[wx] = 0;

                                if (srcX >= 0 && srcX < width)
                                {
                                    for (int wy = 0; wy < wlen; ++wy)
                                    {
                                        int srcY = y + wy - amount;

                                        if (srcY >= 0 && srcY < height)
                                        {
                                            s.GetAlpha(srcX, srcY, out byte aa);
                                            int wp = w[wy];

                                            waSums[wx] += wp;
                                            wp *= aa + (aa >> 7);
                                            wp >>= 8;

                                            aSums[wx] += wp * aa;
                                        }
                                    }

                                    int wwx = w[wx];
                                    waSum += wwx * waSums[wx];
                                    aSum += wwx * aSums[wx];
                                }
                            }

                            if (waSum == 0)
                            {
                                a = 0;
                            }
                            else
                            {
                                a = (byte)(aSum / waSum);
                            }
                            d.SetAlpha(xx, yy, a);
                            if (xx >= width)
                            {
                                yy += 1;
                                xx = 0;
                            }
                            else
                            {
                                xx += 1;
                            }

                            for (int x = left + 1; x < rect.Right; ++x)
                            {
                                for (int i = 0; i < wlen - 1; ++i)
                                {
                                    waSums[i] = waSums[i + 1];
                                    aSums[i] = aSums[i + 1];
                                }

                                waSum = 0;
                                aSum = 0;

                                int wx;
                                for (wx = 0; wx < wlen - 1; ++wx)
                                {
                                    long wwx = w[wx];
                                    waSum += wwx * waSums[wx];
                                    aSum += wwx * aSums[wx];
                                }

                                wx = wlen - 1;

                                waSums[wx] = 0;
                                aSums[wx] = 0;

                                int srcX = x + wx - amount;

                                if (srcX >= 0 && srcX < width)
                                {
                                    for (int wy = 0; wy < wlen; ++wy)
                                    {
                                        int srcY = y + wy - amount;

                                        if (srcY >= 0 && srcY < height)
                                        {
                                            s.GetAlpha(srcX, srcY, out byte aa);
                                            int wp = w[wy];

                                            waSums[wx] += wp;
                                            wp *= aa + (aa >> 7);
                                            wp >>= 8;

                                            aSums[wx] += wp * aa;
                                        }
                                    }

                                    int wr = w[wx];
                                    waSum += wr * waSums[wx];
                                    aSum += wr * aSums[wx];
                                }

                                if (waSum == 0)
                                {
                                    a = 0;
                                }
                                else
                                {
                                    a = (byte)(aSum / waSum);
                                }
                                d.SetAlpha(xx, yy, a);
                                if (xx >= width)
                                {
                                    yy += 1;
                                    xx = 0;
                                }
                                else
                                {
                                    xx += 1;
                                }
                            }
                        }
                    }
                }
            }
            return des;
        }

        #region Kernels

        ///<summary>
        /// Gaussian blur kernel with the size 5x5
        ///</summary>
        public static int[,] KernelGaussianBlur5x5 = {
                                                       {1,  4,  7,  4, 1},
                                                       {4, 16, 26, 16, 4},
                                                       {7, 26, 41, 26, 7},
                                                       {4, 16, 26, 16, 4},
                                                       {1,  4,  7,  4, 1}
                                                 };

        ///<summary>
        /// Gaussian blur kernel with the size 3x3
        ///</summary>
        public static int[,] KernelGaussianBlur3x3 = {
                                                       {16, 26, 16},
                                                       {26, 41, 26},
                                                       {16, 26, 16}
                                                    };

        ///<summary>
        /// Sharpen kernel with the size 3x3
        ///</summary>
        public static int[,] KernelSharpen3x3 = {
                                                 { 0, -2,  0},
                                                 {-2, 11, -2},
                                                 { 0, -2,  0}
                                              };

        #endregion

        #region Convolute

        /// <summary>
        /// 卷积计算
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="kernel">The kernel used for convolution.</param>
        /// <returns>A new Bitmap that is a filtered version of the input.</returns>
        public static Bitmap Convolute(Bitmap bmp, int[,] kernel)
        {
            var kernelFactorSum = 0;
            foreach (var b in kernel)
            {
                kernelFactorSum += b;
            }
            return Convolute(bmp, kernel, kernelFactorSum, 0);
        }

        /// <summary>
        /// Creates a new filtered WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The Bitmap.</param>
        /// <param name="kernel">The kernel used for convolution.</param>
        /// <param name="kernelFactorSum">The factor used for the kernel summing.</param>
        /// <param name="kernelOffsetSum">The offset used for the kernel summing.</param>
        /// <returns>A new Bitmap that is a filtered version of the input.</returns>
        public static unsafe Bitmap Convolute(Bitmap bmp, int[,] kernel, int kernelFactorSum, int kernelOffsetSum)
        {
            var kh = kernel.GetUpperBound(0) + 1;
            var kw = kernel.GetUpperBound(1) + 1;

            if ((kw & 1) == 0)
            {
                throw new InvalidOperationException("Kernel width must be odd!");
            }
            if ((kh & 1) == 0)
            {
                throw new InvalidOperationException("Kernel height must be odd!");
            }

            using (var srcContext = bmp.Lock())
            {
                var w = bmp.Width;
                var h = bmp.Height;
                var result = new Bitmap(w, h);

                using (var resultContext = result.Lock())
                {
                    var pixels = (int*)srcContext.DataPointer;
                    var resultPixels = (int*)resultContext.DataPointer;
                    var index = 0;
                    var kwh = kw >> 1;
                    var khh = kh >> 1;

                    for (var y = 0; y < h; y++)
                    {
                        for (var x = 0; x < w; x++)
                        {
                            var a = 0;
                            var r = 0;
                            var g = 0;
                            var b = 0;

                            for (var kx = -kwh; kx <= kwh; kx++)
                            {
                                var px = kx + x;
                                // Repeat pixels at borders
                                if (px < 0)
                                {
                                    px = 0;
                                }
                                else if (px >= w)
                                {
                                    px = w - 1;
                                }

                                for (var ky = -khh; ky <= khh; ky++)
                                {
                                    var py = ky + y;
                                    // Repeat pixels at borders
                                    if (py < 0)
                                    {
                                        py = 0;
                                    }
                                    else if (py >= h)
                                    {
                                        py = h - 1;
                                    }

                                    var col = pixels[py * w + px];
                                    var k = kernel[ky + kwh, kx + khh];
                                    a += ((col >> 24) & 0x000000FF) * k;
                                    r += ((col >> 16) & 0x000000FF) * k;
                                    g += ((col >> 8) & 0x000000FF) * k;
                                    b += ((col) & 0x000000FF) * k;
                                }
                            }

                            var ta = ((a / kernelFactorSum) + kernelOffsetSum);
                            var tr = ((r / kernelFactorSum) + kernelOffsetSum);
                            var tg = ((g / kernelFactorSum) + kernelOffsetSum);
                            var tb = ((b / kernelFactorSum) + kernelOffsetSum);

                            // Clamp to byte boundaries
                            var ba = (byte)((ta > 255) ? 255 : ((ta < 0) ? 0 : ta));
                            var br = (byte)((tr > 255) ? 255 : ((tr < 0) ? 0 : tr));
                            var bg = (byte)((tg > 255) ? 255 : ((tg < 0) ? 0 : tg));
                            var bb = (byte)((tb > 255) ? 255 : ((tb < 0) ? 0 : tb));

                            resultPixels[index++] = (ba << 24) | (br << 16) | (bg << 8) | (bb);
                        }
                    }
                    return result;
                }
            }
        }
        #endregion
    }
}
