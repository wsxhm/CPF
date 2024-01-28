using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Platform;
using CPF.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace CPF.Controls
{
    /// <summary>
    /// 顶级窗体
    /// </summary>
    [Description("顶级窗体")]
    [DesignerCategory("Form")]
    public class Window : View, IApp, IWindow
    {
        static List<Window> windows = new List<Window>();
        /// <summary>
        /// 获取当前存在的所有窗体
        /// </summary>
        public static IEnumerable<Window> Windows { get { return windows; } }

        IWindowImpl windowImpl;

        bool isLoaded = false;
        public event EventHandler Loaded
        {
            add => AddHandler(value);
            remove => RemoveHandler(value);
        }
        public event EventHandler Closed
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        public event EventHandler<ClosingEventArgs> Closing
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        public Window()
        {
            windowImpl = ViewImpl as IWindowImpl;
            windowImpl.Closed = OnClose;
            windowImpl.Closing = OnClosing;
            windowImpl.WindowStateChanged = WindowStateChanged;
            windows.Add(this);
        }

        protected override IViewImpl CreateView()
        {
            return Application.GetRuntimePlatform().CreateWindow();
        }

        //public Window(IWindowImpl window) : base(window)
        //{
        //    windowImpl = ViewImpl as IWindowImpl;
        //}
        bool setWindowState;
        void WindowStateChanged()
        {
            setWindowState = true;
            WindowState = windowImpl.WindowState;
            //if (windowImpl.WindowState != WindowState.Maximized)
            //{
            //    IsFullScreen = false;
            //}
            setWindowState = false;
        }
        [PropertyChanged(nameof(WindowState))]
        void OnWindowStateChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!setWindowState)
            {
                windowImpl.WindowState = (WindowState)newValue;
            }
        }

        [PropertyMetadata(WindowState.Normal)]
        public WindowState WindowState
        {
            get
            {
                return GetValue<WindowState>();
            }

            set
            {
                SetValue(value);
            }
        }
        [NotCpfProperty]
        public bool CanResize
        {
            get;
            set;
        }
        [PropertyMetadata(true)]
        public bool ShowInTaskbar
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(false)]
        public bool TopMost
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(null), CPF.Design.FileBrowser(".png;.jpg;.jpeg;.bmp;.gif")]
        public Image Icon
        {
            get { return GetValue<Image>(); }
            set { SetValue(value); }
        }

        //public bool IsFullScreen
        //{
        //    get { return GetValue<bool>(); }
        //    set { SetValue(value); }
        //}
        //[PropertyChanged(nameof(IsFullScreen))]
        //void OnFullScreen(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        //{
        //    windowImpl.SetFullscreen((bool)newValue);
        //}

        public void Hide()
        {
            //windowImpl.Hide();
            Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 窗体标题
        /// </summary>
        [PropertyMetadata("窗体标题")]
        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public void Show()
        {
            if (!this.isLoaded)
            {
                this.BeginInvoke(() => RaiseEvent(EventArgs.Empty, nameof(Loaded)));
                this.isLoaded = true;
            }
            //windowImpl.Show();
            Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 拖拽移动窗体，里面任意元素的MouseDown调用该方法就可以移动窗体
        /// </summary>
        public void DragMove()
        {
            //windowImpl.DragMove();
            IsDragMove = true;
            dragMove = MouseDevice.Location;
            startOffset = Position;
            CaptureMouse();
        }
        /// <summary>
        /// 开始拖拽时候窗体位置
        /// </summary>
        internal PixelPoint startOffset;
        /// <summary>
        /// 开始拖拽时候鼠标位置
        /// </summary>
        internal PixelPoint dragMove;
        /// <summary>
        /// 正在拖拽移动中
        /// </summary>
        [Browsable(false)]
        public bool IsDragMove
        {
            get { return GetValue<bool>(); }
            internal set { SetValue(value); }
        }
        /// <summary>
        /// 拖拽的边缘厚度
        /// </summary>
        [PropertyMetadata(5)]
        public int DragThickness
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        public void Close()
        {
            //Visibility = Visibility.Collapsed;
            windowImpl.Close();
        }

        [PropertyChanged(nameof(Title))]
        void RegisterTitle(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            windowImpl.SetTitle(newValue == null ? "" : newValue as string);
        }
        [PropertyChanged(nameof(ShowInTaskbar))]
        void RegisterShowInTaskbar(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            windowImpl.ShowInTaskbar((bool)newValue);
        }
        [PropertyChanged(nameof(TopMost))]
        void RegisterTopMost(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            windowImpl.TopMost((bool)newValue);
        }
        [PropertyChanged(nameof(Icon))]
        void RegisterIcon(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (newValue == null)
            {
                throw new Exception(Icon + "不能为空");
            }
            windowImpl.SetIcon((Image)newValue);
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(Title))
        //    {
        //        windowImpl.SetTitle(newValue as string);
        //    }
        //    else if (propertyName == nameof(ShowInTaskbar))
        //    {
        //        windowImpl.ShowInTaskbar((bool)newValue);
        //    }
        //    else if (propertyName == nameof(TopMost))
        //    {
        //        windowImpl.TopMost((bool)newValue);
        //    }
        //    else if (propertyName == nameof(Icon))
        //    {
        //        if (newValue == null)
        //        {
        //            throw new Exception(Icon + "不能为空");
        //        }
        //        windowImpl.SetIcon((Image)newValue);
        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits));
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"#fff", UIPropertyOptions.AffectsRender));
        }
        /// <summary>
        /// 窗体所在的屏幕
        /// </summary>
        [NotCpfProperty]
        public override Screen Screen
        {
            get
            {
                if (dialgLayoutWindow != null)
                {
                    return dialgLayoutWindow.Screen;
                }
                return base.Screen;
            }
        }

        public override float LayoutScaling
        {
            get
            {
                if (dialgLayoutWindow != null)
                {
                    return dialgLayoutWindow.LayoutScaling;
                }
                return base.LayoutScaling;
            }
        }

        public override float RenderScaling
        {
            get
            {
                if (dialgLayoutWindow != null)
                {
                    return dialgLayoutWindow.RenderScaling;
                }
                return base.RenderScaling;
            }
        }

        Window dialgLayoutWindow;
        /// <summary>
        /// 是否是主窗体
        /// </summary>
        public bool IsMain
        {
            get
            {
                return windowImpl.IsMain;
            }
        }

        bool IApp.IsMain
        {
            get
            {
                return windowImpl.IsMain;
            }
            set
            {
                windowImpl.IsMain = value;
            }
        }

        void SetDialog(bool enable, Window child)
        {
            if (dialogChildren == null)
            {
                dialogChildren = new HashSet<Window>();
            }
            if (enable)
            {
                dialogChildren.Remove(child);
                if (dialogChildren.Count == 0)
                {
                    windowImpl.SetEnable(true);
                }
            }
            else
            {
                dialogChildren.Add(child);
                windowImpl.SetEnable(false);
            }
        }

        //bool isDialog;
        HashSet<Window> dialogChildren;
        Window owner;
        ManualResetEvent invokeMre = new ManualResetEvent(false);
        CancellationTokenSource cancellation;
        /// <summary>
        /// 模态显示，如果owner为null，将禁用之前打开的所有窗体，否则只禁用owner。 需要有主窗体的情况下才能使用
        /// </summary>
        /// <param name="owner"></param>
        public Task<object> ShowDialog(Window owner = null)
        {
            Invoke(() =>
            {
                if (owner == null)
                {
                    owner = Windows.FirstOrDefault(a => a.IsKeyboardFocusWithin);
                    if (owner == null)
                    {
                        owner = Windows.FirstOrDefault(a => a.IsMain);
                    }
                    if (owner == null)
                    {
                        throw new Exception("需要有主窗体的情况下才能使用");
                    }
                    foreach (var item in Windows)
                    {
                        if (item != this)
                        {
                            //item.windowImpl.SetEnable(false);
                            item.SetDialog(false, this);
                            item.GotFocus += Item_GotFocus;
                        }
                    }
                }
                else
                {
                    owner.SetDialog(false, this);
                }
                this.owner = owner;
                //isDialog = true;
                dialgLayoutWindow = owner;
                windowImpl.ShowDialog(owner);
                dialgLayoutWindow = null;
                dialogResult = DialogResult;
            });
            var task = Task<object>.Factory.StartNew(() =>
            {
                if (!IsDisposed)
                {
                    invokeMre.Reset();
                    invokeMre.WaitOne();
                }
                return dialogResult;
            });
            return task;
        }
        /// <summary>
        /// 对话框，同步方式，阻塞当前方法
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public object ShowDialogSync(Window owner = null)
        {
            cancellation = new CancellationTokenSource();
            Invoke(() =>
            {
                if (owner == null)
                {
                    owner = Windows.FirstOrDefault(a => a.IsKeyboardFocusWithin);
                    if (owner == null)
                    {
                        owner = Windows.FirstOrDefault(a => a.IsMain);
                    }
                    if (owner == null)
                    {
                        throw new Exception("需要有主窗体的情况下才能使用");
                    }
                    foreach (var item in Windows)
                    {
                        if (item != this)
                        {
                            //item.windowImpl.SetEnable(false);
                            item.SetDialog(false, this);
                            item.GotFocus += Item_GotFocus;
                        }
                    }
                }
                else
                {
                    owner.SetDialog(false, this);
                }
                this.owner = owner;
                //isDialog = true;
                dialgLayoutWindow = owner;
                windowImpl.ShowDialog(owner);
                dialgLayoutWindow = null;
                dialogResult = DialogResult;
            });
            Application.RunLoop(cancellation.Token);
            foreach (var item in Windows)
            {
                if (item != this)
                {
                    item.GotFocus -= Item_GotFocus;
                    //item.windowImpl.SetEnable(true);
                    item.SetDialog(true, this);
                }
            }
            return dialogResult;
        }

        private void Item_GotFocus(object sender, GotFocusEventArgs e)
        {
            if (this != sender)
            {
                this.Focus();
            }
        }

        bool isClosing;
        bool OnClosing()
        {
            isClosing = true;
            var e = new ClosingEventArgs();
            OnClosing(e);
            if (!e.Cancel)
            {
                BeginInvoke(() =>
                {
                    Visibility = Visibility.Collapsed;
                });
            }
            isClosing = false;
            return e.Cancel;
        }

        protected virtual void OnClosing(ClosingEventArgs e)
        {
            RaiseEvent(e, nameof(Closing));
        }

        void OnClose()
        {
            //isDialog = false;
            invokeMre.Set();
            if (owner != null)
            {
                foreach (var item in Windows)
                {
                    if (item != this)
                    {
                        item.GotFocus -= Item_GotFocus;
                        //item.windowImpl.SetEnable(true);
                        item.SetDialog(true, this);
                    }
                }
            }
            owner = null;
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation = null;
            }
            windows.Remove(this);
            OnClosed(EventArgs.Empty);
            Dispose();
        }

        object dialogResult;
        /// <summary>
        /// 如果是模态，设置该属性自动关闭模态窗体
        /// </summary>
        public object DialogResult
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        [PropertyChanged(nameof(DialogResult))]
        void OnDialogResultChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            dialogResult = newValue;
            if (owner != null && !isClosing && !IsDisposed && !IsDisposing)
            {
                //isDialog = false;
                Close();
                //owner = null;
            }
        }

        protected virtual void OnClosed(EventArgs e)
        {
            RaiseEvent(e, nameof(Closed));
        }

        //protected override void Dispose(bool disposing)
        //{
        //    windowImpl.Dispose();
        //    base.Dispose(disposing);
        //}
    }

    public class ClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }

    //public enum DialogResult : byte
    //{
    //    None,
    //    OK,
    //    Cancel,
    //}
}
