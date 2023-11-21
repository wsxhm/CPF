using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CPF.Controls
{
    public abstract class DataGridColumn : CpfObject
    {
        public float ActualWidth
        {
            get { return GetValue<float>(); }
            internal set { SetValue(value); }
        }

        [PropertyMetadata(typeof(DataGridLength), "auto")]
        public DataGridLength Width
        {
            get { return GetValue<DataGridLength>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(5000f)]
        public float MaxWidth
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(6f)]
        public float MinWidth
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(Visibility.Visible)]
        public Visibility Visibility
        {
            get { return GetValue<Visibility>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个撰写字符串，该字符串指定如果 Header 属性显示为字符串，应如何设置该属性的格式.String.Format
        /// </summary>
        public string HeaderStringFormat
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置属性名称,它指示作为排序依据的成员
        /// </summary>
        public string SortMemberPath
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置模板元素名称
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        //public bool IsReadOnly
        //{
        //    get { return GetValue<bool>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 获取或设置列标题的内容。
        /// </summary>
        public object Header
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        public object Tag
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置该列相对于 DataGrid 中其他列的显示位置。当列显示在关联的 DataGrid 中时，该列从零开始的位置。
        /// </summary>
        [PropertyMetadata(-1)]
        public int DisplayIndex
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置列的排序方向(升序或降序)。
        /// </summary>
        [PropertyMetadata(null)]
        public ListSortDirection? SortDirection
        {
            get { return GetValue<ListSortDirection?>(); }
            set { SetValue(value); }
        }

        public UIElementTemplate<DataGridColumnTemplate> HeaderTemplate
        {
            get { return GetValue<UIElementTemplate<DataGridColumnTemplate>>(); }
            set { SetValue(value); }
        }

        UIElement headerElement;
        /// <summary>
        /// 获取和列关联的UIElement
        /// </summary>
        public UIElement HeaderElement
        {
            get
            {
                if (headerElement == null)
                {
                    var he = HeaderTemplate.CreateElement();
                    he.Column = this;
                    headerElement = he;
                    _ = headerElement[nameof(ContentControl.ContentStringFormat)] <= this[nameof(HeaderStringFormat)];
                    _ = headerElement[nameof(ContentControl.Content)] <= this[nameof(Header)];
                    _ = headerElement[nameof(Name)] <= this[nameof(Name)];
                    _ = headerElement[nameof(ContentControl.Visibility)] <= (this, nameof(Visibility));
                    //_ = headerElement[nameof(ContentControl.Width)] <= this[nameof(ActualWidth), a => (FloatField)(float)a];
                    //_ = headerElement[nameof(ContentControl.MinWidth)] <= this[nameof(MinWidth), a => (FloatField)(float)a];
                    //_ = headerElement[nameof(ContentControl.MaxWidth)] <= this[nameof(MaxWidth), a => (FloatField)(float)a];
                    headerElement[nameof(ContentControl.Width)] = (this, nameof(ActualWidth), a => (FloatField)(float)a);
                    headerElement[nameof(ContentControl.MinWidth)] = (this, nameof(MinWidth), a => (FloatField)(float)a);
                    headerElement[nameof(ContentControl.MaxWidth)] = (this, nameof(MaxWidth), a => (FloatField)(float)a);
                }
                return headerElement;
            }
        }

        //private void HeaderElement_LayoutUpdated(object sender, RoutedEventArgs e)
        //{
        //    ActualWidth = headerElement.ActualSize.Width;
        //}
        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(ActualWidth))
            {
                var v = (float)value;
                v = Math.Min(v, MaxWidth);
                value = Math.Max(v, MinWidth);
            }
            else if (propertyName == nameof(MaxWidth))
            {
                if ((float)value < MinWidth || (float)value < 0)
                {
                    throw new Exception("MaxWidth必须大于0和MinWidth");
                }
            }
            else if (propertyName == nameof(MinWidth))
            {
                if ((float)value > MaxWidth || (float)value < 0)
                {
                    throw new Exception("MinWidth必须大于0和小于MaxWidth");
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        /// <summary>
        /// 获取包含此列的 DataGrid 控件。
        /// </summary>
        [NotCpfProperty]
        public DataGrid DataGridOwner
        {
            get; internal set;
        }
        /// <summary>
        /// 获取或设置一个值，该值指示用户是否可使用鼠标调整列宽。
        /// </summary>
        [PropertyMetadata(true)]
        public bool CanUserResize
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示用户能否通过单击列标题对列进行排序
        /// </summary>
        [PropertyMetadata(true)]
        public bool CanUserSort
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        ///// <summary>
        ///// 单元格模板
        ///// </summary>
        //public UIElementTemplate<DataGridCellTemplate> CellTemplate
        //{
        //    get { return GetValue<UIElementTemplate<DataGridCellTemplate>>(); }
        //    set { SetValue(value); }
        //}
        ///// <summary>
        ///// 编辑模式下的单元格模板
        ///// </summary>
        //public UIElementTemplate<DataGridCellTemplate> CellEditingTemplate
        //{
        //    get { return GetValue<UIElementTemplate<DataGridCellTemplate>>(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 绑定到数据源的属性或者数据表列的名称
        /// </summary>
        public DataGridBinding Binding
        {
            get { return GetValue<DataGridBinding>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 当在派生类中被重写时，获取一个只读元素，该元素绑定到该列的 Binding 属性值。
        /// </summary>
        /// <returns></returns>
        public abstract DataGridCellTemplate GenerateElement();
        /// <summary>
        /// 当在派生类中被重写时，获取一个编辑元素，该元素绑定到该列的 Binding 属性值。
        /// </summary>
        /// <returns></returns>
        public abstract DataGridCellTemplate GenerateEditingElement();

        [PropertyChanged(nameof(DisplayIndex))]
        void RegisterDisplayIndex(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (DataGridOwner != null && !DataGridOwner.updateColIndex)
            {
                DataGridOwner.Columns.Remove(this);
                DataGridOwner.Columns.Insert((int)newValue, this);
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(DisplayIndex))
        //    {
        //        if (DataGridOwner != null && !DataGridOwner.updateColIndex)
        //        {
        //            DataGridOwner.Columns.Remove(this);
        //            DataGridOwner.Columns.Insert((int)newValue, this);
        //        }
        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            //overridePropertys.Override(nameof(CellTemplate), new PropertyMetadataAttribute((UIElementTemplate<DataGridCellTemplate>)typeof(DataGridCellTemplate)));
            //overridePropertys.Override(nameof(CellEditingTemplate), new PropertyMetadataAttribute((UIElementTemplate<DataGridCellTemplate>)typeof(DataGridCellTextEditTemplate)));
            overridePropertys.Override(nameof(HeaderTemplate), new PropertyMetadataAttribute((UIElementTemplate<DataGridColumnTemplate>)typeof(DataGridColumnTemplate)));

        }
    }
    /// <summary>
    /// 定义DataGrid的数据绑定
    /// </summary>
    public class DataGridBinding
    {
        public DataGridBinding(string sourcePropertyName)
        {
            SourcePropertyName = sourcePropertyName;
        }
        public DataGridBinding(string sourcePropertyName, BindingMode bindingMode)
        {
            BindingMode = bindingMode;
            SourcePropertyName = sourcePropertyName;
        }
        /// <summary>
        /// 如果定义了双向绑定的转换器，两个转换器必须对应，否则可能会出现死循环
        /// </summary>
        /// <param name="sourcePropertyName"></param>
        /// <param name="bindingMode"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        public DataGridBinding(string sourcePropertyName, BindingMode bindingMode, Func<object, object> convert, Func<object, object> convertBack)
        {
            BindingMode = bindingMode;
            SourcePropertyName = sourcePropertyName;
            Convert = convert;
            ConvertBack = convertBack;
        }
        /// <summary>
        /// 绑定模式
        /// </summary>
        public BindingMode BindingMode
        {
            get;
            set;
        } = BindingMode.OneWay;
        public string SourcePropertyName { get; set; }

        /// <summary>
        /// 数据绑定的转换
        /// </summary>
        public Func<object, object> Convert { get; set; }
        /// <summary>
        /// 数据绑定的转换，转换回数据源
        /// </summary>
        public Func<object, object> ConvertBack { get; set; }

        //public static DataGridBinding Null
        //{
        //    get { return new DataGridBinding(null); }
        //}

        //public static implicit operator DataGridBinding(string n)
        //{
        //    if (string.IsNullOrWhiteSpace(n))
        //    {
        //        return Null;
        //    }
        //    return new DataGridBinding(n);
        //}
        /// <summary>
        /// 设置绑定的数据源属性名，单向绑定
        /// </summary>
        /// <param name="sourcePropertyName">数据源属性名</param>
        public static implicit operator DataGridBinding(string sourcePropertyName)
        {
            return new DataGridBinding(sourcePropertyName);
        }
    }

    //public enum ListSortDirection : byte
    //{
    //    Ascending,
    //    Descending,
    //}
}
