using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CPF
{
    /// <summary>
    /// 径向渐变
    /// </summary>
    public class RadialGradientFill : ViewFill
    {
        /// <summary>
        /// 径向渐变
        /// </summary>
        public RadialGradientFill()
        {
            //transform.PropertyChanged += Transform_PropertyChanged;
        }

        //private void Transform_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        //{
        //    Invalidate();
        //}

        /// <summary>
        /// 获取或设置径向渐变的最外面圆的中心。
        /// </summary>
        [PropertyMetadata(typeof(PointField), "50%,50%")]
        public PointField Center { get { return (PointField)GetValue(); } set { SetValue(value); } }
        /// <summary>
        /// 获取或设置径向渐变的最外面圆的半径。
        /// </summary>
        [PropertyMetadata(typeof(FloatField), "10")]
        public FloatField Radius { get { return (FloatField)GetValue(); } set { SetValue(value); } }
        ///// <summary>
        ///// 获取或设置径向渐变的最外面圆的垂直半径。
        ///// </summary>
        //public float RadiusY { get { return (float)GetValue(); } set { SetValue(value); } }
        ///// <summary>
        ///// 获取或设置用于定义渐变开始的二维焦点的位置。
        ///// </summary>
        //public Point GradientOrigin { get; }
        public Matrix Matrix
        {
            get { return GetValue<Matrix>(); }
            set { SetValue(value); }
        }

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

        //ViewFillTransform transform = new ViewFillTransform();
        //public ViewFillTransform Transform
        //{
        //    get { return transform; }
        //}
        public override Brush CreateBrush(in Rect rect, in float renderScaling)
        {
            var center = Center;
            return new RadialGradientBrush(new Point(center.X.GetActualValue(rect.Width) + rect.X, center.Y.GetActualValue(rect.Height) + rect.Y), Radius.GetActualValue(Math.Min(rect.Width, rect.Height)), GradientStops.ToArray(), Matrix);
            //throw new Exception("未实现");
        }
    }
}
