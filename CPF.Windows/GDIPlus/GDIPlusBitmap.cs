//#if Net4
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public class GDIPlusBitmap : GDIPlusImage, IBitmapImpl
    {
        public System.Drawing.Bitmap Bitmap
        {
            get { return (System.Drawing.Bitmap)this.Image; }
        }
        public GDIPlusBitmap(System.Drawing.Image image):base(image)
        {
        }
        public GDIPlusBitmap(GDIPlusImage image) : base(new System.Drawing.Bitmap(image.Image))
        { }

        public GDIPlusBitmap(int w, int h) : base(new System.Drawing.Bitmap(w, h))
        {
        }

        public GDIPlusBitmap(Stream stream) : base(new System.Drawing.Bitmap(stream))
        {
        }

        public GDIPlusBitmap(int w, int h, int pitch, PixelFormat pixelFormat, IntPtr data)
        {
            System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            switch (pixelFormat)
            {
                case PixelFormat.Undefined:
                    break;
                case PixelFormat.Bgra:
                    format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                    break;
                case PixelFormat.PBgra:
                    format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
                    break;
                default:
                    throw new Exception("不支持格式：" + pixelFormat);
            }
            Image = new System.Drawing.Bitmap(w, h, pitch, format, data);
        }

        //public override PixelFormat PixelFormat
        //{
        //    get
        //    {
        //        if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        //        {
        //            return PixelFormat.Format32bppArgb;
        //        }
        //        if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        //        {
        //            return PixelFormat.Format32bppPArgb;
        //        }

        //        return PixelFormat.Undefined;
        //    }
        //}

        public void CopyToIntptr(IntPtr intPtr)
        {
            using (MemoryStream m = new MemoryStream())
            {
                Bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] bytes = m.ToArray();
                Marshal.Copy(bytes, 0, intPtr, bytes.Length);
            }
        }

        //public void CopyToStream(Stream stream)
        //{
        //    Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        //}

        public IBitmapLockImpl Lock()
        {
            return new GDIPlusBitmapLock(this);
        }

        public override object Clone()
        {
            return new GDIPlusBitmap(new System.Drawing.Bitmap(Bitmap));//(System.Drawing.Image)Bitmap.Clone()
        }
    }
}
//#endif