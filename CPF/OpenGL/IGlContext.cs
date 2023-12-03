using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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

    public static unsafe class OpenglEx
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

        public delegate void GlTexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, IntPtr pixels);
        [GlImport("glTexSubImage2D")]
        static GlTexSubImage2D texSubImage2D;
        public static void TexSubImage2D(this IGlContext context, uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, IntPtr pixels)
        {
            Load(context);
            texSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
        }

        public delegate void GlBufferData(int target​, int size​, IntPtr data​, int usage​);
        [GlImport("glBufferData")]
        static GlBufferData bufferData;
        public static void BufferData(this IGlContext context, int target​, int size​, IntPtr data​, int usage​)
        {
            Load(context);
            bufferData(target, size, data, usage);
        }

        public delegate void GlBindBuffer(int target, int buffer);
        [GlImport("glBindBuffer")]
        static GlBindBuffer bindBuffer;
        public static void BindBuffer(this IGlContext context, int target, int buffer​)
        {
            Load(context);
            bindBuffer(target, buffer);
        }

        public delegate void GlUniform1i(int location, int v0);
        [GlImport("glUniform1i")]
        static GlUniform1i uniform1i;
        public static void Uniform1(this IGlContext context, int location, int v0​)
        {
            Load(context);
            uniform1i(location, v0);
        }
        public delegate void GlUniform1f(int location, float v0);
        [GlImport("glUniform1f")]
        static GlUniform1f uniform1f;
        public static void Uniform1(this IGlContext context, int location, float v0​)
        {
            Load(context);
            uniform1f(location, v0);
        }

        public delegate void GlBufferSubData(int target, IntPtr offset, int size, IntPtr data);
        [GlImport("glBufferSubData")]
        static GlBufferSubData bufferSubData;
        public static void BufferSubData(this IGlContext context, int target, IntPtr offset, int size, IntPtr data)
        {
            Load(context);
            bufferSubData(target, offset, size, data);
        }


        public delegate void GlTexParameteri(int target, int name, int value);
        [GlImport("glTexParameteri")]
        static GlTexParameteri texParameteri;
        public static void TexParameter(this IGlContext context, int target, int name, int value)
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
        public static int CreateShader(this IGlContext context, int shaderType)
        {
            Load(context);
            return createShader(shaderType);
        }

        public delegate void GlShaderSource(int shader, int count, IntPtr str, int* length);
        [GlImport("glCreateShader")]
        static GlShaderSource shaderSource;
        public static void ShaderSource(this IGlContext context, int shader, string source)
        {
            Load(context);

            int length = source.Length;
            IntPtr intPtr = MarshalStringArrayToPtr(new string[] { source });
            shaderSource(shader, 1, intPtr, &length);
            FreeStringArrayPtr(intPtr, 1);
        }

        static IntPtr MarshalStringArrayToPtr(string[] str_array)
        {
            IntPtr intPtr = IntPtr.Zero;
            if (str_array != null && str_array.Length != 0)
            {
                intPtr = Marshal.AllocHGlobal(str_array.Length * IntPtr.Size);
                if (intPtr == IntPtr.Zero)
                {
                    throw new OutOfMemoryException();
                }

                int i = 0;
                try
                {
                    for (i = 0; i < str_array.Length; i++)
                    {
                        IntPtr val = MarshalStringToPtr(str_array[i]);
                        Marshal.WriteIntPtr(intPtr, i * IntPtr.Size, val);
                    }
                }
                catch (OutOfMemoryException)
                {
                    for (i--; i >= 0; i--)
                    {
                        Marshal.FreeHGlobal(Marshal.ReadIntPtr(intPtr, i * IntPtr.Size));
                    }

                    Marshal.FreeHGlobal(intPtr);
                    throw;
                }
            }

            return intPtr;
        }

        static IntPtr MarshalStringToPtr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return IntPtr.Zero;
            }

            int num = Encoding.UTF8.GetMaxByteCount(str.Length) + 1;
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            if (intPtr == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            fixed (char* chars = str)
            {
                int bytes = Encoding.UTF8.GetBytes(chars, str.Length, (byte*)(void*)intPtr, num);
                Marshal.WriteByte(intPtr, bytes, 0);
                return intPtr;
            }
        }
        static void FreeStringArrayPtr(IntPtr ptr, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Marshal.FreeHGlobal(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
            }

            Marshal.FreeHGlobal(ptr);
        }


        public delegate int GlCreateProgram();
        [GlImport("glCreateProgram")]
        static GlCreateProgram createProgram;
        public static int CreateProgram(this IGlContext context)
        {
            Load(context);
            return createProgram();
        }

        public delegate void GlAttachShader(int program, int shader);
        [GlImport("glAttachShader")]
        static GlAttachShader attachShader;
        public static void AttachShader(this IGlContext context, int program, int shader)
        {
            Load(context);
            attachShader(program, shader);
        }

        public delegate void GlDetachShader(int program, int shader);
        [GlImport("glDetachShader")]
        static GlDetachShader detachShader;
        public static void DetachShader(this IGlContext context, int program, int shader)
        {
            Load(context);
            detachShader(program, shader);
        }

        public delegate void GlGetProgramiv(int program, int pname, int* @params);
        [GlImport("glGetProgramiv")]
        static GlGetProgramiv getProgramiv;
        public static void GetProgram(this IGlContext context, int program, int pname, out int @params)
        {
            Load(context);
            fixed (int* ptr = &@params)
            {
                getProgramiv(program, pname, ptr);
            }
        }

        public delegate void GlGetProgramInfoLog(uint program, int bufSize, int* length, IntPtr infoLog);
        [GlImport("glGetProgramInfoLog")]
        static GlGetProgramInfoLog getProgramInfoLog;
        public static string GetProgramInfoLog(this IGlContext context, uint program)
        {
            Load(context);
            GetProgram(context, (int)program, GlConsts.GL_INFO_LOG_LENGTH, out var @params);
            if (@params == 0)
            {
                return string.Empty;
            }
            int bufSize = @params * 2;
            IntPtr intPtr = Marshal.AllocHGlobal((IntPtr)(bufSize + 1));
            getProgramInfoLog(program, bufSize, &@params, intPtr);
            var infoLog = MarshalPtrToString(intPtr);
            Marshal.FreeHGlobal(intPtr);
            return infoLog;
        }

        public static string GetActiveUniform(this IGlContext context, int program, int uniformIndex, out int size, out int type)
        {
            Load(context);
            GetProgram(context, program, GlConsts.GL_ACTIVE_UNIFORM_MAX_LENGTH, out var @params);
            GetActiveUniform(program, uniformIndex, (@params == 0) ? 1 : @params, out @params, out size, out type, out var name);
            return name;
        }

        public delegate void GlGetActiveUniform(int program, int index, int bufSize, int* length, int* size, int* type, IntPtr name);
        [GlImport("glGetActiveUniform")]
        static GlGetActiveUniform getActiveUniform;
        static void GetActiveUniform(int program, int index, int bufSize, out int length, out int size, out int type, out string name)
        {
            fixed (int* ptr = &length)
            {
                fixed (int* ptr2 = &size)
                {
                    fixed (int* ptr3 = &type)
                    {
                        IntPtr intPtr = Marshal.AllocHGlobal((IntPtr)(bufSize + 1));
                        getActiveUniform(program, index, bufSize, ptr, ptr2, ptr3, intPtr);
                        name = MarshalPtrToString(intPtr);
                        Marshal.FreeHGlobal(intPtr);
                    }
                }
            }
        }
        unsafe static string MarshalPtrToString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentException("ptr");
            }

            sbyte* ptr2 = (sbyte*)(void*)ptr;
            int num = 0;
            for (; *ptr2 != 0; ptr2++)
            {
                num++;
            }

            return new string((sbyte*)(void*)ptr, 0, num, Encoding.UTF8);
        }

        public delegate int GlGetUniformLocation(int program, IntPtr name);
        [GlImport("glGetUniformLocation")]
        static GlGetUniformLocation getUniformLocation;
        public static int GetUniformLocation(this IGlContext context, int program, string name)
        {
            Load(context);
            IntPtr intPtr = MarshalStringToPtr(name);
            var result = getUniformLocation(program, intPtr);
            FreeStringPtr(intPtr);
            return result;
        }
        static void FreeStringPtr(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        public delegate void GlCompileShader(int shader);
        [GlImport("glCompileShader")]
        static GlCompileShader compileShader;
        public static void CompileShader(this IGlContext context, int shader)
        {
            Load(context);
            compileShader(shader);
        }

        public delegate void GlGetShaderiv(int shader, int pname, int* @params);
        [GlImport("glGetShaderiv")]
        static GlGetShaderiv getShaderiv;
        public static void GetShader(this IGlContext context, int shader, int pname, out int @params)
        {
            Load(context);
            fixed (int* ptr = &@params)
            {
                getShaderiv(shader, pname, ptr);
            }
        }

        public static string GetShaderInfoLog(this IGlContext context, int shader)
        {
            Load(context);
            GetShaderInfoLog(context, shader, out var info);
            return info;
        }

        unsafe static void GetShaderInfoLog(IGlContext context, int shader, out string info)
        {
            GetShader(context, shader, GlConsts.GL_INFO_LOG_LENGTH, out var @params);
            if (@params == 0)
            {
                info = string.Empty;
            }
            else
            {
                GetShaderInfoLog((uint)shader, @params * 2, &@params, out info);
            }
        }
        public delegate void GlGetShaderInfoLog(uint shader, int bufSize, int* length, IntPtr infoLog);
        [GlImport("glGetShaderInfoLog")]
        static GlGetShaderInfoLog getShaderInfoLog;
        static void GetShaderInfoLog(uint shader, int bufSize, [Out] int* length, out string infoLog)
        {
            IntPtr intPtr = Marshal.AllocHGlobal((IntPtr)(bufSize + 1));
            getShaderInfoLog(shader, bufSize, length, intPtr);
            infoLog = MarshalPtrToString(intPtr);
            Marshal.FreeHGlobal(intPtr);
        }

        public delegate void GlLinkProgram(int program);
        [GlImport("glLinkProgram")]
        static GlLinkProgram linkProgram;
        public static void LinkProgram(this IGlContext context, int program)
        {
            Load(context);
            linkProgram(program);
        }

        public delegate int GlGetAttribLocation(int program, IntPtr name);
        [GlImport("glGetAttribLocation")]
        static GlGetAttribLocation getAttribLocation;
        public static int GetAttribLocation(this IGlContext context, int program, string name)
        {
            Load(context);
            IntPtr intPtr = MarshalStringToPtr(name);
            int result = getAttribLocation(program, intPtr);
            FreeStringPtr(intPtr);
            return result;
        }

        public delegate void GlUseProgram(int program);
        [GlImport("glUseProgram")]
        static GlUseProgram useProgram;
        public static void UseProgram(this IGlContext context, int program)
        {
            Load(context);
            useProgram(program);
        }

        public delegate void GlGenVertexArrays(int num, uint* arrays);
        [GlImport("glGenVertexArrays")]
        static GlGenVertexArrays genVertexArrays;
        public static int GenVertexArray(this IGlContext context)
        {
            Load(context);
            int result = default(int);
            genVertexArrays(1, (uint*)(&result));
            return result;
        }

        public delegate void GlBindVertexArray(int array);
        [GlImport("glBindVertexArray")]
        static GlBindVertexArray bindVertexArray;
        public static void BindVertexArray(this IGlContext context, int array)
        {
            Load(context);
            bindVertexArray(array);
        }

        public delegate void GlGenBuffers(int num, uint* buffer);
        [GlImport("glGenBuffers")]
        static GlGenBuffers genBuffers;
        public static int GenBuffer(this IGlContext context)
        {
            Load(context);
            int result = default(int);
            genBuffers(1, (uint*)(&result));
            return result;
        }

        public delegate void GlEnableVertexAttribArray(int index);
        [GlImport("glEnableVertexAttribArray")]
        static GlEnableVertexAttribArray enableVertexAttribArray;
        public static void EnableVertexAttribArray(this IGlContext context, int index)
        {
            Load(context);
            enableVertexAttribArray(index);
        }

        public delegate void GlVertexAttribPointer(uint index, int size, int type, bool normalized, int stride, IntPtr pointer);
        [GlImport("glVertexAttribPointer")]
        static GlVertexAttribPointer vertexAttribPointer;
        public static void VertexAttribPointer(this IGlContext context, uint index, int size, int type, bool normalized, int stride, int pointer)
        {
            Load(context);
            vertexAttribPointer(index, size, type, normalized, stride, (IntPtr)pointer);
        }

        public delegate void GlDrawArrays(int mode, int first, int count);
        [GlImport("glDrawArrays")]
        static GlDrawArrays drawArrays;
        public static void DrawArrays(this IGlContext context, int mode, int first, int count)
        {
            Load(context);
            drawArrays(mode, first, count);
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
