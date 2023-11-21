using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using CPF.Animation;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示包含多个项的控件，这些项共享屏幕上的同一空间。
    /// </summary>
    [Description("表示包含多个项的控件，这些项共享屏幕上的同一空间。")]
    public class TabControl : Control
    {
        public TabControl()
        {
            items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, CollectionChangedEventArgs<TabItem> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    if (headerPanel)
                    {
                        AddPage(e.NewItem, e.Index);
                    }
                    break;
                case CollectionChangedAction.Remove:
                    if (headerPanel)
                    {
                        headerPanel.Children.Remove(e.OldItem);
                        if (e.OldItem.ContentElement != null)
                        {
                            contentPanel.Children.Remove(e.OldItem.ContentElement);
                        }
                        e.OldItem.MouseDown -= Item_MouseDown;
                        e.OldItem.PropertyChanged -= TabItem_PropertyChanged;
                        if (SelectedIndex > items.Count)
                        {
                            SelectedIndex = items.Count - 1;
                        }
                    }
                    break;
                case CollectionChangedAction.Replace:
                    if (headerPanel)
                    {
                        AddPage(e.NewItem, e.Index);
                        headerPanel.Children.Remove(e.OldItem);
                        e.OldItem.MouseDown -= Item_MouseDown;
                        e.OldItem.PropertyChanged -= TabItem_PropertyChanged;
                        if (e.OldItem.ContentElement != null)
                        {
                            if (e.NewItem.ContentElement != null)
                            {
                                e.NewItem.ContentElement.Visibility = e.OldItem.ContentElement.Visibility;
                            }
                            contentPanel.Children.Remove(e.OldItem.ContentElement);
                        }
                        if (SelectedIndex > items.Count)
                        {
                            SelectedIndex = items.Count - 1;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (old != null && !isUpdateOld)
            {
                switch (e.Action)
                {
                    case CollectionChangedAction.Add:
                        old.Insert(e.Index, e.NewItem);
                        break;
                    case CollectionChangedAction.Remove:
                        old.RemoveAt(e.Index);
                        break;
                    case CollectionChangedAction.Replace:
                        old[e.Index] = e.NewItem;
                        break;
                    case CollectionChangedAction.Sort:
                        old.Clear();
                        old.AddRange(sender as IEnumerable<TabItem>);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddPage(TabItem tabItem, int index = -1)
        {
            if (index == -1)
            {
                headerPanel.Children.Add(tabItem);
            }
            else
            {
                headerPanel.Children.Insert(index, tabItem);
            }
            tabItem.MouseDown += Item_MouseDown;
            tabItem.PropertyChanged += TabItem_PropertyChanged;
            if (tabItem.ContentElement != null)
            {
                tabItem.ContentElement.Visibility = Visibility.Collapsed;
                contentPanel.Children.Add(tabItem.ContentElement);
            }
        }

        private void TabItem_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabItem.ContentElement))
            {
                if (contentPanel)
                {
                    contentPanel.Children.Remove(e.OldValue as UIElement);
                    contentPanel.Children.Add(e.NewValue as UIElement);
                }
            }
            else if (e.PropertyName == nameof(TabItem.IsSelected))
            {
                if (sender is TabItem item && !item.InnerSetIsSelected)
                {
                    SelectedIndex = Items.IndexOf(item);
                }
            }
        }

        private void Item_MouseDown(object sender, Input.MouseEventArgs e)
        {
            if (!e.Handled)
            {
                var item = sender as TabItem;
                SelectedIndex = items.IndexOf(item);
            }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(SelectedIndex))
            {
                int v = (int)value;
                if (v < 0)
                {
                    value = -1;
                }
                else if (v > items.Count - 1)
                {
                    value = (items.Count - 1);
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        int? selectIndex;
        /// <summary>
        /// 获取或设置当前选择中第一项的索引，如果选择为空，则返回负一(-1)
        /// </summary>
        [PropertyMetadata(-1)]
        [Description("获取或设置当前选择中第一项的索引，如果选择为空，则返回负一(-1)")]
        public int SelectedIndex
        {
            get { return GetValue<int>(); }
            set
            {
                if (!IsInitialized)
                {
                    selectIndex = value;
                }
                else
                {
                    selectIndex = null;
                    SetValue(value);
                }
            }
        }
        /// <summary>
        /// 获取，如果选择为空，则返回 null
        /// </summary>
        public TabItem SelectedItem
        {
            get { return GetValue<TabItem>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        /// 选项卡标题相对于选项卡内容的对齐方式
        /// </summary>
        [PropertyMetadata(Dock.Top), Description("选项卡标题相对于选项卡内容的对齐方式")]
        public Dock TabStripPlacement
        {
            get { return GetValue<Dock>(); }
            set { SetValue(value); }
        }
        ///// <summary>
        ///// 获取或设置选项卡标题如何相对于选项卡内容进行对齐。
        ///// </summary>
        //public Dock TabStripPlacement { get { return GetValue<Dock>(); } set { SetValue(value); } }

        Collection<TabItem> items = new Collection<TabItem>();
        Collection<TabItem> old;
        bool isUpdateOld;
        /// <summary>
        /// TabItem内容集合
        /// </summary>
        [NotCpfProperty]
        public Collection<TabItem> Items
        {
            get { return items; }
            set
            {
                if (old != null)
                {
                    old.CollectionChanged -= Value_CollectionChanged;
                }
                old = value;
                if (value == null)
                {
                    items.Clear();
                }
                else
                {
                    items.Clear();
                    items.AddRange(value);
                    value.CollectionChanged += Value_CollectionChanged;
                }
            }
        }

        private void Value_CollectionChanged(object sender, CollectionChangedEventArgs<TabItem> e)
        {
            isUpdateOld = true;
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    items.Insert(e.Index, e.NewItem);
                    break;
                case CollectionChangedAction.Remove:
                    items.RemoveAt(e.Index);
                    break;
                case CollectionChangedAction.Replace:
                    items[e.Index] = e.NewItem;
                    break;
                case CollectionChangedAction.Sort:
                    items.Clear();
                    items.AddRange(sender as IEnumerable<TabItem>);
                    break;
                default:
                    break;
            }
            isUpdateOld = false;
        }

        protected override void InitializeComponent()
        {
            Children.Add(new Grid
            {
                Width = "100%",
                Height = "100%",
                ColumnDefinitions = { new ColumnDefinition { Width="*",
                    Bindings={ {nameof(ColumnDefinition.Width),nameof(TabStripPlacement),this,BindingMode.OneWay,a=> (Dock)a== Dock.Left?GridLength.Auto:GridLength.Star} } },
                    new ColumnDefinition { Width = 0,
                    Bindings={ {nameof(ColumnDefinition.Width),nameof(TabStripPlacement),this,BindingMode.OneWay,
                                a=>{
                                    switch ((Dock)a)
                                    {
                                        case Dock.Left:
                                        return GridLength.Star;
                                        case Dock.Right:
                                        return GridLength.Auto;
                                        default:
                                        return (GridLength)0;
                                    }
                                } } }
                    } },
                RowDefinitions = { new RowDefinition { Height="auto",
                    Bindings={ {nameof(RowDefinition.Height),nameof(TabStripPlacement),this,BindingMode.OneWay,a=> (Dock)a == Dock.Top ? GridLength.Auto : GridLength.Star } } },
                    new RowDefinition {
                    Bindings={ {nameof(RowDefinition.Height),nameof(TabStripPlacement),this,BindingMode.OneWay,
                                a=>{
                                    switch ((Dock)a)
                                    {
                                        case Dock.Top:
                                        return GridLength.Star;
                                        case Dock.Bottom:
                                        return GridLength.Auto;
                                        default:
                                        return (GridLength)0;
                                    }
                                } } }
                    } },
                Children =
                {
                    new Border {
                        Attacheds={
                            { Grid.ColumnIndex, 0
                                ,nameof(TabStripPlacement),this,BindingMode.OneWay,
                                a=> {
                                    var dock=(Dock)a;
                                    return (dock!= Dock.Right)?0:1;
                                }
                            },
                            { Grid.RowIndex,0
                                ,nameof(TabStripPlacement),this,BindingMode.OneWay,
                                a=> {
                                    var dock=(Dock)a;
                                    return (dock!= Dock.Bottom)?0:1;
                                }
                            } },
                        Name="headBorder",
                        BorderFill="230,230,230",
                        Background="242,242,242",
                        BorderThickness=new Thickness(1),
                        BorderType= BorderType.BorderThickness,
                        Width="100%",
                        MarginLeft=0,
                        Padding=new Thickness(0,0,0,-1),
                        Bindings={ {nameof(WrapPanel.Width),nameof(TabStripPlacement),this,BindingMode.OneWay,a=>((byte)(Dock)a)%2==0?(FloatField)"auto": (FloatField)"100%" },
                            {nameof(WrapPanel.Height),nameof(TabStripPlacement),this,BindingMode.OneWay,a=>((byte)(Dock)a)%2==0?(FloatField)"100%": (FloatField)"auto" } },
                        Child=
                        new WrapPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Name="headerPanel",
                            PresenterFor=this,
                            MarginLeft=0,
                            Width="100%",
                            Bindings={
                                {nameof(WrapPanel.Orientation),nameof(TabStripPlacement),this,BindingMode.OneWay,a=>((byte)(Dock)a)%2==0?Orientation.Vertical:Orientation.Horizontal },
                                {nameof(WrapPanel.Width),nameof(TabStripPlacement),this,BindingMode.OneWay,a=>((byte)(Dock)a)%2==0?(FloatField)"auto": (FloatField)"100%" },
                                {nameof(WrapPanel.Height),nameof(TabStripPlacement),this,BindingMode.OneWay,a=>((byte)(Dock)a)%2==0?(FloatField)"100%": (FloatField)"auto" }
                            }
                        }
                    },

                    new Border
                    {
                        Name="contentBorder",
                        Attacheds={ { Grid.ColumnIndex, 0,nameof(TabStripPlacement),this,BindingMode.OneWay,
                                a=> {
                                    var dock=(Dock)a;
                                    return (dock!= Dock.Left)?0:1;
                                } },{Grid.RowIndex,1,nameof(TabStripPlacement),this,BindingMode.OneWay,
                                a=> {
                                    var dock=(Dock)a;
                                    return (dock!= Dock.Top)?0:1;
                                } } },
                        Width="100%",
                        Height="100%",
                        BorderFill=null,
                        ClipToBounds=true,
                        //Background="#fff",
                        Child = new Panel{ Name="contentPanel",Width="100%",Height="100%" ,PresenterFor=this},
                    }

                }
            });
        }

        Panel headerPanel;
        Panel contentPanel;
        protected override void OnInitialized()
        {
            headerPanel = FindPresenter<Panel>().FirstOrDefault(a => a.Name == "headerPanel");
            if (!headerPanel)
            {
                throw new Exception("headerPanel不存在或者无效对象");
            }
            contentPanel = FindPresenter<Panel>().FirstOrDefault(a => a.Name == "contentPanel");
            if (!contentPanel)
            {
                throw new Exception("contentPanel不存在或者无效对象");
            }
            if (items.Count > 0)
            {
                foreach (TabItem item in items)
                {
                    AddPage(item);
                }
                var index = items.FindIndex(a => a.IsSelected);
                if (index < 0)
                {
                    if (selectIndex.HasValue)
                    {
                        index = selectIndex.Value;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                if (index > items.Count)
                {
                    index = items.Count - 1;
                }
                selectIndex = null;
                SelectedIndex = index;
                //else
                //{
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ContentElement != null)
                    {
                        items[i].ContentElement.Visibility = index != i ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
                //}
            }
            base.OnInitialized();
        }
        [PropertyChanged(nameof(SelectedIndex))]
        void RegisterSelectedIndex(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            int index = (int)newValue;
            int old = (int)oldValue;

            TabItem item = null;
            if (index > -1 && index < items.Count)
            {
                item = items[index];
                item.InnerSetIsSelected = true;
                item.IsSelected = true;
                item.InnerSetIsSelected = false;
            }
            foreach (TabItem tabItem in items)
            {
                if (item != tabItem)
                {
                    tabItem.InnerSetIsSelected = true;
                    tabItem.IsSelected = false;
                    tabItem.InnerSetIsSelected = false;
                }
            }
            SelectedItem = item;
            TabItem oldItem = null;
            if (old < items.Count && old > -1)
            {
                oldItem = items[old];
            }
            if (SwitchAction != null)
            {
                SwitchAction(oldItem, item);
            }
            else
            {
                OnSwitch(oldItem, item);
            }
        }
        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(SelectedIndex))
        //    {
        //        int index = (int)newValue;
        //        int old = (int)oldValue;

        //        TabItem item = null;
        //        if (index > -1 && index < items.Count)
        //        {
        //            item = items[index];
        //            item.InnerSetIsSelected = true;
        //            item.IsSelected = true;
        //            item.InnerSetIsSelected = false;
        //        }
        //        foreach (TabItem tabItem in items)
        //        {
        //            if (item != tabItem)
        //            {
        //                tabItem.InnerSetIsSelected = true;
        //                tabItem.IsSelected = false;
        //                tabItem.InnerSetIsSelected = false;
        //            }
        //        }
        //        SelectedItem = item;
        //        TabItem oldItem = null;
        //        if (old < items.Count && old > -1)
        //        {
        //            oldItem = items[old];
        //        }
        //        if (SwitchAction != null)
        //        {
        //            SwitchAction(oldItem, item);
        //        }
        //        else
        //        {
        //            OnSwitch(oldItem, item);
        //        }
        //    }
        //}
        /// <summary>
        /// 切换选项卡动作, TabItem oldItem, TabItem newItem
        /// </summary>
        [NotCpfProperty]
        public Action<TabItem, TabItem> SwitchAction { get; set; }

        /// <summary>
        /// 切换选项卡动作
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        protected virtual void OnSwitch(TabItem oldItem, TabItem newItem)
        {
            if (oldItem != null && oldItem.ContentElement != null)
            {
                //oldItem.ContentElement.TransitionValue(nameof(MarginLeft), (FloatField)"-100%", TimeSpan.FromSeconds(0.3), new PowerEase(), AnimateMode.EaseOut, () =>
                //{
                oldItem.ContentElement.Visibility = Visibility.Collapsed;
                //});
            }
            if (newItem != null && newItem.ContentElement != null)
            {
                newItem.ContentElement.Visibility = Visibility.Visible;
                //newItem.ContentElement.MarginLeft = "100%";
                //newItem.ContentElement.TransitionValue(nameof(MarginLeft), (FloatField)"0%", TimeSpan.FromSeconds(0.3), new PowerEase(), AnimateMode.EaseOut);
            }
        }

    }
}
