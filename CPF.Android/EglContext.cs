using Android.App;
using Android.Content;
using Android.Graphics;
//using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CPF.Drawing;
using CPF.Input;
using CPF.OpenGL;
using CPF.Platform;
using Javax.Microedition.Khronos.Egl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLES20 = global::Android.Opengl.GLES20;

namespace CPF.Android
{
    class EglContext : IGlContext
    {
        EGLDisplay display;
        IEGL10 egl;
        EGLSurface eglSurface;
        EGLContext context;
        EGLConfig config;
        AndroidView androidView;
        public EglContext(AndroidView androidView)
        {
            this.androidView = androidView;
            //1. 取得EGL实例
            egl = EGLContext.EGL.JavaCast<IEGL10>();
            //2. 选择Display
            display = egl.EglGetDisplay(EGL10.EglDefaultDisplay);
            egl.EglInitialize(display, null);

            int[] attribList = {
                EGL10.EglRedSize, 8,
                EGL10.EglGreenSize, 8,
                EGL10.EglBlueSize, 8,
                EGL10.EglAlphaSize, 8,
                EGL10.EglRenderableType,global:: Android.Opengl.EGL14.EglOpenglEs2Bit,
                EGL10.EglStencilSize, 8,      // placeholder for recordable [@-3]
                EGL10.EglNone
            };
            //3. 选择Config
            EGLConfig[] configs = new EGLConfig[1];
            int[] numConfigs = new int[1];
            egl.EglChooseConfig(display, attribList, configs, configs.Length, numConfigs);
            config = configs[0];

            // 创建Context
            context = egl.EglCreateContext(display, config, EGL10.EglNoContext, new int[]{
                global:: Android.Opengl.EGL14.EglContextClientVersion, 2,
                EGL10.EglNone
            });
        }

        public EGLSurface EglSurface
        {
            get { return eglSurface; }
        }

        //IntPtr window;
        public void OnCreateSurface()
        {
            //window = AndroidView.ANativeWindow_fromSurface(JNIEnv.Handle, androidView.Holder.Surface.Handle);
            // 创建Surface
            eglSurface = egl.EglCreateWindowSurface(display, config, androidView, null);
        }

        public void OnDestroySurface()
        {
            //AndroidView.ANativeWindow_release(window);
            if (eglSurface != null)
            {
                egl.EglDestroySurface(display, eglSurface);
            }
            eglSurface = null;
        }
        EGLContext oldContext;
        EGLDisplay oldDisplay;
        EGLSurface oldReadSurface;
        EGLSurface oldDrawSurface;
        public void MakeCurrent()
        {
            if (eglSurface != null)
            {
                oldContext = egl.EglGetCurrentContext();
                oldDisplay = egl.EglGetCurrentDisplay();
                oldReadSurface = egl.EglGetCurrentSurface(global::Android.Opengl.EGL14.EglRead);
                oldDrawSurface = egl.EglGetCurrentSurface(global::Android.Opengl.EGL14.EglDraw);
                egl.EglMakeCurrent(display, eglSurface, eglSurface, context);
            }
        }

        public void SwapBuffers()
        {
            if (eglSurface != null)
            {
                egl.EglSwapBuffers(display, eglSurface);
                //egl.EglMakeCurrent(display, EGL10.EglNoSurface, EGL10.EglNoSurface, EGL10.EglNoContext);
                egl.EglMakeCurrent(oldDisplay, oldDrawSurface, oldReadSurface, oldContext);
            }
        }

        public IDisposable GRContext { get; set; }

        public void Dispose()
        {
            OnDestroySurface();
            if (display != null && context != null)
            {
                if (display.Handle != IntPtr.Zero && context.Handle != IntPtr.Zero)
                {
                    egl.EglDestroyContext(display, context);
                }
                egl.EglTerminate(display);
                egl.Dispose();
                context = null;
                display = null;
            }
            GRContext?.Dispose();
            GRContext = null;
        }

        public void GetFramebufferInfo(out int framebuffer, out int samples, out int stencil)
        {
            var buffer = new int[3];
            GLES20.GlGetIntegerv(GLES20.GlFramebufferBinding, buffer, 0);
            GLES20.GlGetIntegerv(GLES20.GlStencilBits, buffer, 1);
            GLES20.GlGetIntegerv(GLES20.GlSamples, buffer, 2);
            samples = buffer[2];
            stencil = buffer[1];
            framebuffer = buffer[0];
        }

        public IntPtr GetProcAddress(string name)
        {
            return OpenGLView.eglGetProcAddress(name);
        }
    }
}