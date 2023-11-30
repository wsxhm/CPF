using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using CPF.OpenGL;
using System.Runtime.InteropServices;

namespace CPF.Skia
{
    /// <summary>
    /// 支持OpenGL绘制的控件，在GLRender事件里绘制，开启GPU硬件加速才能使用 new SkiaDrawingFactory { UseGPU = true }
    /// </summary>
    public class GLView : UIElement
    {
        int Id;
        int ColorBuffer;
        int DepthRenderBuffer;
        Size oldSize;
        SKImage image;
        SKPaint paint;
        GRBackendTexture backendTexture;
        /// <summary>
        /// 支持OpenGL绘制的控件，在GLRender事件里绘制，开启GPU硬件加速才能使用 new SkiaDrawingFactory { UseGPU = true }
        /// </summary>
        public GLView() { }

        //IGlContext context;
        protected unsafe override void OnRender(DrawingContext dc)
        {
            var cSize = ActualSize;
            if (cSize.Width <= 0 || cSize.Height <= 0 || DesignMode)
            {
                return;
            }

            var size = new PixelSize((int)Math.Round(cSize.Width * Root.RenderScaling), (int)Math.Round(cSize.Height * Root.RenderScaling));
            var skia = dc as SkiaDrawingContext;
            var _gl = skia.GlContext;
            if (paint == null)
            {
                paint = new SKPaint();
            }
            paint.IsAntialias = IsAntiAlias;
            paint.FilterQuality = IsAntiAlias ? SKFilterQuality.Medium : SKFilterQuality.None;

            if (Id == 0)
            {
                Id = _gl.GenFramebuffer();
                ColorBuffer = _gl.GenTexture();
                DepthRenderBuffer = _gl.GenRenderbuffer();

                _gl.BindTexture(GlConsts.GL_TEXTURE_2D, ColorBuffer);

                _gl.TexParameteri(GlConsts.GL_TEXTURE_2D, GlConsts.GL_TEXTURE_MIN_FILTER, (int)GlConsts.GL_LINEAR);
                _gl.TexParameteri(GlConsts.GL_TEXTURE_2D, GlConsts.GL_TEXTURE_MAG_FILTER, GlConsts.GL_LINEAR);

                _gl.BindTexture(GlConsts.GL_TEXTURE_2D, 0);


                OnGLLoaded(_gl);
            }
            if (cSize != oldSize)
            {
                oldSize = cSize;

                _gl.BindTexture(GlConsts.GL_TEXTURE_2D, ColorBuffer);
                _gl.TexImage2D(GlConsts.GL_TEXTURE_2D, 0, GlConsts.GL_RGBA, (int)size.Width, (int)size.Height, 0, GlConsts.GL_RGB, GlConsts.GL_UNSIGNED_BYTE, IntPtr.Zero);
                _gl.BindTexture(GlConsts.GL_TEXTURE_2D, 0);

                _gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, DepthRenderBuffer);
                _gl.RenderbufferStorage(GlConsts.GL_RENDERBUFFER, GlConsts.GL_DEPTH32F_STENCIL8, (int)size.Width, (int)size.Height);

                _gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, 0);

                _gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, Id);

                _gl.FramebufferTexture2D(GlConsts.GL_FRAMEBUFFER, GlConsts.GL_COLOR_ATTACHMENT0, GlConsts.GL_TEXTURE_2D, ColorBuffer, 0);

                _gl.FramebufferRenderbuffer(GlConsts.GL_FRAMEBUFFER, GlConsts.GL_DEPTH_STENCIL_ATTACHMENT, GlConsts.GL_RENDERBUFFER, DepthRenderBuffer);
                _gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, 0);


                if (image != null)
                {
                    image.Dispose();
                }
                if (backendTexture != null)
                {
                    backendTexture.Dispose();
                }
                backendTexture = new GRBackendTexture((int)(size.Width), (int)(size.Height), false, new GRGlTextureInfo(0x0DE1, (uint)ColorBuffer, SKColorType.Rgba8888.ToGlSizedFormat()));

                image = SKImage.FromTexture((GRContext)skia.GlContext.GRContext, backendTexture, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
            }

            _gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, Id);
            var vp = new float[4];
            _gl.GetFloatv(GlConsts.GL_VIEWPORT, vp);
            _gl.Viewport(0, 0, (int)size.Width, (int)size.Height);
            OnGLRender(_gl);
            _gl.Viewport((int)vp[0], (int)vp[1], (int)vp[2], (int)vp[3]);
            _gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, 0);
            //_gl.Flush();
            skia.SKCanvas.DrawImage(image, new SKRect(0, 0, size.Width, size.Height), new SKRect(0, 0, cSize.Width, cSize.Height), paint);
        }


        protected virtual void OnGLRender(IGlContext gl)
        {
            this.RaiseEvent(new GLEventArgs(gl), nameof(GLRender));
        }

        protected virtual void OnGLLoaded(IGlContext gl)
        {
            this.RaiseEvent(new GLEventArgs(gl), nameof(GLLoaded));
        }

        public event EventHandler<GLEventArgs> GLLoaded
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        public event EventHandler<GLEventArgs> GLRender
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


        protected override void Dispose(bool disposing)
        {
            if (Id != 0)
            {
                OpenglEx.DeleteFramebuffers(null, 1, new int[] { Id });
                OpenglEx.DeleteTextures(null, 1, new int[] { ColorBuffer });
                OpenglEx.DeleteRenderbuffers(null, 1, new int[] { DepthRenderBuffer });
                Id = 0;
            }
            if (image != null)
            {
                image.Dispose();
                image = null;
            }
            if (paint != null)
            {
                paint.Dispose();
                paint = null;
            }
            if (backendTexture != null)
            {
                backendTexture.Dispose();
                backendTexture = null;
            }
            base.Dispose(disposing);
        }
    }

    public class GLEventArgs : EventArgs
    {
        public GLEventArgs(IGlContext gl)
        {
            Context = gl;
        }
        public IGlContext Context { get; private set; }
    }
}
