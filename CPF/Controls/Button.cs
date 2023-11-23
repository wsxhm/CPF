using System;
using System.Collections.Generic;
using System.Text;
using CPF.Styling;
using CPF.Input;
using CPF.Drawing;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 Windows 按钮控件，该按钮对 Click 事件做出反应。
    /// </summary>
    [Description("表示 Windows 按钮控件，该按钮对 Click 事件做出反应。"), Browsable(true)]
    public class Button : ButtonBase
    {
        protected override void InitializeComponent()
        {
            this.Triggers.Add(nameof(IsMouseOver), Relation.Me, null, (nameof(Background), "190,230,253"));
            this.Triggers.Add(nameof(IsPressed), Relation.Me, null, (nameof(Background), "186,209,226"));
            Children.Add(new Border
            {
                Name = "contentPresenter",
                Height = "100%",
                Width = "100%",
                BorderFill = null,
                PresenterFor = this
            });
        }


        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(BorderStroke), new UIPropertyMetadataAttribute(typeof(Stroke), "1", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(BorderFill), new UIPropertyMetadataAttribute((ViewFill)"#101010", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(false, UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"221,221,221", UIPropertyOptions.AffectsRender));
        }

      

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //}

        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseDown(e);

        //    var re = Relation.Me.Parent.Find(a => a is TextBlock);
        //    foreach (var item in re.Query(this))
        //    {
        //        System.Diagnostics.Debug.WriteLine(item);
        //    }
        //}
    }
}
