using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CPF.Android
{
    /// <summary>
    /// 用于承载CPF内容的容器
    /// </summary>
    public class CpfView : AbsoluteLayout, View.IOnLayoutChangeListener
    {
        internal ISurfaceView androidView;
        /// <summary>
        /// 用于承载CPF内容的容器
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="content"></param>
        public CpfView(Context activity, UIElement content) : base(activity)
        {
            AddOnLayoutChangeListener(this);
            //if (CPF.Platform.Application.GetDrawingFactory().UseGPU)
            //{
            //    androidView = new OpenGLView(activity, content, this);
            //}
            //else
            //{
            androidView = new AndroidView(activity, content, this);
            //}
            ////AddView(androidView);
            if (activity is CpfActivity cpf && cpf.Main == null)
            {
                cpf.Main = this;
            }
        }
        /// <summary>
        /// 用于承载CPF内容的容器
        /// </summary>
        /// <param name="content"></param>
        public CpfView(UIElement content) : base(CpfActivity.CurrentActivity)
        {
            //if (CPF.Platform.Application.GetDrawingFactory().UseGPU)
            //{
            //    androidView = new OpenGLView(CpfActivity.CurrentActivity, content, this);
            //}
            //else
            //{
            androidView = new AndroidView(CpfActivity.CurrentActivity, content, this);
            //}
            ////AddView(androidView);
            if (CpfActivity.CurrentActivity is CpfActivity cpf && cpf.Main == null)
            {
                cpf.Main = this;
            }
        }

        public CPF.Controls.View Root
        {
            get { return androidView.Root; }
        }

        public WindowManagerTypes WindowType { get; set; } = WindowManagerTypes.DrawnApplication;

        public WindowManagerFlags WindowFlags { get; set; } = WindowManagerFlags.LayoutNoLimits;

        PixelPoint position;
        public PixelPoint Location
        {
            get { return position; }
            protected set
            {
                position = value;
                (androidView as CPF.Platform.IViewImpl).PositionChanged(position);
                OnMove();
            }
        }

        protected virtual void OnMove()
        {

        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {
            if (!(this is WindowImpl) && !(this is PopupImpl))
            {
                Location = new PixelPoint(left, top);
            }
        }

        public void UpdateLayout(int x, int y, int w, int h)
        {
            var margin = new WindowManagerLayoutParams(WindowType, WindowFlags, global::Android.Graphics.Format.Rgbx8888);
            margin.Gravity = GravityFlags.Left | GravityFlags.Top;
            margin.X = x;
            margin.Y = y;
            margin.Width = w;
            margin.Height = h;
            margin.WindowAnimations = -1;
            //var layoutParamsClass = Java.Lang.Class.ForName("android.view.WindowManager$LayoutParams");
            var layoutParamsClass = margin.Class;
            var privateFlags = layoutParamsClass.GetField("privateFlags");
            var noAnim = layoutParamsClass.GetField("PRIVATE_FLAG_NO_MOVE_ANIMATION");

            int privateFlagsValue = privateFlags.GetInt(margin);
            int noAnimFlag = noAnim.GetInt(margin);
            privateFlagsValue |= noAnimFlag;

            privateFlags.SetInt(margin, privateFlagsValue);
            (Context as Activity).WindowManager.UpdateViewLayout(this, margin);
            //LayoutParameters = margin;

            Location = new PixelPoint(x, y);
            //System.Diagnostics.Debug.WriteLine($"{x},{y}");
        }
    }
}