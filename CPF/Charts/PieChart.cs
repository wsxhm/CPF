using CPF.Controls;
using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using CPF.Input;

namespace CPF.Charts
{
    /// <summary>
    /// 饼图
    /// </summary>
    [Description("饼图")]
    public class PieChart : Control
    {
        public PieChart()
        {
            Data = new Collection<PieChartData>();
        }

        [PropertyChanged(nameof(Data))]
        void OnDataChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var nv = newValue as Collection<PieChartData>;
            if (nv != null)
            {
                foreach (var item in nv)
                {
                    Nv_CollectionChanged(nv, new CollectionChangedEventArgs<PieChartData>(item, 0, null, CollectionChangedAction.Add));
                }
                nv.CollectionChanged += Nv_CollectionChanged;
            }
            else
            {
                var data = new Collection<PieChartData>();
                Data = data;
                var list = newValue as IEnumerable<PieChartData>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        data.Add(item);
                    }
                }
            }
        }

        private void Nv_CollectionChanged(object sender, CollectionChangedEventArgs<PieChartData> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.PropertyChanged += NewItem_PropertyChanged;
                    break;
                case CollectionChangedAction.Remove:
                    e.OldItem.PropertyChanged -= NewItem_PropertyChanged;
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.PropertyChanged += NewItem_PropertyChanged;
                    e.OldItem.PropertyChanged -= NewItem_PropertyChanged;
                    break;
            }
            InvalidateData();
        }

        private void NewItem_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
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
        public void UpdateData()
        {
            Invalidate();
            var data = Data;
            HasData = data.Count > 0;
            refreshData = false;
            var template = SerieTemplate;
            if (seriesPanel && template != null)
            {
                seriesPanel.Children.Clear();
                foreach (var item in Data)
                {
                    var c = template.CreateElement();
                    c.DataContext = item;
                    c.Name = item.Name;
                    //c.Value = item.Value;
                    seriesPanel.Children.Add(c);
                }
            }


            foreach (var item in paths)
            {
                item.Item1.Dispose();
            }
            paths.Clear();

            foreach (var item in Children.Where(a => a.Tag == this && a is CPF.Shapes.Path).ToArray())
            {
                Children.Remove(item);
                item.Dispose();
            }

            var padding = Padding;
            //绘制值标签
            double StartAngle = 0;
            var ownerSize = ActualSize;
            var bounds = new Rect(new Point(), ownerSize);
            var sum = data.Sum(a => a.Value);
            var ringWidth = RingWidth;
            var r = Math.Min(ownerSize.Width - padding.Horizontal, ownerSize.Height - padding.Vertical) / 2;
            if (r < 1)
            {
                return;
            }
            for (int i = 0; i < data.Count; i++)
            {
                var angle = data[i].Value * 360 / sum;
                if (angle == 360)
                {
                    angle = 359.999;
                }
                var centerX = padding.Left + (ownerSize.Width - padding.Horizontal) / 2;
                var centerY = (padding.Top + (ownerSize.Height - padding.Vertical) / 2);
                var path = new PathGeometry();
                if (ringWidth.IsAuto)
                {
                    path.BeginFigure(centerX, centerY);
                }
                else
                {
                    var ww = Math.Max(1, Math.Min(ringWidth.GetActualValue(r), r));
                    path.BeginFigure((float)Math.Sin(StartAngle * Math.PI / 180) * (r - ww) + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * (r - ww));
                }
                path.LineTo((float)Math.Sin(StartAngle * Math.PI / 180) * r + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * r);
                StartAngle += angle;
                path.ArcTo(new Point((float)Math.Sin(StartAngle * Math.PI / 180) * r + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * r), new Size(r, r), (float)angle, true, angle > 180);
                if (!ringWidth.IsAuto)
                {
                    var ww = Math.Max(1, Math.Min(ringWidth.GetActualValue(r), r));
                    path.LineTo((float)Math.Sin(StartAngle * Math.PI / 180) * (r - ww) + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * (r - ww));
                    path.ArcTo(new Point((float)Math.Sin((StartAngle - angle) * Math.PI / 180) * (r - ww) + centerX, centerY - (float)Math.Cos((StartAngle - angle) * Math.PI / 180) * (r - ww)), new Size(r - ww, r - ww), (float)angle, false, angle > 180);
                }
                path.EndFigure(true);
                Children.Add(new CPF.Shapes.Path
                {
                    PresenterFor = this,
                    Name = data[i].Name,
                    Fill = data[i].Fill,
                    ZIndex = -1,
                    Data = path,
                    MarginLeft = -padding.Left,
                    MarginTop = -padding.Top,
                    Tag = this,
                    StrokeFill = null,
                });
                paths.Add((path, data[i]));
            }

        }

        public IList<PieChartData> Data
        {
            get { return GetValue<IList<PieChartData>>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 指示的线条填充
        /// </summary>
        [UIPropertyMetadata(typeof(ViewFill), "#aaa", UIPropertyOptions.AffectsRender)]
        public ViewFill TipLineFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 圆环宽度
        /// </summary>
        [Description("圆环宽度"), UIPropertyMetadata(typeof(FloatField), "auto", UIPropertyOptions.AffectsRender)]
        public FloatField RingWidth
        {
            get { return GetValue<FloatField>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 图例模板
        /// </summary>
        public UIElementTemplate<SerieItem> SerieTemplate
        {
            get { return GetValue<UIElementTemplate<SerieItem>>(); }
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

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(SerieTemplate), new PropertyMetadataAttribute((UIElementTemplate<SerieItem>)typeof(SerieItem)));
            overridePropertys.Override(nameof(Padding), new UIPropertyMetadataAttribute(new Thickness(30, 30, 30, 20), UIPropertyOptions.AffectsMeasure));
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

        //protected override void OnLayoutUpdated()
        //{
        //    base.OnLayoutUpdated();
        //    InvalidateData();
        //}

        [PropertyChanged(nameof(ActualSize))]
        void OnAcSizeChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            InvalidateData();
        }

        List<(PathGeometry, PieChartData)> paths = new List<(PathGeometry, PieChartData)>();

        protected override void OnRender(DrawingContext g)
        {
            base.OnRender(g);
            //绘制值标签
            var ownerSize = ActualSize;
            var bounds = new Rect(new Point(), ownerSize);
            double StartAngle = 0;
            var data = Data;
            var padding = Padding;
            var sum = data.Sum(a => a.Value);
            var ringWidth = RingWidth;
            var r = Math.Min(ownerSize.Width - padding.Horizontal, ownerSize.Height - padding.Vertical) / 2;
            for (int i = 0; i < data.Count; i++)
            {
                //using (var br = data[i].Fill.CreateBrush(bounds, Root.RenderScaling))
                {
                    var angle = data[i].Value * 360 / sum;
                    var centerX = padding.Left + (ownerSize.Width - padding.Horizontal) / 2;
                    var centerY = (padding.Top + (ownerSize.Height - padding.Vertical) / 2);
                    //using (PathGeometry path = new PathGeometry())
                    //{
                    //    if (ringWidth.IsAuto)
                    //    {
                    //        path.BeginFigure(centerX, centerY);
                    //    }
                    //    else
                    //    {
                    //        var ww = Math.Max(1, Math.Min(ringWidth.GetActualValue(r), r));
                    //        path.BeginFigure((float)Math.Sin(StartAngle * Math.PI / 180) * (r - ww) + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * (r - ww));
                    //    }
                    //    path.LineTo((float)Math.Sin(StartAngle * Math.PI / 180) * r + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * r);
                    StartAngle += angle;
                    //    path.ArcTo(new Point((float)Math.Sin(StartAngle * Math.PI / 180) * r + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * r), new Size(r, r), (float)angle, true, angle > 180);
                    //    if (!ringWidth.IsAuto)
                    //    {
                    //        var ww = Math.Max(1, Math.Min(ringWidth.GetActualValue(r), r));
                    //        path.LineTo((float)Math.Sin(StartAngle * Math.PI / 180) * (r - ww) + centerX, centerY - (float)Math.Cos(StartAngle * Math.PI / 180) * (r - ww));
                    //        path.ArcTo(new Point((float)Math.Sin((StartAngle - angle) * Math.PI / 180) * (r - ww) + centerX, centerY - (float)Math.Cos((StartAngle - angle) * Math.PI / 180) * (r - ww)), new Size(r - ww, r - ww), (float)angle, false, angle > 180);
                    //    }
                    //    path.EndFigure(true);
                    //    g.FillPath(br, path);
                    //}
                    var an = StartAngle - angle / 2;
                    var tipFill = TipLineFill;
                    var rr = 12;
                    var l = (float)Math.Sin(an * Math.PI / 180) * (r + rr) + centerX;
                    if (tipFill != null)
                    {
                        using (var tip = tipFill.CreateBrush(bounds, Root.RenderScaling))
                        {
                            var point = new Point((float)Math.Sin(an * Math.PI / 180) * (r + rr) + centerX, centerY - (float)Math.Cos(an * Math.PI / 180) * (r + rr));
                            g.DrawLine(new Stroke(1), tip, new Point((float)Math.Sin(an * Math.PI / 180) * r + centerX, centerY - (float)Math.Cos(an * Math.PI / 180) * r), point);
                            g.DrawLine(new Stroke(1), tip, point, new Point(point.X + (l > centerX ? 8 : -8), point.Y));
                        }
                    }
                    var fore = Foreground;
                    if (fore != null)
                    {
                        using (Font font = new Font(FontFamily, FontSize, FontStyle))
                        {
                            using (var f = fore.CreateBrush(bounds, Root.RenderScaling))
                            {
                                var str = data[i].Name;
                                var s = DrawingFactory.Default.MeasureString(str, font);
                                if (l > centerX)
                                {
                                    g.DrawString(new Point(l + 8, centerY - (float)Math.Cos(an * Math.PI / 180) * (r + rr) - s.Height / 2), f, str, font);
                                }
                                else
                                {
                                    g.DrawString(new Point(l - s.Width - 8, centerY - (float)Math.Cos(an * Math.PI / 180) * (r + rr) - s.Height / 2), f, str, font);
                                }
                            }
                        }
                    }
                }
            }


            //foreach (var item in paths)
            //{
            //    if (item.Item2.Fill != null)
            //    {
            //        using (var br = item.Item2.Fill.CreateBrush(bounds, Root.RenderScaling))
            //        {
            //            g.FillPath(br, item.Item1);
            //        }
            //    }
            //}

        }
        /// <summary>
        /// 通过坐标测试选中的数据
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PieChartData HitTestData(Point point)
        {
            foreach (var item in paths)
            {
                if (item.Item1.Contains(point.X, point.Y))
                {
                    return item.Item2;
                }
            }
            return null;
        }

        PieChartData hoverChartData;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Handled)
            {
                return;
            }
            var hitTest = HitTestData(e.Location);
            if (hitTest != null)
            {
                if (tipPanel)
                {
                    if (hoverChartData != hitTest)
                    {
                        tipPanel.Visibility = Visibility.Visible;
                        tipListPanel.Children.Clear();
                        var template = SerieTemplate;
                        if (template != null)
                        {
                            var c = template.CreateElement();
                            c.DataContext = hitTest;
                            c.Name = hitTest.Name;
                            c.Value = hitTest.Value;
                            tipListPanel.Children.Add(c);
                        }
                        var padding = Padding;
                        tipPanel.MarginLeft = e.Location.X + 5 - padding.Left;
                        tipPanel.MarginTop = e.Location.Y + 5 - padding.Top;
                    }
                }
            }
            else
            {
                if (tipPanel)
                {
                    tipPanel.Visibility = Visibility.Collapsed;
                }
            }
            hoverChartData = hitTest;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var item in paths)
            {
                item.Item1.Dispose();
            }
            paths.Clear();
            base.Dispose(disposing);
        }


    }
}
