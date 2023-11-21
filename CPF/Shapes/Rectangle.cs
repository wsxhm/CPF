using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;
using CPF.Input;

namespace CPF.Shapes
{
    /// <summary>
    /// 矩形
    /// </summary>
    [Description("矩形")]
    public class Rectangle : Shape
    {
        //protected override void OnRender(DrawingContext dc)
        //{
        //    base.OnRender(dc);
        //    using (Font f = new Font("微软雅黑", 12, FontStyles.Regular))
        //    {
        //        using (SolidColorBrush sb = new SolidColorBrush(Color.FromRgb(255, 20, 20)))
        //        {
        //            var ft = new FormattedText(Name + "\n换行", f);
        //            dc.DrawString(new Point(), sb, ft);
        //            dc.DrawRectangle(sb, new Drawing.Stroke(1), new Rect(0, 0, ft.Size.Width, ft.Size.Height));
        //        }
        //    }
        //}
        //LinearGradientBrush lb = new LinearGradientBrush(new GradientStop[] { new GradientStop { Position = 0, Color = Color.White }, new GradientStop { Color = Color.FromArgb(255, 255, 0, 0), Position = 0.3f }, new GradientStop { Color = Color.FromArgb(255, 255, 255, 0), Position = 0.5f }, new GradientStop { Color = Color.FromArgb(255, 0, 0, 255), Position = 1 } }, new Point(0, 0), new Point(100, 50));
        //SolidColorBrush sb = new SolidColorBrush(Color.FromRgb(20, 200, 50));
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    Fill = lb;
        //    Invalidate();
        //    //this.CaptureMouse();
        //}

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    base.OnMouseUp(e);
        //    Fill = sb;
        //    Invalidate();
        //    //this.ReleaseMouseCapture();
        //}

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    if (propertyName == "ActualSize")
        //    {
        //        InvalidateGeometry();
        //    }
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}
        protected override Size MeasureOverride(in Size availableSize)
        {
            return new Size();
        }

        protected override Drawing.PathGeometry CreateDefiningGeometry()
        {
            Drawing.PathGeometry path = new Drawing.PathGeometry();
            Stroke s = StrokeStyle;
            var sw = s.Width;
            //if (UseLayoutRounding)
            //{
            //    var scale = Host.GetDpi().DpiScaleY;
            //    sw = (float)Math.Round(sw * scale) / scale;
            //}
            Size size = ActualSize;
            //var p = Padding;
            path.BeginFigure((sw / 2), (sw / 2));
            path.LineTo((size.Width - sw / 2), (sw / 2));
            path.LineTo((size.Width - sw / 2), (size.Height - sw / 2));
            path.LineTo((sw / 2), (size.Height - sw / 2));
            path.LineTo((sw / 2), (sw / 2));
            path.EndFigure(true);
            return path;
        }
    }
}
