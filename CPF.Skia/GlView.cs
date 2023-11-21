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
    /// 支持OpenGL绘制的控件
    /// </summary>
    public class GlView : CPF.UIElement
    {
        int fb;
        int texture;
        //int[] g_Renderbuffer;
        Size oldSize;
        bool f;
        protected override void OnRender(DrawingContext dc)
        {
            var size1 = ActualSize;
            if (size1.Width <= 0 || size1.Height <= 0 || DesignMode)
            {
                return;
            }

            var size = new PixelSize((int)Math.Round(size1.Width * Root.RenderScaling), (int)Math.Round(size1.Height * Root.RenderScaling));
            var skia = dc as SkiaDrawingContext;
            var gl = skia.GlContext;

            var fbs = new int[1];
            if (fb == 0)
            {
                gl.GenFramebuffers(1, fbs);
                fb = fbs[0];
            }
            else
            {
                fbs[0] = fb;
            }
            gl.GetIntegerv(GlConsts.GL_FRAMEBUFFER_BINDING, out var oldfb);
            gl.GetIntegerv(GlConsts.GL_TEXTURE_BINDING_2D, out var oldTexture);
            gl.GetIntegerv(GlConsts.GL_RENDERBUFFER_BINDING, out var oldRenderbuffer);
            gl.Enable(GlConsts.GL_TEXTURE_2D);

            //保存旧的状态
            gl.PushAttrib(GlConsts.GL_VIEWPORT_BIT);
            gl.MatrixMode(GlConsts.GL_PROJECTION);
            gl.PushMatrix();
            gl.MatrixMode(GlConsts.GL_MODELVIEW);
            gl.PushMatrix();

            gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, fb);

            if (size1 != oldSize)
            {
                oldSize = size1;

                var textures = new int[1] { texture };

                if (texture > 0)
                {
                    gl.DeleteTextures(1, textures);
                    //gl.DeleteRenderbuffers(1, g_Renderbuffer);
                }

                gl.GenTextures(1, textures);
                texture = textures[0];
                //gl.ActiveTexture(GlConsts.GL_TEXTURE0);
                gl.BindTexture(GlConsts.GL_TEXTURE_2D, textures[0]);
                gl.TexImage2D(GlConsts.GL_TEXTURE_2D, 0, GlConsts.GL_RGBA8, (int)size.Width, (int)size.Height, 0, GlConsts.GL_RGBA, GlConsts.GL_UNSIGNED_BYTE, IntPtr.Zero);
                gl.TexParameteri(GlConsts.GL_TEXTURE_2D, GlConsts.GL_TEXTURE_MAG_FILTER, GlConsts.GL_NEAREST);
                gl.TexParameteri(GlConsts.GL_TEXTURE_2D, GlConsts.GL_TEXTURE_MIN_FILTER, GlConsts.GL_NEAREST);
                gl.FramebufferTexture2D(GlConsts.GL_FRAMEBUFFER, GlConsts.GL_COLOR_ATTACHMENT0, GlConsts.GL_TEXTURE_2D, texture, 0);
                //gl.BindTexture(GlConsts.GL_TEXTURE_2D, 0);

                //g_Renderbuffer = new int[1];
                //gl.GenRenderbuffers(1, g_Renderbuffer);
                //gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, g_Renderbuffer[0]);
                //gl.RenderbufferStorage(GlConsts.GL_RENDERBUFFER, GlConsts.GL_DEPTH_COMPONENT, (int)size.Width, (int)size.Height);
                //gl.FramebufferRenderbuffer(GlConsts.GL_FRAMEBUFFER, GlConsts.GL_DEPTH_ATTACHMENT, GlConsts.GL_RENDERBUFFER, g_Renderbuffer[0]);
                //gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER_EXT, 0);

            }
            //gl.BindTexture(GlConsts.GL_TEXTURE_2D, texture);
            // gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, g_Renderbuffer[0]);
            // GlConsts.GL_FRAMEBUFFER_COMPLETE
            var status = gl.CheckFramebufferStatus(GlConsts.GL_FRAMEBUFFER);
            gl.Viewport(0, 0, size.Width, size.Height);


            //var MODELVIEW = new float[16];
            //gl.GetFloatv(GlConsts.GL_MODELVIEW_MATRIX, MODELVIEW);


            //var status = gl.CheckFramebufferStatus(GlConsts.GL_FRAMEBUFFER) == GlConsts.GL_FRAMEBUFFER_COMPLETE;

            //if (!f)
            {
                OnGlRender(skia.GlContext, size);
                f = true;
            }

            //var framebufferInfo = new GRGlFramebufferInfo((uint)fb, SKColorType.Rgba8888.ToGlSizedFormat());

            //gl.GetIntegerv(GlConsts.GL_FRAMEBUFFER_BINDING, out var framebuffer);
            //gl.GetIntegerv(3415, out var stencil);
            //gl.GetIntegerv(32937, out var samples);
            gl.BindRenderbuffer(GlConsts.GL_RENDERBUFFER, oldRenderbuffer);
            gl.BindTexture(GlConsts.GL_TEXTURE_2D, oldTexture);
            gl.BindFramebuffer(GlConsts.GL_FRAMEBUFFER, oldfb);
            //  using (var backendTexture = new GRBackendRenderTarget(size.Width, size.Height, samples, stencil, framebufferInfo))
            using (var backendTexture = new GRBackendTexture((int)size.Width, (int)size.Height, false, new GRGlTextureInfo { Format = GlConsts.GL_RGBA8, Id = (uint)texture, Target = GlConsts.GL_TEXTURE_2D }))
            {
                // using (var surface = SKSurface.Create((GRContext)skia.GlContext.GRContext, backendTexture, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888))
                using (var surface = SKSurface.Create((GRContext)skia.GlContext.GRContext, backendTexture, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888))
                {
                    //if (surface == null)
                    //    return;
                    //byte[] data = new byte[size.Width * size.Height * 4];
                    //gl.GetTexImage(GlConsts.GL_TEXTURE_2D, 0, GlConsts.GL_RGBA, GlConsts.GL_UNSIGNED_BYTE, data);

                    //System.Diagnostics.Debug.WriteLine($"{oldfb},{oldRenderbuffer},{oldTexture}");

                    //恢复状态
                    gl.MatrixMode(GlConsts.GL_MODELVIEW);
                    gl.PopMatrix();
                    gl.MatrixMode(GlConsts.GL_PROJECTION);
                    gl.PopMatrix();
                    gl.PopAttrib();

                    skia.SKCanvas.DrawSurface(surface, 0, 0);
                }
            }
            //unsafe
            //{
            //    fixed (byte* p = data)
            //    {
            //        using (var bitmap = new Bitmap(size.Width, size.Height, size.Width * 4, PixelFormat.Rgba, (IntPtr)p))
            //        {
            //            dc.DrawImage(bitmap, new Rect(0, 0, size.Width, size.Height), new Rect(0, 0, size.Width, size.Height));
            //        }
            //    }
            //}
            //gl.DeleteTextures(1, textures);
            //gl.DeleteRenderbuffers(1, g_Renderbuffer);
            //gl.DeleteFramebuffers(1, fbs);
            base.OnRender(dc);
        }

        Random random = new Random();
        protected void OnGlRender(IGlContext gl, PixelSize viewPort)
        {
            gl.MatrixMode(GlConsts.GL_PROJECTION);
            // gl.LoadIdentity();
            //gl.Ortho(-250, 250, -250, 250, -100, 100);
            gl.MatrixMode(GlConsts.GL_MODELVIEW);
            gl.LoadIdentity();
            //var PROJECTION = new float[4];
            //gl.GetFloatv(GlConsts.GL_VIEWPORT, PROJECTION);
            gl.ClearColor((float)random.NextDouble(), 0.25f, 0.75f, 0.5f);
            gl.Clear(GlConsts.GL_COLOR_BUFFER_BIT);
            gl.Color4f(1, 1, 1, 1);
            gl.Begin(GlConsts.GL_TRIANGLES);                              // Drawing Using Triangles
                                                                          //  gl.Color4f(1.0f, 0.0f, 0.0f, 1);                      // Set The Color To Red
            gl.Vertex3f(0.0f, 0.0f, 0.0f);                  // Top
                                                            //  gl.Color4f(0.0f, 1.0f, 0.0f, 1);                      // Set The Color To Green
            gl.Vertex3f(-1.0f, -1.0f, 0.0f);                  // Bottom Left
                                                              //  gl.Color4f(0.0f, 0.0f, 1.0f, 1);                      // Set The Color To Blue
            gl.Vertex3f(1.0f, -1.0f, 0.0f);                  // Bottom Right
                                                             // Drawing Using Triangles
                                                             //  gl.Color4f(1.0f, 0.0f, 0.0f, 1);                      // Set The Color To Red
            gl.Vertex3f((float)viewPort.Width, (float)viewPort.Height, 0.0f);                  // Top
                                                                                               //  gl.Color4f(0.0f, 1.0f, 0.0f, 1);                      // Set The Color To Green
            gl.Vertex3f(-1.0f, 21.0f, 0.0f);                  // Bottom Left
                                                              //  gl.Color4f(0.0f, 0.0f, 1.0f, 1);                      // Set The Color To Blue
            gl.Vertex3f(21.0f, 5.0f, 0.0f);                  // Bottom Right
            gl.End();

            var PROJECTION = new float[16];
            gl.GetFloatv(GlConsts.GL_PROJECTION_MATRIX, PROJECTION);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (fb > 0)
            {
                OpenglEx.DeleteFramebuffers(null, 1, new int[] { fb });
                fb = 0;
            }
            if (texture > 0)
            {
                OpenglEx.DeleteTextures(null, 1, new int[] { texture });
                //OpenglEx.DeleteRenderbuffers(null, 1, g_Renderbuffer);
                texture = 0;
            }
        }

        //[DllImport("opengl32", SetLastError = true)]
        //private static extern void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);
    }
}
