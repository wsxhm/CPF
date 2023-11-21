using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    /// <summary>
    /// 线性渐变填充
    /// </summary>
    public class LinearGradientFill : ViewFill
    {
        /// <summary>
        /// 线性渐变填充
        /// </summary>
        public LinearGradientFill()
        {
            Matrix = Matrix.Identity;
            //transform.PropertyChanged += Transform_PropertyChanged;
            gradientStops.CollectionChanged += GradientStops_CollectionChanged;
        }

        private void GradientStops_CollectionChanged(object sender, CollectionChangedEventArgs<GradientStop> e)
        {
            NotifyPropertyChanged("GradientStops");
        }
        [PropertyMetadata(typeof(PointField), "0,100%")]
        public PointField EndPoint { get { return GetValue<PointField>(); } set { SetValue(value); } }

        public PointField StartPoint { get { return GetValue<PointField>(); } set { SetValue(value); } }

        ///// <summary>
        ///// 渐变角度0-360
        ///// </summary>
        //public float Angle
        //{
        //    get { return (float)GetValue(); }
        //    set { SetValue(value); }
        //}
        //ViewFillTransform transform = new ViewFillTransform();
        //public ViewFillTransform Transform
        //{
        //    get { return transform; }
        //}

        /// <summary>
        /// 颜色梯度
        /// </summary>
        public Collection<GradientStop> GradientStops
        {
            get
            {
                return gradientStops;
            }
        }

        Collection<GradientStop> gradientStops = new Collection<GradientStop>();

        //protected override bool OnSetValue<T>(string propertyName, ref T value)
        //{
        //    if (propertyName == "Angle")
        //    {
        //        if ((float)(object)value < 0)
        //        {
        //            value = (T)(object)0;
        //        }
        //        else if ((float)(object)value > 360)
        //        {
        //            value = (T)(object)360;
        //        }
        //    }
        //    return base.OnSetValue(propertyName, ref value);
        //}

        public Matrix Matrix
        {
            get { return GetValue<Matrix>(); }
            set { SetValue(value); }
        }

        public override Brush CreateBrush(in Rect rect, in float renderScaling)
        {
            //var w = 0f;
            //var a = transform.Angle % 180;
            //if (rect.Height / rect.Width > Math.Tan(Math.PI / 180 * a))
            //{
            //    if (a > 90)
            //    {
            //        a = 180 - a;
            //        var angle = Math.Atan((rect.Height / 2) / (rect.Width / 2));//弧度
            //        var l = rect.Height / 2 / (float)Math.Sin(angle);
            //        w = 2 * l * (float)Math.Cos(angle - Math.PI / 180 * a);
            //    }
            //    else
            //    {
            //        var angle = Math.Atan((rect.Height / 2) / (rect.Width / 2));//弧度
            //        var l = rect.Height / 2 / (float)Math.Sin(angle);
            //        w = 2 * l * (float)Math.Cos(angle - Math.PI / 180 * a);
            //    }
            //}
            //else
            //{
            //    if (a < 90)
            //    {
            //        a = 90 - a;
            //        var angle = Math.Atan((rect.Width / 2) / (rect.Height / 2));//弧度
            //        var l = rect.Width / 2 / (float)Math.Sin(angle);
            //        w = 2 * l * (float)Math.Cos(angle - Math.PI / 180 * a);
            //    }
            //    else
            //    {
            //        var angle = Math.Atan((rect.Width / 2) / (rect.Height / 2));//弧度
            //        var l = rect.Width / 2 / (float)Math.Sin(angle);
            //        w = 2 * l * (float)Math.Cos(angle - Math.PI / 180 * a);
            //    }
            //}


            return new LinearGradientBrush(gradientStops.ToArray(), new Point(StartPoint.X.GetActualValue(rect.Width) + rect.X, StartPoint.Y.GetActualValue(rect.Height) + rect.Y), new Point(EndPoint.X.GetActualValue(rect.Width) + rect.X, EndPoint.Y.GetActualValue(rect.Height) + rect.Y), Matrix);
        }

        public override string ToString()
        {
            if (gradientStops.Count == 0)
            {
                return "0,0,0,0";
            }
            else if (gradientStops.Count == 1)
            {
                return gradientStops[0].Color.ToString();
            }
            else
            {
                return $"linear-gradient({StartPoint.X} {StartPoint.Y},{EndPoint.X} {EndPoint.Y},{string.Join(",", gradientStops)})";
            }
        }

        public override string GetCreationCode()
        {
            if (gradientStops.Count == 0)
            {
                return "\"0,0,0,0\"";
            }
            else if (gradientStops.Count == 1)
            {
                return gradientStops[0].Color.GetCreationCode();
            }
            else
            {
                return $"\"linear-gradient({StartPoint.X} {StartPoint.Y},{EndPoint.X} {EndPoint.Y},{string.Join(",", gradientStops)})\"";
            }
        }
    }
}
