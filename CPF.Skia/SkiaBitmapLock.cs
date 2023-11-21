using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkiaSharp;
using CPF.Drawing;

namespace CPF.Skia
{
    public unsafe class SkiaBitmapLock : IBitmapLockImpl
    {
        SKBitmap bitmap;
        byte* pointer;
        SKColorType colorType;
        SKAlphaType alphaType;
        int width;
        internal SkiaBitmapLock(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
            pointer = (byte*)DataPointer;
            colorType = bitmap.ColorType;
            alphaType = bitmap.AlphaType;
            width = bitmap.Width;
            if (colorType == SKColorType.Bgra8888)
            {
                PixelFormat = PixelFormat.Bgra;
                if (alphaType == SKAlphaType.Premul)
                {
                    PixelFormat = PixelFormat.PBgra;
                }
            }
            else if (colorType == SKColorType.Rgba8888)
            {
                PixelFormat = PixelFormat.Rgba;
                if (alphaType == SKAlphaType.Premul)
                {
                    PixelFormat = PixelFormat.PRgba;
                }
            }
        }

        //public unsafe byte* this[int x, int y]
        //{
        //    get { return (byte*)(pointer + (y * 4 * width) + (x * 4)); }
        //}

        public IntPtr DataPointer
        {
            get
            {
                IntPtr length;
                return bitmap.GetPixels(out length);
            }
        }

        public PixelFormat PixelFormat
        {
            get;
        }

        public AlphaType AlphaType
        {
            get
            {
                if (alphaType == SKAlphaType.Premul)
                {
                    return AlphaType.Premul;
                }
                else if (this.alphaType == SKAlphaType.Opaque)
                {
                    return AlphaType.Opaque;
                }
                return AlphaType.Unpremul;
            }
        }

        public void Dispose()
        {
            //bitmap.UnlockPixels();
            bitmap.NotifyPixelsChanged();
            bitmap = null;
        }

        //const int RGB565_MASK_RED = 0xF800;
        //const int RGB565_MASK_GREEN = 0x07E0;
        //const int RGB565_MASK_BLUE = 0x001F;

        public unsafe void GetPixel(in int x, in int y, out byte a, out byte r, out byte g, out byte b)
        {
            byte* numPtr;
            //byte a, r, g, b;
            switch (colorType)
            {
                case SKColorType.Rgba8888:
                    numPtr = pointer + (y * 4 * width) + (x * 4);
                    r = numPtr[0];
                    g = numPtr[1];
                    b = numPtr[2];
                    a = numPtr[3];
                    break;
                case SKColorType.Bgra8888:
                    numPtr = pointer + (y * 4 * width) + (x * 4);
                    b = numPtr[0];
                    g = numPtr[1];
                    r = numPtr[2];
                    a = numPtr[3];
                    break;
                case SKColorType.Rgb565:
                    numPtr = pointer + (y * 2 * width) + (x * 2);
                    var rr = numPtr[1] & 0xf8;
                    var gg = (numPtr[1] << 5) | ((numPtr[0] & 0xe0) >> 3);
                    var bb = numPtr[0] << 3;

                    // 补偿  
                    r = (byte)(rr | ((rr & 0x38) >> 3));
                    g = (byte)(gg | ((gg & 0x0c) >> 2));
                    b = (byte)(bb | ((bb & 0x38) >> 3));
                    a = 255;
                    break;
                default:
                    throw new Exception("不支持的像素格式");
            }
            if (alphaType == SKAlphaType.Premul)
            {
                if (a > 0 && a < 255)
                {
                    r = (byte)(r * 255 / a);
                    g = (byte)(g * 255 / a);
                    b = (byte)(b * 255 / a);
                }
            }
            //else
            //{
            //    throw new Exception("不支持的AlphaType:" + alphaType);
            //}

            //return Color.FromArgb(a, r, g, b);
        }

