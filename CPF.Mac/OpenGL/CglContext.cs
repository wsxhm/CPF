using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CPF.Drawing;
using CPF.Mac.AppKit;
using CPF.OpenGL;
using CPF.Platform;

namespace CPF.Mac.OpenGL
{
	internal class CglContext : IGlContext
	{
		//private IntPtr fContext;
		NSOpenGLView view;
		public CglContext(NSOpenGLView view)
		{
			this.view = view;
			//var attributes = new [] {
			//	CglPixelFormatAttribute.kCGLPFAOpenGLProfile, (CglPixelFormatAttribute)CGLOpenGLProfile.kCGLOGLPVersion_3_2_Core,
			//	CglPixelFormatAttribute.kCGLPFADoubleBuffer, 
			//	CglPixelFormatAttribute.kCGLPFANone
			//};

			//IntPtr pixFormat;
			//int npix;

			//Cgl.CGLChoosePixelFormat(attributes, out pixFormat, out npix);

			//if (pixFormat == IntPtr.Zero) {
			//	throw new Exception("CGLChoosePixelFormat failed.");
			//}

			//Cgl.CGLCreateContext(pixFormat, IntPtr.Zero, out fContext);
			//Cgl.CGLReleasePixelFormat(pixFormat);

			//if (fContext == IntPtr.Zero) {
			//	throw new Exception("CGLCreateContext failed.");
			//}
		}

		public IntPtr GetProcAddress(string name)
		{
			return Cgl.GetProcAddress(name);
		}

		public void MakeCurrent()
		{
			view.OpenGLContext.MakeCurrentContext();
			//Cgl.CGLSetCurrentContext(fContext);
		}

		public void SwapBuffers()
		{
			//view.OpenGLContext.FlushBuffer();
			//Cgl.CGLFlushDrawable(fContext);
			Cgl.glFlush();
		}

		public void Dispose()
		{
			GRContext?.Dispose();
			GRContext = null;
			//if (fContext != IntPtr.Zero) {
			//	Cgl.CGLReleaseContext(fContext);
			//	fContext = IntPtr.Zero;
			//}
		}

		//public override GRGlTextureInfo CreateTexture(SKSizeI textureSize)
		//{
		//	var textures = new uint[1];
		//	Cgl.glGenTextures(textures.Length, textures);
		//	var textureId = textures[0];

		//	Cgl.glBindTexture(Cgl.GL_TEXTURE_2D, textureId);
		//	Cgl.glTexImage2D(Cgl.GL_TEXTURE_2D, 0, Cgl.GL_RGBA, textureSize.Width, textureSize.Height, 0, Cgl.GL_RGBA, Cgl.GL_UNSIGNED_BYTE, IntPtr.Zero);
		//	Cgl.glBindTexture(Cgl.GL_TEXTURE_2D, 0);

		//	return new GRGlTextureInfo {
		//		Id = textureId,
		//		Target = Cgl.GL_TEXTURE_2D,
		//		Format = Cgl.GL_RGBA8
		//	};
		//}

		//public override void DestroyTexture(uint texture)
		//{
		//	Cgl.glDeleteTextures(1, new[] { texture });
		//}
		public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;

        public IDisposable GRContext { get; set; }

        public void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil)
		{
			Cgl.glGetIntegerv(GL_FRAMEBUFFER_BINDING, out framebuffer);
			Cgl.glGetIntegerv(3415, out stencil);
			Cgl.glGetIntegerv(32937, out samples);
		}
	}
}
