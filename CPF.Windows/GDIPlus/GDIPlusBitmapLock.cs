#if Net4
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.GDIPlus
{
    public class GDIPlusBitmapLock : IBitmapLockImpl
    {
        System.Drawing.Bitmap bmp;
        System.Drawing.Imaging.BitmapData data;
        int Stride;
        public GDIPlusBitmapLock(GDIPlusBitmap bmp)
        {
            this.bmp = bmp.Bitmap;
            data = this.bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Stride = data.Stride;
        }

        public IntPtr DataPointer
        {
            get
            {
                return data.Scan0;
            }
        }

        public PixelFormat PixelFormat => PixelFormat.PBgra;

        public AlphaType AlphaType => AlphaType.Premul;

        public void Dispose()
        {
            bmp.UnlockBits(data);
        }

        public unsafe void GetAlpha(in int x, in int y, out byte a)
        {
            byte* numPtr = (byte*)(((data.Scan0) + (y * Stride)) + (x * 4));
            a = numPtr[3];
        }

        public unsafe void GetPixel(in int x, in int y, out byte a, out byte r, out byte g, out byte b)
        {
            byte* numPtr = (byte*)(((data.Scan0) + (y * Stride)) + (x * 4));
            a = numPtr[3];
            r = numPtr[2];
            g = numPtr[1];
            b = numPtr[0];

            if (a > 0 && a < 255)
            {
                r = (byte)(r * 255 / a);
                g = (byte)(g * 255 / a);
                b = (byte)(b * 255 / a);
            }
        }

        public unsafe void SetAlpha(in int x, in int y, in byte a)
        {
            byte* numPtr = (byte*)(((data.Scan0) + (y * Stride)) + (x * 4));
            var oa = numPtr[3];
            numPtr[3] = a;
            if (oa == 0)
            {
                return;
            }
            numPtr[0] = (byte)(numPtr[0] * a / oa);
            numPtr[1] = (byte)(numPtr[1] * a / oa);
            numPtr[2] = (byte)(numPtr[2] * a / oa);
        }

        public unsafe void SetPixel(in int x, in int y, in byte a, in byte r, in byte g, in byte b)
        {
            byte* numPtr = (byte*)(((data.Scan0) + (y * Stride)) + (x * 4));

            numPtr[0] = (byte)(b * a / 255);
            numPtr[1] = (byte)(g * a / 255);
            numPtr[2] = (byte)(r * a / 255);
            numPtr[3] = a;
        }

        public unsafe void WritePixels(PixelRect rect, byte* sourceBuffer, PixelFormat sourcePixelFormat)
        {
            int offset = (bmp.Width - rect.Width) * 4;
            var p = sourceBuffer + rect.Y * bmp.Width * 4 + rect.X * 4;
            var target = (byte*)data.Scan0 + rect.Y * bmp.Width * 4 + rect.X * 4;
            var rh = rect.Height;
            var rw = rect.Width;
            switch (sourcePixelFormat)
            {
                case PixelFormat.Undefined:
                    break;
                case PixelFormat.PRgba:
                    for (int y = 0; y < rh; y++)
                    {
                        for (int x = 0; x < rw; x++)
                        {
                            //SetPixel(x + dirtyRect.X, y + dirtyRect.Y, p[3], p[2], p[1], p[0]);
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
                    break;
                case PixelFormat.Rgba:
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
                    break;
                case PixelFormat.PBgra:
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
                    break;
                case PixelFormat.Bgra:
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
                    break;
                default:
                    break;
            }
        }
    }
}
#endif