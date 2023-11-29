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
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class MdiWindow : Control
    {
        public MdiWindow()
        {
            this.Init();
        }
        public WindowState WindowState { get => GetValue<WindowState>(); set => SetValue(value); }
        public UIElement Content { get => GetValue<UIElement>(); set => SetValue(value); }

        [PropertyMetadata("Title")]
        public string Title { get => GetValue<string>(); set => SetValue(value); }

        [PropertyMetadata(true)]
        public bool MaximizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }

        [PropertyMetadata(true)]
        public bool MinimizeBox { get { return GetValue<bool>(); } set { SetValue(value); } }

        [PropertyMetadata(true)]
        public bool CloseBox { get { return GetValue<bool>(); } set { SetValue(value); } }

        [UIPropertyMetadata((byte)5, UIPropertyOptions.AffectsMeasure)]
        public byte ShadowBlur { get { return GetValue<byte>(); } set { SetValue(value); } }

        [PropertyMetadata(true)]
        public bool CanResize
        {
            get => GetValue<bool>();
            set
            {
                SetValue(value);
                if (!value) this.MaximizeBox = false;
            }
        }

        public event EventHandler<ClosingEventArgs> Closing;


        void Init()
        {
            this.Focusable = true;
            var borderColor = (ViewFill)"152,180,208";
            var lostFocusBorderColor = (ViewFill)"214,227,241";
            var dragEnabled = new BindingDescribe(this, nameof(WindowState), BindingMode.OneWay, x =>
            {
                if (this.CanResize)
                {
                    return ((WindowState)x) == WindowState.Normal;
                }
                return false;
            });
            this.Size = new SizeField(500, 500);
            this.Background = null;
            this.MarginLeft = 0;
            this.MarginTop = 0;
            this.ClipToBounds = true;
            this.Children.Add(new Border
            {
                Size = SizeField.Fill,
                Background = "white",
                BorderType = BorderType.BorderStroke,
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
                                Cursor = Cursors.SizeNorthSouth,
                                Attacheds = { { Grid.ColumnIndex,1 } },
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) =>
                                {
                                    var args = e as DragDeltaEventArgs;
                                    if (this.Height.Value - args.VerticalChange > 0)
                                    {
                                        this.MarginTop += args.VerticalChange;
                                        this.Height -= args.VerticalChange;
                                    }
                                }),
                            },
                            new Thumb
                            {
                                Name = "left",
                                Size = "5,100%",
                                Cursor = Cursors.SizeWestEast,
                                Attacheds = { { Grid.RowIndex,1 },{ Grid.RowSpan,2 } },
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) =>
                                {
                                    var args = e as DragDeltaEventArgs;
                                    if (this.Width.Value - args.HorizontalChange > 0)
                                    {
                                        this.MarginLeft += args.HorizontalChange;
                                        this.Width -= args.HorizontalChange;
                                    }
                                }),
                            },
                            new Thumb
                            {
                                Name = "left_top",
                                Size = SizeField.Fill,
                                Cursor = Cursors.TopLeftCorner,
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) =>
                                {
                                    var args = e as DragDeltaEventArgs;
                                    if (this.Width.Value - args.HorizontalChange > 0 && this.Height.Value - args.VerticalChange > 0)
                                    {
                                        this.MarginLeft += args.HorizontalChange;
                                        this.MarginTop += args.VerticalChange;
                                        this.Width -= args.HorizontalChange;
                                        this.Height -= args.VerticalChange;
                                    }
                                }),
                            },
                            new Thumb
                            {
                                Name = "right",
                                Size = "5,100%",
                                Cursor = Cursors.SizeWestEast,
                                MarginRight = 0,
                                Attacheds = { { Grid.ColumnIndex,2 },{ Grid.RowIndex,1 },{ Grid.RowSpan,2 } },
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) => this.Width += (e as DragDeltaEventArgs).HorizontalChange),
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                            },
                            new Thumb
                            {
                                Name = "right_top",
                                Size = SizeField.Fill,
                                Cursor = Cursors.TopRightCorner,
                                MarginRight = 0,
                                Attacheds = { { Grid.ColumnIndex,2 } },
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) =>
                                {
                                    var args = e as DragDeltaEventArgs;
                                    if (this.Width.Value - args.HorizontalChange > 0 && this.Height.Value - args.VerticalChange > 0)
                                    {
                                        this.MarginTop += args.VerticalChange;
                                        this.Width += args.HorizontalChange;
                                        this.Height -= args.VerticalChange;
                                    }
                                }),
                            },
                            new Thumb
                            {
                                Name = "bottom",
                                Size = "100%,5",
                                Cursor = Cursors.SizeNorthSouth,
                                Attacheds = { { Grid.RowIndex,3 },{ Grid.ColumnIndex,1 } },
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) => this.Height += (e as DragDeltaEventArgs).VerticalChange),
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                            },
                            new Thumb
                            {
                                Name = "left_bottom",
                                Size = SizeField.Fill,
                                Cursor = Cursors.BottomLeftCorner,
                                Attacheds = { { Grid.RowIndex,3 } },
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) =>
                                {
                                    var args = e as DragDeltaEventArgs;
                                    if (this.Width.Value - args.HorizontalChange > 0 && this.Height.Value + args.VerticalChange > 0)
                                    {
                                        this.MarginLeft += args.HorizontalChange;
                                        this.Width -= args.HorizontalChange;
                                        this.Height += args.VerticalChange;
                                    }
                                }),
                            },
                            new Thumb
                            {
                                Name = "right_bottom",
                                Size = SizeField.Fill,
                                Cursor = Cursors.BottomRightCorner,
                                Attacheds = { { Grid.RowIndex,3 },{ Grid.ColumnIndex,2 } },
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor),
                                [nameof(IsEnabled)] = dragEnabled,
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((s,e) =>
                                {
                                    var args = e as DragDeltaEventArgs;
                                    if (this.Height.Value + args.VerticalChange > 0 && this.Width.Value + args.HorizontalChange > 0)
                                    {
                                        this.Width += args.HorizontalChange;
                                        this.Height += args.VerticalChange;
                                    }
                                }),
                            },
                            new Thumb
                            {
                                Name = "caption",
                                Attacheds = { { Grid.RowIndex,1 },{ Grid.ColumnIndex,1 } },
                                Size = SizeField.Fill,
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
                                                    [nameof(Visibility)] = new BindingDescribe(this,nameof(MinimizeBox),BindingMode.OneWay,x=>(bool)x?Visibility.Visible: Visibility.Collapsed),
                                                    [nameof(Button.Click)] = new CommandDescribe((s,e) => this.WindowState = WindowState.Minimized)
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
                                                            [nameof(Button.Click)] = new CommandDescribe((s, e) => this.WindowState = WindowState.Maximized),
                                                            [nameof(Visibility)] =
                                                                new BindingDescribe(
                                                                    this,
                                                                    nameof(WindowState),
                                                                    BindingMode.OneWay,
                                                                    x => ((WindowState)x).Or(WindowState.Maximized,WindowState.FullScreen) ? Visibility.Collapsed : Visibility.Visible),
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
                                                            [nameof(Button.Click)] = new CommandDescribe((s, e) => this.WindowState = WindowState.Normal),
                                                            [nameof(Visibility)] =
                                                                new BindingDescribe(
                                                                    this,
                                                                    nameof(WindowState),
                                                                    BindingMode.OneWay,
                                                                    x => ((WindowState)x).Or(WindowState.Normal,WindowState.Minimized)? Visibility.Collapsed : Visibility.Visible)
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
                                                    [nameof(Button.Click)] = new CommandDescribe((ss,ee) => this.Close()),
                                                    [nameof(Visibility)] = new BindingDescribe(this,nameof(this.CloseBox),BindingMode.OneWay,x=>(bool)x?Visibility.Visible: Visibility.Collapsed)
                                                }
                                            }
                                        },
                                    },
                                },
                                [nameof(Thumb.DragDelta)] = new CommandDescribe((ss,ee)=>
                                {
                                    if (this.WindowState.Or(WindowState.Normal))
                                    {
                                        var arge = ee as DragDeltaEventArgs;
                                        this.MarginLeft += arge.HorizontalChange;
                                        this.MarginTop += arge.VerticalChange;
                                    }
                                }),
                                [nameof(DoubleClick)] = new CommandDescribe((ss,ee) => this.Delay(TimeSpan.FromMilliseconds(150),()=>
                                {
                                    if(!this.MaximizeBox) return;
                                    if (this.WindowState.Or(WindowState.Maximized,WindowState.Minimized))
                                    {
                                        this.WindowState = WindowState.Normal;
                                    }
                                    else if (this.WindowState == WindowState.Normal)
                                    {
                                        this.WindowState = WindowState.Maximized;
                                    }
                                })),
                                [nameof(Background)] = new BindingDescribe(this,nameof(IsFocused),BindingMode.OneWay ,x => ((bool)x) ? borderColor : lostFocusBorderColor)
                            },
                            new Decorator
                            {
                                Attacheds = { { Grid.RowIndex,2 } ,{ Grid.ColumnIndex,1 } },
                                Size = SizeField.Fill,
                                [nameof(Decorator.Child)] = new BindingDescribe(this,nameof(Content))
                            },
                        }
                    },
                },
                [nameof(ShadowBlur)] = new BindingDescribe(this, nameof(WindowState), BindingMode.OneWay, x => ((WindowState)x).Or(WindowState.Maximized, WindowState.FullScreen) ? 0 : ShadowBlur),
                [nameof(ShadowBlur)] = new BindingDescribe(this, nameof(IsFocused), BindingMode.OneWay, x => ((bool)x) ? ShadowBlur : 0),
                [nameof(BorderStroke)] = new BindingDescribe(this, nameof(IsFocused), BindingMode.OneWay, x => ((bool)x) ? "0" : "1"),
                [nameof(BorderFill)] = new BindingDescribe(this, nameof(IsFocused), BindingMode.OneWay, x => ((bool)x) ? null : "0,0,0,100"),
            });
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            if (propertyName == nameof(DataContext) && newValue != null)
            {
                if (newValue is IClosable closable)
                {
                    closable.Closable -= Closable_Closable;
                    closable.Closable += Closable_Closable;
                }
                if (newValue is ILoading loading)
                {
                    loading.CreateLoading(this);
                }
            }
            else if (propertyName == nameof(Content) && newValue != null)
            {
                this.Content.Margin = "0";
                this.Content.ClipToBounds = true;
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        }

        private void Closable_Closable(object sender, ClosingEventArgs e)
        {
            this.DoClose(sender, e);
        }

        public void Close()
        {
            if (this.Closing != null)
            {
                this.DoClose(this, new ClosingEventArgs());
            }
            else
            {
                this.Dispose();
            }
        }

        void DoClose(object sender, ClosingEventArgs e)
        {
            if (this.DataContext is IClosable closable)
            {
                closable.OnClosable(sender, e);
            }
            this.Closing.Invoke(sender, e);
        }
    }
}
