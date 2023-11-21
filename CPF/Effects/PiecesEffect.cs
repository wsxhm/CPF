using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Animation;

namespace CPF.Effects
{
    /// <summary>
    /// 切割特效，用来做动画
    /// </summary>
    public class PiecesEffect : Effect
    {
        /// <summary>
        /// 动画进度值0-1
        /// </summary>
        [PropertyMetadata(1f)]
        public float Value { get { return GetValue<float>(); } set { SetValue(value); } }

        /// <summary>
        /// 切割行数
        /// </summary>
        [PropertyMetadata(32u)]
        public uint RowCount { get { return GetValue<uint>(); } set { SetValue(value); } }
        /// <summary>
        /// 切割列数
        /// </summary>
        [PropertyMetadata(32u)]
        public uint ColCount { get { return GetValue<uint>(); } set { SetValue(value); } }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Value))
            {
                var o = (float)value;
                o = Math.Min(o, 1);
                o = Math.Max(0, o);
                value = o;
                opacity = 1 - o;
            }
            else if (propertyName == nameof(RowCount) || propertyName == nameof(ColCount))
            {
                value = Math.Max(2u, (uint)value);
                target = null;
                rects = null;
            }
            return base.OnSetValue(propertyName, ref value);
        }

        float opacity;

        List<Point> target;
        List<Rect> rects;
        int oldW;
        int oldH;


        //public override Rect OverrideRenderRect(Rect rect)
        //{
        //    //return base.OverrideRenderRect(rect);
        //    return new Rect(rect.X - scaleW, rect.Y - scaleH, rect.Width + scaleW * 2, rect.Height + scaleH * 2);
        //}

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            var w = bitmap.Width;
            var h = bitmap.Height;
            if (target == null || oldH != h || oldW != w)
            {
                oldW = w;
                oldH = h;
                int scaleW = w / 2;//分解之后放大的范围
                int scaleH = h / 2;//分解之后放大的范围
                target = new List<Point>();
                rects = new List<Rect>();
                var c = ColCount;
                var r = RowCount;
                Random random = new Random();
                int ah = 0;
                var ow = (int)(w / c);
                var oh = (int)(h / c);
                for (int y = 0; y < r; y++)
                {
                    int aw = 0;
                    for (int x = 0; x < c; x++)
                    {
                        var re = new Rect(x * ow, y * oh, ow, oh);
                        var rx = random.Next(0, scaleW);
                        var ry = random.Next(0, scaleH);
                        var rt = new Point(re.X, re.Y);
                        if (x >= c / 2)
                        {
                            if (x == c - 1)
                            {
                                re.Width = w - aw;
                            }
                            rt.X += rx;
                        }
                        else
                        {
                            rt.X -= rx;
                        }
                        if (y >= r / 2)
                        {
                            if (y == r - 1)
                            {
                                re.Height = h - ah;
                            }
                            rt.Y += ry;
                        }
                        else
                        {
                            rt.Y -= ry;
                        }
                        rects.Add(re);
                        target.Add(rt);
                        aw += ow;
                    }
                    ah += oh;
                }
            }
            for (int i = 0; i < target.Count; i++)
            {
                var t = target[i];
                var s = rects[i];
                var rect = new Rect(0, 0, s.Width, s.Height);
                rect.Location = AnimatedTypeHelpers.InterpolatePoint(s.Location, t, 1 - opacity);
                dc.DrawImage(bitmap, rect, s, opacity);
            }
        }
    }
}
