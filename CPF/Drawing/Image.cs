using System;
using System.Collections.Generic;
using System.Text;
using CPF.Platform;
using System.IO;

namespace CPF.Drawing
{
    public class Image : IImage
    {
        IImage image;

        public Image(IImage image)
        {
            if (image == null)
            {
                throw new Exception("创建Image失败！");
            }
            this.image = image;
        }

        public IImage ImageImpl
        {
            get { return image; }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width
        {
            get { return image.Width; }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height
        {
            get { return image.Height; }
        }

        public void Dispose()
        {
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 帧数，比如GIF
        /// </summary>
        public uint FrameCount
        {
            get
            {
                if (image is IImageImpl img)
                {
                    return img.FrameCount;
                }
                return 1;
            }
        }
        /// <summary>
        /// 当前选中的帧索引，GIF
        /// </summary>
        public uint Index
        {
            get
            {
                if (image is IImageImpl img)
                {
                    return img.Index;
                }
                return 0;
            }
            set
            {
                if (image is IImageImpl img)
                {
                    img.Index = value;
                }
            }
        }
        /// <summary>
        /// 动画时长，GIF
        /// </summary>
        public int Duration
        {
            get
            {
                if (image is IImageImpl img)
                {
                    return img.Duration;
                }
                return 0;
            }
        }
        /// <summary>
        /// GIF的每帧时长
        /// </summary>
        public int[] FrameDelay
        {
            get
            {
                if (image is IImageImpl img)
                {
                    return img.FrameDelay;
                }
                return null;
            }
        }
        /// <summary>
        /// 下一帧的时间，从程序启动开始算，用于播放动画的时候使用，实现同一个Image对象，多个地方同步刷新
        /// </summary>
        public TimeSpan? NextFrame { get; set; }

        ~Image()
        {
            Dispose();
        }

        public Stream SaveToStream(ImageFormat format)
        {
            return image.SaveToStream(format);
        }

        public void SaveToStream(ImageFormat format, Stream m)
        {
            image.SaveToStream(format, m);
        }

        public static Image FromFile(string path)
        {
            return new Image(Application.GetDrawingFactory().ImageFromFile(path));
        }

        public static Image FromStream(Stream stream)
        {
            return new Image(Application.GetDrawingFactory().ImageFromStream(stream));
        }

        public static Image FromBuffer(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return new Image(Application.GetDrawingFactory().ImageFromStream(ms));
            }
        }

        public virtual object Clone()
        {
            return new Image(image.Clone() as IImage);
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            return base.ToString();
        }
        string path;
        public static implicit operator Image(string path)
        {
            var image = CPF.Styling.ResourceManager.GetImage(path).Result;
            if (image != null)
            {
                image.path = path;
            }
            return image;
        }
    }
}
