using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Toolkit.Dialogs
{
    internal class DialogFrame : Control
    {
        public DialogFrame(IWindow window, UIElement content)
        {
            this.window = window;
            this.content = content;
        }

        /// <summary>
        /// 是否显示最大化还原按钮
        /// </summary>
        public bool MaximizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        /// <summary>
        /// 是否显示最小化
        /// </summary>
        [PropertyMetadata(true)]
        public bool MinimizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }

        IWindow window;
        /// <summary>
        /// 关联的窗体
        /// </summary>
        [NotCpfProperty]
        public IWindow Window
        {
            get { return window; }
        }

        UIElement content;
        /// <summary>
        /// 窗体的内容
        /// </summary>
        [NotCpfProperty]
        public UIElement Content
        {
            get { return content; }
        }

        IEnumerable<UIElement> systemButtons;
        /// <summary>
        /// 系统按钮集合
        /// </summary>
        [NotCpfProperty]
        public IEnumerable<UIElement> SystemButtons
        {
            get { return systemButtons; }
        }

        /// <summary>
        /// 阴影宽度
        /// </summary>
        [Description("阴影宽度")]
        [UIPropertyMetadata((byte)5, UIPropertyOptions.AffectsMeasure)]
        public byte ShadowBlur
        {
            get { return GetValue<byte>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 显示标题栏图标
        /// </summary>
        [Description("显示标题栏图标"), PropertyMetadata(true)]
        public bool ShowIcon
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {

            ViewFill color = "black";
            ViewFill hoverColor = "255,255,255,40";
            Width = "100%";
            Height = "100%";
            //窗体阴影
            var frame = Children.Add(new Border
            {
                Name = "frame",
                Width = "100%",
                Height = "100%",
                Background = "#fff",
                BorderType = BorderType.BorderStroke,
                BorderStroke = new Stroke(0),
                ShadowBlur = ShadowBlur,
                ShadowColor = Color.FromRgba(0, 0, 0, 150),
                Bindings =
                {
                    {
                        nameof(Border.ShadowBlur),
                        nameof(IWindow.WindowState),
                        window,
                        BindingMode.OneWay,
                        a => (WindowState)a == WindowState.Maximized||(WindowState)a == WindowState.FullScreen ? 0 : ShadowBlur
                    },
                    {
                        nameof(Border.ShadowBlur),
                        nameof(ShadowBlur),
                        this,
                        BindingMode.OneWay,
                        a =>window.WindowState == WindowState.Maximized||window. WindowState == WindowState.FullScreen ? 0 : (byte)a
                    },
                }
            });
            //用来裁剪内容，不然内容超出阴影
            var clip = new Decorator
            {
                Width = "100%",
                Height = "100%",
                ClipToBounds = true
            };
            frame.Child = clip;
            var grid = (Grid)(clip.Child = new Grid
            {
                Width = "100%",
                Height = "100%",
                ColumnDefinitions =
                {
                    new ColumnDefinition()
                },
                RowDefinitions =
                {
                    new RowDefinition
                    {
                        Height = "auto"
                    },
                    new RowDefinition
                    {

                    }
                },
            });
            //标题栏和按钮
            grid.Children.Add(
            new Panel
            {
                Name = "caption",
                Background = "white",
                Width = "100%",
                Height = "30",
                Commands =
                {
                    {
                        nameof(MouseDown),
                        nameof(IWindow.DragMove),
                        Window
                    },
                    {
                        nameof(DoubleClick),
                        (s,e)=> DoubleClickTitle()
                    }
                },
                Children =
                {
                    new StackPanel
                    {
                        Name="titleBox",
                        MarginLeft=0,
                        Orientation= Orientation.Horizontal,
                        Children=
                        {
                            new Picture
                            {
                                Name="icon",
                                MarginLeft=5,
                                Width=20,
                                Height=20,
                                Stretch= Stretch.Fill,
                                Bindings=
                                {
                                    {
                                        nameof(Picture.Source),
                                        nameof(window.Icon),
                                        window
                                    },
                                    {
                                        nameof(Visibility),
                                        nameof(window.Icon),
                                        window,
                                        BindingMode.OneWay,
                                        a=>a==null||!ShowIcon?Visibility.Collapsed:Visibility.Visible
                                    },
                                    {
                                        nameof(Visibility),
                                        nameof(ShowIcon),
                                        this,
                                        BindingMode.OneWay,
                                        (bool showIcon)=>!showIcon||window.Icon==null?Visibility.Collapsed:Visibility.Visible
                                    }
                                }
                            },
                            new TextBlock
                            {
                                Name="title",
                                MarginLeft=8,
                                MarginTop=2,
                                FontSize = 14,
                                Foreground="Gray",
                                Bindings=
                                {
                                    {
                                        nameof(TextBlock.Text),
                                        nameof(IWindow.Title),
                                        Window
                                    }
                                },
                            },
                        }
                    },
                    new StackPanel
                    {
                        Name="controlBox",
                        MarginRight=0,
                        Height = "100%",
                        Orientation= Orientation.Horizontal,
                        Children =
                        {
                            new SystemButton
                            {
                                Name="close",
                                ToolTip="关闭",
                                Width = 30,
                                Height = "100%",
                                Content=new Panel{
                                    Size=SizeField.Fill,
                                    Children =
                                    {
                                        new Line
                                        {
                                            MarginTop=8,
                                            MarginLeft=8,
                                            StartPoint = new Point(1, 1),
                                            EndPoint = new Point(14, 13),
                                            StrokeStyle = "2",
                                            IsAntiAlias=true,
                                            StrokeFill=color
                                        },
                                        new Line
                                        {
                                            MarginTop=8,
                                            MarginLeft=8,
                                            StartPoint = new Point(14, 1),
                                            EndPoint = new Point(1, 13),
                                            StrokeStyle = "2",
                                            IsAntiAlias=true,
                                            StrokeFill=color
                                        }
                                    }
                                },
                                Commands =
                                {
                                    {
                                        nameof(Button.Click),
                                        (s,e)=>
                                        {
                                            //(e as MouseButtonEventArgs).Handled=true;
                                            Window.Close();
                                        }
                                    }
                                },
                                Triggers=
                                {
                                    new Trigger(nameof(Panel.IsMouseOver), Relation.Me)
                                    {
                                        Setters =
                                        {
                                            {
                                                nameof(Panel.Background),
                                                hoverColor
                                            }
                                        }
                                    }
                                },
                            }
                        }
                    }
                }
            });
            if (Content != null)
            {
                grid.Children.Add(Content, 0, 1);
            }
        }

        protected void DoubleClickTitle()
        {
            if (MaximizeBox)
            {
                this.Delay(TimeSpan.FromMilliseconds(100), () =>
                {
                    if (Window.WindowState == WindowState.Normal)
                    { Window.WindowState = WindowState.Maximized; }
                    else if (Window.WindowState == WindowState.Maximized)
                    { Window.WindowState = WindowState.Normal; }
                });
            }
        }
    }
}
