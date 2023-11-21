using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Widget;
using CPF.Drawing;
using CPF.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Controls;

namespace CPF.Android
{
    class NativeImpl : AbsoluteLayout, INativeImpl
    {
        public NativeImpl() : base(CpfActivity.CurrentActivity)
        {
            //AddView(new global::Android.Widget.Button(CpfActivity.CurrentActivity) { Text= "原生控件" });
        }

        public void SetBackColor(Color color)
        {
            base.Background = new ColorDrawable(global::Android.Graphics.Color.Argb(color.A, color.R, color.G, color.B));
        }

        Rect clipRect;
        bool isVisible;
        int width;
        int height;
        int left;
        int top;
        public void SetBounds(Rect boundsRect, Rect clip, bool visible)
        {
            if (visible)
            {
                var l = (int)(boundsRect.Left * parent.RenderScaling + (parent as IViewImpl).Position.X);
                var t = (int)(boundsRect.Top * parent.RenderScaling + (parent as IViewImpl).Position.Y);
                var w = (int)(boundsRect.Width * parent.RenderScaling);
                var h = (int)(boundsRect.Height * parent.RenderScaling);
                //Layout(l, t, l + w, t + h);
                if (isVisible != visible || clipRect != clip || top != t || l != left || height != h || width != w)
                {
                    top = t;
                    left = l;
                    height = h;
                    width = w;
                    if (!(parent.GeneralView.softKeyboardListner._wasKeyboard && parent.GeneralView.softKeyboardListner.viewImpl != null))
                    {
                        this.Visibility = ViewStates.Visible;
                        var margin = new LayoutParams(w, h, l, t);
                        LayoutParameters = margin;
                        //parent.CpfView.UpdateViewLayout(this, margin);

                        if (!float.IsInfinity(clip.Width) && !float.IsInfinity(clip.Height) && clipRect != clip)
                        {
                            clipRect = clip;
                            base.ClipBounds = new global::Android.Graphics.Rect((int)(clip.X * parent.RenderScaling), (int)(clip.Y * parent.RenderScaling), (int)(clip.Right * parent.RenderScaling), (int)(clip.Bottom * parent.RenderScaling));
                        }
                    }
                }
            }
            else
            {
                this.Visibility = ViewStates.Gone;
            }
            isVisible = visible;
        }

        object content;
        public void SetContent(object content)
        {
            if (content is global::Android.Views.View view)
            {
                this.AddView(view);
            }
            else
            {
                if (this.content is global::Android.Views.View old)
                {
                    this.RemoveView(old);
                }
            }
            this.content = content;
        }

        public void SetOwner(NativeElement owner)
        {

        }
        ISurfaceView parent;

        object INativeImpl.Handle => this;

        public void SetParent(IViewImpl parent)
        {
            if (parent != null)
            {
                if (parent is AndroidView popup)
                {
                    popup.CpfView.AddView(this);
                }
            }
            else
            {
                if (this.parent != null)
                {
                    this.parent.CpfView.RemoveView(this);
                }
            }
            this.parent = parent as ISurfaceView;
        }
    }
}