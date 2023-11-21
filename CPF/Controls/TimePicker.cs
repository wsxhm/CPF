using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个允许用户选择时间的控件。
    /// </summary>
    [Description("表示一个允许用户选择时间的控件。")]
    public class TimePicker : Control
    {
        //模板定义
        protected override void InitializeComponent()
        {
            Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new StackPanel
                    {
                        PresenterFor=this,
                        Name="hourPanel",
                        Orientation = Orientation.Vertical,
                        Focusable=true,
                        FocusFramePadding=new Thickness(0),
                        Children =
                        {
                            new RepeatButton
                            {
                                Name="hourUpButton",
                                Width =10,
                                Height = 10,
                                Focusable=false,
                                Content =new Polyline { Points = { { 0, 5 }, { 5, 0 }, { 10, 5 } },IsAntiAlias=true },
                                Triggers=
                                {
                                    {
                                        nameof(IsMouseOver),
                                        Relation.Me.Find(a => a is Polyline),null,
                                        (nameof(Polyline.StrokeFill),"28,151,234" )
                                    }
                                } ,
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsKeyboardFocusWithin),
                                        this,
                                        BindingMode.OneWay,
                                        (bool visible) => visible?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {nameof(RepeatButton.Click),(s,e)=> HourAdd(1) }
                                }
                            },
                            new TextBlock
                            {
                                PresenterFor=this,
                                Name="hour",
                                Bindings =
                                {
                                    {
                                        nameof(TextBlock.Text),
                                        nameof(SelectedTime),
                                        this,
                                        BindingMode.OneWay,
                                        (TimeSpan time) => ((int)time.TotalHours).ToString("00")
                                    }
                                }
                            },
                            new RepeatButton
                            {
                                Name="hourDownButton",
                                Width = 10,
                                Height = 10,
                                Focusable=false,
                                Content =new Polyline
                                {
                                    Points ={ { 0, 0 },{ 5, 5 },{ 10, 0 } },IsAntiAlias=true
                                },
                                Triggers=
                                {
                                    {
                                        nameof(IsMouseOver),
                                        Relation.Me.Find(a => a is Polyline),null,
                                        (nameof(Polyline.StrokeFill),"28,151,234" )
                                    }
                                },
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsKeyboardFocusWithin),
                                        this,
                                        BindingMode.OneWay,
                                        (bool visible) => visible?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {nameof(RepeatButton.Click),(s,e)=> HourAdd(-1) }
                                }
                            }
                        },
                        Commands =
                        {
                            {nameof(MouseWheel),(s,e)=>HourMouseWheel(e as Input.MouseWheelEventArgs) },
                            {nameof(MouseDown),(s,e)=>(s as UIElement).Focus() },
                            {nameof(KeyDown),(s,e)=>HourKeydown(e as Input.KeyEventArgs) },
                        }
                    },
                    new TextBlock
                    {
                        Classes="colon",
                        Text=":",
                        MarginLeft=3,
                        MarginRight=3,
                    },
                    new StackPanel
                    {
                        PresenterFor=this,
                        Name="minutePanel",
                        Orientation = Orientation.Vertical,
                        Focusable=true,
                        FocusFramePadding=new Thickness(0),
                        Children =
                        {
                            new RepeatButton
                            {
                                Name="minuteUpButton",
                                Width = 10,
                                Height = 10,
                                Focusable=false,
                                Content =new Polyline { Points = { { 0, 5 }, { 5, 0 }, { 10, 5 } },IsAntiAlias=true },
                                Triggers=
                                {
                                    {
                                        nameof(IsMouseOver),
                                        Relation.Me.Find(a => a is Polyline),null,
                                        (nameof(Polyline.StrokeFill),"28,151,234" )
                                    }
                                } ,
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsKeyboardFocusWithin),
                                        this,
                                        BindingMode.OneWay,
                                        (bool visible) => visible?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {nameof(RepeatButton.Click),(s,e)=> MinuteAdd(1) }
                                }
                            },
                            new TextBlock
                            {
                                PresenterFor=this,
                                Name="minute",
                                Bindings =
                                {
                                    {
                                        nameof(TextBlock.Text),
                                        nameof(SelectedTime),
                                        this,
                                        BindingMode.OneWay,
                                        (TimeSpan time) => time.Minutes.ToString("00")
                                    }
                                }
                            },
                            new RepeatButton
                            {
                                Name="minuteDownButton",
                                Width = 10,
                                Height = 10,
                                Focusable=false,
                                Content =new Polyline
                                {
                                    Points ={ { 0, 0 },{ 5, 5 },{ 10, 0 } },IsAntiAlias=true
                                },
                                Triggers=
                                {
                                    {
                                        nameof(IsMouseOver),
                                        Relation.Me.Find(a => a is Polyline),null,
                                        (nameof(Polyline.StrokeFill),"28,151,234" )
                                    }
                                },
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsKeyboardFocusWithin),
                                        this,
                                        BindingMode.OneWay,
                                        (bool visible) => visible?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {nameof(RepeatButton.Click),(s,e)=> MinuteAdd(-1) }
                                }
                            }
                        },
                        Commands =
                        {
                            {nameof(MouseWheel),(s,e)=>MinuteMouseWheel(e as Input.MouseWheelEventArgs) },
                            {nameof(MouseDown),(s,e)=>(s as UIElement).Focus() },
                            {nameof(KeyDown),(s,e)=>MinuteKeydown(e as Input.KeyEventArgs) },
                        }
                    },
                    new TextBlock
                    {
                        Classes="colon",
                        MarginLeft=3,
                        MarginRight=3,
                        Text=":"
                    },
                    new StackPanel
                    {
                        PresenterFor=this,
                        Name="secondPanel",
                        FocusFramePadding=new Thickness(0),
                        Orientation = Orientation.Vertical,
                        Focusable=true,
                        Children =
                        {
                            new RepeatButton
                            {
                                Name="secondUpButton",
                                Width = 10,
                                Height = 10,
                                Focusable=false,
                                Content =new Polyline { Points = { { 0, 5 }, { 5, 0 }, { 10, 5 } },IsAntiAlias=true },
                                Triggers=
                                {
                                    {
                                        nameof(IsMouseOver),
                                        Relation.Me.Find(a => a is Polyline),null,
                                        (nameof(Polyline.StrokeFill),"28,151,234" )
                                    }
                                } ,
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsKeyboardFocusWithin),
                                        this,
                                        BindingMode.OneWay,
                                        (bool visible) => visible?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {nameof(RepeatButton.Click),(s,e)=> SecondAdd(1) }
                                }
                            },
                            new TextBlock
                            {
                                Name="second",
                                Bindings =
                                {
                                    {
                                        nameof(TextBlock.Text),
                                        nameof(SelectedTime),
                                        this,
                                        BindingMode.OneWay,
                                        (TimeSpan time) => time.Seconds.ToString("00")
                                    }
                                }
                            },
                            new RepeatButton
                            {
                                Name="secondDownButton",
                                Width = 10,
                                Height = 10,
                                Focusable=false,
                                Content =new Polyline
                                {
                                    Points ={ { 0, 0 },{ 5, 5 },{ 10, 0 } },IsAntiAlias=true
                                },
                                Triggers=
                                {
                                    {
                                        nameof(IsMouseOver),
                                        Relation.Me.Find(a => a is Polyline),null,
                                        (nameof(Polyline.StrokeFill),"28,151,234" )
                                    }
                                },
                                Bindings =
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(IsKeyboardFocusWithin),
                                        this,
                                        BindingMode.OneWay,
                                        (bool visible) => visible?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {nameof(RepeatButton.Click),(s,e)=> SecondAdd(-1) }
                                }
                            }
                        },
                        Commands =
                        {
                            {nameof(MouseWheel),(s,e)=>SecondMouseWheel(e as Input.MouseWheelEventArgs) },
                            {nameof(MouseDown),(s,e)=>(s as UIElement).Focus() },
                            {nameof(KeyDown),(s,e)=>SecondKeydown(e as Input.KeyEventArgs) },
                        }
                    },
                }
            });
        }

        protected void HourKeydown(Input.KeyEventArgs args)
        {
            if (args.Key == Input.Keys.Up)
            {
                HourAdd(1);
            }
            else if (args.Key == Input.Keys.Down)
            {
                HourAdd(-1);
            }
            else if (args.Key == Input.Keys.Right)
            {
                var minute = FindPresenterByName<UIElement>("minutePanel");
                if (minute)
                {
                    minute.Focus(Input.NavigationMethod.Directional);
                }
            }
        }
        protected void MinuteKeydown(Input.KeyEventArgs args)
        {
            if (args.Key == Input.Keys.Up)
            {
                MinuteAdd(1);
            }
            else if (args.Key == Input.Keys.Down)
            {
                MinuteAdd(-1);
            }
            else if (args.Key == Input.Keys.Right)
            {
                var second = FindPresenterByName<UIElement>("secondPanel");
                if (second)
                {
                    second.Focus(Input.NavigationMethod.Directional);
                }
            }
            else if (args.Key == Input.Keys.Left)
            {
                var hour = FindPresenterByName<UIElement>("hourPanel");
                if (hour)
                {
                    hour.Focus(Input.NavigationMethod.Directional);
                }
            }
        }

        protected void SecondKeydown(Input.KeyEventArgs args)
        {
            if (args.Key == Input.Keys.Up)
            {
                SecondAdd(1);
            }
            else if (args.Key == Input.Keys.Down)
            {
                SecondAdd(-1);
            }
            else if (args.Key == Input.Keys.Left)
            {
                var minute = FindPresenterByName<UIElement>("minutePanel");
                if (minute)
                {
                    minute.Focus(Input.NavigationMethod.Directional);
                }
            }
        }
        protected void HourMouseWheel(Input.MouseWheelEventArgs e)
        {
            if (e.Delta.Y > 0)
            {
                HourAdd(1);
            }
            else
            {
                HourAdd(-1);
            }
        }
        protected void HourAdd(int hour)
        {
            //if (SelectedTime.Hours == 0 && hour < 0)
            //{
            //    return;
            //}
            SelectedTime = SelectedTime.Add(TimeSpan.FromHours(hour));
        }

        protected void MinuteMouseWheel(Input.MouseWheelEventArgs e)
        {
            if (e.Delta.Y > 0)
            {
                MinuteAdd(1);
            }
            else
            {
                MinuteAdd(-1);
            }
        }

        protected void MinuteAdd(int minute)
        {
            SelectedTime = SelectedTime.Add(TimeSpan.FromMinutes(minute));
        }
        protected void SecondMouseWheel(Input.MouseWheelEventArgs e)
        {
            if (e.Delta.Y > 0)
            {
                SecondAdd(1);
            }
            else
            {
                SecondAdd(-1);
            }
        }

        protected void SecondAdd(int Second)
        {
            SelectedTime = SelectedTime.Add(TimeSpan.FromSeconds(Second));
        }
        /// <summary>
        /// 设置最小的时间范围
        /// </summary>
        public TimeSpan MinTime
        {
            get { return (TimeSpan)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 设置最大时间的范围
        /// </summary>
        public TimeSpan MaxTime
        {
            get { return (TimeSpan)GetValue(); }
            set { SetValue(value); }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            //overridePropertys.Override(nameof(MinTime), new PropertyMetadataAttribute(TimeSpan.MinValue));
            overridePropertys.Override(nameof(MaxTime), new PropertyMetadataAttribute(TimeSpan.FromHours(24)));
            //overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
        }

        /// <summary>
        /// 选中的时间
        /// </summary>
        public TimeSpan SelectedTime
        {
            get { return (TimeSpan)GetValue(); }
            set { SetValue(value); }
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(SelectedTime))
            {
                var v = (TimeSpan)value;
                var min = MinTime;
                if (v < min)
                {
                    value = min;
                }
                else
                {
                    var max = MaxTime;
                    if (v > max)
                    {
                        value = max;
                    }
                }
            }
            else if (propertyName == nameof(MaxTime))
            {
                var max = (TimeSpan)value;
                if (max < MinTime)
                {
                    throw new Exception("MaxTime不能小于MinTime");
                }
                if (SelectedTime > max)
                {
                    SelectedTime = max;
                }
            }
            else if (propertyName == nameof(MinTime))
            {
                var min = (TimeSpan)value;
                if (min > MaxTime)
                {
                    throw new Exception("MaxTime不能小于MinTime");
                }
                if (SelectedTime < min)
                {
                    SelectedTime = min;
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

    }
}
