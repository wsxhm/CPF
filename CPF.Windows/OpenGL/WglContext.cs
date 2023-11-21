using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CPF.Drawing;
using CPF.OpenGL;

namespace CPF.Windows.OpenGL
{
    public class WglContext : CPF.OpenGL.IGlContext
    {
        public const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
        public const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
        public const int WGL_CONTEXT_LAYER_PLANE_ARB = 0x2093;
        public const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;
        public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;

        private IntPtr dc;
        private IntPtr rc;
        IntPtr handle;

        public WglContext(IntPtr handle)
        {
            this.handle = handle;
            dc = UnmanagedMethods.GetDC(handle);
            var pfd = new PIXELFORMATDESCRIPTOR();
            pfd.nSize = (ushort)Marshal.SizeOf(pfd);
            pfd.nVersion = 1;
            pfd.dwFlags = Wgl.PFD_DRAW_TO_WINDOW | Wgl.PFD_SUPPORT_OPENGL | Wgl.PFD_SUPPORT_COMPOSITION;
            pfd.iPixelType = Wgl.PFD_TYPE_RGBA;
            pfd.cColorBits = 32;
            pfd.cDepthBits = 24;
            pfd.cStencilBits = 8;
            pfd.cAlphaBits = 8;
            pfd.iLayerType = Wgl.PFD_MAIN_PLANE;

            var format = Wgl.ChoosePixelFormat(dc, ref pfd);
            Wgl.SetPixelFormat(dc, format, ref pfd);
            rc = Wgl.wglCreateContext(dc);

            //var nPixelFormat = new int[] { -1 };
            //var r = Wgl.wglChoosePixelFormatARB(dc, new int[] {
            //Wgl.WGL_SUPPORT_OPENGL_ARB, 1,    // Must support OGL rendering
            //Wgl.WGL_DRAW_TO_WINDOW_ARB, 1,    // pf that can run a window
            //Wgl.WGL_ACCELERATION_ARB, Wgl.WGL_FULL_ACCELERATION_ARB,    // must be HW accelerated

            //Wgl.WGL_PIXEL_TYPE_ARB, Wgl.WGL_TYPE_RGBA_ARB,
            //Wgl.WGL_COLOR_BITS_ARB, 32,             // 8 bits of each R, G and B
            //Wgl. WGL_DEPTH_BITS_ARB, 24,             // 24 bits of depth precision for window
            //Wgl.WGL_STENCIL_BITS_ARB, 8,            // 开启模板缓冲区,模板缓冲区位数=8
            //Wgl.WGL_ALPHA_BITS_ARB, 8, 
            ////Wgl.WGL_SAMPLE_BUFFERS_ARB, 1,    // MSAA on,开启多重采样
            ////Wgl.WGL_SAMPLES_ARB, 4,                 // 4x MSAA ,多重采样样本数量为4

            //0, 0
            //}, new float[] { 0, 0 }, 1, nPixelFormat, out var f);
            //Wgl.SetPixelFormat(dc, nPixelFormat[0], ref pfd);
            //rc = Wgl.WglCreateContextAttribsArb(dc, IntPtr.Zero,
            //new[]
            //{
            //    // major
            //    WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
            //    // minor
            //    WGL_CONTEXT_MINOR_VERSION_ARB,  0,
            //    // core profile
            //    WGL_CONTEXT_PROFILE_MASK_ARB, 1, 
            //    // debug 
            //    // WGL_CONTEXT_FLAGS_ARB, 1,
            //    // end
            //    0, 0
            //});

            if (rc == IntPtr.Zero)
            {
                Console.WriteLine("无法创建OpenGLContext   DC:" + dc + "  Handle:" + handle);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OpenGL Context创建成功");
                Console.WriteLine("OpenGL Context创建成功");
            }
        }

        public IntPtr RC
        {
            get { return rc; }
        }

        public IDisposable GRContext { get; set; }

        public IntPtr GetProcAddress(string name)
        {
            var ptr = Wgl.wglGetProcAddress(name);
            if (ptr == IntPtr.Zero)
            {
                ptr = UnmanagedMethods.GetProcAddress(OpenGl32Handle, name);
            }
            return ptr;
        }
        public static IntPtr OpenGl32Handle = UnmanagedMethods.LoadLibrary("opengl32");
        IntPtr oldRc;
        IntPtr oldDc;
        public void MakeCurrent()
        {
            if (rc != IntPtr.Zero)
            {
                oldDc = Wgl.wglGetCurrentDC();
                oldRc = Wgl.wglGetCurrentContext();
                if (!Wgl.wglMakeCurrent(dc, rc))
                {
                    throw new Exception("Could not set the context.");
                }
            }
        }

        public void SwapBuffers()
        {
            if (rc != IntPtr.Zero)
            {
                if (!Wgl.SwapBuffers(dc))
                {
                    throw new Exception("Could not complete SwapBuffers.");
                }
                Wgl.wglMakeCurrent(oldDc, oldRc);
            }
        }

        public void Dispose()
        {
            UnmanagedMethods.ReleaseDC(handle, dc);
            if (rc != IntPtr.Zero)
            {
                Wgl.wglDeleteContext(rc);
            }
            GRContext?.Dispose();
        }


        //public override GRGlTextureInfo CreateTexture(SKSizeI textureSize)
        //{
        //    var textures = new uint[1];
        //    Wgl.glGenTextures(textures.Length, textures);
        //    var textureId = textures[0];

        //    Wgl.glBindTexture(Wgl.GL_TEXTURE_2D, textureId);
        //    Wgl.glTexImage2D(Wgl.GL_TEXTURE_2D, 0, Wgl.GL_RGBA, textureSize.Width, textureSize.Height, 0, Wgl.GL_RGBA, Wgl.GL_UNSIGNED_BYTE, IntPtr.Zero);
        //    Wgl.glBindTexture(Wgl.GL_TEXTURE_2D, 0);

        //    return new GRGlTextureInfo
        //    {
        //        Id = textureId,
        //        Target = Wgl.GL_TEXTURE_2D,
        //        Format = Wgl.GL_RGBA8
        //    };
        //}

        //public override void DestroyTexture(uint texture)
        //{
        //    Wgl.glDeleteTextures(1, new[] { texture });
        //}

        public void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil)
        {
            Wgl.glGetIntegerv(GL_FRAMEBUFFER_BINDING, out framebuffer);
            Wgl.glGetIntegerv(3415, out stencil);
            Wgl.glGetIntegerv(32937, out samples);
        }
    }
}
