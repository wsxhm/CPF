using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using CPF.Drawing;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace CPF.Controls
{
    /// <summary>
    /// 虚拟化容器，采用第一个元素来计算元素尺寸，自定义模板最好用固定尺寸的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Description("虚拟化容器")]
    public class VirtualizationPresenter<T> : Decorator, IScrollInfo where T : UIElement, ISelectableItem
    {
        [NotCpfProperty]
        internal protected IList Items { get; set; }
        public MultiSelector<T> MultiSelector { get { return GetValue<MultiSelector<T>>(); } set { SetValue(value); } }

        public event EventHandler ScrollChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        StackPanel stackPanel;
        float widthOrHeight = -1;
        int count;
        int scrollValue = 0;
        bool end = false;
        int startId = 0;
        int _count;
        bool collectChanged;
        List<(object obj, float size)> customData;

        public void OnCollectionSorted()
        {
            if (CustomScrollData != null && CustomScrollData.Custom != null)
            {
                List<(int index, float size)> list = new List<(int index, float size)>();
                foreach (var item in customData)
                {
                    var index = -1;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i] == item.obj)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index > -1)
                    {
                        list.Add((index, item.size));
                    }
                }
                CustomScrollData.Custom = list.OrderBy(a => a.index);
            }
            Scroll(true);
        }

        public void OnCollectionStartSort()
        {
            if (CustomScrollData != null && CustomScrollData.Custom != null)
            {
                if (customData == null)
                {
                    customData = new List<(object obj, float size)>();
                }
                customData.Clear();
                foreach (var item in CustomScrollData.Custom.OrderBy(a => a.index))
                {
                    var _item = Items[item.index];
                    customData.Add((_item, item.size));
                }
            }
        }

        public void OnDataCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (stackPanel)
            {
                if (stackPanel.Children.Count == 0 && Items.Count > 0)
                {
                    var item = multiSelector.CreateItemElement();
                    item.DataContext = Items[0];
                    if (item is ContentControl c)
                    {
                        var diss = multiSelector.DisplayMemberPath;
                        if (!string.IsNullOrWhiteSpace(diss))
                        {
                            c.Content = Items[0].GetPropretyValue(diss);
                        }
                        else
                        {
                            c.Content = Items[0];
                        }
                    }
                    item.Index = 0;
                    stackPanel.Children.Add(item);
                    end = false;
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        //if (e.NewStartingIndex >= startId && e.NewStartingIndex <= startId + count)
                        //{
                        if (Items is ItemCollection collection && collection.comparison != null)
                        {
                            multiSelector.SelectedIndex = -1;
                        }
                        else
                        {
                            if (e.NewStartingIndex >= 0 && multiSelector.SelectedIndexs.Count > 0)
                            {
                                for (int j = 0; j < multiSelector.SelectedIndexs.Count; j++)
                                {
                                    if (multiSelector.SelectedIndexs[j] >= e.NewStartingIndex)
                                    {
                                        multiSelector.SelectedIndexs[j]++;
                                    }
                                }
                            }
                        }
                        if (!collectChanged)
                        {
                            collectChanged = true;
                            BeginInvoke(() =>
                            {
                                collectChanged = false;
                                oldCount = 0;
                                Scroll();
                            });
                        }
                        //}
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        //if (e.OldStartingIndex >= startId && e.OldStartingIndex <= startId + count)
                        //{
                        if (multiSelector.SelectedIndex == e.OldStartingIndex)
                        {
                            multiSelector.SelectedIndex = -1;
                        }
                        if (!collectChanged)
                        {
                            collectChanged = true;
                            BeginInvoke(() =>
                            {
                                collectChanged = false;
                                oldCount = 0;
                                Scroll();
                            });
                        }
                        //}
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        var i = e.NewStartingIndex - startId;
                        if (e.NewStartingIndex >= startId && e.NewStartingIndex <= startId + count && stackPanel.Children.Count > i)
                        {
                            var s = e.NewStartingIndex;
                            T item = null;
                            if (multiSelector.HasProperty(nameof(ListBox.VirtualizationMode)) && multiSelector.GetValue<VirtualizationMode>(nameof(ListBox.VirtualizationMode)) == VirtualizationMode.Recycling)
                            {
                                item = stackPanel.Children[i] as T;
                            }
                            else
                            {
                                stackPanel.Children.RemoveAt(i);
                                item = multiSelector.CreateItemElement();
                                stackPanel.Children.Insert(i, item);
                            }
                            item.DataContext = Items[s];
                            if (item is ContentControl c)
                            {
                                var diss = multiSelector.DisplayMemberPath;
                                if (!string.IsNullOrWhiteSpace(diss))
                                {
                                    c.Content = Items[s].GetPropretyValue(diss);
                                }
                                else
                                {
                                    c.Content = Items[s];
                                }
                            }
                            var ac = multiSelector.AlternationCount;
                            if (ac != 0)
                            {
                                multiSelector.SetAlternationIndex((CpfObject)item, (int)(s % ac));
                            }
                            else
                            {
                                multiSelector.SetAlternationIndex((CpfObject)item, s);
                            }
                            item.Index = s;
                            item.IsSetOnOwner = true;
                            item.IsSelected = multiSelector.SelectedIndexs.Any(a => a == s);
                            item.IsSetOnOwner = false;
                            OnSetItem(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        ComputeItems();
                        break;
                }

            }
        }
        protected override void InitializeComponent()
        {
            Children.Add(new Panel
            {
                Name = "contentPresenter",
                //Height = "100%",
                //Width = "100%",
                MarginTop = 0,
                MarginLeft = 0,
                PresenterFor = this
            });
        }
        private void StackPanel_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            ComputeItems();
        }

        [PropertyChanged(nameof(MultiSelector))]
        void RegisterMultiSelector(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            multiSelector = newValue as MultiSelector<T>;
            if (newValue != null)
            {
                multiSelector.PropertyChanged += MultiSelector_PropertyChanged;
                multiSelector.SelectedIndexs.CollectionChanged += SelectedIndexs_CollectionChanged;
            }
            if (oldValue != null)
            {
                (oldValue as MultiSelector<T>).PropertyChanged -= MultiSelector_PropertyChanged;
                (oldValue as MultiSelector<T>).SelectedIndexs.CollectionChanged -= SelectedIndexs_CollectionChanged;
            }
        }
        [PropertyChanged(nameof(Child))]
        void RegisterChild(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (stackPanel != null)
            {
                stackPanel.LayoutUpdated -= StackPanel_LayoutUpdated;
            }
            stackPanel = newValue as StackPanel;
            if (stackPanel)
            {
                stackPanel.LayoutUpdated += StackPanel_LayoutUpdated;
            }
            if (newValue != null && stackPanel == null)
            {
                throw new Exception("虚拟模式必须是StackPanel");
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(MultiSelector))
        //    {
        //        multiSelector = newValue as MultiSelector<T>;
        //        if (newValue != null)
        //        {
        //            multiSelector.PropertyChanged += MultiSelector_PropertyChanged;
        //            multiSelector.SelectedIndexs.CollectionChanged += SelectedIndexs_CollectionChanged;
        //        }
        //        if (oldValue != null)
        //        {
        //            (oldValue as MultiSelector<T>).PropertyChanged -= MultiSelector_PropertyChanged;
        //            (oldValue as MultiSelector<T>).SelectedIndexs.CollectionChanged -= SelectedIndexs_CollectionChanged;
        //        }
        //    }
        //    else if (propertyName == nameof(Child))
        //    {
        //        if (stackPanel != null)
        //        {
        //            stackPanel.LayoutUpdated -= StackPanel_LayoutUpdated;
        //        }
        //        stackPanel = newValue as StackPanel;
        //        if (stackPanel)
        //        {
        //            stackPanel.LayoutUpdated += StackPanel_LayoutUpdated;
        //        }
        //    }
        //}
        MultiSelector<T> multiSelector;
        private void SelectedIndexs_CollectionChanged(object sender, CollectionChangedEventArgs<int> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    if (e.NewItem >= startId && e.NewItem <= startId + count)
                    {
                        var i = multiSelector.ItemsHost.Children.Select(a => a as ISelectableItem).FirstOrDefault(a => a.Index == e.NewItem);
                        if (i != null)
                        {
                            i.IsSetOnOwner = true;
                            i.IsSelected = true;
                            i.IsSetOnOwner = false;
                        }
                    }
                    break;
                case CollectionChangedAction.Remove:
                    if (e.OldItem >= startId && e.OldItem <= startId + count)
                    {
                        var i = multiSelector.ItemsHost.Children.Select(a => a as ISelectableItem).FirstOrDefault(a => a.Index == e.OldItem);
                        if (i != null)
                        {
                            i.IsSetOnOwner = true;
                            i.IsSelected = false;
                            i.IsSetOnOwner = false;
                        }
                    }
                    break;
                case CollectionChangedAction.Replace:
                    if (e.NewItem >= startId && e.NewItem <= startId + count)
                    {
                        var i = multiSelector.ItemsHost.Children.Select(a => a as ISelectableItem).FirstOrDefault(a => a.Index == e.NewItem);
                        if (i != null)
                        {
                            i.IsSetOnOwner = true;
                            i.IsSelected = true;
                            i.IsSetOnOwner = false;
                        }
                    }
                    if (e.OldItem >= startId && e.OldItem <= startId + count)
                    {
                        var i = multiSelector.ItemsHost.Children.Select(a => a as ISelectableItem).FirstOrDefault(a => a.Index == e.OldItem);
                        if (i != null)
                        {
                            i.IsSetOnOwner = true;
                            i.IsSelected = false;
                            i.IsSetOnOwner = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void MultiSelector_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemsControl<T>.ItemTemplate))
            {
                widthOrHeight = -1;
                if (Items != null && Items.Count > 0)
                {
                    stackPanel.Children.DisposeChildren();
                    var item = multiSelector.CreateItemElement();
                    item.DataContext = Items[0];
                    //item.Content = Items[0];
                    if (item is ContentControl c)
                    {
                        var diss = multiSelector.DisplayMemberPath;
                        if (!string.IsNullOrWhiteSpace(diss))
                        {
                            c.Content = Items[0].GetPropretyValue(diss);
                        }
                        else
                        {
                            c.Content = Items[0];
                        }
                    }
                    item.Index = 0;
                    stackPanel.Children.Add(item);
                }
            }
        }

        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            BeginInvoke(ComputeItems);
        }
        bool scrollChanged = true;

        //protected override void InitializeComponent()
        //{
        //    base.InitializeComponent();
        //}

        private void ComputeItems()
        {
            if (stackPanel && Items != null)
            {
                bool changed = false;
                var size = ActualSize;
                if (CustomScrollData == null)
                {
                    if (stackPanel.Children.Count > 0 && size.Width > 0 && size.Height > 0)
                    {
                        //end = false;
                        var s = stackPanel.Children[0].ActualSize;
                        var w = s.Height;
                        int _count = 0;
                        if (stackPanel.Orientation == Orientation.Horizontal)
                        {
                            w = s.Width;
                            if (widthOrHeight != -1)
                            {
                                w = widthOrHeight;
                            }
                            if (w == 0)
                            {
                                return;
                            }
                            _count = (int)Math.Ceiling(size.Width / w);
                            ExtentWidth = Items.Count * w;
                            if (stackPanel.Children.Count > 0)
                            {
                                var ww = 0f;
                                for (int i = 0; i < Math.Min(stackPanel.Children.Count, _count); i++)
                                {
                                    ww += stackPanel.Children[i].ActualSize.Width;
                                }
                                ExtentWidth = Math.Max(ExtentWidth, ww);
                            }
                            //ExtentHeight = stackPanel.ActualSize.Height;
                            var extentHeight = stackPanel.ActualSize.Height;
                            if (extentHeight != ExtentHeight)
                            {
                                ExtentHeight = extentHeight;
                                changed = true;
                            }
                        }
                        else
                        {
                            if (widthOrHeight != -1)
                            {
                                w = widthOrHeight;
                            }
                            if (w == 0)
                            {
                                return;
                            }
                            _count = (int)Math.Ceiling(size.Height / w);
                            ExtentHeight = Items.Count * w;
                            if (stackPanel.Children.Count > 0)
                            {
                                var h = 0f;
                                for (int i = 0; i < Math.Min(stackPanel.Children.Count, _count); i++)
                                {
                                    h += stackPanel.Children[i].ActualSize.Height;
                                }
                                ExtentHeight = Math.Max(ExtentHeight, h);
                            }
                            //ExtentWidth = stackPanel.ActualSize.Width;
                            var extentWidth = stackPanel.ActualSize.Width;
                            if (extentWidth != ExtentWidth)
                            {
                                ExtentWidth = extentWidth;
                                changed = true;
                            }
                        }
                        _count = Math.Min(_count, Items.Count);
                        if (widthOrHeight != w || this.count != _count)
                        {
                            widthOrHeight = w;
                            this.count = _count;
                            Scroll();
                        }
                    }
                }
                else
                {
                    int _count = 0;
                    var r = CustomScrollData;
                    var w = r.DefaultSize;
                    if (w <= 0)
                    {
                        throw new Exception("自定义虚拟滚动的默认尺寸必须大于0");
                    }

                    if (r.Custom != null)
                    {
                        var cc = r.Custom.Count();
                        if (stackPanel.Orientation == Orientation.Horizontal)
                        {
                            _count = (int)Math.Ceiling(size.Width / w);
                            ExtentWidth = r.DefaultSize * (Items.Count - cc) + r.Custom.Sum(a => a.size);
                            var extentHeight = stackPanel.ActualSize.Height;
                            if (extentHeight != ExtentHeight)
                            {
                                ExtentHeight = extentHeight;
                                changed = true;
                            }
                        }
                        else
                        {
                            _count = (int)Math.Ceiling(size.Height / w);
                            ExtentHeight = r.DefaultSize * (Items.Count - cc) + r.Custom.Sum(a => a.size);
                            var extentWidth = stackPanel.ActualSize.Width;
                            if (extentWidth != ExtentWidth)
                            {
                                ExtentWidth = extentWidth;
                                changed = true;
                            }
                        }
                    }
                    else
                    {
                        if (stackPanel.Orientation == Orientation.Horizontal)
                        {
                            _count = (int)Math.Ceiling(size.Width / w);
                            ExtentWidth = r.DefaultSize * Items.Count;
                            var extentHeight = stackPanel.ActualSize.Height;
                            if (extentHeight != ExtentHeight)
                            {
                                ExtentHeight = extentHeight;
                                changed = true;
                            }
                        }
                        else
                        {
                            _count = (int)Math.Ceiling(size.Height / w);
                            ExtentHeight = r.DefaultSize * Items.Count;
                            var extentWidth = stackPanel.ActualSize.Width;
                            if (extentWidth != ExtentWidth)
                            {
                                ExtentWidth = extentWidth;
                                changed = true;
                            }
                        }
                    }

                    _count = Math.Min(_count + 1, Items.Count);
                    if (widthOrHeight != w || this.count != _count)
                    {
                        widthOrHeight = w;
                        this.count = _count;
                        Scroll();
                    }
                }
                if (Items == null || Items.Count == 0)
                {
                    ExtentHeight = 0;
                    ExtentWidth = 0;
                }
                if (ViewportHeight != size.Height)
                {
                    ViewportHeight = size.Height;
                    changed = true;
                }
                if (ViewportWidth != size.Width)
                {
                    ViewportWidth = size.Width;
                    changed = true;
                }
                if (CanVerticallyScroll != (ExtentHeight - ViewportHeight) > 1)
                {
                    CanVerticallyScroll = (ExtentHeight - ViewportHeight) > 1;
                    changed = true;
                }
                if (CanHorizontallyScroll != (ExtentWidth - ViewportWidth) > 1)
                {
                    CanHorizontallyScroll = (ExtentWidth - ViewportWidth) > 1;
                    changed = true;
                }
                if (changed)
                {
                    if (scrollChanged)
                    {
                        scrollChanged = false;
                        this.BeginInvoke(() =>
                        {
                            RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
                            scrollChanged = true;
                        });
                    }
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Width), new UIPropertyMetadataAttribute((FloatField)"100%", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute((FloatField)"100%", UIPropertyOptions.AffectsMeasure));
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            foreach (UIElement child in Children)
            {
                if (stackPanel.Orientation == Orientation.Vertical)
                {
                    if (end && CustomScrollData == null)
                    {
                        rect.Location = new Point(-HorizontalOffset, rect.Height - child.DesiredSize.Height);
                        //rect.Size = child.DesiredSize;
                        child.Arrange(rect);
                    }
                    else
                    {
                        //rect.Size = child.DesiredSize;
                        rect.X = -HorizontalOffset;
                        rect.Y = offset;
                        child.Arrange(rect);
                    }
                }
                else
                {
                    if (end && CustomScrollData == null)
                    {
                        rect.Location = new Point(rect.Width - child.DesiredSize.Width, -VerticalOffset);
                        child.Arrange(rect);
                    }
                    else
                    {
                        rect.Y = -VerticalOffset;
                        rect.X = offset;
                        child.Arrange(rect);
                    }
                }
            }
            return finalSize;
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            var items = Items;
            if (stackPanel && stackPanel.Children.Count > 0 && items != null && items.Count > 0)
            {
                var s = stackPanel.Children[0].ActualSize;
                if (stackPanel.Orientation == Orientation.Horizontal)
                {
                    size = new Size(Math.Max(s.Width * items.Count, size.Width), Math.Max(size.Height, s.Height));
                }
                else
                {
                    size = new Size(Math.Max(s.Width, size.Width), Math.Max(size.Height, s.Height * items.Count));
                }
            }
            return size;
        }

        //protected override Size MeasureOverride(in Size availableSize)
        //{
        //    var s = base.MeasureOverride(availableSize);
        //    return new Size(ExtentWidth, ExtentHeight);
        //}
        /// <summary>
        /// 用来自定义虚拟模式，调整自定义模板里的尺寸，实现正常的虚拟化呈现。模板里要根据数据来修改尺寸，否则可能会对应不上。
        /// </summary>
        /// <returns>返回默认尺寸和自定义尺寸，index：数据里的索引，不能有重复index，size：呈现尺寸。 自定义尺寸可以为null，默认尺寸不能小于等于0，没有在自定义尺寸里的数据使用默认尺寸</returns>
        [NotCpfProperty]
        public CustomScrollData CustomScrollData
        {
            get { return scrollData; }
            set
            {
                if (scrollData != value)
                {
                    if (scrollData != null)
                    {
                        scrollData.owner = null;
                    }
                    if (value.owner != null && value.owner != this)
                    {
                        throw new Exception("CustomScrollData不能共享");
                    }
                    value.owner = this;
                    scrollData = value;
                    InvalidateArrange();
                }
            }
        }
        CustomScrollData scrollData;

        float offset;

        int oldScrollValue = -1;
        int oldCount = 0;
        void Scroll(bool refresh = false)
        {
            if (stackPanel && widthOrHeight != -1)
            {
                if (CustomScrollData == null)
                {
                    if (stackPanel.Orientation == Orientation.Horizontal)
                    {
                        if (ExtentWidth > ViewportWidth && widthOrHeight != 0)
                        {
                            scrollValue = (int)((widthOrHeight / 2 + HorizontalOffset) / widthOrHeight);
                            if (ViewportWidth + HorizontalOffset == ExtentWidth)
                            {
                                scrollValue += 1;
                            }
                        }
                        else
                        {
                            scrollValue = 0;
                        }
                        offset = 0;
                    }
                    else
                    {
                        if (ExtentHeight > ViewportHeight && widthOrHeight != 0)
                        {
                            scrollValue = (int)((widthOrHeight / 2 + VerticalOffset) / widthOrHeight);
                            if (ViewportHeight + VerticalOffset == ExtentHeight)
                            {
                                scrollValue += 1;
                            }
                            //scrollValue = (int)(VerticalOffset / widthOrHeight);
                            //offset = -(VerticalOffset - scrollValue * widthOrHeight);
                        }
                        else
                        {
                            scrollValue = 0;
                        }
                        offset = 0;
                    }
                }
                else
                {
                    var r = CustomScrollData;
                    if (r.DefaultSize <= 0)
                    {
                        throw new Exception("自定义虚拟滚动的默认尺寸必须大于0");
                    }
                    if (r.DefaultSize != widthOrHeight)
                    {
                        throw new Exception("默认尺寸不能每次都不同");
                    }
                    if (r.Custom != null)
                    {
                        var list = r.Custom.OrderBy(a => a.index).ToArray();
                        if (list.Length > 0)
                        {
                            if (stackPanel.Orientation == Orientation.Horizontal)
                            {
                                scrollValue = (int)(HorizontalOffset / widthOrHeight);
                                if (list[0].index >= scrollValue)
                                {
                                    offset = -(HorizontalOffset - scrollValue * widthOrHeight);
                                }
                                else
                                {
                                    var len = list[0].index * widthOrHeight;
                                    for (int i = 0; i < list.Length; i++)
                                    {
                                        len += list[i].size;
                                        if (len > HorizontalOffset)
                                        {
                                            scrollValue = list[i].index;
                                            offset = len - list[i].size - HorizontalOffset;
                                            break;
                                        }
                                        else
                                        {
                                            var c = (int)((HorizontalOffset - len) / widthOrHeight) + list[i].index + 1;
                                            if (i == list.Length - 1 || c < list[i + 1].index)
                                            {
                                                len += (int)((HorizontalOffset - len) / widthOrHeight) * widthOrHeight;
                                                scrollValue = c;
                                                offset = len - HorizontalOffset;
                                                break;
                                            }
                                            else
                                            {
                                                len += widthOrHeight * (list[i + 1].index - list[i].index - 1);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                scrollValue = (int)(VerticalOffset / widthOrHeight);
                                if (list[0].index >= scrollValue)
                                {
                                    offset = -(VerticalOffset - scrollValue * widthOrHeight);
                                }
                                else
                                {
                                    var len = list[0].index * widthOrHeight;
                                    for (int i = 0; i < list.Length; i++)
                                    {
                                        len += list[i].size;
                                        if (len > VerticalOffset)
                                        {
                                            scrollValue = list[i].index;
                                            offset = len - list[i].size - VerticalOffset;
                                            break;
                                        }
                                        else
                                        {
                                            var c = (int)((VerticalOffset - len) / widthOrHeight) + list[i].index + 1;
                                            if (i == list.Length - 1 || c < list[i + 1].index)
                                            {
                                                len += (int)((VerticalOffset - len) / widthOrHeight) * widthOrHeight;
                                                scrollValue = c;
                                                offset = len - VerticalOffset;
                                                break;
                                            }
                                            else
                                            {
                                                len += widthOrHeight * (list[i + 1].index - list[i].index - 1);
                                            }
                                        }
                                    }
                                }
                                //Debug.WriteLine(scrollValue);
                                //Debug.WriteLine(offset);
                            }
                        }
                    }
                    else
                    {
                        if (stackPanel.Orientation == Orientation.Horizontal)
                        {
                            if (ExtentWidth > ViewportWidth && widthOrHeight != 0)
                            {
                                scrollValue = (int)(HorizontalOffset / widthOrHeight);
                                offset = -(HorizontalOffset - scrollValue * widthOrHeight);
                            }
                            else
                            {
                                scrollValue = 0;
                                offset = 0;
                            }
                        }
                        else
                        {
                            if (ExtentHeight > ViewportHeight && widthOrHeight != 0)
                            {
                                scrollValue = (int)(VerticalOffset / widthOrHeight);
                                offset = -(VerticalOffset - scrollValue * widthOrHeight);
                            }
                            else
                            {
                                scrollValue = 0;
                                offset = 0;
                            }
                        }
                    }
                }

                if (oldScrollValue != scrollValue || oldCount != count || refresh)
                {
                    oldCount = count;
                    oldScrollValue = scrollValue;

                    var start = scrollValue;
                    if (CustomScrollData == null && start + count > Items.Count)
                    {
                        end = true;
                        start = Items.Count - count;
                        if (start <= 0)
                        {
                            start = 0;
                            //end = false;
                        }
                    }
                    else
                    {
                        end = false;
                    }
                    startId = start;
                    _count = 0;
                    var diss = multiSelector.DisplayMemberPath;
                    if (multiSelector.HasProperty(nameof(ListBox.VirtualizationMode)) && multiSelector.GetValue<VirtualizationMode>(nameof(ListBox.VirtualizationMode)) == VirtualizationMode.Recycling)
                    {
                        var ac = multiSelector.AlternationCount;
                        for (int i = 0; i < count; i++)
                        {
                            var s = start + i;
                            if (s < Items.Count)
                            {
                                _count++;
                                T item = null;
                                if (stackPanel.Children.Count > i)
                                {
                                    item = stackPanel.Children[i] as T;
                                    item.DataContext = Items[s];
                                }
                                else
                                {
                                    item = multiSelector.CreateItemElement();
                                    item.DataContext = Items[s];
                                    stackPanel.Children.Add(item);
                                }
                                //item.Content = Items[s];
                                if (item is ContentControl c)
                                {
                                    if (!string.IsNullOrWhiteSpace(diss))
                                    {
                                        c.Content = Items[s].GetPropretyValue(diss);
                                    }
                                    else
                                    {
                                        c.Content = Items[s];
                                    }
                                }
                                if (ac != 0)
                                {
                                    multiSelector.SetAlternationIndex((CpfObject)item, (int)(s % ac));
                                }
                                else
                                {
                                    multiSelector.SetAlternationIndex((CpfObject)item, s);
                                }
                                item.Index = s;
                                item.IsSetOnOwner = true;
                                item.IsSelected = multiSelector.SelectedIndexs.Any(a => a == s);
                                item.IsSetOnOwner = false;
                                OnSetItem(item);
                            }
                        }
                        var cc = stackPanel.Children.Count;
                        if (_count > 0 || widthOrHeight > 0)
                        {
                            for (int i = _count; i < cc; i++)
                            {//多余的释放
                                var item = stackPanel.Children[_count];
                                if (item is ContentControl c)
                                {
                                    c.Content = null;
                                }
                                item.DataContext = null;
                                item.Dispose();
                            }
                        }
                    }
                    else
                    {
                        //stackPanel.Children.DisposeChildren();
                        //for (int i = 0; i < count; i++)
                        //{
                        //    if (start + i < Items.Count)
                        //    {
                        //        _count++;
                        //        var item = MultiSelector.CreateItemElement();
                        //        item.DataContext = Items[start + i];
                        //        //item.Content = Items[start + i];
                        //        if (item is ContentControl c)
                        //        {
                        //            if (!string.IsNullOrWhiteSpace(diss))
                        //            {
                        //                c.Content = Items[start + i].GetPropretyValue(diss);
                        //            }
                        //            else
                        //            {
                        //                c.Content = Items[start + i];
                        //            }
                        //        }
                        //        item.Index = start + i;
                        //        item.IsSelected = MultiSelector.SelectedIndexs.Any(a => a == start + i);
                        //        OnSetItem(item);
                        //        stackPanel.Children.Add(item);
                        //    }
                        //}

                        //GCLatencyMode oldMode = GCSettings.LatencyMode;
                        //GCSettings.LatencyMode = GCLatencyMode.LowLatency;
                        List<T> list = new List<T>();
                        foreach (T item in stackPanel.Children)
                        {
                            var index = item.Index;
                            if (index < start || index > start + count - 1)
                            {
                                list.Add(item);
                            }
                        }
                        if (_count > 0 || widthOrHeight > 0)
                        {
                            for (int i = list.Count - 1; i >= 0; i--)
                            {
                                //list[i].Dispose();
                                var item = list[i];
                                if (item is ContentControl c)
                                {
                                    c.Content = null;
                                }
                                item.DataContext = null;
                                item.Dispose();
                            }
                        }
                        var s = -1;
                        var e = -1;
                        if (stackPanel.Children.Count > 0)
                        {
                            s = (stackPanel.Children[0] as T).Index;
                            e = (stackPanel.Children[stackPanel.Children.Count - 1] as T).Index;
                        }
                        var ac = multiSelector.AlternationCount;
                        for (int i = 0; i < count; i++)
                        {
                            var index = start + i;
                            if ((index < s || index > e) && index < Items.Count)
                            {
                                var item = multiSelector.CreateItemElement();
                                item.DataContext = Items[index];
                                stackPanel.Children.Insert(i, item);
                                if (item is ContentControl c)
                                {
                                    if (!string.IsNullOrWhiteSpace(diss))
                                    {
                                        c.Content = Items[index].GetPropretyValue(diss);
                                    }
                                    else
                                    {
                                        c.Content = Items[index];
                                    }
                                }
                                if (ac != 0)
                                {
                                    multiSelector.SetAlternationIndex((CpfObject)item, (int)(index % ac));
                                }
                                else
                                {
                                    multiSelector.SetAlternationIndex((CpfObject)item, index);
                                }
                                item.Index = index;
                                item.IsSetOnOwner = true;
                                item.IsSelected = multiSelector.SelectedIndexs.Any(a => a == index);
                                item.IsSetOnOwner = false;
                                OnSetItem(item);
                            }
                            else if(index < Items.Count)
                            {
                                var item = stackPanel.Children[i] as T;
                                item.DataContext = Items[index];
                                if (item is ContentControl c)
                                {
                                    if (!string.IsNullOrWhiteSpace(diss))
                                    {
                                        c.Content = Items[index].GetPropretyValue(diss);
                                    }
                                    else
                                    {
                                        c.Content = Items[index];
                                    }
                                }
                                if (ac != 0)
                                {
                                    multiSelector.SetAlternationIndex((CpfObject)item, (int)(index % ac));
                                }
                                else
                                {
                                    multiSelector.SetAlternationIndex((CpfObject)item, index);
                                }
                                item.Index = index;
                                item.IsSetOnOwner = true;
                                item.IsSelected = multiSelector.SelectedIndexs.Any(a => a == index);
                                item.IsSetOnOwner = false;
                                OnSetItem(item);
                            }
                        }
                        //GC.Collect();
                        //GCSettings.LatencyMode = oldMode;
                    }
                }

                var size = ActualSize;
                //bool changed = false;
                //if (extentHeight != ExtentHeight)
                //{
                //    ExtentHeight = extentHeight;
                //    changed = true;
                //}
                //if (extentWidth != ExtentWidth)
                //{
                //    ExtentWidth = extentWidth;
                //    changed = true;
                //}
                if (ViewportHeight != size.Height)
                {
                    ViewportHeight = size.Height;
                    //changed = true;
                }
                if (ViewportWidth != size.Width)
                {
                    ViewportWidth = size.Width;
                    //changed = true;
                }
                if (CanVerticallyScroll != (ExtentHeight - ViewportHeight) > 1)
                {
                    CanVerticallyScroll = (ExtentHeight - ViewportHeight) > 1;
                    //changed = true;
                }
                if (CanHorizontallyScroll != (ExtentWidth - ViewportWidth) > 1)
                {
                    CanHorizontallyScroll = (ExtentWidth - ViewportWidth) > 1;
                    //changed = true;
                }
                InvalidateArrange();
                RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            }
        }
        /// <summary>
        /// 对元素设置数据之后
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnSetItem(T item)
        {
            if (SetItem != null)
            {
                SetItem(this, item);
            }
        }

        /// <summary>
        /// 对元素设置数据之后
        /// </summary>
        public event EventHandler<T> SetItem;
        //protected override void OnLayoutUpdated()
        //{
        //    var size = ActualSize;
        //    bool changed = false;
        //    if (extentHeight != ExtentHeight)
        //    {
        //        ExtentHeight = extentHeight;
        //        changed = true;
        //    }
        //    if (extentWidth != ExtentWidth)
        //    {
        //        ExtentWidth = extentWidth;
        //        changed = true;
        //    }
        //    if (ViewportHeight != size.Height)
        //    {
        //        ViewportHeight = size.Height;
        //        changed = true;
        //    }
        //    if (ViewportWidth != size.Width)
        //    {
        //        ViewportWidth = size.Width;
        //        changed = true;
        //    }
        //    if (CanVerticallyScroll != ExtentHeight > ViewportHeight)
        //    {
        //        CanVerticallyScroll = ExtentHeight > ViewportHeight;
        //        changed = true;
        //    }
        //    if (CanHorizontallyScroll != ExtentWidth > ViewportWidth)
        //    {
        //        CanHorizontallyScroll = ExtentWidth > ViewportWidth;
        //        changed = true;
        //    }
        //    if (changed)
        //    {
        //        RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
        //    }
        //    base.OnLayoutUpdated();
        //}

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - widthOrHeight);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + widthOrHeight);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - widthOrHeight);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + widthOrHeight);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset + widthOrHeight * 2);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset - widthOrHeight * 2);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - widthOrHeight * 2);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + widthOrHeight * 2);
        }

        CPF.Threading.DispatcherTimer timer;
        void SetScroll()
        {
            if (timer == null && !IsDisposed && !IsDisposing)
            {
                timer = new Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10), IsEnabled = true };
                timer.Tick += delegate
                {
                    Scroll();
                    timer.Dispose();
                    timer = null;
                };
            }
        }

        public void SetHorizontalOffset(float offset)
        {
            HorizontalOffset = offset;
            if (HorizontalOffset < 0 || ExtentWidth < ViewportWidth)
            {
                HorizontalOffset = 0;
            }
            else if (HorizontalOffset > ExtentWidth - ViewportWidth)
            {
                HorizontalOffset = ExtentWidth - ViewportWidth;
            }
            SetScroll();
            //Scroll();
            //RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            if (stackPanel && stackPanel.Orientation == Orientation.Vertical)
            {
                InvalidateArrange();
            }
        }

        public void SetVerticalOffset(float offset)
        {
            VerticalOffset = offset;
            if (VerticalOffset < 0 || ExtentHeight < ViewportHeight)
            {
                VerticalOffset = 0;
            }
            else if (VerticalOffset > ExtentHeight - ViewportHeight)
            {
                VerticalOffset = ExtentHeight - ViewportHeight;
            }
            SetScroll();
            //Scroll();
            //RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            if (stackPanel && stackPanel.Orientation == Orientation.Horizontal)
            {
                InvalidateArrange();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            base.Dispose(disposing);
        }

        [NotCpfProperty]
        public bool CanVerticallyScroll
        {
            get;
            set;
        }

        [NotCpfProperty]
        public bool CanHorizontallyScroll
        {
            get;
            set;
        }

        [NotCpfProperty]
        public float ExtentWidth
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ExtentHeight
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ViewportWidth
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ViewportHeight
        {
            get; private set;
        }

        [NotCpfProperty]
        public float HorizontalOffset
        {
            get; private set;
        }

        [NotCpfProperty]
        public float VerticalOffset
        {
            get;
            private set;
        }

        [NotCpfProperty]
        public ScrollViewer ScrollOwner
        {
            get;
            set;
        }
        /// <summary>
        /// 开始ID
        /// </summary>
        [NotCpfProperty]
        public int StartId
        {
            get
            {
                return startId;
            }
        }
        /// <summary>
        /// 当前显示的数量
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }
    }
    /// <summary>
    /// 用来自定义虚拟模式，调整自定义模板里的尺寸，实现正常的虚拟化呈现
    /// </summary>
    public class CustomScrollData
    {
        /// <summary>
        /// 用来自定义虚拟模式，调整自定义模板里的尺寸，实现正常的虚拟化呈现。模板里要根据数据来修改尺寸，否则可能会对应不上。返回默认尺寸和自定义尺寸，index：数据里的索引，不能有重复index，size：呈现尺寸，必须大于默认值。 自定义尺寸可以为null，默认尺寸不能小于等于0，没有在自定义尺寸里的数据使用默认尺寸
        /// </summary>
        public CustomScrollData()
        { }

        float defaultSize;

        internal UIElement owner;

        IEnumerable<(int index, float size)> custom;
        /// <summary>
        /// 默认尺寸，必须大于0
        /// </summary>
        public float DefaultSize
        {
            get => defaultSize;
            set
            {
                defaultSize = value;
                if (value <= 0)
                {
                    throw new Exception("默认尺寸必须大于0");
                }
                if (owner != null)
                {
                    owner.InvalidateArrange();
                }
            }
        }
        /// <summary>
        /// index：数据里的索引，不能有重复index，size：呈现尺寸。 自定义尺寸可以为null，没有在自定义尺寸里的数据使用默认尺寸
        /// </summary>
        public IEnumerable<(int index, float size)> Custom
        {
            get => custom;
            set
            {
                custom = value;
                if (owner != null)
                {
                    owner.InvalidateArrange();
                }
            }
        }
    }
}
