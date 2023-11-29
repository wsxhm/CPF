using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CPF.Drawing;

namespace CPF.OpenGL
{
    public interface IGlContext : IDisposable
    {
        /// <summary>
        /// 用来获取和保存Skia创建的GRContext
        /// </summary>
        IDisposable GRContext { get; set; }
        void MakeCurrent();
        //void SwapBuffers();
        //void Dispose();
        //public abstract GRGlTextureInfo CreateTexture(SKSizeI textureSize);
        //public abstract void DestroyTexture(uint texture);
        /// <summary>
        /// 获取默认帧缓存信息
        /// </summary>
        /// <param name="framebuffer"></param>
        /// <param name="samples"></param>
        /// <param name="stencil"></param>
        void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil);
        /// <summary>
        /// OpenGL里获取函数地址的方法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IntPtr GetProcAddress(string name);
    }

    public static class OpenglEx
    {
        static bool loaded;
        public static void Load(IGlContext context)
        {
            if (!loaded)
            {
                if (context == null)
                {
                    throw new Exception("请开启硬件加速");
                }
                var fs = typeof(OpenglEx).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var item in fs)
                {
                    if (!item.FieldType.IsValueType)
                    {
                        var attr = item.GetCustomAttributes(typeof(GlImportAttribute), true);
                        if (attr != null && attr.Length > 0)
                        {
                            var name = (attr[0] as GlImportAttribute).Name;
                            var ptr = context.GetProcAddress(name);
                            if (ptr == IntPtr.Zero)
                            {
                                Console.WriteLine("无法加载OpenGL方法：" + name);
                                System.Diagnostics.Debug.WriteLine("无法加载OpenGL方法：" + name);
                            }
                            else
                            {
                                item.SetValue(null, Marshal.GetDelegateForFunctionPointer(ptr, item.FieldType));
                            }
                        }
                    }
                }
                loaded = true;
            }
        }

        public delegate int GlGetError();
        [GlImport("glGetError")]
        static GlGetError getError;

        public static int GetError(this IGlContext context)
        {
            Load(context);
            return getError();
        }


        public delegate void GlClearStencil(int s);
        [GlImport("glClearStencil")]
        static GlClearStencil clearStencil;
        public static void ClearStencil(this IGlContext context, int s)
        {
            Load(context);
            clearStencil(s);
        }

        public delegate void GlClearColor(float r, float g, float b, float a);
        [GlImport("glClearColor")]
        static GlClearColor clearColor;
        public static void ClearColor(this IGlContext context, float r, float g, float b, float a)
        {
            Load(context);
            clearColor(r, g, b, a);
        }

        public delegate void GlClear(int bits);
        [GlImport("glClear")]
        static GlClear clear;
        public static void Clear(this IGlContext context, int bits)
        {
            Load(context);
            clear(bits);
        }

        public delegate void GlViewport(int x, int y, int width, int height);
        [GlImport("glViewport")]
        static GlViewport viewport;
        public static void Viewport(this IGlContext context, int x, int y, int width, int height)
        {
            Load(context);
            viewport(x, y, width, height);
        }

        [GlImport("glFlush")]
        static Action flush;
        public static void Flush(this IGlContext context)
        {
            Load(context);
            flush();
        }

        //[GlImport("glFinish")]
        //static Action finish;
        //public static void Finish(this IGlContext context)
        //{
        //    Load(context);
        //    finish();
        //}

        public delegate IntPtr GlGetString(int v);
        [GlImport("glGetString")]
        static GlGetString getStringNative;
        public static string GetString(this IGlContext context, int v)
        {
            Load(context);
            var ptr = getStringNative(v);
            if (ptr != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(ptr);
            return null;
        }


        public delegate void GlGetIntegerv(int name, out int rv);
        [GlImport("glGetIntegerv")]
        static GlGetIntegerv getIntegerv;
        public static void GetIntegerv(this IGlContext context, int name, out int rv)
        {
            Load(context);
            getIntegerv(name, out rv);
        }

        public delegate void GlGenFramebuffers(int count, int[] res);
        [GlImport("glGenFramebuffers")]
        static GlGenFramebuffers genFramebuffers;
        public static void GenFramebuffers(this IGlContext context, int count, int[] res)
        {
            Load(context);
            genFramebuffers(count, res);
        }

        public static int GenFramebuffer(this IGlContext context)
        {
            int[] fbs = new int[1];
            context.GenFramebuffers(1, fbs);
            return fbs[0];
        }

        public delegate void GlDeleteFramebuffers(int count, int[] framebuffers);
        [GlImport("glDeleteFramebuffers")]
        static GlDeleteFramebuffers deleteFramebuffers;
        public static void DeleteFramebuffers(this IGlContext context, int count, int[] framebuffers)
        {
            Load(context);
            deleteFramebuffers(count, framebuffers);
        }

        public delegate void GlBindFramebuffer(int target, int fb);
        [GlImport("glBindFramebuffer")]
        static GlBindFramebuffer bindFramebuffer;
        public static void BindFramebuffer(this IGlContext context, int target, int fb)
        {
            Load(context);
            bindFramebuffer(target, fb);
        }

        public delegate int GlCheckFramebufferStatus(int target);
        [GlImport("glCheckFramebufferStatus")]
        static GlCheckFramebufferStatus checkFramebufferStatus;
        public static int CheckFramebufferStatus(this IGlContext context, int target)
        {
            Load(context);
            return checkFramebufferStatus(target);
        }

        //    public delegate void GlBlitFramebuffer(int srcX0,
        //int srcY0,
        //int srcX1,
        //int srcY1,
        //int dstX0,
        //int dstY0,
        //int dstX1,
        //int dstY1,
        //int mask,
        //int filter);
        //    [GlMinVersionEntryPoint("glBlitFramebuffer", 3, 0), GlOptionalEntryPoint]
        //    static GlBlitFramebuffer BlitFramebuffer ;

        public delegate void GlGenRenderbuffers(int count, int[] res);
        [GlImport("glGenRenderbuffers")]
        static GlGenRenderbuffers genRenderbuffers;
        public static void GenRenderbuffers(this IGlContext context, int count, int[] res)
        {
            Load(context);
            genRenderbuffers(count, res);
        }
        public static int GenRenderbuffer(this IGlContext context)
        {
            var res = new int[1];
            context.GenRenderbuffers(1, res);
            return res[0];
        }

        public delegate void GlDeleteRenderbuffers(int count, int[] renderbuffers);
        [GlImport("glDeleteRenderbuffers")]
        static GlDeleteTextures deleteRenderbuffers;
        public static void DeleteRenderbuffers(this IGlContext context, int count, int[] renderbuffers)
        {
            Load(context);
            deleteRenderbuffers(count, renderbuffers);
        }

        public delegate void GlBindRenderbuffer(int target, int fb);
        [GlImport("glBindRenderbuffer")]
        static GlBindRenderbuffer bindRenderbuffer;
        public static void BindRenderbuffer(this IGlContext context, int target, int fb)
        {
            Load(context);
            bindRenderbuffer(target, fb);
        }

        public delegate void GlRenderbufferStorage(int target, int internalFormat, int width, int height);
        [GlImport("glRenderbufferStorage")]
        static GlRenderbufferStorage renderbufferStorage;
        public static void RenderbufferStorage(this IGlContext context, int target, int internalFormat, int width, int height)
        {
            Load(context);
            renderbufferStorage(target, internalFormat, width, height);
        }

        public delegate void GlFramebufferRenderbuffer(int target, int attachment,
            int renderbufferTarget, int renderbuffer);
        [GlImport("glFramebufferRenderbuffer")]
        static GlFramebufferRenderbuffer framebufferRenderbuffer;
        public static void FramebufferRenderbuffer(this IGlContext context, int target, int attachment,
            int renderbufferTarget, int renderbuffer)
        {
            Load(context);
            framebufferRenderbuffer(target, attachment, renderbufferTarget, renderbuffer);
        }

        public delegate void GlGenTextures(int count, int[] res);
        [GlImport("glGenTextures")]
        static GlGenTextures genTextures;
        public static void GenTextures(this IGlContext context, int count, int[] res)
        {
            Load(context);
            genTextures(count, res);
        }
        public static int GenTexture(this IGlContext context)
        {
            var res = new int[1];
            context.GenTextures(1, res);
            return res[0];
        }


        public delegate void GlBindTexture(int target, int fb);
        [GlImport("glBindTexture")]
        static GlBindTexture bindTexture;
        public static void BindTexture(this IGlContext context, int target, int fb)
        {
            Load(context);
            bindTexture(target, fb);
        }

        public delegate void GlActiveTexture(int texture);
        [GlImport("glActiveTexture")]
        static GlActiveTexture activeTexture;
        public static void ActiveTexture(this IGlContext context, int texture)
        {
            Load(context);
            activeTexture(texture);
        }

        public delegate void GlDeleteTextures(int count, int[] textures);
        [GlImport("glDeleteTextures")]
        static GlDeleteTextures deleteTextures;
        public static void DeleteTextures(this IGlContext context, int count, int[] textures)
        {
            Load(context);
            deleteTextures(count, textures);
        }


        public delegate void GlTexImage2D(int target, int level, int internalFormat, int width, int height, int border,
            int format, int type, IntPtr data);
        [GlImport("glTexImage2D")]
        static GlTexImage2D texImage2D;
        public static void TexImage2D(this IGlContext context, int target, int level, int internalFormat, int width, int height, int border,
            int format, int type, IntPtr data)
        {
            Load(context);
            texImage2D(target, level, internalFormat, width, height, border,
              format, type, data);
        }

        public delegate void GlCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y,
            int width, int height);

        [GlImport("glCopyTexSubImage2D")]
        static GlCopyTexSubImage2D copyTexSubImage2D;
        public static void CopyTexSubImage2D(this IGlContext context, int target, int level, int xoffset, int yoffset, int x, int y,
            int width, int height)
        {
            Load(context);
            copyTexSubImage2D(target, level, xoffset, yoffset, x, y,
              width, height);
        }

        public delegate void GlTexParameteri(int target, int name, int value);
        [GlImport("glTexParameteri")]
        static GlTexParameteri texParameteri;
        public static void TexParameteri(this IGlContext context, int target, int name, int value)
        {
            Load(context);
            texParameteri(target, name, value);
        }

        public delegate void GlFramebufferTexture2D(int target, int attachment,
            int texTarget, int texture, int level);
        [GlImport("glFramebufferTexture2D")]
        static GlFramebufferTexture2D framebufferTexture2D;
        public static void FramebufferTexture2D(this IGlContext context, int target, int attachment,
            int texTarget, int texture, int level)
        {
            Load(context);
            framebufferTexture2D(target, attachment,
              texTarget, texture, level);
        }

        public delegate int GlCreateShader(int shaderType);
        [GlImport("glCreateShader")]
        static GlCreateShader createShader;
        public static void CreateShader(this IGlContext context, int shaderType)
        {
            Load(context);
            createShader(shaderType);
        }

        public delegate void GlEnable(int what);
        [GlImport("glEnable")]
        static GlEnable enable;
        public static void Enable(this IGlContext context, int what)
        {
            Load(context);
            enable(what);
        }

        public delegate void GlDeleteBuffers(int count, int[] buffers);
        [GlImport("glDeleteBuffers")]
        static GlDeleteBuffers deleteBuffers;
        public static void DeleteBuffers(this IGlContext context, int count, int[] buffers)
        {
            Load(context);
            deleteBuffers(count, buffers);
        }

        public delegate void GlDeleteProgram(int program);
        [GlImport("glDeleteProgram")]
        static GlDeleteProgram deleteProgram;
        public static void DeleteProgram(this IGlContext context, int program)
        {
            Load(context);
            deleteProgram(program);
        }

        public delegate void GlDeleteShader(int shader);
        [GlImport("glDeleteShader")]
        static GlDeleteShader deleteShader;
        public static void DeleteShader(this IGlContext context, int shader)
        {
            Load(context);
            deleteShader(shader);
        }

        //public delegate void GlColor4f(float red, float green, float blue, float alpha);
        //[GlImport("glColor4f")]
        //static GlColor4f color4f;
        //public static void Color4f(this IGlContext context, float red, float green, float blue, float alpha)
        //{
        //    Load(context);
        //    color4f(red, green, blue, alpha);
        //}

        //public delegate void GlBegin(uint mode);
        //[GlImport("glBegin")]
        //static GlBegin begin;
        //public static void Begin(this IGlContext context, uint mode)
        //{
        //    Load(context);
        //    begin(mode);
        //}

        //public delegate void GlVertex3f(float x, float y, float z);
        //[GlImport("glVertex3f")]
        //static GlVertex3f vertex3f;
        //public static void Vertex3f(this IGlContext context, float x, float y, float z)
        //{
        //    Load(context);
        //    vertex3f(x, y, z);
        //}

        //public delegate void GlEnd();
        //[GlImport("glEnd")]
        //static GlEnd end;
        //public static void End(this IGlContext context)
        //{
        //    Load(context);
        //    end();
        //}

        public delegate void GlLoadIdentity();
        [GlImport("glLoadIdentity")]
        static GlLoadIdentity loadIdentity;
        public static void LoadIdentity(this IGlContext context)
        {
            Load(context);
            loadIdentity();
        }

        public delegate void GlPushMatrix();
        [GlImport("glPushMatrix")]
        static GlPushMatrix pushMatrix;
        public static void PushMatrix(this IGlContext context)
        {
            Load(context);
            pushMatrix();
        }

        public delegate void GlPopMatrix();
        [GlImport("glPopMatrix")]
        static GlPopMatrix popMatrix;
        public static void PopMatrix(this IGlContext context)
        {
            Load(context);
            popMatrix();
        }
        public delegate void GlMatrixMode(uint mode);
        [GlImport("glMatrixMode")]
        static GlMatrixMode matrixMode;
        public static void MatrixMode(this IGlContext context, uint mode)
        {
            Load(context);
            matrixMode(mode);
        }

        public delegate void GlGetTexImage(uint target, int level, uint format, uint type, byte[] pixels);
        [GlImport("glGetTexImage")]
        static GlGetTexImage getTexImage;
        public static void GetTexImage(this IGlContext context, uint target, int level, uint format, uint type, byte[] pixels)
        {
            Load(context);
            getTexImage(target, level, format, type, pixels);
        }


        public delegate void GlPushAttrib(uint mask);
        [GlImport("glPushAttrib")]
        static GlPushAttrib pushAttrib;
        public static void PushAttrib(this IGlContext context, uint mask)
        {
            Load(context);
            pushAttrib(mask);
        }

        public delegate void GlPopAttrib();
        [GlImport("glPopAttrib")]
        static GlPopAttrib popAttrib;
        public static void PopAttrib(this IGlContext context)
        {
            Load(context);
            popAttrib();
        }

        //public delegate void GlOrtho(double left, double right, double bottom, double top, double zNear, double zFar);
        //[GlImport("glOrtho")]
        //static GlOrtho ortho;
        //public static void Ortho(this IGlContext context, double left, double right, double bottom, double top, double zNear, double zFar)
        //{
        //    Load(context);
        //    ortho(left, right, bottom, top, zNear, zFar);
        //}

        public delegate void GlGetFloatv(uint pname, float[] params_notkeyword);
        [GlImport("glGetFloatv")]
        static GlGetFloatv getFloatv;
        public static void GetFloatv(this IGlContext context, uint pname, float[] params_notkeyword)
        {
            Load(context);
            getFloatv(pname, params_notkeyword);
        }
    }

    //public delegate IntPtr GetProcAddressDelegate(string procName);
}
