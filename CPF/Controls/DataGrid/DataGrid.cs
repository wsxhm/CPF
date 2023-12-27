using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Input;
using System.Collections.Specialized;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示用于在可自定义的网格中显示数据的控件。 DataGrid只有虚拟模式，自定义DataGridRow模板需要注意DataGrid的VirtualizationMode属性，Row控件状态不保存
    /// </summary>
    [Description("表示用于在可自定义的网格中显示数据的控件。DataGrid只有虚拟模式，自定义DataGridRow模板需要注意DataGrid的VirtualizationMode属性"), Browsable(true)]
    public class DataGrid : MultiSelector<DataGridRow>
    {
        public DataGrid()
        {
            SelectedIndexs.CollectionChanged += SelectedIndexs_CollectionChanged;
            Columns = new Collection<DataGridColumn>();
            //isVirtualizing = IsVirtualizing;
        }
        private void SelectedIndexs_CollectionChanged(object sender, CollectionChangedEventArgs<int> e)
        {
            if (!presenter)
            {
                var items = Items;
                switch (e.Action)
                {
                    case CollectionChangedAction.Add:
                        if (e.NewItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.NewItem] as DataGridRow);
                            item.IsSetOnOwner = true;
                            item.IsSelected = true;
                            item.IsSetOnOwner = false;
                        }

                        break;
                    case CollectionChangedAction.Remove:
                        if (e.OldItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.OldItem] as DataGridRow);
                            item.IsSetOnOwner = true;
                            item.IsSelected = false;
                            item.IsSetOnOwner = false;
                        }

                        break;
                    case CollectionChangedAction.Replace:
                        if (e.NewItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.NewItem] as DataGridRow);
                            item.IsSetOnOwner = true;
                            item.IsSelected = true;
                            item.IsSetOnOwner = false;
                        }
                        if (e.OldItem < items.Count)
                        {
                            var item = (ItemsHost.Children[e.OldItem] as DataGridRow);
                            item.IsSetOnOwner = true;
                            item.IsSelected = false;
                            item.IsSetOnOwner = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 选择行为
        /// </summary>
        [PropertyMetadata(DataGridSelectionMode.Extended)]
        public DataGridSelectionMode SelectionMode
        {
            get { return GetValue<DataGridSelectionMode>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 选择行为
        /// </summary>
        [PropertyMetadata(DataGridSelectionUnit.Cell)]
        public DataGridSelectionUnit SelectionUnit
        {
            get { return GetValue<DataGridSelectionUnit>(); }
            set { SetValue(value); }
        }
        Collection<DataGridCellInfo> selectedCells = new Collection<DataGridCellInfo>();
        /// <summary>
        /// 获取当前选定单元格的列表。
        /// </summary>
        [NotCpfProperty]
        public IEnumerable<DataGridCellInfo> SelectedCells
        {
            get { return selectedCells; }
        }

        //Collection<DataGridColumn> columns = new Collection<DataGridColumn>();
        /// <summary>
        /// 获取一个集合，该集合包含 DataGrid 中的所有列。
        /// </summary>
        public Collection<DataGridColumn> Columns
        {
            get { return GetValue<Collection<DataGridColumn>>(); }
            set { SetValue(value); }
        }

        ///// <summary>
        ///// 行模板
        ///// </summary>
        //public UIElementTemplate<DataGridRow> RowTemplate
        //{
        //    get { return GetValue<UIElementTemplate<DataGridRow>>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 这里无效，不建议使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override UIElementTemplate<Panel> ItemsPanel
        {
            get => base.ItemsPanel;
            set => base.ItemsPanel = value;
        }
        //bool isVirtualizing;
        /// <summary>
        /// 是否虚拟化UI，只支持StackPanel的虚拟化数据显示。初始化之前设置
        /// </summary>
        [Description("是否虚拟化UI，只支持StackPanel的虚拟化数据显示。初始化之前设置"), PropertyMetadata(true)]
        public bool IsVirtualizing
        {
            get { return GetValue<bool>(); }
            set
            {
                //isVirtualizing = value;
                SetValue(value);
            }
        }

        /// <summary>
        /// 用来自定义虚拟模式，调整自定义模板里的尺寸，实现正常的虚拟化呈现。模板里要根据数据来修改尺寸，否则可能会对应不上。
        /// </summary>
        /// <returns>返回默认尺寸和自定义尺寸，index：数据里的索引，不能有重复index，size：呈现尺寸，必须大于默认值。 自定义尺寸可以为null，默认尺寸不能小于等于0，没有在自定义尺寸里的数据使用默认尺寸</returns>
        public CustomScrollData CustomScrollData
        {
            get { return GetValue<CustomScrollData>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否自动创建列
        /// </summary>
        public bool AutoGenerateColumns
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {
            BorderStroke = new Stroke(1);
            BorderFill = "#bbb";
            if (IsVirtualizing)
            {
                Children.Add(new DataGridScrollViewer
                {
                    Name = "DG_ScrollViewer",
                    PresenterFor = this,
                    Width = "100%",
                    Height = "100%",
                    Content = new VirtualizationPresenter<DataGridRow>
                    {
                        Child = new StackPanel
                        {
                            Name = "itemsPanel",
                            PresenterFor = this,
                            MarginTop = 0,
                            MarginLeft = 0
                        },
                        Bindings =
                        {
                            {nameof(CustomScrollData),nameof(CustomScrollData),this }
                        }
                    },
                });
            }
            else
            {
                Children.Add(new DataGridScrollViewer
                {
                    Name = "DG_ScrollViewer",
                    PresenterFor = this,
                    Width = "100%",
                    Height = "100%",
                    Content = new StackPanel { Name = "itemsPanel", PresenterFor = this, MarginTop = 0, MarginLeft = 0 },
                });
            }
        }
        internal DataGridScrollViewer viewer;
        VirtualizationPresenter<DataGridRow> presenter;
        protected override void OnInitialized()
        {
            viewer = FindPresenter<DataGridScrollViewer>().FirstOrDefault(a => a.Name == "DG_ScrollViewer");
            if (!viewer)
            {
                throw new Exception("需要Name为DG_ScrollViewer的DataGridScrollViewer");
            }
            presenter = Find<VirtualizationPresenter<DataGridRow>>().FirstOrDefault();
            if (presenter && IsVirtualizing)
            {
                presenter.Items = Items;
                presenter.MultiSelector = this;
                if (Items.Count > 0)
                {
                    presenter.OnDataCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Items));
                }
                presenter.SetItem += Presenter_SetItem;
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
                            var i = ((DataGridRow)itemHost.Children[item]);
                            i.IsSetOnOwner = true;
                            i.IsSelected = true;
                            i.IsSetOnOwner = false;
                        }
                    }
                }
            }
            var columns = Columns;
            if (columns.Count > 0)
            {
                foreach (var item in columns)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    item.DisplayIndex = viewer.PART_ColumnHeadersPresenter.Children.Count;
                    viewer.PART_ColumnHeadersPresenter.Children.Add(item.HeaderElement);
                    item.DataGridOwner = this;
                    //item.DataContext = DataContext;
                    item[nameof(DataContext)] = new BindingDescribe(this, nameof(DataContext));
                    item[nameof(CommandContext)] = new BindingDescribe(this, nameof(CommandContext));
                    //item.CommandContext = CommandContext;
                }
            }
            columns.CollectionChanged += Columns_CollectionChanged;
            viewer.contentPresenter.LayoutUpdated += ContentPresenter_LayoutUpdated;
            base.OnInitialized();
            var panel = ItemsHost as StackPanel;
            if (!panel || panel.Orientation == Orientation.Horizontal)
            {
                throw new Exception("必须是Orientation为Vertical的StackPanel");
            }
            ItemsHost.LayoutUpdated += ItemsHost_LayoutUpdated;
        }
        [PropertyChanged(nameof(Columns))]
        void OnColumnsChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (newValue == null)
            {
                throw new Exception("Columns不能为null");
            }
            if (oldValue is Collection<DataGridColumn> cls)
            {
                cls.CollectionChanged -= Columns_CollectionChanged;
                foreach (var item in cls)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                    viewer.PART_ColumnHeadersPresenter.Children.Remove(item.HeaderElement);
                    item.DataGridOwner = null;
                }
            }

            var columns = newValue as Collection<DataGridColumn>;
            if (columns.Count > 0 && IsInitialized)
            {
                foreach (var item in columns)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    item.DisplayIndex = viewer.PART_ColumnHeadersPresenter.Children.Count;
                    viewer.PART_ColumnHeadersPresenter.Children.Add(item.HeaderElement);
                    item.DataGridOwner = this;
                    //item.DataContext = DataContext;
                    item[nameof(DataContext)] = new BindingDescribe(this, nameof(DataContext));
                    item[nameof(CommandContext)] = new BindingDescribe(this, nameof(CommandContext));
                    //item.CommandContext = CommandContext;
                }
            }
            columns.CollectionChanged += Columns_CollectionChanged;
        }

        private void Presenter_SetItem(object sender, DataGridRow e)
        {
            isSetEditCell = true;
            if (editCell != null && e.Index == editCell.RowIndex)
            {
                for (int i = 0; i < e.itemsPanel.Children.Count; i++)
                {
                    (e.itemsPanel.Children[i] as DataGridCell).IsEditing = i == editCell.CellIndex;
                }
            }
            else
            {
                foreach (DataGridCell item in e.itemsPanel.Children)
                {
                    item.IsEditing = false;
                }
            }
            isSetEditCell = false;
            var columns = Columns;
            //foreach (DataGridRow item in ElementItems)
            //{
            //    if (item.itemsPanel)
            //    {
            //        for (int i = 0; i < columns.Count; i++)
            //        {
            //            if (item.itemsPanel.Children.Count > i)
            //            {
            //                item.itemsPanel.Children[i].Width = columns[i].ActualWidth;
            //            }
            //        }
            //    }
            //}
            if (e.itemsPanel)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    if (e.itemsPanel.Children.Count > i)
                    {
                        e.itemsPanel.Children[i].Width = columns[i].ActualWidth;
                    }
                }
            }

            if (e.itemsPanel)
            {
                for (int i = 0; i < e.itemsPanel.Children.Count; i++)
                {
                    var cell = e.itemsPanel.Children[i] as DataGridCell;
                    var isSelect = false;
                    foreach (var item in selectedCells)
                    {
                        isSelect = e.Index == item.RowIndex && cell.Column == item.Column;
                        if (isSelect)
                        {
                            break;
                        }
                    }
                    cell.IsSelected = isSelect;
                }
            }
        }

        internal void SetEditCell(int rowIndex, int cellIndex)
        {
            if (editCell == null)
            {
                editCell = new EditCell();
            }
            editCell.CellIndex = cellIndex;
            editCell.RowIndex = rowIndex;
        }
        internal void SetEditCell()
        {
            editCell = null;
        }
        EditCell editCell;
        internal bool isSetEditCell;

        bool updateIndex = true;
        private void ItemsHost_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            UpdateIndex();
        }

        private void UpdateIndex()
        {
            if (updateIndex)
            {
                updateIndex = false;
                if (!presenter)
                {
                    var ac = this.AlternationCount;
                    var c = ItemsHost.Children;
                    for (int i = 0; i < c.Count; i++)
                    {
                        var item = c[i] as ISelectableItem;
                        if (ac != 0)
                        {
                            SetAlternationIndex((CpfObject)item, (int)(i % ac));
                        }
                        else
                        {
                            SetAlternationIndex((CpfObject)item, i);
                        }
                        item.Index = i;
                    }
                }
            }
        }

        //bool layoutcolums = false;
        private void ContentPresenter_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            //if (!layoutcolums)
            //{
            //    layoutcolums = true;
            //    BeginInvoke(() =>
            //    {
            //layoutcolums = false;
            var columns = Columns;
            var percent = columns.FirstOrDefault(a => a.Width.UnitType == DataGridLengthUnitType.Star) != null;
            if (percent)
            {
                var aw = viewer.contentPresenter.ActualSize.Width - 0.01f;
                var m = columns.Where(a => a.Width.UnitType == DataGridLengthUnitType.Default).Sum(a => a.Width.Value);
                var min = columns.Where(a => a.Width.UnitType == DataGridLengthUnitType.Star).Sum(a => a.MinWidth);
                if (m + min > aw)
                {
                    var s = aw - min;//剩余空间
                    var all = columns.Where(a => a.Width.UnitType == DataGridLengthUnitType.Default).Sum(a => a.Width.Value);//绝对宽度的总值
                    foreach (var item in columns)
                    {
                        if (item.Width.UnitType == DataGridLengthUnitType.Default)
                        {
                            item.ActualWidth = item.Width.Value * s / all;
                        }
                        else
                        {
                            item.ActualWidth = item.MinWidth;
                        }
                    }
                }
                else
                {
                    var s = aw - m;//剩余空间
                    var all = columns.Where(a => a.Width.UnitType == DataGridLengthUnitType.Star).Sum(a => a.Width.Value);//百分比总值
                    foreach (var item in columns)
                    {
                        if (item.Width.UnitType == DataGridLengthUnitType.Default)
                        {
                            item.ActualWidth = item.Width.Value;
                        }
                        else
                        {
                            item.ActualWidth = item.Width.Value * s / all;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in columns)
                {
                    if (item.HasLocalValue(nameof(item.ActualWidth)))
                    {
                        item.Width = item.ActualWidth;
                    }
                    else
                    {
                        item.ActualWidth = item.Width.Value;
                    }

                }
            }
            //    });
            //}

        }

        /// <summary>
        /// 虚拟模式下元素使用方式
        /// </summary>
        [PropertyMetadata(VirtualizationMode.Recycling)]
        public VirtualizationMode VirtualizationMode
        {
            get { return GetValue<VirtualizationMode>(); }
            set { SetValue(value); }
        }
        private void Item_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataGridColumn.ActualWidth))
            {
                var columns = Columns;
                var i = columns.IndexOf(sender as DataGridColumn);
                foreach (DataGridRow item in ElementItems)
                {
                    if (item.itemsPanel)
                    {
                        //for (int i = 0; i < columns.Count; i++)
                        //{
                        item.itemsPanel.Children[i].Width = columns[i].ActualWidth;
                        //}
                    }
                }
            }
            else if (e.PropertyName == nameof(DataGridColumn.Visibility))
            {
                var columns = Columns;
                var i = columns.IndexOf(sender as DataGridColumn);
                foreach (DataGridRow item in ElementItems)
                {
                    if (item.itemsPanel)
                    {
                        //for (int i = 0; i < columns.Count; i++)
                        //{
                        item.itemsPanel.Children[i].Visibility = columns[i].Visibility;
                        //}
                    }
                }
            }
            else if (e.PropertyName == nameof(DataGridColumn.SortDirection))
            {
                var so = (ListSortDirection?)e.NewValue;
                if (so != null)
                {
                    foreach (var item in Columns)
                    {
                        if (item != sender)
                        {
                            item.SortDirection = null;
                        }
                    }
                    var items = Items as ItemCollection;
                    var pn = (sender as DataGridColumn).SortMemberPath;
                    if (string.IsNullOrWhiteSpace(pn) && (sender as DataGridColumn).Binding != null)
                    {
                        pn = (sender as DataGridColumn).Binding.SourcePropertyName;
                    }
                    if (string.IsNullOrWhiteSpace(pn))
                    {
                        return;
                    }
                    var descending = so == ListSortDirection.Descending;
                    try
                    {
                        items.Sort(Comparer<object>.Default.Compare, pn, descending);
                    }
                    catch (Exception ee)
                    {
                        throw new Exception("排序出错", ee);
                    }
                }
            }
            else if (e.PropertyName == nameof(DataGridColumn.Width))
            {
                if (viewer && viewer.contentPresenter)
                {
                    viewer.contentPresenter.InvalidateArrange();
                }
            }
        }

        internal bool updateColIndex = false;
        private void Columns_CollectionChanged(object sender, CollectionChangedEventArgs<DataGridColumn> e)
        {
            if (!IsInitialized)
            {
                return;
            }
            if (viewer && viewer.contentPresenter)
            {
                viewer.contentPresenter.InvalidateArrange();
            }
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.DataGridOwner = this;
                    //e.NewItem.DataContext = DataContext;
                    e.NewItem[nameof(DataContext)] = new BindingDescribe(this, nameof(DataContext));
                    e.NewItem[nameof(CommandContext)] = new BindingDescribe(this, nameof(CommandContext));
                    //e.NewItem.CommandContext = CommandContext;
                    viewer.PART_ColumnHeadersPresenter.Children.Insert(e.Index, e.NewItem.HeaderElement);
                    e.NewItem.PropertyChanged += Item_PropertyChanged;
                    break;
                case CollectionChangedAction.Remove:
                    e.OldItem.DataGridOwner = null;
                    e.OldItem.PropertyChanged -= Item_PropertyChanged;
                    viewer.PART_ColumnHeadersPresenter.Children.RemoveAt(e.Index);
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.DataGridOwner = this;
                    //e.NewItem.DataContext = DataContext;
                    e.NewItem[nameof(DataContext)] = new BindingDescribe(this, nameof(DataContext));
                    e.NewItem[nameof(CommandContext)] = new BindingDescribe(this, nameof(CommandContext));
                    //e.NewItem.CommandContext = CommandContext;
                    viewer.PART_ColumnHeadersPresenter.Children[e.Index] = e.NewItem.HeaderElement;
                    e.OldItem.DataGridOwner = null;
                    e.OldItem.PropertyChanged -= Item_PropertyChanged;
                    break;
            }
            if (!updateColIndex)
            {
                updateColIndex = true;
                BeginInvoke(() =>
                {
                    var columns = Columns;
                    for (int i = 0; i < columns.Count; i++)
                    {
                        columns[i].DisplayIndex = i;
                    }
                    updateColIndex = false;
                });
            }
        }
        protected override void OnItemElementAdded(UIElement element)
        {
            element.MouseDown += Element_MouseDown;
        }
        protected override void OnItemElementRemoved(UIElement element)
        {
            element.MouseDown -= Element_MouseDown;
        }

        int shiftLastId = -1;
        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectionUnit == DataGridSelectionUnit.Cell || e.MouseButton != MouseButton.Left)
            {
                return;
            }
            var item = sender as DataGridRow;
            {
                //var selectIndexs = SelectedIndexs;
                //var selectItems = SelectedItems;
                switch (SelectionMode)
                {
                    case DataGridSelectionMode.Extended:
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
                    case DataGridSelectionMode.Single:
                        SelectedIndexs.Clear();
                        SelectedIndexs.Add(item.Index);
                        break;
                }
            }
        }
        //protected override bool IsItemElement(object item)
        //{
        //    return item is DataGridRow;
        //}
        public override DataGridRow CreateItemElement()
        {
            var row = ItemTemplate.CreateElement();
            row.DataGridOwner = this;
            return row;
        }

        [PropertyChanged(nameof(Items))]
        void OnItems(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (presenter)
            {
                presenter.Items = newValue as IList;
            }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Items) && AutoGenerateColumns)
            {
                Items.Clear();
                Columns.Clear();
                if (value != null)
                {
                    //if (value is DataRows rows)
                    //{
                    //    foreach (var item in rows.DataTable.Columns)
                    //    {

                    //    }
                    //}
                    if (value is IList list)
                    {
                        if (list.Count > 0)
                        {
                            var v = list[0];
                            if (v is CpfObject cpf)
                            {
                                foreach (var item in cpf.GetProperties())
                                {
                                    CreateColumn(item.PropertyType, item.PropertyName);
                                }
                            }
                            else
                            {
                                var t = v.GetType();
                                var ps = t.GetProperties();
                                foreach (var item in ps)
                                {
                                    CreateColumn(item.PropertyType, item.Name);
                                }
                            }
                        }
                        else if (list.Count == 0)
                        {
                            var t = list.GetType();
                            if (t.IsArray)
                            {
                                var ps = t.GetElementType().GetProperties();
                                foreach (var item in ps)
                                {
                                    CreateColumn(item.PropertyType, item.Name);
                                }
                            }
                            else
                            {
                                var gs = t.GetGenericArguments();
                                if (gs != null && gs.Length == 1)
                                {
                                    var ps = gs[0].GetProperties();
                                    foreach (var item in ps)
                                    {
                                        CreateColumn(item.PropertyType, item.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        private void CreateColumn(Type PropertyType, string PropertyName)
        {
            DataGridColumn column;

            if (PropertyType == typeof(bool))
            {
                column = new DataGridCheckBoxColumn { };
            }
            else if (PropertyType.IsEnum)
            {
                //var kp = new List<(string, object)>();
                //var vs = Enum.GetValues(item.PropertyType);
                //foreach (var vv in vs)
                //{
                //    kp.Add((Enum.GetName(item.PropertyType, vv), vv));
                //}
                //column = new DataGridComboBoxColumn { DisplayMemberPath = "Item1", SelectedValuePath = "Item2", Items = kp };
                column = new DataGridComboBoxColumn { Items = Enum.GetValues(PropertyType) };
            }
            else
            {
                column = new DataGridTextColumn { };
            }
            column.Width = 100;
            column.Binding = PropertyName;
            column.Header = PropertyName;
            var auto = new DataGridAutoGeneratingColumnEventArgs(column, PropertyName, PropertyType);
            OnAutoGeneratingColumn(auto);
            if (!auto.Cancel && auto.Column != null)
            {
                Columns.Add(auto.Column);
            }
        }



        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(Items) && presenter)
        //    {
        //        presenter.Items = newValue as IList;
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}


        object[] select = null;
        protected override void OnCollectionStartSort()
        {
            foreach (var item in selectedCells)
            {
                if (presenter)
                {
                    if (item.RowIndex >= (ItemsHost.Children[0] as DataGridRow).Index && item.RowIndex <= (ItemsHost.Children[ItemsHost.Children.Count - 1] as DataGridRow).Index)
                    {
                        var row = ItemsHost.Children[item.RowIndex - (ItemsHost.Children[0] as DataGridRow).Index] as DataGridRow;
                        if (row != null)
                        {
                            if (row.itemsPanel && item.Column.DisplayIndex < row.itemsPanel.Children.Count)
                            {
                                (row.itemsPanel.Children[item.Column.DisplayIndex] as DataGridCell).IsSelected = false;
                            }
                        }
                    }
                }
                else
                {
                    if (ItemsHost.Children.Count > item.RowIndex)
                    {
                        var row = ItemsHost.Children[item.RowIndex] as DataGridRow;
                        if (row.itemsPanel && item.Column.DisplayIndex < row.itemsPanel.Children.Count)
                        {
                            (row.itemsPanel.Children[item.Column.DisplayIndex] as DataGridCell).IsSelected = true;
                        }
                    }
                }
            }
            selectedCells.Clear();
            select = null;
            if (presenter)
            {
                var items = Items;
                if (SelectedIndexs.Count > 0)
                {
                    //select = SelectedIndexs.Select(a => items[a]).ToArray();
                    if (items.Count > 0)
                    {
                        List<object> list = new List<object>();
                        foreach (var item in SelectedIndexs)
                        {
                            if (item >= items.Count || item < 0)
                            {
                                continue;
                            }
                            list.Add(items[item]);
                        }
                        if (list.Count > 0)
                        {
                            select = list.ToArray();
                        }
                    }
                }
                presenter.OnCollectionStartSort();
            }
        }

        protected override void OnCollectionSorted()
        {
            if (presenter)
            {
                presenter.OnCollectionSorted();
                if (select != null)
                {
                    var items = Items;
                    SelectedIndexs.Clear();
                    Dictionary<object, int> indexs = new Dictionary<object, int>();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (!indexs.ContainsKey(items[i]))
                        {
                            indexs.Add(items[i], i);
                        }
                    }
                    foreach (var item in select)
                    {
                        SelectedIndexs.Add(indexs[item]);
                    }
                    select = null;
                }
            }
            else
            {
                DataGridRow[] select = null;
                if (SelectedIndexs.Count > 0)
                {
                    select = SelectedIndexs.Select(a => ItemsHost.Children[a] as DataGridRow).ToArray();
                }
                base.OnCollectionSorted();
                updateIndex = true;
                UpdateIndex();
                if (select != null)
                {
                    SelectedIndexs.Clear();
                    foreach (var item in select)
                    {
                        SelectedIndexs.Add(item.Index);
                    }
                }
            }
        }

        protected override void OnDataCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (presenter)
            {
                presenter.OnDataCollectionChanged(e);
            }
            else
            {
                updateIndex = true;
                base.OnDataCollectionChanged(e);
                if (ItemsHost)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                        case NotifyCollectionChangedAction.Replace:
                            if (e.NewStartingIndex >= 0 && e.NewStartingIndex < ItemsHost.Children.Count)
                            {
                                var columns = Columns;
                                var item = ItemsHost.Children[e.NewStartingIndex] as DataGridRow;
                                for (int i = 0; i < columns.Count; i++)
                                {
                                    if (item.itemsPanel)
                                    {
                                        item.itemsPanel.Children[i].Width = columns[i].ActualWidth;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        internal void OnCellMouseEnter(DataGridCellMouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed && SelectionUnit == DataGridSelectionUnit.Cell && SelectionMode == DataGridSelectionMode.Extended)
            {
                SelectRangeCell(args);
            }
        }

        private void SelectRangeCell(DataGridCellMouseEventArgs args)
        {
            if (selectedCells.Count > 0)
            {
                var temp = new List<DataGridCellInfo>();
                HashSet<DataGridCellInfo> h = new HashSet<DataGridCellInfo>();
                if (selectedCells.Count > 1)
                {
                    for (int i = 1; i < selectedCells.Count; i++)
                    {
                        var item = selectedCells[i];
                        temp.Add(item);
                    }
                    selectedCells.RemoveRange(1, selectedCells.Count - 1);
                }
                var columns = Columns;
                var startIndex = -1;
                var index = 0;
                for (int r = Math.Min(selectedCells[0].RowIndex, args.Cell.Row.Index); r <= Math.Max(selectedCells[0].RowIndex, args.Cell.Row.Index); r++)
                {
                    for (int c = Math.Min(selectedCells[0].Column.DisplayIndex, args.Cell.Column.DisplayIndex); c <= Math.Max(selectedCells[0].Column.DisplayIndex, args.Cell.Column.DisplayIndex); c++)
                    {
                        if (!(selectedCells[0].Column == columns[c] && r == selectedCells[0].RowIndex))
                        {
                            var t = new DataGridCellInfo(r, columns[c]);
                            selectedCells.Add(t);
                            h.Add(t);
                            if (startIndex == -1)
                            {
                                if (presenter)
                                {
                                    if (r >= (ItemsHost.Children[0] as DataGridRow).Index && r <= (ItemsHost.Children[ItemsHost.Children.Count - 1] as DataGridRow).Index)
                                    {
                                        var row = ItemsHost.Children.Select((a, i) => new { row = a as DataGridRow, index = i }).FirstOrDefault(a => a.row.Index == r);
                                        if (row != null)
                                        {
                                            startIndex = row.index;
                                        }
                                    }
                                }
                                else
                                {
                                    startIndex = r;
                                }
                            }
                            if (startIndex != -1 && ItemsHost.Children.Count > startIndex + index)
                            {
                                var row = ItemsHost.Children[startIndex + index] as DataGridRow;
                                if (row.itemsPanel && c < row.itemsPanel.Children.Count)
                                {
                                    (row.itemsPanel.Children[c] as DataGridCell).IsSelected = true;
                                }
                            }
                        }
                    }
                    if (startIndex != -1)
                    {
                        index++;
                    }
                }

                foreach (var item in temp)
                {
                    if (!h.Contains(item))
                    {
                        if (presenter)
                        {
                            if (item.RowIndex >= (ItemsHost.Children[0] as DataGridRow).Index && item.RowIndex <= (ItemsHost.Children[ItemsHost.Children.Count - 1] as DataGridRow).Index)
                            {
                                var row = ItemsHost.Children[item.RowIndex - (ItemsHost.Children[0] as DataGridRow).Index] as DataGridRow;
                                if (row != null)
                                {
                                    if (row.itemsPanel && item.Column.DisplayIndex < row.itemsPanel.Children.Count)
                                    {
                                        (row.itemsPanel.Children[item.Column.DisplayIndex] as DataGridCell).IsSelected = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ItemsHost.Children.Count > item.RowIndex)
                            {
                                var row = ItemsHost.Children[item.RowIndex] as DataGridRow;
                                if (row.itemsPanel && item.Column.DisplayIndex < row.itemsPanel.Children.Count)
                                {
                                    (row.itemsPanel.Children[item.Column.DisplayIndex] as DataGridCell).IsSelected = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        DataGridCell mouseDonwCell;
        internal protected virtual void OnCellMouseDown(DataGridCellMouseEventArgs args)
        {
            if (SelectionUnit == DataGridSelectionUnit.Cell && args.MouseButton == MouseButton.Left)
            {
                if (SelectionMode == DataGridSelectionMode.Extended && args.Device.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Control))
                {
                    args.Cell.IsSelected = !args.Cell.IsSelected;
                    if (args.Cell.IsSelected)
                    {
                        selectedCells.Add(new DataGridCellInfo(args.Cell.Row.Index, args.Cell.Column));
                    }
                    else
                    {
                        selectedCells.Remove(new DataGridCellInfo(args.Cell.Row.Index, args.Cell.Column));
                    }
                }
                else if (SelectionMode == DataGridSelectionMode.Extended && args.Device.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Shift))
                {
                    SelectRangeCell(args);
                }
                else
                {
                    if (selectedCells.Count > 0)
                    {
                        foreach (var item in selectedCells)
                        {
                            var row = ItemsHost.Children.Select(a => a as DataGridRow).FirstOrDefault(a => a.Index == item.RowIndex);
                            if (row != null && row.itemsPanel && item.Column.DisplayIndex < row.itemsPanel.Children.Count && item.Column.DisplayIndex > -1)
                            {
                                (row.itemsPanel.Children[item.Column.DisplayIndex] as DataGridCell).IsSelected = false;
                            }
                        }
                        selectedCells.Clear();
                    }
                    args.Cell.IsSelected = true;
                    selectedCells.Add(new DataGridCellInfo(args.Cell.Row.Index, args.Cell.Column));
                }
            }
            mouseDonwCell = args.Cell;
            RaiseEvent(args, nameof(CellMouseDown));
        }

        public event EventHandler<DataGridCellMouseEventArgs> CellMouseDown
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnCellMouseUp(DataGridCellMouseEventArgs args)
        {
            RaiseEvent(args, nameof(CellMouseUp));
            if (!args.Handled && mouseDonwCell == args.Cell)
            {
                OnCellClick(new DataGridCellEventArgs(args.Cell));
            }
            mouseDonwCell = null;
        }

        public event EventHandler<DataGridCellMouseEventArgs> CellMouseUp
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnCellDoubleClick(DataGridCellEventArgs args)
        {
            RaiseEvent(args, nameof(CellDoubleClick));
        }

        public event EventHandler<DataGridCellEventArgs> CellDoubleClick
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnCellClick(DataGridCellEventArgs args)
        {
            RaiseEvent(args, nameof(CellClick));
        }

        public event EventHandler<DataGridCellEventArgs> CellClick
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs args)
        {
            RaiseEvent(args, nameof(BeginningEdit));
        }
        /// <summary>
        /// 在行或单元格进入编辑模式之前发生。
        /// </summary>
        public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        internal protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs args)
        {
            RaiseEvent(args, nameof(CellEditEnding));
        }
        /// <summary>
        /// 在提交或取消单元格编辑之前发生。
        /// </summary>
        public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)typeof(StackPanel)));
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<DataGridRow>)typeof(DataGridRow)));
        }

        public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs eventArgs)
        {
            this.RaiseEvent(eventArgs, nameof(AutoGeneratingColumn));
        }

        class EditCell
        {
            public int RowIndex;
            public int CellIndex;
        }
    }

    public class DataGridCellMouseEventArgs : MouseButtonEventArgs
    {
        public DataGridCellMouseEventArgs(DataGridCell cell, UIElement source, bool LeftButtonDown, bool RightButtonDown, bool MiddleButtonDown, Point location, MouseDevice mouseDevice, MouseButton mouseButton, bool isTouch) : base(source, LeftButtonDown, RightButtonDown, MiddleButtonDown, location, mouseDevice, mouseButton, isTouch)
        {
            Cell = cell;
        }

        public DataGridCell Cell { get; internal set; }
    }

    public class DataGridCellEventArgs : RoutedEventArgs
    {
        public DataGridCellEventArgs(DataGridCell cell)
        {
            Cell = cell;
        }

        public DataGridCell Cell { get; internal set; }
    }


    public class DataGridBeginningEditEventArgs : EventArgs
    {
        public DataGridBeginningEditEventArgs(DataGridColumn column, DataGridCell cell)
        {
            this.Column = column;
            this.Cell = cell;
        }

        /// <summary>
        /// 获取或设置指示是否应取消事件的值。
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// 获取包含要编辑的单元格的列
        /// </summary>
        public DataGridColumn Column { get; private set; }
        ///// <summary>
        ///// 获取包含要编辑的单元格的行。
        ///// </summary>
        //public DataGridRow Row { get; private set; }
        /// <summary>
        /// 获取包含要编辑的单元格的
        /// </summary>
        public DataGridCell Cell { get; private set; }
    }
    public class DataGridCellEditEndingEventArgs : EventArgs
    {
        public DataGridCellEditEndingEventArgs(DataGridColumn column, DataGridCell cell, DataGridCellTemplate editingElement)
        {
            this.Column = column;
            this.Cell = cell;
            this.EditingElement = editingElement;
        }

        /// <summary>
        /// 获取包含要编辑的单元格的列
        /// </summary>
        public DataGridColumn Column { get; private set; }
        ///// <summary>
        ///// 获取包含要编辑的单元格的行。
        ///// </summary>
        //public DataGridRow Row { get; private set; }
        /// <summary>
        /// 获取包含要编辑的单元格的
        /// </summary>
        public DataGridCell Cell { get; private set; }
        /// <summary>
        /// 获取单元格在编辑模式中显示的元素。
        /// </summary>
        public DataGridCellTemplate EditingElement { get; private set; }
    }
    public class DataGridAutoGeneratingColumnEventArgs : EventArgs
    {
        public DataGridAutoGeneratingColumnEventArgs(DataGridColumn column, string propertyName, Type propertyType)
        {
            Column = column;
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
        public bool Cancel { get; set; }

        public DataGridColumn Column { get; set; }

        public string PropertyName { get; }

        public Type PropertyType { get; }
    }
}
