using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CPF.Charts
{
    /// <summary>
    /// 提供折线图，曲线图，柱状图
    /// </summary>
    [Description("提供折线图，曲线图，柱状图")]
    public class Chart : Control
    {
        public Chart()
        {
            Data = new Collection<IChartData>();
            XAxis = new Collection<string>();
        }

        protected override void InitializeComponent()
        {
            Children.Add(new WrapPanel
            {
                Name = "seriesPanel",
                PresenterFor = this,
                Orientation = Orientation.Horizontal,
                MaxWidth = "100%",
                MarginTop = -30,
            });
            Children.Add(new StackPanel
            {
                Name = "tipPanel",
                PresenterFor = this,
                Orientation = Orientation.Vertical,
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false,
                Background = "#ffffff99",
                BorderFill = "#00000055",
                BorderStroke = new Stroke(1),
                Children =
                {
                    new TextBlock
                    {
                        Name="tipName",
                        PresenterFor=this,
                        MarginLeft=3,
                    },
                    new StackPanel
                    {
                        Name = "tipListPanel",
                        PresenterFor = this,
                        Orientation= Orientation.Vertical,
                        Margin="5",
                    }
                }
            });
        }

        Panel seriesPanel;
        Panel tipPanel;
        TextBlock tipName;
        Panel tipListPanel;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            seriesPanel = FindPresenterByName<Panel>("seriesPanel");
            tipPanel = FindPresenterByName<Panel>("tipPanel");
            tipName = FindPresenterByName<TextBlock>("tipName");
            tipListPanel = FindPresenterByName<Panel>("tipListPanel");
        }

        private void Data_CollectionChanged(object sender, CollectionChangedEventArgs<IChartData> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.SetOwnerChart(this);
                    e.NewItem.PropertyChanged += NewItem_PropertyChanged;
                    break;
                case CollectionChangedAction.Remove:
                    e.OldItem.PropertyChanged -= NewItem_PropertyChanged;
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.SetOwnerChart(this);
                    e.NewItem.PropertyChanged += NewItem_PropertyChanged;
                    e.OldItem.PropertyChanged -= NewItem_PropertyChanged;
                    break;
            }
            InvalidateData();
        }

        private void NewItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateData();
        }

        /// <summary>
        /// 数据
        /// </summary>
        public IList<IChartData> Data
        {
            get { return GetValue<IList<IChartData>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 水平缩放值 大于等于1
        /// </summary>
        [Description("水平缩放值 大于等于1"), PropertyMetadata(1f)]
        public float HorizontalScaling
        {
            get { return GetValue<float>(); }
            set
            {
                SetValue(value);
            }
        }
        [PropertyChanged(nameof(HorizontalScaling))]
        [PropertyChanged(nameof(ScrollValue))]
        [PropertyChanged(nameof(XAxis))]
        [PropertyChanged(nameof(MaxValue))]
        [PropertyChanged(nameof(MinValue))]
        void OnUpdateData(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            InvalidateData();
        }

        [PropertyChanged(nameof(Data))]
        void OnDataChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var nv = newValue as Collection<IChartData>;
            if (nv != null)
            {
                foreach (var item in nv)
                {
                    Data_CollectionChanged(nv, new CollectionChangedEventArgs<IChartData>(item, 0, null, CollectionChangedAction.Add));
                }
                nv.CollectionChanged += Data_CollectionChanged;
            }
            else
            {
                var data = new Collection<IChartData>();
                Data = data;
                var list = newValue as IEnumerable<IChartData>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        data.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 滚动值
        /// </summary>
        [Browsable(false)]
        public float ScrollValue
        {
            get { return GetValue<float>(); }
            set
            {
                SetValue(value);
            }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(ScrollValue))
            {
                var scrollValue = (float)value;
                if (scrollValue > 1)
                {
                    scrollValue = 1;
                }
                else if (scrollValue < 0)
                {
                    scrollValue = 0;
                }
                value = scrollValue;
            }
            else if (propertyName == nameof(HorizontalScaling))
            {
                var horizontalScaling = (float)value;
                if (horizontalScaling < 1)
                {
                    horizontalScaling = 1;
                }
                var xA = XAxis;
                if (xA.Count > 0)
                {
                    if (xA.Count / 2 < horizontalScaling)
                    {
                        horizontalScaling = Math.Max(1, xA.Count / 2);
                    }
                }
                value = horizontalScaling;
            }
            else if (propertyName == nameof(XAxis))
            {
                Collection<string> vs = value as Collection<string>;
                if (vs == null)
                {
                    var xa = XAxis as Collection<string>;
                    xa.Clear();
                    var list = value as IList<string>;
                    if (list != null)
                    {
                        xa.AddRange(list);
                    }
                }
                else
                {
                    vs.CollectionChanged += Vs_CollectionChanged;
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        private void Vs_CollectionChanged(object sender, CollectionChangedEventArgs<string> e)
        {
            InvalidateData();
        }
        bool refreshData;
        /// <summary>
        /// 刷新数据，下次更新的时候更新
        /// </summary>
        public void InvalidateData()
        {
            if (!refreshData)
            {
                refreshData = true;
                BeginInvoke(() =>
                {
                    UpdateData();
                });
            }
        }
        ///// <summary>
        ///// 图表区域内部间距
        ///// </summary>
        //[Description("图表区域内部间距"), UIPropertyMetadata(typeof(Thickness), "50, 30, 10, 30",UIPropertyOptions.AffectsRender)]
        //public Thickness Padding
        //{
        //    get { return GetValue<Thickness>(); }
        //    set
        //    {
        //        SetValue(value);
        //    }
        //}


        /// <summary>
        /// X轴颜色
        /// </summary>
        [Description("X轴颜色"), UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender)]
        public ViewFill XAxisFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Y轴颜色
        /// </summary>
        [Description("Y轴颜色"), UIPropertyMetadata(typeof(ViewFill), "Black", UIPropertyOptions.AffectsRender)]
        public ViewFill YAxisFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 图表区域填充
        /// </summary>
        [Description("图表区域填充"), UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill ChartFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 是否可以缩放滚动
        /// </summary>
        [Description("是否可以缩放滚动"), UIPropertyMetadata(false, UIPropertyOptions.AffectsRender)]
        public bool CanScroll
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// X轴文字
        /// </summary>
        [Description("X轴文字")]
        public IList<string> XAxis
        {
            get { return GetValue<IList<string>>(); }
            set { SetValue(value); }
        }


        double maxValue = 0;
        double minValue = 0;
        /// <summary>
        /// Y轴最大值
        /// </summary>
        [Description("Y轴最大值")]
        public double? MaxValue
        {
            get { return GetValue<double?>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// Y轴最小值
        /// </summary>
        [Description("Y轴最小值")]
        public double? MinValue
        {
            get { return GetValue<double?>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// Y轴刻度分割数量，大于等于1
        /// </summary>
        [Description("Y轴刻度分割数量，大于等于1"), UIPropertyMetadata(1u, UIPropertyOptions.AffectsRender)]
        public uint YAxisScaleCount
        {
            get { return GetValue<uint>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 鼠标移入选中的线条填充
        /// </summary>
        [Description("鼠标移入选中的线条填充"), UIPropertyMetadata(typeof(ViewFill), "0, 0, 0, 50", UIPropertyOptions.AffectsRender)]
        public ViewFill HoverSelectLineFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 鼠标移入选中的坐标轴提示背景填充
        /// </summary>
        [Description("鼠标移入选中的坐标轴提示背景填充"), UIPropertyMetadata(typeof(ViewFill), "0, 0, 0, 100", UIPropertyOptions.AffectsRender)]
        public ViewFill HoverSelectTipBackFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 鼠标移入选中的坐标轴提示文字填充
        /// </summary>
        [Description("鼠标移入选中的坐标轴提示文字填充"), UIPropertyMetadata(typeof(ViewFill), "White", UIPropertyOptions.AffectsRender)]
        public ViewFill HoverSelectTipFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 网格显示模式
        /// </summary>
        [Description("网格显示模式"), UIPropertyMetadata(GridShowMode.None, UIPropertyOptions.AffectsRender)]
        public GridShowMode GridShowMode
        {
            get { return GetValue<GridShowMode>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 网格填充
        /// </summary>
        [Description("网格填充"), UIPropertyMetadata(typeof(ViewFill), "Silver", UIPropertyOptions.AffectsRender)]
        public ViewFill GridFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 显示滚动缩放值的线条填充
        /// </summary>
        [Description("显示滚动缩放值的线条填充"), UIPropertyMetadata(typeof(ViewFill), "Red", UIPropertyOptions.AffectsRender)]
        public ViewFill ScrollLineFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 鼠标移入图表的时候显示信息
        /// </summary>
        [Description("鼠标移入图表的时候显示信息"), PropertyMetadata(true)]
        public bool MouseHoverShowTip
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 是否有数据
        /// </summary>
        [Description("是否有数据")]
        public bool HasData
        {
            get { return GetValue<bool>(); }
            private set { SetValue(value); }
        }
        /// <summary>
        /// 图例模板
        /// </summary>
        public UIElementTemplate<SerieItem> SerieTemplate
        {
            get { return GetValue<UIElementTemplate<SerieItem>>(); }
            set { SetValue(value); }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(SerieTemplate), new PropertyMetadataAttribute((UIElementTemplate<SerieItem>)typeof(SerieItem)));
            overridePropertys.Override(nameof(Padding), new UIPropertyMetadataAttribute(new Thickness(30), UIPropertyOptions.AffectsMeasure));
        }

        Point mouseHover = new Point();

        //Direction direction = Direction.LeftToRight;

        /// <summary>
        /// 添加数据之后要调用该方法来更新界面
        /// </summary>
        public void UpdateData()
        {
            var data = Data;
            HasData = data.Count > 0;
            hoverIndex = -1;
            var xAxis = XAxis;
            if (xAxis == null || xAxis.Count == 0)
            {
                maxValue = 0;
                minValue = 0;
                return;
            }
            int startIndex, length;
            GetRange(HorizontalScaling, ScrollValue, xAxis.Count, out startIndex, out length);
            double max = 0;
            double min = 0;
            foreach (IChartData item in data)
            {
                max = Math.Max(item.GetMaxValue(startIndex, length), max);
                min = Math.Min(item.GetMinValue(startIndex, length), min);
            }
            if (MaxValue.HasValue)
            {
                max = MaxValue.Value;
            }
            if (MinValue.HasValue)
            {
                min = MinValue.Value;
            }
            maxValue = max;
            minValue = min;
            refreshData = false;
            Invalidate();
            var template = SerieTemplate;
            if (seriesPanel && template != null)
            {
                seriesPanel.Children.Clear();
                foreach (var item in data)
                {
                    var c = template.CreateElement();
                    c.DataContext = item;
                    c.Name = item.Name;
                    seriesPanel.Children.Add(c);
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var g = dc;
            var fillBrush = ChartFill;
            var padding = Padding;
            var ownerSize = ActualSize;
            var bounds = new Rect(new Point(), ownerSize);
            if (fillBrush != null)
            {
                using (var fb = fillBrush.CreateBrush(bounds, Root.RenderScaling))
                {
                    g.FillRectangle(fb, new Rect(padding.Left, padding.Top, ownerSize.Width - padding.Horizontal, ownerSize.Height - padding.Vertical));
                }
            }
            var xAxis = XAxis;
            var _size = new Size(Math.Max(0, ownerSize.Width - padding.Horizontal), Math.Max(0, ownerSize.Height - padding.Vertical));
            if (xAxis == null || xAxis.Count == 0)
            {
                return;
            }
            var data = Data;
            int startIndex, length;
            var horizontalScaling = HorizontalScaling;
            var scrollValue = ScrollValue;
            var yAxisColor = YAxisFill;
            var gridColor = GridFill;
            var gridShowMode = GridShowMode;
            var scrollLineColor = ScrollLineFill;
            var xAxisColor = XAxisFill;
            var mouseHoverShowTip = MouseHoverShowTip;
            var hoverSelectTipColor = HoverSelectTipFill;
            var hoverSelectTipBackColor = HoverSelectTipBackFill;
            var hoverSelectLineColor = HoverSelectLineFill;
            var yAxisScaleCount = YAxisScaleCount;

            GetRange(horizontalScaling, scrollValue, xAxis.Count, out startIndex, out length);

            Font Font = new Font(FontFamily, FontSize, FontStyle);
            var mustOffset = Data.Any(a => a is ChartBarData);
            using (Brush foreColorBrush = Foreground == null ? new SolidColorBrush(Color.Transparent) : Foreground.CreateBrush(bounds, Root.RenderScaling))
            {
                using (var gpen = gridColor.CreateBrush(bounds, Root.RenderScaling))
                using (var yApen = yAxisColor.CreateBrush(bounds, Root.RenderScaling))
                {
                    var size = new Size();
                    var max = maxValue;
                    if (minValue == maxValue)
                    {
                        max = minValue + 1;
                    }
                    g.DrawLine(new Stroke(1), yApen, new Point(padding.Left, padding.Top), new Point(padding.Left, ownerSize.Height - padding.Bottom));
                    var v = (max - minValue) / yAxisScaleCount;
                    var offset1 = v / (max - minValue) * (ownerSize.Height - padding.Vertical);
                    for (int i = 0; i < yAxisScaleCount + 1; i++)
                    {
                        g.DrawLine(new Stroke(1), yApen, new Point(padding.Left - 3, (int)(padding.Top + offset1 * i)), new Point(padding.Left, (int)(padding.Top + offset1 * i)));
                        if (gridShowMode == GridShowMode.Horizontal || gridShowMode == GridShowMode.All)
                        {
                            g.DrawLine(new Stroke(1), gpen, new Point(ownerSize.Width - padding.Right, (int)(padding.Top + offset1 * i)), new Point(padding.Left, (int)(padding.Top + offset1 * i)));
                        }
                        if (i != 0 && i != yAxisScaleCount)
                        {
                            var vv = max - v * i;
                            var s = g.DrawingFactory.MeasureString(vv.ToString("0.##"), Font);
                            g.DrawString(new Point(padding.Left - s.Width - 3, (float)(padding.Top + offset1 * i - s.Height / 2)), foreColorBrush, vv.ToString("0.##"), Font);
                        }
                    }

                    size = g.DrawingFactory.MeasureString(max.ToString(), Font);
                    g.DrawString(new Point(padding.Left - size.Width - 3, padding.Top - size.Height / 2), foreColorBrush, max.ToString(), Font);

                    size = g.DrawingFactory.MeasureString(minValue.ToString(), Font);
                    g.DrawString(new Point(padding.Left - size.Width - 3, ownerSize.Height - padding.Bottom - size.Height / 2), foreColorBrush, minValue.ToString(), Font);


                    var offsetWidth = 1.0 * _size.Width / (length - (mustOffset ? 0 : 1));
                    var paddingLeft = mustOffset ? (padding.Left + (int)(offsetWidth / 2)) : (padding.Left);
                    var width = mustOffset ? (ownerSize.Width - padding.Horizontal - offsetWidth) : (ownerSize.Width - padding.Horizontal);

                    foreach (IChartData item in data)
                    {
                        if (startIndex > item.DataCount - 1)
                        {
                            continue;
                        }
                        var len = length;
                        //if (startIndex + length > item.DataCount)
                        //{
                        //    len = item.DataCount - startIndex;
                        //}

                        item.PaintBackground(dc, startIndex, length,
                            new Rect(padding.Left + (mustOffset ? (int)(offsetWidth / 2) : 0),
                            padding.Top,
                            Math.Max(0, (int)((len - 1) * offsetWidth)),
                            _size.Height), maxValue, minValue, bounds, Root.RenderScaling);
                    }

                    using (var pen = xAxisColor.CreateBrush(bounds, Root.RenderScaling))
                    {
                        g.DrawLine(new Stroke(CanScroll ? 5 : 1), pen, new Point(padding.Left, ownerSize.Height - padding.Bottom), new Point(ownerSize.Width - padding.Right, ownerSize.Height - padding.Bottom));
                    }
                    if (CanScroll)
                    {
                        using (var p = scrollLineColor.CreateBrush(bounds, Root.RenderScaling))
                        {
                            var offset = (ownerSize.Width - padding.Horizontal - (ownerSize.Width - padding.Horizontal) / horizontalScaling) * scrollValue;
                            g.DrawLine(new Stroke(3), p, new Point(padding.Left + (int)offset, ownerSize.Height - padding.Bottom), new Point(padding.Left + (int)(offset + (ownerSize.Width - padding.Horizontal) / horizontalScaling), ownerSize.Height - padding.Bottom));
                        }
                    }

                    //GetRange(horizontalScaling, scrollValue, xAxis.Length, out int startIndex, out int length);
                    if (length > 1)
                    {
                        //using (StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            size = g.DrawingFactory.MeasureString(xAxis[startIndex], Font);
                            //g.DrawString(xAxis[startIndex], Font, foreColorBrush, new RectangleF(paddingLeft - (size.Width + size.Width / 5) / 2,ownerSize.Height - padding.Bottom + Font.Height / 4, size.Width + size.Width / 5, size.Height + size.Height / 5), stringFormat);
                            var w1 = size.Width;
                            size = g.DrawingFactory.MeasureString(xAxis[startIndex + length - 1], Font);
                            //g.DrawString(xAxis[startIndex + length - 1], Font, foreColorBrush, new RectangleF(paddingLeft + (int)width - (size.Width + size.Width / 5) / 2,ownerSize.Height - padding.Bottom + Font.Height / 4, size.Width + size.Width / 5, size.Height + size.Height / 5), stringFormat);
                            if (gridShowMode == GridShowMode.Vertical || gridShowMode == GridShowMode.All)
                            {
                                g.DrawLine(new Stroke(1), gpen, new Point(ownerSize.Width - padding.Right, ownerSize.Height - padding.Bottom), new Point(ownerSize.Width - padding.Right, padding.Top));
                            }

                            if (length > 1)
                            {
                                var count = (width + size.Width) / ((w1 + size.Width) / 2 + Font.DefaultLineHeight);
                                var offset = 1.0 * (width) / (length - 1);
                                var of = length / count;
                                if (of < 1)
                                {
                                    of = 1;
                                }

                                double index = startIndex;
                                var w = 0f;
                                double preOffset = paddingLeft - w1 / 2;
                                while (index < startIndex + length && (((int)index - startIndex) * offset + paddingLeft + w <= ownerSize.Width))
                                {
                                    var c = xAxis[(int)index];
                                    var cs = g.DrawingFactory.MeasureString(c, Font);

                                    var x = (int)(((int)index - startIndex) * offset + paddingLeft);

                                    if (x > preOffset + w)
                                    {
                                        if (mustOffset)
                                        {
                                            preOffset = x + (int)offsetWidth / 2;
                                            g.DrawLine(new Stroke(1), yApen, new Point((float)preOffset, ownerSize.Height - padding.Bottom), new Point((float)preOffset, ownerSize.Height - padding.Bottom + 4));
                                        }
                                        else
                                        {
                                            preOffset = x;
                                            g.DrawLine(new Stroke(1), yApen, new Point(x, ownerSize.Height - padding.Bottom), new Point(x, ownerSize.Height - padding.Bottom + 4));
                                        }
                                        if (gridShowMode == GridShowMode.Vertical || gridShowMode == GridShowMode.All)
                                        {
                                            g.DrawLine(new Stroke(1), gpen, new Point(x, ownerSize.Height - padding.Bottom), new Point(x, padding.Top));
                                        }
                                        g.DrawString(new Point(x - cs.Width / 2, ownerSize.Height - padding.Bottom + Font.DefaultLineHeight / 4), foreColorBrush, c, Font, TextAlignment.Center, cs.Width + cs.Width / 5);
                                    }
                                    w = cs.Width;
                                    index += of;
                                }


                            }
                        }
                    }

                    foreach (IChartData item in data)
                    {
                        if (startIndex > item.DataCount - 1)
                        {
                            continue;
                        }
                        var len = length;
                        //if (startIndex + length > item.DataCount)
                        //{
                        //    len = item.DataCount - startIndex;
                        //}
                        //var w = 1.0 * _size.Width / (length - 1);

                        item.Paint(dc, startIndex, length, new Rect(padding.Left + (mustOffset ? (int)(offsetWidth / 2) : 0),
                            padding.Top,
                            Math.Max(0, (int)((len - 1) * offsetWidth)),
                            _size.Height), maxValue, minValue, bounds, Root.RenderScaling);
                    }

                    if (mouseHover.X > padding.Left && mouseHover.Y > padding.Top && mouseHover.Y < ownerSize.Height - padding.Bottom && mouseHover.X < ownerSize.Width - padding.Right && mouseHoverShowTip)
                    {
                        var yValue = (maxValue - (maxValue - minValue) / (ownerSize.Height - padding.Vertical) * (mouseHover.Y - padding.Top)).ToString("0.00");

                        size = g.DrawingFactory.MeasureString(yValue, Font);
                        using (Brush sb = hoverSelectTipBackColor.CreateBrush(bounds, Root.RenderScaling), b = hoverSelectTipColor.CreateBrush(bounds, Root.RenderScaling))
                        {
                            g.FillRectangle(sb, new Rect(padding.Left - size.Width, mouseHover.Y - size.Height / 2, size.Width, size.Height));
                            g.DrawString(new Point(padding.Left - size.Width, mouseHover.Y - size.Height / 2), b, yValue, Font);
                            using (var pen = hoverSelectLineColor.CreateBrush(bounds, Root.RenderScaling))
                            {
                                Stroke stroke = new Stroke(1);
                                if (xAxis != null && xAxis.Count > 0)
                                {
                                    //GetRange(horizontalScaling, scrollValue, xAxis.Length, out int startIndex, out int length);
                                    if (length > 1)
                                    {
                                        var offset = 1.0 * width / (length - 1);
                                        var index = (int)((mouseHover.X - paddingLeft + offset / 2) / offset);

                                        if (mustOffset)
                                        {
                                            stroke.Width = (int)offsetWidth;
                                            g.DrawLine(stroke, pen, new Point((float)(index * offset + paddingLeft), padding.Top), new Point((float)(index * offset + paddingLeft), ownerSize.Height - padding.Bottom));
                                        }
                                        else
                                        {
                                            g.DrawLine(stroke, pen, new Point((float)(index * offset + paddingLeft), padding.Top), new Point((float)(index * offset + paddingLeft), ownerSize.Height - padding.Bottom));
                                        }

                                        if (xAxis.Count > index + startIndex)
                                        {
                                            size = g.DrawingFactory.MeasureString(xAxis[index + startIndex], Font);
                                            g.FillRectangle(sb, new Rect((int)(index * offset + paddingLeft) - size.Width / 2, ownerSize.Height - padding.Bottom + 4, size.Width, size.Height));
                                            g.DrawString(new Point((float)(index * offset + paddingLeft) - size.Width / 2, ownerSize.Height - padding.Bottom + 4), b, xAxis[index + startIndex], Font);
                                            //var str = xAxis[index + startIndex];
                                            //size = g.DrawingFactory.MeasureString(str, Font);
                                            //var rect = new Rect(mouseHover.X + 15, mouseHover.Y + 15, size.Width, size.Height);
                                            //List<KeyValuePair<Point, ChartData>> textPoints = new List<KeyValuePair<Point, ChartData>>();
                                            //foreach (ChartData item in data)
                                            //{
                                            //    if (index + startIndex < item.DataCount)
                                            //    {
                                            //        var s = item.GetTipString(index + startIndex);
                                            //        size = g.DrawingFactory.MeasureString(s, Font);
                                            //        textPoints.Add(new KeyValuePair<Point, ChartData>(new Point(20, (int)rect.Height), item));
                                            //        rect.Size = new Size(Math.Max(rect.Width, 20 + size.Width), rect.Height + size.Height);
                                            //    }
                                            //}
                                            //if (rect.X + rect.Width > ownerSize.Width)
                                            //{
                                            //    rect.X = mouseHover.X - rect.Width - 10;
                                            //}
                                            //if (rect.Y + rect.Height > ownerSize.Height)
                                            //{
                                            //    rect.Y = mouseHover.Y - rect.Height - 10;
                                            //}
                                            //g.FillRectangle(sb, rect);
                                            //g.DrawString(new Point(rect.X, rect.Y), b, str, Font);
                                            //foreach (var item in textPoints)
                                            //{
                                            //    using (Brush brush = item.Value.Fill.CreateBrush(bounds, Root.RenderScaling))
                                            //    {
                                            //        g.FillEllipse(brush, new Point(rect.X + 5 + 4, rect.Y + item.Key.Y + (Font.DefaultLineHeight - 8) / 2 + 4), 4, 4);
                                            //    }
                                            //    g.DrawString(new Point(item.Key.X + rect.X, item.Key.Y + rect.Y), b, item.Value.GetTipString(index + startIndex), Font);
                                            //}
                                        }
                                    }
                                }
                                stroke.Width = 1;
                                stroke.DashStyle = DashStyles.Dash;
                                stroke.DashPattern = new float[] { 4, 4 };
                                g.DrawLine(stroke, pen, new Point(padding.Left, mouseHover.Y), new Point(ownerSize.Width - padding.Right, mouseHover.Y));
                            }
                        }
                    }

                    //Size textSize = new Size();
                    //List<KeyValuePair<float, ChartData>> texts = new List<KeyValuePair<float, ChartData>>();
                    //foreach (ChartData item in data)
                    //{
                    //    var s = item.Name;
                    //    size = g.DrawingFactory.MeasureString(s, Font);
                    //    texts.Add(new KeyValuePair<float, ChartData>(20 + size.Width, item));
                    //    textSize = new Size(20 + textSize.Width + size.Width, Math.Max(textSize.Height, size.Height));
                    //}

                    //var left = (ownerSize.Width - textSize.Width) / 2;
                    //foreach (var item in texts)
                    //{
                    //    using (Brush brush = item.Value.Fill.CreateBrush(bounds, Root.RenderScaling))
                    //    {
                    //        g.FillEllipse(brush, new Point(left + 4, (textSize.Height - 8) / 2 + 8), 4, 4);
                    //    }
                    //    g.DrawString(new Point(left + 10, 5), foreColorBrush, item.Value.Name, Font);
                    //    left += item.Key;
                    //}
                }
            }
        }
        /// <summary>
        /// 通过坐标获取该位置的数据索引
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int HitTestDataIndex(Point point, out int startIndex, out int length)
        {
            var ownerSize = ActualSize;
            var padding = Padding;
            var _size = new Size(ownerSize.Width - padding.Horizontal, ownerSize.Height - padding.Vertical);
            var mustOffset = Data.Any(a => a is ChartBarData);
            GetRange(HorizontalScaling, ScrollValue, XAxis.Count, out startIndex, out length);
            if (length == 1)
            {
                return startIndex;
            }
            var offsetWidth = 1.0 * _size.Width / (length - (mustOffset ? 0 : 1));
            var paddingLeft = mustOffset ? (padding.Left + (int)(offsetWidth / 2)) : (padding.Left);
            var width = mustOffset ? (ownerSize.Width - padding.Horizontal - offsetWidth) : (ownerSize.Width - padding.Horizontal);
            var offset = 1.0 * width / (length - 1);
            var index = (int)((mouseHover.X - paddingLeft + offset / 2) / offset);
            return index + startIndex;
        }

        public static void GetRange(float horizontalScaling, float scrollValue, int count, out int startIndex, out int length)
        {
            var range = 1 / horizontalScaling;//索引范围宽度
            var scroll = 1 - range;//可以滚动的范围
            var scrollStart = scroll * scrollValue;
            startIndex = (int)Math.Round(scrollStart * count, MidpointRounding.AwayFromZero);
            length = (int)Math.Round(range * count, MidpointRounding.AwayFromZero);
            if (startIndex > count - 1)
            {
                startIndex = count - 1;
                length = 1;
            }
            if (startIndex + length > count)
            {
                length = count - startIndex;
            }
        }

        protected override void OnMouseWheel(Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!CanScroll)
            {
                return;
            }
            if (e.Delta.Y > 0)
            {
                HorizontalScaling += 1;
            }
            else
            {
                HorizontalScaling -= 1;
            }
        }

        Point oldPoint;
        protected override void OnMouseDown(Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            oldPoint = e.Location;
            this.CaptureMouse();
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            this.ReleaseMouseCapture();
        }

        int hoverIndex = -1;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            mouseHover = e.Location;
            base.OnMouseMove(e);
            var ownerSize = ActualSize;
            if (IsMouseCaptured && HorizontalScaling != 1 && CanScroll)
            {
                var w = (ownerSize.Width - Padding.Horizontal) * HorizontalScaling - (ownerSize.Width - Padding.Horizontal);
                var offset = e.Location.X - oldPoint.X;
                ScrollValue -= (offset / w);
                oldPoint = e.Location;
            }
            var padding = Padding;
            var mouseHoverShowTip = MouseHoverShowTip;
            var xAxis = XAxis;
            var index = HitTestDataIndex(e.Location, out var startIndex, out var length);
            if (mouseHover.X > padding.Left && mouseHover.Y > padding.Top && mouseHover.Y < ownerSize.Height - padding.Bottom && mouseHover.X < ownerSize.Width - padding.Right && mouseHoverShowTip && index < xAxis.Count)
            {
                if (tipPanel)
                {
                    var data = Data;
                    tipPanel.Visibility = Visibility.Visible;
                    var str = xAxis[index];
                    if (tipName)
                    {
                        tipName.Text = str;
                    }
                    Font Font = new Font(FontFamily, FontSize, FontStyle);
                    var size = DrawingFactory.Default.MeasureString(xAxis[index], Font);
                    var rect = new Rect(mouseHover.X + 15, mouseHover.Y + 15, size.Width, size.Height);

                    //foreach (IChartData item in data)
                    //{
                    //    if (index < item.DataCount)
                    //    {
                    //        var s = item.Name + ":" + item.GetValue(index);
                    //        size = DrawingFactory.Default.MeasureString(s, Font);

                    //        rect.Size = new Size(Math.Max(rect.Width, 20 + size.Width), rect.Height + size.Height);
                    //    }
                    //}
                    if (hoverIndex != index)
                    {
                        tipListPanel.Children.Clear();
                        var template = SerieTemplate;
                        if (template != null)
                        {
                            foreach (var item in data)
                            {
                                if (index < item.DataCount)
                                {
                                    //var s = item.GetTipString(index);
                                    var c = template.CreateElement();
                                    c.DataContext = item;
                                    c.Name = item.Name;
                                    c.Value = item.GetValue(index);
                                    tipListPanel.Children.Add(c);
                                }
                            }
                        }
                    }
                    //tipPanel.Measure(ownerSize);
                    rect.Size = tipPanel.ActualSize;
                    if (rect.X + rect.Width > ownerSize.Width)
                    {
                        rect.X = mouseHover.X - rect.Width - 10;
                    }
                    if (rect.Y + rect.Height > ownerSize.Height)
                    {
                        rect.Y = mouseHover.Y - rect.Height - 10;
                    }

                    tipPanel.MarginLeft = rect.X - padding.Left;
                    tipPanel.MarginTop = rect.Y - padding.Top;

                }
            }
            else
            {
                if (tipPanel)
                {
                    tipPanel.Visibility = Visibility.Collapsed;
                }
            }
            hoverIndex = index;
            Invalidate();
        }

        protected override void OnMouseLeave(Input.MouseEventArgs e)
        {
            mouseHover = new Point();
            base.OnMouseLeave(e);
            Invalidate();
            if (tipPanel)
            {
                tipPanel.Visibility = Visibility.Collapsed;
            }
        }
    }

    public enum GridShowMode
    {
        None,
        All,
        Horizontal,
        Vertical
    }
}
