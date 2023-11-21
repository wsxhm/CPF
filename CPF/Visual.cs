using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF
{
    /// <summary>
    /// 表示一个可视化元素，提供呈现支持，包括命中测试，坐标变换
    /// </summary>
    public class Visual : CpfObject
    {
        //#region 注册属性和设置默认值
        //public readonly static Type VisualType = typeof(Visual);
        //public static readonly DependencyProperty VisualOffsetProperty = new DependencyProperty("VisualOffset", PointType, VisualType, new Point());
        //public static readonly DependencyProperty VisualTransformProperty = new DependencyProperty("VisualTransform", TransformType, VisualType, Transform.Identity);
        //public static readonly DependencyProperty VisualClipProperty = new DependencyProperty("VisualClip", GeometryType, VisualType, null);
        //public static readonly DependencyProperty CacheModeProperty = new DependencyProperty("CacheMode", CacheModeType, VisualType, CacheMode.Auto);
        //public static readonly DependencyProperty ClipToBoundsProperty = new DependencyProperty("ClipToBounds", BoolType, VisualType, false);
        //#endregion
        /// <summary>
        /// 可视化对象位置偏移
        /// </summary>
        //[PropertyMetadata(typeof(Point), "0,0")]
        [NotCpfProperty]
        protected virtual Point VisualOffset
        {
            get;
            set;
        }
        /// <summary>
        /// 可视化对象变换
        /// </summary>
        //[PropertyMetadata(typeof(Transform), "Identity")]
        [NotCpfProperty]
        protected virtual Transform VisualTransform
        {
            get;
            set;
        } = Transform.Identity;
        /// <summary>
        /// 可视对象的剪辑区域
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        protected virtual Geometry VisualClip
        {
            get { return (Geometry)GetValue(3); }
            set { SetValue(value,3); }
        }
        ///// <summary>
        ///// 图像缓存模式
        ///// </summary>
        //[PropertyMetadata(CacheMode.Auto)]
        //public CacheMode CacheMode
        //{
        //    get { return (CacheMode)GetValue(); }
        //    set { SetValue(value); }
        //}
        /// <summary>
        /// 获取或设置一个值，该值指示是否剪切此元素的内容(或来自此元素的子元素的内容)使其适合包含元素的大小。这是一个依赖项属性。
        /// </summary>
        [Description("获取或设置一个值，该值指示是否剪切此元素的内容(或来自此元素的子元素的内容)使其适合包含元素的大小。这是一个依赖项属性。")]
        [UIPropertyMetadata(false, UIPropertyOptions.AffectsRender)]
        public bool ClipToBounds
        {
            get { return (bool)GetValue(2); }
            set { SetValue(value,2); }
        }

        public virtual void Render(DrawingContext dc)
        {
            OnRender(dc);
        }
        

        protected virtual void OnRender(DrawingContext dc)
        {

        }
        /// <summary>
        /// 确定点是否在可视对象的内部，point需要包含偏移
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool HitTestCore(Point point)
        {
            if (VisualClip == null)
            {
                return GetContentBounds().Contains(point);
            }
            else
            {
                Point offset = VisualOffset;
                point.Offset(-offset.X, -offset.Y);
                return VisualClip.Contains(point);
            }
        }

        /// <summary>
        /// 获取内容边界,包含偏移，子类必须重写。不受变换影响
        /// </summary>
        /// <returns></returns>
        public virtual Rect GetContentBounds()
        {
            return Rect.Empty;
        }

        //public Rect VisualContentBounds
        //{
        //    get
        //    {
        //        return GetContentBounds();
        //    }
        //}

        public virtual Point PointToScreen(Point point)
        {
            return point;
        }

    }




}
