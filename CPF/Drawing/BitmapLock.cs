using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    /// <summary>
    /// 位图锁，使用完了记得Dispose
    /// </summary>
    public class BitmapLock : IDisposable
    {
        public BitmapLock(IBitmapLockImpl bmplock)
        {
            this.bmplock = bmplock;
        }
        ///// <summary>
        ///// 像素地址，不同平台不同图形库对应的像素地址的ARGB顺序可能不一样
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <returns></returns>
        //public unsafe byte* this[int x, int y]
        //{
        //    get { return bmplock[x, y]; }
        //}

        IBitmapLockImpl bmplock;
        /// <summary>
        /// 设置像素颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(in int x, in int y, in byte a, in byte r, in byte g, in byte b)
        {
            bmplock.SetPixel(in x, in y, in a, in r, in g, in b);
        }
        /// <summary>
        /// 获取像素颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public void GetPixel(in int x, in int y, out byte a, out byte r, out byte g, out byte b)
        {
            bmplock.GetPixel(in x, in y, out a, out r, out g, out b);
        }
        /// <summary>
        /// 设置某个点的Alpha
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        public void SetAlpha(in int x, in int y, in byte a)
        {
            bmplock.SetAlpha(x, y, a);
        }
        /// <summary>
        /// 获取某个点的Alpha
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        public void GetAlpha(in int x, in int y, out byte a)
        {
            bmplock.GetAlpha(x, y, out a);
        }
        ///// <summary>
        ///// 像素拷贝覆盖，比SetPixel要快，sourceBuffer数据长度要和Bitmap的一样
        ///// </summary>
        ///// <param name="rect">目标区域和数据源区域</param>
        ///// <param name="sourceBuffer"></param>
        ///// <param name="sourcePixelFormat"></param>
        //public unsafe void WritePixels(PixelRect rect, byte* sourceBuffer, PixelFormat sourcePixelFormat)
        //{
        //    bmplock.WritePixels(rect, sourceBuffer, sourcePixelFormat);
        //}

        public void Dispose()
        {
            bmplock.Dispose();
        }

        /// <summary>
        /// 数据地址
        /// </summary>
        public IntPtr DataPointer { get { return bmplock.DataPointer; } }

        public PixelFormat PixelFormat
        {
            get { return bmplock.PixelFormat; }
        }

        public AlphaType AlphaType
        {
            get { return bmplock.AlphaType; }
        }
    }
}
