using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// DataGrid的一行
    /// </summary>
    [Description("DataGrid的一行"), Browsable(false)]
    public class DataGridRow : Control, ISelectableItem
    {
        /// <summary>
        /// 获取包含此列的 DataGrid 控件。
        /// </summary>
        [NotCpfProperty]
        public DataGrid DataGridOwner
        {
            get; internal set;
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 在表格中的索引
        /// </summary>
        public int Index
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        [NotCpfProperty]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSetOnOwner { get; set; }

        ///// <summary>
        ///// 定义布局容器，初始化或者附加到可视化树之前设置
        ///// </summary>
        //public UIElementTemplate<Panel> ItemsPanel
        //{
        //    get { return GetValue<UIElementTemplate<Panel>>(); }
        //    set { SetValue(value); }
        //}
        protected override void InitializeComponent()
        {
            BorderType = BorderType.BorderThickness;
            BorderThickness = new Thickness(0, 0, 0, 1);
            BorderFill = "#000";
            Children.Add(new StackPanel { Orientation = Orientation.Horizontal, Name = "itemsPanel", PresenterFor = this, Size = new SizeField("100%", "100%") });
            Triggers.Add(new Styling.Trigger { Property = nameof(IsSelected), Setters = { { nameof(Background), "#ddd" } } });
        }
        internal Panel itemsPanel;
        protected override void OnInitialized()
        {
            itemsPanel = FindPresenter<Panel>().FirstOrDefault(a => a.Name == "itemsPanel");
            if (!itemsPanel)
            {
                throw new Exception("未定义itemsPanel");
            }
            if (DataGridOwner != null)
            {
                foreach (var item in DataGridOwner.Columns)
                {
                    itemsPanel.Children.Add(new DataGridCell { Column = item, Row = this, Width = item.ActualWidth, Visibility = item.Visibility });
                }
            }

            base.OnInitialized();
        }

        [PropertyChanged(nameof(IsSelected))]
        void OnIsSelected(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var v = (bool)newValue;
            if (!IsSetOnOwner && DataGridOwner != null)
            {
                if (v)
                {
                    DataGridOwner.SelectedIndexs.Add(Index);
                }
                else
                {
                    DataGridOwner.SelectedIndexs.Remove(Index);
                }
            }
        }
        /// <summary>
        /// 内部DataGridCell
        /// </summary>
        [NotCpfProperty]
        public IEnumerable<DataGridCell> Cells
        {
            get { return itemsPanel.Children.Select(a => a as DataGridCell); }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(MarginLeft), new UIPropertyMetadataAttribute((FloatField)0, UIPropertyOptions.AffectsArrange));
            //overridePropertys.Override(nameof(ItemsPanel), new PropertyMetadataAttribute((UIElementTemplate<Panel>)new StackPanel { Orientation = Orientation.Horizontal }));
        }
    }
}
