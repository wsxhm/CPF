using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    public interface IBitmapLockImpl : IDisposable
    {
        ///// <summary>
        ///// 获取该位置的像素地址
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <returns></returns>
        //unsafe byte* this[int x, int y] { get; }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        void GetPixel(in int x, in int y, out byte a, out byte r, out byte g, out byte b);
        /// <summary>
        /// 设置像素颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        void SetPixel(in int x, in int y, in byte a, in byte r, in byte g, in byte b);

        void GetAlpha(in int x, in int y, out byte a);

        void SetAlpha(in int x, in int y, in byte a);
        /// <summary>
        /// 数据地址
        /// </summary>
        IntPtr DataPointer { get; }
        /// <summary>
        /// 位图像素格式
        /// </summary>
        PixelFormat PixelFormat { get; }
        /// <summary>
        /// 透明数据格式
        /// </summary>
        AlphaType AlphaType { get; }
        ///// <summary>
        ///// 像素拷贝覆盖，比SetPixel要快
        ///// </summary>
        ///// <param name="rect"></param>
        ///// <param name="sourceBuffer"></param>
        ///// <param name="sourcePixelFormat"></param>
        //unsafe void WritePixels(PixelRect rect, byte* sourceBuffer, PixelFormat sourcePixelFormat);
    }
    /// <summary>
    /// 位图像素格式
    /// </summary>
    public enum PixelFormat : byte
    {
        Undefined,
        /// <summary>
        /// alpha premultiplied
        /// </summary>
        PRgba,
        Rgba,
        /// <summary>
        /// alpha premultiplied
        /// </summary>
        PBgra,
        Bgra,
        Rgb565,
    }
    public enum AlphaType : byte
    {
        Unpremul,
        Premul,
        Opaque,
    }
}
