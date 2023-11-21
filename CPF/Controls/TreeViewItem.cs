using System;
using System.Collections.Generic;
using System.Text;
using CPF.Styling;
using CPF.Shapes;
using System.Linq;
using System.ComponentModel;
using CPF.Input;

namespace CPF.Controls
{
    /// <summary>
    /// 在 TreeView 控件中实现可选择的项。
    /// </summary>
    [Description("在 TreeView 控件中实现可选择的项。"), Browsable(false)]
    public class TreeViewItem : ItemsControl<TreeViewItem>, IHeadered
    {
        /// <summary>
        /// 获取或设置 TreeViewItem 中的嵌套项是处于展开状态还是折叠状态。
        /// </summary>
        public bool IsExpanded
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 获取，该值指示是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            internal set { SetValue(value); }
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

        ///// <summary>
        ///// 定义数据类型关联的模板，初始化或者附加到可视化树之前设置
        ///// </summary>
        //public UIElementTemplate<TreeViewItem> ItemTemplate
        //{
        //    get { return GetValue<UIElementTemplate<TreeViewItem>>(); }
        //    set { SetValue(value); }
        //}

        /// <summary>
        /// 获取或设置用于显示控件标头的内容的模板。
        /// </summary>
        [Browsable(false)]
        public UIElementTemplate<ContentTemplate> HeaderTemplate
        {
            get { return GetValue<UIElementTemplate<ContentTemplate>>(); }
            set { SetValue(value); }
        }

        public override TreeViewItem CreateItemElement()
        {
            var item = ItemTemplate.CreateElement();
            item.TreeView = TreeView;
            return item;
        }
        ///// <summary>
        ///// 获取或设置源对象上某个值的路径，该值作为对象的可视化表示形式。
        ///// </summary>
        //[PropertyMetadata("")]
        //public string DisplayMemberPath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 获取或设置源对象上某个值的路径，该值作为对象的子节点。
        /// </summary>
        [PropertyMetadata("")]
        public string ItemsMemberPath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {
            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var b1 = this[nameof(Header)] <= DisplayMemberPath;
            }
            if (!string.IsNullOrWhiteSpace(ItemsMemberPath))
            {
                var b2 = this[nameof(Items)] <= ItemsMemberPath;
            }


            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.MarginLeft = 20;
            panel[nameof(Visibility)] = (this, nameof(IsExpanded), a => (bool)a ? Visibility.Visible : Visibility.Collapsed);
            Children.Add(new StackPanel
            {
                MarginLeft = 0,
                Orientation = Orientation.Vertical,
                Name = "mainPanel",
                PresenterFor = this,
                Children = {
                   new StackPanel
                   {
                       Name="treeViewItem",
                       PresenterFor=this,
                       MarginLeft=0,
                       Orientation= Orientation.Horizontal,
                       Children={
                            new Panel
                            {
                                MarginLeft=3,
                                Width=12,
                                Children =
                                {
                                    new Polygon
                                    {
                                        IsAntiAlias=true,
                                        RenderTransformOrigin=new PointField("30%","70%"),
                                        Points={new Drawing.Point(2,2),new Drawing.Point(2,10),new Drawing.Point(6,6), },
                                        Bindings={
                                            { nameof(Polygon.RenderTransform),nameof(IsExpanded),this,BindingMode.OneWay,a=>(bool)a?new RotateTransform(45):Transform.Identity},
                                        }
                                    }
                                },
                                Bindings =
                                {
                                    { nameof(Visibility),nameof(HasItems),this,BindingMode.OneWay,a=>(bool)a?Visibility.Visible:Visibility.Hidden }
                                },
                                Commands=
                                {
                                    {nameof(MouseDown),(s,e)=> { ((RoutedEventArgs)e).Handled = true; IsExpanded = !IsExpanded; } }
                                },
                                Triggers=
                                {
                                     new Trigger{ Property=nameof(IsMouseOver), TargetRelation= Relation.Me.Children(a=>a is Polygon), Setters = { {nameof(Shape.StrokeFill),"4,124,205" } } }
                                }
                            },
                            new ContentControl{MarginLeft=3 ,Bindings={ {nameof(ContentControl.Content),nameof(TreeViewItem.Header),3 }, {nameof(ContentControl.ContentTemplate),nameof(TreeViewItem.HeaderTemplate),3 } } }
                       },
                       Triggers={
                           new Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a && !IsSelected, Setters = { { nameof(Background), "232,242,252" } } },

                       },
                       Commands={
                           {nameof(MouseDown),(s,e)=> { SingleSelect(); } },
                           {nameof(DoubleClick),(s,e)=>IsExpanded=!IsExpanded }
                       },
                   },
                   IsExpanded? panel:null,//如果展开的话，就直接加入到容器里，否则等待到展开的时候再加入到容器。提高性能。
                },
            });

