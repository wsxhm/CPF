using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Charts
{
    /// <summary>
    /// 图表折线，曲线显示数据
    /// </summary>
    public class ChartLineData : CpfObject, IChartData
    {
        /// <summary>
        /// 图表折线，曲线显示数据
        /// </summary>
        public ChartLineData()
        {
            Stroke = new Stroke(1);
            Data = new Collection<double>();
        }
        /// <summary>
        /// 数据
        /// </summary>
        public IList<double> Data
        {
            get { return GetValue<IList<double>>(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(Data))]
        void OnDataChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var nv = newValue as Collection<double>;
            if (nv != null)
            {
                nv.CollectionChanged += Nv_CollectionChanged;
            }
            else
            {
                var data = new Collection<double>();
                var list = newValue as IEnumerable<double>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        data.Add(item);
                    }
                }
                Data = data;
            }
        }

        private void Nv_CollectionChanged(object sender, CollectionChangedEventArgs<double> e)
        {
            NotifyPropertyChanged("Data");
        }
        /// <summary>
        /// 数值格式化方式 double.ToString(format)
        /// </summary>
        public string Format
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 点上面显示数值
        /// </summary>
        public bool ShowValueTip
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        //public string GetTipString(int index)
        //{
        //    return Name + "：" + Data[index];
        //}
        public double GetValue(int index)
        {
            return Data[index];
        }
        /// <summary>
        /// 显示数据点
        /// </summary>
        [PropertyMetadata(true)]
        public bool ShowPoint
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        List<Point> points = new List<Point>();
        public void PaintBackground(DrawingContext graphics, int startIndex, int length, Rect rectangle, double maxValue, double minValue, in Rect rect, in float renderScaling)
        {
            if (maxValue == minValue)
            {
                return;
            }
            var data = Data;
            points.Clear();
            if (data.Count == 0)
            {
                return;
            }
            if (startIndex > data.Count - 1)
            {
                return;
            }
            double width = rectangle.Width / 2.0;
            if (length > 1)
            {
                width = rectangle.Width * 1.0 / (length - 1);
            }
            if (startIndex + length > data.Count)
            {
                length = data.Count - startIndex;
            }
            double offset = 0;

            for (int i = startIndex; i < startIndex + length; i++)
            {
                var value = data[i];
                var h2 = rectangle.Height - (value - minValue) / (maxValue - minValue) * rectangle.Height;
                points.Add(new Point((int)(rectangle.X + offset), (int)(h2 + rectangle.Top)));
                offset += width;
            }

            if (BottomFill != null && points.Count > 1)
            {
                using (PathGeometry path = new PathGeometry())
                {
                    if (LineType == LineTypes.StraightLine || points.Count == 2)
                    {
                        path.BeginFigure(points[0].X, points[0].Y);
                        for (int i = 1; i < points.Count; i++)
                        {
                            path.LineTo(points[i].X, points[i].Y);
                        }
                        path.EndFigure(false);
                    }
                    else if (LineType == LineTypes.Curve)
                    {
                        var targetPoints = new List<Point>();
                        targetPoints.Add(points[0]);
                        targetPoints.AddRange(points);
                        targetPoints.Add(points[points.Count - 1]);

                        path.BeginFigure(
                            targetPoints[0].X, targetPoints[0].Y);
                        for (int i = 1; i < targetPoints.Count - 2; i++)
                        {
                            var controllerPoint1 = new Point(
                              targetPoints[i].X + (targetPoints[i + 1].X - targetPoints[i - 1].X) / 4,
                              targetPoints[i].Y + (targetPoints[i + 1].Y - targetPoints[i - 1].Y) / 4
                            );
                            var controllerPoint2 = new Point(
                              targetPoints[i + 1].X - (targetPoints[i + 2].X - targetPoints[i].X) / 4,
                              targetPoints[i + 1].Y - (targetPoints[i + 2].Y - targetPoints[i].Y) / 4
                            );
                            path.CubicTo(
                                new Point(controllerPoint1.X, controllerPoint1.Y),
                                new Point(controllerPoint2.X, controllerPoint2.Y),
                                new Point(targetPoints[i + 1].X, targetPoints[i + 1].Y)
                                );
                        }
                        path.EndFigure(false);
                    }
                    //path.AddLines(new Point[] { points[points.Count - 1], new Point(points[points.Count - 1].X, rectangle.Bottom), new Point(rectangle.Left, rectangle.Bottom), new Point(rectangle.X, rectangle.Y) });
                    path.BeginFigure(points[points.Count - 1].X, points[points.Count - 1].Y);
                    path.LineTo(points[points.Count - 1].X, rectangle.Bottom);
                    path.LineTo(rectangle.Left, rectangle.Bottom);
                    path.LineTo(points[0].X, points[0].Y);
                    path.EndFigure(true);
                    using (var fb = BottomFill.CreateBrush(rect, renderScaling))
                    {
                        graphics.FillPath(fb, path);
                    }
                }
            }
        }
        public void Paint(DrawingContext g, int startIndex, int length, Rect rectangle, double maxValue, double minValue, in Rect rect, in float renderScaling)
        {
            if (maxValue == minValue)
            {
                return;
            }
            if (points.Count == 0)
            {
                return;
            }
            if (LineFill != null)
            {
                using (var pen = LineFill.CreateBrush(rect, renderScaling))
                {
                    if (LineType == LineTypes.StraightLine || points.Count == 2)
                    {
                        for (int i = 1; i < points.Count; i++)
                        {
                            g.DrawLine(Stroke, pen, points[i - 1], points[i]);
                        }
                    }
                    else if (LineType == LineTypes.Curve)
                    {
                        using (PathGeometry path = new PathGeometry())
                        {
                            var targetPoints = new List<Point>();
                            targetPoints.Add(points[0]);
                            targetPoints.AddRange(points);
                            targetPoints.Add(points[points.Count - 1]);

                            for (int i = 1; i < targetPoints.Count - 2; i++)
                            {
                                path.BeginFigure(
                                    targetPoints[i].X, targetPoints[i].Y);
                                var controllerPoint1 = new Point(
                                  targetPoints[i].X + (targetPoints[i + 1].X - targetPoints[i - 1].X) / 4,
                                  targetPoints[i].Y + (targetPoints[i + 1].Y - targetPoints[i - 1].Y) / 4
                                );
                                var controllerPoint2 = new Point(
                                  targetPoints[i + 1].X - (targetPoints[i + 2].X - targetPoints[i].X) / 4,
                                  targetPoints[i + 1].Y - (targetPoints[i + 2].Y - targetPoints[i].Y) / 4
                                );
                                path.CubicTo(
                                    new Point(controllerPoint1.X, controllerPoint1.Y),
                                    new Point(controllerPoint2.X, controllerPoint2.Y),
                                    new Point(targetPoints[i + 1].X, targetPoints[i + 1].Y)
                                    );
                                path.EndFigure(false);
                            }
                            g.DrawPath(pen, Stroke, path);
                        }
                        //var array = points.ToArray();
                        //if (array.Length > 2)
                        //{
                        //    g.DrawCurve(pen, array);
                        //}
                        //else if (array.Length == 2)
                        //{
                        //    g.DrawLine(Stroke, pen, array[0], array[1]);
                        //}
                    }

                    if (ShowPoint)
                    {
                        using (Brush brush = new SolidColorBrush(Color.White))
                        {
                            foreach (var item in points)
                            {
                                g.FillEllipse(brush, new Point(item.X, item.Y), 3, 3);
                                g.DrawEllipse(pen, Stroke, new Point(item.X, item.Y), 3, 3);
                            }
                        }
                    }
                    if (ShowValueTip)
                    {
                        var format = Format;
                        var data = Data;
                        if (startIndex + length > data.Count)
                        {
                            length = data.Count - startIndex;
                        }
                        var font = new Font(chart.FontFamily,chart.FontSize,chart.FontStyle);
                        for (int i = startIndex; i < startIndex + length; i++)
                        {
                            var value = data[i];
                            var point = points[i - startIndex];
                            var str = value.ToString(format);
                            var s = g.DrawingFactory.MeasureString(str, font);
                            g.DrawString(new Point(point.X - s.Width / 2, point.Y - s.Height - 5), pen, str, font);
                        }
                    }
                }
            }
        }

        public double GetMaxValue(int startIndex, int length)
        {
            var data = Data;
            if (data.Count == 0)
            {
                return 0;
            }
            if (data.Count == 1)
            {
                return data[0];
            }
            else
            {
                //var range = 1 / horizontalScaling;//索引范围宽度
                //var scroll = 1 - range;//可以滚动的范围
                //var scrollStart = scroll * scrollValue;
                //int startIndex;
                //int length;
                //Dui2DChart.GetRange(horizontalScaling, scrollValue, Data.Count, out int startIndex, out int length);
                //return Data.Select(a => (double)a).Skip(startIndex).Take(length).Max();

                if (startIndex > data.Count - 1)
                {
                    return 0;
                }
                if (startIndex + length > data.Count)
                {
                    length = data.Count - startIndex;
                }
                double max = 0;
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    max = Math.Max(max, data[i]);
                }
                return max;
            }
        }

        public double GetMinValue(int startIndex, int length)
        {
            var data = Data;
            if (data.Count == 0)
            {
                return 0;
            }
            if (data.Count == 1)
            {
                return data[0];
            }
            else
            {
                if (startIndex > data.Count - 1)
                {
                    return 0;
                }
                if (startIndex + length > data.Count)
                {
                    length = data.Count - startIndex;
                }
                double min = 0;
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    min = Math.Min(min, data[i]);
                }
                return min;
            }
        }

        Chart chart;
        public void SetOwnerChart(Chart chart)
        {
            this.chart = chart;
        }
        /// <summary>
        /// 线条填充
        /// </summary>
        public ViewFill LineFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 线条类型
        /// </summary>
        public Stroke Stroke
        {
            get { return GetValue<Stroke>(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public int DataCount { get { return Data.Count; } }
        /// <summary>
        /// 线条类型
        /// </summary>
        [PropertyMetadata(LineTypes.StraightLine)]
        public LineTypes LineType
        {
            get { return GetValue<LineTypes>(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public ViewFill Fill { get { return LineFill; } }

        /// <summary>
        /// 底部填充笔刷
        /// </summary>
        public ViewFill BottomFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
    /// <summary>
    /// 图表线条类型
    /// </summary>
    public enum LineTypes
    {
        /// <summary>
        /// 不显示
        /// </summary>
        None,
        /// <summary>
        /// 曲线
        /// </summary>
        Curve,
        /// <summary>
        /// 直线
        /// </summary>
        StraightLine
    }
}
