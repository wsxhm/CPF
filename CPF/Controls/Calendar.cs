using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using CPF.Drawing;
using CPF.Shapes;

namespace CPF.Controls
{
    /// <summary>
    /// 代表一个控件，此控件允许用户使用可视的日历显示来选择日期
    /// </summary>
    [Description("代表一个控件，此控件允许用户使用可视的日历显示来选择日期")]
    public class Calendar : Control
    {
        public Calendar()
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

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            if (pm.PropertyName == nameof(FirstDayOfWeek))
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            }
            return base.OnGetDefaultValue(pm);
        }

        Grid Month;
        Grid Year;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Month = FindPresenterByName<Grid>("MonthView");
            Year = FindPresenterByName<Grid>("YearView");
            var date = DisplayDate.Date;
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            if (Month != null)
            {
                //Month.LineStroke = "1";
                Month.Children.Add(new Panel
                {
                    Name = "Header",
                    Background = "233,238,243",
                    Size = SizeField.Fill,
                    Children =
                    {
                        new Panel
                        {
                            MarginLeft=0,
                            Children =
                            {
                                new Polygon
                                {
                                    MarginLeft=10,
                                    Points =
                                    {
                                        new Point(0,5),
                                        new Point(6,0),
                                        new Point(6,10),
                                    },
                                    StrokeFill=null,
                                    Fill="#000",
                                    IsAntiAlias=true,
                                }
                            },
                            Triggers =
                            {
                                {nameof(IsMouseOver),Relation.Me.Children(),null,(nameof(Shape.Fill),"115,169,216") }
                            },
                            Commands =
                            {
                                {nameof(MouseDown),preMonth }
                            }
                        },
                        new Panel
                        {
                            MarginRight=0,
                            Children =
                            {
                                new Polygon
                                {
                                    MarginRight=10,
                                    Points =
                                    {
                                        new Point(),
                                        new Point(6,5),
                                        new Point(0,10),
                                    },
                                    StrokeFill=null,
                                    Fill="#000",
                                    IsAntiAlias=true,
                                }
                            },
                            Triggers =
                            {
                                {nameof(IsMouseOver),Relation.Me.Children(),null,(nameof(Shape.Fill),"115,169,216") }
                            },
                            Commands =
                            {
                                {nameof(MouseDown),nextMonth }
                            }
                        },
                        new TextBlock
                        {
                            Triggers =
                            {
                                {nameof(IsMouseOver),Relation.Me,null,(nameof(TextBlock.Foreground),"115,169,216") }
                            },
                            Bindings =
                            {
                                {nameof(TextBlock.Text),nameof(DisplayDate),this,BindingMode.OneWay,(DateTime a)=>a.ToString("yyyy-MM") }
                            },
                            Commands =
                            {
                                {nameof(MouseDown),(s,e)=>DisplayMode= CalendarMode.Year }
                            }
                        }
                    },
                    Attacheds =
                    {
                        {Grid.ColumnSpan,7 }
                    }
                });
                var first = (int)FirstDayOfWeek;
                for (int i = 0; i < 7; i++)
                {
                    Month.Children.Add(new TextBlock
                    {
                        Text = cultureInfo.DateTimeFormat.ShortestDayNames[first],
                        DataContext = first,
                        Attacheds =
                        {
                            {Grid.ColumnIndex,i },
                            {Grid.RowIndex,1 }
                        }
                    });
                    first++;
                    if (first > 7 - 1)
                    {
                        first = 0;
                    }
                }
                int day = date.Day;
                DateTime one = date.AddDays(-day + 1);//这个月的第一天
                DateTime start = one.AddDays(first - (int)one.DayOfWeek);
                for (int i = 0; i < 42; i++)
                {
                    DateTime d = start.AddDays(i);
                    Month.Children.Add(new CalendarDayButton
                    {
                        Width = "100%",
                        Content = d.Day,
                        DataContext = d,
                        Attacheds =
                        {
                            {Grid.ColumnIndex,i%7 },
                            {Grid.RowIndex,i/7+2 }
                        },
                        Bindings =
                        {
                            {nameof(CalendarDayButton.IsSelected),nameof(SelectedDate),this,BindingMode.OneWay,(DateTime? a)=>a.HasValue&&a.Value.Date==(DateTime)Binding.Current.Owner.DataContext },
                            {nameof(CalendarDayButton.IsBlackedOut),nameof(DisplayDate),this,BindingMode.OneWay,(DateTime a)=>a.Month!=((DateTime)Binding.Current.Owner.DataContext).Month }
                        },
                        Commands =
                        {
                            {nameof(Button.Click),click }
                        }
                    });
                }
            }

            if (Year != null)
            {
                Year.Children.Add(new Panel
                {
                    Name = "header",
                    Background = "233,238,243",
                    Size = SizeField.Fill,
                    Children =
                    {
                        new Panel
                        {
                            MarginLeft=0,
                            Children =
                            {
                                new Polygon
                                {
                                    MarginLeft=10,
                                    Points =
                                    {
                                        new Point(0,5),
                                        new Point(6,0),
                                        new Point(6,10),
                                    },
                                    StrokeFill=null,
                                    Fill="#000",
                                    IsAntiAlias=true,
                                }
                            },
                            Triggers =
                            {
                                {nameof(IsMouseOver),Relation.Me.Children(),null,(nameof(Shape.Fill),"115,169,216") }
                            },
                            Commands =
                            {
                                {nameof(MouseDown),preYear }
                            }
                        },
                        new Panel
                        {
                            MarginRight=0,
                            Children =
                            {
                                new Polygon
                                {
                                    MarginRight=10,
                                    Points =
                                    {
                                        new Point(),
                                        new Point(6,5),
                                        new Point(0,10),
                                    },
                                    StrokeFill=null,
                                    Fill="#000",
                                    IsAntiAlias=true,
                                }
                            },
                            Triggers =
                            {
                                {nameof(IsMouseOver),Relation.Me.Children(),null,(nameof(Shape.Fill),"115,169,216") }
                            },
                            Commands =
                            {
                                {nameof(MouseDown),nextYear }
                            }
                        },
                        new TextBlock
                        {
                            Bindings =
                            {
                                {nameof(TextBlock.Text),nameof(DisplayDate),this,BindingMode.OneWay,(DateTime a)=>a.ToString("yyyy") }
                            }
                        }
                    },
                    Attacheds =
                    {
                        {Grid.ColumnSpan,7 }
                    }
                });

                for (int i = 0; i < 12; i++)
                {
                    Year.Children.Add(new CalendarButton
                    {
                        Size = SizeField.Fill,
                        Content = cultureInfo.DateTimeFormat.AbbreviatedMonthNames[i],
                        DataContext = new DateTime(date.Year, i + 1, 1),
                        Attacheds =
                        {
                            {Grid.ColumnIndex,i%4 },
                            {Grid.RowIndex,i/4+1 }
                        },
                        Bindings =
                        {
                             {nameof(CalendarButton.IsSelected),nameof(SelectedDate),this,BindingMode.OneWay,(DateTime? a)=>a.HasValue&&a.Value.Month==((DateTime)Binding.Current.Owner.DataContext).Month },
                        },
                        Commands =
                        {
                            {nameof(Button.Click),(s,e)=>{DisplayMode= CalendarMode.Month;SelectedDate=((DateTime)s.DataContext); } }
                        }
                    });
                }
            }
        }
        void preYear(CpfObject s, object e)
        {
            DisplayDate = DisplayDate.AddYears(-1);
            var i = 1;
            foreach (CalendarButton item in Year.Children.Where(a => a is CalendarButton))
            {
                item.DataContext = new DateTime(DisplayDate.Year, i, 1);
                i++;
            }
        }
        void nextYear(CpfObject s, object e)
        {
            DisplayDate = DisplayDate.AddYears(1);
            var i = 1;
            foreach (CalendarButton item in Year.Children.Where(a => a is CalendarButton))
            {
                item.DataContext = new DateTime(DisplayDate.Year, i, 1);
                i++;
            }
        }

        void preMonth(CpfObject s, object e)
        {
            DisplayDate = DisplayDate.AddMonths(-1);
        }
        void nextMonth(CpfObject s, object e)
        {
            DisplayDate = DisplayDate.AddMonths(1);
        }

        void click(CpfObject s, object e)
        {
            //var button = s as CalendarDayButton;
            SelectedDate = (DateTime)s.DataContext;
        }
        [PropertyChanged(nameof(DisplayDate))]
        void OnDisplayDate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!IsInitialized)
            {
                return;
            }
            //switch (DisplayMode)
            //{
            //    case CalendarMode.Month:
            if (Month)
            {
                var date = ((DateTime)newValue).Date;
                var i = 0;
                int day = date.Day;
                DateTime one = date.AddDays(-day + 1);//这个月的第一天
                var first = (int)FirstDayOfWeek;
                DateTime start = one.AddDays(first - (int)one.DayOfWeek);
                var select = SelectedDate;
                foreach (CalendarDayButton item in Month.Children.Where(a => a is CalendarDayButton))
                {
                    DateTime d = start.AddDays(i);
                    item.Content = d.Day;
                    item.DataContext = d;
                    item.IsSelected = select.HasValue && select.Value.Date == d;
                    i++;
                }
            }
            //        break;
            //    case CalendarMode.Year:
            //        break;
            //    default:
            //        break;
            //}
        }

        [PropertyChanged(nameof(SelectedDate))]
        void OnSelectDate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var date = (DateTime?)newValue;
            DisplayDate = date.HasValue ? date.Value.Date : DateTime.Now;
        }

        public void ClearDate(CpfObject obj, EventArgs args)
        {
            SelectedDate = null;
        }

        public void CloseUp(CpfObject obj, EventArgs args)
        {
            if (Root != null)
                (Root as Popup)?.Hide();
        }

        protected override void InitializeComponent()
        {
            Background = "#fff";
            BorderStroke = "1";
            BorderFill = "#aaa";
            BorderType = BorderType.BorderStroke;
            //CornerRadius = "3";
            Children.Add(new Grid
            {
                Margin = "3",
                MarginBottom = 40,
                Width = "100%",
                Height = "100%",
                Name = "MonthView",
                PresenterFor = this,
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                    new ColumnDefinition{ Width="auto", MinWidth=25 },
                },
                RowDefinitions =
                {
                    new RowDefinition{ Height="auto",MinHeight=25 },
                    new RowDefinition{ Height="auto",MinHeight=20 },
                    new RowDefinition{ Height="auto" },
                    new RowDefinition{ Height="auto" },
                    new RowDefinition{ Height="auto" },
                    new RowDefinition{ Height="auto" },
                    new RowDefinition{ Height="auto" },
                    new RowDefinition{ Height="auto" },
                },
                Bindings =
                {
                    {nameof(Visibility),nameof(DisplayMode),this,BindingMode.OneWay,(CalendarMode m)=>m== CalendarMode.Month?Visibility.Visible:Visibility.Collapsed }
                }
            });
            Children.Add(new StackPanel
            {
                Name = "buttons",
                Orientation = Orientation.Horizontal,
                MarginBottom = 10,
                Bindings =
                {
                    {nameof(Visibility),nameof(DisplayMode),this,BindingMode.OneWay,(CalendarMode m)=>m== CalendarMode.Month?Visibility.Visible:Visibility.Collapsed }
                },
                Children = {
                new TextBlock
                {
                    //Classes = "lblgreen",
                    Foreground = "#05AA69",
                    PresenterFor = this,
                    FontSize = 12f,
                    Name = "clear",
                    Text = "Clear",
                    Cursor = Cursors.Hand,
                    Commands =
                    {
                        {
                            nameof(TextBlock.MouseUp),
                            nameof(ClearDate),
                            this,
                            CommandParameter.EventSender,
                            CommandParameter.EventArgs
                        }
                    },
                },
                new TextBlock
                {
                    //Classes = "lblgreen",
                    Foreground = "#05AA69",
                    PresenterFor = this,
                    FontSize = 12f,
                    MarginLeft = 30,
                    Name = "ok",
                    Text = "OK",
                    Cursor = Cursors.Hand,
                    Commands =
                    {
                        {
                            nameof(TextBlock.MouseUp),
                            nameof(CloseUp),
                            this,
                            CommandParameter.EventSender,
                            CommandParameter.EventArgs
                        }
                    },

                } }
            });
            //Children.Add(new TextBlock
            //{
            //    Classes = "lblgreen",
            //    Foreground = "#05AA69",
            //    PresenterFor = this,
            //    MarginBottom = 10,
            //    FontSize = 14f,
            //    Name = "清除",
            //    Text = "清除",
            //    Cursor = Cursors.Hand,
            //    Commands =
            //    {
            //        {
            //            nameof(TextBlock.MouseUp),
            //            nameof(ClearDate),
            //            this,
            //            CommandParameter.EventSender,
            //            CommandParameter.EventArgs
            //        }
            //    },
            //    Bindings =
            //    {
            //        {nameof(Visibility),nameof(DisplayMode),this,BindingMode.OneWay,(CalendarMode m)=>m== CalendarMode.Month?Visibility.Visible:Visibility.Collapsed }
            //    }
            //});
            Children.Add(new Grid
            {
                Margin = "3",
                Width = "100%",
                Height = "100%",
                Name = "YearView",
                PresenterFor = this,
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width="auto", MinWidth=40 },
                    new ColumnDefinition{ Width="auto", MinWidth=40 },
                    new ColumnDefinition{ Width="auto", MinWidth=40 },
                    new ColumnDefinition{ Width="auto", MinWidth=40 },
                },
                RowDefinitions =
                {
                    new RowDefinition{ Height="auto",MinHeight=25 },
                    new RowDefinition{ Height="auto",MinHeight=40 },
                    new RowDefinition{ Height="auto",MinHeight=40 },
                    new RowDefinition{ Height="auto",MinHeight=40 },
                },
                Bindings =
                {
                    {nameof(Visibility),nameof(DisplayMode),this,BindingMode.OneWay,(CalendarMode m)=>m== CalendarMode.Year?Visibility.Visible:Visibility.Collapsed }
                }
            });
        }
    }


    public enum CalendarMode : byte
    {
        /// <summary>
        /// The Calendar displays a month at a time.
        /// </summary>
        Month = 0,

        /// <summary>
        ///  The Calendar displays a year at a time.
        /// </summary>
        Year = 1,

        ///// <summary>
        ///// The Calendar displays a decade at a time.
        ///// </summary>
        //Decade = 2,
    }
}
