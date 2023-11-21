using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CPF.Drawing;
using SkiaSharp;

namespace CPF.Skia
{
    internal class WglContext : GlContext
    {
        public const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
        public const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
        public const int WGL_CONTEXT_LAYER_PLANE_ARB = 0x2093;
        public const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;
        public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;
        private static readonly object fLock = new object();

        //private static ushort gWC;

        //private static IntPtr fWindow;
        //private static IntPtr fDeviceContext;

        //private IntPtr fPbuffer;
        private IntPtr fPbufferDC;
        private IntPtr fPbufferGlContext;

        static WglContext()
        {
            //var wc = new WNDCLASS
            //{
            //	cbClsExtra = 0,
            //	cbWndExtra = 0,
            //	hbrBackground = IntPtr.Zero,
            //	hCursor = User32.LoadCursor(IntPtr.Zero, (int)User32.IDC_ARROW),
            //	hIcon = User32.LoadIcon(IntPtr.Zero, (IntPtr)User32.IDI_APPLICATION),
            //	hInstance = Kernel32.CurrentModuleHandle,
            //	lpfnWndProc = (WNDPROC)User32.DefWindowProc,
            //	lpszClassName = "Griffin",
            //	lpszMenuName = null,
            //	style = User32.CS_HREDRAW | User32.CS_VREDRAW | User32.CS_OWNDC
            //};

            //gWC = User32.RegisterClass(ref wc);
            //if (gWC == 0)
            //{
            //    throw new Exception("Could not register window class.");
            //}

            //fWindow = User32.CreateWindow(
            //	"Griffin",
            //	"The Invisible Man",
            //	WindowStyles.WS_OVERLAPPEDWINDOW,
            //	0, 0,
            //	1, 1,
            //	IntPtr.Zero, IntPtr.Zero, Kernel32.CurrentModuleHandle, IntPtr.Zero);
            //if (fWindow == IntPtr.Zero)
            //{
            //	throw new Exception($"Could not create window.");
            //}

            //fDeviceContext = User32.GetDC(fWindow);
            //if (fDeviceContext == IntPtr.Zero)
            //{
            //    DestroyWindow();
            //    throw new Exception("Could not get device context.");
            //}

            //if (!Wgl.HasExtension(fDeviceContext, "WGL_ARB_pixel_format") ||
            //    !Wgl.HasExtension(fDeviceContext, "WGL_ARB_pbuffer"))
            //{
            //    DestroyWindow();
            //    throw new Exception("DC does not have extensions.");
            //}
        }

