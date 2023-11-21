using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.OpenGL
{
    public class OpenGlRenderTarget<T> : OpenGlRenderTarget
    {
        public OpenGlRenderTarget(T fbt, IGlContext context, int width, int height, int framebuffer, int samples, int stencil) : base(context, width, height, framebuffer, samples, stencil)
        {
            FailBackTarget = fbt;
        }

        public T FailBackTarget { get; }
    }

    public class OpenGlRenderTarget : IRenderTarget
    {
        public OpenGlRenderTarget(IGlContext glContext, int width, int height, int framebuffer, int samples, int stencil)
        {
            Height = height;
            Width = width;
            Framebuffer = framebuffer;
            Samples = samples;
            Stencil = stencil;
            GlContext = glContext;
        }

        public int Height { get; }
        public int Width { get; }
        public int Framebuffer { get; }
        public int Samples { get; }
        public int Stencil { get; }
        public IGlContext GlContext { get; }
        public bool CanUseGPU => true;
    }
}
