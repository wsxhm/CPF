using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
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
using GLES20 = global::Android.Opengl.GLES20;

namespace CPF.Android
{
    internal class AndroidView : SurfaceView, ISurfaceView, ISurfaceHolderCallback
    {
        public AndroidView(Context activity, UIElement content, CpfView cpfView) : base(activity)
        {
            if (CPF.Platform.Application.GetDrawingFactory().UseGPU)
            {
                eglContext = new EglContext(this);
                Holder.AddCallback(this);
                //SetLayerType(LayerType.Hardware, null);
            }
            CpfView = cpfView;
            this.Holder.SetFormat(Format.Rgba8888);
            //SetOnTouchListener(this);
            //SetOnKeyListener(this);
            //this.activity = activity as Activity;
            //if (content != null)
            //{
            //    root = new InnerView(this);
            //    root.Background = CPF.Drawing.Color.White;
            //    root.Children.Add(content);
            //    root.Visibility = CPF.Visibility.Visible;
            //    root.CanActivate = true;
            //    root.LayoutUpdated += Root_LayoutUpdated;
            //    scale = root.LayoutScaling;
            //}
            //this.FocusableInTouchMode = true;
            //softKeyboardListner = new SoftKeyboardListner(this);
            //ViewTreeObserver.AddOnGlobalLayoutListener(softKeyboardListner);
            generalView = new GeneralView(this, content);
            cpfView.AddView(this);
            generalView.Create();
        }
        EglContext eglContext;
        GeneralView generalView;
        public CpfView CpfView { get; }

        public GeneralView GeneralView { get { return generalView; } }


        //internal Activity activity;
        //CPF.Controls.View root;
        //PixelSize oldSize;
        public CPF.Controls.View Root
        {
            get { return generalView.Root; }
        }

        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            //Invalidate();
            generalView.OnCreateDrawableState(extraSpace);
            return base.OnCreateDrawableState(extraSpace);
        }


        //bool _invalidateQueued;
        public override void Invalidate()
        {
            //if (_invalidateQueued)
            //{
            //    return;
            //}
            //_invalidateQueued = true;
            //Threading.Dispatcher.MainThread.BeginInvoke(() =>
            //{
            //    _invalidateQueued = false;
            //    if (CpfView.Handle != default)
            //    {
            //        OnPaint();
            //    }
            //});
            generalView.Invalidate();
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
            //base.OnLayout(changed, left, top, right, bottom);
            //var size = new PixelSize(right, bottom);
            //var sc = LayoutScaling;
            //if (LayoutScaling != scale)
            //{
            //    scale = sc;
            //    ScalingChanged();
            //}
            //if (size != oldSize)
            //{
            //    oldSize = size;
            //    Resized(new Size(size.Width / LayoutScaling, size.Height / LayoutScaling));
            //}
            generalView.OnLayout(changed, left, top, right, bottom);
        }

        PixelSize pixelSize;
        public virtual void OnPaint()
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            var root = generalView.Root;
            if (eglContext != null)
            {
                var _window = ANativeWindow_fromSurface(JNIEnv.Handle, Holder.Surface.Handle);
                if (_window == IntPtr.Zero)
                    return;//获取window操作可以解决下拉框图像错位问题
                ANativeWindow_release(_window);

                root.LayoutManager.ExecuteLayoutPass();
                if (eglContext.EglSurface != null)
                {
                    eglContext.MakeCurrent();
                    //var ir = generalView.invalidateRect * RenderScaling;
                    //GLES20.GlViewport(ir.X,ir.Y,ir.Width,ir.Height);
                    eglContext.GetFramebufferInfo(out var fb, out var sam, out var ste);
                    using (DrawingContext dc = DrawingContext.FromRenderTarget(new OpenGlRenderTarget(eglContext, pixelSize.Width, pixelSize.Height, fb, sam, ste)))
                    {
                        if (root.LayoutManager.VisibleUIElements != null)
                        {
                            root.RenderView(dc, new Drawing.Rect(0, 0, pixelSize.Width, pixelSize.Height));
                        }
                    }
                    eglContext.SwapBuffers();
                }
            }
            else
            {
                var surface = this.Holder.Surface;
                var _window = ANativeWindow_fromSurface(JNIEnv.Handle, surface.Handle);
                if (_window == IntPtr.Zero)
                    return;
                //throw new Exception("Unable to obtain ANativeWindow");
                ANativeWindow_Buffer buffer;
                var rc = new ARect()
                {
                    right = ANativeWindow_getWidth(_window),
                    bottom = ANativeWindow_getHeight(_window)
                };
                var size = new PixelSize(rc.right, rc.bottom);
                if (size.Width != Width || size.Height != Height)
                {//有时候渲染尺寸和控件尺寸不一样
                 //surface.SetSize(size.Width, size.Height);
                 //Holder.SetSizeFromLayout();
                    Holder.SetFixedSize(size.Width, size.Height);
                }
                rc = new ARect()
                {
                    right = ANativeWindow_getWidth(_window),
                    bottom = ANativeWindow_getHeight(_window)
                };
                size = new PixelSize(rc.right, rc.bottom);
                ANativeWindow_lock(_window, out buffer, ref rc);

                root.LayoutManager.ExecuteLayoutPass();

                var Format = buffer.format == AndroidPixelFormat.WINDOW_FORMAT_RGB_565
                    ? CPF.Drawing.PixelFormat.Rgb565 : CPF.Drawing.PixelFormat.Rgba;

                var RowBytes = buffer.stride * (Format == CPF.Drawing.PixelFormat.Rgb565 ? 2 : 4);
                var Address = buffer.bits;

                using (Drawing.Bitmap bmp = new Drawing.Bitmap(size.Width, size.Height, RowBytes, Format, Address))
                {
                    using (DrawingContext dc = DrawingContext.FromBitmap(bmp))
                    {
                        if (root.LayoutManager.VisibleUIElements != null)
                        {
                            //generalView.invalidateRect * RenderScaling
                            root.RenderView(dc, new Drawing.Rect(0,0,size.Width,size.Height));
                        }
                    }
                }
                ANativeWindow_unlockAndPost(_window);
                ANativeWindow_release(_window);
            }
            //System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
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

