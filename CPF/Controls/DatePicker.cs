using CPF.Shapes;
using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Svg;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个允许用户选择日期的控件。
    /// </summary>
    [Description("表示一个允许用户选择日期的控件。")]
    public class DatePicker : Control
    {
        public DatePicker()
        {
            var sd = SelectedDate;
            if (sd != null)
            {
                DisplayDate = sd.Value;
            }
            else
            {
                DisplayDate = DateTime.Now;
            }
        }

        /// <summary>
        /// 选中的日期
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 获取或设置要显示的日期。
        /// </summary>
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 当前显示模式
        /// </summary>
        public CalendarMode DisplayMode
        {
            get { return (CalendarMode)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置被视为一周开始的日期。
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示组合框的下拉部分当前是否打开
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置用于显示选定日期的格式
        /// </summary>
        [PropertyMetadata("yyyy-MM-dd")]
        public string SelectedDateFormat
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 是否显示清空按钮
        /// </summary>
        public bool ShowClearButton
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 选中一个日期之后自动关闭
        /// </summary>
        public bool AutoClose
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 下拉框容器
        /// </summary>
        [NotCpfProperty]
        public Panel DropDownPanel { get; private set; } = new Panel { Name = "DatePickerDropDownPanel" };

        static Popup popup;
        private static Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = new Popup { Name = "DatePickerDropDownPopup", Padding = new Thickness(2) };
                    popup.Background = null;
                    popup.CanActivate = false;
                }
                return popup;
            }
        }
        protected override void InitializeComponent()
        {
            Background = "#fff";
            BorderFill = "#aaa";
            BorderType = BorderType.BorderStroke;
            BorderStroke = "1";
            //CornerRadius = "3";

            Children.Add(new TextBox
            {
                MarginRight = 18,
                MarginLeft = 0,
                MinHeight = FontSize,
                HScrollBarVisibility = ScrollBarVisibility.Hidden,
                VScrollBarVisibility = ScrollBarVisibility.Hidden,
                AcceptsReturn = false,
                AcceptsTab = false,
                IsReadOnly = true,
                Bindings =
                {
                    {nameof(TextBox.Text),nameof(SelectedDate),this,BindingMode.OneWay,(DateTime? date)=>date.HasValue?date.Value.ToString(SelectedDateFormat):"" },
                    {nameof(TextBox.Text),nameof(SelectedDateFormat),this,BindingMode.OneWay,(string format)=>SelectedDate.HasValue?SelectedDate.Value.ToString(format):"" },
                }
            });
            Children.Add(new Panel
            {
                MarginRight = 18,
                Children =
                {
                    new Line{ StartPoint=new Point(0,0), EndPoint=new Point(7,7), IsAntiAlias=true,StrokeFill="#aaa"},
                    new Line{ StartPoint=new Point(7,0), EndPoint=new Point(0,7), IsAntiAlias=true,StrokeFill="#aaa"},
                },
                Commands =
                {
                    {nameof(MouseDown),(s,e)=>SelectedDate=null }
                },
                Triggers =
                {
                    {nameof(IsMouseOver),Relation.Me.Children(),null,(nameof(Line.StrokeFill),"#ccc") }
                },
                Bindings =
                {
                    {nameof(Visibility),nameof(ShowClearButton),this,BindingMode.OneWay,(bool a)=>a?Visibility.Visible:Visibility.Collapsed }
                }
            });
            Children.Add(new Panel
            {
                Height = "100%",
                MarginRight = 0,
                Children =
                {
                    //new Polyline { MarginRight = 5, IsAntiAlias = true, Points = { new Point(), new Point(4, 4), new Point(8, 0) } },
                    new SVG("<svg p-id=\"15636\" width=\"16\" height=\"16\"><path d=\"M671.32617 1.527866c17.674161 0 31.988771 14.31461 31.988771 31.988771v31.988771h127.809017a191.056222 191.056222 0 0 1 190.983188 191.494424v574.994508a191.056222 191.056222 0 0 1-190.983188 191.056222h-639.775421A191.056222 191.056222 0 0 1 0.365349 837.252768V251.303201A191.056222 191.056222 0 0 1 191.42157 65.505408h128.466321V33.516637a31.988771 31.988771 0 1 1 63.977542 0v31.988771h255.471966V33.516637c0-17.674161 14.31461-31.988771 31.988771-31.988771z m287.533771 415.269754H63.612553v415.488855c0 70.550577 57.25844 127.735983 127.809017 127.735983h639.702388a127.955084 127.955084 0 0 0 127.735983-127.809017V416.870653zM296.15193 768.235899a36.516862 36.516862 0 1 1 0 73.033724h-109.550586a36.516862 36.516862 0 0 1 0-73.033724h109.550586z m292.061862 0a36.516862 36.516862 0 1 1 0 73.033724H442.146344a36.516862 36.516862 0 1 1 0-73.033724h146.067448z m255.471966 0a36.516862 36.516862 0 1 1 0 73.033724H734.354274a36.516862 36.516862 0 0 1 0-73.033724h109.404518zM296.15193 549.280795a36.516862 36.516862 0 1 1 0 73.033724h-109.550586a36.516862 36.516862 0 0 1 0-73.033724h109.550586z m292.061862 0a36.516862 36.516862 0 0 1 0 73.033724H442.146344a36.516862 36.516862 0 0 1 0-73.033724h146.067448z m255.471966 0a36.516862 36.516862 0 1 1 0 73.033724H734.354274a36.516862 36.516862 0 0 1 0-73.033724h109.404518zM319.157553 129.336882h-127.809016A127.809017 127.809017 0 0 0 64.415924 257.438034v95.528111h894.517051l-0.730338-96.039347a127.955084 127.955084 0 0 0-127.809016-127.735983H702.730671v63.320238a31.988771 31.988771 0 0 1-64.050576 0v-63.320238H383.135095v63.320238a31.988771 31.988771 0 0 1-63.977542 0v-63.320238z\" p-id=\"15637\"></path></svg>"){Width=10, Stretch= Stretch.Uniform,MarginRight = 5, IsAntiAlias = true, }
                },
                Commands =
                {
                    {nameof(PreviewMouseDown),(s,e)=>{if (!IsDropDownOpen){IsDropDownOpen = true;} } }
                }
            });
            DropDownPanel.Children.Add(new Calendar
            {
                Bindings =
                {
                    {nameof(SelectedDate),nameof(SelectedDate),this,BindingMode.TwoWay },
                    {nameof(DisplayDate),nameof(DisplayDate),this,BindingMode.TwoWay },
                    {nameof(DisplayMode),nameof(DisplayMode),this,BindingMode.TwoWay },
                    {nameof(FirstDayOfWeek),nameof(FirstDayOfWeek),this,BindingMode.TwoWay },
                }
            });
        }

        [PropertyChanged(nameof(IsDropDownOpen))]
        void OnIsDropDownOpen(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if ((bool)newValue)
            {
                if (Popup.PlacementTarget is DatePicker datePicker)
                {
                    datePicker.IsDropDownOpen = true;
                }
                //Popup.styleSheet = Root.StyleSheet;
                Popup.LoadStyle(Root);
                Popup.Placement = PlacementMode.Padding;
                popup.PropertyChanged += Popup_PropertyChanged;
                popup.MarginLeft = 0;
                popup.MarginTop = "100%";
                popup.Height = "auto";
                popup.Width = "auto";
                Popup.Children.Add(DropDownPanel);
                popup.PlacementTarget = this;
                popup.StaysOpen = false;
                Popup.Visibility = Visibility.Visible;
            }
            else
            {
                Popup.PropertyChanged -= Popup_PropertyChanged;
                Popup.Children.Remove(DropDownPanel);
                if (popup.PlacementTarget == this)
                {
                    Popup.Visibility = Visibility.Collapsed;
                    popup.PlacementTarget = null;
                }
            }
        }
        [PropertyChanged(nameof(DisplayMode))]
        void OnDisplayMode(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (popup != null && popup.PlacementTarget == this)
            {
                popup.Width = "auto";
                popup.Height = "auto";
            }
        }

        [PropertyChanged(nameof(SelectedDate))]
        void OnSelectedDate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (AutoClose)
            {
                IsDropDownOpen = false;
            }
        }

        protected override void OnDetachedFromVisualTree()
        {
            base.OnDetachedFromVisualTree();
            IsDropDownOpen = false;
        }

        private void Popup_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Visibility) && popup.PlacementTarget == this)
            {
                var newValue = (Visibility)e.NewValue;
                BeginInvoke(() =>
                {
                    IsDropDownOpen = newValue == Visibility.Visible;
                });
            }
        }
    }
}
