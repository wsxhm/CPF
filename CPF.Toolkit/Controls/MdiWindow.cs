using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class MdiWindow : Control
    {
        [PropertyMetadata(typeof(WindowState), "0")]
        public WindowState WindowState { get => GetValue<WindowState>(); set => SetValue(value); }
        [PropertyMetadata("title")]
        public string Title { get => GetValue<string>(); set => SetValue(value); }
        public UIElement Content { get => GetValue<UIElement>(); set => SetValue(value); }
        [PropertyMetadata(true)]
        public bool MaximizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(true)]
        public bool MinimizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        [PropertyMetadata(true)]
        public bool CloseBox { get { return GetValue<bool>(); } set { SetValue(value); } }
        [UIPropertyMetadata((byte)5, UIPropertyOptions.AffectsMeasure)]
        public byte ShadowBlur { get { return GetValue<byte>(); } set { SetValue(value); } }

        public event EventHandler<ClosingEventArgs> Closing;

        WindowState oldState;
        SizeField normalSize = new SizeField(500, 500);
        Point normalPos = new Point(0, 0);
        protected override void InitializeComponent()
        {
            var bar = (ViewFill)"154,180,208";
            var thubmEnabled = new BindingDescribe(this, nameof(WindowState), BindingMode.OneWay, b => ((WindowState)b) == WindowState.Normal);
            this.Size = normalSize;
            this.Background = null;
            this.MarginLeft = 0;
            this.MarginTop = 0;
            this.MinWidth = 200;
            this.MinHeight = 70;
            this.ClipToBounds = true;
            this.Children.Add(new Border
            {
                Size = SizeField.Fill,
                Background = "#fff",
                BorderType = BorderType.BorderStroke,
                BorderStroke = new Stroke(0),
                ShadowBlur = ShadowBlur,
                ShadowColor = Color.FromRgba(0, 0, 0, 150),
                Child = new Decorator
                {
                    Size = SizeField.Fill,
                    ClipToBounds = true,
                    Child = new Grid
                    {
                        Size = SizeField.Fill,
                        ColumnDefinitions =
                        {
                            new ColumnDefinition{ Width = "auto" },
                            new ColumnDefinition{ },
                            new ColumnDefinition{ Width = "auto" },
                        },
                        RowDefinitions =
                        {
                            new RowDefinition{ Height = "auto" },
                            new RowDefinition{ Height = 30 },
                            new RowDefinition{  },
                            new RowDefinition{ Height = "auto" },
                        },
                        Children =
                        {
                            new Thumb
                            {
                                Name = "top",
                                Size = "100%,5",
                                Background = bar,
                                Cursor = Cursors.SizeNorthSouth,
                                Attacheds = { { Grid.ColumnSpan,3 } },
                                [nameof(IsEnabled)] = thubmEnabled,
                                Commands =
                                {
                                    {
                                        nameof(Thumb.DragDelta),(s,e) =>
                                        {
                                            var args = e as DragDeltaEventArgs;
                                            if (this.Height.Value - args.VerticalChange > 0)
                                            {
                                                this.MarginTop += args.VerticalChange;
                                                this.Height -= args.VerticalChange;
                                            }
                                        }
                                    }
                                },
                            },
                            new Thumb
                            {
                                Name = "left",
                                Size = "5,100%",
                                Background = bar,
                                Cursor = Cursors.SizeWestEast,
                                IsEnabled = false,
                                Attacheds = { { Grid.ColumnIndex,0 } ,{ Grid.RowSpan,4 } },
                                [nameof(IsEnabled)] = thubmEnabled,
                                Commands =
                                {
                                    {
                                        nameof(Thumb.DragDelta),(s,e) =>
                                        {
                                            var args = e as DragDeltaEventArgs;
                                            if (this.Width.Value - args.HorizontalChange > 0)
                                            {
                                                this.MarginLeft += args.HorizontalChange;
                                                this.Width -= args.HorizontalChange;
                                            }
                                        }
                                    }
                                }
                            },
                            new Thumb
                            {
                                Name = "right",
                                Size = "5,100%",
                                Background = bar,
                                Cursor = Cursors.SizeWestEast,
                                MarginRight = 0,
                                Attacheds = { { Grid.ColumnIndex,2 },{ Grid.RowSpan,4 } },
                                [nameof(IsEnabled)] = thubmEnabled,
                                Commands = { { nameof(Thumb.DragDelta),(s,e) => this.Width += (e as DragDeltaEventArgs).HorizontalChange } }
                            },
                            new Thumb
                            {
                                Name = "bottom",
                                Size = "100%,5",
                                Background = bar,
                                Cursor = Cursors.SizeNorthSouth,
                                Attacheds = { { Grid.RowIndex,3 },{ Grid.ColumnSpan,3 } },
                                [nameof(IsEnabled)] = thubmEnabled,
                                Commands = { { nameof(Thumb.DragDelta),(s,e) => this.Height += (e as DragDeltaEventArgs).VerticalChange } }
                            },
                            new Thumb
                            {
                                Name = "caption",
                                Attacheds = { { Grid.RowIndex,1 },{ Grid.ColumnIndex,1 } },
                                Size = SizeField.Fill,
                                Background = bar,
                                Child = new Panel
                                {
                                    Size = SizeField.Fill,
                                    Children =
                                    {
                                        new StackPanel
                                        {
                                            Orientation = Orientation.Horizontal,
                                            MarginLeft = 0,
                                            Children =
                                            {
                                                new TextBlock
                                                {
                                                    [nameof(TextBlock.Text)] = new BindingDescribe(this,nameof(this.Title),BindingMode.OneWay),
                                                    FontSize = 14,
                                                    MarginLeft = 10,
                                                },
                                            }
                                        },
                                        new StackPanel
                                        {
                                            Orientation = Orientation.Horizontal,
                                            MarginRight = 0,
                                            Height = "100%",
                                            Children =
                                            {
                                                new SystemButton
                                                {
                                                    Name = "min",
                                                    Size = new SizeField(30,"100%"),
                                                    Content = new Line
                                                    {
                                                        MarginLeft = "auto",
                                                        StartPoint = new Point(1,13),
                                                        EndPoint = new Point(14,13),
                                                        StrokeStyle = "2",
                                                        IsAntiAlias = true,
                                                        StrokeFill = "black"
                                                    },
                                                    [nameof(Visibility)] = new BindingDescribe(this,nameof(MinimizeBox),BindingMode.OneWay,a=>(bool)a?Visibility.Visible: Visibility.Collapsed),
                                                    Commands = { { nameof(Button.Click),(s,e) => this.WindowState = WindowState.Minimized } },
                                                },
                                                new Panel
                                                {
                                                    Height = "100%",
                                                    [nameof(Visibility)] = new BindingDescribe(this,nameof(MaximizeBox),BindingMode.OneWay,a => (bool)a ? Visibility.Visible : Visibility.Collapsed),
                                                    Children =
                                                    {
                                                        new SystemButton
                                                        {
                                                            Name = "max",
                                                            Size = new SizeField(30,"100%"),
                                                            Content = new Rectangle
                                                            {
                                                                Size = new SizeField(14,12),
                                                                MarginTop = 5,
                                                                StrokeStyle = "2",
                                                            },
                                                            Commands = { { nameof(Button.Click),(s,e) => this.WindowState = WindowState.Maximized } },
                                                            [nameof(Visibility)] = new BindingDescribe(this,
                                                                                                        nameof(WindowState),
                                                                                                        BindingMode.OneWay,
                                                                                                        a => ((WindowState)a).Or(WindowState.Maximized,WindowState.FullScreen)
                                                                                                        ? Visibility.Collapsed : Visibility.Visible),
                                                        },
                                                        new SystemButton
                                                        {
                                                            Name = "nor",
                                                            Visibility = Visibility.Collapsed,
                                                            Size = new SizeField(30,"100%"),
                                                            Content = new Panel
                                                            {
                                                                Size = SizeField.Fill,
                                                                Children =
                                                                {
                                                                    new Rectangle
                                                                    {
                                                                        MarginTop = 10,
                                                                        MarginLeft =8,
                                                                        Size = new SizeField(11,8),
                                                                        StrokeStyle = "1.5",
                                                                    },
                                                                    new Polyline
                                                                    {
                                                                        MarginTop =5,
                                                                        MarginLeft = 12,
                                                                        Points =
                                                                        {
                                                                            new Point(0,3),
                                                                            new Point(0,0),
                                                                            new Point(9,0),
                                                                            new Point(9,7),
                                                                            new Point(6,7)
                                                                        },
                                                                        StrokeStyle = "2"
                                                                    }
                                                                }
                                                            },
                                                            Commands = { { nameof(Button.Click),(s,e) => this.WindowState = WindowState.Normal } },
                                                            [nameof(Visibility)] = new BindingDescribe(this,
                                                                                                        nameof(WindowState),
                                                                                                        BindingMode.OneWay,
                                                                                                        a => ((WindowState)a).Or( WindowState.Normal, WindowState.Minimized)
                                                                                                        ? Visibility.Collapsed :
                                                                                                        Visibility.Visible)
                                                        }
                                                    }
                                                },
                                                new SystemButton
                                                {
                                                    Name = "close",
                                                    Size = new SizeField(30,"100%"),
                                                    Content = new Panel
                                                    {
                                                        Size = SizeField.Fill,
                                                        Children =
                                                        {
                                                            new Line
                                                            {
                                                                MarginTop=4,
                                                                MarginLeft=8,
                                                                StartPoint = new Point(1, 1),
                                                                EndPoint = new Point(14, 13),
                                                                StrokeStyle = "2",
                                                                IsAntiAlias=true,
                                                            },
                                                            new Line
                                                            {
                                                                MarginTop=4,
                                                                MarginLeft=8,
                                                                StartPoint = new Point(14, 1),
                                                                EndPoint = new Point(1, 13),
                                                                StrokeStyle = "2",
                                                                IsAntiAlias=true,
                                                            }
                                                        }
                                                    },
                                                    Commands =
                                                    {
                                                        {
                                                            nameof(Button.Click),(ss,ee) =>
                                                            {
                                                                var e = new ClosingEventArgs();
                                                                this.Closing?.Invoke(this,e);
                                                                if (!e.Cancel)
                                                                {
                                                                    this.Visibility = Visibility.Collapsed;
                                                                    this.Dispose();
                                                                }
                                                            }
                                                        }
                                                    },
                                                    [nameof(Visibility)] = new BindingDescribe(this,nameof(this.CloseBox),BindingMode.OneWay,a=>(bool)a?Visibility.Visible: Visibility.Collapsed)
                                                }
                                            }
                                        },
                                    },
                                },
                                Commands =
                                {
                                    {
                                        nameof(Thumb.DragDelta),
                                        (s,e) =>
                                        {
                                            if (this.WindowState.Or(WindowState.Normal))
                                            {
                                                var arge = e as DragDeltaEventArgs;
                                                this.MarginLeft += arge.HorizontalChange;
                                                this.MarginTop += arge.VerticalChange;
                                                this.normalPos = new Point(this.MarginLeft.Value, this.MarginTop.Value);
                                            }
                                        }
                                    },
                                    {
                                        nameof(DoubleClick),
                                        (s,e) => this.Delay(TimeSpan.FromMilliseconds(150),() =>
                                        {
                                            if (this.WindowState.Or(WindowState.Maximized,WindowState.Minimized))
                                            {
                                                this.WindowState = WindowState.Normal;
                                            }
                                            else if (this.WindowState == WindowState.Normal)
                                            {
                                                this.WindowState = WindowState.Maximized;
                                            }
                                        })
                                    }
                                },
                            },
                            new Decorator
                            {
                                Attacheds = { { Grid.RowIndex,2 } ,{ Grid.ColumnIndex,1 } },
                                Size = SizeField.Fill,
                                Child = this.Content,
                            },
                        }
                    },
                },
                [nameof(Border.ShadowBlur)] = new BindingDescribe(this,
                                                                  nameof(WindowState),
                                                                  BindingMode.OneWay,
                                                                  a => ((WindowState)a).Or(WindowState.Maximized, WindowState.FullScreen,WindowState.Minimized) ? 0 : ShadowBlur),
            });

            this.Content.Margin = "0";
            this.Content.ClipToBounds = true;
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            switch (propertyName)
            {
                case nameof(WindowState):
                    {
                        switch (this.WindowState)
                        {
                            case WindowState.Normal:
                                this.Size = this.normalSize;
                                this.MarginLeft = this.normalPos.X;
                                this.MarginTop = this.normalPos.Y;
                                break;
                            case WindowState.Minimized:
                                this.Visibility = Visibility.Collapsed;
                                this.oldState = (WindowState)oldValue;
                                break;
                            case WindowState.Maximized:
                            case WindowState.FullScreen:
                                this.Size = SizeField.Fill;
                                this.MarginLeft = 0;
                                this.MarginTop = 0;
                                break;
                        }
                    }
                    break;

                case nameof(Size):
                case nameof(Width):
                case nameof(Height):
                case nameof(ActualSize):
                    switch (this.WindowState)
                    {
                        case WindowState.Normal:
                            this.normalSize = this.Size;
                            break;
                        case WindowState.Minimized:
                            //this.Width = this.MinWidth;
                            //this.Height = this.MinHeight;
                            break;
                        case WindowState.Maximized:
                        case WindowState.FullScreen:
                            break;
                    }
                    break;

                case nameof(MarginLeft):
                    {
                        if (this.MarginLeft.Value <= 0)
                        {
                            this.MarginLeft = 0;
                        }
                    }
                    break;
                case nameof(MarginTop):
                    if (MarginTop.Value <= 0)
                    {
                        this.MarginTop = 0;
                    }
                    break;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        }

        public override string ToString()
        {
            return this.Title;
        }

        public void ReWindowState()
        {
            this.WindowState = this.oldState;
        }
    }
}
