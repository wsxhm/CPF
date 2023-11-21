using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static CPF.Windows.UnmanagedMethods;

namespace CPF.Windows.OpenGL
{
    public class Wgl
    {
        private const string opengl32 = "opengl32.dll";

        public const int NONE = 0;
        public const int FALSE = 0;
        public const int TRUE = 1;

        public const int GL_VENDOR = 0x1F00;
        public const int GL_VERSION = 0x1F02;
        public const int GL_RENDERER = 0x1F01;
        public const int GL_EXTENSIONS = 0x1F03;
        public const int GL_TEXTURE_2D = 0x0DE1;
        public const int GL_UNSIGNED_BYTE = 0x1401;
        public const int GL_RGBA = 0x1908;
        public const int GL_RGBA8 = 0x8058;

        public const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
        public const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
        public const int WGL_CONTEXT_LAYER_PLANE_ARB = 0x2093;
        public const int WGL_CONTEXT_FLAGS_ARB = 0x2094;
        public const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;

        public const int WGL_NUMBER_PIXEL_FORMATS_ARB = 0x2000;
        public const int WGL_DRAW_TO_WINDOW_ARB = 0x2001;
        public const int WGL_DRAW_TO_BITMAP_ARB = 0x2002;
        public const int WGL_ACCELERATION_ARB = 0x2003;
        public const int WGL_NEED_PALETTE_ARB = 0x2004;
        public const int WGL_NEED_SYSTEM_PALETTE_ARB = 0x2005;
        public const int WGL_SWAP_LAYER_BUFFERS_ARB = 0x2006;
        public const int WGL_SWAP_METHOD_ARB = 0x2007;
        public const int WGL_NUMBER_OVERLAYS_ARB = 0x2008;
        public const int WGL_NUMBER_UNDERLAYS_ARB = 0x2009;
        public const int WGL_TRANSPARENT_ARB = 0x200A;
        public const int WGL_TRANSPARENT_RED_VALUE_ARB = 0x2037;
        public const int WGL_TRANSPARENT_GREEN_VALUE_ARB = 0x2038;
        public const int WGL_TRANSPARENT_BLUE_VALUE_ARB = 0x2039;
        public const int WGL_TRANSPARENT_ALPHA_VALUE_ARB = 0x203A;
        public const int WGL_TRANSPARENT_INDEX_VALUE_ARB = 0x203B;
        public const int WGL_SHARE_DEPTH_ARB = 0x200C;
        public const int WGL_SHARE_STENCIL_ARB = 0x200D;
        public const int WGL_SHARE_ACCUM_ARB = 0x200E;
        public const int WGL_SUPPORT_GDI_ARB = 0x200F;
        public const int WGL_SUPPORT_OPENGL_ARB = 0x2010;
        public const int WGL_DOUBLE_BUFFER_ARB = 0x2011;
        public const int WGL_STEREO_ARB = 0x2012;
        public const int WGL_PIXEL_TYPE_ARB = 0x2013;
        public const int WGL_COLOR_BITS_ARB = 0x2014;
        public const int WGL_RED_BITS_ARB = 0x2015;
        public const int WGL_RED_SHIFT_ARB = 0x2016;
        public const int WGL_GREEN_BITS_ARB = 0x2017;
        public const int WGL_GREEN_SHIFT_ARB = 0x2018;
        public const int WGL_BLUE_BITS_ARB = 0x2019;
        public const int WGL_BLUE_SHIFT_ARB = 0x201A;
        public const int WGL_ALPHA_BITS_ARB = 0x201B;
        public const int WGL_ALPHA_SHIFT_ARB = 0x201C;
        public const int WGL_ACCUM_BITS_ARB = 0x201D;
        public const int WGL_ACCUM_RED_BITS_ARB = 0x201E;
        public const int WGL_ACCUM_GREEN_BITS_ARB = 0x201F;
        public const int WGL_ACCUM_BLUE_BITS_ARB = 0x2020;
        public const int WGL_ACCUM_ALPHA_BITS_ARB = 0x2021;
        public const int WGL_DEPTH_BITS_ARB = 0x2022;
        public const int WGL_STENCIL_BITS_ARB = 0x2023;
        public const int WGL_AUX_BUFFERS_ARB = 0x2024;
        public const int WGL_NO_ACCELERATION_ARB = 0x2025;
        public const int WGL_GENERIC_ACCELERATION_ARB = 0x2026;
        public const int WGL_FULL_ACCELERATION_ARB = 0x2027;
        public const int WGL_SWAP_EXCHANGE_ARB = 0x2028;
        public const int WGL_SWAP_COPY_ARB = 0x2029;
        public const int WGL_SWAP_UNDEFINED_ARB = 0x202A;
        public const int WGL_TYPE_RGBA_ARB = 0x202B;
        public const int WGL_TYPE_COLORINDEX_ARB = 0x202C;
        public const int WGL_SAMPLE_BUFFERS_ARB = 0x2041;
        public const int WGL_SAMPLES_ARB = 0x2042;

