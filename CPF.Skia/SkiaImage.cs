using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using System.IO;
using CPF.Drawing;

namespace CPF.Skia
{
    public class SkiaImage : IImageImpl
    {
        int frameCount;
        Stream stream;
        //SKBitmap[] bitmaps;
        SKCodec codec;
        SKBitmap bitmap;

        public SkiaImage(string path)
        {
            stream = File.OpenRead(path);
            //#if Net4
            using (SKData data = SKData.Create(path))
            {
                codec = SKCodec.Create(data);
            }
            //#else
            //            codec = SKCodec.Create(stream);
            //#endif
            frameCount = Math.Max(1, codec.FrameCount);
            bitmap = SKBitmap.Decode(codec);
            Codec();
        }

        public SkiaImage(Stream stream)
        {
            this.stream = new MemoryStream();
            stream.CopyTo(this.stream);
            this.stream.Position = 0;
            //#if Net4
            using (SKData data = SKData.Create(this.stream))
            {
                codec = SKCodec.Create(data);
            }
            //#else
            //            codec = SKCodec.Create(this.stream);
            //#endif
            if (codec == null)
            {
                this.stream.Position = 0;
                codec = SKCodec.Create(this.stream, out var result);
                if (codec == null)
                {
                    throw new Exception("图片解码失败" + result);
                }
            }
            frameCount = Math.Max(1, codec.FrameCount);
            bitmap = SKBitmap.Decode(codec);
            Codec();
        }

        int[] durations;

        public int[] FrameDelay { get { return durations; } }

        void Codec()
        {
            if (frameCount > 1)
            {
                durations = new int[frameCount];
                for (int frame = 0; frame < frameCount; frame++)
                {
                    // From the FrameInfo collection, get the duration of each frame
                    durations[frame] = codec.FrameInfo[frame].Duration;
                }
                Duration = 0;
                // Sum up the total duration
                for (int frame = 0; frame < durations.Length; frame++)
                {
                    Duration += durations[frame];
                }
            }
            if (bitmap == null)
            {
                throw new Exception("图片解析失败");
            }
        }

        //void Codec(SKCodec codec)
        //{
        //    // Get frame count and allocate bitmaps
        //    frameCount = codec.FrameCount;
        //    if (frameCount == 0)
        //    {
        //        frameCount = 1;
        //        bitmaps = new SKBitmap[frameCount];
        //        bitmaps[0] = SKBitmap.Decode(codec);
        //    }
        //    else
        //    {
        //        bitmaps = new SKBitmap[frameCount];
        //        var durations = new int[frameCount];
        //        //var accumulatedDurations = new int[frameCount];

        //        // Note: There's also a RepetitionCount property of SKCodec not used here

        //        // Loop through the frames
        //        for (int frame = 0; frame < frameCount; frame++)
        //        {
        //            // From the FrameInfo collection, get the duration of each frame
        //            durations[frame] = codec.FrameInfo[frame].Duration;

        //            // Create a full-color bitmap for each frame
        //            SKImageInfo imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);
        //            bitmaps[frame] = new SKBitmap(imageInfo);

        //            // Get the address of the pixels in that bitmap
        //            IntPtr pointer = bitmaps[frame].GetPixels();

        //            // Create an SKCodecOptions value to specify the frame
        //            SKCodecOptions codecOptions = new SKCodecOptions(frame);

        //            // Copy pixels from the frame into the bitmap
        //            codec.GetPixels(imageInfo, pointer, codecOptions);
        //        }
        //        Duration = 0;
        //        // Sum up the total duration
        //        for (int frame = 0; frame < durations.Length; frame++)
        //        {
        //            Duration += durations[frame];
        //        }

        //        // Calculate the accumulated durations
        //        //for (int frame = 0; frame < durations.Length; frame++)
        //        //{
        //        //    accumulatedDurations[frame] = durations[frame] +
        //        //        (frame == 0 ? 0 : accumulatedDurations[frame - 1]);
        //        //}
        //    }
        //}

        public SKBitmap Image
        {
            get
            {
                if (needUpdate)
                {
                    needUpdate = false;

                    IntPtr pointer = bitmap.GetPixels();

                    // Create an SKCodecOptions value to specify the frame
#if Net4
                    SKCodecOptions codecOptions = new SKCodecOptions((int)index, true);
#else
                    SKCodecOptions codecOptions = new SKCodecOptions((int)index, (int)index - 1);
#endif
                    SKImageInfo imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                    // Copy pixels from the frame into the bitmap
                    codec.GetPixels(imageInfo, pointer, codecOptions);
                    bitmap.NotifyPixelsChanged();
                }
                return bitmap;
            }
        }

        public int Width
        {
            get
            {
                return Image.Width;
            }
        }

        public int Height
        {
            get
            {
                return Image.Height;
            }
        }

        public uint FrameCount
        {
            get
            {
                return (uint)frameCount;
            }
        }

        bool needUpdate;
        uint index;
        public uint Index
        {
            get { return index; }
            set
            {
                if (index != value)
                {
                    index = value;
                    needUpdate = true;
                }
            }
        }
        ~SkiaImage()
        {
            Dispose();
        }
        public int Duration { get; set; }

        public void Dispose()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            if (codec != null)
            {
                codec.Dispose();
                codec = null;
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
            GC.SuppressFinalize(this);
        }

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
            using (var image = SKImage.FromBitmap(Image))
            {
                var data = image.Encode(f, 100);
                var d = data.AsStream();
                return d;
            }
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
            using (var image = SKImage.FromBitmap(Image))
            {
                var data = image.Encode(f, 100);
                //var d = data.AsStream();
                //return d;
                data.SaveTo(m);
                data.Dispose();
            }
        }

        public object Clone()
        {
            stream.Position = 0;
            return new SkiaImage(stream);
        }
    }
}
