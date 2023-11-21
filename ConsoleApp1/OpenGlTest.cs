using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CPF.Windows;
using CPF.Windows.OpenGL;
using static CPF.Windows.UnmanagedMethods;
#if !Net4
using SkiaSharp;

namespace ConsoleApp1
{
    class OpenGlTest
    {

        WNDCLASSEX wc = new WNDCLASSEX();
        private string _className;
        IntPtr handle;
        WglContext wglContext;
        int w = 500;
        int h = 500;
        public OpenGlTest()
        {
            _className = "CPFWindow-" + Guid.NewGuid();
            // 初始化窗口类结构  
            wc.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            //wc.style = (int)ClassStyles.CS_DBLCLKS;
            wc.lpfnWndProc = WndProc;
            wc.hInstance = WindowImpl.ModuleHandle;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszClassName = _className;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = WindowImpl.DefaultCursor;
            wc.lpszMenuName = "";
            // 注册窗口类  
            RegisterClassEx(ref wc);
            // 创建并显示窗口  
            handle = CreateWindowEx((int)ExStyle, _className, "窗体标题", (uint)Style,
              CW_USEDEFAULT, CW_USEDEFAULT, w, h, IntPtr.Zero, IntPtr.Zero, WindowImpl.ModuleHandle, null);


            if (ShCoreAvailable)
            {
                uint dpix, dpiy;

                var monitor = MonitorFromWindow(
                    handle,
                    MONITOR.MONITOR_DEFAULTTONEAREST);

                if (GetDpiForMonitor(
                        monitor,
                        MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI,
                        out dpix,
                        out dpiy) == 0)
                {

                }
            }

            var useGPU = true;
            if (useGPU)
            {
                wglContext = new CPF.Windows.OpenGL.WglContext(handle);
            }

            {
                DWM_BLURBEHIND dwm = new DWM_BLURBEHIND();
                dwm.dwFlags = DWM_BB_ENABLE | DWM_BB_BLURREGION;
                dwm.fEnable = true;
                dwm.hRegionBlur = CreateRectRgn(0, 0, -1, -1);
                DwmEnableBlurBehindWindow(handle, ref dwm);
                DeleteObject(dwm.hRegionBlur);
                //MARGINS sRT = new MARGINS();
                //sRT.Right = sRT.Left = sRT.Top = sRT.Bottom = -1;
                //DwmExtendFrameIntoClientArea(handle, ref sRT);
            }
        }

        public void Run()
        {
            ShowWindow(handle, ShowWindowCommand.Normal);
            MSG msg = new MSG();
            while (GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }
        GRContext grContext;
        protected unsafe virtual IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch ((WindowsMessage)msg)
            {
                case WindowsMessage.WM_PAINT:
                    wglContext.MakeCurrent();
                    if (grContext == null)
                    {
                        grContext = GRContext.CreateGl();
                    }
                    var maxSamples = grContext.GetMaxSurfaceSampleCount(SKColorType.Rgba8888);
                    wglContext.GetFramebufferInfo(out var Framebuffer, out var samples,out var Stencil);
                    if (samples > maxSamples)
                        samples = maxSamples;
                    var framebufferInfo = new GRGlFramebufferInfo((uint)Framebuffer, SKColorType.Rgba8888.ToGlSizedFormat());
                    var backendRenderTarget = new GRBackendRenderTarget(w, h, samples, Stencil, framebufferInfo);
                    var surface = SKSurface.Create(grContext, backendRenderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
                    var canvas = surface.Canvas;

                    canvas.Clear(new SKColor(255, 0, 0, 100));

                    canvas.Flush();
                    canvas.Dispose();
                    surface.Dispose();
                    backendRenderTarget.Dispose();
                    //testPaint();
                    wglContext.SwapBuffers();
                    break;
                case WindowsMessage.WM_DESTROY:

                    break;
            }
            return DefWindowProc(hwnd, msg, wParam, lParam);
        }

        protected virtual WindowStyles ExStyle
        {
            get
            {
                var style = WindowStyles.WS_EX_LEFT;
                return style;
            }
        }

        protected virtual WindowStyles Style
        {
            get
            {
                return WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_POPUP |
                  //WindowStyles.WS_BORDER |
                  //WindowStyles.WS_THICKFRAME |
                  WindowStyles.WS_MINIMIZEBOX
                  ;
            }
        }


        void testPaint()
        {
            OpenGl gl = new OpenGl();
            gl.Enable(OpenGl.GL_ALPHA_TEST);
            gl.Enable(OpenGl.GL_DEPTH_TEST);
            gl.Enable(OpenGl.GL_COLOR_MATERIAL);

            gl.Enable(OpenGl.GL_LIGHTING);
            gl.Enable(OpenGl.GL_LIGHT0);

            gl.Enable(OpenGl.GL_BLEND);
            gl.BlendFunc(OpenGl.GL_SRC_ALPHA, OpenGl.GL_ONE_MINUS_SRC_ALPHA);
            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(OpenGl.GL_COLOR_BUFFER_BIT | OpenGl.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();

            gl.Color(0, 1, 1);
            gl.Begin(OpenGl.GL_TRIANGLES);                              // Drawing Using Triangles
            gl.Color(1.0f, 0.0f, 0.0f);                      // Set The Color To Red
            gl.Vertex(0.0f, 1.0f, 0.0f);                  // Top
            gl.Color(0.0f, 1.0f, 0.0f);                      // Set The Color To Green
            gl.Vertex(-1.0f, -1.0f, 0.0f);                  // Bottom Left
            gl.Color(0.0f, 0.0f, 1.0f);                      // Set The Color To Blue
            gl.Vertex(1.0f, -1.0f, 0.0f);                  // Bottom Right
            gl.End();

            gl.PopMatrix();
            gl.Flush();
        }
    }
}
#endif