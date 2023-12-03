using System;
using System.Collections.Generic;
using System.Text;
using CPF.Input;
using CPF.Drawing;
using CPF.Styling;
using System.Linq;
using CPF.Shapes;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个控件，该控件可让用户通过沿 Thumb 移动 Track 控件从一个值范围中进行选择。
    /// </summary>
    [Description("表示一个控件，该控件可让用户通过沿 Thumb 移动 Track 控件从一个值范围中进行选择。"), Browsable(true)]
    public class Slider : RangeBase
    {
        public Slider()
        {
            DecreaseLargeChanged = () =>
            {
                Value -= LargeChange;
            };
            IncreaseLargeChanged = () =>
            {
                Value += LargeChange;
            };
            DecreaseSmallChange = () => { Value -= SmallChange; };
            IncreaseSmallChange = () => { Value += SmallChange; };
        }


        protected override void InitializeComponent()
        {
            //Background = "#f00";
            Children.Add(new TickBar
            {
                PresenterFor = this,
                Name = "TopTick",
                Height = 8,
                MarginLeft = 5,
                MarginRight = 5,
                MarginTop = 0,
                Fill = "229,229,229",
                Bindings =
                {
                    { nameof(TickBar.Maximum),nameof(Maximum),this},
                    { nameof(TickBar.Minimum),nameof(Minimum),this},
                    { nameof(TickBar.IsDirectionReversed),nameof(IsDirectionReversed),this},
                    { nameof(TickBar.TickFrequency),nameof(TickFrequency),this},
                    { nameof(TickBar.Ticks),nameof(Ticks),this},
                    { nameof(TickBar.Visibility),nameof(TickPlacement),this,BindingMode.OneWay,a=>(TickPlacement)a== TickPlacement.Both||(TickPlacement)a== TickPlacement.TopLeft?Visibility.Visible:Visibility.Collapsed}
                }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(Orientation),
                PropertyConditions = a => (Orientation)a == Orientation.Vertical,
                TargetRelation = Relation.Me.Children(a => a.Name == "TopTick" && a.PresenterFor == this),
                Setters =
                {
                    {nameof(MarginBottom),5 },
                    {nameof(MarginTop),5 },
                    {nameof(MarginLeft),0 },
                    {nameof(MarginRight),FloatField.Auto },
                    {nameof(Height),FloatField.Auto },
                    {nameof(Width),8 },
                    {nameof(TickBar.Placement),TickBarPlacement.Left },
                }
            });
            Children.Add(new TickBar
            {
                PresenterFor = this,
                Name = "BottomTick",
                Height = 8,
                MarginLeft = 5,
                MarginRight = 5,
                MarginBottom = 0,
                Fill = "229,229,229",
                Placement = TickBarPlacement.Bottom,
                Bindings =
                {
                    { nameof(TickBar.Maximum),nameof(Maximum),this},
                    { nameof(TickBar.Minimum),nameof(Minimum),this},
                    { nameof(TickBar.IsDirectionReversed),nameof(IsDirectionReversed),this},
                    { nameof(TickBar.TickFrequency),nameof(TickFrequency),this},
                    { nameof(TickBar.Ticks),nameof(Ticks),this},
                    { nameof(TickBar.Visibility),nameof(TickPlacement),this,BindingMode.OneWay,a=>(TickPlacement)a== TickPlacement.Both||(TickPlacement)a== TickPlacement.BottomRight?Visibility.Visible:Visibility.Collapsed}
                }
            });

            Triggers.Add(new Trigger
            {
                Property = nameof(Orientation),
                PropertyConditions = a => (Orientation)a == Orientation.Vertical,
                TargetRelation = Relation.Me.Children(a => a.Name == "BottomTick" && a.PresenterFor == this),
                Setters =
                {
                    {nameof(MarginBottom),5 },
                    {nameof(MarginTop),5 },
                    {nameof(MarginLeft),FloatField.Auto },
                    {nameof(MarginRight),0 },
                    {nameof(Height),FloatField.Auto },
                    {nameof(Width),8 },
                    {nameof(TickBar.Placement),TickBarPlacement.Right },
                }
            });
            Children.Add(new Border
            {
                Name = "TrackBackground",
                PresenterFor = this,
                MarginRight = 5,
                MarginLeft = 5,
                Height = 4,
                BorderFill = "187,187,187",
                Background = "231,234,234",
            });

            Triggers.Add(new Trigger
            {
                Property = nameof(Orientation),
                PropertyConditions = a => (Orientation)a == Orientation.Vertical,
                TargetRelation = Relation.Me.Children(a => a.Name == "TrackBackground" && a.PresenterFor == this),
                Setters =
                {
                    {nameof(MarginBottom),5 },
                    {nameof(MarginTop),5 },
                    {nameof(MarginLeft),FloatField.Auto },
                    {nameof(MarginRight),FloatField.Auto },
                    {nameof(Height),FloatField.Auto },
                    {nameof(Width),4 },
                }
            });
            Children.Add(new Track
            {
                PresenterFor = this,
                Name = "PART_Track",
                Height = "100%",
                Width = "100%",
                IsDirectionReversed = true,
                Thumb = new Thumb
                {
                    Width = "100%",
                    Height = "10",
                    BorderFill = "187,187,187",
                    BorderStroke = new Stroke(1),
                    Bindings =
                    {
                        { nameof(Width),nameof(Orientation),this,BindingMode.OneWay,a=>((Orientation)a)==Orientation.Horizontal?(FloatField)10:16 },
                        { nameof(Height),nameof(Orientation),this,BindingMode.OneWay,a=>((Orientation)a)==Orientation.Horizontal?(FloatField)16:10 },
                    },
                    Triggers =
                    {
                        new Trigger { Property = nameof(IsMouseOver), Setters = { { "Background", "190,230,253" } } },
                        new Trigger { Property = nameof(Thumb.IsDragging), Setters = { { "Background", "186,209,226" } } }
                    },
                    Commands =
                    {
                        { nameof(Thumb.DragStarted), nameof(OnThumbDragStarted), this,CommandParameter.EventSender, CommandParameter.EventArgs },
                        { nameof(Thumb.DragCompleted), nameof(OnThumbDragCompleted), this,CommandParameter.EventSender, CommandParameter.EventArgs },
                        { nameof(Thumb.DragDelta), nameof(OnThumbDragDelta), this,CommandParameter.EventSender, CommandParameter.EventArgs },
                    }
                },
                IncreaseRepeatButton = new RepeatButton
                {
                    Name = "increaseRepeatButton",
                    Width = "100%",
                    Height = "100%",
                    Commands = { { nameof(RepeatButton.Click), nameof(CommandIncreaseLargeChanged), this } },
                    Bindings =
                    {
                        {nameof(RepeatButton.Delay),nameof(Delay),this },
                        {nameof(RepeatButton.Interval),nameof(Interval),this },
                    }
                },
                DecreaseRepeatButton = new RepeatButton
                {
                    Name = "decreaseRepeatButton",
                    Width = "100%",
                    Height = "100%",
                    Commands = { { nameof(RepeatButton.Click), nameof(CommandDecreaseLargeChanged), this } },
                    Bindings =
                    {
                        {nameof(RepeatButton.Delay),nameof(Delay),this },
                        {nameof(RepeatButton.Interval),nameof(Interval),this },
                    }
                },
                Bindings =
                {
                    { nameof(Track.Orientation), nameof(Orientation),1 },
                    { nameof(Track.Maximum),nameof(Maximum),1},
                    { nameof(Track.Minimum),nameof(Minimum),1},
                    { nameof(Track.Value),nameof(Value),1,BindingMode.TwoWay},
                    { nameof(Track.IsDirectionReversed),nameof(IsDirectionReversed),1},
                }
            });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Track = FindPresenter<Track>().FirstOrDefault(a => a.Name == "PART_Track");
        }



        /// <summary>
        /// 减
        /// </summary>
        public Action DecreaseLargeChanged;

        protected void CommandDecreaseLargeChanged()
        {
            DecreaseLargeChanged?.Invoke();
        }
        /// <summary>
        /// 加
        /// </summary>
        public Action IncreaseLargeChanged;
        protected void CommandIncreaseLargeChanged()
        {
            IncreaseLargeChanged?.Invoke();
        }
        /// <summary>
        /// 减
        /// </summary>
        public Action DecreaseSmallChange;
        protected void CommandDecreaseSmallChange()
        {
            DecreaseSmallChange?.Invoke();
        }
        /// <summary>
        /// 加
        /// </summary>
        public Action IncreaseSmallChange;
        protected void CommandIncreaseSmallChange()
        {
            IncreaseSmallChange?.Invoke();
        }
        /// <summary>
        /// 布局方向
        /// </summary>
        [Description("布局方向")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置与 Track 的 Slider 相关的刻度线的位置。
        /// </summary>
        [PropertyMetadata(TickPlacement.None)]
        [Description("获取或设置与 Track 的 Slider 相关的刻度线的位置。")]
        public TickPlacement TickPlacement
        {
            get
            {
                return (TickPlacement)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// 如果增加值的方向向左（对于水平滑块）或向下（对于垂直滑块），则为 true；否则为 false。 默认值为 false。
        /// </summary>
        [PropertyMetadata(false)]
        [Description("如果增加值的方向向左（对于水平滑块）或向下（对于垂直滑块），则为 true；否则为 false。 默认值为 false。")]
        public bool IsDirectionReversed
        {
            get
            {
                return (bool)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 获取或设置在按下 RepeatButton 之后等待执行用于移动 Thumb 的命令（如 DecreaseLarge 命令）的时间（以毫秒为单位）。
        /// </summary>
        [PropertyMetadata(250)]
        [Description("获取或设置在按下 RepeatButton 之后等待执行用于移动 Thumb 的命令（如 DecreaseLarge 命令）的时间（以毫秒为单位）。")]
        public int Delay
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 获取或设置当用户单击 RepeatButton 的 Slider 时增加或减少命令之间的时间量（以毫秒为单位）
        /// </summary>
        [PropertyMetadata(100)]
        [Description("获取或设置当用户单击 RepeatButton 的 Slider 时增加或减少命令之间的时间量（以毫秒为单位）")]
        public int Interval
        {
            get
            {
                return (int)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示 Slider 是否自动将 Thumb 移动到最近的刻度线
        /// </summary>
        [PropertyMetadata(true)]
        [Description("获取或设置一个值，该值指示 Slider 是否自动将 Thumb 移动到最近的刻度线")]
        public bool IsSnapToTickEnabled
        {
            get
            {
                return (bool)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 刻度线之间的距离。 默认值为 (1.0)。
        /// </summary>
        [PropertyMetadata(1f)]
        [Description(" 刻度线之间的距离。 默认值为 (1.0)。")]
        public float TickFrequency
        {
            get
            {
                return (float)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 获取或设置刻度线的位置。
        /// </summary>
        [Description("获取或设置刻度线的位置。")]
        public Collection<float> Ticks
        {
            get
            {
                return (Collection<float>)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否立即将 Slider 的 Thumb 移动到在鼠标指针悬停在 Slider 轨道的上方时鼠标单击的位置。
        /// </summary>
        //[PropertyMetadata(true)]
        [Description(" 获取或设置一个值，该值指示是否立即将 Slider 的 Thumb 移动到在鼠标指针悬停在 Slider 轨道的上方时鼠标单击的位置。")]
        public bool IsMoveToPointEnabled
        {
            get
            {
                return (bool)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }


        /// <summary>
        /// Gets or sets reference to Slider's Track element.
        /// </summary>
        [NotCpfProperty]
        internal Track Track
        {
            get;
            set;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (IsMoveToPointEnabled && Track != null && Track.Thumb != null && !Track.Thumb.IsMouseOver)
            {
                // Move Thumb to the Mouse location

                Point pt = MouseDevice.GetPosition(Track);
                var newValue = Track.ValueFromPoint(pt);
                if (FloatUtil.IsFloatFinite(newValue))
                {
                    UpdateValue(newValue);
                }
                e.Handled = true;
            }
            base.OnPreviewMouseDown(e);
        }


        protected virtual void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //// Show AutoToolTip if needed.
            //Thumb thumb = e.OriginalSource as Thumb;

            //if ((thumb == null) || (this.AutoToolTipPlacement == Primitives.AutoToolTipPlacement.None))
            //{
            //    return;
            //}

            //// Save original tooltip
            //_thumbOriginalToolTip = thumb.ToolTip;

            //if (_autoToolTip == null)
            //{
            //    _autoToolTip = new ToolTip();
            //    _autoToolTip.Placement = PlacementMode.Custom;
            //    _autoToolTip.PlacementTarget = thumb;
            //    _autoToolTip.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(this.AutoToolTipCustomPlacementCallback);
            //}

            //thumb.ToolTip = _autoToolTip;
            //_autoToolTip.Content = GetAutoToolTipNumber();
            //_autoToolTip.IsOpen = true;
            //((Popup)_autoToolTip.Parent).Reposition();
        }

        /// <summary>
        /// Called when user dragging the Thumb.
        /// This function can be override to customize the way Slider handles Thumb movement.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.OriginalSource as Thumb;
            // Convert to Track's co-ordinate
            if (Track != null && thumb == Track.Thumb)
            {

                float newValue = (float)Value + Track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);
                if (FloatUtil.IsFloatFinite(newValue))
                {
                    UpdateValue(newValue);
                }

                //// Show AutoToolTip if needed
                //if (this.AutoToolTipPlacement != Primitives.AutoToolTipPlacement.None)
                //{
                //    if (_autoToolTip == null)
                //    {
                //        _autoToolTip = new ToolTip();
                //    }

                //    _autoToolTip.Content = GetAutoToolTipNumber();

                //    if (thumb.ToolTip != _autoToolTip)
                //    {
                //        thumb.ToolTip = _autoToolTip;
                //    }

                //    if (!_autoToolTip.IsOpen)
                //    {
                //        _autoToolTip.IsOpen = true;
                //    }
                //    ((Popup)_autoToolTip.Parent).Reposition();
                //}
            }
        }

        protected virtual void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //// Show AutoToolTip if needed.
            //Thumb thumb = e.OriginalSource as Thumb;

            //if ((thumb == null) || (this.AutoToolTipPlacement == Primitives.AutoToolTipPlacement.None))
            //{
            //    return;
            //}

            //if (_autoToolTip != null)
            //{
            //    _autoToolTip.IsOpen = false;
            //}

            //thumb.ToolTip = _thumbOriginalToolTip;
        }

        /// <summary>
        /// Helper function for value update.
        /// This function will also snap the value to tick, if IsSnapToTickEnabled is true.
        /// </summary>
        /// <param name="value"></param>
        private void UpdateValue(float value)
        {
            float snappedValue = SnapToTick(value);

            if (snappedValue != Value)
            {
                Value = Math.Max(this.Minimum, Math.Min(this.Maximum, snappedValue));
            }
        }

        /// <summary>
        /// Snap the input 'value' to the closest tick.
        /// If input value is exactly in the middle of 2 surrounding ticks, it will be snapped to the tick that has greater value.
        /// </summary>
        /// <param name="value">Value that want to snap to closest Tick.</param>
        /// <returns>Snapped value if IsSnapToTickEnabled is 'true'. Otherwise, returns un-snaped value.</returns>
        private float SnapToTick(float value)
        {
            if (IsSnapToTickEnabled)
            {
                var previous = Minimum;
                var next = Maximum;

                // This property is rarely set so let's try to avoid the GetValue
                // caching of the mutable default value
                var ticks = Ticks;
                //bool hasModifiers;
                //if (GetValueSource(TicksProperty, null, out hasModifiers)
                //    != BaseValueSourceInternal.Default || hasModifiers)
                //{
                //    ticks = Ticks;
                //}

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int i = 0; i < ticks.Count; i++)
                    {
                        var tick = ticks[i];
                        if (FloatUtil.AreClose(tick, value))
                        {
                            return value;
                        }

                        if (FloatUtil.LessThan(tick, value) && FloatUtil.GreaterThan(tick, (float)previous))
                        {
                            previous = tick;
                        }
                        else if (FloatUtil.GreaterThan(tick, value) && FloatUtil.LessThan(tick, (float)next))
                        {
                            next = tick;
                        }
                    }
                }
                else if (FloatUtil.GreaterThan(TickFrequency, 0f))
                {
                    previous = (float)(Minimum + (Math.Round(((value - Minimum) / TickFrequency)) * TickFrequency));
                    next = Math.Min(Maximum, previous + TickFrequency);
                }

                // Choose the closest value between previous and next. If tie, snap to 'next'.
                value = FloatUtil.GreaterThanOrClose((float)value, (float)(previous + next) * 0.5f) ? (float)next : (float)previous;
            }

            return value;
        }

        // Sets Value = SnapToTick(value+direction), unless the result of SnapToTick is Value,
        // then it searches for the next tick greater(if direction is positive) than value
        // and sets Value to that tick
        private void MoveToNextTick(float direction)
        {
            if (direction != 0.0)
            {
                var value = this.Value;

                // Find the next value by snapping
                var next = SnapToTick(Math.Max((float)this.Minimum, Math.Min((float)this.Maximum, (float)value + direction)));

                bool greaterThan = direction > 0; //search for the next tick greater than value?

                // If the snapping brought us back to value, find the next tick point
                if (next == value
                    && !(greaterThan && value == Maximum)  // Stop if searching up if already at Max
                    && !(!greaterThan && value == Minimum)) // Stop if searching down if already at Min
                {
                    // This property is rarely set so let's try to avoid the GetValue
                    // caching of the mutable default value
                    //DoubleCollection ticks = null;
                    //bool hasModifiers;
                    //if (GetValueSource(TicksProperty, null, out hasModifiers)
                    //    != BaseValueSourceInternal.Default || hasModifiers)
                    //{
                    var ticks = Ticks;
                    //}

                    // If ticks collection is available, use it.
                    // Note that ticks may be unsorted.
                    if ((ticks != null) && (ticks.Count > 0))
                    {
                        for (int i = 0; i < ticks.Count; i++)
                        {
                            var tick = ticks[i];

                            // Find the smallest tick greater than value or the largest tick less than value
                            if ((greaterThan && FloatUtil.GreaterThan(tick, (float)value) && (FloatUtil.LessThan(tick, next) || next == value))
                             || (!greaterThan && FloatUtil.LessThan(tick, (float)value) && (FloatUtil.GreaterThan(tick, next) || next == value)))
                            {
                                next = tick;
                            }
                        }
                    }
                    else if (FloatUtil.GreaterThan(TickFrequency, 0f))
                    {
                        // Find the current tick we are at
                        var tickNumber = (float)Math.Round((value - Minimum) / TickFrequency);

                        if (greaterThan)
                            tickNumber += 1f;
                        else
                            tickNumber -= 1f;

                        next = (float)Minimum + tickNumber * TickFrequency;
                    }
                }


                // Update if we've found a better value
                if (next != value)
                {
                    this.Value = next;
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                e.Handled = Focus() || e.Handled;
            }
            base.OnMouseDown(e);
        }


        /// <summary>
        /// Perform arrangement of slider's children
        /// </summary>
        /// <param name="finalSize"></param>
        protected override Size ArrangeOverride(in Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);

            UpdateSelectionRangeElementPositionAndSize();

            return size;
        }
        [PropertyChanged(nameof(Value))]
        void OnValueChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            UpdateSelectionRangeElementPositionAndSize();
        }

        /// <summary>
        /// Resize and resposition the SelectionRangeElement.
        /// </summary>
        private void UpdateSelectionRangeElementPositionAndSize()
        {
            //Size trackSize = new Size(0f, 0f);
            //Size thumbSize = new Size(0f, 0f);

            //if (Track == null || FloatField.LessThan(SelectionEnd, SelectionStart))
            //{
            //    return;
            //}

            //trackSize = Track.ActualSize;
            //thumbSize = (Track.Thumb != null) ? Track.Thumb.ActualSize : new Size(0f, 0f);

            //float range = Maximum - Minimum;
            //float valueToSize;

            //FrameworkElement rangeElement = this.SelectionRangeElement as FrameworkElement;

            //if (rangeElement == null)
            //{
            //    return;
            //}

            //if (Orientation == Orientation.Horizontal)
            //{
            //    // Calculate part size for HorizontalSlider
            //    if (DoubleUtil.AreClose(range, 0d) || (DoubleUtil.AreClose(trackSize.Width, thumbSize.Width)))
            //    {
            //        valueToSize = 0d;
            //    }
            //    else
            //    {
            //        valueToSize = Math.Max(0.0, (trackSize.Width - thumbSize.Width) / range);
            //    }

            //    rangeElement.Width = ((SelectionEnd - SelectionStart) * valueToSize);
            //    if (IsDirectionReversed)
            //    {
            //        Canvas.SetLeft(rangeElement, (thumbSize.Width * 0.5) + Math.Max(Maximum - SelectionEnd, 0) * valueToSize);
            //    }
            //    else
            //    {
            //        Canvas.SetLeft(rangeElement, (thumbSize.Width * 0.5) + Math.Max(SelectionStart - Minimum, 0) * valueToSize);
            //    }
            //}
            //else
            //{
            //    // Calculate part size for VerticalSlider
            //    if (DoubleUtil.AreClose(range, 0d) || (DoubleUtil.AreClose(trackSize.Height, thumbSize.Height)))
            //    {
            //        valueToSize = 0d;
            //    }
            //    else
            //    {
            //        valueToSize = Math.Max(0.0, (trackSize.Height - thumbSize.Height) / range);
            //    }

            //    rangeElement.Height = ((SelectionEnd - SelectionStart) * valueToSize);
            //    if (IsDirectionReversed)
            //    {
            //        Canvas.SetTop(rangeElement, (thumbSize.Height * 0.5) + Math.Max(SelectionStart - Minimum, 0) * valueToSize);
            //    }
            //    else
            //    {
            //        Canvas.SetTop(rangeElement, (thumbSize.Height * 0.5) + Math.Max(Maximum - SelectionEnd, 0) * valueToSize);
            //    }
            //}
        }


        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Maximum), new PropertyMetadataAttribute(10d));
            overridePropertys.Override(nameof(SmallChange), new PropertyMetadataAttribute(1d));
            overridePropertys.Override(nameof(LargeChange), new PropertyMetadataAttribute(2d));
        }
    }

    /// <summary>
    /// Placement options for Slider's Tickbar
    /// </summary>
    public enum TickPlacement : byte
    {
        /// <summary>
        /// No TickMark
        /// </summary>
        None,
        /// <summary>
        /// Show TickMark above the Track (for HorizontalSlider), or left of the Track (for VerticalSlider)
        /// </summary>
        TopLeft,
        /// <summary>
        /// Show TickMark below the Track (for HorizontalSlider), or right of the Track (for VerticalSlider)
        /// </summary>
        BottomRight,
        /// <summary>
        /// Show TickMark on both side of the Track
        /// </summary>
        Both,

        // NOTE: if you add or remove any values in this enum, be sure to update Slider.IsValidTickPlacement()    
    };
}
