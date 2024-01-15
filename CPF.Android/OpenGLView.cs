using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CPF.Drawing;
using CPF.OpenGL;
using CPF.Platform;
using Javax.Microedition.Khronos.Opengles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CPF.Android
{
    class OpenGLView : GLSurfaceView, ISurfaceView, GLSurfaceView.IRenderer, IGlContext
    {
        public OpenGLView(Context context, UIElement content, CpfView cpfView) : base(context)
        {
            SetEGLContextClientVersion(2);
            SetEGLConfigChooser(8, 8, 8, 8, 0, 8);
            CpfView = cpfView;
            generalView = new GeneralView(this, content);
            cpfView.AddView(this);
            generalView.Create();
            SetRenderer(this);
        }

        IGlContext glContext;
        GeneralView generalView;
        public CpfView CpfView { get; }

        public GeneralView GeneralView { get { return generalView; } }

        public CPF.Controls.View Root
        {
            get { return generalView.Root; }
        }

        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            generalView.OnCreateDrawableState(extraSpace);
            return base.OnCreateDrawableState(extraSpace);
        }


        public override void Invalidate()
        {
            generalView.Invalidate();
            base.Invalidate();
        }

        public override void Invalidate(global::Android.Graphics.Rect dirty)
        {
            Invalidate();
        }

        public override void Invalidate(int l, int t, int r, int b)
        {
            Invalidate();
        }

        //float scale;
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            generalView.OnLayout(changed, left, top, right, bottom);
        }

        public void OnPaint()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var root = generalView.Root;
            GetFramebufferInfo(out var fb, out var sam, out var sten);

            using (DrawingContext dc = DrawingContext.FromRenderTarget(new OpenGlRenderTarget<object>(null, this, newSize.Width, newSize.Height, fb, sam, sten)))
            {
                root.Invoke(() =>
                {
                    root.LayoutManager.ExecuteLayoutPass();
                    if (root.LayoutManager.VisibleUIElements != null)
                    {
                        root.RenderView(dc, new CPF.Drawing.Rect(0, 0, newSize.Width, newSize.Height));
                    }
                });

            }
            System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        public override IInputConnection OnCreateInputConnection(EditorInfo outAttrs)
        {
            return generalView.OnCreateInputConnection(outAttrs);
        }

        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            generalView.OnKey(v, keyCode, e);
            return false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {

            return generalView.OnTouch(v, e);
        }

        protected override void DispatchSetActivated(bool activated)
        {
            base.DispatchSetActivated(activated);
            generalView.DispatchSetActivated(activated);
        }


        public Screen Screen
        {
            get
            {
                return generalView.Screen;
            }
        }

        public float RenderScaling
        {
            get
            {
                return generalView.RenderScaling;
            }
        }

        public float LayoutScaling => RenderScaling;

        public Action ScalingChanged { get => generalView.ScalingChanged; set => generalView.ScalingChanged = value; }
        public Action<Size> Resized { get => generalView.Resized; set => generalView.Resized = value; }
        Action<PixelPoint> IViewImpl.PositionChanged { get => generalView.PositionChanged; set => generalView.PositionChanged = value; }
        Action IViewImpl.Activated { get => generalView.Activated; set => generalView.Activated = value; }
        public Action Deactivated { get => generalView.Deactivated; set => generalView.Deactivated = value; }
        bool IViewImpl.CanActivate { get => generalView.CanActivate; set => generalView.CanActivate = value; }
        PixelPoint IViewImpl.Position
        {
            get
            {
                return generalView.Position;
            }
            set
            {
                generalView.Position = value;
            }
        }

        public IDisposable GRContext { get; set; }

        void IViewImpl.Activate()
        {
            generalView.Activate();
        }

        void IViewImpl.Capture()
        {
            generalView.Capture();
        }

        void IViewImpl.Invalidate(in Drawing.Rect rect)
        {
            Invalidate();
        }

        Drawing.Point IViewImpl.PointToClient(Drawing.Point point)
        {
            return generalView.PointToClient(point);
        }

        Drawing.Point IViewImpl.PointToScreen(Drawing.Point point)
        {
            return generalView.PointToScreen(point);
        }

        void IViewImpl.ReleaseCapture()
        {
            generalView.ReleaseCapture();
        }

        void IViewImpl.SetCursor(Cursor cursor)
        {
            generalView.SetCursor(cursor);
        }

        void IViewImpl.SetIMEEnable(bool enable)
        {
            generalView.SetIMEEnable(enable);
        }

        public void ShowKeyboard(bool show, IEditor editor)
        {
            generalView.ShowKeyboard(show, editor);
        }

        void IViewImpl.SetIMEPosition(Drawing.Point point)
        {
            //throw new NotImplementedException();
        }

        void IViewImpl.SetRoot(Controls.View view)
        {
            generalView.SetRoot(view);
        }

        void IViewImpl.SetVisible(bool visible)
        {
            generalView.SetVisible(visible);
        }

        protected override void Dispose(bool disposing)
        {
            glContext?.Dispose();
            glContext = null;
            base.Dispose(disposing);
        }

        public void OnDrawFrame(IGL10 gl)
        {
            OnPaint();
        }

        //private Size lastSize;
        private PixelSize newSize;
        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            newSize = new PixelSize(width, height);
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {

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
            return eglGetProcAddress(name);
        }
        const string lib = "/system/lib/egl/libEGL_mali.so";
        [DllImport(lib)]
        public extern static IntPtr eglGetProcAddress(string procname);

        public void MakeCurrent()
        {
            
        }
    }
}