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
            //this.MinWidth = 80;
            //this.MinHeight = 35;
            //this.MarginRight = 1;
            this.Triggers.Add(new Trigger
            {
                Property = nameof(IsMouseOver),
                TargetRelation = Relation.Me,
                PropertyConditions = x => Convert.ToBoolean(x),
                Setters =
                {
                    { nameof(Background),"#E5F1FB" },
                    { nameof(BorderFill),"#0078D7" },
                    { nameof(Foreground),"black" },
                }
            });
            this.Triggers.Add(new Trigger
            {
                Property = nameof(IsPressed),
                TargetRelation = Relation.Me,
                PropertyConditions = x => Convert.ToBoolean(x),
                Setters =
                {
                    { nameof(Background),"204,228,247" },
                    { nameof(BorderFill),"0,84,153" },
                }
            });
            this.Triggers.Add(new Trigger
            {
                Property = nameof(IsFocused),
                TargetRelation = Relation.Me,
                PropertyConditions = x => Convert.ToBoolean(x),
                Setters =
                {
                    { nameof(Background),"204,228,247" },
                    { nameof(BorderFill),"0,120,215" },
                    { nameof(BorderStroke),"2" },
                }
            });
            this.triggers.Add(new Trigger
            {
                Property = nameof(IsEnabled),
                TargetRelation = Relation.Me,
                PropertyConditions = x => !Convert.ToBoolean(x),
                Setters =
                {
                    { nameof(Background),"204,204,204" },
                    { nameof(BorderFill),"191,191,191" },
                    { nameof(Foreground),"131,131,131" },
                }
            });
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
            overridePropertys.Override(nameof(BorderFill), new UIPropertyMetadataAttribute((ViewFill)"#ADADAD", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(false, UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"#E1E1E1", UIPropertyOptions.AffectsRender));
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
