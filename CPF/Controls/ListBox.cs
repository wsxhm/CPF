using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 包含可选项列表
    /// </summary>
    [Description("包含可选项列表"), Browsable(true)]
    public class ListBox : MultiSelector<ListBoxItem>
    {
        public ListBox()
        {
            isVirtualizing = IsVirtualizing;
            SelectedIndexs.CollectionChanged += SelectedIndexs_CollectionChanged;
        }

        private void SelectedIndexs_CollectionChanged(object sender, CollectionChangedEventArgs<int> e)
        {
            if (ItemsHost == null)
            {
                return;
            }
            var items = Items;
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    if (isVirtualizing)
                    {
                        //if (presenter)
                        //{
                        //    if (e.Item >= presenter.StartId && e.Item <= presenter.StartId + presenter.Count)
                        //    {
                        //        var i = InnerPanel.Children.Select(a => a as ListBoxItem).FirstOrDefault(a => a.Index == e.Item);
                        //        if (i != null)
                        //        {
                        //            i.IsSelected = true;
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        if (e.NewItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.NewItem] as ListBoxItem);
                            item.IsSetOnOwner = true;
                            item.IsSelected = true;
                            item.IsSetOnOwner = false;
                        }
                    }
                    break;
                case CollectionChangedAction.Remove:
                    if (isVirtualizing)
                    {
                        //if (presenter)
                        //{
                        //    if (e.Item >= presenter.StartId && e.Item <= presenter.StartId + presenter.Count)
                        //    {
                        //        var i = InnerPanel.Children.Select(a => a as ListBoxItem).FirstOrDefault(a => a.Index == e.Item);
                        //        if (i != null)
                        //        {
                        //            i.IsSelected = false;
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        if (e.OldItem < items.Count && ItemsHost.Children.Count > e.OldItem)
                        {
                            var item = (ItemsHost.Children[e.OldItem] as ListBoxItem);
                            item.IsSetOnOwner = true;
                            item.IsSelected = false;
                            item.IsSetOnOwner = false;
                        }
                    }
                    break;
                case CollectionChangedAction.Replace:
                    if (!isVirtualizing)
                    {
                        if (e.NewItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.NewItem] as ListBoxItem);
                            item.IsSetOnOwner = true;
                            item.IsSelected = true;
                            item.IsSetOnOwner = false;
                        }
                        if (e.OldItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.OldItem] as ListBoxItem);
                            item.IsSetOnOwner = true;
                            item.IsSelected = false;
                            item.IsSetOnOwner = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 选择行为，单选，多选方式
        /// </summary>
        [PropertyMetadata(SelectionMode.Single), Description("选择行为，单选，多选方式")]
        public SelectionMode SelectionMode
        {
            get { return GetValue<SelectionMode>(); }
            set { SetValue(value); }
        }


        bool isVirtualizing;
        /// <summary>
        /// 是否虚拟化UI，只支持StackPanel的虚拟化数据显示。初始化之前设置
        /// </summary>
        [Description("是否虚拟化UI，只支持StackPanel的虚拟化数据显示。初始化之前设置")]
        public bool IsVirtualizing
        {
            get { return GetValue<bool>(); }
            set { isVirtualizing = value; SetValue(value); }
        }
        /// <summary>
        /// 虚拟模式下元素使用方式
        /// </summary>
        [PropertyMetadata(VirtualizationMode.Standard), Description("虚拟模式下元素使用方式")]
        public VirtualizationMode VirtualizationMode
        {
            get { return GetValue<VirtualizationMode>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 鼠标选中方式
        /// </summary>
        [PropertyMetadata(SelectionMethod.MouseDown), Description("鼠标选中方式")]
        public SelectionMethod SelectionMethod
        {
            get { return GetValue<SelectionMethod>(); }
            set { SetValue(value); }
        }
        ///// <summary>
        ///// 定义数据类型关联的模板，初始化或者附加到可视化树之前设置
        ///// </summary>
        //public UIElementTemplate<ListBoxItem> ItemTemplate
        //{
        //    get { return GetValue<UIElementTemplate<ListBoxItem>>(); }
        //    set { SetValue(value); }
        //}

        //public override ListBoxItem CreateItemElement()
        //{
        //    return ItemTemplate.CreateElement();
        //}
        protected override void InitializeComponent()
        {
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.MinHeight = "100%";
            panel.MinWidth = "100%";
            panel.MarginLeft = 0;
            panel.MarginTop = 0;
            if (IsVirtualizing)
            {
                Children.Add(new ScrollViewer { Width = "100%", Height = "100%", Content = new VirtualizationPresenter<ListBoxItem> { Child = panel, PresenterFor = this }, });
            }
            else
            {
                Children.Add(new ScrollViewer { Width = "100%", Height = "100%", Content = panel, });
            }
        }

        VirtualizationPresenter<ListBoxItem> presenter;
        //Panel InnerPanel;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (isVirtualizing)
            {
                presenter = FindPresenter<VirtualizationPresenter<ListBoxItem>>().FirstOrDefault();
                if (!presenter)
                {
                    throw new Exception("虚拟模式下未找到VirtualizationPresenter");
                }
                presenter.Items = Items;
                presenter.MultiSelector = this;
                if (Items.Count > 0)
                {
                    presenter.OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Items));
                }
            }
            else
            {
                if (SelectedIndexs.Count > 0)
                {
                    var itemHost = ItemsHost;
                    foreach (var item in SelectedIndexs)
                    {
                        if (item < itemHost.Children.Count)
                        {
                            var i = ((ListBoxItem)itemHost.Children[item]);
                            i.IsSetOnOwner = true;
                            i.IsSelected = true;
                            i.IsSetOnOwner = false;
                        }
                    }
                }
            }
            //InnerPanel = GetItemsPanel();
            //if (InnerPanel == null)
            //{
            //    throw new Exception("未定义ItemsPanel");
            //}
            //foreach (UIElement item in ItemsHost.Children)
            //{
            //    item.MouseDown += Element_MouseDown;
            //}
            //ItemsHost.UIElementAdded += InnerPanel_UIElementAdded;
            //ItemsHost.UIElementRemoved += InnerPanel_UIElementRemoved;
            ItemsHost.LayoutUpdated += ItemsHost_LayoutUpdated;
            ItemsHost.PreviewMouseDown += ItemsHost_MouseDown;
            ItemsHost.PreviewMouseUp += ItemsHost_MouseUp;
        }

        Point? touch;
        bool isTouchSelect;
        private void ItemsHost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.IsTouch && SelectionMode == SelectionMode.Single)
            {
                if (touch == e.Location)
                {
                    isTouchSelect = true;
                }
                touch = null;
            }
        }

        private void ItemsHost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isTouchSelect = false;
            if (e.IsTouch)
            {
                touch = e.Location;
            }
        }

        //protected override bool IsItemElement(object item)
        //{
        //    return item is ListBoxItem;
        //}
        protected override void OnItemElementAdded(UIElement element)
        {
            if (element is ListBoxItem item)
            {
                item.ListBoxOwner = this;
                if (item.IsSelected)
                {
                    needUpdateSelect = true;
                }
            }
            //element.MouseDown += Element_MouseDown;
            //element.MouseUp += Element_MouseUp;
        }

        protected override void OnItemElementRemoved(UIElement element)
        {
            if (element is ListBoxItem item)
            {
                item.ListBoxOwner = null;
                if (item.IsSelected)
                {
                    needUpdateSelect = true;
                }
            }
            //element.MouseDown -= Element_MouseDown;
            //element.MouseUp -= Element_MouseUp;
        }

        //private void Element_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (SelectionMethod == SelectionMethod.MouseUp && (!e.IsTouch || isTouchSelect))
        //    {
        //        SelectItem(sender);
        //    }
        //}

        //private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (SelectionMethod == SelectionMethod.MouseDown)
        //    {
        //        SelectItem(sender);
        //    }
        //}

        private void SelectItem(object sender)
        {
            var item = sender as ListBoxItem;
            switch (SelectionMode)
            {
                case SelectionMode.Extended:
                    if (ItemsHost.Root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Control))
                    {
                        if (!item.IsSelected)
                        {
                            SelectedIndexs.Add(item.Index);
                        }
                        else
                        {
                            SelectedIndexs.Remove(item.Index);
                        }
                        shiftLastId = item.Index;
                    }
                    else if (ItemsHost.Root.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Shift))
                    {
                        if (SelectedIndexs.Count == 0 || shiftLastId == -1)
                        {
                            SelectedIndexs.Add(item.Index);
                            shiftLastId = item.Index;
                        }
                        else
                        {
                            var index = shiftLastId;
                            if (index != item.Index)
                            {
                                SelectedIndexs.Clear();
                                var min = Math.Min(index, item.Index);
                                var max = Math.Max(index, item.Index);
                                for (int i = min; i <= max; i++)
                                {
                                    SelectedIndexs.Add(i);
                                }
                            }
                        }
                    }
                    else
                    {
                        SelectedIndexs.Clear();
                        SelectedIndexs.Add(item.Index);
                        shiftLastId = item.Index;
                    }
                    break;
                case SelectionMode.Multiple:
                    if (!item.IsSelected)
                    {
                        SelectedIndexs.Add(item.Index);
                    }
                    else
                    {
                        SelectedIndexs.Remove(item.Index);
                    }
                    break;
                case SelectionMode.Single:
                    SelectedIndexs.Clear();
                    SelectedIndexs.Add(item.Index);
                    break;
            }
        }

        bool needUpdateSelect;
        bool updateIndex = true;
        private void ItemsHost_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            if (updateIndex)
            {
                updateIndex = false;
                if (!isVirtualizing)
                {
                    var ac = AlternationCount;
                    var c = ItemsHost.Children;
                    List<int> indexs = null;
                    if (needUpdateSelect)
                    {
                        indexs = new List<int>();
                    }
                    for (int i = 0; i < c.Count; i++)
                    {
                        var item = c[i] as ListBoxItem;
                        if (ac != 0)
                        {
                            SetAlternationIndex(item, (int)(i % ac));
                        }
                        else
                        {
                            SetAlternationIndex(item, i);
                        }
                        item.Index = i;
                        if (indexs != null && item.IsSelected)
                        {
                            indexs.Add(i);
                        }
                    }
                    if (indexs != null)
                    {
                        SelectedIndexs.Clear();
                        SelectedIndexs.AddRange(indexs);
                    }
                    needUpdateSelect = false;
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<ListBoxItem>)new ListBoxItem()));
            overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)new StackPanel { Width = "100%", }));
        }

        [PropertyChanged(nameof(Items))]
        void RegisterItems(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (presenter)
            {
                presenter.Items = newValue as IList;
            }
        }

        [PropertyChanged(nameof(ItemTemplate))]
        void RegisterItemTemplate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (newValue == null)
            {
                throw new Exception(nameof(ItemTemplate) + "不能为null");
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(Items) && presenter)
        //    {
        //        presenter.Items = newValue as IList;
        //    }
        //    else if (propertyName == nameof(ItemTemplate) && newValue == null)
        //    {
        //        throw new Exception(nameof(ItemTemplate) + "不能为null");
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);

        //}
        protected override void OnCollectionSorted()
        {
            if (presenter)
            {
                presenter.OnCollectionSorted();
            }
            else
            {
                base.OnCollectionSorted();
            }
        }

        protected override void OnDataCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (isVirtualizing)
            {
                if (presenter)
                {
                    presenter.OnDataCollectionChanged(e);
                }
            }
            else
            {
                updateIndex = true;
                base.OnDataCollectionChanged(e);
            }
        }


        int shiftLastId = -1;




        internal protected virtual void OnItemMouseDown(ListBoxItemMouseEventArgs args)
        {
            if (SelectionMethod == SelectionMethod.MouseDown)
            {
                SelectItem(args.Item);
            }
            RaiseEvent(args, nameof(ItemMouseDown));
        }

        public event EventHandler<ListBoxItemMouseEventArgs> ItemMouseDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnItemMouseUp(ListBoxItemMouseEventArgs args)
        {
            if (SelectionMethod == SelectionMethod.MouseUp && (!args.IsTouch || isTouchSelect))
            {
                SelectItem(args.Item);
            }
            RaiseEvent(args, nameof(ItemMouseUp));
        }

        public event EventHandler<ListBoxItemMouseEventArgs> ItemMouseUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnItemDoubleClick(ListBoxItemEventArgs args)
        {
            RaiseEvent(args, nameof(ItemDoubleClick));
        }

        public event EventHandler<ListBoxItemEventArgs> ItemDoubleClick
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
    }
    /// <summary>
    /// 选中方式
    /// </summary>
    public enum SelectionMethod : byte
    {
        MouseDown,
        MouseUp,
    }

    public class ListBoxItemMouseEventArgs : MouseButtonEventArgs
    {
        public ListBoxItemMouseEventArgs(ListBoxItem item, UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, MouseDevice mouseDevice, MouseButton mouseButton, bool isTouch) : base(source, LeftButtonDown, RightButtonDown, MiddleButtonDown, location, mouseDevice, mouseButton, isTouch)
        {
            Item = item;
        }

        public ListBoxItem Item { get; internal set; }
    }
    public class ListBoxItemEventArgs : RoutedEventArgs
    {
        public ListBoxItemEventArgs(ListBoxItem item)
        {
            Item = item;
        }

        public ListBoxItem Item { get; internal set; }
    }
}
