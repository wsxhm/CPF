using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Animation;
using CPF.Input;
using CPF.Effects;
using System.ComponentModel;
using CPF.Controls;
using System.Diagnostics;

namespace CPF.Controls
{
    /// <summary>
    /// 通用窗体框架，包含窗体边框，系统按钮，阴影这些元素
    /// </summary>
    [Description("通用窗体框架，包含窗体边框，系统按钮，阴影这些元素")]
    public class WindowFrame : ContentControl
    {
        /// <summary>
        /// 通用窗体框架，包含窗体边框，系统按钮，阴影这些元素
        /// </summary>
        /// <param name="window">绑定的窗体</param>
        /// <param name="content"></param>
        /// <param name="systemButtons">系统按钮，一般加SystemButton</param>
        public WindowFrame(IWindow window, UIElement content, params UIElement[] systemButtons)
        {
            this.window = window;
            this.Content = content;
            this.systemButtons = systemButtons;
        }

        public WindowFrame() { }

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

        //UIElement content;
        ///// <summary>
        ///// 窗体的内容
        ///// </summary>
        //[NotCpfProperty]
        //public UIElement Content
        //{
        //    get { return content; }
        //}

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

            ViewFill color = "#fff";
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
                Name = "contentGrid",
                PresenterFor = this,
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
                Children =
                {
                    new Border
                    {
                        Name = "contentPresenter",
                        Height = "100%",
                        Width = "100%",
                        BorderFill = null,
                        BorderStroke="0",
                        PresenterFor = this,
                        [Grid.RowIndex]=1,
                    }
                }
            });
            //标题栏和按钮
            grid.Children.Add(
            new Panel
            {
                Name = "caption",
                Background = "#1E9FFF",
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
                                Bindings=
                                {
                                    {
                                        nameof(TextBlock.Text),
                                        nameof(IWindow.Title),
                                        Window
                                    }
                                },
                                Foreground="#fff"
                            },
                        }
                    },
                    new StackPanel
                    {
                        Name="controlBox",
                        PresenterFor=this,
                        MarginRight=0,
                        Height = "100%",
                        Orientation= Orientation.Horizontal,
                        Children =
                        {
                            systemButtons,
                            new SystemButton
                            {
                                ToolTip="最小化",
                                Name="min",
                                Width = 30,
                                Height = "100%",
                                Content=
                                new Line
                                {
                                    MarginLeft="auto",
                                    MarginTop=5,
                                    StartPoint = new Point(1, 13),
                                    EndPoint = new Point(14, 13),
                                    StrokeStyle = "2",
                                    IsAntiAlias=true,
                                    StrokeFill=color
                                },
                                Bindings=
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(MinimizeBox),
                                        this,
                                        BindingMode.OneWay,
                                        a=>(bool)a?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Commands =
                                {
                                    {
                                        nameof(Button.Click),
                                        (s,e)=>
                                        {
                                            //(e as MouseButtonEventArgs).Handled = true;
                                            Window.WindowState = WindowState.Minimized;
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
                            ,
                            new Panel
                            {
                                Height = "100%",
                                Bindings=
                                {
                                    {
                                        nameof(Visibility),
                                        nameof(MaximizeBox),
                                        this,
                                        BindingMode.OneWay,
                                        a=>(bool)a?Visibility.Visible: Visibility.Collapsed
                                    }
                                },
                                Children=
                                {
                                    new SystemButton
                                    {
                                        ToolTip="最大化",
                                        Name="max",
                                        Width = 30,
                                        Height = "100%",
                                        Content=
                                            new Rectangle
                                            {
                                                Width=14,
                                                Height=12,
                                                MarginTop=10,
                                                StrokeStyle="2",
                                                StrokeFill = color
                                            },
                                        Commands =
                                        {
                                            {
                                                nameof(Button.Click),
                                                (s,e)=>
                                                {
                                                    //(e as MouseButtonEventArgs).Handled = true;
                                                    Window.WindowState= WindowState.Maximized;
                                                }
                                            }
                                        },
                                        Bindings =
                                        {
                                            {
                                                nameof(Border.Visibility),
                                                nameof(Window.WindowState),
                                                Window,
                                                BindingMode.OneWay,
                                                a => (WindowState)a == WindowState.Maximized||(WindowState)a == WindowState.FullScreen ? Visibility.Collapsed : Visibility.Visible
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
                                    },
                                    new SystemButton
                                    {
                                        ToolTip="向下还原",
                                        Name="nor",
                                        Width = 30,
                                        Height = "100%",
                                        Content=new Panel{
                                            Size=SizeField.Fill,
                                            Children=
                                            {
                                                new Rectangle
                                                {
                                                    MarginTop=15,
                                                    MarginLeft=8,
                                                    Width=11,
                                                    Height=8,
                                                    StrokeStyle="1.5",
                                                    StrokeFill = color
                                                },
                                                new Polyline
                                                {
                                                    MarginTop=11,
                                                    MarginLeft=12,
                                                    Points=
                                                    {
                                                        new Point(0,3),
                                                        new Point(0,0),
                                                        new Point(9,0),
                                                        new Point(9,7),
                                                        new Point(6,7)
                                                    },
                                                    StrokeFill = color,
                                                    StrokeStyle="2"
                                                }
                                            }
                                        },
                                        Commands =
                                        {
                                            {
                                                nameof(Button.Click),
                                                (s,e)=>
                                                {
                                                    //(e as MouseButtonEventArgs).Handled = true;
                                                    Window.WindowState = WindowState.Normal;
                                                }
                                            }
                                        },
                                        Bindings =
                                        {
                                            {
                                                nameof(Border.Visibility),
                                                nameof(Window.WindowState),
                                                Window,
                                                BindingMode.OneWay,
                                                a => (WindowState)a == WindowState.Normal ? Visibility.Collapsed : Visibility.Visible
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
                            },
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
            //if (Content != null)
            //{
            //    grid.Children.Add(Content, 0, 1);
            //}
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

        protected override void OnAttachedToVisualTree()
        {
            var parent = Parent;
            while (parent != null)
            {
                if (parent is IWindow window)
                {
                    this.window = window;
                    break;
                }
                parent = parent.Parent;
            }
            if (window == null)
            {
                window = (IWindow)Root;
            }
            base.OnAttachedToVisualTree();
        }

    }
    /// <summary>
    /// 通用窗体接口
    /// </summary>
    public interface IWindow
    {
        Image Icon { get; set; }
        void DragMove();
        void Close();
        WindowState WindowState { get; set; }
        string Title { get; set; }
    }
}
