using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Effects
{
    /// <summary>
    /// 应用多个位图特效
    /// </summary>
    public class EffectGroup : Effect, IEnumerable<Effect>
    {
        /// <summary>
        /// 应用多个位图特效
        /// </summary>
        public EffectGroup()
        { }
        public Effect this[int index]
        {
            get { return effects[index]; }
            set { effects[index] = value; }
        }

        public void Add(Effect effect)
        {
            effects.Add(effect);
        }

        public void Remove(Effect effect)
        {
            effects.Remove(effect);
        }

        public void RemoveAt(int i)
        {
            effects.RemoveAt(i);
        }

        public void Clear()
        {
            effects.Clear();
        }

        public int Count
        {
            get { return effects.Count; }
        }

        Collection<Effect> effects = new Collection<Effect>();
        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
            if (effects.Count == 0)
            {
                var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect, rect);
            }
            else if (effects.Count == 1)
            {
                effects[0].DoEffect(dc, bitmap);
            }
            else
            {
                var b = bitmap;
                foreach (var item in effects)
                {
                    Bitmap bmp = new Bitmap(bitmap.Width, bitmap.Height);
                    using (var d = DrawingContext.FromBitmap(bmp))
                    {
                        d.Clear(Color.Transparent);
                        item.DoEffect(d, b);
                    }
                    if (b != bitmap)
                    {
                        b.Dispose();
                    }
                    b = bmp;
                }
                var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
                dc.DrawImage(b, rect, rect);
                b.Dispose();
            }
        }

        public override Rect OverrideRenderRect(Rect rect)
        {
            if (effects.Count == 0)
            {
                return base.OverrideRenderRect(rect);
            }
            else
            {
                foreach (var item in effects)
                {
                    rect = item.OverrideRenderRect(rect);
                }
                return rect;
            }
        }

        public IEnumerator<Effect> GetEnumerator()
        {
            return effects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return effects.GetEnumerator();
        }
    }
}
