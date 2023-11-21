using CPF.Drawing;
using CPF.Mac.AppKit;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Mac.CoreGraphics;

namespace CPF.Mac
{
    class ScreenImpl : Screen
    {
        public ScreenImpl(NSScreen screen, Rect bounds, Rect workingArea, bool primary) : base(bounds, workingArea, primary)
        {
            this.screen = screen;
        }
        NSScreen screen;

        public unsafe override Bitmap Screenshot()
        {
            using (var img = new CGImage(CGImage.CGWindowListCreateImage(screen.Frame, CGWindowListOption.All, 0, CGWindowImageOption.Default), owns: true))
            {
                //CGImage imageRef = CGImage.CGWindowListCreateImage(mainRect, kCGWindowListOptionOnScreenOnly, kCGNullWindowID, kCGWindowImageNominalResolution | kCGWindowImageShouldBeOpaque);
                var height = img.Height;
                var width = img.Width;
                var bpr = img.BytesPerRow;
                var bpp = img.BitsPerPixel;
                var bpc = img.BitsPerComponent;
                var bytes_per_pixel = bpp / bpc;

                using (var data = img.DataProvider.CopyData())
                {
                    var bitmap = new Bitmap(img.Width, img.Height);
                    var bytes = (byte*)data.Bytes;
                    using (var b = bitmap.Lock())
                    {
                        for (var row = 0; row < height; row++)
                        {
                            for (var col = 0; col < width; col++)
                            {
                                var pixel = &bytes[row * bpr + col * bytes_per_pixel];

                                b.SetPixel(col, row, pixel[3], pixel[2], pixel[1], pixel[0]);
                            }
                        }
                    }
                    return bitmap;
                }
            }
        }
    }
}
