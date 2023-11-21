using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 DataGrid 控件的单元格。
    /// </summary>
    [Description("表示 DataGrid 控件的单元格。"), Browsable(false)]
    public class DataGridCell : Control
    {
        [NotCpfProperty]
        public DataGridColumn Column { get; internal set; }

        [NotCpfProperty]
        public DataGridRow Row { get; internal set; }

        /// <summary>
        /// 尝试获取单元格值
        /// </summary>
        /// <returns></returns>
        public object GetCellValue()
        {
            if (Column != null && Column.Binding != null && DataContext != null)
            {
                return DataContext.GetPropretyValue(Column.Binding.SourcePropertyName);
            }
            return null;
        }

        /// <summary>
        /// 是否是编辑模式
        /// </summary>
        public bool IsEditing
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public bool IsReadOnly
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        public string ColumnName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(IsEditing) && (bool)value)
            {
                if (IsReadOnly || Column.Binding != null && Column.Binding.BindingMode != BindingMode.TwoWay)
                {
                    return false;
                }
                var start = new DataGridBeginningEditEventArgs(Column, this);
                Column.DataGridOwner.OnBeginningEdit(start);
                if (start.Cancel)
                {
                    return false;
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        protected override void InitializeComponent()
        {
            var ele = Column.GenerateElement();
            ele.Cell = this;
            ele.PresenterFor = this;
            ele.Name = "CellTemplate";
            Size = new SizeField("100%", "100%");
            Children.Add(ele);
            Bindings.Add(nameof(ColumnName), nameof(Column.Name), Column);
            Triggers.Add(nameof(IsSelected), Relation.Me, null, (nameof(Background), "75,178,255"));
        }
        DataGridCellTemplate cellTemplate;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            cellTemplate = FindPresenter<DataGridCellTemplate>().FirstOrDefault(a => a.Name == "CellTemplate");
        }

        [PropertyChanged(nameof(IsEditing))]
        void OnIsEditing(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var v = (bool)newValue;
            if (v)
            {
                if (!this.Column.DataGridOwner.isSetEditCell)
                {
                    Column.DataGridOwner.SetEditCell(Row.Index, Row.itemsPanel.Children.IndexOf(this));
                }
                BeginInvoke(() =>
                {
                    if (cellTemplate)
                    {
                        cellTemplate.Dispose();
                    }
                    cellTemplate = Column.GenerateEditingElement();
                    cellTemplate.Cell = this;
                    cellTemplate.PresenterFor = this;
                    cellTemplate.Name = "CellTemplate";
                    Children.Add(cellTemplate);
                });
            }
            else
            {
                if (!this.Column.DataGridOwner.isSetEditCell)
                {
                    this.Column.DataGridOwner.SetEditCell();
                }
                Column.DataGridOwner.OnCellEditEnding(new DataGridCellEditEndingEventArgs(Column, this, cellTemplate));
                BeginInvoke(() =>
                {
                    if (cellTemplate)
                    {
                        cellTemplate.Dispose();
                    }
                    cellTemplate = Column.GenerateElement();
                    cellTemplate.Cell = this;
                    cellTemplate.PresenterFor = this;
                    cellTemplate.Name = "CellTemplate";
                    Children.Add(cellTemplate);
                });
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(IsSelected) && !(bool)newValue)
        //    {

        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute((FloatField)"100%", UIPropertyOptions.AffectsMeasure));
        }
    }
}
