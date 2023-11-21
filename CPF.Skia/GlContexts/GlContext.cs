using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CPF.Drawing;
using SkiaSharp;

namespace CPF.Skia
{
    public abstract class GlContext : IDisposable
    {
        public abstract void MakeCurrent();
        public abstract void SwapBuffers();
        public abstract void Dispose();
        public abstract GRGlTextureInfo CreateTexture(SKSizeI textureSize);
        public abstract void DestroyTexture(uint texture);

        public abstract void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil);

        public static GlContext Create(IRenderTarget renderTarget)
        {
            switch (CPF.Platform.Application.OperatingSystem)
            {
                case Platform.OperatingSystemType.Windows:
                    return new WglContext(renderTarget);
                case Platform.OperatingSystemType.Linux:
                    return new GlxContext(renderTarget);
                case Platform.OperatingSystemType.OSX:
                    return new CglContext(renderTarget);
                case Platform.OperatingSystemType.Android:
                    break;
                case Platform.OperatingSystemType.iOS:
                    break;
            }
            return null;
        }
    }
}
