using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CPF.Drawing
{
    public interface IBitmapImpl : IImage
    {
        ///// <summary>
        ///// 宽度
        ///// </summary>
        //uint Width
        //{
        //    get;
        //}

        ///// <summary>
        ///// 高度
        ///// </summary>
        //uint Height
        //{
        //    get;
        //}
        ///// <summary>
        ///// 像素格式
        ///// </summary>
        //public abstract PixelFormat PixelFormat
        //{
        //    get;
        //}
        IBitmapLockImpl Lock();

        //void CopyToStream(Stream stream);

        //void CopyToIntptr(IntPtr intPtr);

    }
}
