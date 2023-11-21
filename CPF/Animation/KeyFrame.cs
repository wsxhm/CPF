using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Drawing;

namespace CPF.Animation
{
    /// <summary>
    /// 关键帧
    /// </summary>
    public abstract class KeyFrame //: ICloneable
    {
        /// <summary>
        /// 动画模式
        /// </summary>
        public AnimateMode AnimateMode { get; set; }
        /// <summary>
        /// 需要变化的属性
        /// </summary>
        public string Property { get; set; }

        public object InterpolateValue(object baseValue, float keyFrameProgress)
        {
            if (keyFrameProgress < 0.0
           || keyFrameProgress > 1.0)
            {
                throw new ArgumentOutOfRangeException("keyFrameProgress");
            }

            switch (AnimateMode)
            {
                case AnimateMode.EaseIn:
                    keyFrameProgress = EaseInCore(keyFrameProgress);
                    break;
                case AnimateMode.EaseOut:
                    // EaseOut is the same as EaseIn, except time is reversed & the result is flipped.
                    keyFrameProgress = 1.0f - EaseInCore(1f - keyFrameProgress);
                    break;
                case AnimateMode.EaseInOut:
                    // EaseInOut is a combination of EaseIn & EaseOut fit to the 0-1, 0-1 range.
                    keyFrameProgress = (keyFrameProgress < 0.5) ?
                               EaseInCore(keyFrameProgress * 2f) * 0.5f :
                        (1f - EaseInCore((1f - keyFrameProgress) * 2f)) * 0.5f + 0.5f;
                    break;
                default:
                    break;
            }
            return InterpolateValueCore(baseValue, keyFrameProgress);
        }

        protected abstract object InterpolateValueCore(object baseValue, float keyFrameProgress);
        /// <summary>
        /// 获取设置的目标值
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();
        /// <summary>
        /// 缓动动画动画进度变化
        /// </summary>
        /// <param name="normalizedTime"></param>
        /// <returns></returns>
        protected abstract float EaseInCore(float normalizedTime);
        //public abstract object Clone();

