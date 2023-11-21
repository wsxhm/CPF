using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CPF.Drawing;
using System.Threading.Tasks;

namespace CPF.Effects
{
    internal class BlurStack
    {
        internal byte R;

        internal byte G;

        internal byte B;

        internal byte A;

        internal BlurStack Next;
    }

    /// <summary> 
    /// 高斯模糊高性能算法
    /// </summary>
    public static class StackBlur
    {
        private static readonly int[] MulTable =
        {
            512, 512, 456, 512, 328, 456, 335, 512, 405, 328, 271, 456, 388, 335, 292, 512,
            454, 405, 364, 328, 298, 271, 496, 456, 420, 388, 360, 335, 312, 292, 273, 512,
            482, 454, 428, 405, 383, 364, 345, 328, 312, 298, 284, 271, 259, 496, 475, 456,
            437, 420, 404, 388, 374, 360, 347, 335, 323, 312, 302, 292, 282, 273, 265, 512,
            497, 482, 468, 454, 441, 428, 417, 405, 394, 383, 373, 364, 354, 345, 337, 328,
            320, 312, 305, 298, 291, 284, 278, 271, 265, 259, 507, 496, 485, 475, 465, 456,
            446, 437, 428, 420, 412, 404, 396, 388, 381, 374, 367, 360, 354, 347, 341, 335,
            329, 323, 318, 312, 307, 302, 297, 292, 287, 282, 278, 273, 269, 265, 261, 512,
            505, 497, 489, 482, 475, 468, 461, 454, 447, 441, 435, 428, 422, 417, 411, 405,
            399, 394, 389, 383, 378, 373, 368, 364, 359, 354, 350, 345, 341, 337, 332, 328,
            324, 320, 316, 312, 309, 305, 301, 298, 294, 291, 287, 284, 281, 278, 274, 271,
            268, 265, 262, 259, 257, 507, 501, 496, 491, 485, 480, 475, 470, 465, 460, 456,
            451, 446, 442, 437, 433, 428, 424, 420, 416, 412, 408, 404, 400, 396, 392, 388,
            385, 381, 377, 374, 370, 367, 363, 360, 357, 354, 350, 347, 344, 341, 338, 335,
            332, 329, 326, 323, 320, 318, 315, 312, 310, 307, 304, 302, 299, 297, 294, 292,
            289, 287, 285, 282, 280, 278, 275, 273, 271, 269, 267, 265, 263, 261, 259
        };

        private static readonly int[] ShgTable =
        {
            9, 11, 12, 13, 13, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 17,
            17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19,
            19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20,
            20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 21,
            21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
            21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22,
            22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
            22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 23,
            23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
            23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
            23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
            23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24
        };

        ///// <summary>
        ///// 透明图片支持不行，会有灰色边缘 https://github.com/fossabot/StackBlur/blob/master/StackBlur/StackBlur.cs
        ///// </summary>
        ///// <param name="bitmap">The bitmap to process.</param>
        ///// <param name="radius">Gaussian blur radius.</param>
        //public static unsafe Bitmap Process(Bitmap bitmap, int radius)
        //{
        //    var width = bitmap.Width;
        //    var height = bitmap.Height;

        //    var div = radius + radius + 1;
        //    var widthMinus1 = width - 1;
        //    var heightMinus1 = height - 1;
        //    var radiusPlus1 = radius + 1;
        //    var sumFactor = radiusPlus1 * (radiusPlus1 + 1) / 2;

        //    var stack = new BlurStack();
        //    var stackStart = stack;
        //    var stackEnd = stack;

        //    for (var i = 1; i < div; i++)
        //    {
        //        stack = stack.Next = new BlurStack();
        //        if (i == radiusPlus1)
        //        {
        //            stackEnd = stack;
        //        }
        //    }

        //    stack.Next = stackStart;

        //    Debug.Assert(stackEnd != null);

        //    var yw = 0;
        //    var yi = 0;

