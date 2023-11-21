using CPF.Styling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 Calendar 上的日。
    /// </summary>
    [Description("表示 Calendar 上的日。"), Browsable(false)]
    public class CalendarDayButton : Button
    {
        protected override void InitializeComponent()
        {
            BorderFill = null;
            Background = null;
            this.Triggers.Add(new Trigger { Property = nameof(IsMouseOver), Setters = { { nameof(Background), "220,237,244" } } });
            this.Triggers.Add(new Trigger { Property = nameof(IsSelected), Setters = { { nameof(Background), "203,229,238" },{nameof(BorderFill), "76,189,218" } } });
            this.Triggers.Add(new Trigger { Property = nameof(IsBlackedOut), PropertyConditions = a => (bool)a, Setters = { { nameof(Foreground), "139,139,139" } } });
            Children.Add(new Border
            {
                Name = "contentPresenter",
                Height = "100%",
                Width = "100%",
                BorderFill = null,
                PresenterFor = this
            });
            BorderType = BorderType.BorderStroke;
            CornerRadius = new CornerRadius(3);
            IsAntiAlias = true;
            FontSize = 10;
        }
        /// <summary>
        /// 是否不是本月的
        /// </summary>
        public bool IsBlackedOut
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }
        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }
}
