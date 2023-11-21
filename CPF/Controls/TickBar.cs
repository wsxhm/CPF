using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个控件，该控件为 Slider 控件绘制一组刻度线。
    /// </summary>
    [Description("表示一个控件，该控件为 Slider 控件绘制一组刻度线。"), Browsable(false)]
    public class TickBar : UIElement
    {
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Fill
        {
            get
            {
                return (ViewFill)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 最小值
        /// </summary>
        [Description("最小值")]
        public float Minimum
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        [Description("最大值")]
        [PropertyMetadata(1f)]
        public float Maximum
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置选择的起点。选择值范围中的第一个值。 默认值为-1.0。
        /// </summary>
        [PropertyMetadata(-1f)]
        public float SelectionStart
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置选择的终点。选择值范围中的最后一个值。 默认值为-1.0。
        /// </summary>
        [PropertyMetadata(-1f)]
        public float SelectionEnd
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置刻度线之间的间隔。刻度线之间的距离。 默认值为一 (1.0) 。
        /// </summary>
        [PropertyMetadata(1f)]
        public float TickFrequency
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示 Slider 是否沿 Slider 显示选择范围
        /// </summary>
        [Description("获取或设置一个值，该值指示 Slider 是否沿 Slider 显示选择范围")]
        public bool IsSelectionRangeEnabled
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置刻度线的位置。
        /// </summary>
        [Description("获取或设置刻度线的位置")]
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public Collection<float> Ticks
        {
            get
            {
                return (Collection<float>)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 获取或设置刻度线的增加值的方向。
        /// </summary>
        [Description("获取或设置刻度线的增加值的方向。")]
        [UIPropertyMetadata(false, UIPropertyOptions.AffectsRender)]
        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 刻度线相对于该控件实现的 Track 的位置
        /// </summary>
        [Description("刻度线相对于该控件实现的 Track 的位置")]
        [UIPropertyMetadata(TickBarPlacement.Top, UIPropertyOptions.AffectsRender)]
        public TickBarPlacement Placement
        {
            get { return (TickBarPlacement)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置包含为 TickBar 指定的刻度线的区域的空间缓冲区。
        /// </summary>
        [Description("获取或设置包含为 TickBar 指定的刻度线的区域的空间缓冲区。")]
        public float ReservedSpace
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        protected override void OnRender(DrawingContext dc)
        {
            var ActualWidth = ActualSize.Width;
            var ActualHeight = ActualSize.Height;
            Size size = new Size(ActualWidth, ActualHeight);
            float range = Maximum - Minimum;
            float tickLen = 0.0f;  // Height for Primary Tick (for Mininum and Maximum value)
            float tickLen2;        // Height for Secondary Tick
            float logicalToPhysical = 1f;
            float progression = 1f;
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);

            // Take Thumb size in to account
            float halfReservedSpace = ReservedSpace * 0.5f;

            switch (Placement)
            {
                case TickBarPlacement.Top:
                    if (FloatUtil.GreaterThanOrClose(ReservedSpace, size.Width))
                    {
                        return;
                    }
                    size.Width -= ReservedSpace;
                    tickLen = -size.Height;
                    startPoint = new Point(halfReservedSpace, size.Height);
                    endPoint = new Point(halfReservedSpace + size.Width, size.Height);
                    logicalToPhysical = size.Width / range;
                    progression = 1;
                    break;

                case TickBarPlacement.Bottom:
                    if (FloatUtil.GreaterThanOrClose(ReservedSpace, size.Width))
                    {
                        return;
                    }
                    size.Width -= ReservedSpace;
                    tickLen = size.Height;
                    startPoint = new Point(halfReservedSpace, 0f);
                    endPoint = new Point(halfReservedSpace + size.Width, 0f);
                    logicalToPhysical = size.Width / range;
                    progression = 1;
                    break;

                case TickBarPlacement.Left:
                    if (FloatUtil.GreaterThanOrClose(ReservedSpace, size.Height))
                    {
                        return;
                    }
                    size.Height -= ReservedSpace;
                    tickLen = -size.Width;
                    startPoint = new Point(size.Width, size.Height + halfReservedSpace);
                    endPoint = new Point(size.Width, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    progression = -1;
                    break;

                case TickBarPlacement.Right:
                    if (FloatUtil.GreaterThanOrClose(ReservedSpace, size.Height))
                    {
                        return;
                    }
                    size.Height -= ReservedSpace;
                    tickLen = size.Width;
                    startPoint = new Point(0f, size.Height + halfReservedSpace);
                    endPoint = new Point(0f, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    progression = -1;
                    break;
            };

            tickLen2 = tickLen * 0.75f;

            // Invert direciton of the ticks
            if (IsDirectionReversed)
            {
                progression = -progression;
                logicalToPhysical *= -1;

                // swap startPoint & endPoint
                Point pt = startPoint;
                startPoint = endPoint;
                endPoint = pt;
            }

            var pen = new Stroke(1);
            var f = Fill;
            if (f != null)
            {
                using (var fill = f.CreateBrush(new Rect(0, 0, size.Width, size.Height), Root.RenderScaling))
                {
                    bool snapsToDevicePixels = UseLayoutRounding;
                    Collection<float> xLines = snapsToDevicePixels ? new Collection<float>() : null;
                    Collection<float> yLines = snapsToDevicePixels ? new Collection<float>() : null;

                    // Is it Vertical?
                    if ((Placement == TickBarPlacement.Left) || (Placement == TickBarPlacement.Right))
                    {
                        // Reduce tick interval if it is more than would be visible on the screen
                        float interval = TickFrequency;
                        if (interval > 0.0)
                        {
                            float minInterval = (Maximum - Minimum) / size.Height;
                            if (interval < minInterval)
                            {
                                interval = minInterval;
                            }
                        }

                        // Draw Min & Max tick
                        dc.DrawLine(pen, fill, startPoint, new Point(startPoint.X + tickLen, startPoint.Y));
                        dc.DrawLine(pen, fill, new Point(startPoint.X, endPoint.Y),
                                         new Point(startPoint.X + tickLen, endPoint.Y));

                        if (snapsToDevicePixels)
                        {
                            xLines.Add(startPoint.X);
                            yLines.Add(startPoint.Y - 0.5f);
                            xLines.Add(startPoint.X + tickLen);
                            yLines.Add(endPoint.Y - 0.5f);
                            xLines.Add(startPoint.X + tickLen2);
                        }

                        // This property is rarely set so let's try to avoid the GetValue
                        // caching of the mutable default value
                        var ticks = Ticks;


                        // Draw ticks using specified Ticks collection
                        if ((ticks != null) && (ticks.Count > 0))
                        {
                            for (int i = 0; i < ticks.Count; i++)
                            {
                                if (FloatUtil.LessThanOrClose(ticks[i], Minimum) || FloatUtil.GreaterThanOrClose(ticks[i], Maximum))
                                {
                                    continue;
                                }

                                float adjustedTick = ticks[i] - Minimum;

                                float y = adjustedTick * logicalToPhysical + startPoint.Y;
                                dc.DrawLine(pen, fill,
                                    new Point(startPoint.X, y),
                                    new Point(startPoint.X + tickLen2, y));

                                if (snapsToDevicePixels)
                                {
                                    yLines.Add(y - 0.5f);
                                }
                            }
                        }
                        // Draw ticks using specified TickFrequency
                        else if (interval > 0.0)
                        {
                            for (float i = interval; i < range; i += interval)
                            {
                                float y = i * logicalToPhysical + startPoint.Y;

                                dc.DrawLine(pen, fill,
                                    new Point(startPoint.X, y),
                                    new Point(startPoint.X + tickLen2, y));

                                if (snapsToDevicePixels)
                                {
                                    yLines.Add(y - 0.5f);
                                }
                            }
                        }

                        // Draw Selection Ticks
                        if (IsSelectionRangeEnabled)
                        {
                            float y0 = (SelectionStart - Minimum) * logicalToPhysical + startPoint.Y;
                            Point pt0 = new Point(startPoint.X, y0);
                            Point pt1 = new Point(startPoint.X + tickLen2, y0);
                            Point pt2 = new Point(startPoint.X + tickLen2, y0 + Math.Abs(tickLen2) * progression);

                            //        PathSegment[] segments = new PathSegment[] {
                            //    new LineSegment(pt2, true),
                            //    new LineSegment(pt0, true),
                            //};
                            PathGeometry path = new PathGeometry();
                            path.BeginFigure(pt1.X, pt1.Y);
                            path.LineTo(pt2.X, pt2.Y);
                            path.LineTo(pt0.X, pt0.Y);
                            path.EndFigure(true);

                            //Geometry geo = new Geometry(path);

                            dc.DrawPath(fill, pen, path);

                            //geo.Dispose();
                            path.Dispose();


                            y0 = (SelectionEnd - Minimum) * logicalToPhysical + startPoint.Y;
                            pt0 = new Point(startPoint.X, y0);
                            pt1 = new Point(startPoint.X + tickLen2, y0);
                            pt2 = new Point(startPoint.X + tickLen2, y0 - Math.Abs(tickLen2) * progression);

                            //        segments = new PathSegment[] {
                            //    new LineSegment(pt2, true),
                            //    new LineSegment(pt0, true),
                            //};
                            path = new PathGeometry();
                            path.BeginFigure(pt1.X, pt1.Y);
                            path.LineTo(pt2.X, pt2.Y);
                            path.LineTo(pt0.X, pt0.Y);
                            path.EndFigure(true);
                            //geo = new Geometry(path);
                            dc.DrawPath(fill, pen, path);
                            //geo.Dispose();
                            path.Dispose();
                        }
                    }
                    else  // Placement == Top || Placement == Bottom
                    {
                        // Reduce tick interval if it is more than would be visible on the screen
                        float interval = TickFrequency;
                        if (interval > 0.0)
                        {
                            float minInterval = (Maximum - Minimum) / size.Width;
                            if (interval < minInterval)
                            {
                                interval = minInterval;
                            }
                        }

                        // Draw Min & Max tick
                        dc.DrawLine(pen, fill, startPoint, new Point(startPoint.X, startPoint.Y + tickLen));
                        dc.DrawLine(pen, fill, new Point(endPoint.X, startPoint.Y),
                                         new Point(endPoint.X, startPoint.Y + tickLen));

                        if (snapsToDevicePixels)
                        {
                            xLines.Add(startPoint.X - 0.5f);
                            yLines.Add(startPoint.Y);
                            xLines.Add(startPoint.X - 0.5f);
                            yLines.Add(endPoint.Y + tickLen);
                            yLines.Add(endPoint.Y + tickLen2);
                        }

                        // This property is rarely set so let's try to avoid the GetValue
                        // caching of the mutable default value
                        var ticks = Ticks;

                        // Draw ticks using specified Ticks collection
                        if ((ticks != null) && (ticks.Count > 0))
                        {
                            for (int i = 0; i < ticks.Count; i++)
                            {
                                if (FloatUtil.LessThanOrClose(ticks[i], Minimum) || FloatUtil.GreaterThanOrClose(ticks[i], Maximum))
                                {
                                    continue;
                                }
                                float adjustedTick = ticks[i] - Minimum;

                                float x = adjustedTick * logicalToPhysical + startPoint.X;
                                dc.DrawLine(pen, fill,
                                    new Point(x, startPoint.Y),
                                    new Point(x, startPoint.Y + tickLen2));

                                if (snapsToDevicePixels)
                                {
                                    xLines.Add(x - 0.5f);
                                }
                            }
                        }
                        // Draw ticks using specified TickFrequency
                        else if (interval > 0.0)
                        {
                            for (float i = interval; i < range; i += interval)
                            {
                                float x = i * logicalToPhysical + startPoint.X;
                                dc.DrawLine(pen, fill,
                                    new Point(x, startPoint.Y),
                                    new Point(x, startPoint.Y + tickLen2));

                                if (snapsToDevicePixels)
                                {
                                    xLines.Add(x - 0.5f);
                                }
                            }
                        }

                        // Draw Selection Ticks
                        if (IsSelectionRangeEnabled)
                        {
                            float x0 = (SelectionStart - Minimum) * logicalToPhysical + startPoint.X;
                            Point pt0 = new Point(x0, startPoint.Y);
                            Point pt1 = new Point(x0, startPoint.Y + tickLen2);
                            Point pt2 = new Point(x0 + Math.Abs(tickLen2) * progression, startPoint.Y + tickLen2);

                            PathGeometry path = new PathGeometry();
                            path.BeginFigure(pt1.X, pt1.Y);
                            path.LineTo(pt2.X, pt2.Y);
                            path.LineTo(pt0.X, pt0.Y);
                            path.EndFigure(true);

                            //Geometry geo = new Geometry(path);

                            dc.DrawPath(fill, pen, path);

                            //geo.Dispose();
                            path.Dispose();

                            x0 = (SelectionEnd - Minimum) * logicalToPhysical + startPoint.X;
                            pt0 = new Point(x0, startPoint.Y);
                            pt1 = new Point(x0, startPoint.Y + tickLen2);
                            pt2 = new Point(x0 - Math.Abs(tickLen2) * progression, startPoint.Y + tickLen2);

                            path = new PathGeometry();
                            path.BeginFigure(pt1.X, pt1.Y);
                            path.LineTo(pt2.X, pt2.Y);
                            path.LineTo(pt0.X, pt0.Y);
                            path.EndFigure(true);

                            //geo = new Geometry(path);

                            dc.DrawPath(fill, pen, path);

                            //geo.Dispose();
                            path.Dispose();
                        }
                    }

                    if (snapsToDevicePixels)
                    {
                        xLines.Add(ActualWidth);
                        yLines.Add(ActualHeight);
                        //VisualXSnappingGuidelines = xLines;
                        //VisualYSnappingGuidelines = yLines;
                    }
                }
            }
        }

    }

    public enum TickBarPlacement : byte
    {
        /// <summary>
        /// Position this tick at the left of target element.
        /// </summary>
        Left,

        /// <summary>
        /// Position this tick at the top of target element.
        /// </summary>
        Top,

        /// <summary>
        /// Position this tick at the right of target element.
        /// </summary>
        Right,

        /// <summary>
        /// Position this tick at the bottom of target element.
        /// </summary>
        Bottom,

        // NOTE: if you add or remove any values in this enum, be sure to update TickBar.IsValidTickBarPlacement()
    };

}
