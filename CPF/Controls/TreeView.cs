using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using CPF.Input;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个控件，该控件在树结构（其中的项可以展开和折叠）中显示分层数据。
    /// </summary>
    [Description("表示一个控件，该控件在树结构（其中的项可以展开和折叠）中显示分层数据。"), Browsable(true)]
    public class TreeView : ItemsControl<TreeViewItem>
    {
        public TreeView()
        {

        }
        ///// <summary>
        ///// 获取或设置源对象上某个值的路径，该值作为对象的可视化表示形式。
        ///// </summary>
        //[PropertyMetadata("")]
        //public string HeaderMemberPath
        //{
        //    get { return GetValue<string>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 获取或设置用于显示控件标头的内容的模板。
        /// </summary>
        public UIElementTemplate<ContentTemplate> HeaderTemplate
        {
            get { return GetValue<UIElementTemplate<ContentTemplate>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 获取或设置源对象上某个值的路径，该值作为对象的子节点。
        /// </summary>
        [PropertyMetadata("")]
        public string ItemsMemberPath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public TreeViewItem SelectedItem
        {
            get { return GetValue<TreeViewItem>(); }
            private set { SetValue(value); }
        }

        public object SelectedValue
        {
            get { return GetValue(); }
            private set { SetValue(value); }
        }

        ///// <summary>
        ///// 定义数据类型关联的模板，初始化或者附加到可视化树之前设置
        ///// </summary>
        //public UIElementTemplate<TreeViewItem> ItemTemplate
        //{
        //    get { return GetValue<UIElementTemplate<TreeViewItem>>(); }
        //    set { SetValue(value); }
        //}

        public override TreeViewItem CreateItemElement()
        {
            var item = ItemTemplate.CreateElement();
            item.TreeView = this;
            return item;
        }

        protected override void InitializeComponent()
        {
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.MarginTop = 0;
            panel.MarginLeft = 0;
            panel.MinWidth = "100%";
            Children.Add(new ScrollViewer { Width = "100%", Height = "100%", Content = panel, });
        }

        protected override void OnItemElementAdded(UIElement element)
        {
            var item = (element as TreeViewItem);
            item.TreeView = this;
            item.ParentItem = this;
        }
        protected override void OnItemElementRemoved(UIElement element)
        {
            var item = (element as TreeViewItem);
            item.TreeView = null;
            item.ParentItem = null;
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<TreeViewItem>)typeof(TreeViewItem)));
            overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)new StackPanel { }));
            overridePropertys.Override(nameof(HeaderTemplate), new PropertyMetadataAttribute((UIElementTemplate<ContentTemplate>)typeof(TreeViewContentTemplate)));
        }

        /// <summary>
        /// 选择更改时发生
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        bool sc = false;
        internal void RaiseSelectionChanged()
        {
            if (!sc)
            {
                sc = true;
                BeginInvoke(() =>
                {
                    sc = false;
                    OnSelectionChanged(EventArgs.Empty);
                });
            }
        }

        protected virtual void OnSelectionChanged(EventArgs eventArgs)
        {
            var s = AllItems().FirstOrDefault(a => a.IsSelected);
            SelectedItem = s;
            if (s != null)
            {
                SelectedValue = s.DataContext;
            }
            else
            {
                SelectedValue = null;
            }
            this.RaiseEvent(eventArgs, nameof(SelectionChanged));
        }

        //protected override void OnDataCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var item in e.NewItems)
        //        {
        //            if (item is TreeViewItem element)
        //            {
        //                element.TreeView = this;
        //            }
        //        }
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Replace)
        //    {
        //        if (e.NewItems[0] is TreeViewItem treeViewItem)
        //        {
        //            treeViewItem.TreeView = this;
        //        }
        //    }
        //    base.OnDataCollectionChanged(e);
        //}
        //protected override bool IsItemElement(object item)
        //{
        //    return item is TreeViewItem;
        //}
        /// <summary>
        /// 获取所有节点，包括子孙后代
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TreeViewItem> AllItems()
        {
            return GetItems(this);
        }

        IEnumerable<TreeViewItem> GetItems(ItemsControl<TreeViewItem> treeViewItem)
        {
            foreach (TreeViewItem item in treeViewItem.ElementItems.Where(a => a is TreeViewItem))
            {
                yield return item;
                foreach (var i in GetItems(item))
                {
                    yield return i;
                }
            }
        }


        internal protected virtual void OnItemMouseDown(TreeViewItemMouseEventArgs args)
        {
            RaiseEvent(args, nameof(ItemMouseDown));
        }

        public event EventHandler<TreeViewItemMouseEventArgs> ItemMouseDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnItemMouseUp(TreeViewItemMouseEventArgs args)
        {
            RaiseEvent(args, nameof(ItemMouseUp));
        }

        public event EventHandler<TreeViewItemMouseEventArgs> ItemMouseUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnItemDoubleClick(TreeViewItemEventArgs args)
        {
            RaiseEvent(args, nameof(ItemDoubleClick));
        }

        public event EventHandler<TreeViewItemEventArgs> ItemDoubleClick
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

    }
    public class TreeViewItemMouseEventArgs : MouseButtonEventArgs
    {
        public TreeViewItemMouseEventArgs(TreeViewItem item, UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, MouseDevice mouseDevice, MouseButton mouseButton, bool isTouch) : base(source, LeftButtonDown, RightButtonDown, MiddleButtonDown, location, mouseDevice, mouseButton, isTouch)
        {
            Item = item;
        }

        public TreeViewItem Item { get; internal set; }
    }
    public class TreeViewItemEventArgs : RoutedEventArgs
    {
        public TreeViewItemEventArgs(TreeViewItem item)
        {
            Item = item;
        }

        public TreeViewItem Item { get; internal set; }
    }
}