        //    var mulSum = MulTable[radius];
        //    var shgSum = ShgTable[radius];

        //    //var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
        //    var result = (Bitmap)bitmap.Clone();
        //    var scan = bitmap.Width * 4;
        //    //using (var data = bitmap.Lock())
        //    //{
        //    using (var r = result.Lock())
        //    {
        //        //var pixels = (byte*)r.DataPointer;

        //        for (var y = 0; y < height; y++)
        //        {
        //            var rInSum = 0;
        //            var gInSum = 0;
        //            var bInSum = 0;
        //            var aInSum = 0;

        //            var rSum = 0;
        //            var gSum = 0;
        //            var bSum = 0;
        //            var aSum = 0;

        //            r.GetPixel(yi % scan / 4, yi / scan, out var pa, out var pr, out var pg, out var pb);

        //            var rOutSum = radiusPlus1 * pr;
        //            var gOutSum = radiusPlus1 * pg;
        //            var bOutSum = radiusPlus1 * pb;
        //            var aOutSum = radiusPlus1 * pa;

        //            rSum += sumFactor * pr;
        //            gSum += sumFactor * pg;
        //            bSum += sumFactor * pb;
        //            aSum += sumFactor * pa;

        //            stack = stackStart;

        //            for (var i = 0; i < radiusPlus1; i++)
        //            {
        //                stack.R = pr;
        //                stack.G = pg;
        //                stack.B = pb;
        //                stack.A = pa;
        //                stack = stack.Next;
        //            }

        //            for (var i = 1; i < radiusPlus1; i++)
        //            {
        //                var p = yi + ((widthMinus1 < i ? widthMinus1 : i) << 2);
        //                var rbs = radiusPlus1 - i;
        //                r.GetPixel(p % scan / 4, p / scan, out var aa, out var rr, out var gg, out var bb);
        //                rSum += (stack.R = pr = rr) * rbs;
        //                gSum += (stack.G = pg = gg) * rbs;
        //                bSum += (stack.B = pb = bb) * rbs;
        //                aSum += (stack.A = pa = aa) * rbs;

        //                rInSum += pr;
        //                gInSum += pg;
        //                bInSum += pb;
        //                aInSum += pa;

        //                stack = stack.Next;
        //            }

        //            var stackIn = stackStart;
        //            var stackOut = stackEnd;

        //            for (var x = 0; x < width; x++)
        //            {
        //                //pixels[yi + 3] = pa;
        //                //if (pa != 0)
        //                //{
        //                r.SetPixel(yi % scan / 4, yi / scan, (byte)((aSum * mulSum) >> shgSum), (byte)(((rSum * mulSum) >> shgSum)), (byte)(((gSum * mulSum) >> shgSum)), (byte)(((bSum * mulSum) >> shgSum)));
        //                //}
        //                //else
        //                //{
        //                //    r.SetPixel(yi % scan / 4, yi / scan, 0, 0, 0, 0);
        //                //}

        //                rSum -= rOutSum;
        //                gSum -= gOutSum;
        //                bSum -= bOutSum;
        //                aSum -= aOutSum;

        //                rOutSum -= stackIn.R;
        //                gOutSum -= stackIn.G;
        //                bOutSum -= stackIn.B;
        //                aOutSum -= stackIn.A;

        //                var p = x + radius + 1;
        //                p = (yw + (p < widthMinus1 ? p : widthMinus1)) << 2;

        //                r.GetPixel(p % scan / 4, p / scan, out var aa, out var rr, out var gg, out var bb);
        //                rInSum += stackIn.R = rr;
        //                gInSum += stackIn.G = gg;
        //                bInSum += stackIn.B = bb;
        //                aInSum += stackIn.A = aa;

        //                rSum += rInSum;
        //                gSum += gInSum;
        //                bSum += bInSum;
        //                aSum += aInSum;

        //                stackIn = stackIn.Next;

