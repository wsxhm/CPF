using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CPF.Drawing;
using SkiaSharp;

namespace CPF.Linux.OpenGL
{
    internal class GlxContext : CPF.OpenGL.IGlContext
    {
        private IntPtr fDisplay;
        //private IntPtr fPixmap;
        //private IntPtr fGlxPixmap;
        private IntPtr fContext;
        IntPtr handle;

        public GlxContext(X11Window window)
        {
            this.handle = window.Handle;
            //fDisplay = XLib.XOpenDisplay(IntPtr.Zero);
            //if (fDisplay == IntPtr.Zero) {
            //	Dispose();
            //	throw new Exception("Failed to open X display.");
            //}
            fDisplay = LinuxPlatform.Platform.Display;

            var visualAttribs = new[] {
                Glx.GLX_X_RENDERABLE, Glx.True,
                Glx.GLX_DRAWABLE_TYPE, Glx.GLX_WINDOW_BIT | Glx.GLX_PBUFFER_BIT,
                Glx.GLX_RENDER_TYPE, Glx.GLX_RGBA_BIT,
                Glx.GLX_DOUBLEBUFFER, Glx.True,
                Glx.GLX_RED_SIZE, 8,
                Glx.GLX_GREEN_SIZE, 8,
                Glx.GLX_BLUE_SIZE, 8,
                Glx.GLX_ALPHA_SIZE, 8,
                Glx.GLX_DEPTH_SIZE, 1,
                Glx.GLX_STENCIL_SIZE, 8,
				// Glx.GLX_SAMPLE_BUFFERS, 1,
				// Glx.GLX_SAMPLES, 4,
				Glx.None
            };
            try
            {
                int glxMajor, glxMinor;

                if (!Glx.glXQueryVersion(fDisplay, out glxMajor, out glxMinor) ||
                    (glxMajor < 1) ||
                    (glxMajor == 1 && glxMinor < 3))
                {
                    Console.WriteLine($"GLX version 1.3 or higher required ({glxMajor}.{glxMinor} provided).");
                    return;
                }

                var fbc = Glx.ChooseFBConfig(fDisplay, LinuxPlatform.Platform.Info.DefaultScreen, visualAttribs);
                if (fbc.Length == 0)
                {
                    Console.WriteLine("Failed to retrieve a framebuffer config.");
                    return;
                }

                var bestFBC = IntPtr.Zero;
                var bestNumSamp = -1;
                for (int i = 0; i < fbc.Length; i++)
                {
                    int sampleBuf, samples;
                    Glx.glXGetFBConfigAttrib(fDisplay, fbc[i], Glx.GLX_SAMPLE_BUFFERS, out sampleBuf);
                    Glx.glXGetFBConfigAttrib(fDisplay, fbc[i], Glx.GLX_SAMPLES, out samples);

                    var visual = Glx.GetVisualFromFBConfig(fDisplay, fbc[i]);
                    if (bestFBC == IntPtr.Zero || (sampleBuf > 0 && samples > bestNumSamp || visual.depth == 32))
                    {
                        bestFBC = fbc[i];
                        bestNumSamp = samples;
                    }
                }
                fContext = Glx.glXCreateNewContext(fDisplay, bestFBC, Glx.GLX_RGBA_TYPE, IntPtr.Zero, Glx.True);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public IntPtr Context { get { return fContext; } }

        IntPtr oldHandle;
        IntPtr oldDisplay;
        IntPtr oldContext;
        public void MakeCurrent()
        {
            oldContext = Glx.glXGetCurrentContext();
            oldDisplay = Glx.glXGetCurrentDisplay();
            oldHandle = Glx.glXGetCurrentDrawable();
            if (!Glx.glXMakeCurrent(fDisplay, handle, fContext))
            {
                Dispose();
                throw new Exception("Failed to set the context.");
            }
        }

        public void SwapBuffers()
        {
            Glx.glXSwapBuffers(fDisplay, handle);
            Glx.glXMakeCurrent(oldDisplay, oldHandle, oldContext);
        }

        public void Dispose()
        {
            if (fDisplay != IntPtr.Zero)
            {
                //Glx.glXMakeCurrent(fDisplay, IntPtr.Zero, IntPtr.Zero);

                if (fContext != IntPtr.Zero)
                {
                    Glx.glXDestroyContext(fDisplay, fContext);
                    fContext = IntPtr.Zero;
                }

                //if (fGlxPixmap != IntPtr.Zero) {
                //	Glx.glXDestroyGLXPixmap(fDisplay, fGlxPixmap);
                //	fGlxPixmap = IntPtr.Zero;
                //}

                //if (fPixmap != IntPtr.Zero) {
                //	XLib.XFreePixmap(fDisplay, fPixmap);
                //	fPixmap = IntPtr.Zero;
                //}

                fDisplay = IntPtr.Zero;
            }
            GRContext?.Dispose();
            GRContext = null;
        }
        public IntPtr GetProcAddress(string name)
        {
            return Glx.glXGetProcAddressARB(name);
        }
        //public override GRGlTextureInfo CreateTexture(SKSizeI textureSize)
        //{
        //	var textures = new uint[1];
        //	Glx.glGenTextures(textures.Length, textures);
        //	var textureId = textures[0];

        //	Glx.glBindTexture(Glx.GL_TEXTURE_2D, textureId);
        //	Glx.glTexImage2D(Glx.GL_TEXTURE_2D, 0, Glx.GL_RGBA, textureSize.Width, textureSize.Height, 0, Glx.GL_RGBA, Glx.GL_UNSIGNED_BYTE, IntPtr.Zero);
        //	Glx.glBindTexture(Glx.GL_TEXTURE_2D, 0);

        //	return new GRGlTextureInfo {
        //		Id = textureId,
        //		Target = Glx.GL_TEXTURE_2D,
        //		Format = Glx.GL_RGBA8
        //	};
        //}

        //public override void DestroyTexture(uint texture)
        //{
        //	Glx.glDeleteTextures(1, new[] { texture });
        //}
        public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;

        public IDisposable GRContext { get; set; }

        public void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil)
        {
            Glx.glGetIntegerv(GL_FRAMEBUFFER_BINDING, out framebuffer);
            Glx.glGetIntegerv(3415, out stencil);
            Glx.glGetIntegerv(32937, out samples);
        }
    }
}
