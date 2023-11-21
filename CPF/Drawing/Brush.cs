using System;
using System.Collections.Generic;
using System.Text;
using CPF;

namespace CPF.Drawing
{
    /// <summary>
    /// 笔刷
    /// </summary>
    public abstract class Brush : IDisposable
    {
        //public static readonly DependencyProperty<Matrix> MatrixProperty = new DependencyProperty<Matrix>(nameof(Transform), typeof(Brush), Matrix.Identity);

        IDisposable brush;
        /// <summary>
        /// 适配器创建的笔刷，绘制过程中才会被创建
        /// </summary>
        public IDisposable AdapterBrush
        {
            get { return brush; }
            internal set { brush = value; }
        }
        //[PropertyMetadata(typeof(Matrix), "Identity")]
        //public Matrix Transform
        //{
        //    get { return GetValue<Matrix>(); }
        //    set { SetValue(value); }
        //}

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (brush != null)
        //    {
        //        brush.Dispose();
        //        brush = null;
        //    }
        //}
        

        public static Brush Parse(string str)
        {
            return new SolidColorBrush(Color.Parse(str));
        }

        public void Dispose()
        {
            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }
            Disposing();
            IsDisposed = true;
        }
        /// <summary>
        /// 释放之后由内部对象池保留，请不要在外部保留引用
        /// </summary>
        public bool IsDisposed
        {
            get;
            internal set;
        }

        protected virtual void Disposing()
        {

        }

        /// <summary>
        /// Parses a color string. #ffffff、r,g,b、a,r,g,b
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator Brush(string n)
        {
            return Parse(n);
        }
        /// <summary>
        /// Parses a color to brush
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator Brush(Color n)
        {
            return new SolidColorBrush(n);
        }
    }
}