        //                rOutSum += pr = stackOut.R;
        //                gOutSum += pg = stackOut.G;
        //                bOutSum += pb = stackOut.B;
        //                aOutSum += pa = stackOut.A;

        //                rInSum -= pr;
        //                gInSum -= pg;
        //                bInSum -= pb;
        //                aInSum -= pa;

        //                stackOut = stackOut.Next;

        //                yi += 4;
        //            }

        //            yw += width;
        //        }

        //        for (var x = 0; x < width; x++)
        //        {
        //            var rInSum = 0;
        //            var gInSum = 0;
        //            var bInSum = 0;
        //            var aInSum = 0;

        //            var rSum = 0;
        //            var gSum = 0;
        //            var bSum = 0;
        //            var aSum = 0;

        //            yi = x << 2;
        //            r.GetPixel(yi % scan / 4, yi / scan, out var pa, out var pr, out var pg, out var pb);

        //            var rOutSum = radiusPlus1 * pr;
        //            var gOutSum = radiusPlus1 * pg;
        //            var bOutSum = radiusPlus1 * pb;
        //            var aOutSum = radiusPlus1 * pa;

        //            rSum += sumFactor * pr;
        //            gSum += sumFactor * pg;
        //            bSum += sumFactor * pb;
        //            aSum += sumFactor * pa;

        //            stack = stackStart;

        //            for (var i = 0; i < radiusPlus1; i++)
        //            {
        //                stack.R = pr;
        //                stack.G = pg;
        //                stack.B = pb;
        //                stack.A = pa;
        //                stack = stack.Next;
        //            }

        //            var yp = width;

        //            for (var i = 1; i <= radius; i++)
        //            {
        //                yi = (yp + x) << 2;

        //                r.GetPixel(yi % scan / 4, yi / scan, out var aa, out var rr, out var gg, out var bb);
        //                var rbs = radiusPlus1 - i;
        //                rSum += (stack.R = pr = rr) * rbs;
        //                gSum += (stack.G = pg = gg) * rbs;
        //                bSum += (stack.B = pb = bb) * rbs;
        //                aSum += (stack.A = pa = aa) * rbs;

        //                rInSum += pr;
        //                gInSum += pg;
        //                bInSum += pb;
        //                aInSum += pa;

        //                stack = stack.Next;

        //                if (i < heightMinus1) yp += width;
        //            }

        //            yi = x;
        //            var stackIn = stackStart;
        //            var stackOut = stackEnd;

        //            for (var y = 0; y < height; y++)
        //            {
        //                var p = yi << 2;
        //                ;//pa = (byte)(255 / pa)
        //                r.SetPixel(p % scan / 4, p / scan, (byte)((aSum * mulSum) >> shgSum), (byte)(((rSum * mulSum) >> shgSum)), (byte)(((gSum * mulSum) >> shgSum)), (byte)(((bSum * mulSum) >> shgSum)));


        //                rSum -= rOutSum;
        //                gSum -= gOutSum;
        //                bSum -= bOutSum;
        //                aSum -= aOutSum;

        //                rOutSum -= stackIn.R;
        //                gOutSum -= stackIn.G;
        //                bOutSum -= stackIn.B;
        //                aOutSum -= stackIn.A;

        //                p = (x + ((p = y + radiusPlus1) < heightMinus1 ? p : heightMinus1) * width) << 2;

        //                r.GetPixel(p % scan / 4, p / scan, out var aa, out var rr, out var gg, out var bb);
        //                rSum += rInSum += stackIn.R = rr;
        //                gSum += gInSum += stackIn.G = gg;
        //                bSum += bInSum += stackIn.B = bb;
        //                aSum += aInSum += stackIn.A = aa;

        //                stackIn = stackIn.Next;

        //                rOutSum += pr = stackOut.R;
        //                gOutSum += pg = stackOut.G;
        //                bOutSum += pb = stackOut.B;
        //                aOutSum += pa = stackOut.A;