        /// <summary>
        /// 缓动动画效果
        /// </summary>
        public IEase Ease { get; set; }
    }
    /// <summary>
    /// 定义一个类型数据的关键帧，默认支持byte,Color,double,float,int,short,long,Point,Rect,Size,Thickness,Vector,GeneralTransform,Matrix,Transform,SolidColorFill
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyFrame<T> : KeyFrame
    {
        /// <summary>
        /// 定义一个类型数据的关键帧，默认支持byte,Color,double,float,int,short,long,Point,Rect,Size,Thickness,Vector,GeneralTransform,Matrix,Transform,SolidColorFill
        /// </summary>
        public KeyFrame()
        { }

        /// <summary>
        /// 目标值
        /// </summary>
        public T Value { get; set; }

        public override object GetValue()
        {
            return Value;
        }
        /// <summary>
        /// 用了解决Aot模式下无法使用反射泛型构建KeyFrame问题
        /// </summary>
        internal static Dictionary<Type, Func<KeyFrame>> KeyFrameTypes = new Dictionary<Type, Func<KeyFrame>>
        {

            { typeof(FloatField),()=>new KeyFrame<FloatField>()},
            { typeof(SolidColorFill),()=>new KeyFrame<SolidColorFill>()},
            { typeof(byte),()=>new KeyFrame<byte>()},
            { typeof(short),()=>new KeyFrame<short>()},
            { typeof(int),()=>new KeyFrame<int> ()},
            { typeof(long),()=>new KeyFrame<long>()},
            { typeof(float),()=>new KeyFrame<float>()},
            { typeof(double),()=>new KeyFrame<double>()},
            { typeof(Color),()=>new KeyFrame<Color>()},
            { typeof(Point),()=>new KeyFrame<Point>()},
            { typeof(Rect),()=>new KeyFrame<Rect>()},
            { typeof(Size),()=>new KeyFrame<Size>()},
            { typeof(Thickness),()=>new KeyFrame<Thickness>()},
            { typeof(Vector),()=>new KeyFrame<Vector>()},
            { typeof(GeneralTransform),()=>new KeyFrame<GeneralTransform>()},
            { typeof(Transform),()=>new KeyFrame<Transform>()},
            { typeof(Matrix),()=>new KeyFrame<Matrix>()},
        };


        Type type;
        protected override object InterpolateValueCore(object baseValue, float keyFrameProgress)
        {
            if (keyFrameProgress == 0.0)
            {
                return baseValue;
            }
            else if (keyFrameProgress == 1.0)
            {
                return Value;
            }
            if (type == null)
            {
                if (Value != null)
                {
                    type = Value.GetType();
                }
                else
                {
                    type = typeof(T);
                }
            }
            if (type == typeof(FloatField))
            {
                var target = (FloatField)GetValue();
                var v = AnimatedTypeHelpers.InterpolateSingle(((FloatField)baseValue).Value, target.Value, keyFrameProgress);
                return new FloatField(v, target.Unit);
            }
            if (type == typeof(SolidColorFill))
            {
                var baseBrush = baseValue as SolidColorFill;
                Color color = Color.Transparent;
                if (baseBrush != null)
                {
                    color = baseBrush.Color;
                }
                var toBrush = GetValue() as SolidColorFill;
                var c = AnimatedTypeHelpers.InterpolateColor(color, toBrush.Color, keyFrameProgress);
                return new SolidColorFill { Color = c };
            }
            if (type == typeof(byte))
            {
                return AnimatedTypeHelpers.InterpolateByte((byte)baseValue, (byte)GetValue(), keyFrameProgress);
            }
            if (type == typeof(short))
            {
                return AnimatedTypeHelpers.InterpolateInt16((short)baseValue, (short)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(int))
            {
                return AnimatedTypeHelpers.InterpolateInt32((int)baseValue, (int)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(long))
            {
                return AnimatedTypeHelpers.InterpolateInt64((long)baseValue, (long)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(float))
            {
                return AnimatedTypeHelpers.InterpolateSingle((float)baseValue, (float)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(double))
            {
                return AnimatedTypeHelpers.InterpolateDouble((double)baseValue, (double)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(Color))
            {
                return AnimatedTypeHelpers.InterpolateColor((Color)baseValue, (Color)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(Point))
            {
                return AnimatedTypeHelpers.InterpolatePoint((Point)baseValue, (Point)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(Rect))
            {
                return AnimatedTypeHelpers.InterpolateRect((Rect)baseValue, (Rect)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(Size))
            {
                return AnimatedTypeHelpers.InterpolateSize((Size)baseValue, (Size)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(Thickness))
            {
                return AnimatedTypeHelpers.InterpolateThickness((Thickness)baseValue, (Thickness)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(Vector))
            {
                return AnimatedTypeHelpers.InterpolateVector((Vector)baseValue, (Vector)GetValue(), keyFrameProgress);
            }
            else if (type == typeof(GeneralTransform))
            {
                float box = 0f, boy = 0f, bskx = 0f, bsky = 0f, ba = 0f;
                float bscx = 1f, bscy = 1f;
                var b = baseValue as GeneralTransform;
                if (b != null)
                {
                    box = b.OffsetX;
                    boy = b.OffsetY;
                    bskx = b.SkewX;
                    bsky = b.SkewY;
                    bscx = b.ScaleX;
                    bscy = b.ScaleY;
                    ba = b.Angle;
                }
                //if (keyFrameProgress == 0f || b == null)
                {
                    b = new GeneralTransform();
                }
                var v = (GeneralTransform)GetValue();
                b.OffsetX = AnimatedTypeHelpers.InterpolateSingle(box, v.OffsetX, keyFrameProgress);
                b.OffsetY = AnimatedTypeHelpers.InterpolateSingle(boy, v.OffsetY, keyFrameProgress);
                b.SkewX = AnimatedTypeHelpers.InterpolateSingle(bskx, v.SkewX, keyFrameProgress);
                b.SkewY = AnimatedTypeHelpers.InterpolateSingle(bsky, v.SkewY, keyFrameProgress);
                b.ScaleX = AnimatedTypeHelpers.InterpolateSingle(bscx, v.ScaleX, keyFrameProgress);
                b.ScaleY = AnimatedTypeHelpers.InterpolateSingle(bscy, v.ScaleY, keyFrameProgress);
                b.Angle = AnimatedTypeHelpers.InterpolateSingle(ba, v.Angle, keyFrameProgress);
                return b;
                //return new GeneralTransform(ox, oy, skx, sky, scx, scy, a);
            }
            else if (type == typeof(Transform))
            {
                Transform a = baseValue as Transform;
                Matrix m = Matrix.Identity;
                if (a != null)
                {
                    m = a.Value;
                }
                Matrix t = ((Transform)GetValue()).Value;
                return new MatrixTransform(AnimatedTypeHelpers.InterpolateMatrix(m, t, keyFrameProgress));
            }
            else if (type == typeof(Matrix))
            {
                return AnimatedTypeHelpers.InterpolateMatrix((Matrix)baseValue, (Matrix)GetValue(), keyFrameProgress);
            }
            throw new NotImplementedException("不支持该类型动画:" + type);
        }
        //protected virtual T InterpolateValueCore(T baseValue, float keyFrameProgress)
        //{
        //    throw new NotImplementedException();
        //}

        protected override float EaseInCore(float normalizedTime)
        {
            if (Ease != null)
            {
                return Ease.EaseInCore(normalizedTime);
            }
            throw new NotImplementedException("Ease不能为null");
        }

        //public override object Clone()
        //{
        //    return new KeyFrame<T>() { AnimateMode = AnimateMode, Ease = Ease, Property = Property, Value = Value };
        //}
    }

}
