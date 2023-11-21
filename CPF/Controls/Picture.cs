using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
using CPF.Styling;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 显示图像，支持路径、Url、Image、Bitmap、Stream、byte[]、支持GIF播放
    /// </summary>
    [Description("显示图像，支持路径、Url、Image、Bitmap、Stream、byte[]、支持GIF播放")]
    [DefaultProperty(nameof(Source))]
    public class Picture : UIElement
    {
        /// <summary>
        /// 显示图像，支持路径、Url、Image、Bitmap、Stream、byte[]、支持GIF播放
        /// </summary>
        public Picture()
        {

        }

        /// <summary>
        /// 图片源，可以是路径、Url、Drawing.Image对象、Stream、byte[]
        /// </summary>
        [TypeConverter(typeof(StringConverter)), CPF.Design.FileBrowser(".png;.jpg;.bmp;.gif")]
        [PropertyMetadata(null)]
        [Description("图片源，可以是路径、Url、Drawing.Image对象、Stream、byte[]")]
        public object Source
        {
            get { return GetValue<object>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 图片缩放模式
        /// </summary>
        [UIPropertyMetadata(Stretch.None, UIPropertyOptions.AffectsMeasure), Description("图片缩放模式")]
        public Stretch Stretch
        {
            get { return GetValue<Stretch>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 描述如何对内容应用缩放，并限制对已命名像素类型的缩放。
        /// </summary>
        [Description("描述如何对内容应用缩放，并限制对已命名像素类型的缩放。")]
        [UIPropertyMetadata(StretchDirection.Both, UIPropertyOptions.AffectsMeasure)]
        public StretchDirection StretchDirection
        {
            get { return GetValue<StretchDirection>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取加载成功了的图片
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return img;
        }
        /// <summary>
        /// 播放GIF
        /// </summary>
        public void Play()
        {
            if (timer != null)
            {
                timer.Start();
            }
        }
        /// <summary>
        /// 停止播放gif
        /// </summary>
        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }


        Threading.DispatcherTimer timer;
        bool needDisposeImg = false;
        Image img;
        int index = 0;
        int count = 0;
        void SetImage(Image img)
        {
            index = 0;
            if (this.img != null && needDisposeImg)
            {
                this.img.Dispose();
            }
            this.img = img;
            InvalidateMeasure();
            Invalidate();
            if (img != null && img.FrameCount > 1)
            {
                count = (int)img.FrameCount;
                if (Root != null)
                {
                    if (timer == null)
                    {
                        timer = new Threading.DispatcherTimer();
                        timer.Tick += Timer_Tick;
                        timer.Interval = TimeSpan.FromMilliseconds(25);
                    }
                    timer.Start();
                }
            }
            else
            {
                count = 0;
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }
        }
        //int frameTimer = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Root != null && Root.LayoutManager.IsVisible(this) && !DesignMode)
            {
                if (img is Image image)
                {
                    //frameTimer += 50;
                    //if (frameTimer >= image.FrameDelay[index])
                    //{
                    //    index++;
                    //    if (index >= count)
                    //    {
                    //        index = 0;
                    //    }
                    //    frameTimer = 0;
                    //    image.Index = (uint)index;
                    //}
                    if (image.NextFrame == null)
                    {
                        image.NextFrame = CPF.Platform.Application.Elapsed + TimeSpan.FromMilliseconds(image.FrameDelay[0]);
                        image.Index = 0;
                    }
                    else
                    {
                        if (image.NextFrame <= CPF.Platform.Application.Elapsed)
                        {
                            index++;
                            if (index >= count)
                            {
                                index = 0;
                            }
                            image.NextFrame = image.NextFrame + TimeSpan.FromMilliseconds(image.FrameDelay[index]);
                            image.Index = (uint)index;
                        }
                    }
                }
                Invalidate();
            }
        }
        [PropertyChanged(nameof(Source))]
        void RegisterSource(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (newValue != null)
            {
                if (newValue is Image image)
                {
                    SetImage(image);
                    needDisposeImg = false;
                }
                else if (newValue is Bitmap bitmap)
                {
                    SetImage(bitmap);
                    needDisposeImg = false;
                }
                else if (newValue is string)
                {
                    var s = newValue as string;
                    ResourceManager.GetImage(s, a =>
                    {
                        Invoke(() =>
                        {
                            SetImage(a);
                            needDisposeImg = false;
                            if (a == null)
                            {
                                SetImage(ResourceManager.ErrorImage);
                                RaiseEvent(EventArgs.Empty, nameof(ImageFailed));
                            }
                        });
                    });
                }
                else if (newValue is Stream)
                {
                    try
                    {
                        SetImage(Image.FromStream(newValue as Stream));
                        needDisposeImg = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("加载图片失败：" + e.Message);
                        SetImage(ResourceManager.ErrorImage);
                        RaiseEvent(EventArgs.Empty, nameof(ImageFailed));
                    }
                }
                else if (newValue is byte[])
                {
                    try
                    {
                        SetImage(Image.FromBuffer((byte[])newValue));
                        needDisposeImg = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("加载图片失败：" + e.Message);
                        SetImage(ResourceManager.ErrorImage);
                        RaiseEvent(EventArgs.Empty, nameof(ImageFailed));
                    }
                }
            }
            else
            {
                SetImage(null);
            }
        }


        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            if (count > 1 && img != null)
            {
                if (timer == null)
                {
                    timer = new Threading.DispatcherTimer();
                    timer.Tick += Timer_Tick;
                    timer.Interval = TimeSpan.FromMilliseconds(50);
                }

                timer.Start();
            }
        }

        protected override void OnDetachedFromVisualTree()
        {
            base.OnDetachedFromVisualTree();
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            if (img != null && img.ImageImpl == null)
            {
                img = null;
            }
            if (img != null)
            {
                var naturalSize = new Size(img.Width, img.Height);
                var size = availableSize;
                var maxW = MaxWidth;
                if (!maxW.IsAuto && maxW.Unit == Unit.Default)
                {
                    size.Width = maxW.Value;
                }
                var maxH = MaxHeight;
                if (!maxH.IsAuto && maxH.Unit == Unit.Default)
                {
                    size.Height = maxH.Value;
                }
                Size scaleFactor = ComputeScaleFactor(size,
                                                         naturalSize,
                                                         this.Stretch, StretchDirection);

                // Returns our minimum size & sets DesiredSize.
                return new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);

            }
            return base.MeasureOverride(availableSize);
        }

        //protected override Size ArrangeOverride(in Size finalSize)
        //{
        //    if (img != null)
        //    {
        //        var naturalSize = new Size(img.Width, img.Height);
        //        Size scaleFactor = ComputeScaleFactor(finalSize,
        //                                                 naturalSize,
        //                                                 this.Stretch, StretchDirection);

        //        // Returns our minimum size & sets DesiredSize.
        //        return new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);

        //    }
        //    return base.ArrangeOverride(finalSize);
        //}

        /// <summary>
        /// This is a helper function that computes scale factors depending on a target size and a content size
        /// </summary>
        /// <param name="availableSize">Size into which the content is being fitted.</param>
        /// <param name="contentSize">Size of the content, measured natively (unconstrained).</param>
        /// <param name="stretch">Value of the Stretch property on the element.</param>
        /// <param name="stretchDirection">Value of the StretchDirection property on the element.</param>
        internal static Size ComputeScaleFactor(Size availableSize,
                                                Size contentSize,
                                                Stretch stretch, StretchDirection stretchDirection)
        {
            // Compute scaling factors to use for axes
            float scaleX = 1.0f;
            float scaleY = 1.0f;

            bool isConstrainedWidth = !float.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !float.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill)
                 && (isConstrainedWidth || isConstrainedHeight))
            {
                // Compute scaling factors for both axes
                scaleX = (FloatUtil.IsZero(contentSize.Width)) ? 0f : availableSize.Width / contentSize.Width;
                scaleY = (FloatUtil.IsZero(contentSize.Height)) ? 0f : availableSize.Height / contentSize.Height;

                if (!isConstrainedWidth) scaleX = scaleY;
                else if (!isConstrainedHeight) scaleY = scaleX;
                else
                {
                    // If not preserving aspect ratio, then just apply transform to fit
                    switch (stretch)
                    {
                        case Stretch.Uniform:       //Find minimum scale that we use for both axes
                            float minscale = scaleX < scaleY ? scaleX : scaleY;
                            scaleX = scaleY = minscale;
                            break;

                        case Stretch.UniformToFill: //Find maximum scale that we use for both axes
                            float maxscale = scaleX > scaleY ? scaleX : scaleY;
                            scaleX = scaleY = maxscale;
                            break;

                        case Stretch.Fill:          //We already computed the fill scale factors above, so just use them
                            break;
                    }
                }

                //Apply stretch direction by bounding scales.
                //In the uniform case, scaleX=scaleY, so this sort of clamping will maintain aspect ratio
                //In the uniform fill case, we have the same result too.
                //In the fill case, note that we change aspect ratio, but that is okay
                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0) scaleX = 1f;
                        if (scaleY < 1.0) scaleY = 1f;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0) scaleX = 1f;
                        if (scaleY > 1.0) scaleY = 1f;
                        break;

                    case StretchDirection.Both:
                        break;

                    default:
                        break;
                }
            }
            //Return this as a size now
            return new Size(scaleX, scaleY);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (img != null && needDisposeImg)
            {
                img.Dispose();
                img = null;
            }
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }
        /// <summary>
        /// 图片加载失败
        /// </summary>
        public event EventHandler ImageFailed
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ClipToBounds), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (img != null)
            {
                var size = ActualSize;
                var w = img.Width;
                var h = img.Height;
                switch (this.Stretch)
                {
                    case Stretch.None:
                        dc.DrawImage(img, new Rect((size.Width - w) / 2, (size.Height - h) / 2, w, h), new Rect(0, 0, w, h));
                        break;
                    case Stretch.Fill:
                        dc.DrawImage(img, new Rect(0, 0, size.Width, size.Height), new Rect(0, 0, w, h));
                        break;
                    case Stretch.Uniform:
                        var ww = size.Width;
                        var hh = size.Height;
                        if (w / size.Width > h / size.Height)
                        {
                            hh = size.Width * h / w;
                        }
                        else
                        {
                            ww = size.Height * w / h;
                        }
                        dc.DrawImage(img, new Rect((size.Width - ww) / 2, (size.Height - hh) / 2, ww, hh), new Rect(0, 0, w, h));
                        break;
                    case Stretch.UniformToFill:
                        var www = size.Width;
                        var hhh = size.Height;
                        if (w / size.Width < h / size.Height)
                        {
                            hhh = size.Width * h / w;
                        }
                        else
                        {
                            www = size.Height * w / h;
                        }
                        dc.DrawImage(img, new Rect((size.Width - www) / 2, (size.Height - hhh) / 2, www, hhh), new Rect(0, 0, w, h));
                        break;
                }
            }
            base.OnRender(dc);
        }
    }

    public enum Stretch : byte
    {
        /// <summary>
        /// 内容保持其原始大小。
        /// </summary>
        None,
        /// <summary>
        /// 调整内容大小以填充目标尺寸。 不保留纵横比。
        /// </summary>
        Fill,
        /// <summary>
        /// 在保留内容原有纵横比的同时调整内容的大小，以适合目标尺寸。
        /// </summary>
        Uniform,
        /// <summary>
        /// 在保留内容原有纵横比的同时调整内容的大小，以填充目标尺寸。 如果目标矩形的纵横比不同于源矩形的纵横比，则对源内容进行剪裁以适合目标尺寸。
        /// </summary>
        UniformToFill,
    }

    public enum StretchDirection : byte
    {
        /// <summary>
        /// 内容仅在小于父级时扩展。 如果内容较大，则不执行缩放。
        /// </summary>
        UpOnly,

        /// <summary>
        /// 内容仅在大于父级时缩放。 如果内容较小，则不会执行任何扩展。
        /// </summary>
        DownOnly,

        /// <summary>
        /// 内容根据 Stretch 模式进行拉伸以适合父项的大小。
        /// </summary>
        Both
    }
}
