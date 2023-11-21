using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Input;
using CPF.Styling;
using CPF.Shapes;
using CPF.Reflection;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using CPF.Controls;
using CPF.Drawing;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace CPF
{
    /// <summary>
    /// 提供UI相关的特性，图像，鼠标事件，触摸事件，布局，拖拽事件
    /// </summary>
    //[ContentProperty("Children")]
    //[Designer(typeof(Design.DocumentDesigner), typeof(IRootDesigner))]
    //[Designer(typeof(Design.DocumentDesigner), typeof(IDesigner))]
    [DesignerCategory("Component")]
    //[DesignerSerializer(typeof(),typeof(System.ComponentModel.Design.Serialization.CodeDomSerializer))]
    [ToolboxItem(false)]
    public class UIElement : Visual, IComponent//, IEnumerable<UIElement>//, INamed//, IInputElement
    {
        //internal bool needSortZIndex = false;
        //List<UIElement> visibleElements;
        internal UIElement mouseOverChild = null;
        internal UIElement dragOverChild = null;
        Size desiredSize;
        Rect previousRenderRect;
        //Size previousDesiredSize;
        //bool isMeasureValid = true;
        internal Rect? _previousArrange;
        internal Size? _previousMeasure;
        Rect contentBounds;
        HybridDictionary<INotifyPropertyChanged, HashSet<UIPropertyMetadataAttribute>> notifyList;
        Effects.Effect effect;
        static Popup tooltip;
        static Threading.DispatcherTimer timer;
        internal Delegate afterStyle;
        static Popup ToolTipHost
        {
            get
            {
                if (tooltip == null)
                {
                    tooltip = new Popup() { Name = "tooltip", CanActivate = false, Background = Color.Transparent };
                }
                return tooltip;
            }
        }

        static void ShowTip(View root, UIElement toolTipElement)
        {
            if (timer == null)
            {
                timer = new Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(.3);
                timer.Tick += Timer_Tick;
            }
            UIElement.toolTipRoot = root;
            ToolTipUIElement = toolTipElement;
            timer.Start();
        }
        static View toolTipRoot;
        static UIElement toolTipUIElement;
        static UIElement ToolTipUIElement
        {
            get { return toolTipUIElement; }
            set
            {
                if (toolTipUIElement != value)
                {
                    if (toolTipUIElement != null)
                    {
                        ToolTipHost.Children.Remove(toolTipUIElement);
                        if (tooltip.Visibility == Visibility.Visible)
                        {
                            tooltip.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                toolTipUIElement = value;
            }
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if (toolTipRoot == null || !toolTipUIElement)
            {
                return;
            }
            //var p = MouseDevice.Location;
            //ToolTipHost.MarginLeft = p.X / toolTipRoot.LayoutScaling + 10;
            //ToolTipHost.MarginTop = p.Y / toolTipRoot.LayoutScaling + 10;
            //ToolTipHost.Position = new PixelPoint(p.X + 10, p.Y + 10);
            ToolTipHost.MarginLeft = 10;
            ToolTipHost.MarginTop = 10;
            ToolTipHost.Placement = PlacementMode.Mouse;
            ToolTipHost.PlacementTarget = toolTipRoot;
            ToolTipHost.LoadStyle(toolTipRoot);
            ToolTipHost.Children.Add(toolTipUIElement);
            ToolTipHost.Visibility = Visibility.Visible;
            UIElement.toolTipRoot = null;
            //toolTipUIElement = null;
        }

        internal Classes classes;
        internal long lastMouseDownTime;
        Point lastMouseDown;
        internal void RaiseDeviceEvent(EventArgs e, EventType eventName)
        {
            if (Root == null)
            {
                return;
            }
            switch (eventName)
            {
                case EventType.MouseMove:
                    OnMouseMove((MouseEventArgs)e);
                    break;
                case EventType.PreviewMouseDown:
                    OnPreviewMouseDown((MouseButtonEventArgs)e);
                    break;
                case EventType.PreviewMouseUp:
                    OnPreviewMouseUp((MouseButtonEventArgs)e);
                    break;
                case EventType.MouseDown:
                    {
                        OnMouseDown((MouseButtonEventArgs)e);
                        var p = ((MouseButtonEventArgs)e).Location;
                        if (lastMouseDownTime != 0)
                        {
                            var time = DateTime.FromBinary(lastMouseDownTime);
                            var now = DateTime.FromBinary(((MouseButtonEventArgs)e).timestamp);
                            if (Platform.Application.GetRuntimePlatform().DoubleClickTime > now - time && Math.Abs(p.X - lastMouseDown.X) < 2 && Math.Abs(p.Y - lastMouseDown.Y) < 2)
                            {
                                OnDoubleClick((RoutedEventArgs)e);
                                return;
                            }
                        }
                        lastMouseDown = p;
                        lastMouseDownTime = ((MouseButtonEventArgs)e).timestamp;
                        break;
                    }

                case EventType.MouseUp:
                    OnMouseUp((MouseButtonEventArgs)e);
                    break;
                case EventType.MouseEnter:
                    OnMouseEnter((MouseEventArgs)e);
                    break;
                case EventType.MouseLeave:
                    OnMouseLeave((MouseEventArgs)e);
                    break;
                case EventType.MouseWheel:
                    OnMouseWheel((MouseWheelEventArgs)e);
                    break;
                case EventType.KeyDown:
                    OnKeyDown((KeyEventArgs)e);
                    break;
                case EventType.KeyUp:
                    OnKeyUp((KeyEventArgs)e);
                    break;
                case EventType.TextInput:
                    OnTextInput((TextInputEventArgs)e);
                    break;
                case EventType.DragOver:
                    OnDragOver((DragEventArgs)e);
                    break;
                case EventType.DragEnter:
                    var dea = e as DragEventArgs;
                    OnDragEnter(dea);
                    break;
                case EventType.DragLeave:
                    OnDragLeave(e);
                    break;
                case EventType.Drop:
                    OnDrop((DragEventArgs)e);
                    break;
                case EventType.TouchDown:
                    OnTouchDown((TouchEventArgs)e);
                    break;
                case EventType.TouchUp:
                    OnTouchUp((TouchEventArgs)e);
                    break;
                //case EventType.TouchEnter:
                //    OnTouchEnter((TouchEventArgs)e);
                //    break;
                //case EventType.TouchLeave:
                //    OnTouchLeave((TouchEventArgs)e);
                //break;
                case EventType.TouchMove:
                    OnTouchMove((TouchEventArgs)e);
                    break;
            }
        }
        public UIElement()
        {
            loadStyle = true;
            IsMeasureValid = true;
            if (inheritsPropertyName != null)
            {
                inheritsValues = new Dictionary<string, InheritsValue>();
            }
        }
        Dictionary<string, InheritsValue> inheritsValues;

        List<UIElement> presenters;
        /// <summary>
        /// 被标记了的元素
        /// </summary>
        [NotCpfProperty, Browsable(false)]
        public IEnumerable<UIElement> Presenters
        {
            get { return presenters; }
        }

        UIElement presenterFor;
        /// <summary>
        /// 用作模板中的特殊元素的标记，强引用，不使用了的元素只Remove是不行的，需要设置PresenterFor=null或者调用Dispose
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public UIElement PresenterFor
        {
            get { return presenterFor; }
            set
            {
                if (presenterFor != value)
                {
                    if (value != null)
                    {
                        if (value.presenters == null)
                        {
                            value.presenters = new List<UIElement>();
                        }
                        value.presenters.Add(this);
                    }
                    if (presenterFor != null && presenterFor.presenters != null)
                    {
                        presenterFor.presenters.Remove(this);
                    }
                    presenterFor = value;
                }

            }
        }
        /// <summary>
        /// 查找标记了的特殊元素
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FindPresenter<T>() where T : UIElement
        {
            //return Find<UIElement>().Where(a => a.PresenterFor == this);
            if (presenters == null)
            {
                yield break;
            }
            foreach (var item in presenters)
            {
                if (item is T t)
                {
                    yield return t;
                }
            }
        }
        /// <summary>
        /// 查找标记了的特殊元素，绑定的时候使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Func<UIElement, UIElement> FindPresenter<T>(Func<T, bool> func) where T : UIElement
        {
            Func<UIElement, UIElement> func1 = a =>
             {
                 return FindPresenter<T>().FirstOrDefault(func);
             };
            return func1;
        }

        /// <summary>
        /// 查找标记了的特殊元素
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UIElement> FindPresenter()
        {
            return FindPresenter<UIElement>();
        }
        /// <summary>
        /// 通过Name查找标记了的特殊元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T FindPresenterByName<T>(string name) where T : UIElement
        {
            return FindPresenter<T>().FirstOrDefault(a => a.Name == name);
        }
        /// <summary>
        /// 通过Name查找标记了的特殊元素。绑定的时候用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Func<UIElement, UIElement> FindPresenterByName(string name)
        {
            Func<UIElement, UIElement> func1 = b =>
             {
                 return FindPresenter<UIElement>().FirstOrDefault(a => a.Name == name);
             };
            return func1;
        }

        ///// <summary>
        ///// 添加子元素 Children.Add(element);
        ///// </summary>
        ///// <param name="element"></param>
        //public void Add(UIElement element)
        //{
        //    Children.Add(element);
        //}

        /// <summary>
        /// 位图特效
        /// </summary>
        [Description("位图特效")]
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        [Browsable(false)]
        public Effects.Effect Effect
        {
            get { return GetValue<Effects.Effect>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置在用户界面 (UI) 中为此元素显示的工具提示对象
        /// </summary>
        [Description("获取或设置在用户界面 (UI) 中为此元素显示的工具提示对象"), TypeConverter(typeof(StringConverter))]
        public object ToolTip
        {
            get { return GetValue(40); }
            set { SetValue(value, 40); }
        }
        UIElement toolTipElement;
        //public UIElement(IViewImpl host) : this()
        //{
        //    this.Host = host;
        //    IsRoot = true;
        //}
        /// <summary>
        /// 元素名称
        /// </summary>
        [Category("设计")]
        [Description("元素名称")]
        public virtual string Name
        {
            get { return GetValue<string>(34); }
            set { SetValue(value, 34); }
        }
        /// <summary>
        /// 键盘焦点
        /// </summary>
        [UIPropertyMetadata(false, UIPropertyOptions.AffectsRender)]
        [Description("键盘焦点")]
        public bool IsKeyboardFocused
        {
            get { return (bool)GetValue(21); }
            private set { SetValue(value, 21); }
        }
        /// <summary>
        /// 获取一个值，该值指示键盘焦点是否位于元素或其可视化树子元素内的任意位置
        /// </summary>
        [PropertyMetadata(false)]
        [Description("获取一个值，该值指示键盘焦点是否位于元素或其可视化树子元素内的任意位置")]
        public bool IsKeyboardFocusWithin
        {
            get { return (bool)GetValue(22); }
            private set { SetValue(value, 22); }
        }
        /// <summary>
        /// 获取焦点的导航方式
        /// </summary>
        [Description("获取焦点的导航方式")]
        [PropertyMetadata(null)]
        public NavigationMethod? FocusMethod
        {
            get { return (NavigationMethod?)GetValue(14); }
            private set { SetValue(value, 14); }
        }

        /// <summary>
        /// 是否可以获取焦点
        /// </summary>
        [Description("是否可以获取焦点")]
        [PropertyMetadata(false)]
        public bool Focusable
        {
            get { return GetValue<bool>(10); }
            set { SetValue(value, 10); }
        }

        /// <summary>
        /// 与控件关联的用户自定义数据
        /// </summary>
        [Description("与控件关联的用户自定义数据"), DefaultValue(null), TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get { return GetValue(39); }
            set { SetValue(value, 39); }
        }
        /// <summary>
        /// 是否有逻辑焦点
        /// </summary>
        [PropertyMetadata(false)]
        [Description("是否有逻辑焦点")]
        public bool IsFocused
        {
            get { return GetValue<bool>(19); }
            private set { SetValue(value, 19); }
        }
        /// <summary>
        /// tab键切换元素焦点时候的顺序
        /// </summary>
        [PropertyMetadata(0)]
        [Description("tab键切换元素焦点时候的顺序")]
        public int TabIndex
        {
            get { return GetValue<int>(38); }
            set { SetValue(value, 38); }
        }
        /// <summary>
        /// 按tab键切换焦点显示的聚焦框填充
        /// </summary>
        [Description("按tab键切换焦点显示的聚焦框填充")]
        [PropertyMetadata(typeof(ViewFill), "#000")]
        public ViewFill FocusFrameFill
        {
            get { return GetValue<ViewFill>(11); }
            set { SetValue(value, 11); }
        }
        /// <summary>
        /// 按tab键切换焦点显示的聚焦框
        /// </summary>
        [Description("按tab键切换焦点显示的聚焦框")]
        [PropertyMetadata(typeof(Stroke), "1,Dash")]
        public Stroke FocusFrameStroke
        {
            get { return GetValue<Stroke>(13); }
            set { SetValue(value, 13); }
        }
        /// <summary>
        /// 聚焦框到元素边缘距离
        /// </summary>
        [UIPropertyMetadata(typeof(Thickness), "3", UIPropertyOptions.AffectsRender), Description("聚焦框到元素边缘距离")]
        public Thickness FocusFramePadding
        {
            get { return GetValue<Thickness>(12); }
            set { SetValue(value, 12); }
        }

        /// <summary>
        /// 图形抗锯齿
        /// </summary>
        [Description("图形抗锯齿")]
        [UIPropertyMetadata(false, UIPropertyOptions.Inherits | UIPropertyOptions.AffectsRender)]
        public bool IsAntiAlias
        {
            get { return GetValue<bool>(16); }
            set { SetValue(value, 16); }
        }

        /// <summary>
        /// 该值指示此元素是否捕获了鼠标
        /// </summary>
        [PropertyMetadata(false)]
        [Description("该值指示此元素是否捕获了鼠标")]
        public bool IsMouseCaptured
        {
            get { return GetValue<bool>(23); }
            private set { SetValue(value, 23); }
        }

        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure)]
        public FloatField MinWidth
        {
            get { return GetValue<FloatField>(33); }
            set { SetValue(value, 33); }
        }
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure)]
        public FloatField MinHeight
        {
            get { return GetValue<FloatField>(32); }
            set { SetValue(value, 32); }
        }

        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public FloatField MaxWidth
        {
            get { return GetValue<FloatField>(31); }
            set { SetValue(value, 31); }
        }
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public FloatField MaxHeight
        {
            get { return GetValue<FloatField>(30); }
            set { SetValue(value, 30); }
        }
        /// <summary>
        /// 是否可以通过鼠标点击到
        /// </summary>
        [Description("是否可以通过鼠标点击到")]
        [PropertyMetadata(true)]
        public bool IsHitTestVisible
        {
            get { return GetValue<bool>(20); }
            set { SetValue(value, 20); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示此元素能否用作拖放操作的目标。
        /// </summary>
        [Description("获取或设置一个值，该值指示此元素能否用作拖放操作的目标。")]
        [UIPropertyMetadata(false, true)]
        public bool AllowDrop
        {
            get { return GetValue<bool>(5); }
            set { SetValue(value, 5); }
        }

        internal DragDropEffects DragDropEffects;

        /// <summary>
        /// Z轴
        /// </summary>
        [Category("布局")]
        [Description("Z轴")]
        [UIPropertyMetadata(0, UIPropertyOptions.AffectsArrange)]
        public int ZIndex
        {
            get { return GetValue<int>(44); }
            set { SetValue(value, 44); }
        }
        /// <summary>
        /// 默认值为 Auto。此值必须大于或等于 0。
        /// </summary>
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public virtual FloatField Width
        {
            get { return GetValue<FloatField>(43); }
            set { SetValue(value, 43); }
        }
        /// <summary>
        /// 默认值为 Auto。此值必须大于或等于 0。
        /// </summary>
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public virtual FloatField Height
        {
            get { return GetValue<FloatField>(15); }
            set { SetValue(value, 15); }
        }
        /// <summary>
        /// 非依赖属性
        /// </summary>
        [Browsable(false), NotCpfProperty]
        public virtual SizeField Size
        {
            get { return new SizeField(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        /// <summary>
        /// 非依赖属性
        /// </summary>
        [Browsable(false), NotCpfProperty]
        public virtual ThicknessField Margin
        {
            get { return new ThicknessField(MarginLeft, MarginTop, MarginRight, MarginBottom); }
            set
            {
                MarginLeft = value.Left;
                MarginTop = value.Top;
                MarginRight = value.Right;
                MarginBottom = value.Bottom;
            }
        }


        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public virtual FloatField MarginRight
        {
            get { return GetValue<FloatField>(28); }
            set { SetValue(value, 28); }
        }
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public virtual FloatField MarginBottom
        {
            get { return GetValue<FloatField>(26); }
            set { SetValue(value, 26); }
        }
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public virtual FloatField MarginLeft
        {
            get { return GetValue<FloatField>(27); }
            set { SetValue(value, 27); }
        }
        [Category("布局")]
        [UIPropertyMetadata(typeof(FloatField), "Auto", UIPropertyOptions.AffectsMeasure)]
        public virtual FloatField MarginTop
        {
            get { return GetValue<FloatField>(29); }
            set { SetValue(value, 29); }
        }
        ///// <summary>
        ///// 百分比是相对自身
        ///// </summary>
        //[UIPropertyMetadata(typeof(FloatValue), "0", UIPropertyOptions.AffectsMeasure)]
        //public FloatValue PaddingRight
        //{
        //    get { return (FloatValue)GetValue(); }
        //    set { SetValue(value); }
        //}
        ///// <summary>
        ///// 百分比是相对自身
        ///// </summary>
        //[UIPropertyMetadata(typeof(FloatValue), "0", UIPropertyOptions.AffectsMeasure)]
        //public FloatValue PaddingBottom
        //{
        //    get { return (FloatValue)GetValue(); }
        //    set { SetValue(value); }
        //}
        ///// <summary>
        ///// 百分比是相对自身
        ///// </summary>
        //[UIPropertyMetadata(typeof(FloatValue), "0", UIPropertyOptions.AffectsMeasure)]
        //public FloatValue PaddingLeft
        //{
        //    get { return (FloatValue)GetValue(); }
        //    set { SetValue(value); }
        //}
        ///// <summary>
        ///// 百分比是相对自身
        ///// </summary>
        //[UIPropertyMetadata(typeof(FloatValue), "0", UIPropertyOptions.AffectsMeasure)]
        //public FloatValue PaddingTop
        //{
        //    get { return (FloatValue)GetValue(); }
        //    set { SetValue(value); }
        //}
        //internal bool isRoot;
        /// <summary>
        /// 是否为根元素
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public bool IsRoot
        {
            get { return GetFlag(CoreFlags.isRoot); }
            internal set { SetFlag(CoreFlags.isRoot, value); }
        }

        /// <summary>
        /// 根元素
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public virtual View Root
        {
            get;
            internal set;
        }
        /// <summary>
        /// 获取一个值，该值指示此元素布局中的子元素的计算大小和位置是否有效。
        /// </summary>
        [NotCpfProperty]
        [Description("获取一个值，该值指示此元素布局中的子元素的计算大小和位置是否有效。")]
        public bool IsArrangeValid
        {
            get { return GetFlag(CoreFlags.IsArrangeValid); }
            internal set { SetFlag(CoreFlags.IsArrangeValid, value); }
        }

        /// <summary>
        /// 父级元素
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public virtual UIElement Parent
        {
            get;
            internal set;
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        [UIPropertyMetadata(true, true)]
        [Description("是否启用")]
        public virtual bool IsEnabled
        {
            get { return GetValue<bool>(18); }
            set { SetValue(value, 18); }
        }

        /// <summary>
        /// UI元素可见性
        /// </summary>
        [Category("布局")]
        [Description("UI元素可见性")]
        [UIPropertyMetadata(Visibility.Visible, UIPropertyOptions.AffectsMeasure)]
        public virtual Visibility Visibility
        {
            get { return (Visibility)GetValue(42); }
            set { SetValue(value, 42); }
        }
        /// <summary>
        /// 光标，用Cursors.***来设置
        /// </summary>
        [Description("光标")]
        [UIPropertyMetadata(typeof(Cursor), "Arrow", true)]
        public Cursor Cursor
        {
            get { return GetValue<Cursor>(7); }
            set { SetValue(value, 7); }
        }
        ///// <summary>
        ///// 元素是否有效，如果为false，将不能显示，而且不参与布局。在布局过程中使用，用来优化布局
        ///// </summary>
        //[NotCPFProperty]
        //public bool Valid
        //{
        //    get; set;
        //} = true;
        ///// <summary>
        ///// 外边距
        ///// </summary>
        //public virtual Thickness Margin
        //{
        //    get { return (Thickness)GetValue(MarginProperty); }
        //    set { SetValue(MarginProperty, value); }
        //}
        Rect renderBounds;
        /// <summary>
        /// 布局之后相对于根元素的矩形剪辑区域
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        [Description("布局之后相对于根元素的矩形剪辑区域")]
        public Rect RenderBounds { get { return renderBounds; } }

        /// <summary>
        /// 渲染变换
        /// </summary>
        [Browsable(false)]
        [Description("渲染变换")]
        [UIPropertyMetadata(typeof(Transform), "Identity", UIPropertyOptions.AffectsArrange)]
        public virtual Transform RenderTransform
        {
            get { return GetValue<Transform>(36); }
            set { SetValue(value, 36); }
        }
        /// <summary>
        /// 渲染原点
        /// </summary>
        [Category("布局")]
        [Description("渲染原点")]
        [UIPropertyMetadata(typeof(PointField), "50%,50%", UIPropertyOptions.AffectsArrange)]
        public PointField RenderTransformOrigin
        {
            get { return GetValue<PointField>(37); }
            set { SetValue(value, 37); }
        }
        /// <summary>
        /// 获取一个值，该值指示鼠标指针是否位于此元素（包括可视树上的子元素）上
        /// </summary>
        [PropertyMetadata(false)]
        [Description("获取一个值，该值指示鼠标指针是否位于此元素（包括可视树上的子元素）上")]
        public bool IsMouseOver
        {
            get { return GetValue<bool>(24); }
            private set { SetValue(value, 24); }
        }
        /// <summary>
        ///  获取一个值，该值指示鼠标拖拽指针是否位于此元素（包括可视树上的子元素）上
        /// </summary>
        [PropertyMetadata(false)]
        [Description("获取一个值，该值指示鼠标拖拽指针是否位于此元素（包括可视树上的子元素）上")]
        public bool IsDragOver
        {
            get { return GetValue<bool>(17); }
            private set { SetValue(value, 17); }
        }
        ///// <summary>
        ///// 获取一个值，鼠标左键是否按下
        ///// </summary>
        //[PropertyMetadata(false)]
        //public bool IsMouseLeftButtonDown
        //{
        //    get { return GetValue<bool>(); }
        //    private set { SetValue(value); }
        //}

        /// <summary>
        /// 获取元素呈现的尺寸
        /// </summary>
        //[NotCpfProperty]
        [Description("获取元素呈现的尺寸")]
        public Size ActualSize
        {
            get { return GetValue<Size>(4); }
            protected set { SetValue(value, 4); }
        }
        /// <summary>
        /// 元素偏移位置
        /// </summary>
        [NotCpfProperty]
        [Description("元素偏移位置")]
        public Point ActualOffset
        {
            get { return VisualOffset; }
        }
        /// <summary>
        /// 获取一个值，该值指示布局度量值返回的当前大小是否有效。
        /// </summary>
        [NotCpfProperty]
        [Description("获取一个值，该值指示布局度量值返回的当前大小是否有效。")]
        public bool IsMeasureValid
        {
            get { return GetFlag(CoreFlags.isMeasureValid); }
            private set { SetFlag(CoreFlags.isMeasureValid, value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应向此元素的大小和位置布局应用布局舍入。
        /// </summary>
        [PropertyMetadata(false)]
        [Description("获取或设置一个值，该值指示是否应向此元素的大小和位置布局应用布局舍入。")]
        public bool UseLayoutRounding
        {
            get { return GetValue<bool>(41); }
            set { SetValue(value, 41); }
        }
        /// <summary>
        /// 右键菜单
        /// </summary>
        [Browsable(false)]
        [Description("右键菜单")]
        public ContextMenu ContextMenu
        {
            get { return GetValue<ContextMenu>(6); }
            set { SetValue(value, 6); }
        }

        /// <summary>
        /// 使图像无效化，下次更新的时候重绘
        /// </summary>
        public void Invalidate()
        {
            var host = Root;
            if (host != null)
            {
                host.Invalidate(renderBounds);
            }
        }
        //public void Invalidate(Rect rect)
        //{
        //    IView host = Host;
        //    if (host != null)
        //    {
        //        host.Invalidate(GetHostClipBounds(rect));
        //    }
        //}
        /// <summary>
        /// 是否是该元素的祖先
        /// </summary>
        /// <param name="ancestors"></param>
        /// <returns></returns>
        public bool IsAncestors(UIElement ancestors)
        {
            if (ancestors == this)
            {
                return false;
            }
            var control = Parent;
            while (control != null)
            {
                if (control == this)
                {
                    return false;
                }
                else if (control == ancestors)
                {
                    return true;
                }
                control = control.Parent;
            }
            return false;
        }

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            //CpfObject p;
            //if (pm.PropertyName != nameof(Parent) && (p = Parent) != null && (pm is UIPropertyMetadataAttribute pma) && pma.Inherits && p.HasProperty(pm.PropertyName))
            //{
            //    return p.GetValue(pm.PropertyName);
            //}
            if (inheritsValues != null && inheritsValues.TryGetValue(pm.PropertyName, out InheritsValue value))
            {
                return value.Value;
            }
            return base.OnGetDefaultValue(pm);
        }
        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (value == null && propertyName == nameof(RenderTransform))
            {
                value = Transform.Identity;
            }
            return base.OnSetValue(propertyName, ref value);
        }

        public override bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (Root != null && !Threading.Dispatcher.MainThread.CheckAccess())
            {
                return InvokeSetValue(value, propertyName);
            }
            return base.SetValue(value, propertyName);
        }

        private bool InvokeSetValue<T>(T value, string propertyName)
        {
            var r = false;
            Invoke(() =>
            {
                r = base.SetValue(value, propertyName);
            });
            return r;
        }

        /// <summary>
        /// 内部使用，请勿调用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyIndex"></param>
        /// <returns></returns>
        protected override bool SetValue(object value, in byte propertyIndex)
        {
            if (Root != null && !Threading.Dispatcher.MainThread.CheckAccess())
            {
                return InovkeSetValue(value, propertyIndex);
            }
            return base.SetValue(value, in propertyIndex);
        }

        private bool InovkeSetValue(object value, byte propertyIndex)
        {
            var r = false;
            var pi = propertyIndex;
            Invoke(() =>
            {
                r = base.SetValue(value, pi);
            });
            return r;
        }

        public override T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (Root != null && !Threading.Dispatcher.MainThread.CheckAccess())
            {
                return InvokeGetValue<T>(propertyName);
            }
            return base.GetValue<T>(propertyName);
        }

        private T InvokeGetValue<T>(string propertyName)
        {
            T r = default;
            Invoke(() =>
            {
                r = base.GetValue<T>(propertyName);
            });
            return r;
        }

        /// <summary>
        /// 内部使用，请勿调用
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override object GetValue(in byte index)
        {
            if (Root != null && !Threading.Dispatcher.MainThread.CheckAccess())
            {
                return InvokeGetValue(index);
            }
            return base.GetValue(index);
        }

        private object InvokeGetValue(byte index)
        {
            object r = default;
            var i = index;
            Invoke(() =>
            {
                r = base.GetValue(i);
            });
            return r;
        }

        [NotCpfProperty]
        bool inheritsSet { set { SetFlag(CoreFlags.inheritsSet, value); } get { return GetFlag(CoreFlags.inheritsSet); } }

        [PropertyChanged(nameof(Effect))]
        void RegisterEffect(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            effect = newValue as Effects.Effect;
        }

        [PropertyChanged(nameof(ZIndex))]
        void RegisterZIndex(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            UIElement parent = Parent;
            if (parent != null)
            {
                parent.children.InvalidateZIndex();
            }
        }
        [PropertyChanged(nameof(RenderTransform))]
        void RegisterRenderTransform(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            VisualTransform = RenderTransform;
        }
        [PropertyChanged(nameof(ToolTip))]
        void RegisterToolTip(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (toolTipElement)
            {
                toolTipElement.Dispose();
                toolTipElement = null;
            }
            if (newValue != null)
            {
                UIElement element = newValue as UIElement;
                if (element != null)
                {
                    toolTipElement = element;
                }
                else
                {
                    toolTipElement = new Border
                    {
                        Name = "tooltipContent",
                        UseLayoutRounding = true,
                        BorderStroke = new Stroke(1),
                        MarginTop = 1,
                        MarginRight = 1,
                        MarginLeft = 1,
                        MarginBottom = 1,
                        BorderFill = "#aaa",
                        Background = "#fff",
                        Child = new ContentControl { Content = newValue, MarginBottom = 2, MarginLeft = 4, MarginRight = 4, MarginTop = 2 }
                    };
                }
            }
        }

        [PropertyChanged(nameof(Visibility))]
        void RegisterVisibility(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            Invalidate();
            if ((Visibility)newValue == Visibility.Collapsed && Root != null)
            {
                if (IsMouseOver)
                {
                    var e = new MouseEventArgs(Root.InputManager.MouseDevice, this, false, false, false, new Point());
                    Root.InputManager.MouseDevice.MouseLeave(this, e);
                }
                if (IsKeyboardFocusWithin || IsFocused)
                {
                    Root.InputManager.KeyboardDevice.SetFocus(null);
                    InnerLostFocus();
                }
            }
        }




        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            //if (propertyName == nameof(RenderTransform))
            //{
            //    this.VisualTransform = RenderTransform;
            //}
            //else
            //if (propertyName == nameof(ZIndex))
            //{
            //    UIElement parent = Parent;
            //    if (parent != null)
            //    {
            //        parent.children.InvalidateZIndex();
            //    }
            //}
            //else
            //if (propertyName == nameof(Effect))
            //{
            //    effect = newValue as Effects.Effect;
            //}
            //else
            //if (propertyName == nameof(ToolTip))
            //{
            //    if (toolTipElement)
            //    {
            //        toolTipElement.Dispose();
            //        toolTipElement = null;
            //    }
            //    if (newValue != null)
            //    {
            //        UIElement element = newValue as UIElement;
            //        if (element != null)
            //        {
            //            toolTipElement = element;
            //        }
            //        else
            //        {
            //            toolTipElement = new Control { UseLayoutRounding = true, BorderStroke = new Stroke(1), MarginTop = 1, MarginRight = 1, MarginLeft = 1, MarginBottom = 1, BorderFill = "#aaa", Background = "#fff", Children = { new ContentControl { Content = newValue.ToString(), MarginBottom = 2, MarginLeft = 4, MarginRight = 4, MarginTop = 2 } } };
            //        }
            //    }
            //}
            //else
            //if (propertyName == nameof(Visibility))
            //{
            //    if ((Visibility)newValue == Visibility.Collapsed)
            //    {
            //        //IsMouseOver = false;
            //        if (IsMouseOver)
            //        {
            //            var e = new MouseEventArgs(Root.InputManager.MouseDevice, this, false, false, false, new Point(), 0);
            //            //RaiseDeviceEvent(e, EventType.MouseLeave);
            //            Root.InputManager.MouseDevice.MouseLeave(this, e);
            //        }
            //        //foreach (var item in Find<UIElement>().Where(a => a.IsMouseOver))
            //        //{
            //        //    //item.IsMouseOver = false;
            //        //    //item.IsMouseLeftButtonDown = false;
            //        //}
            //        if (IsKeyboardFocusWithin || IsFocused)
            //        {
            //            Root.InputManager.KeyboardDevice.SetFocus(null);
            //            InnerLostFocus();
            //        }
            //    }
            //}
            //if (Name == "more" && propertyName == "MarginLeft")
            //{
            //    Debug.WriteLine(newValue);
            //}
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
            var data = propertyMetadata as UIPropertyMetadataAttribute;
            if (data != null)
            {
                if (data.Inherits)
                {

                    if (!inheritsSet)
                    {
                        for (int i = 0; i < Children.Count; i++)
                        {
                            Inherits(children[i], propertyName, oldValue, newValue);
                        }
                    }
                }
                if (data.AffectsMeasure)
                {
                    InvalidateMeasure();
                    //Parent?.OnChildInvalidateMeasure(this);
                }
                if (data.AffectsArrange)
                {
                    InvalidateArrange();
                    //Parent?.OnChildInvalidateArrange(this);
                }
                if (data.AffectsRender)
                {
                    Invalidate();
                }
                var notify = oldValue as INotifyPropertyChanged;
                if (notify != null)
                {
                    //notify.PropertyChanged -= Notify_PropertyChanged;
                    Binding.CancellationPropertyChanged(notify, Notify_PropertyChanged);
                    if (notifyList != null)
                    {
                        if (notifyList.TryGetValue(notify, out var p))
                        {
                            p.Remove(data);
                            if (p.Count == 0)
                            {
                                notifyList.Remove(notify);
                            }
                        }
                    }
                }
                notify = newValue as INotifyPropertyChanged;
                if (notify != null)
                {
                    //notify.PropertyChanged += Notify_PropertyChanged;
                    Binding.RegisterPropertyChanged(notify, Notify_PropertyChanged);
                    if (notifyList == null)
                    {
                        notifyList = new HybridDictionary<INotifyPropertyChanged, HashSet<UIPropertyMetadataAttribute>>();
                    }
                    if (!notifyList.TryGetValue(notify, out var p))
                    {
                        notifyList.Add(notify, new HashSet<UIPropertyMetadataAttribute> { data });
                    }
                    else
                    {
                        p.Add(data);
                    }
                }
            }

            if (triggers != null)
            {
                var c = triggers.Count;
                for (int i = 0; i < c; i++)
                {
                    if (i >= triggers.Count)
                    {
                        break;
                    }
                    var t = triggers[i];
                    if (t.Property == propertyName)
                    {
                        var set = t.Condition(this);
                        if (t.TargetRelation != null && t.TargetRelation != Relation.Me)
                        {
                            foreach (var item in t.TargetRelation.Query(this))
                            {
                                SetTrigger(t, item, set);
                            }
                        }
                        else
                        {
                            SetTrigger(t, this, set);
                        }
                    }
                }
            }

        }

        protected override void OnAttachedChanged(Type ownerType, string propertyName, object defaultValue, object oldValue, object newValue)
        {
            base.OnAttachedChanged(ownerType, propertyName, defaultValue, oldValue, newValue);
            if (triggers != null)
            {
                var c = triggers.Count;
                for (int i = 0; i < c; i++)
                {
                    if (i >= triggers.Count)
                    {
                        break;
                    }
                    var t = triggers[i];
                    if (t.Property == ownerType.Name + "." + propertyName)
                    {
                        var set = t.Condition(this);
                        if (t.TargetRelation != null && t.TargetRelation != Relation.Me)
                        {
                            foreach (var item in t.TargetRelation.Query(this))
                            {
                                SetTrigger(t, item, set);
                            }
                        }
                        else
                        {
                            SetTrigger(t, this, set);
                        }
                    }
                }
            }
        }

        private void SetTrigger(Trigger t, UIElement target, bool setTriggerValue, bool onAdd = false)
        {
            if (setTriggerValue)
            {
                if (t.Animation != null && Root != null && (!onAdd || PlayAnimationOnAddTrigger))
                {
                    t.Animation.Start(this, target, t.AnimationDuration, t.AnimationIterationCount, t.AnimationEndBehavior, t);
                }
                foreach (var item in t.Setters)
                {
                    if (target.HasProperty(item.Key))
                    {
                        target.SetTriggerValue(item.Key, t, item.Value);
                    }
                }
            }
            else
            {
                //foreach (var item in t.Setters)
                //{
                //    target.ClearTriggerValue(t, item.Key);
                //}
                if (t.Animation != null)
                {
                    t.Animation.Remove(target);
                }
                if (t.SetPropertys != null)
                {
                    if (t.SetPropertys.TryGetValue(target, out List<string> ps))
                    {
                        foreach (var item in ps)
                        {
                            target.ClearTriggerValue(t, item);
                        }
                    }

                    //t.SetPropertys.Clear();
                }
            }
        }

        private void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (notifyList != null)
            {
                if (notifyList.TryGetValue((sender as INotifyPropertyChanged), out var p))
                {
                    bool AffectsMeasure = false;
                    bool AffectsArrange = false;
                    bool AffectsRender = false;
                    foreach (var item in p)
                    {
                        AffectsMeasure = AffectsMeasure || item.AffectsMeasure;
                        AffectsRender = AffectsRender || item.AffectsRender;
                        AffectsArrange = AffectsArrange || item.AffectsArrange;
                    }

                    if (AffectsMeasure)
                    {
                        InvalidateMeasure();
                        //Parent?.OnChildInvalidateMeasure(this);
                    }
                    else if (AffectsArrange)
                    {
                        InvalidateArrange();
                        //Parent?.OnChildInvalidateArrange(this);
                    }
                    else if (AffectsRender)
                    {
                        Invalidate();
                    }
                }
            }
        }

        void Inherits(UIElement element, string propertyName, object oldValue, object newValue)
        {
            if (!element.HasLocalOrStyleValue(propertyName, out var p))
            {
                element.inheritsSet = true;
                //var p = element.GetPropertyMetadata(propertyName);
                if (p != null)
                {
                    if (p is UIPropertyMetadataAttribute ui && ui.Inherits)
                    {
                        element.inheritsValues.Remove(propertyName);
                        element.inheritsValues.Add(propertyName, new InheritsValue { Value = newValue, ValueForm = ValueFrom.Property });
                    }
                    element.OnPropertyChanged(propertyName, oldValue, newValue, p);
                }
                element.inheritsSet = false;
                //if (propertyName != nameof(DataContext) || !element.HasLocalOrStyleValue(propertyName, out p))
                //{
                for (int i = 0; i < element.Children.Count; i++)
                {
                    Inherits(element.children[i], propertyName, oldValue, newValue);
                }
                //}
            }
        }

        internal override void OnSetValue(string propertyName, PropertyMetadataAttribute property, ValueFrom valueForm, object newValue)
        {
            if (property is UIPropertyMetadataAttribute ui && ui.Inherits)
            {
                inheritsValues.Remove(propertyName);
                inheritsValues.Add(propertyName, new InheritsValue { Value = newValue, ValueForm = valueForm });
            }
        }

        internal override void OnClearLocalValue(string propertyName, EffectiveValue value)
        {
            if (inheritsValues.TryGetValue(propertyName, out InheritsValue value1) && value1.ValueForm == ValueFrom.Property)
            {
                inheritsValues.Remove(propertyName);
            }
        }
        internal override void OnClearTriggerValue(string propertyName, EffectiveValue value)
        {
            if (value.TriggerValue == null || value.TriggerValue.Count == 0)
            {
                if (inheritsValues.TryGetValue(propertyName, out InheritsValue value1) && value1.ValueForm == ValueFrom.Trigger)
                {
                    inheritsValues.Remove(propertyName);
                }
            }
        }

        internal override void OnClearAnimationValue(string propertyName, EffectiveValue value)
        {
            if (value.AnimationValue == null || value.AnimationValue.Count == 0)
            {
                if (inheritsValues.TryGetValue(propertyName, out InheritsValue value1) && value1.ValueForm == ValueFrom.Animation)
                {
                    inheritsValues.Remove(propertyName);
                }
            }
        }

        internal override void OnClearStyleValue(string propertyName, EffectiveValue value)
        {
            if (value.styleValues == null || value.styleValues.Count == 0)
            {
                if (inheritsValues.TryGetValue(propertyName, out InheritsValue value1) && value1.ValueForm == ValueFrom.Style)
                {
                    inheritsValues.Remove(propertyName);
                }
            }
        }

        /// <summary>
        /// 获取在布局流程的度量传递过程中此元素计算所得的大小，包含margin。其实就是能包含所有内容的最小尺寸
        /// </summary>
        /// <returns></returns>
        [NotCpfProperty]
        public Size DesiredSize
        {
            get { return desiredSize; }
        }
        /// <summary>
        /// 在派生类中重写时，测量子元素或者内容在布局中所需的大小，availableSize不包含当前对象的Margin和Padding，并确定由 UIElement 派生的类的大小。
        /// </summary>
        /// <param name="availableSize">一般不要使用该属性参与计算</param>
        /// <returns>此元素基于其对子元素大小的计算确定它在布局期间所需要的大小。</returns>
        protected virtual Size MeasureOverride(in Size availableSize)
        {
            Size contentDesiredSize = new Size();

            if (children != null)
            {
                foreach (UIElement item in Children)
                {
                    item.Measure(availableSize);
                    contentDesiredSize.Width = Math.Max(contentDesiredSize.Width, item.DesiredSize.Width);
                    contentDesiredSize.Height = Math.Max(contentDesiredSize.Height, item.DesiredSize.Height);
                }
            }

            return contentDesiredSize;
        }
        /// <summary>
        /// 测量期望尺寸
        /// </summary>
        /// <param name="availableSize">一般不要使用该属性</param>
        public void Measure(in Size availableSize)
        {
            if (!IsMeasureValid || (_previousMeasure != availableSize && (Width.IsAuto || Width.Unit != Unit.Default || Height.IsAuto || Height.Unit != Unit.Default)))
            {
                var previousDesiredSize = desiredSize;


                //try
                //{
                //_measuring = true;
                desiredSize = MeasureCore(availableSize);//.Constrain(availableSize);
                IsMeasureValid = true;
                //Console.WriteLine(this.ToString() + ":" + desiredSize.ToString());
                //}
                //finally
                //{
                //    //_measuring = false;
                //}

                //if (IsInvalidSize(desiredSize))
                //{
                //    throw new InvalidOperationException("Invalid size returned for Measure.");
                //}

                _previousMeasure = availableSize;

                //Logger.Verbose(LogArea.Layout, this, "Measure requested {DesiredSize}", DesiredSize);

                if (DesiredSize != previousDesiredSize)
                {
                    Parent?.OnChildDesiredSizeChanged(this, previousDesiredSize);
                    this.RaiseEvent(EventArgs.Empty, nameof(DesiredSizeChanged));
                }
            }

        }

        internal static float RoundLayoutValue(float value, float dpiScale)
        {
            float newValue;

            // If DPI == 1, don't use DPI-aware rounding.
            if (!FloatUtil.AreClose(dpiScale, 1f))
            {
                newValue = (float)Math.Round(value * dpiScale) / dpiScale;
                // If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
                if (FloatUtil.IsNaN(newValue) ||
                    float.IsInfinity(newValue) ||
                    FloatUtil.AreClose(newValue, float.MaxValue))
                {
                    newValue = value;
                }
            }
            else
            {
                newValue = (float)Math.Round(value);
            }

            return newValue;
        }
        protected virtual void OnChildDesiredSizeChanged(UIElement child, in Size old)
        {
            if (((!Width.IsAuto && Width.Unit != Unit.Percent)) && ((!Height.IsAuto && Height.Unit != Unit.Percent)))
            {
                InvalidateArrange();
            }
            else
            {
                if (IsMeasureValid)
                {
                    InvalidateMeasure();
                }
                else
                {
                    //if (desiredSizeChangedChildren == null)
                    //{
                    //    desiredSizeChangedChildren = new Dictionary<UIElement, Size>();
                    //}
                    //desiredSizeChangedChildren[child] = old;
                }

            }
        }
        //Dictionary<UIElement, Size> desiredSizeChangedChildren;

        static Type panelType = typeof(Panel);
        /// <summary>
        /// 测量期望尺寸
        /// </summary>
        /// <param name="availableSize">相当于父容器能提供的尺寸，如果值为PositiveInfinity，则父容器未定义尺寸依赖子元素的尺寸</param>
        /// <returns>The desired size for the control.</returns>
        protected virtual Size MeasureCore(in Size availableSize)
        {
            if (Visibility != Visibility.Collapsed)
            {
                var l = MarginLeft;
                var t = MarginTop;
                var r = MarginRight;
                var b = MarginBottom;
                var w = Width;
                var h = Height;
                var maxw = MaxWidth;
                var maxh = MaxHeight;
                var minw = MinWidth;
                var minh = MinHeight;

                var constrainedWidth = float.PositiveInfinity;//可以提供的最大尺寸
                var constrainedHeight = float.PositiveInfinity;

                if (!l.IsAuto && !r.IsAuto)
                {
                    constrainedWidth = Math.Max(0, availableSize.Width - l.GetActualValue(availableSize.Width) - r.GetActualValue(availableSize.Width));
                }
                if (!w.IsAuto)
                {
                    //if (w.Unit == Unit.Percent && Parent != null && Parent.Width.IsAuto)
                    //{
                    //    constrainedWidth = float.PositiveInfinity;
                    //}
                    //else
                    //{
                    constrainedWidth = Math.Max(0, w.GetActualValue(availableSize.Width));
                    //}
                }
                if (!float.IsInfinity(constrainedWidth) && !maxw.IsAuto && !float.IsInfinity(availableSize.Width))
                {
                    var max = maxw.GetActualValue(availableSize.Width);
                    if (constrainedWidth > max)
                    {
                        constrainedWidth = max;
                    }
                }
                if (!minw.IsAuto)
                {
                    var min = minw.GetActualValue(availableSize.Width);
                    if (constrainedWidth < min)
                    {
                        constrainedWidth = min;
                    }
                }

                if (!t.IsAuto && !b.IsAuto)
                {
                    constrainedHeight = Math.Max(0, availableSize.Height - t.GetActualValue(availableSize.Height) - b.GetActualValue(availableSize.Height));
                }
                if (!h.IsAuto)
                {
                    //if (h.Unit == Unit.Percent && Parent != null && Parent.Height.IsAuto)
                    //{
                    //    constrainedHeight = float.PositiveInfinity;
                    //}
                    //else
                    //{
                    constrainedHeight = Math.Max(0, h.GetActualValue(availableSize.Height));
                    //}
                }

                if (!float.IsInfinity(constrainedHeight) && !maxh.IsAuto && !float.IsInfinity(availableSize.Height))
                {
                    var max = maxh.GetActualValue(availableSize.Height);
                    if (constrainedHeight > max)
                    {
                        constrainedHeight = max;
                    }
                }
                if (!minh.IsAuto)
                {
                    var min = minh.GetActualValue(availableSize.Height);
                    if (constrainedHeight < min)
                    {
                        constrainedHeight = min;
                    }
                }

                //内容尺寸
                var measured = MeasureOverride(new Size(constrainedWidth, constrainedHeight));
                if ((measured.Height > availableSize.Height && !maxh.IsAuto && float.IsInfinity(constrainedHeight) && !float.IsInfinity(availableSize.Height) && h.IsAuto) ||
                    (measured.Width > availableSize.Width && !maxw.IsAuto && float.IsInfinity(constrainedWidth) && !float.IsInfinity(availableSize.Width) && w.IsAuto))
                {
                    measured = MeasureOverride(new Size(maxw.GetActualValue(availableSize.Width), maxh.GetActualValue(availableSize.Height)));
                }
                //if (desiredSizeChangedChildren != null)
                //{
                //    foreach (var item in desiredSizeChangedChildren)
                //    {
                //        if (item.Value != item.Key.desiredSize)
                //        {
                //            InvalidateMeasure();
                //            break;
                //        }
                //    }
                //    desiredSizeChangedChildren = null;
                //}

                var width = measured.Width;
                var height = measured.Height;

                var wPercent = float.NaN;
                var hPercent = float.NaN;
                if (!w.IsAuto)
                {
                    if (w.Unit == Unit.Default || (!float.IsInfinity(availableSize.Width) && !float.IsNaN(availableSize.Width)))
                    {
                        width = w.GetActualValue(availableSize.Width);
                    }
                    else
                    {
                        wPercent = w.Value;
                    }
                }
                if (!h.IsAuto)
                {
                    if (h.Unit == Unit.Default || (!float.IsInfinity(availableSize.Height) && !float.IsNaN(availableSize.Height)))
                    {
                        height = h.GetActualValue(availableSize.Height);
                    }
                    else
                    {
                        hPercent = h.Value;
                    }
                }
                float pW;//计算如果是百分比布局为100%的时候占用的尺寸
                if (!float.IsNaN(wPercent))
                {
                    pW = width / wPercent;
                }
                else
                {
                    pW = width;
                }
                float pH;
                if (!float.IsNaN(hPercent))
                {
                    pH = height / hPercent;
                }
                else
                {
                    pH = height;
                }


                if (!minw.IsAuto)
                {
                    if (minw.Unit == Unit.Default)
                    {
                        if (width < minw.Value)
                        {
                            width = minw.Value;
                        }
                    }
                    else
                    {
                        if (!float.IsNaN(pW))
                        {
                            var min = pW * minw.Value;
                            if (min > width)
                            {
                                width = min;
                            }
                        }
                    }
                }
                if (!minh.IsAuto)
                {
                    if (minh.Unit == Unit.Default)
                    {
                        if (height < minh.Value)
                        {
                            height = minh.Value;
                        }
                    }
                    else
                    {
                        if (!float.IsNaN(pH))
                        {
                            var min = pH * minh.Value;
                            if (min > height)
                            {
                                height = min;
                            }
                        }
                    }
                }
                if (!maxw.IsAuto)
                {
                    if (maxw.Unit == Unit.Default)
                    {
                        if (width > maxw.Value)
                        {
                            width = maxw.Value;
                        }
                    }
                    else
                    {
                        if (!float.IsNaN(pW))
                        {
                            var max = pW * maxw.Value;
                            if (max < width)
                            {
                                width = max;
                            }
                        }
                    }
                }
                if (!maxh.IsAuto)
                {
                    if (maxh.Unit == Unit.Default)
                    {
                        if (height > maxh.Value)
                        {
                            height = maxh.Value;
                        }
                    }
                    else
                    {
                        if (!float.IsNaN(pH))
                        {
                            var max = pH * maxh.Value;
                            if (max < height)
                            {
                                height = max;
                            }
                        }
                    }
                }

                if (!l.IsAuto)
                {
                    if (l.Unit == Unit.Default)
                    {
                        width += l.Value;
                    }
                    else
                    {
                        if (float.IsNaN(availableSize.Width) || float.IsPositiveInfinity(availableSize.Width))
                        {
                            width += pW * l.Value;
                        }
                        else
                        {
                            width += availableSize.Width * l.Value;
                        }
                    }
                }
                if (!r.IsAuto)
                {
                    if (r.Unit == Unit.Default)
                    {
                        width += r.Value;
                    }
                    else
                    {
                        if (float.IsNaN(availableSize.Width) || float.IsPositiveInfinity(availableSize.Width))
                        {
                            width += pW * r.Value;
                        }
                        else
                        {
                            width += availableSize.Width * r.Value;
                        }
                    }
                }
                if (!t.IsAuto)
                {
                    if (t.Unit == Unit.Default)
                    {
                        height += t.Value;
                    }
                    else
                    {
                        if (float.IsPositiveInfinity(availableSize.Height) || float.IsNaN(availableSize.Height))
                        {
                            height += pH * t.Value;
                        }
                        else
                        {
                            height += availableSize.Height * t.Value;
                        }
                    }
                }
                if (!b.IsAuto)
                {
                    if (b.Unit == Unit.Default)
                    {
                        height += b.Value;
                    }
                    else
                    {
                        if (float.IsPositiveInfinity(availableSize.Height) || float.IsNaN(availableSize.Height))
                        {
                            height += pH * b.Value;
                        }
                        else
                        {
                            height += availableSize.Height * b.Value;
                        }
                    }
                }



                //if (wPercent != 0)
                //{
                //    width = width / wPercent;
                //    width = Math.Max(width, constrainedWidth);
                //}
                //if (hPercent != 0)
                //{
                //    height = height / hPercent;
                //    height = Math.Max(height, constrainedHeight);
                //}

                if (UseLayoutRounding)
                {
                    var scale = Root.RenderScaling;
                    width = (float)Math.Ceiling(width * scale) / scale;
                    height = (float)Math.Ceiling(height * scale) / scale;
                }

                return new Size(width, height);
            }
            else
            {
                return new Size();
            }
        }
        //protected virtual Size MeasureCore(in Size availableSize)
        //{
        //    if (Visibility != Visibility.Collapsed)
        //    {
        //        var l = MarginLeft;
        //        var t = MarginTop;
        //        var r = MarginRight;
        //        var b = MarginBottom;
        //        var w = Width;
        //        var h = Height;
        //        var maxw = MaxWidth;
        //        var maxh = MaxHeight;
        //        var minw = MinWidth;
        //        var minh = MinHeight;

        //        var constrainedWidth = availableSize.Width;//可以提供的最大尺寸
        //        var constrainedHeight = availableSize.Height;

        //        if (!l.IsAuto && !r.IsAuto)
        //        {
        //            constrainedWidth = constrainedWidth - l.GetActualValue(availableSize.Width);
        //            constrainedWidth = constrainedWidth - r.GetActualValue(availableSize.Width);
        //        }
        //        if (!w.IsAuto)
        //        {
        //            constrainedWidth = w.GetActualValue(availableSize.Width);
        //        }
        //        if (!maxw.IsAuto)
        //        {
        //            var max = maxw.GetActualValue(availableSize.Width);
        //            if (constrainedWidth > max)
        //            {
        //                constrainedWidth = max;
        //            }
        //        }
        //        if (!minw.IsAuto)
        //        {
        //            var min = minw.GetActualValue(availableSize.Width);
        //            if (constrainedWidth < min)
        //            {
        //                constrainedWidth = min;
        //            }
        //        }

        //        if (!t.IsAuto && !b.IsAuto)
        //        {
        //            constrainedHeight = constrainedHeight - t.GetActualValue(availableSize.Height);
        //            //}
        //            //if ()
        //            //{
        //            constrainedHeight = constrainedHeight - b.GetActualValue(availableSize.Height);
        //        }
        //        if (!h.IsAuto)
        //        {
        //            constrainedHeight = h.GetActualValue(availableSize.Height);
        //        }

        //        if (!maxh.IsAuto)
        //        {
        //            var max = maxh.GetActualValue(availableSize.Height);
        //            if (constrainedHeight > max)
        //            {
        //                constrainedHeight = max;
        //            }
        //        }
        //        if (!minh.IsAuto)
        //        {
        //            var min = minh.GetActualValue(availableSize.Height);
        //            if (constrainedHeight < min)
        //            {
        //                constrainedHeight = min;
        //            }
        //        }

        //        //var margin = new Thickness();
        //        //if (!IsRoot)
        //        //{
        //        //    margin = new Thickness(l.GetActualValue(availableSize.Width), t.GetActualValue(availableSize.Height), r.GetActualValue(availableSize.Width), b.GetActualValue(availableSize.Height));
        //        //}

        //        //.Deflate(margin);
        //        //var padding = new Thickness(pl.GetActualValue(availableSize.Width), pt.GetActualValue(availableSize.Height), pr.GetActualValue(availableSize.Width), pb.GetActualValue(availableSize.Height));

        //        //内容尺寸
        //        var measured = MeasureOverride(new Size(constrainedWidth, constrainedHeight));
        //        var width = measured.Width;
        //        var height = measured.Height;



        //        if (!w.IsAuto)
        //        {
        //            width = w.GetActualValue(availableSize.Width);
        //        }
        //        if (!h.IsAuto)
        //        {
        //            height = h.GetActualValue(availableSize.Height);
        //        }
        //        if (!minw.IsAuto)
        //        {
        //            var min = minw.GetActualValue(availableSize.Width);
        //            if (width < min)
        //            {
        //                width = min;
        //            }
        //        }
        //        if (!minh.IsAuto)
        //        {
        //            var min = minh.GetActualValue(availableSize.Height);
        //            if (height < min)
        //            {
        //                height = min;
        //            }
        //        }
        //        if (!maxw.IsAuto)
        //        {
        //            var max = maxw.GetActualValue(availableSize.Width);
        //            if (width > max)
        //            {
        //                width = max;
        //            }
        //        }
        //        if (!maxh.IsAuto)
        //        {
        //            var max = maxh.GetActualValue(availableSize.Height);
        //            if (height > max)
        //            {
        //                height = max;
        //            }
        //        }

        //        if (!l.IsAuto)
        //        {
        //            width += l.GetActualValue(availableSize.Width);
        //        }
        //        if (!r.IsAuto)
        //        {
        //            width += r.GetActualValue(availableSize.Width);
        //        }
        //        if (!t.IsAuto)
        //        {
        //            height += t.GetActualValue(availableSize.Height);
        //        }
        //        if (!b.IsAuto)
        //        {
        //            height += b.GetActualValue(availableSize.Height);
        //        }


        //        if (UseLayoutRounding)
        //        {
        //            var scale = Root.Scaling;
        //            width = (float)Math.Ceiling(width * scale) / scale;
        //            height = (float)Math.Ceiling(height * scale) / scale;
        //        }

        //        return new Size(Math.Max(0, width), Math.Max(0, height));
        //    }
        //    else
        //    {
        //        return new Size();
        //    }
        //}
        /// <summary>
        /// Arranges the control and its children.
        /// </summary>
        /// <param name="rect">The control's new bounds.</param>
        public void Arrange(Rect rect)
        {
            Root.LayoutManager._toArrange.Remove(this);
            if (IsInvalidRect(rect))
            {
                throw new InvalidOperationException("Invalid Arrange rectangle.");
            }

            if (!IsMeasureValid)
            {
                Measure(_previousMeasure ?? rect.Size);
            }

            if (!IsArrangeValid || _previousArrange != rect)
            {
                IsArrangeValid = true;
                _previousArrange = rect;

                if (Visibility != Visibility.Collapsed)
                {
                    ArrangeCore(rect);
                    contentBounds = new Rect(VisualOffset, ActualSize);
                }
                else
                {
                    renderBounds = new Rect();
                }

                OnLayoutUpdated();
                if (Parent != null && Parent.IsRoot)
                {
                    Parent.viewRenderRect = true;
                }
            }
        }

        /// <summary>
        /// 获取在View上对应的剪辑区域
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rect GetHostClipBounds(Rect rect)
        {
            UIElement parent = this.Parent;
            var bounds = this.GetClipBounds(rect);
            while (parent != null)
            {
                bounds = parent.GetClipBounds(bounds);
                parent = parent.Parent;
            }
            return bounds;
        }
        [NotCpfProperty]
        bool transform { set { SetFlag(CoreFlags.transform, value); } get { return GetFlag(CoreFlags.transform); } }
        internal void RendRect()
        {
            previousRenderRect = renderBounds;
            UIElement parent = this.Parent;
            renderBounds = this.GetClipBounds();
            if (parent != null)
            {
                transform = parent.transform || VisualTransform != Transform.Identity;
            }
            else
            {
                transform = VisualTransform != Transform.Identity;
            }
            if (transform)
            {
                while (parent != null)
                {
                    renderBounds = parent.GetClipBounds(renderBounds);
                    parent = parent.Parent;
                }
            }
            else
            {
                if (parent != null)
                {
                    renderBounds = new Rect(renderBounds.X + parent.renderBounds.X, renderBounds.Y + parent.renderBounds.Y, renderBounds.Width, renderBounds.Height);
                }
            }
            if (effect != null)
            {
                renderBounds = effect.OverrideRenderRect(renderBounds);
            }
            var rect = previousRenderRect;
            rect.Union(renderBounds);
            Root.Invalidate(rect);
        }
        /// <summary>
        /// 获取该元素相对Root的最终变换矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix GetMatrixToRoot()
        {
            if (IsRoot || Root == null)
            {
                return Matrix.Identity;
            }
            List<UIElement> elements = new List<UIElement>();
            elements.Add(this);
            var parent = Parent;
            while (parent != null && !parent.IsRoot)
            {
                elements.Add(parent);
                parent = parent.Parent;
            }
            elements.Reverse();
            var mat = Matrix.Identity;
            foreach (var item in elements)
            {
                Rect rect = item.GetContentBounds();
                var op = item.RenderTransformOrigin;
                mat.TranslatePrepend(rect.X + op.X.GetActualValue(rect.Width), rect.Y + op.Y.GetActualValue(rect.Height));
                mat.Prepend(item.RenderTransform.Value);
                mat.TranslatePrepend(-op.X.GetActualValue(rect.Width), -op.Y.GetActualValue(rect.Height));
            }
            return mat;
        }
        /// <summary>
        /// 获取该元素相对Parent的变换矩阵
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Matrix GetMatrixToParent(UIElement element)
        {
            if (IsRoot || Root == null || element.Root != Root)
            {
                return Matrix.Identity;
            }
            List<UIElement> elements = new List<UIElement>();
            elements.Add(this);
            var parent = Parent;
            while (parent != null && !parent.IsRoot && element != parent)
            {
                elements.Add(parent);
                parent = parent.Parent;
            }
            elements.Reverse();
            var mat = Matrix.Identity;
            foreach (var item in elements)
            {
                Rect rect = item.GetContentBounds();
                var op = item.RenderTransformOrigin;
                mat.TranslatePrepend(rect.X + op.X.GetActualValue(rect.Width), rect.Y + op.Y.GetActualValue(rect.Height));
                mat.Prepend(item.RenderTransform.Value);
                mat.TranslatePrepend(-op.X.GetActualValue(rect.Width), -op.Y.GetActualValue(rect.Height));
            }
            return mat;
        }

        /// <summary>
        /// The default implementation of the control's arrange pass.
        /// </summary>
        /// <param name="finalRect">The control's new bounds.</param>
        /// <remarks>
        /// This method calls <see cref="ArrangeOverride(in Size)"/> which is probably the method you
        /// want to override in order to modify a control's arrangement.
        /// </remarks>
        protected virtual void ArrangeCore(in Rect finalRect)
        {
            var l = MarginLeft;
            var t = MarginTop;
            var r = MarginRight;
            var b = MarginBottom;
            var w = Width;
            var h = Height;
            var maxw = MaxWidth;
            var maxh = MaxHeight;
            var minw = MinWidth;
            var minh = MinHeight;

            var scale = Root.RenderScaling;

            float constrainedWidth;
            float constrainedHeight;
            bool isPanel = true;
            var p = Parent;
            if (p == null || !(p is Panel) || p.Type == panelType)
            {//普通元素布局
                isPanel = false;
                if (!l.IsAuto && !r.IsAuto)
                {
                    constrainedWidth = finalRect.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width);
                }
                else
                {
                    constrainedWidth = desiredSize.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width);
                }
                if (!w.IsAuto)
                {
                    constrainedWidth = w.GetActualValue(finalRect.Width);
                }
                if (!maxw.IsAuto)
                {
                    var max = maxw.GetActualValue(finalRect.Width);
                    if (constrainedWidth > max)
                    {
                        constrainedWidth = max;
                    }
                }
                if (!minw.IsAuto)
                {
                    var min = minw.GetActualValue(finalRect.Width);
                    if (constrainedWidth < min)
                    {
                        constrainedWidth = min;
                    }
                }

                if (!t.IsAuto && !b.IsAuto)
                {
                    constrainedHeight = finalRect.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height);
                }
                else
                {
                    constrainedHeight = desiredSize.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height);
                }
                if (!h.IsAuto)
                {
                    constrainedHeight = h.GetActualValue(finalRect.Height);
                }

                if (!maxh.IsAuto)
                {
                    var max = maxh.GetActualValue(finalRect.Height);
                    if (constrainedHeight > max)
                    {
                        constrainedHeight = max;
                    }
                }
                if (!minh.IsAuto)
                {
                    var min = minh.GetActualValue(finalRect.Height);
                    if (constrainedHeight < min)
                    {
                        constrainedHeight = min;
                    }
                }
            }
            else
            {//panel的继承的布局容器布局
                //if (this is TextBlock textBlock && textBlock.Text == "跨列")
                //{

                //}
                if (!w.IsAuto)
                {
                    constrainedWidth = w.GetActualValue(finalRect.Width);
                }
                else
                {
                    if (!l.IsAuto && !r.IsAuto)
                    {
                        constrainedWidth = finalRect.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width);
                    }
                    else
                    {
                        constrainedWidth = desiredSize.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width);
                    }
                }
                if (!w.IsAuto)
                {
                    constrainedWidth = w.GetActualValue(finalRect.Width);
                }
                if (!maxw.IsAuto)
                {
                    var max = maxw.GetActualValue(finalRect.Width);
                    if (constrainedWidth > max)
                    {
                        constrainedWidth = max;
                    }
                }
                if (!minw.IsAuto)
                {
                    var min = minw.GetActualValue(finalRect.Width);
                    if (constrainedWidth < min)
                    {
                        constrainedWidth = min;
                    }
                }
                if (!h.IsAuto)
                {
                    constrainedHeight = h.GetActualValue(finalRect.Height);
                }
                else
                {
                    if (!t.IsAuto && !b.IsAuto)
                    {
                        constrainedHeight = finalRect.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height);
                    }
                    else
                    {
                        constrainedHeight = desiredSize.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height);
                    }
                }
                if (!h.IsAuto)
                {
                    constrainedHeight = h.GetActualValue(finalRect.Height);
                }

                if (!maxh.IsAuto)
                {
                    var max = maxh.GetActualValue(finalRect.Height);
                    if (constrainedHeight > max)
                    {
                        constrainedHeight = max;
                    }
                }
                if (!minh.IsAuto)
                {
                    var min = minh.GetActualValue(finalRect.Height);
                    if (constrainedHeight < min)
                    {
                        constrainedHeight = min;
                    }
                }
            }
            //var s = _previousArrangeResultSize;
            //if (_previousArrangeFinalSize != size)
            //{
            var size = new Size(constrainedWidth, constrainedHeight);
            size = ArrangeOverride(size);
            //    _previousArrangeResultSize = s;
            //    _previousArrangeFinalSize = size;
            //}
            if (!l.IsAuto && !r.IsAuto)
            {
                size.Width = finalRect.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width);
            }
            if (!t.IsAuto && !b.IsAuto)
            {
                size.Height = finalRect.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height);
            }

            if (!w.IsAuto)
            {
                size.Width = Math.Max(0, w.GetActualValue(finalRect.Width));
            }
            if (!h.IsAuto)
            {
                size.Height = Math.Max(0, h.GetActualValue(finalRect.Height));
            }

            if (!minw.IsAuto)
            {
                size.Width = Math.Max(size.Width, minw.GetActualValue(finalRect.Width));
            }
            if (!minh.IsAuto)
            {
                size.Height = Math.Max(size.Height, minh.GetActualValue(finalRect.Height));
            }
            if (!maxw.IsAuto)
            {
                size.Width = Math.Min(size.Width, maxw.GetActualValue(finalRect.Width));
            }
            if (!maxh.IsAuto)
            {
                size.Height = Math.Min(size.Height, maxh.GetActualValue(finalRect.Height));
            }


            var originX = finalRect.X + l.GetActualValue(finalRect.Width);
            var originY = finalRect.Y + t.GetActualValue(finalRect.Height);
            if (l.IsAuto && !r.IsAuto)
            {
                originX = finalRect.Right - size.Width - r.GetActualValue(finalRect.Width);
            }
            else if (l.IsAuto && r.IsAuto)
            {
                originX = finalRect.Left + (finalRect.Width - size.Width) / 2;
                if (isPanel && size.Width > finalRect.Width)
                {
                    originX = finalRect.X;
                }
            }
            if (t.IsAuto && !b.IsAuto)
            {
                originY = finalRect.Bottom - size.Height - b.GetActualValue(finalRect.Height);
            }
            else if (t.IsAuto && b.IsAuto)
            {
                originY = finalRect.Top + (finalRect.Height - size.Height) / 2;
                if (isPanel && size.Height > finalRect.Height)
                {
                    originY = finalRect.Y;
                }
            }
            if (UseLayoutRounding)
            {
                originX = (float)Math.Floor(originX * scale) / scale;
                originY = (float)Math.Floor(originY * scale) / scale;
                size.Width = (float)Math.Ceiling(size.Width * scale) / scale;
                size.Height = (float)Math.Ceiling(size.Height * scale) / scale;
            }

            var offset = new Point(originX, originY);
            VisualOffset = offset;
            if (ActualSize != size)
            {
                ActualSize = size;
                viewRenderRect = true;
            }
        }
        [NotCpfProperty]
        internal bool viewRenderRect { get { return GetFlag(CoreFlags.viewRenderRect); } set { SetFlag(CoreFlags.viewRenderRect, value); } }
        //protected virtual void ArrangeCore(in Rect finalRect)
        //{
        //    var l = MarginLeft;
        //    var t = MarginTop;
        //    var r = MarginRight;
        //    var b = MarginBottom;
        //    var w = Width;
        //    var h = Height;
        //    var maxw = MaxWidth;
        //    var maxh = MaxHeight;
        //    var minw = MinWidth;
        //    var minh = MinHeight;
        //    var margin = new Thickness(
        //        l.GetActualValue(finalRect.Width),
        //        t.GetActualValue(finalRect.Height),
        //        r.GetActualValue(finalRect.Width),
        //        b.GetActualValue(finalRect.Height));
        //    //var originX = finalRect.X + margin.Left;
        //    //var originY = finalRect.Y + margin.Top;
        //    //var availableSizeMinusMargins = new Size(
        //    //    Math.Max(0, finalRect.Width - margin.Left - margin.Right),
        //    //    Math.Max(0, finalRect.Height - margin.Top - margin.Bottom));

        //    //var size = availableSizeMinusMargins;
        //    var scale = Root.Scaling;

        //    //if (Name == "2号")
        //    //{

        //    //}

        //    var size = DesiredSize.Deflate(margin);
        //    if (!l.IsAuto && !r.IsAuto)
        //    {
        //        size.Width = Math.Max(finalRect.Width - l.GetActualValue(finalRect.Width) - r.GetActualValue(finalRect.Width), 0);
        //    }
        //    if (!t.IsAuto && !b.IsAuto)
        //    {
        //        size.Height = Math.Max(finalRect.Height - t.GetActualValue(finalRect.Height) - b.GetActualValue(finalRect.Height), 0);
        //    }
        //    if (!w.IsAuto)
        //    {
        //        size.Width = Math.Max(0, w.GetActualValue(finalRect.Width));
        //    }
        //    if (!h.IsAuto)
        //    {
        //        size.Height = Math.Max(0, h.GetActualValue(finalRect.Height));
        //    }
        //    if (!minw.IsAuto)
        //    {
        //        size.Width = Math.Max(size.Width, minw.GetActualValue(finalRect.Width));
        //    }
        //    if (!minh.IsAuto)
        //    {
        //        size.Height = Math.Max(size.Height, minh.GetActualValue(finalRect.Height));
        //    }
        //    if (!maxw.IsAuto)
        //    {
        //        size.Width = Math.Min(size.Width, maxw.GetActualValue(finalRect.Width));
        //    }
        //    if (!maxh.IsAuto)
        //    {
        //        size.Height = Math.Min(size.Height, maxh.GetActualValue(finalRect.Height));
        //    }

        //    //var s = _previousArrangeResultSize;
        //    //if (_previousArrangeFinalSize != size)
        //    //{
        //    var s = ArrangeOverride(size);
        //    //    _previousArrangeResultSize = s;
        //    //    _previousArrangeFinalSize = size;
        //    //}

        //    size = new Size(Math.Max(s.Width, size.Width), Math.Max(s.Height, size.Height));

        //    if (!minw.IsAuto)
        //    {
        //        size.Width = Math.Max(size.Width, minw.GetActualValue(finalRect.Width));
        //    }
        //    if (!minh.IsAuto)
        //    {
        //        size.Height = Math.Max(size.Height, minh.GetActualValue(finalRect.Height));
        //    }
        //    if (!maxw.IsAuto)
        //    {
        //        size.Width = Math.Min(size.Width, maxw.GetActualValue(finalRect.Width));
        //    }
        //    if (!maxh.IsAuto)
        //    {
        //        size.Height = Math.Min(size.Height, maxh.GetActualValue(finalRect.Height));
        //    }


        //    var originX = finalRect.X + l.GetActualValue(finalRect.Width);
        //    var originY = finalRect.Y + t.GetActualValue(finalRect.Height);
        //    if (l.IsAuto && !r.IsAuto)
        //    {
        //        originX = finalRect.Right - size.Width - r.GetActualValue(finalRect.Width);
        //    }
        //    else if (l.IsAuto && r.IsAuto)
        //    {
        //        originX = finalRect.Left + (finalRect.Width - size.Width) / 2;
        //    }
        //    if (t.IsAuto && !b.IsAuto)
        //    {
        //        originY = finalRect.Bottom - size.Height - b.GetActualValue(finalRect.Height);
        //    }
        //    else if (t.IsAuto && b.IsAuto)
        //    {
        //        originY = finalRect.Top + (finalRect.Height - size.Height) / 2;
        //    }
        //    if (UseLayoutRounding)
        //    {
        //        originX = (float)Math.Floor(originX * scale) / scale;
        //        originY = (float)Math.Floor(originY * scale) / scale;
        //        size.Width = (float)Math.Ceiling(size.Width * scale) / scale;
        //        size.Height = (float)Math.Ceiling(size.Height * scale) / scale;
        //    }

        //    //var previousSize = ActualSize;
        //    //var previousOffset = VisualOffset;
        //    //var bounds = GetHostClipBounds(new Rect(new Point(), previousSize));
        //    //if (previousSize != size || previousOffset != new Point(originX, originY))
        //    //{
        //    var offset = new Point(originX, originY);
        //    VisualOffset = offset;
        //    ActualSize = size;
        //    //    if (children != null)
        //    //    {
        //    //        foreach (UIElement item in children)
        //    //        {
        //    //            var rect = item.previousRenderRect;
        //    //            rect.Union(item.RenderBounds);
        //    //            bounds.Union(rect);
        //    //            //Host.Invalidate(rect);
        //    //        }
        //    //    }
        //    //    bounds.Union(GetHostClipBounds(new Rect(new Point(), size)));
        //    //    Host.Invalidate(bounds);
        //    //}

        //    //var p = Parent;
        //    //if (p != null)
        //    //{
        //    //    p.Invalidate();
        //    //}
        //}
        /// <summary>
        /// Positions child elements as part of a layout pass.
        /// </summary>
        /// <param name="finalSize">The size available to the control.</param>
        /// <returns>The actual size used.</returns>
        protected virtual Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            foreach (UIElement child in Children)
            {
                child.Arrange(rect);
            }

            return finalSize;
        }
        ///// <summary>
        ///// 去掉Padding
        ///// </summary>
        ///// <param name="finalSize"></param>
        ///// <returns></returns>
        //protected Rect GetArrangeRect(Size finalSize)
        //{
        //    //var rect = new Rect(finalSize);
        //    return new Rect(0, 0, finalSize.Width, finalSize.Height);
        //}

        /// <summary>
        /// Invalidates the measurement of the control and queues a new layout pass.测量
        /// </summary>
        public void InvalidateMeasure()
        {
            if (IsMeasureValid && !IsDisposed)
            {
                //Logger.Verbose(LogArea.Layout, this, "Invalidated measure");

                IsMeasureValid = false;
                IsArrangeValid = false;
                Root?.InvalidateMeasure(this);
            }
        }
        /// <summary>
        /// Invalidates the arrangement of the control and queues a new layout pass.布局
        /// </summary>
        public void InvalidateArrange()
        {
            if (IsArrangeValid && !IsDisposed)
            {
                IsArrangeValid = false;
                //LayoutManager.Instance?.InvalidateArrange(this);
                Root?.InvalidateArrange(this);
                //var parent = Parent;
                //if (parent != null)
                //{
                //    parent.Invalidate();
                //}
            }
        }

        /// <summary>
        /// Tests whether any of a <see cref="Rect"/>'s properties incude nagative values,
        /// a NaN or Infinity.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>True if the rect is invalid; otherwise false.</returns>
        private static bool IsInvalidRect(Rect rect)
        {
            return rect.Width < 0 || rect.Height < 0 ||
                float.IsInfinity(rect.X) || float.IsInfinity(rect.Y) ||
                float.IsInfinity(rect.Width) || float.IsInfinity(rect.Height) ||
                float.IsNaN(rect.X) || float.IsNaN(rect.Y) ||
                float.IsNaN(rect.Width) || float.IsNaN(rect.Height);
        }

        /// <summary>
        /// 坐标转换，将父级坐标通过逆向变换为自己内部坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual Point TransformPointInvert(Point point)
        {
            Matrix m = VisualTransform.Value;
            var op = RenderTransformOrigin;
            Rect rect = GetContentBounds();
            Point p = new Point(point.X - op.X.GetActualValue(rect.Width) - rect.X, point.Y - op.Y.GetActualValue(rect.Height) - rect.Y);
            m.TranslatePrepend(-rect.X, -rect.Y);
            m.Invert();
            m.Translate(op.X.GetActualValue(rect.Width), op.Y.GetActualValue(rect.Height));
            return m.Transform(p);
        }
        /// <summary>
        /// 坐标转换，将自己内部坐标转换成父级坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual Point TransformPoint(Point point)
        {
            Rect rect = this.GetContentBounds();
            Matrix m = Matrix.Identity;
            var op = this.RenderTransformOrigin;
            m.TranslatePrepend(rect.X + op.X.GetActualValue(rect.Width), rect.Y + op.Y.GetActualValue(rect.Height));
            m.Prepend(this.VisualTransform.Value);
            m.TranslatePrepend(-op.X.GetActualValue(rect.Width), -op.Y.GetActualValue(rect.Height));
            return m.Transform(point);
        }

        private Point TransformPoint(in Point point, in Rect rect, in PointField op)
        {
            Matrix m = Matrix.Identity;
            m.TranslatePrepend(rect.X + op.X.GetActualValue(rect.Width), rect.Y + op.Y.GetActualValue(rect.Height));
            m.Prepend(this.VisualTransform.Value);
            m.TranslatePrepend(-op.X.GetActualValue(rect.Width), -op.Y.GetActualValue(rect.Height));
            return m.Transform(point);
        }
        /// <summary>
        /// 用自己的内部坐标测试内部元素，从最深的后代元素到根（自己）
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<UIElement> HitTest(Point point)
        {
            List<UIElement> list = new List<UIElement>();
            if (HitTest(point, this, list))
            {
                list.Add(this);
            }
            else
            {
                if (HitTestCore(new Point(point.X + ActualOffset.X, point.Y + ActualOffset.Y)))
                {
                    list.Add(this);
                }
            }
            return list;
        }

        bool HitTest(Point point, UIElement element, List<UIElement> list)
        {
            bool isHit = false;
            for (int i = element.Children.Count - 1; i >= 0; i--)
            {
                var item = element.Children[i];
                if (item.Visibility != Visibility.Collapsed)
                {
                    Point l = item.ActualOffset;
                    Point r = item.TransformPointInvert(point);
                    Point t = new Point(r.X - l.X, r.Y - l.Y);

                    isHit = item.HitTestCore(r);
                    if (item.ClipToBounds)//如果是裁剪了，需要先判断自己，再判断子元素
                    {
                        if (isHit)
                        {
                            HitTest(t, item, list);
                        }
                    }
                    else
                    {
                        isHit = (HitTest(t, item, list) || isHit);
                    }
                    if (isHit)
                    {
                        list.Add(item);
                    }
                }
            }
            return isHit;
        }


        public override void Render(DrawingContext dc)
        {
            base.Render(dc);
            if (Site != null)
            {
                if (Site.GetType().GetProperty("ShowBorder") == null || (bool)Site.GetPropretyValue("ShowBorder"))
                {
                    var s = ActualSize;
                    s.Width = Math.Max(1, s.Width);
                    s.Height = Math.Max(1, s.Height);
                    ViewFill fill = Color.FromRgb(80, 80, 80);
                    using (var sb = fill.CreateBrush(new Rect(0, 0, s.Width, s.Height), Root.RenderScaling))
                    {
                        var dpi = Root.RenderScaling;
                        var str = new Stroke(1);
                        str.DashStyle = DashStyles.Dash;
                        str.Width = Math.Max(0.1f, (float)Math.Round(dpi * str.Width) / dpi);
                        dc.DrawRectangle(sb, str, new Rect(str.Width / 2, str.Width / 2, s.Width - str.Width, s.Height - str.Width));
                    }
                }
            }
            var fm = FocusMethod;
            if (fm == NavigationMethod.Directional || fm == NavigationMethod.Tab)
            {
                var s = ActualSize;
                var f = FocusFrameFill;
                var padding = FocusFramePadding;
                if (s.Width > padding.Horizontal && s.Height > padding.Vertical && f != null)
                {
                    using (var sb = f.CreateBrush(new Rect(0, 0, s.Width, s.Height), Root.RenderScaling))
                    {
                        var dpi = Root.RenderScaling;
                        var str = FocusFrameStroke;
                        str.Width = (float)Math.Round(dpi * str.Width) / dpi;
                        dc.DrawRectangle(sb, str, new Rect(padding.Left, padding.Top, s.Width - padding.Horizontal, s.Height - padding.Vertical));
                    }
                }
            }
        }
        /// <summary>
        /// 获取相对于父级的正矩形剪辑区域
        /// </summary>
        /// <returns></returns>
        public Rect GetClipBounds()
        {
            Rect r = GetContentBounds();
            if (VisualTransform == Transform.Identity)
            {
                return r;
            }
            Point lt = new Point();
            Point tr = new Point(r.Width, 0);
            Point lb = new Point(0, r.Height);
            Point rb = new Point(r.Width, r.Height);
            var op = RenderTransformOrigin;
            //Rect r = GetContentBounds();
            lt = TransformPoint(lt, r, op);
            tr = TransformPoint(tr, r, op);
            lb = TransformPoint(lb, r, op);
            rb = TransformPoint(rb, r, op);

            float minX = Math.Min(Math.Min(Math.Min(lt.X, tr.X), lb.X), rb.X);
            float minY = Math.Min(Math.Min(Math.Min(lt.Y, tr.Y), lb.Y), rb.Y);
            float maxX = Math.Max(Math.Max(Math.Max(lt.X, tr.X), lb.X), rb.X);
            float maxY = Math.Max(Math.Max(Math.Max(lt.Y, tr.Y), lb.Y), rb.Y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
        /// <summary>
        /// 获取内部相对于父级的正矩形剪辑区域
        /// </summary>
        /// <returns></returns>
        public Rect GetClipBounds(in Rect rect)
        {
            Rect r = GetContentBounds();
            if (VisualTransform == Transform.Identity)
            {
                return new Rect(r.X + rect.X, rect.Y + r.Y, rect.Width, rect.Height);
            }
            Point lt = new Point(rect.Left, rect.Top);
            Point tr = new Point(rect.Left + rect.Width, rect.Top);
            Point lb = new Point(rect.Left, rect.Top + rect.Height);
            Point rb = new Point(rect.Left + rect.Width, rect.Top + rect.Height);
            var op = RenderTransformOrigin;
            lt = TransformPoint(lt, r, op);
            tr = TransformPoint(tr, r, op);
            lb = TransformPoint(lb, r, op);
            rb = TransformPoint(rb, r, op);

            float minX = Math.Min(Math.Min(Math.Min(lt.X, tr.X), lb.X), rb.X);
            float minY = Math.Min(Math.Min(Math.Min(lt.Y, tr.Y), lb.Y), rb.Y);
            float maxX = Math.Max(Math.Max(Math.Max(lt.X, tr.X), lb.X), rb.X);
            float maxY = Math.Max(Math.Max(Math.Max(lt.Y, tr.Y), lb.Y), rb.Y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public override Rect GetContentBounds()
        {
            if (IsRoot)
            {
                //Size s = Root.Size;
                return new Rect(new Point(), Root.ActualSize);
            }
            //return new Rect(VisualOffset, new Size(Width, Height));
            return contentBounds;
            //return new Rect(VisualOffset, ActualSize);
        }
        //private float GetLayoutScale()
        //{
        //    return Root.Scaling;
        //}

        //bool drawFocusRect = false;
        //private bool DrawFocusRect
        //{
        //    get { return drawFocusRect; }
        //    set
        //    {
        //        if (drawFocusRect != value)
        //        {
        //            drawFocusRect = value;
        //            Invalidate();
        //        }
        //    }
        //}
        public void Invoke(Action action)
        {
            if (!this.IsDisposed)
            {
                Threading.Dispatcher.MainThread.Invoke(action);
            }
        }
        //public void Invoke(SendOrPostCallback action, object data)
        //{
        //    if (!this.IsDisposed)
        //    {
        //        Threading.Dispatcher.MainThread.Invoke(action, data);
        //    }
        //}

        public void BeginInvoke(Action action)
        {
            if (!this.IsDisposed)
            {
                Threading.Dispatcher.MainThread.BeginInvoke(action);
            }
        }
        public void BeginInvoke(SendOrPostCallback action, object data)
        {
            if (!this.IsDisposed)
            {
                Threading.Dispatcher.MainThread.BeginInvoke(action, data);
            }
        }

        public bool Focus(NavigationMethod m)
        {
            FocusMethod = m;
            IsFocused = true;
            //System.Diagnostics.Debug.WriteLine(this);
            var host = Root;
            if (Visibility != Visibility.Visible || !IsEnabled || host == null || !host.CanActivate)
            {
                return false;
            }

            host.InputManager.KeyboardDevice.SetFocus(this);
            IsKeyboardFocused = true;
            IsKeyboardFocusWithin = true;
            //if (m == NavigationMethod.Tab || m == NavigationMethod.Directional)
            //{
            //}
            var e = new GotFocusEventArgs(this) { NavigationMethod = m };
            OnGotFocus(e);
            InnerFocus(e);
            return true;
        }
        public bool Focus()
        {
            return Focus(NavigationMethod.Unspecified);
        }

        private void InnerFocus(GotFocusEventArgs e)
        {
            if (e.OriginalSource != this)
            {
                IsKeyboardFocusWithin = true;
                IsKeyboardFocused = false;
                IsFocused = false;
                OnGotFocus(e);
            }
            //else
            //{
            //    IsFocused = true;
            //    IsKeyboardFocused = true;
            //}
            var p = Parent;
            if (p != null)
            {
                if (!e.Handled)
                {
                    p.InnerFocus(e);
                }
                foreach (UIElement item in p.Children)
                {
                    if (item != this)
                    {//同级的其他元素失去焦点
                        item.InnerLostFocus();
                    }
                }
            }
        }

        internal void InnerLostFocus()
        {
            if (!IsDisposed && IsKeyboardFocusWithin)
            {
                IsKeyboardFocused = false;
                IsKeyboardFocusWithin = false;
                IsFocused = false;
                FocusMethod = null;
                OnLostFocus(new RoutedEventArgs(this));
                foreach (UIElement item in Children)
                {//子元素都失去焦点
                    item.InnerLostFocus();
                }
            }
        }

        UIElementCollection children;
        /// <summary>
        /// 子级，一般自定义组件的时候使用
        /// </summary>
        [NotCpfProperty]
        internal protected virtual UIElementCollection Children
        {
            get
            {
                if (children == null)
                {
                    children = new UIElementCollection(this);
                }
                return children;
            }
        }
        /// <summary>
        /// 获取子元素
        /// </summary>
        /// <returns></returns>

#if Net4
        public IList<UIElement> GetChildren()
#else
        public IReadOnlyList<UIElement> GetChildren()
#endif
        {
            return Children;
        }
        /// <summary>
        /// 当添加触发器时并且触发器有设置动画，如果满足条件是否播放动画
        /// </summary>
        [Description("当添加触发器时并且触发器有设置动画，如果满足条件是否播放动画")]
        public bool PlayAnimationOnAddTrigger
        {
            get { return GetValue<bool>(35); }
            set { SetValue(value, 35); }
        }

        internal Triggers triggers;
        /// <summary>
        /// 触发器集合
        /// </summary>
        [NotCpfProperty]
        [Category("设计")]
        [Description("设置触发器")]
        public Triggers Triggers
        {
            get
            {
                if (triggers == null)
                {
                    triggers = new Triggers();
                    triggers.CollectionChanged += (s, e) =>
                    {
                        Trigger n = e.NewItem;
                        var o = e.OldItem;
                        switch (e.Action)
                        {
                            case CollectionChangedAction.Add:
                                if (!string.IsNullOrEmpty(n.Property) && n.Condition(this))
                                {
                                    if (n.TargetRelation != null && n.TargetRelation != Relation.Me)
                                    {
                                        foreach (var item in n.TargetRelation.Query(this))
                                        {
                                            SetTrigger(n, item, true, true);
                                        }
                                    }
                                    else
                                    {
                                        SetTrigger(n, this, true, true);
                                    }
                                }
                                break;
                            case CollectionChangedAction.Remove:
                                //foreach (var setter in e.Item.Setters)
                                //{
                                //    ClearTriggerValue(e.Item, setter.Key);
                                //}
                                if (o.TargetRelation != null && o.TargetRelation != Relation.Me)
                                {
                                    foreach (var item in o.TargetRelation.Query(this))
                                    {
                                        SetTrigger(o, item, false);
                                    }
                                }
                                else
                                {
                                    SetTrigger(o, this, false);
                                }
                                break;
                            case CollectionChangedAction.Replace:
                                if (!string.IsNullOrEmpty(n.Property) && n.Condition(this))
                                {
                                    if (n.TargetRelation != null && n.TargetRelation != Relation.Me)
                                    {
                                        foreach (var item in n.TargetRelation.Query(this))
                                        {
                                            SetTrigger(n, item, true, true);
                                        }
                                    }
                                    else
                                    {
                                        SetTrigger(n, this, true, true);
                                    }
                                }
                                if (o.TargetRelation != null && o.TargetRelation != Relation.Me)
                                {
                                    foreach (var item in o.TargetRelation.Query(this))
                                    {
                                        SetTrigger(o, item, false);
                                    }
                                }
                                else
                                {
                                    SetTrigger(o, this, false);
                                }
                                break;
                        }
                    };
                }
                return triggers;
            }
        }

        [NotCpfProperty]
        [Browsable(false)]
        public ISite Site
        {
            get;
            set;
        }
        /// <summary>
        /// 是否处在设计模式
        /// </summary>
        [UIPropertyMetadata(false, true)]
        [Browsable(false)]
        public bool DesignMode
        {
            get
            {
                return (bool)GetValue();
            }
        }
        /// <summary>
        /// 是否是设计模式下的根元素
        /// </summary>
        [Browsable(false)]
        public bool IsRootInDesignMode
        {
            get
            {
                return (bool)GetValue();
            }
        }

        /// <summary>
        /// 将指定工作区点的位置计算成屏幕坐标。像素坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Point PointToScreen(Point point)
        {
            var p = this;
            while (p != null)
            {
                point = p.TransformPoint(point);
                p = p.Parent;
            }
            point = Root.PointToScreen(point);
            return point;
        }
        /// <summary>
        /// 将指定工作区点的位置计算成相对View的位置
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point PointToView(Point point)
        {
            var p = this;
            while (p != null)
            {
                point = p.TransformPoint(point);
                p = p.Parent;
            }
            return point;
        }
        /// <summary>
        /// 将指定屏幕点的像素位置计算成工作区坐标。
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point PointToClient(Point point)
        {
            point = Root.ViewImpl.PointToClient(point);
            var p = this;
            List<UIElement> list = new List<UIElement>();
            while (p != null)
            {
                list.Add(p);
                p = p.Parent;
            }
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                Point l = new Point();
                if (!item.IsRoot)
                {
                    l = item.ActualOffset;
                }
                Point r = item.TransformPointInvert(point);
                point = new Point(r.X - l.X, r.Y - l.Y);
            }
            return point;
        }

        /// <summary>
        /// 应用到元素上的类名，多个类用,分割
        /// </summary>
        [Description("应用到元素上的类名，多个类用,分割")]
        [Category("设计")]
        [NotCpfProperty]
        public Classes Classes
        {
            get
            {
                if (classes == null)
                {
                    classes = new Classes();
                }
                return classes;
            }
            set
            {
                classes = value;
            }
        }

        //Styles styles;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PresenterFor = null;
                if (toolTipElement)
                {
                    if (ToolTipUIElement == toolTipElement)
                    {
                        ToolTipHost.Visibility = Visibility.Collapsed;
                        ToolTipUIElement = null;
                    }
                    toolTipElement.Dispose();
                }
                if (triggers != null)
                {
                    triggers.Clear();
                }
                if (notifyList != null)
                {
                    foreach (var item in notifyList)
                    {
                        item.Key.PropertyChanged -= Notify_PropertyChanged;
                    }
                }
                if (children != null)
                {
                    var c = children.Count;
                    //for (int i = 0; i < c; i++)
                    //{
                    //    var cc = children[0];
                    //    cc.Dispose();
                    //}
                    for (int i = c - 1; i >= 0; i--)
                    {
                        children[i].Dispose();
                    }
                }
                var p = Parent;
                if (p != null)
                {
                    p.children.Remove(this);
                }
            }
            this.RaiseEvent(EventArgs.Empty, nameof(Disposed));
            base.Dispose(disposing);
        }


        /// <summary>
        /// 查询所有内部元素，包含所有层
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Find<T>() where T : UIElement
        {
            if (children == null)
            {
                yield break;
            }
            foreach (UIElement item in Children)
            {
                if (item is T)
                {
                    yield return (T)item;
                }
                foreach (var c in item.Find<T>())
                {
                    yield return c;
                }
            }
        }

        internal void LoadStyle()
        {
            var host = Root;
            //if (host.StyleSheet != null)
            //{
            //    foreach (var item in host.StyleSheet.StyleRules)
            //    {
            //        //host.SetStyle(this, item);
            //    }
            //}
            if (host.Styles.Count > 0)
            {
                host.Styles.Update();
                host.SetStyle(this);
            }
            if (afterStyle != null)
            {
                if (host.IsInitialized)
                {
                    afterStyle.Method.FastInvoke(afterStyle.Target, this);
                }
                else
                {
                    if (host.afterStyles == null)
                    {
                        host.afterStyles = new List<(Delegate, UIElement)>();
                    }
                    host.afterStyles.Add((afterStyle, this));
                }
            }
        }
        [NotCpfProperty]
        internal bool loadStyle { set { SetFlag(CoreFlags.loadStyle, value); } get { return GetFlag(CoreFlags.loadStyle); } }// = true;
        void AttachedToVisualTree()
        {
            if (loadStyle)
            {
                Root.OnElementInitialize(this);
            }
            var host = Root;
            if (bindings != null)
            {
                foreach (var item in bindings.binds)
                {
                    foreach (var binding in item.Value)
                    {
                        if (binding.SourceElementLayer.HasValue)
                        {
                            UIElement obj = this;
                            for (int i = 1; i < binding.SourceElementLayer.Value + 1; i++)
                            {
                                obj = obj.Parent;
                            }
                            if (binding.Source == null)
                            {
                                binding.Source = new WeakReference(obj);
                            }
                            var bindingMode = binding.BindingMode;
                            if (bindingMode == BindingMode.OneWay || bindingMode == BindingMode.TwoWay)
                            {
                                //((INotifyPropertyChanged)obj).PropertyChanged += binding.PropertyChanged;
                                binding.RegisterPropertyChanged((INotifyPropertyChanged)obj);
                                binding.SourceToTarget();
                            }
                            else if (bindingMode == BindingMode.OneWayToSource)
                            {
                                binding.TargetToSource();
                            }
                            else if (bindingMode == BindingMode.OneTime)
                            {
                                binding.SourceToTarget();
                            }
                        }
                    }
                }
            }
            if (inheritsPropertyName != null)
            {
                foreach (var item in inheritsPropertyName)
                {
                    if (!HasLocalOrStyleValue(item, out var attribute))
                    {
                        var p = Parent;
                        while (p != null)
                        {
                            if (p.inheritsPropertyName != null)
                            {
                                if (p.inheritsPropertyName.Contains(item))
                                {
                                    if (p.inheritsValues.TryGetValue(item, out InheritsValue v))
                                    {
                                        var old = GetValue(item);
                                        inheritsValues.Remove(item);
                                        inheritsValues.Add(item, v);
                                        if (!old.Equal(v.Value))
                                        {
                                            //OnPropertyChanged(item, old, v.Value, GetPropertyMetadata(item));
                                            OnPropertyChanged(item, old, v.Value, attribute);
                                        }
                                        break;
                                    }
                                    else if (p.HasLocalOrStyleValue(item, out _))
                                    {
                                        var value = p.GetValue(item);
                                        var old = GetValue(item);
                                        inheritsValues.Remove(item);
                                        v = new InheritsValue { Value = value, ValueForm = ValueFrom.Property };
                                        inheritsValues.Add(item, v);
                                        if (!old.Equal(v.Value))
                                        {
                                            //OnPropertyChanged(item, old, v.Value, GetPropertyMetadata(item));
                                            OnPropertyChanged(item, old, v.Value, attribute);
                                        }
                                        break;
                                    }
                                }
                            }
                            p = p.Parent;
                        }
                    }
                }
            }


            if (children != null)
            {
                foreach (UIElement item in children)
                {
                    item.Root = host;
                }
            }
            OnAttachedToVisualTree();
            if (children != null)
            {
                foreach (UIElement item in children)
                {
                    item.AttachedToVisualTree();
                }
            }
        }

        /// <summary>
        /// Called when the control is added to a visual tree.
        /// </summary>
        protected virtual void OnAttachedToVisualTree()
        {

        }

        /// <summary>
        /// Called when the control is removed from a visual tree.
        /// </summary>
        protected virtual void OnDetachedFromVisualTree()
        {
            //if (this is Control control && control.IsInitialized)
            //{
            //    Root.OnElementFinalize(this);
            //}
            //else
            //{
            //    Root.OnElementFinalize(this);
            //}
            if (toolTipElement)
            {
                if (ToolTipUIElement == toolTipElement)
                {
                    ToolTipHost.Visibility = Visibility.Collapsed;
                    ToolTipUIElement = null;
                }
            }
            if (inheritsPropertyName != null && !IsDisposing)
            {
                foreach (var item in inheritsValues.ToArray())
                {
                    if (!HasLocalOrStyleValue(item.Key, out var attribute))
                    {
                        var old = GetValue(item.Key);
                        //if (inheritsValues.ContainsKey(item))
                        //{
                        inheritsValues.Remove(item.Key);
                        //}
                        var v = GetValue(item.Key);
                        if (!old.Equal(v))
                        {
                            //OnPropertyChanged(item.Key, old, v, GetPropertyMetadata(item.Key));
                            OnPropertyChanged(item.Key, old, v, attribute);
                        }
                    }
                }
                //inheritsValues.Clear();
            }
            //var name = Name;
            //if (!string.IsNullOrWhiteSpace(name))
            //{
            //    var r = Root;
            //    if (r.nameDic.TryGetValue(name, out UIElement e))
            //    {
            //        if (e == this)
            //        {
            //            r.nameDic.Remove(name);
            //        }
            //    }
            //}
            IsArrangeValid = false;
            if (children != null)
            {
                foreach (UIElement item in children)
                {
                    item.OnDetachedFromVisualTree();
                    item.Root = null;
                }
            }
            if (triggers != null && !IsDisposing)
            {
                foreach (var item in triggers.Where(a => a.Style != null).ToArray())
                {
                    triggers.Remove(item);
                }
            }
            if (bindings != null)
            {
                foreach (var item in bindings.binds)
                {
                    foreach (var binding in item.Value)
                    {
                        if (binding.SourceElementLayer.HasValue && binding.Source != null && binding.Source.IsAlive)
                        {
                            //((INotifyPropertyChanged)binding.Source.Target).PropertyChanged -= binding.PropertyChanged;
                            binding.CancellationPropertyChanged((INotifyPropertyChanged)binding.Source.Target);
                            binding.Source = null;
                        }
                    }
                }
            }
            ClearStyleValues();
            //if (commands != null)
            //{
            //    foreach (var item in commands.commands)
            //    {
            //        foreach (var command in item.Value)
            //        {
            //            if (command.SourceElementLayer.HasValue)
            //            {
            //                command.Target = null;
            //            }
            //        }
            //    }
            //}
        }

        internal void RaiseUIElementAdded(UIElement child)
        {
            //if (child.ZIndex != 0)
            //{
            //    //needSortZIndex = true;
            //    children.InvalidateZIndex();
            //}
            if (child.IsRoot)
            {
                throw new Exception("根元素不能作为子元素嵌套");
            }
            if (child.IsDisposed)
            {
                throw new Exception("被释放了的元素不能再添加使用");
            }
            child.Root = Root;
            if (Root != null)
            {
                child.IsMeasureValid = false;
                child.AttachedToVisualTree();
            }
            viewRenderRect = true;
            InvalidateMeasure();
            OnUIElementAdded(new UIElementAddedEventArgs(child, this));
        }

        protected virtual void OnUIElementAdded(UIElementAddedEventArgs e)
        {
            this.RaiseEvent(e, "UIElementAdded");
        }
        //public static readonly RoutedEvent EventUIElementAdded = new RoutedEvent("UIElementAdded", UIElementType, typeof(EventHandler<UIElementAddedEventArgs>));
        /// <summary>
        /// 添加可视化对象的时候
        /// </summary>
        public virtual event EventHandler<UIElementAddedEventArgs> UIElementAdded
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal void RaiseUIElementRemoved(UIElement child)
        {
            if (child.Root != null)
            {
                child.OnDetachedFromVisualTree();
                child.Root = null;
            }

            if (!IsDisposed)
            {
                viewRenderRect = true;
                InvalidateMeasure();
            }
            OnUIElementRemoved(new UIElementRemovedEventArgs(child, this));
            if (child.needDispose)
            {
                child.Dispose();
            }
        }
        protected virtual void OnUIElementRemoved(UIElementRemovedEventArgs e)
        {
            this.RaiseEvent(e, "UIElementRemoved");
        }
        //public static readonly RoutedEvent EventUIElementRemoved = new RoutedEvent("UIElementRemoved", UIElementType, typeof(EventHandler<UIElementRemovedEventArgs>));
        /// <summary>
        /// 移除可视化对象的时候
        /// </summary>
        public virtual event EventHandler<UIElementRemovedEventArgs> UIElementRemoved
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        public virtual event EventHandler DesiredSizeChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("OnPreviewMouseDown" + this);
            this.RaiseEvent(e, nameof(PreviewMouseDown));
        }

        public virtual event EventHandler<MouseButtonEventArgs> PreviewMouseDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("OnPreviewMouseUp" + this + e.OriginalSource);
            this.RaiseEvent(e, nameof(PreviewMouseUp));
        }

        public virtual event EventHandler<MouseButtonEventArgs> PreviewMouseUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            //if (e.MouseButton == MouseButton.Left)
            //{
            //    IsMouseLeftButtonDown = true;
            //}
            //System.Diagnostics.Debug.WriteLine(e.OriginalSource);
            this.RaiseEvent(e, nameof(MouseDown));
        }

        public virtual event EventHandler<MouseButtonEventArgs> MouseDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            //if (e.MouseButton == MouseButton.Left)
            //{
            //    IsMouseLeftButtonDown = false;
            //}
            this.RaiseEvent(e, nameof(MouseUp));
            if (e.MouseButton == MouseButton.Right && !e.Handled)
            {
                var cm = ContextMenu;
                if (cm != null)
                {
                    cm.DataContext = DataContext;
                    cm.CommandContext = CommandContext;
                    cm.PlacementTarget = this;
                    cm.IsOpen = true;
                }
            }
        }
        //public static readonly RoutedEvent MouseUpEvent = new RoutedEvent("MouseUp", UIElementType, MouseEventType);
        public virtual event EventHandler<MouseButtonEventArgs> MouseUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnDoubleClick(RoutedEventArgs e)
        {
            this.RaiseEvent(e, nameof(DoubleClick));
        }

        public virtual event EventHandler<RoutedEventArgs> DoubleClick
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                Root.SetCursor(Cursor);
            }
            this.RaiseEvent(e, nameof(MouseMove));
        }
        //public static readonly RoutedEvent MouseMoveEvent = new RoutedEvent("MouseMove", UIElementType, MouseEventType);
        public virtual event EventHandler<MouseEventArgs> MouseMove
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnMouseEnter(MouseEventArgs e)
        {
            if (toolTipElement)
            {
                var re = new RoutedEventArgs(this);
                RaiseEvent(re, nameof(ToolTipOpening));
                if (!re.Handled)
                {
                    ToolTipHost.Width = FloatField.Auto;
                    ToolTipHost.Height = FloatField.Auto;
                    ShowTip(Root, toolTipElement);
                }
            }
            IsMouseOver = true;
            this.RaiseEvent(e, nameof(MouseEnter));
        }
        //public static readonly RoutedEvent MouseEnterEvent = new RoutedEvent("MouseEnter", UIElementType, MouseEventType);
        public virtual event EventHandler<MouseEventArgs> MouseEnter
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnMouseLeave(MouseEventArgs e)
        {
            if (toolTipElement)
            {
                if (ToolTipUIElement == toolTipElement)
                {
                    ToolTipHost.Visibility = Visibility.Collapsed;
                    if (!CPF.Platform.Application.DisablePopupClose)
                    {
                        ToolTipUIElement = null;
                    }
                }
            }
            IsMouseOver = false;
            this.RaiseEvent(e, nameof(MouseLeave));
        }

        public virtual event EventHandler<MouseEventArgs> MouseLeave
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        //public virtual event EventHandler<TouchEventArgs> TouchLeave
        //{
        //    add { AddHandler(value); }
        //    remove { RemoveHandler(value); }
        //}

        //protected virtual void OnTouchLeave(TouchEventArgs e)
        //{
        //    IsMouseOver = false;
        //    //Debug.WriteLine("TouchLeave" + this + e.Position);
        //    this.RaiseEvent(e, nameof(TouchLeave));
        //}

        //public virtual event EventHandler<TouchEventArgs> TouchEnter
        //{
        //    add { AddHandler(value); }
        //    remove { RemoveHandler(value); }
        //}

        //protected virtual void OnTouchEnter(TouchEventArgs e)
        //{
        //    IsMouseOver = true;
        //    //Debug.WriteLine("TouchEnter" + this + e.Position);
        //    this.RaiseEvent(e, nameof(TouchEnter));
        //}

        public virtual event EventHandler<TouchEventArgs> TouchUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnTouchUp(TouchEventArgs e)
        {
            //Debug.WriteLine("TouchUp" + this + e.Position);
            this.RaiseEvent(e, nameof(TouchUp));
        }

        public virtual event EventHandler<TouchEventArgs> TouchDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnTouchDown(TouchEventArgs e)
        {
            //Debug.WriteLine("TouchDown" + this + e.Position);
            this.RaiseEvent(e, nameof(TouchDown));
        }


        public virtual event EventHandler<TouchEventArgs> TouchMove
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnTouchMove(TouchEventArgs e)
        {
            //Debug.WriteLine("TouchMove" + this + e.Position);
            this.RaiseEvent(e, nameof(TouchMove));
        }

        //public virtual event EventHandler<TouchEventArgs> ManipulationStarted
        //{
        //    add { AddHandler(value); }
        //    remove { RemoveHandler(value); }
        //}

        //protected virtual void OnManipulationStarted(TouchEventArgs e)
        //{
        //    this.RaiseEvent(e, nameof(ManipulationStarted));
        //}
        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            this.RaiseEvent(e, nameof(MouseWheel));
        }

        public virtual event EventHandler<MouseWheelEventArgs> MouseWheel
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            this.RaiseEvent(e, nameof(KeyDown));
        }

        public virtual event EventHandler<KeyEventArgs> KeyDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            this.RaiseEvent(e, nameof(KeyUp));
        }
        //public static readonly RoutedEvent KeyUpEvent = new RoutedEvent("KeyUp", UIElementType, KeyEventType);
        public virtual event EventHandler<KeyEventArgs> KeyUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            this.RaiseEvent(e, nameof(TextInput));
        }
        public virtual event EventHandler<TextInputEventArgs> TextInput
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnGotFocus(GotFocusEventArgs e)
        {
            this.RaiseEvent(e, nameof(GotFocus));
        }
        //public static readonly RoutedEvent GotFocusEvent = new RoutedEvent("GotFocus", UIElementType, typeof(EventHandler<GotFocusEventArgs>));
        public virtual event EventHandler<GotFocusEventArgs> GotFocus
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnLostFocus(RoutedEventArgs e)
        {
            RaiseEvent(e, nameof(LostFocus));
        }
        //public static readonly RoutedEvent LostFocusEvent = new RoutedEvent("LostFocus", UIElementType, typeof(EventHandler<RoutedEventArgs>));
        public virtual event EventHandler<RoutedEventArgs> LostFocus
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


        protected virtual void OnLayoutUpdated()
        {
            this.RaiseEvent(new RoutedEventArgs(this), nameof(LayoutUpdated));
        }
        /// <summary>
        /// 捕获鼠标，只有鼠标在元素范围内按下，而且IsEnabled为TRUE的时候才能捕获。当对象已捕获鼠标后，它接收鼠标输入，不论鼠标指针是否在其边界区域。 通常只有在执行模拟拖动操作时才捕获鼠标。 若要释放鼠标捕获，请对具有捕获的对象调用 ReleaseMouseCapture 方法。
        /// </summary>
        /// <returns></returns>
        public bool CaptureMouse()
        {
            var host = Root;
            if (IsEnabled && host != null)
            {
                host.InputManager.MouseDevice.Capture(this);
                IsMouseCaptured = true;
                OnCaptureMouse();
                return true;
            }
            return false;
        }

        protected virtual void OnCaptureMouse()
        {

        }

        /// <summary>
        /// ReleaseMouseCapture 方法对于已通过使用 CaptureMouse 方法捕获鼠标的对象禁用鼠标捕获。 当对象已捕获鼠标后，它接收鼠标输入，不论鼠标指针是否在其边界区域。 对不具有鼠标捕获的对象调用 ReleaseMouseCapture 无效。
        /// </summary>
        public void ReleaseMouseCapture()
        {
            var host = Root;
            if (host != null)
            {
                host.InputManager.MouseDevice.Capture(null);
            }
            SetReleaseMouseCapture();
        }

        internal void SetReleaseMouseCapture()
        {
            if (IsMouseCaptured)
            {
                IsMouseCaptured = false;
                OnReleaseMouseCapture();
            }
        }

        protected virtual void OnReleaseMouseCapture()
        {

        }

        //public static readonly RoutedEvent LayoutUpdatedEvent = new RoutedEvent("LayoutUpdated", UIElementType, typeof(EventHandler<RoutedEventArgs>));
        public virtual event EventHandler<RoutedEventArgs> LayoutUpdated
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        public event EventHandler Disposed
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        //public event EventHandler<RoutedEventArgs> ToolTipClosing
        //{
        //    add { AddHandler(value); }
        //    remove { RemoveHandler(value); }
        //}
        public event EventHandler<RoutedEventArgs> ToolTipOpening
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnDragEnter(DragEventArgs e)
        {
            IsDragOver = true;
            //System.Diagnostics.Debug.WriteLine(this + " " + e.DragEffects);
            RaiseEvent(e, nameof(DragEnter));
        }
        public event EventHandler<DragEventArgs> DragEnter
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnDragLeave(EventArgs e)
        {
            IsDragOver = false;
            RaiseEvent(e, nameof(DragLeave));
        }
        public event EventHandler DragLeave
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnDragOver(DragEventArgs e)
        {
            RaiseEvent(e, nameof(DragOver));
        }
        public event EventHandler<DragEventArgs> DragOver
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        protected virtual void OnDrop(DragEventArgs e)
        {
            RaiseEvent(e, nameof(Drop));
        }
        public event EventHandler<DragEventArgs> Drop
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return "#" + Name;
            }
            return base.ToString();
        }
        /// <summary>
        /// 克隆依赖属性,绑定,子元素，触发器
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var e = base.Clone() as UIElement;
            if (children != null && children.Count > 0)
            {
                foreach (UIElement item in children)
                {
                    e.Children.Add(item.Clone() as UIElement);
                }
            }
            if (triggers != null && triggers.Count > 0)
            {
                foreach (Trigger item in triggers)
                {
                    e.Triggers.Add(item);
                }
            }
            if (inheritsValues != null)
            {
                foreach (var item in inheritsValues)
                {
                    if (HasLocalOrStyleValue(item.Key, out _))
                    {
                        e.inheritsValues.Add(item.Key, item.Value);
                    }
                }
            }
            return e;
        }
        /// <summary>
        /// 将依赖属性本地值，绑定，子元素，触发器，拷贝到另外个对象
        /// </summary>
        /// <param name="element"></param>
        public void CopyTo(UIElement element)
        {
            base.CopyTo(element);
            if (children != null && children.Count > 0)
            {
                element.Children.Clear();
                foreach (UIElement item in children)
                {
                    element.Children.Add(item.Clone() as UIElement);
                }
            }
            if (triggers != null && triggers.Count > 0)
            {
                element.Triggers.Clear();
                foreach (Trigger item in triggers)
                {
                    element.Triggers.Add(item);
                }
            }
        }

        //public IEnumerator<UIElement> GetEnumerator()
        //{
        //    return Children.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return Children.GetEnumerator();
        //}
        //public static UIElement Parse(string str)
        //{
        //    return null;
        //}

        ///// <summary>
        ///// 字符串解析为UI元素
        ///// </summary>
        ///// <param name="n"></param>
        //public static implicit operator UIElement(string n)
        //{
        //    return Parse(n);
        //}

        IntPtr id;
        /// <summary>
        /// 获取对象唯一地址
        /// </summary>
        /// <returns></returns>
        public IntPtr GetIntPtr()
        {
            if (id == IntPtr.Zero)
            {
                GCHandle h = GCHandle.Alloc(this, GCHandleType.WeakTrackResurrection);
                id = GCHandle.ToIntPtr(h);
            }
            return id;
        }
        [NotCpfProperty]
        internal bool needDispose
        {
            get { return GetFlag(CoreFlags.needDispose); }
            set { SetFlag(CoreFlags.needDispose, value); }
        }

        internal bool GetFlag(CoreFlags field)
        {
            return (_flags & field) != 0;
        }

        internal void SetFlag(CoreFlags field, bool value)
        {
            if (value)
            {
                _flags |= field;
            }
            else
            {
                _flags &= (~field);
            }
        }
        /// <summary>
        /// 用来省内存，8个bool字段只要1字节，省7个字节
        /// </summary>
        private CoreFlags _flags;
    }

    [Flags]
    enum CoreFlags : byte
    {
        None = 0,
        needDispose = 0b10000000,
        loadStyle = 0b01000000,
        viewRenderRect = 0b00100000,
        transform = 0b00010000,
        inheritsSet = 0b00001000,
        IsArrangeValid = 0b00000100,
        isMeasureValid = 0b00000010,
        isRoot = 0b00000001,
    }

    struct InheritsValue
    {
        public object Value;
        public ValueFrom ValueForm;
    }

    enum ValueFrom : byte
    {
        Property,
        Trigger,
        Style,
        Animation,
    }
}
