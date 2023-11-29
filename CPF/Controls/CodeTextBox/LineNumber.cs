using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Documents;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CPF.Controls
{
    [Browsable(false)]
    public class LineNumber : Control
    {


        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            using (var font = new Font(FontFamily, FontSize, FontStyle))
            {
                var fore = Foreground;
                if (fore != null)
                {
                    using (var brush = fore.CreateBrush(new Rect(new Point(), ActualSize), Root.RenderScaling))
                    {
                        var top = Offset;
                        var lh = LineHeight;
                        for (int i = 0; i < Count; i++)
                        {
                            dc.DrawString(new Point(0, top), brush, (Start + i + 1).ToString(), font);
                            top += lh;
                        }
                    }
                }
            }
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            var s = base.MeasureOverride(availableSize);
            using (var font = new Font(FontFamily, FontSize, FontStyle))
            {
                s.Height += Count * LineHeight;
                s.Width = DrawingFactory.Default.MeasureString((Start + Count).ToString(), font).Width;
                return s;
            }
        }

        [UIPropertyMetadata(0, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure)]
        public int Start
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        [UIPropertyMetadata(0, UIPropertyOptions.AffectsRender | UIPropertyOptions.AffectsMeasure)]
        public int Count
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        [UIPropertyMetadata(0f, UIPropertyOptions.AffectsRender)]
        public float Offset
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        [UIPropertyMetadata(12f, UIPropertyOptions.AffectsRender)]
        public float LineHeight
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Foreground), new UIPropertyMetadataAttribute((ViewFill)"43,145,175", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits));
        }

    }
}