        public unsafe void SetPixel(in int x, in int y, in byte a, in byte r, in byte g, in byte b)
        {
            byte* numPtr;

            switch (colorType)
            {
                case SKColorType.Rgba8888:
                    numPtr = (pointer + (y * 4 * width) + (x * 4));
                    if (alphaType == SKAlphaType.Premul)
                    {
                        //double pa = a / 255.0;
                        numPtr[2] = (byte)(b * a / 255.0);
                        numPtr[1] = (byte)(g * a / 255.0);
                        numPtr[0] = (byte)(r * a / 255.0);
                        numPtr[3] = a;
                    }
                    else
                    {
                        numPtr[0] = r;
                        numPtr[1] = g;
                        numPtr[2] = b;
                        numPtr[3] = a;
                    }
                    break;
                case SKColorType.Bgra8888:
                    numPtr = (pointer + (y * 4 * width) + (x * 4));
                    if (alphaType == SKAlphaType.Premul)
                    {
                        numPtr[0] = (byte)(b * a / 255.0);
                        numPtr[1] = (byte)(g * a / 255.0);
                        numPtr[2] = (byte)(r * a / 255.0);
                        numPtr[3] = a;
                    }
                    else
                    {
                        numPtr[0] = b;
                        numPtr[1] = g;
                        numPtr[2] = r;
                        numPtr[3] = a;
                    }
                    break;
                case SKColorType.Rgb565:
                    numPtr = (pointer + (y * 2 * width) + (x * 2));
                    var rgb = (ushort)((((r) << 8) & 0xF800) | (((g) << 3) & 0x7E0) | (((b) >> 3)));
                    numPtr[0] = (byte)(rgb & 0xFF);
                    numPtr[1] = (byte)((rgb >> 8) & 0xFF);
                    break;
                default:
                    throw new Exception("不支持的像素格式");
            }

        }

        public unsafe void GetAlpha(in int x, in int y, out byte a)
        {
            switch (colorType)
            {
                case SKColorType.Rgba8888:
                    a = (pointer + (y * 4 * width) + (x * 4))[3];
                    break;
                case SKColorType.Bgra8888:
                    a = (pointer + (y * 4 * width) + (x * 4))[3];
                    break;
                default:
                    throw new Exception("不支持的像素格式");
            }
            //if (alphaType == SKAlphaType.Premul)
            //{
            //    if (a > 0 && a < 255)
            //    {
            //        r = (byte)(r * 255 / a);
            //        g = (byte)(g * 255 / a);
            //        b = (byte)(b * 255 / a);
            //    }
            //}
            //else
            //{
            //    throw new Exception("不支持的AlphaType:" + alphaType);
            //}
        }

        public unsafe void SetAlpha(in int x, in int y, in byte a)
        {
            byte* numPtr = pointer + (y * 4 * width) + (x * 4);
            var oa = numPtr[3];
            numPtr[3] = a;
            if (alphaType == SKAlphaType.Premul && oa != 0)
            {
                numPtr[0] = (byte)(numPtr[0] * a / oa);
                numPtr[1] = (byte)(numPtr[1] * a / oa);
                numPtr[2] = (byte)(numPtr[2] * a / oa);
            }
        }

