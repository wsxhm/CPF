using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CPF.Mac.OpenGL
{
    internal class Cgl
    {
        private const string libGL = "/System/Library/Frameworks/OpenGL.framework/Versions/A/OpenGL";

        public const int GL_TEXTURE_2D = 0x0DE1;
        public const int GL_UNSIGNED_BYTE = 0x1401;
        public const int GL_RGBA = 0x1908;
        public const int GL_RGBA8 = 0x8058;
        [DllImport(libGL)]
        public static extern void glGetIntegerv(int name, out int rv);
        [DllImport(libGL)]
        public extern static void CGLGetVersion(out int majorvers, out int minorvers);
        [DllImport(libGL)]
        public extern static CGLError CGLChoosePixelFormat([In] CglPixelFormatAttribute[] attribs, out IntPtr pix, out int npix);
        [DllImport(libGL)]
        public extern static CGLError CGLCreateContext(IntPtr pix, IntPtr share, out IntPtr ctx);
        [DllImport(libGL)]
        public extern static CGLError CGLReleasePixelFormat(IntPtr pix);
        [DllImport(libGL)]
        public extern static CGLError CGLSetCurrentContext(IntPtr ctx);
        [DllImport(libGL)]
        public extern static void CGLReleaseContext(IntPtr ctx);
        [DllImport(libGL)]
        public extern static CGLError CGLFlushDrawable(IntPtr ctx);
        [DllImport(libGL)]
        public static extern void glGenTextures(int n, uint[] textures);
        [DllImport(libGL)]
        public static extern void glDeleteTextures(int n, uint[] textures);
        [DllImport(libGL)]
        public static extern void glBindTexture(uint target, uint texture);
        [DllImport(libGL)]
        public static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);
        [DllImport(libGL, SetLastError = true)]
        public static extern void glClearColor(float red, float green, float blue, float alpha);
        [DllImport(libGL, SetLastError = true)]
        public static extern void glFlush();
        [DllImport(libGL, SetLastError = true)]
        public static extern void glClear(uint mask);

        static IntPtr cglLib;
        public static IntPtr GetProcAddress(string name)
        {
            if (cglLib == IntPtr.Zero)
            {
                cglLib = dlopen(libGL, 1);
            }
            return dlsym(cglLib, name);
        }


        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern IntPtr dlopen(string path, int flags);

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern IntPtr dlerror();

    }
}
