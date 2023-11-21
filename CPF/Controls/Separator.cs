using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Shapes;

namespace CPF.Controls
{
    /// <summary>
    /// 分割线
    /// </summary>
    [Description("分割线")]
    public class Separator : Control
    {
        /// <summary>
        /// 分割线
        /// </summary>
        public Separator() { }

        //protected override void InitializeComponent()
        //{
        //    Background = "#ccc";
        //    Height = 1;
        //    //Width = "100%";
        //    MarginBottom = 2;
        //    MarginLeft = 30;
        //    MarginRight = 2;
        //    MarginTop = 2;
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute(typeof(FloatField), "1", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginBottom), new UIPropertyMetadataAttribute(typeof(FloatField), "2", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginLeft), new UIPropertyMetadataAttribute(typeof(FloatField), "30", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginRight), new UIPropertyMetadataAttribute(typeof(FloatField), "2", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginTop), new UIPropertyMetadataAttribute(typeof(FloatField), "2", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute(typeof(ViewFill), "#ccc", UIPropertyOptions.AffectsRender));
        }
    }
}
