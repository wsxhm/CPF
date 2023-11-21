using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Styling;

namespace CPF.Controls
{
    /// <summary>
    /// 表示 Calendar 对象上的月份或年份。
    /// </summary>
    [Description(" 表示 Calendar 对象上的月份或年份。"), Browsable(false)]
    public class CalendarButton : Button
    {
        protected override void InitializeComponent()
        {
            BorderFill = null;
            Background = null;
            this.Triggers.Add(new Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a, Setters = { { nameof(Background), "220,237,244" } } });
            this.Triggers.Add(new Trigger { Property = nameof(IsSelected), PropertyConditions = a => (bool)a, Setters = { { nameof(Background), "203,229,238" }, { nameof(BorderFill), "76,189,218" } } });
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

        public bool IsSelected
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }
}
