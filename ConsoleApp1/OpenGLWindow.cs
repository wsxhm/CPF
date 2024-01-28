#if !NET40
using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.OpenGL;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using LibMpv.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class OpenGLWindow : Window
    {
        protected override void InitializeComponent()
        {
            LoadStyleFile("res://ConsoleApp1/Stylesheet1.css");
            //加载样式文件，文件需要设置为内嵌资源

            Title = "标题";
            Width = 500;
            Height = 400;
            Background = null;
            CanResize = true;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children = //内容元素放这里
                {
                    new ScrollBar
                    {
                        MarginLeft = 387,
                        MarginTop = 14,
                        Width = 35,
                        Height = 236,
                    },
                    new VideoView
                    {
                        Height = "100%",
                        Width = "100%",
                        PresenterFor=this,
                        Name="view"
                    },
                    new TextBox
                    {
                        MarginLeft = 14,
                        MarginTop = 23,
                        Height = 94,
                        Width = 153,
                    },
                }
            })
            { MaximizeBox = true });
            if (!DesignMode)//设计模式下不执行，也可以用#if !DesignMode
            {

            }
        }
        VideoView video;
#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();
            video = FindPresenterByName<VideoView>("view");
            VideoView.InitMpv();
            DynamicallyLoadedBindings.ThrowErrorIfFunctionNotFound = true;
            video.MpvContext = new MpvContext() { };
            //video.LoadFile(@"D:\xhm\Videos\202204201620.mp4");
            //video.Play();
            WindowState = WindowState.Maximized;
        }
        //用户代码

#endif
    }

    public class VideoView : CPF.Skia.GLView, IMpvVideoView
    {
        public static void InitMpv()
        {
            var platform = IntPtr.Size == 8 ? "" : "x86";
            var platformId = FunctionResolverFactory.GetPlatformId();
            if (platformId == LibMpvPlatformID.Win32NT)
            {
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, platform);
                LibMpv.Client.LibMpv.UseLibMpv(2).UseLibraryPath(path);
            }
            else if (platformId == LibMpvPlatformID.Unix)
            {
                var path = $"/usr/lib/{platform}-linux-gnu";
                LibMpv.Client.LibMpv.UseLibMpv(0).UseLibraryPath(path);
            }
        }

        public void LoadFile(string fileName, string mode = "replace")
        {
            //mpvContext.SetOptionString("opengl-glfinish", "yes");
            //mpvContext.SetLogLevel(MpvLogLevel.MpvLogLevelDebug);
            mpvContext.Command("loadfile", fileName, mode);
        }
        public void Play()
        {
            mpvContext.SetPropertyFlag("pause", false);
        }
        public MpvContext? MpvContext
        {
            get { return mpvContext; }
            set
            {
                if (ReferenceEquals(value, mpvContext))
                    return;
                if (mpvContext != null)
                    DetachMpvContext(mpvContext);
                mpvContext = value;
                if (mpvContext != null)
                    AttachMpvContext(mpvContext);
            }
        }

        protected override void OnGLRender(IGlContext gl)
        {
            base.OnGLRender(gl);
            if (mpvContext != null && getProcAddres != null)
            {
                var scaling = Root.RenderScaling;
                var Bounds = ActualSize;
                var width = Math.Max(1, (int)(Bounds.Width * scaling));
                var height = Math.Max(1, (int)(Bounds.Height * scaling));
            var stop = new System.Diagnostics.Stopwatch();
            stop.Start();
                mpvContext.RenderOpenGl(width, height, FramebufferId, 1);
            stop.Stop();
            System.Diagnostics.Debug.WriteLine(stop.ElapsedMilliseconds);
            }
            update = false;
        }

        private void DetachMpvContext(MpvContext context)
        {
            context.StopRendering();
        }

        private void AttachMpvContext(MpvContext context)
        {
            if (getProcAddres != null)
            {
                mpvContext?.ConfigureRenderer(new OpenGlRendererConfiguration() { OpnGlGetProcAddress = getProcAddres, UpdateCallback = this.UpdateVideoView });
            }
        }

        protected override void OnGLLoaded(IGlContext gl)
        {
            base.OnGLLoaded(gl);
            if (getProcAddres != null) return;

            getProcAddres = gl.GetProcAddress;
            if (mpvContext != null)
            {
                AttachMpvContext(mpvContext);//需要绑定之后才能加载视频
                //LoadFile(@"F:\新建文件夹\....mp4");
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (mpvContext != null)
                DetachMpvContext(mpvContext);
        }
        bool update;
        private void UpdateVideoView()
        {
            if (!update)
            {
                update = true;
                CPF.Threading.Dispatcher.MainThread.BeginInvoke(() =>
                {
                    Invalidate();
                });
            }

        }

        private MpvContext? mpvContext = null;
        private OpenGlGetProcAddressDelegate getProcAddres;
    }
}
#endif