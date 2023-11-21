using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Runtime.CompilerServices;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 网格布局
    /// </summary>
    [Description("网格布局")]
    public class Grid : Panel
    {
        Collection<ColumnDefinition> columnDefinitions = new Collection<ColumnDefinition>();
        Collection<RowDefinition> rowDefinitions = new Collection<RowDefinition>();

        /// <summary>
        /// 网格布局
        /// </summary>
        public Grid()
        {
            columnDefinitions.CollectionChanged += ColumnDefinitions_CollectionChanged;
            //columnDefinitions.ItemRemoved += ColumnDefinitions_ItemRemoved;
            rowDefinitions.CollectionChanged += RowDefinitions_CollectionChanged;
            //rowDefinitions.ItemRemoved += RowDefinitions_ItemRemoved;
        }

        private void RowDefinitions_CollectionChanged(object sender, CollectionChangedEventArgs<RowDefinition> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.PropertyChanged += Item_PropertyChanged;
                    e.NewItem[nameof(DataContext)] = new (this, nameof(DataContext));
                    e.NewItem[nameof(CommandContext)] = new (this, nameof(CommandContext));
                    break;
                case CollectionChangedAction.Remove:
                    e.OldItem.PropertyChanged -= Item_PropertyChanged;
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.PropertyChanged += Item_PropertyChanged;
                    e.NewItem[nameof(DataContext)] = new (this, nameof(DataContext));
                    e.NewItem[nameof(CommandContext)] = new (this, nameof(CommandContext));
                    e.OldItem.PropertyChanged -= Item_PropertyChanged;
                    break;
                default:
                    break;
            }
            InvalidateMeasure();
        }

        private void ColumnDefinitions_CollectionChanged(object sender, CollectionChangedEventArgs<ColumnDefinition> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.PropertyChanged += Item_PropertyChanged;
                    e.NewItem[nameof(DataContext)] = new BindingDescribe(this, nameof(DataContext));
                    e.NewItem[nameof(CommandContext)] = new BindingDescribe(this, nameof(CommandContext));
                    break;
                case CollectionChangedAction.Remove:
                    e.OldItem.PropertyChanged -= Item_PropertyChanged;
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.PropertyChanged += Item_PropertyChanged;
                    e.NewItem[nameof(DataContext)] = new BindingDescribe(this, nameof(DataContext));
                    e.NewItem[nameof(CommandContext)] = new BindingDescribe(this, nameof(CommandContext));
                    e.OldItem.PropertyChanged -= Item_PropertyChanged;
                    break;
            }
            InvalidateMeasure();
        }

        private void Item_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            InvalidateMeasure();
        }
        /// <summary>
        /// 列，在CSS里设置 ColumnDefinitions-索引-Width:'*';  值包含*的情况下要加引号
        /// </summary>
        [NotCpfProperty, Category("设计")]
        public Collection<ColumnDefinition> ColumnDefinitions
        {
            get
            {
                return columnDefinitions;
            }
        }
        /// <summary>
        /// 行，在CSS里设置 RowDefinitions-索引-Height:'*';  值包含*的情况下要加引号
        /// </summary>
        [NotCpfProperty, Category("设计")]
        public Collection<RowDefinition> RowDefinitions
        {
            get
            {
                return rowDefinitions;
            }
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            if (rowDefinitions.Count == 0 && columnDefinitions.Count == 0)
            {
                return base.MeasureOverride(availableSize);
            }
            else
            {
                if (rowDefinitions.Count == 0)
                {
                    rowDefinitions.Add(new RowDefinition { });
                }
                if (columnDefinitions.Count == 0)
                {
                    columnDefinitions.Add(new ColumnDefinition { });
                }
                var allWidth = 0f;
                var allHeight = 0f;
                var allWStar = 0f;
                var allHStar = 0f;

                //先计算绝对值的
                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    var item = columnDefinitions[i];
                    var w = item.Width;
                    if (w.IsAbsolute)
                    {
                        item.ActualWidth = w.Value;
                        if (!item.MaxWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Min(item.ActualWidth, item.MaxWidth.GetActualValue(availableSize.Width));
                        }
                        if (!item.MinWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Max(item.ActualWidth, item.MinWidth.GetActualValue(availableSize.Width));
                        }
                    }
                    else// if (w.IsAuto)
                    {
                        item.ActualWidth = 0;
                    }
                    if (w.IsStar)
                    {
                        allWStar += w.Value;
                    }
                    allWidth += item.ActualWidth;
                    if (item.UIElements == null)
                    {
                        item.UIElements = new List<UIElement>();
                    }
                    item.UIElements.Clear();
                }

                for (int i = 0; i < rowDefinitions.Count; i++)
                {
                    var item = rowDefinitions[i];
                    var h = item.Height;
                    if (h.IsAbsolute)
                    {
                        item.ActualHeight = h.Value;
                        if (!item.MaxHeight.IsAuto)
                        {
                            item.ActualHeight = Math.Min(item.ActualHeight, item.MaxHeight.GetActualValue(availableSize.Height));
                        }
                        if (!item.MinHeight.IsAuto && item.MinHeight.Unit != Unit.Percent)
                        {
                            item.ActualHeight = Math.Max(item.ActualHeight, item.MinHeight.GetActualValue(availableSize.Height));
                        }
                    }
                    else// if (h.IsAuto)
                    {
                        item.ActualHeight = 0;
                    }
                    if (h.IsStar)
                    {
                        allHStar += h.Value;
                    }
                    allHeight += item.ActualHeight;
                    if (item.UIElements == null)
                    {
                        item.UIElements = new List<UIElement>();
                    }
                    item.UIElements.Clear();
                }
                //更新Star的尺寸
                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    var item = columnDefinitions[i];
                    var w = item.Width;
                    if (w.IsStar)
                    {
                        item.ActualWidth = Math.Max((availableSize.Width - allWidth) * (w.Value / allWStar), 0);
                        if (!item.MaxWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Min(item.ActualWidth, item.MaxWidth.GetActualValue(availableSize.Width));
                        }
                        if (!item.MinWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Max(item.ActualWidth, item.MinWidth.GetActualValue(availableSize.Width));
                        }
                    }
                }
                for (int i = 0; i < rowDefinitions.Count; i++)
                {
                    var item = rowDefinitions[i];
                    var h = item.Height;
                    if (h.IsStar)
                    {
                        item.ActualHeight = Math.Max((availableSize.Height - allHeight) * (h.Value / allHStar), 0);
                        if (!item.MaxHeight.IsAuto)
                        {
                            item.ActualHeight = Math.Min(item.ActualHeight, item.MaxHeight.GetActualValue(availableSize.Height));
                        }
                        if (!item.MinHeight.IsAuto && item.MinHeight.Unit != Unit.Percent)
                        {
                            item.ActualHeight = Math.Max(item.ActualHeight, item.MinHeight.GetActualValue(availableSize.Height));
                        }
                    }
                }

                //先计算子元素尺寸，再计算自动尺寸的
                for (int i = 0; i < Children.Count; i++)
                {
                    var item = Children[i];
                    var child = Children[i];
                    var c = GetColumnIndex(child);
                    var r = GetRowIndex(child);
                    var col = columnDefinitions[c];
                    var row = rowDefinitions[r];
                    col.UIElements.Add(item);
                    row.UIElements.Add(item);
                    var spanCol = Math.Max(ColumnSpan(child), 1);
                    var spanRow = Math.Max(RowSpan(child), 1);
                    var w = col.ActualWidth;
                    var h = row.ActualHeight;
                    for (int j = c + 1; j < Math.Min(c + spanCol, columnDefinitions.Count); j++)
                    {
                        w += columnDefinitions[j].ActualWidth;
                    }
                    for (int j = r + 1; j < Math.Min(r + spanRow, rowDefinitions.Count); j++)
                    {
                        h += rowDefinitions[j].ActualHeight;
                    }
                    item.Measure(new Size(
                        col.Width.IsAuto ? float.PositiveInfinity : w,
                        row.Height.IsAuto ? float.PositiveInfinity : h));
                }

                //计算自动尺寸的
                foreach (ColumnDefinition item in columnDefinitions)
                {
                    var w = item.Width;
                    if (!w.IsAbsolute && item.UIElements.Count > 0)
                    {
                        var q = item.UIElements.Where(a => ColumnSpan(a) == 1);
                        if (q.Count() > 0)
                        {
                            item.ActualWidth = q.Max(a => a.DesiredSize.Width);
                        }
                        else
                        {
                            item.ActualWidth = 0;
                        }
                    }
                    else if (!w.IsAbsolute)
                    {
                        item.ActualWidth = 0;
                    }
                    if (!item.MaxWidth.IsAuto)
                    {
                        item.ActualWidth = Math.Min(item.ActualWidth, item.MaxWidth.GetActualValue(availableSize.Width));
                    }
                    if (!item.MinWidth.IsAuto)
                    {
                        item.ActualWidth = Math.Max(item.ActualWidth, item.MinWidth.GetActualValue(availableSize.Width));
                    }
                }
                foreach (RowDefinition item in rowDefinitions)
                {
                    var h = item.Height;
                    if (!h.IsAbsolute && item.UIElements.Count > 0)
                    {
                        var q = item.UIElements.Where(a => RowSpan(a) == 1);
                        if (q.Count() > 0)
                        {
                            item.ActualHeight = q.Max(a => a.DesiredSize.Height);
                        }
                        else
                        {
                            item.ActualHeight = 0;
                        }
                    }
                    else if (!h.IsAbsolute)
                    {
                        item.ActualHeight = 0;
                    }
                    if (!item.MaxHeight.IsAuto)
                    {
                        item.ActualHeight = Math.Min(item.ActualHeight, item.MaxHeight.GetActualValue(availableSize.Height));
                    }
                    if (!item.MinHeight.IsAuto && item.MinHeight.Unit != Unit.Percent)
                    {
                        item.ActualHeight = Math.Max(item.ActualHeight, item.MinHeight.GetActualValue(availableSize.Height));
                    }
                }
                return new Size(columnDefinitions.Sum(a => a.ActualWidth), rowDefinitions.Sum(a => a.ActualHeight));
            }
        }
        protected override Size ArrangeOverride(in Size finalSize)
        {
            if (rowDefinitions.Count == 0 && columnDefinitions.Count == 0)
            {
                return base.ArrangeOverride(finalSize);
            }
            else
            {
                var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
                //var offsetX = 0f;
                var allWidth = 0f;//除了star的总宽度
                var allWStar = 0f;
                //计算权重总和和验证最大化最小化
                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    var item = columnDefinitions[i];
                    var w = item.Width;
                    if (!w.IsStar)
                    {
                        if (!item.MaxWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Min(item.ActualWidth, item.MaxWidth.GetActualValue(finalSize.Width));
                        }
                        if (!item.MinWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Max(item.ActualWidth, item.MinWidth.GetActualValue(finalSize.Width));
                        }
                        allWidth += item.ActualWidth;
                    }
                    else
                    {
                        allWStar += w.Value;
                    }
                }
                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    var item = columnDefinitions[i];
                    var w = item.Width;
                    if (w.IsStar)
                    {
                        item.ActualWidth = Math.Max(finalSize.Width - allWidth, 0) * (w.Value / allWStar);
                        if (!item.MaxWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Min(item.ActualWidth, item.MaxWidth.GetActualValue(finalSize.Width));
                        }
                        if (!item.MinWidth.IsAuto)
                        {
                            item.ActualWidth = Math.Max(item.ActualWidth, item.MinWidth.GetActualValue(finalSize.Width));
                        }
                    }
                }

                var offsetX = 0f;//更新offset
                for (int i = 0; i < columnDefinitions.Count; i++)
                {
                    var item = columnDefinitions[i];
                    item.offset = offsetX;
                    offsetX += item.ActualWidth;
                }

                var allHeight = 0f;
                var allHStar = 0f;
                for (int i = 0; i < rowDefinitions.Count; i++)
                {
                    var item = rowDefinitions[i];
                    var h = item.Height;
                    if (!h.IsStar)
                    {
                        if (!item.MaxHeight.IsAuto)
                        {
                            item.ActualHeight = Math.Min(item.ActualHeight, item.MaxHeight.GetActualValue(finalSize.Height));
                        }
                        if (!item.MinHeight.IsAuto && item.MinHeight.Unit != Unit.Percent)
                        {
                            item.ActualHeight = Math.Max(item.ActualHeight, item.MinHeight.GetActualValue(finalSize.Height));
                        }
                        allHeight += item.ActualHeight;
                    }
                    else
                    {
                        allHStar += h.Value;
                    }
                }
                for (int i = 0; i < rowDefinitions.Count; i++)
                {
                    var item = rowDefinitions[i];
                    var h = item.Height;
                    if (h.IsStar)
                    {
                        item.ActualHeight = Math.Max(finalSize.Height - allHeight, 0) * (h.Value / allHStar);
                        if (!item.MaxHeight.IsAuto)
                        {
                            item.ActualHeight = Math.Min(item.ActualHeight, item.MaxHeight.GetActualValue(finalSize.Height));
                        }
                        if (!item.MinHeight.IsAuto)
                        {
                            item.ActualHeight = Math.Max(item.ActualHeight, item.MinHeight.GetActualValue(finalSize.Height));
                        }
                    }
                }

                var offsetY = 0f;
                for (int i = 0; i < rowDefinitions.Count; i++)
                {
                    var item = rowDefinitions[i];
                    item.offset = offsetY;
                    offsetY += item.ActualHeight;
                }

                for (int i = 0; i < Children.Count; i++)
                {
                    var child = Children[i];
                    var c = GetColumnIndex(child);
                    var r = GetRowIndex(child);
                    var col = columnDefinitions[c];
                    var row = rowDefinitions[r];
                    var spanCol = Math.Max(ColumnSpan(child), 1);
                    var spanRow = Math.Max(RowSpan(child), 1);
                    var w = col.ActualWidth;
                    var h = row.ActualHeight;
                    for (int j = c + 1; j < Math.Min(c + spanCol, columnDefinitions.Count); j++)
                    {
                        w += columnDefinitions[j].ActualWidth;
                    }
                    for (int j = r + 1; j < Math.Min(r + spanRow, rowDefinitions.Count); j++)
                    {
                        h += rowDefinitions[j].ActualHeight;
                    }
                    child.Arrange(new Rect(col.offset, row.offset, w, h));
                }
                return finalSize;
            }
        }

        int GetRowIndex(UIElement element)
        {
            var row = RowIndex(element);
            if (row > rowDefinitions.Count - 1)
            {
                row = rowDefinitions.Count - 1;
            }
            else if (row < 0)
            {
                row = 0;
            }
            return row;
        }

        int GetColumnIndex(UIElement element)
        {
            var col = ColumnIndex(element);
            if (col > columnDefinitions.Count - 1)
            {
                col = columnDefinitions.Count - 1;
            }
            else if (col < 0)
            {
                col = 0;
            }
            return col;
        }
        /// <summary>
        /// 获取或设置元素行索引
        /// </summary>
        [Description("获取或设置元素行索引")]
        public static Attached<int> RowIndex
        {
            get
            {
                return RegisterAttached(0, typeof(Grid), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                 {
                     if (obj is UIElement element && element.Parent != null)
                     {
                         element.Parent.InvalidateMeasure();
                     }
                 });
            }
        }

        /// <summary>
        /// 获取或设置元素列索引
        /// </summary>
        [Description("获取或设置元素列索引")]
        public static Attached<int> ColumnIndex
        {
            get
            {
                return RegisterAttached(0, typeof(Grid), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
              {
                  if (obj is UIElement element && element.Parent != null)
                  {
                      element.Parent.InvalidateMeasure();
                  }
              });
            }
        }
        /// <summary>
        /// 获取或设置元素跨行
        /// </summary>
        [Description("获取或设置元素跨行")]
        public static Attached<int> RowSpan
        {
            get
            {
                return RegisterAttached(1, typeof(Grid), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                  {
                      if (obj is UIElement element && element.Parent != null)
                      {
                          element.Parent.InvalidateMeasure();
                      }
                  });
            }
        }
        /// <summary>
        /// 获取或设置元素跨列
        /// </summary>
        [Description("获取或设置元素跨列")]
        public static Attached<int> ColumnSpan
        {
            get
            {
                return RegisterAttached(1, typeof(Grid), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                  {
                      if (obj is UIElement element && element.Parent != null)
                      {
                          element.Parent.InvalidateMeasure();
                      }
                  });
            }
        }

        /// <summary>
        /// 网格线条填充
        /// </summary>
        [Description("边框线条填充")]
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill LineFill
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置线条类型
        /// </summary>
        [Description("获取或设置线条类型")]
        [UIPropertyMetadata(typeof(Stroke), "0", UIPropertyOptions.AffectsRender)]
        public Stroke LineStroke
        {
            get { return (Stroke)GetValue(); }
            set { SetValue(value); }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var stroke = LineStroke;
            var lineFill = LineFill;
            var ac = ActualSize;
            if (DesignMode && Site != null)
            {
                if (Site.GetType().GetProperty("ShowBorder") == null || (bool)Site.GetPropretyValue("ShowBorder"))
                {
                    using (SolidColorBrush brush = new SolidColorBrush(Color.Gray))
                    {
                        Stroke stroke1 = new Stroke(1, DashStyles.DashDotDot);
                        for (int i = 1; i < columnDefinitions.Count; ++i)
                        {
                            DrawGridLine(
                                dc,
                                columnDefinitions[i].offset, 0f,
                                columnDefinitions[i].offset, ac.Height, stroke1, brush);
                        }

                        for (int i = 1; i < rowDefinitions.Count; ++i)
                        {
                            DrawGridLine(
                                dc,
                                0f, rowDefinitions[i].offset,
                                ac.Width, rowDefinitions[i].offset, stroke1, brush);
                        }
                    }
                }
            }
            if (stroke.Width > 0 && lineFill != null)
            {
                using (var brush = lineFill.CreateBrush(new Rect(0, 0, ac.Width, ac.Height), Root.RenderScaling))
                {
                    for (int i = 1; i < columnDefinitions.Count; ++i)
                    {
                        DrawGridLine(
                            dc,
                            columnDefinitions[i].offset, 0f,
                            columnDefinitions[i].offset, ac.Height, stroke, brush);
                    }

                    for (int i = 1; i < rowDefinitions.Count; ++i)
                    {
                        DrawGridLine(
                            dc,
                            0f, rowDefinitions[i].offset,
                            ac.Width, rowDefinitions[i].offset, stroke, brush);
                    }
                }
            }
        }

        private static void DrawGridLine(DrawingContext drawingContext, float startX, float startY, float endX, float endY, in Stroke stroke, in Brush brush)
        {
            Point start = new Point(startX, startY);
            Point end = new Point(endX, endY);
            //drawingContext.DrawLine(s_oddDashPen, start, end);
            drawingContext.DrawLine(stroke, brush, start, end);
        }

    }

}


