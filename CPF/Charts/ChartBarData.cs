using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPF.Charts
{
    /// <summary>
    /// 柱状图数据
    /// </summary>
    public class ChartBarData : CpfObject, IChartData
    {
        /// <summary>
        /// 柱状图数据
        /// </summary>
        public ChartBarData()
        {
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
        /// <summary>
        /// 显示数值
        /// </summary>
        public bool ShowValueTip
        {
            get { return GetValue<bool>(); }
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
        /// 数据表示填充
        /// </summary>
        public ViewFill Fill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 是否是层叠样式
        /// </summary>
        public bool StackStyle
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public int DataCount { get { return Data.Count; } }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 定义数值格式化方式 double.ToString(format)
        /// </summary>
        public string Format
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public double GetMaxValue(int startIndex, int length)
        {
            var data = this.Data;
            if (StackStyle)
            {
                data = new List<double>();
                var list = Chart.Data.Where(a => a is ChartBarData && ((ChartBarData)a).StackStyle).Select(a => a as ChartBarData).ToList();
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    double value = 0;
                    foreach (var item in list)
                    {
                        if (item.DataCount > i)
                        {
                            value += item.Data[i];
                        }
                    }
                    data.Add(value);
                }
                startIndex = 0;
                length = data.Count;
            }
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
            var data = this.Data;
            if (StackStyle)
            {
                data = new List<double>();
                var list = Chart.Data.Where(a => a is ChartBarData && ((ChartBarData)a).StackStyle).Select(a => a as ChartBarData).ToList();
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    double value = 0;
                    foreach (var item in list)
                    {
                        if (item.DataCount > i)
                        {
                            value += item.Data[i];
                        }
                    }
                    data.Add(value);
                }

            }
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

        //public string GetTipString(int index)
        //{
        //    return Name + "：" + Data[index];
        //}

        public void Paint(DrawingContext graphics, int startIndex, int length, Rect rectangle, double maxValue, double minValue, in Rect rect, in float renderScaling)
        {
            if (maxValue == minValue)
            {
                return;
            }
            var data = Data;
            if (StackStyle)
            {
                var list = Chart.Data.Where(a => a is ChartBarData && ((ChartBarData)a).StackStyle).Select(a => a as ChartBarData).ToList();
                double width = rectangle.Width / 2.0;
                if (length > 1)
                {
                    width = rectangle.Width * 1.0 / (length - 1);
                }
                double offset = 0;
                if (Fill != null)
                {
                    using (var sb = Fill.CreateBrush(rect, renderScaling))
                    {
                        for (int i = startIndex; i < startIndex + length; i++)
                        {
                            double value = 0;
                            foreach (var item in list)
                            {
                                if (item.DataCount > i)
                                {
                                    if (item == this)
                                    {
                                        var h = (item.Data[i] - minValue) / (maxValue - minValue) * rectangle.Height;
                                        var h1 = (value - minValue) / (maxValue - minValue) * rectangle.Height;
                                        graphics.FillRectangle(sb, new Rect((float)(rectangle.X + offset - width / 4), (float)(rectangle.Top + rectangle.Height - h - h1), (float)(width / 2), (float)h));
                                        offset += width;
                                        break;
                                    }
                                    else
                                    {
                                        value += item.Data[i];
                                    }

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                List<Point> points = new List<Point>();
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
                var index = Chart.Data.Where(a => a is ChartBarData && !((ChartBarData)a).StackStyle).Select((a, b) => new { a, b }).First(a => a.a == this).b;
                var count = Chart.Data.Where(a => a is ChartBarData && !((ChartBarData)a).StackStyle).Count();
                var offsetWidth = width / (count * 4 + 1);

                for (int i = startIndex; i < startIndex + length; i++)
                {
                    var value = data[i];
                    var h2 = rectangle.Height - (value - minValue) / (maxValue - minValue) * rectangle.Height;
                    points.Add(new Point((int)(rectangle.X + offset), (int)(h2 + rectangle.Top)));
                    offset += width;
                }
                if (Fill != null)
                {
                    using (var sb = Fill.CreateBrush(rect, renderScaling))
                    {
                        foreach (var item in points)
                        {
                            graphics.FillRectangle(sb, new Rect((int)(item.X - width / 2 + offsetWidth + offsetWidth * index * 4), item.Y, Math.Max(0, (int)(offsetWidth * 3)), Math.Max(0, rectangle.Height - item.Y + rectangle.Top - 2)));
                        }
                        if (ShowValueTip)
                        {
                            var format = Format;
                            //var data = Data;
                            if (startIndex + length > data.Count)
                            {
                                length = data.Count - startIndex;
                            }
                            var font = new Font(Chart.FontFamily, Chart.FontSize, Chart.FontStyle);
                            for (int i = startIndex; i < startIndex + length; i++)
                            {
                                var value = data[i];
                                var point = points[i - startIndex];
                                var str = value.ToString(format);
                                var s = graphics.DrawingFactory.MeasureString(str, font);
                                graphics.DrawString(new Point((float)(point.X - width / 2 + offsetWidth + offsetWidth * index * 4) - s.Width / 2 + Math.Max(0, (int)(offsetWidth * 3)) / 2, point.Y - s.Height - 5), sb, str, font);
                            }
                        }
                    }
                }

            }
        }

        public void PaintBackground(DrawingContext graphics, int startIndex, int length, Rect rectangle, double maxValue, double minValue, in Rect rect, in float renderScaling)
        {
            //throw new NotImplementedException();
        }

        Chart Chart;
        public void SetOwnerChart(Chart chart)
        {
            Chart = chart;
        }

        public double GetValue(int index)
        {
            return Data[index];
        }
    }
}