        public void WritePixels(PixelRect rect, byte* sourceBuffer, PixelFormat sourcePixelFormat)
        {
            int offset = (width - rect.Width) * 4;
            var p = sourceBuffer + rect.Y * width * 4 + rect.X * 4;
            var target = pointer + rect.Y * width * 4 + rect.X * 4;
            var rh = rect.Height;
            var rw = rect.Width;
            switch (sourcePixelFormat)
            {
                case PixelFormat.Undefined:
                    break;
                case PixelFormat.PRgba:
                    switch (colorType)
                    {
                        case SKColorType.Rgba8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        //SetPixel(x + dirtyRect.X, y + dirtyRect.Y, p[3], p[2], p[1], p[0]);
                                        target[0] = p[0];
                                        target[1] = p[1];
                                        target[2] = p[2];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        var a = p[3];
                                        target[0] = (byte)(p[0] * a / 255);
                                        target[1] = (byte)(p[1] * a / 255);
                                        target[2] = (byte)(p[2] * a / 255);
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        case SKColorType.Bgra8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[2];
                                        target[1] = p[1];
                                        target[2] = p[0];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        var a = p[3];
                                        target[0] = (byte)(p[2] * a / 255);
                                        target[1] = (byte)(p[1] * a / 255);
                                        target[2] = (byte)(p[0] * a / 255);
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        default:
                            throw new Exception("不支持的像素格式");
                    }
                    break;
                case PixelFormat.Rgba:
                    switch (colorType)
                    {
                        case SKColorType.Rgba8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        //SetPixel(x + dirtyRect.X, y + dirtyRect.Y, p[3], p[2], p[1], p[0]);
                                        var a = p[3];
                                        target[0] = (byte)(p[0] * a / 255);
                                        target[1] = (byte)(p[1] * a / 255);
                                        target[2] = (byte)(p[2] * a / 255);
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[0];
                                        target[1] = p[1];
                                        target[2] = p[2];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        case SKColorType.Bgra8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        var a = p[3];
                                        target[0] = (byte)(p[2] * a / 255);
                                        target[1] = (byte)(p[1] * a / 255);
                                        target[2] = (byte)(p[0] * a / 255);
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[2];
                                        target[1] = p[1];
                                        target[2] = p[0];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        default:
                            throw new Exception("不支持的像素格式");
                    }
                    break;
                case PixelFormat.PBgra:
                    switch (colorType)
                    {
                        case SKColorType.Rgba8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[2];
                                        target[1] = p[1];
                                        target[2] = p[0];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        var a = p[3];
                                        target[0] = (byte)(p[2] * a / 255);
                                        target[1] = (byte)(p[1] * a / 255);
                                        target[2] = (byte)(p[0] * a / 255);
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        case SKColorType.Bgra8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[0];
                                        target[1] = p[1];
                                        target[2] = p[2];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        var a = p[3];
                                        target[0] = (byte)(p[0] * a / 255);
                                        target[1] = (byte)(p[1] * a / 255);
                                        target[2] = (byte)(p[2] * a / 255);
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        default:
                            throw new Exception("不支持的像素格式");
                    }
                    break;
                case PixelFormat.Bgra:
                    switch (colorType)
                    {
                        case SKColorType.Rgba8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        //SetPixel(x + dirtyRect.X, y + dirtyRect.Y, p[3], p[2], p[1], p[0]);
                                        var a = p[3];
                                        if (a > 0)
                                        {
                                            target[0] = (byte)(p[2] * 255 / a);
                                            target[1] = (byte)(p[1] * 255 / a);
                                            target[2] = (byte)(p[0] * 255 / a);
                                        }
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[2];
                                        target[1] = p[1];
                                        target[2] = p[0];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        case SKColorType.Bgra8888:
                            if (alphaType == SKAlphaType.Premul)
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        var a = p[3];
                                        if (a > 0)
                                        {
                                            target[0] = (byte)(p[0] * 255 / a);
                                            target[1] = (byte)(p[1] * 255 / a);
                                            target[2] = (byte)(p[2] * 255 / a);
                                        }
                                        target[3] = a;
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            else
                            {
                                for (int y = 0; y < rh; y++)
                                {
                                    for (int x = 0; x < rw; x++)
                                    {
                                        target[0] = p[0];
                                        target[1] = p[1];
                                        target[2] = p[2];
                                        target[3] = p[3];
                                        target += 4;
                                        p += 4;
                                    } // x
                                    p += offset;
                                    target += offset;
                                } // y
                            }
                            break;
                        default:
                            throw new Exception("不支持的像素格式");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
