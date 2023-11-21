using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using CPF.Platform;

namespace CPF.Drawing
{
    /// <summary>
    /// 位图
    /// </summary>
    public class Bitmap : Image
    {
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Bitmap(int w, int h) : this(Application.GetDrawingFactory().CreateBitmap(w, h))
        { }
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="pitch">指定一次扫描行的开头之间的字节偏移量的整数。 这通常字节数乘以位图的宽度。4*width</param>
        /// <param name="pixelFormat"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Bitmap(int w, int h, int pitch, PixelFormat pixelFormat, IntPtr data) : this(Application.GetDrawingFactory().CreateBitmap(w, h, pitch, pixelFormat, data))
        { }
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Bitmap(Stream stream) : this(Application.GetDrawingFactory().CreateBitmap(stream))
        { }
        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public Bitmap(Image img) : this(Application.GetDrawingFactory().CreateBitmap(img))
        { }

        IBitmapImpl bitmap;

        public Bitmap(IBitmapImpl bmp) : base(bmp)
        {
            this.bitmap = bmp;
        }

        ///// <summary>
        ///// 宽度
        ///// </summary>
        //public uint Width
        //{
        //    get { return bitmap.Width; }
        //}

        ///// <summary>
        ///// 高度
        ///// </summary>
        //public uint Height
        //{
        //    get { return bitmap.Height; }
        //}

        //public void Dispose()
        //{
        //    bitmap.Dispose();
        //}

        public IBitmapImpl BitmapImpl
        {
            get
            {
                return bitmap;
            }
        }

        ///// <summary>
        ///// 像素格式
        ///// </summary>
        //public abstract PixelFormat PixelFormat
        //{
        //    get;
        //}
        /// <summary>
        /// 锁定位图到内存用以像素操作，BitmapLock使用完要Dispose
        /// </summary>
        /// <returns></returns>
        public BitmapLock Lock()
        {
            return new BitmapLock(BitmapImpl.Lock());
        }

        //public void CopyToStream(Stream stream)
        //{
        //    BitmapImpl.CopyToStream(stream);
        //}

        //public void CopyToIntptr(IntPtr intPtr)
        //{
        //    BitmapImpl.CopyToIntptr(intPtr);
        //}

        public override object Clone()
        {
            return new Bitmap((IBitmapImpl)BitmapImpl.Clone());
        }

    }
}
