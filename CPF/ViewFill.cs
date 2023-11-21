using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Linq;
using System.ComponentModel;

namespace CPF
{
    ///// <summary>
    ///// 定义一个可视化元素的填充
    ///// </summary>
    //public interface IViewFill
    //{
    //    /// <summary>
    //    /// 创建笔刷
    //    /// </summary>
    //    /// <param name="rect">笔刷限定区域</param>
    //    /// <param name="drawingContext">绘图上下文</param>
    //    /// <returns></returns>
    //    Brush CreateBrush(Rect rect, DrawingContext drawingContext);
    //    /// <summary>
    //    /// 需要刷新界面的时候
    //    /// </summary>
    //    event Action Invalidated;
    //}
    /// <summary>
    /// 定义一个可视化元素的填充，默认支持 隐式转换的字符串格式，支持格式： #rrggbbaa、#rrggbb、r,g,b、r,g,b,a、或者直接设置图片，渐变格式 linear-gradient(startX startY,endX endY,color1 stop1,color2 stop2....) 颜色只能是#开头的格式,stop是0到1的数字，图片格式：url(img.gif) [no-repeat/repeat] [none/fill/uniform/UniformToFill]
    /// </summary>
    [TypeConverter(typeof(StringConverter))]
    [Description("定义一个可视化元素的填充，默认支持 隐式转换的字符串格式，支持格式： #rrggbbaa、#rrggbb、r,g,b、r,g,b,a、或者直接设置图片，渐变格式 linear-gradient(startX startY,endX endY,color1 stop1,color2 stop2....) 颜色只能是#开头的格式,stop是0到1的数字，图片格式：url(img.gif) [no-repeat/repeat] [none/fill/uniform/UniformToFill]")]
    public abstract class ViewFill : CpfObject
    {

        //public event Action Invalidated
        //{
        //    add { AddHandler(value); }
        //    remove { RemoveHandler(value); }
        //}
        ///// <summary>
        ///// 无效化笔刷区域，下次重绘
        ///// </summary>
        //public void Invalidate()
        //{
        //    var e = (Action)Events["Invalidated"];
        //    if (e != null)
        //    {
        //        e();
        //    }
        //}

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    Invalidate();
        //}
        /// <summary>
        /// 创建笔刷
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="renderScaling">DPI</param>
        /// <returns></returns>
        public abstract Brush CreateBrush(in Rect rect, in float renderScaling);


        public static ViewFill Parse(string s)
        {
            var str = s.ToLower();

            if (str.StartsWith("linear-gradient("))//
            {
                try
                {
                    var l = new LinearGradientFill { };
                    str = str.Substring(16).Trim(')');
                    var temp = str.Split(',');
                    if (temp[0].StartsWith("#"))
                    {
                        if (temp.Length < 2)
                        {
                            throw new Exception("渐变至少需要两个颜色");
                        }
                        foreach (var item in temp)
                        {
                            var t = item.Split(' ');
                            l.GradientStops.Add(new GradientStop(Color.Parse(t[0].Trim()), float.Parse(t[1])));
                        }
                    }
                    else
                    {
                        if (temp.Length < 4)
                        {
                            throw new Exception("渐变至少需要两个颜色");
                        }
                        if (temp[2].Trim().IndexOf(' ') < 0)
                        {
                            var w = 1.0f / (temp.Length - 3);
                            var offset = 0f;
                            for (int i = 2; i < temp.Length; i++)
                            {
                                var item = temp[i];
                                if (i == temp.Length - 1)
                                {
                                    offset = 1;
                                }
                                l.GradientStops.Add(new GradientStop(Color.Parse(item.Trim()), offset));
                                offset += w;
                            }
                        }
                        else
                        {
                            for (int i = 2; i < temp.Length; i++)
                            {
                                var item = temp[i];
                                var t = item.Split(' ');
                                l.GradientStops.Add(new GradientStop(Color.Parse(t[0].Trim()), float.Parse(t[1])));
                            }
                        }
                        var t1 = temp[0].Split(' ');
                        l.StartPoint = new PointField(t1[0], t1[1]);
                        var t2 = temp[1].Split(' ');
                        l.EndPoint = new PointField(t2[0], t2[1]);
                    }

                    return l;
                }
                catch (Exception e)
                {
                    throw new Exception("字符串转ViewFill失败:" + s + e.Message, e);
                }
            }
            else if (str.StartsWith("url("))
            {
                return new TextureFill(s);
            }
            else if (Color.TryParse(str, out Color color))
            {
                return new SolidColorFill() { Color = color };
            }
            throw new Exception("字符串转ViewFill失败:" + s);
        }

        /// <summary>
        /// 颜色格式字符串
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator ViewFill(string n)
        {
            return Parse(n);
        }
        /// <summary>
        /// Parses a color to Fill
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator ViewFill(Color n)
        {
            return new SolidColorFill() { Color = n };
        }
        public static implicit operator ViewFill(Styling.HtmlColor n)
        {
            return (ViewFill)(Color)n;
        }
        public static implicit operator ViewFill(Image n)
        {
            return new TextureFill(n);
        }
    }

    //public class ViewFillTransform : CPFObject
    //{
    //    public Matrix GetMatrix(Rect rect)
    //    {

    //        var m = Matrix.Identity;
    //        Point op = new Point(OriginX.GetActualValue(rect.Width), OriginY.GetActualValue(rect.Height));
    //        m.Translate(-op.X, -op.Y);
    //        //m.Prepend(this.VisualTransform.Value);
    //        m.Skew(SkewX, SkewY);
    //        m.Scale(ScaleX, ScaleY);
    //        m.Rotate(Angle);
    //        m.Translate(TranslateX, TranslateY);

    //        m.Translate(rect.Width / 2 + rect.X, rect.Height / 2 - rect.Y);
    //        return m;
    //    }
    //    /// <summary>
    //    /// 平移
    //    /// </summary>
    //    public float TranslateX { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 平移
    //    /// </summary>
    //    public float TranslateY { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 缩放
    //    /// </summary>
    //    [PropertyMetadata(1f)]
    //    public float ScaleX { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 缩放
    //    /// </summary>
    //    [PropertyMetadata(1f)]
    //    public float ScaleY { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 倾斜
    //    /// </summary>
    //    public float SkewX { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 倾斜
    //    /// </summary>
    //    public float SkewY { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 旋转
    //    /// </summary>
    //    public float Angle { get { return (float)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 变换原点
    //    /// </summary>
    //    [PropertyMetadata(typeof(FloatField), "50%")]
    //    public FloatField OriginX { get { return (FloatField)GetValue(); } set { SetValue(value); } }
    //    /// <summary>
    //    /// 变换原点
    //    /// </summary>
    //    [PropertyMetadata(typeof(FloatField), "50%")]
    //    public FloatField OriginY { get { return (FloatField)GetValue(); } set { SetValue(value); } }
    //}
}