        //                rInSum -= pr;
        //                gInSum -= pg;
        //                bInSum -= pb;
        //                aInSum -= pa;

        //                stackOut = stackOut.Next;

        //                yi += width;
        //            }
        //        }

        //        //Marshal.Copy(pixels, 0, data.DataPointer, pixels.Length);

        //    }
        //    //}
        //    return result;
        //}


        /// <summary>
        /// 快速模糊
        /// </summary>
        /// <param name="bitmap">The bitmap to process.</param>
        /// <param name="radius">Gaussian blur radius.</param>
        public static unsafe Bitmap Process(Bitmap bitmap, int radius)
        {
            var result = (Bitmap)bitmap.Clone();
            ProcessOwner(result, radius);
            return result;
        }
        /// <summary>
        /// 快速模糊
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="radius"></param>
        public static unsafe void ProcessOwner(Bitmap bitmap, int radius)
        {
            if (radius < 1)
            {
                return;
            }
            var width = bitmap.Width;
            var height = bitmap.Height;

            var div = radius + radius + 1;
            var widthMinus1 = width - 1;
            var heightMinus1 = height - 1;
            var radiusPlus1 = radius + 1;
            var sumFactor = radiusPlus1 * (radiusPlus1 + 1) / 2;

            var stack = new BlurStack();
            var stackStart = stack;
            var stackEnd = stack;

            for (var i = 1; i < div; i++)
            {
                stack = stack.Next = new BlurStack();
                if (i == radiusPlus1)
                {
                    stackEnd = stack;
                }
            }

            stack.Next = stackStart;

            Debug.Assert(stackEnd != null);

            var yw = 0;
            var yi = 0;

            var mulSum = MulTable[radius];
            var shgSum = ShgTable[radius];

            using (var data = bitmap.Lock())
            {
                var pixels = (byte*)data.DataPointer;
                var length = width * height;
                if (data.AlphaType == AlphaType.Unpremul)
                {//如果像素格式不是预乘的，需要先预乘
                    for (int i = 0; i < length; i++)
                    {
                        var p = pixels + i * 4;
                        var a = p[3];
                        p[0] = (byte)(p[0] * a / 255);
                        p[1] = (byte)(p[1] * a / 255);
                        p[2] = (byte)(p[2] * a / 255);
                    }
                }

                for (var y = 0; y < height; y++)
                {
                    var rInSum = 0;
                    var gInSum = 0;
                    var bInSum = 0;
                    var aInSum = 0;

                    var rSum = 0;
                    var gSum = 0;
                    var bSum = 0;
                    var aSum = 0;

                    var pr = pixels[yi];
                    var pg = pixels[yi + 1];
                    var pb = pixels[yi + 2];
                    var pa = pixels[yi + 3];

                    var rOutSum = radiusPlus1 * pr;
                    var gOutSum = radiusPlus1 * pg;
                    var bOutSum = radiusPlus1 * pb;
                    var aOutSum = radiusPlus1 * pa;

                    rSum += sumFactor * pr;
                    gSum += sumFactor * pg;
                    bSum += sumFactor * pb;
                    aSum += sumFactor * pa;

                    stack = stackStart;

                    for (var i = 0; i < radiusPlus1; i++)
                    {
                        stack.R = pr;
                        stack.G = pg;
                        stack.B = pb;
                        stack.A = pa;
                        stack = stack.Next;
                    }

                    for (var i = 1; i < radiusPlus1; i++)
                    {
                        var p = yi + ((widthMinus1 < i ? widthMinus1 : i) << 2);
                        var rbs = radiusPlus1 - i;
                        rSum += (stack.R = pr = pixels[p]) * rbs;
                        gSum += (stack.G = pg = pixels[p + 1]) * rbs;
                        bSum += (stack.B = pb = pixels[p + 2]) * rbs;
                        aSum += (stack.A = pa = pixels[p + 3]) * rbs;

                        rInSum += pr;
                        gInSum += pg;
                        bInSum += pb;
                        aInSum += pa;

                        stack = stack.Next;
                    }

                    var stackIn = stackStart;
                    var stackOut = stackEnd;

                    for (var x = 0; x < width; x++)
                    {
                        pa = (byte)((aSum * mulSum) >> shgSum);
                        pixels[yi + 3] = pa;
                        pixels[yi] = (byte)(((rSum * mulSum) >> shgSum));
                        pixels[yi + 1] = (byte)(((gSum * mulSum) >> shgSum));
                        pixels[yi + 2] = (byte)(((bSum * mulSum) >> shgSum));


                        rSum -= rOutSum;
                        gSum -= gOutSum;
                        bSum -= bOutSum;
                        aSum -= aOutSum;

                        rOutSum -= stackIn.R;
                        gOutSum -= stackIn.G;
                        bOutSum -= stackIn.B;
                        aOutSum -= stackIn.A;

                        var p = x + radius + 1;
                        p = (yw + (p < widthMinus1 ? p : widthMinus1)) << 2;

                        rInSum += stackIn.R = pixels[p];
                        gInSum += stackIn.G = pixels[p + 1];
                        bInSum += stackIn.B = pixels[p + 2];
                        aInSum += stackIn.A = pixels[p + 3];

                        rSum += rInSum;
                        gSum += gInSum;
                        bSum += bInSum;
                        aSum += aInSum;

                        stackIn = stackIn.Next;

                        rOutSum += pr = stackOut.R;
                        gOutSum += pg = stackOut.G;
                        bOutSum += pb = stackOut.B;
                        aOutSum += pa = stackOut.A;

                        rInSum -= pr;
                        gInSum -= pg;
                        bInSum -= pb;
                        aInSum -= pa;

                        stackOut = stackOut.Next;

                        yi += 4;
                    }

                    yw += width;
                }

                for (var x = 0; x < width; x++)
                {
                    var rInSum = 0;
                    var gInSum = 0;
                    var bInSum = 0;
                    var aInSum = 0;

                    var rSum = 0;
                    var gSum = 0;
                    var bSum = 0;
                    var aSum = 0;

                    yi = x << 2;
                    var pr = pixels[yi];
                    var pg = pixels[yi + 1];
                    var pb = pixels[yi + 2];
                    var pa = pixels[yi + 3];

                    var rOutSum = radiusPlus1 * pr;
                    var gOutSum = radiusPlus1 * pg;
                    var bOutSum = radiusPlus1 * pb;
                    var aOutSum = radiusPlus1 * pa;

                    rSum += sumFactor * pr;
                    gSum += sumFactor * pg;
                    bSum += sumFactor * pb;
                    aSum += sumFactor * pa;

                    stack = stackStart;

                    for (var i = 0; i < radiusPlus1; i++)
                    {
                        stack.R = pr;
                        stack.G = pg;
                        stack.B = pb;
                        stack.A = pa;
                        stack = stack.Next;
                    }

                    var yp = width;

                    for (var i = 1; i <= radius; i++)
                    {
                        yi = (yp + x) << 2;

                        var rbs = radiusPlus1 - i;
                        rSum += (stack.R = pr = pixels[yi]) * rbs;
                        gSum += (stack.G = pg = pixels[yi + 1]) * rbs;
                        bSum += (stack.B = pb = pixels[yi + 2]) * rbs;
                        aSum += (stack.A = pa = pixels[yi + 3]) * rbs;

                        rInSum += pr;
                        gInSum += pg;
                        bInSum += pb;
                        aInSum += pa;

                        stack = stack.Next;

                        if (i < heightMinus1) yp += width;
                    }

                    yi = x;
                    var stackIn = stackStart;
                    var stackOut = stackEnd;

                    for (var y = 0; y < height; y++)
                    {
                        var p = yi << 2;
                        pa = (byte)((aSum * mulSum) >> shgSum);
                        pixels[p + 3] = pa;
                        pixels[p] = (byte)(((rSum * mulSum) >> shgSum));
                        pixels[p + 1] = (byte)(((gSum * mulSum) >> shgSum));
                        pixels[p + 2] = (byte)(((bSum * mulSum) >> shgSum));


                        rSum -= rOutSum;
                        gSum -= gOutSum;
                        bSum -= bOutSum;
                        aSum -= aOutSum;

                        rOutSum -= stackIn.R;
                        gOutSum -= stackIn.G;
                        bOutSum -= stackIn.B;
                        aOutSum -= stackIn.A;

                        p = (x + ((p = y + radiusPlus1) < heightMinus1 ? p : heightMinus1) * width) << 2;

                        rSum += rInSum += stackIn.R = pixels[p];
                        gSum += gInSum += stackIn.G = pixels[p + 1];
                        bSum += bInSum += stackIn.B = pixels[p + 2];
                        aSum += aInSum += stackIn.A = pixels[p + 3];

                        stackIn = stackIn.Next;

                        rOutSum += pr = stackOut.R;
                        gOutSum += pg = stackOut.G;
                        bOutSum += pb = stackOut.B;
                        aOutSum += pa = stackOut.A;

                        rInSum -= pr;
                        gInSum -= pg;
                        bInSum -= pb;
                        aInSum -= pa;

                        stackOut = stackOut.Next;

                        yi += width;
                    }
                }

                if (data.AlphaType == AlphaType.Unpremul)
                {//恢复非预乘格式
                    for (int i = 0; i < length; i++)
                    {
                        var p = pixels + i * 4;
                        var a = p[3];
                        if (a > 0 && a < 255)
                        {
                            p[0] = (byte)(p[0] * 255 / a);
                            p[1] = (byte)(p[1] * 255 / a);
                            p[2] = (byte)(p[2] * 255 / a);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 处理阴影，
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="radius"></param>
        /// <param name="rect">过滤掉中间的区域，提高性能</param>
        public static unsafe void ProcessShadow(Bitmap bitmap, int radius, Rect rect)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

            var div = radius + radius + 1;
            var widthMinus1 = width - 1;
            var heightMinus1 = height - 1;
            var radiusPlus1 = radius + 1;
            var sumFactor = radiusPlus1 * (radiusPlus1 + 1) / 2;

            var stack = new BlurStack();
            var stackStart = stack;
            var stackEnd = stack;

            for (var i = 1; i < div; i++)
            {
                stack = stack.Next = new BlurStack();
                if (i == radiusPlus1)
                {
                    stackEnd = stack;
                }
            }

            stack.Next = stackStart;

            Debug.Assert(stackEnd != null);

            var yw = 0;
            var yi = 0;

            var mulSum = MulTable[radius];
            var shgSum = ShgTable[radius];

            using (var data = bitmap.Lock())
            {
                var pixels = (byte*)data.DataPointer;
                var length = width * height;
                if (data.AlphaType == AlphaType.Unpremul)
                {//如果像素格式不是预乘的，需要先预乘
                    for (int i = 0; i < length; i++)
                    {
                        var p = pixels + i * 4;
                        var a = p[3];
                        p[0] = (byte)(p[0] * a / 255);
                        p[1] = (byte)(p[1] * a / 255);
                        p[2] = (byte)(p[2] * a / 255);
                    }
                }

                for (var y = 0; y < height; y++)
                {
                    if (y > rect.Top && y < (int)(rect.Top + rect.Height) - 1)
                    {
                        y = (int)(rect.Top + rect.Height) - 1;
                        continue;
                    }
                    var rInSum = 0;
                    var gInSum = 0;
                    var bInSum = 0;
                    var aInSum = 0;

                    var rSum = 0;
                    var gSum = 0;
                    var bSum = 0;
                    var aSum = 0;

                    var pr = pixels[yi];
                    var pg = pixels[yi + 1];
                    var pb = pixels[yi + 2];
                    var pa = pixels[yi + 3];

                    var rOutSum = radiusPlus1 * pr;
                    var gOutSum = radiusPlus1 * pg;
                    var bOutSum = radiusPlus1 * pb;
                    var aOutSum = radiusPlus1 * pa;

                    rSum += sumFactor * pr;
                    gSum += sumFactor * pg;
                    bSum += sumFactor * pb;
                    aSum += sumFactor * pa;

                    stack = stackStart;

                    for (var i = 0; i < radiusPlus1; i++)
                    {
                        stack.R = pr;
                        stack.G = pg;
                        stack.B = pb;
                        stack.A = pa;
                        stack = stack.Next;
                    }

                    for (var i = 1; i < radiusPlus1; i++)
                    {
                        var p = yi + ((widthMinus1 < i ? widthMinus1 : i) << 2);
                        var rbs = radiusPlus1 - i;
                        rSum += (stack.R = pr = pixels[p]) * rbs;
                        gSum += (stack.G = pg = pixels[p + 1]) * rbs;
                        bSum += (stack.B = pb = pixels[p + 2]) * rbs;
                        aSum += (stack.A = pa = pixels[p + 3]) * rbs;

                        rInSum += pr;
                        gInSum += pg;
                        bInSum += pb;
                        aInSum += pa;

                        stack = stack.Next;
                    }

                    var stackIn = stackStart;
                    var stackOut = stackEnd;

                    for (var x = 0; x < width; x++)
                    {
                        if (x > rect.Left && x < (int)(rect.Left + rect.Width) - 1)
                        {
                            x = (int)(rect.Left + rect.Width) - 1;
                            continue;
                        }
                        pa = (byte)((aSum * mulSum) >> shgSum);
                        pixels[yi + 3] = pa;
                        pixels[yi] = (byte)(((rSum * mulSum) >> shgSum));
                        pixels[yi + 1] = (byte)(((gSum * mulSum) >> shgSum));
                        pixels[yi + 2] = (byte)(((bSum * mulSum) >> shgSum));


                        rSum -= rOutSum;
                        gSum -= gOutSum;
                        bSum -= bOutSum;
                        aSum -= aOutSum;

                        rOutSum -= stackIn.R;
                        gOutSum -= stackIn.G;
                        bOutSum -= stackIn.B;
                        aOutSum -= stackIn.A;

                        var p = x + radius + 1;
                        p = (yw + (p < widthMinus1 ? p : widthMinus1)) << 2;

                        rInSum += stackIn.R = pixels[p];
                        gInSum += stackIn.G = pixels[p + 1];
                        bInSum += stackIn.B = pixels[p + 2];
                        aInSum += stackIn.A = pixels[p + 3];

                        rSum += rInSum;
                        gSum += gInSum;
                        bSum += bInSum;
                        aSum += aInSum;

                        stackIn = stackIn.Next;

                        rOutSum += pr = stackOut.R;
                        gOutSum += pg = stackOut.G;
                        bOutSum += pb = stackOut.B;
                        aOutSum += pa = stackOut.A;

                        rInSum -= pr;
                        gInSum -= pg;
                        bInSum -= pb;
                        aInSum -= pa;

                        stackOut = stackOut.Next;

                        yi += 4;
                    }

                    yw += width;
                }

                for (var x = 0; x < width; x++)
                {
                    if (x > rect.Left && x < (int)(rect.Left + rect.Width) - 1)
                    {
                        x = (int)(rect.Left + rect.Width) - 1;
                        continue;
                    }
                    var rInSum = 0;
                    var gInSum = 0;
                    var bInSum = 0;
                    var aInSum = 0;

                    var rSum = 0;
                    var gSum = 0;
                    var bSum = 0;
                    var aSum = 0;

                    yi = x << 2;
                    var pr = pixels[yi];
                    var pg = pixels[yi + 1];
                    var pb = pixels[yi + 2];
                    var pa = pixels[yi + 3];

                    var rOutSum = radiusPlus1 * pr;
                    var gOutSum = radiusPlus1 * pg;
                    var bOutSum = radiusPlus1 * pb;
                    var aOutSum = radiusPlus1 * pa;

                    rSum += sumFactor * pr;
                    gSum += sumFactor * pg;
                    bSum += sumFactor * pb;
                    aSum += sumFactor * pa;

                    stack = stackStart;

                    for (var i = 0; i < radiusPlus1; i++)
                    {
                        stack.R = pr;
                        stack.G = pg;
                        stack.B = pb;
                        stack.A = pa;
                        stack = stack.Next;
                    }

                    var yp = width;

                    for (var i = 1; i <= radius; i++)
                    {
                        yi = (yp + x) << 2;

                        var rbs = radiusPlus1 - i;
                        rSum += (stack.R = pr = pixels[yi]) * rbs;
                        gSum += (stack.G = pg = pixels[yi + 1]) * rbs;
                        bSum += (stack.B = pb = pixels[yi + 2]) * rbs;
                        aSum += (stack.A = pa = pixels[yi + 3]) * rbs;

                        rInSum += pr;
                        gInSum += pg;
                        bInSum += pb;
                        aInSum += pa;

                        stack = stack.Next;

                        if (i < heightMinus1) yp += width;
                    }

                    yi = x;
                    var stackIn = stackStart;
                    var stackOut = stackEnd;

                    for (var y = 0; y < height; y++)
                    {
                        if (y > rect.Top && y < (int)(rect.Top + rect.Height) - 1)
                        {
                            y = (int)(rect.Top + rect.Height) - 1;
                            continue;
                        }
                        var p = yi << 2;
                        pa = (byte)((aSum * mulSum) >> shgSum);
                        pixels[p + 3] = pa;
                        pixels[p] = (byte)(((rSum * mulSum) >> shgSum));
                        pixels[p + 1] = (byte)(((gSum * mulSum) >> shgSum));
                        pixels[p + 2] = (byte)(((bSum * mulSum) >> shgSum));


                        rSum -= rOutSum;
                        gSum -= gOutSum;
                        bSum -= bOutSum;
                        aSum -= aOutSum;

                        rOutSum -= stackIn.R;
                        gOutSum -= stackIn.G;
                        bOutSum -= stackIn.B;
                        aOutSum -= stackIn.A;

                        p = (x + ((p = y + radiusPlus1) < heightMinus1 ? p : heightMinus1) * width) << 2;

                        rSum += rInSum += stackIn.R = pixels[p];
                        gSum += gInSum += stackIn.G = pixels[p + 1];
                        bSum += bInSum += stackIn.B = pixels[p + 2];
                        aSum += aInSum += stackIn.A = pixels[p + 3];

                        stackIn = stackIn.Next;

                        rOutSum += pr = stackOut.R;
                        gOutSum += pg = stackOut.G;
                        bOutSum += pb = stackOut.B;
                        aOutSum += pa = stackOut.A;

                        rInSum -= pr;
                        gInSum -= pg;
                        bInSum -= pb;
                        aInSum -= pa;

                        stackOut = stackOut.Next;

                        yi += width;
                    }
                }

                if (data.AlphaType == AlphaType.Unpremul)
                {//恢复非预乘格式
                    for (int i = 0; i < length; i++)
                    {
                        var p = pixels + i * 4;
                        var a = p[3];
                        if (a > 0 && a < 255)
                        {
                            p[0] = (byte)(p[0] * 255 / a);
                            p[1] = (byte)(p[1] * 255 / a);
                            p[2] = (byte)(p[2] * 255 / a);
                        }
                    }
                }
            }
        }
    }
}