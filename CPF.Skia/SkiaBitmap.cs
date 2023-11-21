using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;
using CPF.Drawing;

namespace CPF.Skia
{
    public class SkiaBitmap : IBitmapImpl
    {
        public SkiaBitmap(SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new Exception("创建位图失败");
            }
            this.bitmap = bitmap;
        }

        SKBitmap bitmap;

        public int Width
        {
            get
            {
                return bitmap.Width;
            }
        }

        public int Height
        {
            get
            {
                return bitmap.Height;
            }
        }

        public SKBitmap Bitmap
        {
            get
            {
                return bitmap;
            }
        }

        //public void CopyToIntptr(IntPtr intPtr)
        //{

        //}
        ~SkiaBitmap()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            GC.SuppressFinalize(this);
        }

        public IBitmapLockImpl Lock()
        {
            //bitmap.LockPixels();
            return new SkiaBitmapLock(bitmap);
        }

        //public void SaveToFile(string fileName, ImageFormat format)
        //{
        //    var f = SKEncodedImageFormat.Jpeg;
        //    switch (format)
        //    {
        //        case ImageFormat.Bmp:
        //            f = SKEncodedImageFormat.Bmp;
        //            break;
        //        case ImageFormat.Gif:
        //            f = SKEncodedImageFormat.Gif;
        //            break;
        //        case ImageFormat.Jpeg:
        //            f = SKEncodedImageFormat.Jpeg;
        //            break;
        //        case ImageFormat.Png:
        //            f = SKEncodedImageFormat.Png;
        //            break;
        //        default:
        //            break;
        //    }
        //    var stream = new SKFileWStream(fileName);
        //    SKPixmap.Encode(stream, bitmap, f, 100);
        //    //stream.Flush();
        //    stream.Dispose();
        //}

        public Stream SaveToStream(ImageFormat format)
        {
            var f = SKEncodedImageFormat.Jpeg;
            switch (format)
            {
                case ImageFormat.Bmp:
                    f = SKEncodedImageFormat.Bmp;
                    break;
                case ImageFormat.Gif:
                    f = SKEncodedImageFormat.Gif;
                    break;
                case ImageFormat.Jpeg:
                    f = SKEncodedImageFormat.Jpeg;
                    break;
                case ImageFormat.Png:
                    f = SKEncodedImageFormat.Png;
                    break;
                default:
                    break;
            }
            var m = new MemoryStream();
            var stream = new SKManagedWStream(m);
            SKPixmap.Encode(stream, bitmap, f, 100);
            stream.Dispose();
            m.Position = 0;
            return m;
        }

        public void SaveToStream(ImageFormat format, Stream m)
        {
            var f = SKEncodedImageFormat.Jpeg;
            switch (format)
            {
                case ImageFormat.Bmp:
                    f = SKEncodedImageFormat.Bmp;
                    break;
                case ImageFormat.Gif:
                    f = SKEncodedImageFormat.Gif;
                    break;
                case ImageFormat.Jpeg:
                    f = SKEncodedImageFormat.Jpeg;
                    break;
                case ImageFormat.Png:
                    f = SKEncodedImageFormat.Png;
                    break;
                default:
                    break;
            }
            //var m = new MemoryStream();
            var stream = new SKManagedWStream(m);
            SKPixmap.Encode(stream, bitmap, f, 100);
            stream.Dispose();
            //return m;
        }

        public object Clone()
        {
            return new SkiaBitmap(bitmap.Copy());
        }
    }
}