        //public override bool DispatchKeyEvent(KeyEvent e)
        //{
        //    //var m = new WindowManagerLayoutParams(100, 100, 100, 100, WindowManagerTypes.Phone | WindowManagerTypes.ApplicationOverlay, WindowManagerFlags.NotFocusable, global::Android.Graphics.Format.Rgbx8888);
        //    ////m.Gravity = GravityFlags.Left | GravityFlags.Top;
        //    //activity.WindowManager.AddView(new TestView(activity) { Background = new global::Android.Graphics.Drawables.ColorDrawable(global::Android.Graphics.Color.Argb(100, 255, 0, 0)) }, m);


        //}

        protected override void DispatchSetActivated(bool activated)
        {
            base.DispatchSetActivated(activated);
            //if (activated)
            //{
            //    (this as IViewImpl).Activated();
            //}
            //else
            //{
            //    Deactivated();
            //}
            generalView.DispatchSetActivated(activated);
        }

        [DllImport("android")]
        internal static extern IntPtr ANativeWindow_fromSurface(IntPtr jniEnv, IntPtr handle);
        [DllImport("android")]
        internal static extern int ANativeWindow_getWidth(IntPtr window);
        [DllImport("android")]
        internal static extern int ANativeWindow_getHeight(IntPtr window);
        [DllImport("android")]
        internal static extern void ANativeWindow_release(IntPtr window);
        [DllImport("android")]
        internal static extern void ANativeWindow_unlockAndPost(IntPtr window);

        [DllImport("android")]
        internal static extern int ANativeWindow_lock(IntPtr window, out ANativeWindow_Buffer outBuffer, ref ARect inOutDirtyBounds);
        public enum AndroidPixelFormat
        {
            WINDOW_FORMAT_RGBA_8888 = 1,
            WINDOW_FORMAT_RGBX_8888 = 2,
            WINDOW_FORMAT_RGB_565 = 4,
        }

        internal struct ARect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        internal struct ANativeWindow_Buffer
        {
            // The number of pixels that are show horizontally.
            public int width;

            // The number of pixels that are shown vertically.
            public int height;

            // The number of *pixels* that a line in the buffer takes in
            // memory.  This may be >= width.
            public int stride;

            // The format of the buffer.  One of WINDOW_FORMAT_*
            public AndroidPixelFormat format;

            // The actual bits.
            public IntPtr bits;

            // Do not touch.
            uint reserved1;
            uint reserved2;
            uint reserved3;
            uint reserved4;
            uint reserved5;
            uint reserved6;
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
                //return global::Android.App.Application.Context.Resources.DisplayMetrics.ScaledDensity;
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

        void IViewImpl.Activate()
        {
            //throw new NotImplementedException();
            //RequestFocus();
            generalView.Activate();
        }

        void IViewImpl.Capture()
        {
            //RequestPointerCapture();
            //throw new NotImplementedException();
            generalView.Capture();
        }

        void IViewImpl.Invalidate(in Drawing.Rect rect)
        {
            generalView.Invalidate(in rect);
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
            //throw new NotImplementedException();
            generalView.SetCursor(cursor);
        }

        //internal bool IMEEnable = true;
        void IViewImpl.SetIMEEnable(bool enable)
        {
            //IMEEnable = enable;
            generalView.SetIMEEnable(enable);
        }

        //bool showInput;
        //internal static IEditor editor;
        public void ShowKeyboard(bool show, IEditor editor)
        {
            generalView.ShowKeyboard(show, editor);
        }

        void IViewImpl.SetIMEPosition(Drawing.Point point)
        {
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
            eglContext?.Dispose();
            eglContext = null;
            base.Dispose(disposing);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            pixelSize = new PixelSize(width, height);
            eglContext.OnDestroySurface();
            eglContext.OnCreateSurface();
            //GLES20.GlViewport(0, 0, width, height);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            eglContext?.OnCreateSurface();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            eglContext?.OnDestroySurface();
        }
        //public bool OnCapturedPointer(View view, MotionEvent e)
        //{
        //    System.Diagnostics.Debug.WriteLine(view + "--" + e.Action);
        //    return false;
        //}
    }

    class InnerView : CPF.Controls.View
    {
        ISurfaceView cpfView;
        public InnerView(ISurfaceView cpfView) : base(cpfView)
        {
            this.cpfView = cpfView;
        }

        protected override IViewImpl CreateView()
        {
            return cpfView;
        }
    }

    //class TestView : View
    //{
    //    public TestView(global::Android.Content.Context context) : base(context)
    //    {
    //        Focusable = true;
    //        FocusableInTouchMode = true;
    //    }

    //}
}