        static Wgl()
        {
            try
            {

                // save the current GL context
                var prevDC = Wgl.wglGetCurrentDC();
                var prevGLRC = Wgl.wglGetCurrentContext();

                // get the the dummy window bounds
                var windowRect = new RECT { left = 0, right = 8, top = 0, bottom = 8 };
                AdjustWindowRectEx(ref windowRect, (uint)WindowStyles.WS_SYSMENU, false, (uint)WindowStyles.WS_EX_CLIENTEDGE);

                //// register the dummy window class
                //var wc = new WNDCLASS
                //{
                //    style = (int)(ClassStyles.CS_HREDRAW | ClassStyles.CS_VREDRAW | ClassStyles.CS_OWNDC),
                //    lpfnWndProc = DefWindowProc,
                //    cbClsExtra = 0,
                //    cbWndExtra = 0,
                //    hInstance = GetModuleHandle(null),
                //    hCursor = WindowImpl.DefaultCursor,
                //    hIcon = LoadIcon(IntPtr.Zero, (IntPtr)32517),
                //    hbrBackground = IntPtr.Zero,
                //    lpszMenuName = null,
                //    lpszClassName = "DummyClass"
                //};
                //if (RegisterClass(ref wc) == 0)
                //{
                //    throw new Exception("Could not register dummy class.");
                //}
                WNDCLASSEX wc = new WNDCLASSEX();
                wc.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
                //wc.style = (int)ClassStyles.CS_DBLCLKS;
                wc.lpfnWndProc = DefWindowProc;
                wc.hInstance = GetModuleHandle(null);
                wc.hbrBackground = (IntPtr)6;
                wc.lpszClassName = "DummyClass";
                wc.cbClsExtra = 0;
                wc.cbWndExtra = 0;
                wc.hIcon = IntPtr.Zero;
                wc.hCursor = WindowImpl.DefaultCursor;
                wc.lpszMenuName = "";
                // 注册窗口类  
                RegisterClassEx(ref wc);

                // create the dummy window
                var dummyWND = CreateWindowEx(
                    (int)WindowStyles.WS_EX_CLIENTEDGE,
                    "DummyClass",
                    "DummyWindow",
                   (uint)(WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_SYSMENU),
                    0, 0,
                    windowRect.right - windowRect.left, windowRect.bottom - windowRect.top,
                    IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
                if (dummyWND == IntPtr.Zero)
                {
                    UnregisterClass("DummyClass", GetModuleHandle(null));
                    return;
                }

                // get the dummy DC
                var dummyDC = UnmanagedMethods.GetDC(dummyWND);

                // get the dummy pixel format
                var dummyPFD = new PIXELFORMATDESCRIPTOR();
                dummyPFD.nSize = (ushort)Marshal.SizeOf(dummyPFD);
                dummyPFD.nVersion = 1;
                dummyPFD.dwFlags = Wgl.PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_SUPPORT_COMPOSITION;
                dummyPFD.iPixelType = PFD_TYPE_RGBA;
                dummyPFD.cColorBits = 24;
                dummyPFD.cDepthBits = 32;
                dummyPFD.cStencilBits = 8;
                dummyPFD.cAlphaBits = 8;
                dummyPFD.iLayerType = PFD_MAIN_PLANE;
                var dummyFormat = ChoosePixelFormat(dummyDC, ref dummyPFD);
                var r = SetPixelFormat(dummyDC, dummyFormat, ref dummyPFD);

                // get the dummy GL context
                var dummyGLRC = Wgl.wglCreateContext(dummyDC);
                if (dummyGLRC == IntPtr.Zero)
                {
                    //throw new Exception("Could not create dummy GL context.");
                    System.Diagnostics.Debug.WriteLine("创建OpenGL上下文失败，请尝试设置当前程序以集显方式启动");
                    UnmanagedMethods.ReleaseDC(IntPtr.Zero, dummyDC);
                    return;
                }
                Wgl.wglMakeCurrent(dummyDC, dummyGLRC);

                VersionString = Wgl.GetString(Wgl.GL_VERSION);
                //var RENDERER = Wgl.GetString(GL_RENDERER);
                //var VENDOR = GetString(GL_VENDOR);
                //var EXTENSIONS = GetString(GL_EXTENSIONS);

                // get the extension methods using the dummy context
                wglGetExtensionsStringARB = Wgl.wglGetProcAddress<wglGetExtensionsStringARBDelegate>("wglGetExtensionsStringARB");
                wglChoosePixelFormatARB = Wgl.wglGetProcAddress<wglChoosePixelFormatARBDelegate>("wglChoosePixelFormatARB");
                wglCreatePbufferARB = Wgl.wglGetProcAddress<wglCreatePbufferARBDelegate>("wglCreatePbufferARB");
                wglDestroyPbufferARB = Wgl.wglGetProcAddress<wglDestroyPbufferARBDelegate>("wglDestroyPbufferARB");
                wglGetPbufferDCARB = Wgl.wglGetProcAddress<wglGetPbufferDCARBDelegate>("wglGetPbufferDCARB");
                wglReleasePbufferDCARB = Wgl.wglGetProcAddress<wglReleasePbufferDCARBDelegate>("wglReleasePbufferDCARB");
                wglSwapIntervalEXT = Wgl.wglGetProcAddress<wglSwapIntervalEXTDelegate>("wglSwapIntervalEXT");
                WglCreateContextAttribsArb = Wgl.wglGetProcAddress<WglCreateContextAttribsARBDelegate>("wglCreateContextAttribsARB");
                GlDebugMessageCallback =  wglGetProcAddress<GlDebugMessageCallbackDelegate>("glDebugMessageCallback");

                // destroy the dummy GL context 
                Wgl.wglMakeCurrent(dummyDC, IntPtr.Zero);
                Wgl.wglDeleteContext(dummyGLRC);
                UnmanagedMethods.ReleaseDC(IntPtr.Zero, dummyDC);

                // destroy the dummy window
                DestroyWindow(dummyWND);
                UnregisterClass("DummyClass", GetModuleHandle(null));

                // reset the initial GL context
                Wgl.wglMakeCurrent(prevDC, prevGLRC);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("OpenGL初始化出错:" + e);
                Console.WriteLine("OpenGL初始化出错:" + e);
            }
        }

        public static string VersionString { get; }

        //public static bool HasExtension(IntPtr dc, string ext)
        //{
        //    if (wglGetExtensionsStringARB == null)
        //    {
        //        return false;
        //    }

        //    if (ext == "WGL_ARB_extensions_string")
        //    {
        //        return true;
        //    }

        //    return Array.IndexOf(GetExtensionsARB(dc), ext) != -1;
        //}

        //public static string GetExtensionsStringARB(IntPtr dc)
        //{
        //    return Marshal.PtrToStringAnsi(wglGetExtensionsStringARB(dc));
        //}

        //public static string[] GetExtensionsARB(IntPtr dc)
        //{
        //    var str = GetExtensionsStringARB(dc);
        //    if (string.IsNullOrEmpty(str))
        //    {
        //        return new string[0];
        //    }
        //    return str.Split(' ');
        //}

        private static readonly DebugCallbackDelegate _debugCallback = DebugCallback;
        private static void DebugCallback(int source, int type, int id, int severity, int len, IntPtr message, IntPtr userparam)
        {
            var err = Marshal.PtrToStringAnsi(message, len);
            Console.Error.WriteLine(err);
            System.Diagnostics.Debug.WriteLine(err);
        }
        /// <summary>
        /// 注册debug消息,委托要自己保存，不要直接用临时的Lambda
        /// </summary>
        public static void GlDebug(DebugCallbackDelegate _debugCallback)
        {
            GlDebugMessageCallback(Marshal.GetFunctionPointerForDelegate(_debugCallback), IntPtr.Zero);
        }

        public static readonly wglGetExtensionsStringARBDelegate wglGetExtensionsStringARB;
        public static readonly wglChoosePixelFormatARBDelegate wglChoosePixelFormatARB;
        public static readonly wglCreatePbufferARBDelegate wglCreatePbufferARB;
        public static readonly wglDestroyPbufferARBDelegate wglDestroyPbufferARB;
        public static readonly wglGetPbufferDCARBDelegate wglGetPbufferDCARB;
        public static readonly wglReleasePbufferDCARBDelegate wglReleasePbufferDCARB;
        public static readonly wglSwapIntervalEXTDelegate wglSwapIntervalEXT;
        public static readonly WglCreateContextAttribsARBDelegate WglCreateContextAttribsArb;


        public delegate void GlDebugMessageCallbackDelegate(IntPtr callback, IntPtr userParam);

        public static GlDebugMessageCallbackDelegate GlDebugMessageCallback;

        public delegate void DebugCallbackDelegate(int source, int type, int id, int severity, int len, IntPtr message,
            IntPtr userParam);
        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr wglGetCurrentDC();

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr wglGetCurrentContext();

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr wglCreateContext(IntPtr hDC);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern bool wglMakeCurrent(IntPtr hDC, IntPtr hRC);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern bool wglDeleteContext(IntPtr hRC);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr wglGetProcAddress([MarshalAs(UnmanagedType.LPStr)] string lpszProc);

        [DllImport(opengl32, SetLastError = true)]
        public static extern void glClearColor(float red, float green, float blue, float alpha);
        public static T wglGetProcAddress<T>(string lpszProc)
        {
            var ptr = wglGetProcAddress(lpszProc);
            if (ptr == IntPtr.Zero)
            {
                return default(T);
            }
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr glGetString(uint value);

        public static string GetString(uint value)
        {
            var intPtr = glGetString(value);
            return Marshal.PtrToStringAnsi(intPtr);
        }

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern void glGetIntegerv(int name, out int rv);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern void glGenTextures(int n, uint[] textures);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern void glDeleteTextures(int n, uint[] textures);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern void glBindTexture(uint target, uint texture);

        [DllImport(opengl32, CallingConvention = CallingConvention.Winapi)]
        public static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);



        private const string gdi32 = "gdi32.dll";

        public const byte PFD_TYPE_RGBA = 0;

        public const byte PFD_MAIN_PLANE = 0;

        public const uint PFD_DOUBLEBUFFER = 0x00000001;
        public const uint PFD_DRAW_TO_WINDOW = 0x00000004;
        public const uint PFD_SUPPORT_OPENGL = 0x00000020;
        public const uint PFD_SUPPORT_COMPOSITION = 0x00008000;
        public const uint PFD_DRAW_TO_BITMAP = 8u;
        public const uint PFD_SUPPORT_GDI = 16u;

        [DllImport(gdi32, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport(gdi32, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport(gdi32, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwapBuffers(IntPtr hdc);
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr wglGetExtensionsStringARBDelegate(IntPtr dc);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool wglChoosePixelFormatARBDelegate(
        IntPtr dc,
        [In] int[] attribIList,
        [In] float[] attribFList,
        uint maxFormats,
        [Out] int[] pixelFormats,
        out uint numFormats);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr wglCreatePbufferARBDelegate(IntPtr dc, int pixelFormat, int width, int height, [In] int[] attribList);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool wglDestroyPbufferARBDelegate(IntPtr pbuffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr wglGetPbufferDCARBDelegate(IntPtr pbuffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int wglReleasePbufferDCARBDelegate(IntPtr pbuffer, IntPtr dc);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool wglSwapIntervalEXTDelegate(int interval);

    public delegate IntPtr WglCreateContextAttribsARBDelegate(IntPtr hDC, IntPtr hShareContext, int[] attribList);
}