        public WglContext(IRenderTarget renderTarget)
        {
            //        var iAttrs = new int[]
            //        {
            //            Wgl.WGL_ACCELERATION_ARB, Wgl.WGL_FULL_ACCELERATION_ARB,
            //            Wgl.WGL_DRAW_TO_WINDOW_ARB, Wgl.TRUE,
            ////Wgl.WGL_DOUBLE_BUFFER_ARB, (doubleBuffered ? TRUE : FALSE),
            //Wgl.WGL_SUPPORT_OPENGL_ARB, Wgl.TRUE,
            //            Wgl.WGL_RED_BITS_ARB, 8,
            //            Wgl.WGL_GREEN_BITS_ARB, 8,
            //            Wgl.WGL_BLUE_BITS_ARB, 8,
            //            Wgl.WGL_ALPHA_BITS_ARB, 8,
            //            Wgl.WGL_STENCIL_BITS_ARB, 8,
            //            Wgl.NONE, Wgl.NONE
            //        };
            //        var piFormats = new int[1];
            //        uint nFormats;
            //        lock (fLock)
            //        {
            //            // HACK: This call seems to cause deadlocks on some systems.
            //            Wgl.wglChoosePixelFormatARB(fDeviceContext, iAttrs, null, (uint)piFormats.Length, piFormats, out nFormats);
            //        }
            //        if (nFormats == 0)
            //        {
            //            Dispose();
            //            throw new Exception("Could not get pixel formats.");
            //        }

            //        fPbuffer = Wgl.wglCreatePbufferARB(fDeviceContext, piFormats[0], 1, 1, null);
            //        if (fPbuffer == IntPtr.Zero)
            //        {
            //            Dispose();
            //            throw new Exception("Could not create Pbuffer.");
            //        }

            //        fPbufferDC = Wgl.wglGetPbufferDCARB(fPbuffer);
            //        if (fPbufferDC == IntPtr.Zero)
            //        {
            //            Dispose();
            //            throw new Exception("Could not get Pbuffer DC.");
            //        }

            //        var prevDC = Wgl.wglGetCurrentDC();
            //        var prevGLRC = Wgl.wglGetCurrentContext();
            //        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //        stopwatch.Start();

            //        fPbufferGlContext = Wgl.wglCreateContext(fPbufferDC);

            //        stopwatch.Stop();
            //        System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);

            //        Wgl.wglMakeCurrent(prevDC, prevGLRC);
            //        if (fPbufferGlContext == IntPtr.Zero)
            //        {
            //            Dispose();
            //            throw new Exception("Could not creeate Pbuffer GL context.");
            //        }

            var dc = ((CPF.Platform.HDCRenderTarget)renderTarget).Hdc;
            var dummyPFD = new PIXELFORMATDESCRIPTOR();
            dummyPFD.nSize = (ushort)Marshal.SizeOf(dummyPFD);
            dummyPFD.nVersion = 1;
            dummyPFD.dwFlags = Gdi32.PFD_DRAW_TO_WINDOW | Gdi32.PFD_SUPPORT_OPENGL;
            dummyPFD.iPixelType = Gdi32.PFD_TYPE_RGBA;
            dummyPFD.cColorBits = 24;
            dummyPFD.cDepthBits = 32;
            dummyPFD.cStencilBits = 0;
            dummyPFD.iLayerType = Gdi32.PFD_MAIN_PLANE;
            var dummyFormat = Gdi32.ChoosePixelFormat(dc, ref dummyPFD);
            Gdi32.SetPixelFormat(dc, dummyFormat, ref dummyPFD);

            var rc = Wgl.wglCreateContext(dc);
            //var rc = Wgl.WglCreateContextAttribsArb(dc, IntPtr.Zero,
            //            new[]
            //            {
            //                // major
            //                WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
            //                // minor
            //                WGL_CONTEXT_MINOR_VERSION_ARB,  0,
            //                // core profile
            //                WGL_CONTEXT_PROFILE_MASK_ARB, 1, 
            //                // debug 
            //                // WGL_CONTEXT_FLAGS_ARB, 1,
            //                // end
            //                0, 0
            //            });
            if (rc == IntPtr.Zero)
            {
                throw new Exception("Could not create GL context.");
            }
            fPbufferDC = dc;
            fPbufferGlContext = rc;
        }

        public override void MakeCurrent()
        {
            if (!Wgl.wglMakeCurrent(fPbufferDC, fPbufferGlContext))
            {
                Dispose();
                throw new Exception("Could not set the context.");
            }
        }

        public override void SwapBuffers()
        {
            if (!Gdi32.SwapBuffers(fPbufferDC))
            {
                Dispose();
                throw new Exception("Could not complete SwapBuffers.");
            }
        }

        public override void Dispose()
        {
            if (!Wgl.HasExtension(fPbufferDC, "WGL_ARB_pbuffer"))
            {
                // ASSERT
            }

            Wgl.wglDeleteContext(fPbufferGlContext);

            //Wgl.wglReleasePbufferDCARB?.Invoke(fPbuffer, fPbufferDC);

            //Wgl.wglDestroyPbufferARB?.Invoke(fPbuffer);
        }


        public override GRGlTextureInfo CreateTexture(SKSizeI textureSize)
        {
            var textures = new uint[1];
            Wgl.glGenTextures(textures.Length, textures);
            var textureId = textures[0];

            Wgl.glBindTexture(Wgl.GL_TEXTURE_2D, textureId);
            Wgl.glTexImage2D(Wgl.GL_TEXTURE_2D, 0, Wgl.GL_RGBA, textureSize.Width, textureSize.Height, 0, Wgl.GL_RGBA, Wgl.GL_UNSIGNED_BYTE, IntPtr.Zero);
            Wgl.glBindTexture(Wgl.GL_TEXTURE_2D, 0);

            return new GRGlTextureInfo
            {
                Id = textureId,
                Target = Wgl.GL_TEXTURE_2D,
                Format = Wgl.GL_RGBA8
            };
        }

        public override void DestroyTexture(uint texture)
        {
            Wgl.glDeleteTextures(1, new[] { texture });
        }

        public override void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil)
        {
            Wgl.glGetIntegerv(GL_FRAMEBUFFER_BINDING, out framebuffer);
            Wgl.glGetIntegerv(3415, out stencil);
            Wgl.glGetIntegerv(32937, out samples);
        }
    }
}
