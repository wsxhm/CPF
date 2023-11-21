using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 右键菜单
    /// </summary>
    [Description("右键菜单"), Browsable(false)]
    public class ContextMenu : ItemsControl<MenuItem>
    {
        ///// <summary>
        ///// 定义数据类型关联的模板，初始化或者附加到可视化树之前设置
        ///// </summary>
        //public UIElementTemplate<MenuItem> ItemTemplate
        //{
        //    get { return GetValue<UIElementTemplate<MenuItem>>(); }
        //    set { SetValue(value); }
        //}
        //public override MenuItem CreateItemElement()
        //{
        //    return ItemTemplate.CreateElement();
        //}

        Popup popup;
        [NotCpfProperty]
        public Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = new Popup { StaysOpen = false, Name = "MenuPop" };
                    popup.LostFocus += Popup_LostFocus;
                    popup.Children.Add(this);
                    popup.Background = null;
                    //popup.LayoutUpdated += Popup_LayoutUpdated;
                    var b1 = popup[nameof(PlacementTarget)] <= this[nameof(PlacementTarget)];
                    var b2 = popup[nameof(Placement)] <= this[nameof(Placement)];
                    //var b3 = this[nameof(IsOpen), a => (bool)a ? Visibility.Visible : Visibility.Collapsed] == popup[nameof(Visibility), a => (Visibility)a == Visibility.Visible];
                    this[nameof(IsOpen)] = (popup, nameof(Visibility), BindingMode.TwoWay, a => (Visibility)a == Visibility.Visible, a => (bool)a ? Visibility.Visible : Visibility.Collapsed);
                    var b4 = popup[nameof(MarginBottom)] <= this[nameof(PopupMarginBottm)];
                    var b5 = popup[nameof(MarginLeft)] <= this[nameof(PopupMarginLeft)];
                    var b6 = popup[nameof(MarginRight)] <= this[nameof(PopupMarginRight)];
                    var b7 = popup[nameof(MarginTop)] <= this[nameof(PopupMarginTop)];
                }
                return popup;
            }
        }

        //private void Popup_LayoutUpdated(object sender, RoutedEventArgs e)
        //{
        //    BeginInvoke(() =>
        //    {
        //        popup.Size = "auto,auto";
        //    });
        //}

        public ContextMenu()
        {

        }

        private void Popup_LostFocus(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        protected override void InitializeComponent()
        {
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.MarginLeft = 0;
            panel.Width = "100%";
            Children.Add(new Border { Width = "100%", Height = "100%", Child = panel, ShadowBlur = 4, Background = "#eee", BorderFill = "#999" });
        }

        /// <summary>
        /// 获取或设置是否显示，通过这个属性来显示和隐藏
        /// </summary>
        public bool IsOpen
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 获取或设置当打开 Popup 控件时该控件相对于其放置的元素。
        /// </summary>
        public UIElement PlacementTarget
        {
            get { return GetValue<UIElement>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置 Popup 控件打开时的控件方向，并指定 Popup 控件在与屏幕边界重叠时的控件行为
        /// </summary>
        [PropertyMetadata(PlacementMode.Mouse)]
        public PlacementMode Placement
        {
            get { return GetValue<PlacementMode>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 这里不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Visibility Visibility { get => base.Visibility; set => base.Visibility = value; }
        /// <summary>
        /// 这里不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override FloatField MarginBottom { get => base.MarginBottom; set => base.MarginBottom = value; }
        /// <summary>
        /// 这里不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override FloatField MarginLeft { get => base.MarginLeft; set => base.MarginLeft = value; }
        /// <summary>
        /// 这里不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override FloatField MarginTop { get => base.MarginTop; set => base.MarginTop = value; }
        /// <summary>
        /// 这里不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override FloatField MarginRight { get => base.MarginRight; set => base.MarginRight = value; }
        /// <summary>
        /// 这里不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), NotCpfProperty]
        public override ThicknessField Margin { get => base.Margin; set => base.Margin = value; }

        /// <summary>
        /// 弹出时候的定位 默认值 1
        /// </summary>
        [PropertyMetadata(typeof(FloatField), "1")]
        public FloatField PopupMarginLeft { get { return GetValue<FloatField>(); } set { SetValue(value); } }
        /// <summary>
        /// 弹出时候的定位
        /// </summary>
        public FloatField PopupMarginRight { get { return GetValue<FloatField>(); } set { SetValue(value); } }
        /// <summary>
        /// 弹出时候的定位 默认值 1
        /// </summary>
        [PropertyMetadata(typeof(FloatField), "1")]
        public FloatField PopupMarginTop { get { return GetValue<FloatField>(); } set { SetValue(value); } }
        /// <summary>
        /// 弹出时候的定位
        /// </summary>
        public FloatField PopupMarginBottm { get { return GetValue<FloatField>(); } set { SetValue(value); } }


        protected override bool IsItemElement(object item)
        {
            return item is MenuItem || item is Separator;
        }
        protected override void OnItemElementAdded(UIElement element)
        {
            if (element is MenuItem menuItem)
            {
                menuItem.OwnerContextMenu = this;
                menuItem.ParentItem = this;
                menuItem.PropertyChanged += MenuItem_PropertyChanged;
            }
            if (popup)
            {
                popup.Size = "auto,auto";
                //popup.InvalidateMeasure();
            }
        }

        private void MenuItem_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Visibility))
            {
                if (popup)
                {
                    popup.Size = "auto,auto";
                }
            }
        }

        protected override void OnItemElementRemoved(UIElement element)
        {
            if (element is MenuItem menuItem)
            {
                menuItem.OwnerContextMenu = null;
                menuItem.ParentItem = null;
                menuItem.PropertyChanged -= MenuItem_PropertyChanged;
            }
            if (popup)
            {
                popup.Size = "auto,auto";
                //popup.InvalidateMeasure();
            }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(IsOpen))
            {
                var p = Popup;
                p.CanActivate = PlacementTarget == null;
            }
            return base.OnSetValue(propertyName, ref value);
        }

        [PropertyChanged(nameof(IsOpen))]
        void RegisterIsOpen(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var v = (bool)newValue;
            var taget = PlacementTarget;
            if (v)
            {
                if (taget)
                {
                    popup.LoadStyle(taget.Root);
                    popup.DataContext = DataContext;
                    popup.CommandContext = CommandContext;
                }
            }
            else
            {
                foreach (MenuItem item in ElementItems.Where(a => a is MenuItem))
                {
                    item.IsOpen = false;
                }
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(IsOpen))
        //    {
        //        var v = (bool)newValue;
        //        var taget = PlacementTarget;
        //        if (v)
        //        {
        //            if (taget)
        //            {
        //                popup.styleSheet = taget.Root.styleSheet;
        //            }
        //        }
        //        else
        //        {
        //            foreach (MenuItem item in ElementItems.Where(a => a is MenuItem))
        //            {
        //                item.IsOpen = false;
        //            }
        //        }
        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<MenuItem>)typeof(MenuItem)));
            overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)new StackPanel { }));
            //overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"#eee", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Width), new UIPropertyMetadataAttribute(typeof(FloatField), "150", UIPropertyOptions.AffectsMeasure));
        }

        protected override void Dispose(bool disposing)
        {
            if (popup != null)
            {
                popup.Children.Remove(this);
                popup.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}
