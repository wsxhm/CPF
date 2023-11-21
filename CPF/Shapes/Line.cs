using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Shapes
{
    /// <summary>
    /// 在两个点之间绘制直线。
    /// </summary>
    [Description("在两个点之间绘制直线。")]
    public class Line : Shape
    {
        // public static readonly DependencyProperty<Point> StartPointProperty =
        //new DependencyProperty<Point>("StartPoint",typeof(Line), new Point());

        // public static readonly DependencyProperty<Point> EndPointProperty =
        //     new DependencyProperty<Point>("EndPoint",typeof(Line), new Point(100, 100));
        [PropertyMetadata(typeof(Point), "0,0")]
        public Point StartPoint
        {
            get { return (Point)GetValue(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(typeof(Point), "100,100")]
        public Point EndPoint
        {
            get { return (Point)GetValue(); }
            set { SetValue(value); }
        }

        [PropertyChanged(nameof(StartPoint))]
        [PropertyChanged(nameof(EndPoint))]
        void RegisterInvalidateGeometry(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            InvalidateGeometry();
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == nameof(StartPoint) || propertyName == nameof(EndPoint))
        //    {
        //        InvalidateGeometry();
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        protected override Drawing.PathGeometry CreateDefiningGeometry()
        {
            Drawing.PathGeometry path = new Drawing.PathGeometry();
            Stroke s = StrokeStyle;
            Point start = StartPoint;
            Point end = EndPoint;
            path.BeginFigure(start.X, start.Y);
            path.LineTo(end.X, end.Y);
            path.EndFigure(false);
            return path;
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(MarginTop), new UIPropertyMetadataAttribute((FloatField)0, UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginLeft), new UIPropertyMetadataAttribute((FloatField)0, UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MinHeight), new UIPropertyMetadataAttribute((FloatField)1, UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MinWidth), new UIPropertyMetadataAttribute((FloatField)1, UIPropertyOptions.AffectsMeasure));
        }
    }
}
