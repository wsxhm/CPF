using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using CPF.Threading;
using CPF.Shapes;
using System.ComponentModel;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 Menu 内某个可选择的项。
    /// </summary>
    [Description("表示 Menu 内某个可选择的项。")]
    public class MenuItem : ItemsControl<MenuItem>
    {
        public MenuItem()
        {

        }

        //DispatcherTimer dispatcherTimer;
        private void Popup_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (dispatcherTimer == null)
            //{
            //    dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10), IsEnabled = true, };
            //    dispatcherTimer.Tick += delegate
            //    {
            //        if (dispatcherTimer != null)
            //        {
            //            dispatcherTimer.Dispose();
            //            dispatcherTimer = null;
            //        }
            //        if (timer != null)
            //        {
            //            timer.Dispose();
            //            timer = null;
            //        }
            //    };
            //}
            BeginInvoke(() =>
            {
                //System.Diagnostics.Debug.WriteLine("PopupPanel_MouseEnter");
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            });
        }

        ///// <summary>
        ///// 定义数据类型关联的模板，初始化或者附加到可视化树之前设置
        ///// </summary>
        //public UIElementTemplate<MenuItem> ItemTemplate
        //{
        //    get { return GetValue<UIElementTemplate<MenuItem>>(); }
        //    set { SetValue(value); }
        //}

        //public override UIElement CreateItemElement()
        //{
        //    return ItemTemplate.CreateElement();
        //}
        /// <summary>
        /// 获取或设置一个值，该值指示在单击此 MenuItem 时，该项所在的子菜单不应关闭。
        /// </summary>
        public bool StaysOpenOnClick
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置标记控件的项
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public object Header
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 显示在 MenuItem 中的图标
        /// </summary>
        public object Icon
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取一个指示是否可选中 MenuItem 的值。
        /// </summary>
        public bool IsCheckable
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个指示是否选中 MenuItem 的值
        /// </summary>
        public bool IsChecked
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置用于显示控件标头的内容的模板。
        /// </summary>
        [Browsable(false)]
        public UIElementTemplate<ContentTemplate> HeaderTemplate
        {
            get { return GetValue<UIElementTemplate<ContentTemplate>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 获取或设置是否显示子菜单
        /// </summary>
        public bool IsOpen
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        ContextMenu contextMenu;
        /// <summary>
        /// 所在的ContextMenu
        /// </summary>
        [NotCpfProperty]
        [Browsable(false)]
        public ContextMenu OwnerContextMenu
        {
            get { return contextMenu; }
            internal set
            {
                if (contextMenu != value)
                {
                    contextMenu = value;
                    foreach (MenuItem item in ElementItems.Where(a => a is MenuItem))
                    {
                        item.OwnerContextMenu = value;
                    }
                }
            }
        }
        [NotCpfProperty]
        public ItemsControl<MenuItem> ParentItem
        {
            get;
            internal set;
        }
        /// <summary>
        /// #MenuPop
        /// </summary>
        Popup popup;
        protected Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = new Popup();
                    //popup.LoadStyle(Root.styleSheet?.ToString());
                    popup.LoadStyle(Root);
                    popup.Name = "MenuPop";
                    popup.PlacementTarget = this;
                    popup.Placement = PlacementMode.Margin;
                    popup.MarginLeft = 0;
                    popup.MarginBottom = "-100%";
                    //popup.LostFocus += Popup_LostFocus;
                    popup.MouseEnter += Popup_MouseEnter;
                    popup.StaysOpen = false;
                    popup.CanActivate = false;
                    popup.Background = null;
                    popup.PropertyChanged += Popup_PropertyChanged;
                }
                return popup;
            }
        }

        private void Popup_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Visibility))
            {
                if ((Visibility)e.NewValue != Visibility.Visible)
                {
                    IsOpen = false;
                }
                else
                {
                    IsOpen = true;
                }
            }
        }

        protected override bool IsItemElement(object item)
        {
            return item is MenuItem || item is Separator;
        }
        //DispatcherTimer dispatcherTimer;
        //private void Popup_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    //if (dispatcherTimer == null)
        //    //{
        //    //    dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50), IsEnabled = true };
        //    //    dispatcherTimer.Tick += delegate
        //    //    {
        //    //        dispatcherTimer.Dispose();
        //    //        dispatcherTimer = null;
        //    //        if (!IsMouseOverMenu(contextMenu) && !contextMenu.popup.IsMouseOver)
        //    //        {
        //    //            contextMenu.IsOpen = false;
        //    //        }
        //    //    };
        //    //}
        //    BeginInvoke(() =>
        //    {
        //        if (!IsMouseOverMenu(contextMenu) && !contextMenu.Popup.IsMouseOver)
        //        {
        //            contextMenu.IsOpen = false;
        //        }
        //    });
        //}

        bool IsMouseOverMenu(ItemsControl<MenuItem> itemsControl)
        {
            foreach (MenuItem item in itemsControl.ElementItems.Where(a => a is MenuItem))
            {
                if (item.popup != null && item.popup.IsMouseOver)
                {
                    return true;
                }
                if (item.popup != null)
                {
                    if (IsMouseOverMenu(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        Panel popupPanel = new Panel { Name = "menuPanel" };
        protected Panel PopupPanel
        {
            get { return popupPanel; }
        }
        //protected override Panel GetItemsPanel()
        //{
        //    var p = popupPanel.Find<Panel>().FirstOrDefault(a => a.PresenterFor == this && a.Name == "itemsPanel");
        //    if (p == null)
        //    {
        //        var sv = popupPanel.Find<ScrollViewer>().FirstOrDefault(a => a.Content is Panel && (a.Content as Panel).PresenterFor == this && (a.Content as Panel).Name == "itemsPanel");
        //        if (sv != null)
        //        {
        //            return sv.Content as Panel;
        //        }
        //    }
        //    return p;
        //}

        protected override void InitializeComponent()
        {
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.Width = 150;
            //panel.Background = "#eee";
            PopupPanel.Children.Add(new Border { Child = panel, ShadowBlur = 4, Background = "#eee", BorderFill = "#999" });

            this.Children.Add(new ContentControl
            {
                MarginLeft = 5,
                Width = 18,
                Height = 18,
                Bindings = {
                    { nameof(ContentControl.Content),nameof(Icon),this } ,
                    { nameof(Panel.Visibility),nameof(IsCheckable),this,BindingMode.OneWay,a=>!(bool)a?Visibility.Visible:Visibility.Collapsed } ,
                }
            }
            );
            this.Children.Add(new Panel
            {
                Name = "checkPanel",
                MarginLeft = 5,
                Width = 18,
                Height = 18,
                Children = { new Polyline { Points = { new Drawing.Point(0, 3), new Drawing.Point(4, 8), new Drawing.Point(12, 1) }, StrokeStyle = new Drawing.Stroke(2), IsAntiAlias = true } },
                Bindings = {
                    { nameof(Panel.Visibility),nameof(IsChecked),this,BindingMode.OneWay,a=>(bool)a?Visibility.Visible:Visibility.Collapsed } ,
                }
            }
            );
            this.Children.Add(new Panel
            {
                MarginLeft = 30,
                MarginRight = 5,
                MarginBottom = 3,
                MarginTop = 3,
                Name = "contentPanel",
                Children =
                {
                    new ContentControl
                    {
                        MarginLeft=0,
                        Height = 25,
                        Bindings = {
                            { nameof(ContentControl.Content),nameof(Header),this } ,
                            { nameof(ContentControl.ContentTemplate),nameof(HeaderTemplate),this } ,
                        }
                    }
                }
            }
            );
            this.Children.Add(new Polygon { Points = { new Drawing.Point(), new Drawing.Point(0, 6), new Drawing.Point(3, 3) }, Fill = "#000", MarginRight = 5, Bindings = { { nameof(Visibility), nameof(HasItems), this, BindingMode.OneWay, a => (bool)a ? Visibility.Visible : Visibility.Collapsed } } });

            MarginLeft = 0;
            MarginRight = 0;
            this.Triggers.Add(new Styling.Trigger { Property = nameof(IsMouseOver), Setters = { { nameof(Background), "#fff" } } });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!e.Handled)
            {
                OnClick(e);
                if (!StaysOpenOnClick && !HasItems)
                {
                    contextMenu.IsOpen = false;
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled && IsCheckable)
            {
                IsChecked = !IsChecked;
            }
        }
        protected virtual void OnClick(EventArgs args)
        {
            RaiseEvent(args, nameof(Click));
        }

        public event EventHandler Click
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected override void OnItemElementAdded(UIElement element)
        {
            if (element is MenuItem menuItem)
            {
                menuItem.OwnerContextMenu = OwnerContextMenu;
                menuItem.ParentItem = this;
            }
            if (popup != null)
            {
                Popup.Height = "Auto";
                Popup.Width = "Auto";
                Popup.InvalidateMeasure();
            }

        }
        protected override void OnItemElementRemoved(UIElement element)
        {
            if (element is MenuItem menuItem)
            {
                menuItem.OwnerContextMenu = null;
                menuItem.ParentItem = null;
            }
            if (popup != null)
            {
                Popup.Height = "Auto";
                Popup.Width = "Auto";
                Popup.InvalidateMeasure();
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!e.Handled && HasItems)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                foreach (MenuItem item in ParentItem.ElementItems.Where(a => a is MenuItem && (a as MenuItem).IsOpen && a != this))
                {
                    item.IsOpen = false;
                }
                if (contextMenu.IsOpen)
                {
                    IsOpen = true;
                }
            }
        }
        DispatcherTimer timer;
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            //System.Diagnostics.Debug.WriteLine("OnMouseLeave");
            if (IsOpen && HasItems)
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500), IsEnabled = true };
                    timer.Tick += Timer_Tick;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            IsOpen = false;
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            //if (Root != null)
            //{
            //    Root.Focus();
            //}
            //if (contextMenu != null)
            //{
            //    //if (contextMenu.ElementItems.Where(a => a is MenuItem && (a as MenuItem).IsOpen).FirstOrDefault() == null)
            //    //{
            //    contextMenu.popup.Focus();
            //    //}
            //}
        }
        [PropertyChanged(nameof(IsOpen))]
        void RegisterIsOpen(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var v = (bool)newValue;
            if (v)
            {
                if (popupPanel.Parent == null)
                {
                    Popup.LoadStyle(OwnerContextMenu.Popup);
                    Popup.Children.Add(PopupPanel);
                }
                //OwnerContextMenu.needHide = false;
                Popup.DataContext = DataContext;
                Popup.CommandContext = CommandContext;
                Popup.Height = "Auto";
                Popup.Width = "Auto";
                Popup.InvalidateMeasure();
                Popup.Visibility = Visibility.Visible;
            }
            else
            {
                if (popup != null)
                {
                    popup.Visibility = Visibility.Collapsed;
                    foreach (MenuItem item in ElementItems.Where(a => a is MenuItem))
                    {
                        item.IsOpen = false;
                    }
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<MenuItem>)typeof(MenuItem)));
            overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)new StackPanel { }));
            overridePropertys.Override(nameof(HeaderTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)typeof(ContentTemplate)));
            overridePropertys.Override(nameof(MarginLeft), new UIPropertyMetadataAttribute(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure));
        }
        protected override void Dispose(bool disposing)
        {
            if (popup != null)
            {
                popup.Dispose();
            }
            popupPanel.Dispose();
            base.Dispose(disposing);
        }
    }
}
