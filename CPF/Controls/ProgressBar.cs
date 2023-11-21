using System;
using System.Collections.Generic;
using System.Text;
using CPF.Shapes;
using CPF.Styling;
using System.Linq;
using CPF.Drawing;
using CPF.Animation;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 指示操作进度。
    /// </summary>
    [Description("指示操作进度。"), Browsable(true)]
    public class ProgressBar : RangeBase
    {
        /// <summary>
        /// 获取或设置 ProgressBar 是显示实际值，还是显示一般的连续进度反馈。
        /// </summary>
        [Description("获取或设置 ProgressBar 是显示实际值，还是显示一般的连续进度反馈。")]
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(); }
            set { SetValue(value); }
        }

        protected override void InitializeComponent()
        {
            ClipToBounds = true;
            Background = "#E6E6E6";
            BorderFill = "#BCBCBC";
            BorderStroke = new Stroke(1);
            Children.Add(new Border
            {
                PresenterFor = this,
                Name = "Indicator",
                BorderFill = null,
                MarginLeft = 0,
                MarginTop = 0,
                MarginBottom = 0,
                Bindings =
                {
                    {nameof(Border.Background),nameof(Foreground),this },
                    {nameof(Border.Visibility),nameof(IsIndeterminate),this,BindingMode.OneWay,a=>(bool)a?Visibility.Collapsed:Visibility.Visible },
                }
            });
            Children.Add(new Border
            {
                PresenterFor = this,
                Name = "Animation",
                BorderFill = null,
                MarginLeft = "-30%",
                MarginTop = 0,
                MarginBottom = 0,
                Width = "30%",
                Bindings =
                {
                    {nameof(Border.Background),nameof(Foreground),this },
                    {nameof(Border.Visibility),nameof(IsIndeterminate),this,BindingMode.OneWay,a=>(bool)a?Visibility.Visible:Visibility.Collapsed },
                }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(Orientation),
                PropertyConditions = a => (Orientation)a == Orientation.Vertical,
                TargetRelation = Relation.Me.Children(a => a.PresenterFor == this && a.Name == "Indicator"),
                Setters =
                {
                    {nameof(MarginBottom),0 },
                    {nameof(MarginLeft),0 },
                    {nameof(MarginTop),FloatField.Auto },
                    {nameof(MarginRight),0 },
                }
            });
            Triggers.Add(new Trigger
            {
                Property = nameof(Orientation),
                PropertyConditions = a => (Orientation)a == Orientation.Vertical,
                TargetRelation = Relation.Me.Children(a => a.PresenterFor == this && a.Name == "Animation"),
                Setters =
                {
                    {nameof(MarginBottom),"-30%" },
                    {nameof(MarginLeft),0 },
                    {nameof(MarginTop),FloatField.Auto },
                    {nameof(MarginRight),0 },
                    {nameof(Width),FloatField.Auto },
                    {nameof(Height),"30%" },
                }
            });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _indicator = FindPresenter().FirstOrDefault(a => a.Name == "Indicator");
            _Animation = FindPresenter().FirstOrDefault(a => a.Name == "Animation");
        }

        [PropertyChanged(nameof(Orientation))]
        [PropertyChanged(nameof(Minimum))]
        [PropertyChanged(nameof(Maximum))]
        [PropertyChanged(nameof(Value))]
        [PropertyChanged(nameof(IsIndeterminate))]
        void OnValueChanged(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (_indicator != null)
            {
                var min = Minimum;
                var max = Maximum;
                var val = Value;

                // When indeterminate or maximum == minimum, have the indicator stretch the 
                // whole length of track
                var percent = IsIndeterminate || max <= min ? 1f : (val - min) / (max - min);
                if (Orientation == Orientation.Horizontal)
                {
                    _indicator.Width = percent * ActualSize.Width;
                    _indicator.Height = FloatField.Auto;
                }
                else
                {
                    _indicator.Width = FloatField.Auto;
                    _indicator.Height = percent * ActualSize.Height;
                }
            }
        }
        [PropertyChanged(nameof(IsIndeterminate))]
        [PropertyChanged(nameof(Orientation))]
        [PropertyChanged(nameof(Visibility))]
        void OnIsIndeterminate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            UpdateAnimation();
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (_Animation && IsIndeterminate && Visibility == Visibility.Visible && Root != null && !DesignMode)
            {
                if (storyboard != null)
                {
                    storyboard.Dispose();
                }
                if (storyboard == null)
                {
                    storyboard = new Storyboard();
                }
                if (Orientation == Orientation.Horizontal)
                {
                    storyboard.Timelines.Add(new Timeline(1)
                    {
                        KeyFrames =
                        {
                            new KeyFrame<FloatField>{ Property=nameof(MarginLeft), Value="100%" }
                        }
                    });
                }
                else
                {
                    storyboard.Timelines.Add(new Timeline(1)
                    {
                        KeyFrames =
                        {
                            new KeyFrame<FloatField>{ Property=nameof(MarginBottom), Value="100%" }
                        }
                    });
                }
                storyboard.Start(_Animation, TimeSpan.FromSeconds(2), 0);
            }
            else
            {
                if (storyboard != null)
                {
                    storyboard.Dispose();
                    storyboard = null;
                }
            }
        }

        protected override void OnDetachedFromVisualTree()
        {
            base.OnDetachedFromVisualTree();
            if (storyboard != null)
            {
                if (_Animation != null)
                {
                    storyboard.Stop(_Animation);
                }
                storyboard.Dispose();
                storyboard = null;
            }
        }

        Storyboard storyboard;

        Size size;
        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            if (size != ActualSize)
            {
                size = ActualSize;
                OnValueChanged(null, null, null);
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Maximum), new PropertyMetadataAttribute(100f));
            overridePropertys.Override(nameof(SmallChange), new PropertyMetadataAttribute(1f));
            overridePropertys.Override(nameof(LargeChange), new PropertyMetadataAttribute(5f));
            overridePropertys.Override(nameof(Foreground), new UIPropertyMetadataAttribute((ViewFill)"#06B025FF", UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits));
        }

        private UIElement _indicator;
        UIElement _Animation;
    }
}
