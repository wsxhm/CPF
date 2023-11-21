using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using System.Reflection;
using System.Linq;
using CPF.Drawing;
using CPF.Reflection;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 支持可视化设计的控件基类
    /// </summary>
    [Description("支持可视化设计的控件基类")]
    public class Control : UIElement
    {
        /// <summary>
        /// 背景填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender), Description("背景填充")]
        public ViewFill Background
        {
            get { return (ViewFill)GetValue(45); }
            set { SetValue(value, 45); }
        }

        /// <summary>
        /// 边框线条填充
        /// </summary>
        [Description("边框线条填充")]
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill BorderFill
        {
            get { return (ViewFill)GetValue(46); }
            set { SetValue(value, 46); }
        }
        /// <summary>
        /// 获取或设置线条类型
        /// </summary>
        [Description("获取或设置线条类型")]
        [UIPropertyMetadata(typeof(Stroke), "0", UIPropertyOptions.AffectsRender)]
        public Stroke BorderStroke
        {
            get { return (Stroke)GetValue(47); }
            set { SetValue(value, 47); }
        }

        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// </summary>
        [Description("获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom")]
        [UIPropertyMetadata(typeof(Thickness), "0", UIPropertyOptions.AffectsMeasure)]
        public Thickness Padding
        {
            get { return GetValue<Thickness>(56); }
            set { SetValue(value, 56); }
        }
        /// <summary>
        /// 四周边框粗细
        /// </summary>
        [Description("四周边框粗细")]
        [UIPropertyMetadata(typeof(Thickness), "0", UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender)]
        public Thickness BorderThickness
        {
            get { return GetValue<Thickness>(48); }
            set { SetValue(value, 48); }
        }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// </summary>
        [Description("获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft")]
        [UIPropertyMetadata(typeof(CornerRadius), "0", UIPropertyOptions.AffectsRender)]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(50); }
            set { SetValue(value, 50); }
        }

        /// <summary>
        /// 边框类型，BorderStroke和BorderThickness
        /// </summary>
        [UIPropertyMetadata(BorderType.BorderStroke, UIPropertyOptions.AffectsMeasure | UIPropertyOptions.AffectsRender), Description("边框类型，BorderStroke和BorderThickness")]
        public BorderType BorderType
        {
            get { return GetValue<BorderType>(49); }
            set { SetValue(value, 49); }
        }

        /// <summary>
        /// 字体名
        /// </summary>
        [UIPropertyMetadata("宋体", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits), Description("字体名")]
        public string FontFamily
        {
            get { return (string)GetValue(51); }
            set { SetValue(value, 51); }
        }

        /// <summary>
        /// 字体尺寸，点
        /// </summary>
        [UIPropertyMetadata(12f, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits), Description("字体尺寸，点")]
        public float FontSize
        {
            get { return (float)GetValue(52); }
            set { SetValue(value, 52); }
        }

        /// <summary>
        /// 字体样式
        /// </summary>
        [UIPropertyMetadata(FontStyles.Regular, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure | UIPropertyOptions.Inherits), Description("字体样式")]
        public FontStyles FontStyle
        {
            get { return (FontStyles)GetValue(53); }
            set { SetValue(value, 53); }
        }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// </summary>
        [UIPropertyMetadata(typeof(TextDecoration), "", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits)]
        [Description("表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]")]
        public TextDecoration TextDecoration
        {
            get { return (TextDecoration)GetValue(57); }
            set { SetValue(value, 57); }
        }
        /// <summary>
        /// 控件文字的填充
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits), Description("控件文字的填充")]
        public ViewFill Foreground
        {
            get { return (ViewFill)GetValue(54); }
            set { SetValue(value, 54); }
        }

        /// <summary>
        /// 初始化模板，重写时一般不需要调用base.InitializeComponent()
        /// </summary>
        protected virtual void InitializeComponent()
        {

        }
        /// <summary>
        /// 是否初始化组件完成
        /// </summary>
        [Browsable(false)]
        public bool IsInitialized
        {
            get { return GetValue<bool>(55); }
            internal set { SetValue(value, 55); }
        }

        public Control()
        {
            loadStyle = false;
        }
        /// <summary>
        /// 模板，用来外部替换InitializeComponent操作，第一个参数就是当前对象，第二个就是当前对象的Children，由于Children大部分情况下是Protect，所以这里通过参数提供。初始化组件之前设置才有意义
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public Action<UIElement, UIElementCollection> Template { get; set; }

        [PropertyChanged(nameof(Visibility))]
        void OnVisibility(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!IsInitialized && Root != null)
            {
                if (NeedInitialize(this))
                {
                    //Initialize();
                    Root.OnElementInitialize(this);
                }
            }
            else if ((Visibility)newValue == Visibility.Visible && Root != null && Children.Count > 0 && NeedInitialize(this))
            {
                foreach (Control item in Children.Where(a => a is Control))
                {
                    item.CheckIni();
                }
            }
        }

        void CheckIni()
        {
            if (Visibility == Visibility.Visible)
            {
                if (!IsInitialized)
                {
                    Root.OnElementInitialize(this);
                }
                else
                {
                    foreach (Control item in Children.Where(a => a is Control))
                    {
                        item.CheckIni();
                    }
                }
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(Visibility))
        //    {
        //        if (!IsInitialized && Root != null)
        //        {
        //            if (NeedInitialize(this))
        //            {
        //                Initialize();
        //            }
        //        }
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            if (NeedInitialize(this))
            {
                //Initialize();
                Root.OnElementInitialize(this);
            }
        }

        bool NeedInitialize(UIElement element)
        {
            if (element.Visibility == Visibility.Collapsed && !(element is Popup))
            {
                return false;
            }
            if (element.Parent is Control control)
            {
                return NeedInitialize(control);
            }
            return true;
        }

        void ini(Control control)
        {
            foreach (Control item in control.Children.Where(a => a is Control))
            {
                if (!item.IsInitialized && item.Visibility != Visibility.Collapsed)
                {
                    //item.Initialize();
                    Root.OnElementInitialize(item);
                    ini(item);
                }
            }
        }

        /// <summary>
        /// 调用内部InitializeComponent初始化组件
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                if (Children.Count > 0)
                {
                    //foreach (Control item in Find<Control>())
                    //{
                    //    if (item.Visibility != Visibility.Collapsed)
                    //    {
                    //        item.Initialize();
                    //    }
                    //}
                    ini(this);
                }
                if (Template == null)
                {
                    InitializeComponent();
                }
                else
                {
                    Template(this, this.Children);
                }
                IsInitialized = true;
                OnInitialized();
            }
            LoadStyle();
        }

        protected virtual void OnInitialized()
        {
            if (triggers != null)
            {
                triggers.Sort((a, b) =>
                {
                    if (a.Style == b.Style)
                    {
                        return 0;
                    }
                    if (a.Style != null && b.Style != null)
                    {
                        return a.Style.Index - b.Style.Index;
                    }
                    if (a.Style != null)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                });
            }
            this.RaiseEvent(EventArgs.Empty, nameof(Initialized));
        }

        public event EventHandler Initialized
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


        protected override Size MeasureOverride(in Size availableSize)
        {
            var ava = availableSize;
            var p = Padding;
            Size size;
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                ava.Width = Math.Max(0, ava.Width - p.Left - p.Right - b.Width - b.Width);
                ava.Height = Math.Max(0, ava.Height - p.Top - p.Bottom - b.Width - b.Width);
                size = base.MeasureOverride(ava);
                size.Width = p.Left + p.Right + size.Width + b.Width + b.Width;
                size.Height = p.Top + p.Bottom + size.Height + b.Width + b.Width;
            }
            else
            {
                var b = BorderThickness;
                ava.Width = Math.Max(0, ava.Width - p.Left - p.Right - b.Left - b.Right);
                ava.Height = Math.Max(0, ava.Height - p.Top - p.Bottom - b.Top - b.Bottom);
                size = base.MeasureOverride(ava);
                size.Width = p.Left + p.Right + size.Width + b.Left + b.Right;
                size.Height = p.Top + p.Bottom + size.Height + b.Top + b.Bottom;
            }
            return size;
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            var p = Padding;
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                rect.Location = new Point(p.Left + b.Width, p.Top + b.Width);
                rect.Size = new Size(Math.Max(0, rect.Width - p.Left - p.Right - b.Width - b.Width), Math.Max(0, rect.Height - p.Top - p.Bottom - b.Width - b.Width));
            }
            else
            {
                var b = BorderThickness;
                rect.Location = new Point(p.Left + b.Left, p.Top + b.Top);
                rect.Size = new Size(Math.Max(0, rect.Width - p.Left - p.Right - b.Left - b.Right), Math.Max(0, rect.Height - p.Top - p.Bottom - b.Top - b.Bottom));
            }
            foreach (UIElement child in Children)
            {
                child.Arrange(rect);
            }
            return finalSize;
        }
        protected override void OnRender(DrawingContext dc)
        {
            var cr = CornerRadius;
            var s = ActualSize;

            var rect = new Rect(0, 0, Math.Max(0, s.Width), Math.Max(0, s.Height));
            if (BorderType == BorderType.BorderStroke)
            {
                var bs = BorderStroke;

                using (var path = new PathGeometry())
                {
                    if (UseLayoutRounding)
                    {
                        bs.Width = (float)Math.Round(bs.Width * Root.RenderScaling) / Root.RenderScaling;
                    }
                    var w2 = bs.Width / 2;
                    path.BeginFigure(w2 + cr.TopLeft + rect.Left, w2 + rect.Top);
                    path.LineTo(rect.Right - w2 - cr.TopRight, w2 + rect.Top);
                    path.ArcTo(new Point(rect.Right - w2, w2 + cr.TopRight + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                    path.LineTo(rect.Right - w2, rect.Bottom - cr.BottomRight - w2);
                    path.ArcTo(new Point(rect.Right - w2 - cr.BottomRight, rect.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                    path.LineTo(w2 + cr.BottomLeft + rect.Left, rect.Bottom - w2);
                    path.ArcTo(new Point(w2 + rect.Left, rect.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                    path.LineTo(w2 + rect.Left, w2 + cr.TopLeft + rect.Top);
                    path.ArcTo(new Point(w2 + cr.TopLeft + rect.Left, w2 + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                    path.EndFigure(true);

                    var ba = Background;
                    if (ba != null)
                    {
                        using (var brush = ba.CreateBrush(rect, Root.RenderScaling))
                        {
                            dc.FillPath(brush, path);
                        }
                    }
                    if (bs.Width > 0)
                    {
                        var bf = BorderFill;
                        if (bf != null)
                        {
                            using (var brush = bf.CreateBrush(rect, Root.RenderScaling))
                            {
                                dc.DrawPath(brush, bs, path);
                            }
                        }
                    }
                }

            }
            else
            {
                var bt = BorderThickness;

                if (!cr.IsZero)
                {
                    using (var path = new PathGeometry())
                    {
                        using (var p = new PathGeometry())
                        {
                            var w2 = 0.5f;

                            p.BeginFigure(w2 + cr.TopLeft + bt.Left + rect.Left, w2 + bt.Top + rect.Top);
                            p.LineTo(rect.Right - w2 - cr.TopRight - bt.Right, w2 + bt.Top + rect.Top);
                            p.ArcTo(new Point(rect.Right - w2 - bt.Right, w2 + cr.TopRight + bt.Top + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                            p.LineTo(rect.Right - w2 - bt.Right, rect.Bottom - cr.BottomRight - w2 - bt.Bottom);
                            p.ArcTo(new Point(rect.Right - w2 - cr.BottomRight - bt.Right, rect.Bottom - w2 - bt.Bottom), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                            p.LineTo(w2 + cr.BottomLeft + bt.Left + rect.Left, rect.Bottom - w2 - bt.Bottom);
                            p.ArcTo(new Point(w2 + bt.Left + rect.Left, rect.Bottom - w2 - cr.BottomLeft - bt.Bottom), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                            p.LineTo(w2 + bt.Left + rect.Left, w2 + cr.TopLeft + bt.Top + rect.Top);
                            p.ArcTo(new Point(w2 + cr.TopLeft + bt.Left + rect.Left, w2 + bt.Top + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                            p.EndFigure(true);
                            //w2 = 1;
                            path.BeginFigure(w2 + cr.TopLeft + rect.Left, w2 + rect.Top);
                            path.LineTo(rect.Right - w2 - cr.TopRight, w2 + rect.Top);
                            path.ArcTo(new Point(rect.Right - w2, w2 + cr.TopRight + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                            path.LineTo(rect.Right - w2, rect.Bottom - cr.BottomRight - w2);
                            path.ArcTo(new Point(rect.Right - w2 - cr.BottomRight, rect.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                            path.LineTo(w2 + cr.BottomLeft + rect.Left, rect.Bottom - w2);
                            path.ArcTo(new Point(w2 + rect.Left, rect.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                            path.LineTo(w2 + rect.Left, w2 + cr.TopLeft + rect.Top);
                            path.ArcTo(new Point(w2 + cr.TopLeft + rect.Left, w2 + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                            path.EndFigure(true);

                            using (var geo = new Geometry(path))
                            {
                                using (var ge = new Geometry(p))
                                {
                                    geo.Xor(ge);

                                    var bf = BorderFill;
                                    Brush brush = null;
                                    var str = new Stroke(1);
                                    var ww = 0.8f;
                                    if (bf != null)
                                    {
                                        brush = bf.CreateBrush(rect, Root.RenderScaling);
                                    }
                                    if (brush != null)
                                    {
                                        dc.FillGeometry(brush, geo);
                                    }

                                    var ba = Background;
                                    if (ba != null)
                                    {
                                        using (var brush1 = ba.CreateBrush(rect, Root.RenderScaling))
                                        {
                                            dc.FillGeometry(brush1, ge);
                                            dc.DrawPath(brush1, new Stroke(0.8f), p);
                                        }
                                    }
                                    if (brush != null)
                                    {
                                        if (cr.TopLeft > 0 && (bt.Top > 0 || bt.Left > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(ww + rect.Left, ww + cr.TopLeft + rect.Top);
                                                pp.ArcTo(new Point(ww + cr.TopLeft + rect.Left, ww + rect.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }
                                        if (cr.TopRight > 0 && (bt.Top > 0 || bt.Right > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(rect.Right - ww - cr.TopRight, ww + rect.Top);
                                                pp.ArcTo(new Point(rect.Right - ww, ww + cr.TopRight + rect.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }
                                        if (cr.BottomLeft > 0 && (bt.Bottom > 0 || bt.Left > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(ww + cr.BottomLeft + rect.Left, rect.Bottom - ww);
                                                pp.ArcTo(new Point(ww + rect.Left, rect.Bottom - ww - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }
                                        if (cr.BottomRight > 0 && (bt.Bottom > 0 || bt.Right > 0))
                                        {
                                            using (var pp = new PathGeometry())
                                            {
                                                pp.BeginFigure(rect.Right - ww, rect.Bottom - cr.BottomRight - ww);
                                                pp.ArcTo(new Point(rect.Right - ww - cr.BottomRight, rect.Bottom - ww), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
                                                pp.EndFigure(false);
                                                dc.DrawPath(brush, str, pp);
                                            }
                                        }

                                        ww = 0.5f;
                                        str = new Stroke(1);
                                        if (bt.Left > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(ww + rect.Left, ww + cr.TopLeft + rect.Top), new Point(ww + rect.Left, rect.Bottom - cr.BottomLeft - ww));
                                        }
                                        if (bt.Top > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(ww + cr.TopLeft + rect.Left, ww + rect.Top), new Point(rect.Right - ww - cr.TopRight, ww + rect.Top));
                                        }
                                        if (bt.Right > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(rect.Right - ww, ww + cr.TopRight + rect.Top), new Point(rect.Right - ww, rect.Bottom - cr.BottomRight - ww));
                                        }
                                        if (bt.Bottom > 0)
                                        {
                                            dc.DrawLine(str, brush, new Point(ww + cr.BottomLeft + rect.Left, rect.Bottom - ww), new Point(rect.Right - ww - cr.BottomRight, rect.Bottom - ww));
                                        }
                                        brush.Dispose();
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    var bf = BorderFill;
                    if (UseLayoutRounding || !IsAntiAlias)
                    {
                        var sc = Root.RenderScaling;
                        bt.Left = (float)Math.Round(bt.Left * sc) / sc;
                        bt.Right = (float)Math.Round(bt.Right * sc) / sc;
                        bt.Top = (float)Math.Round(bt.Top * sc) / sc;
                        bt.Bottom = (float)Math.Round(bt.Bottom * sc) / sc;
                    }
                    if (bf != null)
                    {
                        using (var brush = bf.CreateBrush(rect, Root.RenderScaling))
                        {
                            if (bt.Top > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Top), brush, new Point(rect.Left, bt.Top / 2 + rect.Top), new Point(rect.Right, bt.Top / 2 + rect.Top));
                            }
                            if (bt.Left > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Left), brush, new Point(bt.Left / 2 + rect.Left, rect.Top), new Point(bt.Left / 2 + rect.Left, rect.Bottom));
                            }
                            if (bt.Right > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Right), brush, new Point(rect.Right - bt.Right / 2, rect.Top), new Point(rect.Right - bt.Right / 2, rect.Bottom));
                            }
                            if (bt.Bottom > 0)
                            {
                                dc.DrawLine(new Stroke(bt.Bottom), brush, new Point(rect.Left, rect.Bottom - bt.Bottom / 2), new Point(rect.Right, rect.Bottom - bt.Bottom / 2));
                            }
                        }
                    }
                    var ba = Background;
                    if (ba != null)
                    {
                        using (var brush = ba.CreateBrush(rect, Root.RenderScaling))
                        {
                            dc.FillRectangle(brush, new Rect(bt.Left + rect.Left, bt.Top + rect.Top, Math.Max(0, rect.Width - bt.Left - bt.Right), Math.Max(0, rect.Height - bt.Top - bt.Bottom)));
                        }
                    }
                }
            }
        }

        //protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        //{
        //    base.OnOverrideMetadata(overridePropertys);
        //    overridePropertys.Override(nameof(UseLayoutRounding), new PropertyMetadataAttribute(true));
        //}

        public override object Clone()
        {
            var control = base.Clone() as Control;
            if (this.IsInitialized)
            {
                control.Initialize();
            }
            return control;
        }
    }
}
