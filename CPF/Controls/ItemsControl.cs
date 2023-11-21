using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using CPF.Input;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示可用于呈现一组项的控件。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Description("表示可用于呈现一组项的控件。"), Browsable(false)]
    public abstract class ItemsControl<T> : Control where T : UIElement
    {
        public ItemsControl()
        {
            Items = new ItemCollection { };
        }
        /// <summary>
        /// Item模板
        /// </summary>
        [Description("Item模板")]
        public UIElementTemplate<T> ItemTemplate
        {
            get { return GetValue<UIElementTemplate<T>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 创建模板的元素
        /// </summary>
        /// <returns></returns>
        public virtual T CreateItemElement()
        {
            return ItemTemplate.CreateElement();
        }

        /// <summary>
        /// 返回CPF.Controls.ItemCollection类型，可以直接将数据源设置过来
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsMeasure), Description("返回CPF.Controls.ItemCollection类型，可以直接将数据源设置过来")]
        public IList Items { get { return GetValue<IList>(); } set { SetValue(value); } }

        /// <summary>
        /// 定义布局容器，初始化或者附加到可视化树之前设置
        /// </summary>
        [Description("定义布局容器，初始化或者附加到可视化树之前设置")]
        public virtual UIElementTemplate<Panel> ItemsPanel
        {
            get { return GetValue<UIElementTemplate<Panel>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取一个值，该值指示 ItemsControl 是否包含项。
        /// </summary>
        [Description("获取一个值，该值指示 ItemsControl 是否包含项。")]
        public bool HasItems
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }
        bool hasSet = false;
        /// <summary>
        /// 显示的数据字段或属性
        /// </summary>
        [PropertyMetadata(""), Description("显示的数据字段或属性")]
        public string DisplayMemberPath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 获取或设置 ItemsControl 中的交替项容器的数目，该控件可使交替容器具有唯一外观，通过附加数据AttachedExtenstions.AlternationIndex 读取循环的ID
        /// </summary>
        [Description("获取或设置 ItemsControl 中的交替项容器的数目，该控件可使交替容器具有唯一外观，通过附加数据AttachedExtenstions.AlternationIndex 读取循环的ID")]
        public uint AlternationCount
        {
            get { return GetValue<uint>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 当使用交替项目容器时，设置项目容器的分配值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public void SetAlternationIndex(CpfObject obj, int value)
        {
            AttachedExtenstions.AlternationIndex(obj, value);
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Items))
            {
                //if (value == null)
                //{
                //    throw new Exception("Items不能为null");
                //}
                if (value != null)
                {
                    if (!(value is ItemCollection))
                    {
                        var n = value as INotifyCollectionChanged;
                        if (n != null)
                        {
                            var ic = new ItemCollection(n);
                            value = ic;
                            var items = Items;
                            if ((items != null && items.Count > 0) || (panel && panel.Children.Count > 0))
                            {
                                OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            }
                            //OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, n));
                        }
                        else
                        {
                            var items = Items;
                            if (items == null)
                            {
                                items = new ItemCollection();
                                OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            }
                            var ic = items;
                            if (ic.Count > 0)
                            {
                                ic.Clear();
                                //OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            }
                            //ic.Clear();
                            //ic.Add(value as IEnumerable);
                            if (value is IEnumerable enumer)
                            {
                                foreach (var item in enumer)
                                {
                                    ic.Add(item);
                                    //OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                                }
                            }
                            value = ic;
                        }
                    }
                }
                else
                {
                    var ic = Items;
                    if (ic == null)
                    {
                        ic = new ItemCollection { owner = this };
                    }
                    else
                    {
                        ic = GetValue(nameof(Items)) as ItemCollection;
                        ic.Clear();
                    }
                    value = ic;
                }
            }
            else if (propertyName == nameof(ItemsPanel))
            {
                if (value == null)
                {
                    throw new Exception("ItemsPanel不能为空");
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        protected override void OnDetachedFromVisualTree()
        {
            base.OnDetachedFromVisualTree();
            if (Items == null)
            {
                Items = new ItemCollection { };
            }
        }


        Panel panel;
        /// <summary>
        /// 获取作为Item的UIElement
        /// </summary>
        public IEnumerable<UIElement> ElementItems
        {
            get
            {
                if (panel)
                {
                    return panel.Children;
                }
                return new UIElement[] { };
            }
        }
        /// <summary>
        /// 存放Items的Panel
        /// </summary>
        public Panel ItemsHost
        {
            get { return panel; }
        }

        [PropertyChanged(nameof(Items))]
        void OnItems(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (newValue != null)
            {
                var list = newValue as IList;
                if (list.Count > 0)
                {
                    OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
                }

                var collection = ((ItemCollection)newValue);
                collection.owner = this;
                collection.CollectionChanged += Data_CollectionChanged;
                collection.Sorted += Collection_Sorted;
                collection.StartSort += Collection_StartSort;
            }

            if (oldValue != null)
            {
                ((ItemCollection)oldValue).CollectionChanged -= Data_CollectionChanged;
                ((ItemCollection)oldValue).Sorted -= Collection_Sorted;
                ((ItemCollection)oldValue).StartSort -= Collection_StartSort;
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(Items))
        //    {
        //        //if (panel != null)
        //        //{
        //        //    panel.Children.Clear();
        //        //}
        //        var list = newValue as IList;
        //        if (list.Count > 0)
        //        {
        //            OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
        //        }

        //        var collection = ((ItemCollection)newValue);
        //        collection.CollectionChanged += Data_CollectionChanged;

        //        if (oldValue != null)
        //        {
        //            ((ItemCollection)oldValue).CollectionChanged -= Data_CollectionChanged;
        //        }
        //    }
        //}

        protected override void InitializeComponent()
        {
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.MinHeight = "100%";
            panel.MinWidth = "100%";
            Children.Add(new ScrollViewer { Width = "100%", Height = "100%", Content = panel, });
        }


        protected virtual Panel GetItemsPanel()
        {
            return FindPresenter<Panel>().FirstOrDefault(a => a.Name == "itemsPanel");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            panel = GetItemsPanel();
            //if (panel == null)
            //{
            //    throw new Exception("未定义itemsPanel");
            //}
            if (panel != null)
            {
                if (panel.Children.Count > 0)
                {
                    foreach (var item in panel.Children)
                    {
                        OnItemElementAdded(item);
                    }
                }
                panel.UIElementAdded += Panel_UIElementAdded;
                panel.UIElementRemoved += Panel_UIElementRemoved;
            }
            var items = Items;
            if (items != null && items.Count > 0)
            {
                OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
            }
        }

        private void Panel_UIElementRemoved(object sender, UIElementRemovedEventArgs e)
        {
            OnItemElementRemoved(e.Element);
        }

        private void Panel_UIElementAdded(object sender, UIElementAddedEventArgs e)
        {
            OnItemElementAdded(e.Element);
        }

        protected virtual void OnItemElementAdded(UIElement element)
        {

        }
        protected virtual void OnItemElementRemoved(UIElement element)
        {

        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnDataCollectionChanged(e);
        }
        /// <summary>
        /// 判断是否是当前容器的特殊元素，比如TreeView中的TreeViewItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool IsItemElement(object item)
        {
            return item is T;
        }
        /// <summary>
        /// 判断该元素是否在Item里，如果是则返回item，否则为null。比如，可以在ListBox的MouseDown里判断点击到的Item，参数用事件里的OriginalSource
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public T IsInItem(UIElement element)
        {
            if (element == null)
            {
                return null;
            }
            if (element is T item && item.Parent == panel)
            {
                return item;
            }
            var parent = element.Parent;
            while (parent != null)
            {
                if (parent is T i && i.Parent == panel)
                {
                    return i;
                }
                parent = parent.Parent;
            }
            return null;
        }


        private void Collection_StartSort(object sender, EventArgs e)
        {
            OnCollectionStartSort();
        }

        private void Collection_Sorted(object sender, EventArgs e)
        {
            OnCollectionSorted();
        }

        protected virtual void OnCollectionStartSort()
        {

        }

        protected virtual void OnCollectionSorted()
        {
            if (panel)
            {
                var old = panel.Children.ElementList.Select(a => new KeyValue<UIElement, object>(a, a.DataContext)).ToArray();
                var children = panel.Children.ElementList;
                var items = Items;
                for (int i = 0; i < old.Length; i++)
                {
                    var item = items[i];
                    for (int j = 0; j < old.Length; j++)
                    {
                        var kv = old[j];
                        if (kv.Value == item || (IsItemElement(item) && item == kv.Key))
                        {
                            children[i] = kv.Key;
                            break;
                        }
                    }
                }
                panel.InvalidateArrange();
            }
        }

        protected virtual void OnDataCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (panel)
            {
                var children = panel.Children;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        var dis = DisplayMemberPath;
                        var index = 0;
                        foreach (var item in e.NewItems)
                        {
                            if (IsItemElement(item))
                            {
                                if (e.NewStartingIndex < 0)
                                {
                                    children.Add((UIElement)item);
                                }
                                else
                                {
                                    children.Insert(e.NewStartingIndex + index, (UIElement)item);
                                }
                            }
                            else
                            {
                                var i = CreateItemElement();
                                i.needDispose = true;
                                if (i is ContentControl cc)
                                {
                                    if (!string.IsNullOrWhiteSpace(dis))
                                    {
                                        cc.Content = item.GetPropretyValue(dis);
                                    }
                                    else
                                    {
                                        cc.Content = item;
                                    }
                                }
                                i.DataContext = item;
                                if (e.NewStartingIndex < 0)
                                {
                                    children.Add(i);
                                }
                                else
                                {
                                    children.Insert(e.NewStartingIndex + index, i);
                                }
                            }
                            index++;
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        var move = children[e.OldStartingIndex];
                        children.Remove(move);
                        children.Insert(e.NewStartingIndex, move);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        children.RemoveAt(e.OldStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        var data = e.NewItems[0];
                        if (IsItemElement(data))
                        {
                            children[e.OldStartingIndex] = (UIElement)data;
                        }
                        else
                        {
                            var n = children[e.OldStartingIndex];
                            if (n is ContentControl c)
                            {
                                var diss = DisplayMemberPath;
                                if (!string.IsNullOrWhiteSpace(diss))
                                {
                                    c.Content = e.NewItems[0].GetPropretyValue(diss);
                                }
                                else
                                {
                                    c.Content = e.NewItems[0];
                                }
                            }
                            n.DataContext = e.NewItems[0];
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        children.Clear();
                        break;
                }
            }
            if (!hasSet)
            {
                hasSet = true;
                BeginInvoke(() =>
                {
                    hasSet = false;
                    HasItems = Items.Count > 0;
                });
            }
        }

        //public override object Clone()
        //{
        //    var i = base.Clone() as ItemsControl<T>;
        //    i.Items = new ItemCollection();
        //    //i.HasItems = false;
        //    return i;
        //}
    }
}