            Triggers.Add(new Trigger { Property = nameof(IsSelected), TargetRelation = Relation.Me.Find(a => a.Name == "treeViewItem" && a.PresenterFor == this), Setters = { { nameof(Background), "203,233,246" } } });
        }

        [PropertyChanged(nameof(IsExpanded))]
        void OnIsExpanded(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var itemsPanel = FindPresenterByName<Panel>("itemsPanel");
            if ((bool)newValue && itemsPanel && itemsPanel.Root == null)
            {
                var mainPanel = FindPresenterByName<Panel>("mainPanel");
                if (mainPanel)
                {
                    mainPanel.Children.Add(itemsPanel);
                }
            }
        }

        TreeView treeView;
        [NotCpfProperty]
        public TreeView TreeView
        {
            get { return treeView; }
            internal set
            {
                if (treeView != value)
                {
                    treeView = value;
                    foreach (TreeViewItem item in ElementItems)
                    {
                        item.TreeView = value;
                    }
                }
            }
        }
        /// <summary>
        /// 父节点
        /// </summary>
        [NotCpfProperty]
        public ItemsControl<TreeViewItem> ParentItem
        {
            get;
            internal set;
        }
        /// <summary>
        /// 获取 TreeView 控件中的树视图的深度（从零开始）。
        /// </summary>
        public uint Level
        {
            get { return GetValue<uint>(); }
            private set { SetValue(value); }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<TreeViewItem>)typeof(TreeViewItem)));
            overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)new StackPanel { }));
            overridePropertys.Override(nameof(HeaderTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)typeof(TreeViewContentTemplate)));
            overridePropertys.Override(nameof(MarginLeft), new UIPropertyMetadataAttribute((FloatField)0, UIPropertyOptions.AffectsMeasure));
        }

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            if (pm.PropertyName == nameof(ItemTemplate))
            {
                if (treeView)
                {
                    return treeView.ItemTemplate;
                }
            }
            else if (pm.PropertyName == nameof(DisplayMemberPath))
            {
                if (treeView)
                {
                    return treeView.DisplayMemberPath;
                }
            }
            else if (pm.PropertyName == nameof(ItemsMemberPath))
            {
                if (treeView)
                {
                    return treeView.ItemsMemberPath;
                }
            }
            else if (pm.PropertyName == nameof(HeaderTemplate))
            {
                if (treeView)
                {
                    return treeView.HeaderTemplate;
                }
            }
            return base.OnGetDefaultValue(pm);
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            UpdateLevel();
        }

        private void UpdateLevel()
        {
            if (treeView != null)
            {
                var l = 0;
                var p = ParentItem as TreeViewItem;
                while (p != null)
                {
                    p = p.ParentItem as TreeViewItem;
                    l++;
                }
                Level = (uint)l;
            }
        }

        //protected override bool IsItemElement(object item)
        //{
        //    return item is TreeViewItem;
        //}

        /// <summary>
        /// 单选，将当前元素选中，其他元素取消选择
        /// </summary>
        public void SingleSelect()
        {
            if (IsSelected)
            {
                return;
            }
            var tree = TreeView;
            if (tree != null)
            {
                foreach (var item in tree.AllItems().Where(a => a.IsSelected))
                {
                    item.IsSelected = false;
                }
            }
            IsSelected = true;
        }
        /// <summary>
        /// 选中并展开所有父节点
        /// </summary>
        public void ShowMe()
        {
            var tree = TreeView;
            if (tree != null)
            {
                foreach (var item in tree.AllItems().Where(a => a.IsSelected))
                {
                    item.IsSelected = false;
                }
            }
            IsSelected = true;
            var parent = ParentItem;
            while (parent != null)
            {
                if (parent is TreeViewItem item)
                {
                    item.IsExpanded = true;
                    parent = item.ParentItem;
                }
                else
                {
                    parent = null;
                }
            }
        }

        /// <summary>
        /// 展开 TreeViewItem 控件及其所有子 TreeViewItem 元素。
        /// </summary>
        public void ExpandSubtree()
        {
            IsExpanded = true;
            ExpandSubtree(this);
        }
        void ExpandSubtree(TreeViewItem treeViewItem)
        {
            foreach (TreeViewItem item in treeViewItem.ElementItems)
            {
                item.IsExpanded = true;
                ExpandSubtree(item);
            }
        }


        protected override void OnItemElementAdded(UIElement element)
        {
            var item = (element as TreeViewItem);
            item.ParentItem = this;
            item.TreeView = TreeView;
            item.UpdateLevel();
        }
        protected override void OnItemElementRemoved(UIElement element)
        {
            var item = (element as TreeViewItem);
            item.ParentItem = null;
            item.TreeView = null;
        }

        [PropertyChanged(nameof(IsSelected))]
        void RegisterIsSelected(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (ParentItem != null)
            {
                var treeview = TreeView;
                if (treeview != null)
                {
                    treeview.RaiseSelectionChanged();
                }
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(IsSelected))
        //    {
        //        if (ParentItem != null)
        //        {
        //            var treeview = TreeView;
        //            if (treeview != null)
        //            {
        //                treeview.RaiseSelectionChanged();
        //            }
        //        }
        //    }
        //}

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled && treeView)
            {
                var args = new TreeViewItemMouseEventArgs(this, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, e.MouseButton, e.IsTouch);
                treeView.OnItemMouseDown(args);
                e.Handled = args.Handled;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!e.Handled && treeView)
            {
                var args = new TreeViewItemMouseEventArgs(this, (UIElement)e.OriginalSource, e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed, e.Location, e.MouseDevice, e.MouseButton, e.IsTouch);
                treeView.OnItemMouseUp(args);
                e.Handled = args.Handled;
            }
        }

        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            if (!e.Handled && treeView)
            {
                var args = new TreeViewItemEventArgs(this);
                treeView.OnItemDoubleClick(args);
                e.Handled = args.Handled;
            }
        }

    }
}
