#if Net4
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using CPF.Drawing;
using ImageFormat = CPF.Drawing.ImageFormat;

namespace CPF.GDIPlus
{
    public class GDIPlusImage : IImageImpl
    {
        public GDIPlusImage(System.Drawing.Image image)
        {
            this.image = image;

            if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
            {
                //var dimension = new System.Drawing.Imaging.FrameDimension(image.FrameDimensionsList[0]);
                frameCount = (uint)image.GetFrameCount(FrameDimension.Time);

                PropertyItem frameDelayItem = image.GetPropertyItem(PropertyTagFrameDelay);

                if (frameDelayItem != null)
                {
                    byte[] values = frameDelayItem.Value;
                    //Debug.Assert(values.Length == 4 * FrameCount, "PropertyItem has invalid value byte array");
                    frameDelay = new int[FrameCount];
                    for (int i = 0; i < FrameCount; ++i)
                    {
                        frameDelay[i] = (values[i * 4] + 256 * values[i * 4 + 1] + 256 * 256 * values[i * 4 + 2] + 256 * 256 * 256 * values[i * 4 + 3]) * 10;
                        duration += frameDelay[i];
                    }

                }
                ////Store each frame
                //imgList = new System.Drawing.Image[frameCount];
                //for (int i = 0; i < frameCount; i++)
                //{
                //    image.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Time, i);//This action requires the stream opened.
                //    imgList[i] = (System.Drawing.Image)image.Clone();
                //}
            }

        }

        internal GDIPlusImage() { }

        //System.Drawing.Image[] imgList;
        System.Drawing.Image image;

        public System.Drawing.Image Image
        {
            get
            {
                //if (imgList != null)
                //{
                //    return imgList[index];
                //}
                return image;
            }
            protected set { image = value; }
        }


        public int Width
        {
            get { return image.Width; }
        }

        public int Height
        {
            get { return image.Height; }
        }


        uint frameCount = 1;
        public uint FrameCount
        {
            get
            {
                //if (frameCount == 0)
                //{
                //    if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
                //    {
                //        FrameDimension fd = new FrameDimension(image.FrameDimensionsList[0]);
                //        frameCount = (uint)image.GetFrameCount(fd);
                //    }
                //    else
                //    {
                //        frameCount = 1;
                //    }
                //}
                return frameCount;
            }
        }

        uint index;
        public uint Index
        {
            get { return index; }
            set
            {
                index = value;
                image.SelectActiveFrame(FrameDimension.Time, (int)value);
            }
        }

        const int PropertyTagFrameDelay = 0x5100;
        int[] frameDelay;

        public int[] FrameDelay
        {
            get
            {
                //var d = Duration;
                return frameDelay;
            }
        }

        int duration = 0;
        public int Duration
        {
            get
            {
                //if (duration < 0)
                //{
                //    duration = 0;
                //    if (FrameCount > 1)
                //    {
                //        PropertyItem frameDelayItem = image.GetPropertyItem(PropertyTagFrameDelay);

                //        // If the image does not have a frame delay, we just return 0.                                     
                //        //
                //        if (frameDelayItem != null)
                //        {
                //            // Convert the frame delay from byte[] to int
                //            //
                //            byte[] values = frameDelayItem.Value;
                //            //Debug.Assert(values.Length == 4 * FrameCount, "PropertyItem has invalid value byte array");
                //            frameDelay = new int[FrameCount];
                //            for (int i = 0; i < FrameCount; ++i)
                //            {
                //                frameDelay[i] = (values[i * 4] + 256 * values[i * 4 + 1] + 256 * 256 * values[i * 4 + 2] + 256 * 256 * 256 * values[i * 4 + 3]) * 10;
                //                duration += frameDelay[i];
                //            }

                //        }
                //    }
                //}
                return duration;
            }
        }

        public void Dispose()
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
            //if (imgList != null)
            //{
            //    foreach (var item in imgList)
            //    {
            //        item.Dispose();
            //    }
            //    imgList = null;
            //}
        }

        public void SaveToFile(string fileName, ImageFormat format)
        {
            var f = System.Drawing.Imaging.ImageFormat.Jpeg;
            switch (format)
            {
                case ImageFormat.Bmp:
                    f = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case ImageFormat.Gif:
                    f = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case ImageFormat.Jpeg:
                    f = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ImageFormat.Png:
                    f = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                default:
                    break;
            }
            image.Save(fileName, f);
        }

        public Stream SaveToStream(ImageFormat format)
        {
            var m = new MemoryStream();
            System.Drawing.Imaging.ImageFormat imageFormat = null;
            switch (format)
            {
                case ImageFormat.Bmp:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case ImageFormat.Gif:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case ImageFormat.Jpeg:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ImageFormat.Png:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
            }
            if (imageFormat == null)
            {
                throw new Exception("不支持该格式图片:" + format);
            }
            image.Save(m, imageFormat);
            m.Position = 0;
            return m;
        }
        public void SaveToStream(ImageFormat format, Stream m)
        {
            //var m = new MemoryStream();
            System.Drawing.Imaging.ImageFormat imageFormat = null;
            switch (format)
            {
                case ImageFormat.Bmp:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case ImageFormat.Gif:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case ImageFormat.Jpeg:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ImageFormat.Png:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
            }
            if (imageFormat == null)
            {
                throw new Exception("不支持该格式图片:" + format);
            }
            image.Save(m, imageFormat);
            //m.Position = 0;
            //return m;
        }

        public virtual object Clone()
        {
            return new GDIPlusImage((System.Drawing.Image)image.Clone());
        }
    }
}
#endif