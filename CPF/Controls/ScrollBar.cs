using System;
using System.Collections.Generic;
using System.Text;
using CPF.Styling;
using CPF.Shapes;
using CPF.Drawing;
using CPF.Input;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 滚动条
    /// </summary>
    [Description("滚动条"), Browsable(true)]
    public class ScrollBar : RangeBase
    {
        /// <summary>
        /// 滚动条方向
        /// </summary>
        [PropertyMetadata(Orientation.Vertical)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(); }
            set { SetValue(value); }
        }

        //Binding valueBinding;
        /// <summary>
        /// 滚动条
        /// </summary>
        public ScrollBar()
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
            var decorator = Children.Add(new Decorator { Size = SizeField.Fill });
            decorator.Bindings.Add(nameof(Decorator.Child), nameof(Orientation), this, BindingMode.OneWay, orientation =>
            {
                if ((Orientation)orientation == Orientation.Horizontal)
                {
                    return new Grid
                    {
                        Width = "100%",
                        Height = "100%",
                        ColumnDefinitions ={
                                new ColumnDefinition {Width="auto" },
                                new ColumnDefinition {Width="*" },
                                new ColumnDefinition {Width="auto" }, },
                        Children =
                            {
                                { new RepeatButton{
                                    Name="PART_LineUpButton",
                        Width =20,
                        Height ="100%",
                        Content =new Polyline { Points = {new Point(5,0),new Point(0,5),new Point(5,10) },IsAntiAlias=true },
                        Commands = { { nameof(RepeatButton.Click), nameof(CommandDecreaseSmallChange), this } },
                            Triggers ={ new Trigger { Property=nameof(IsMouseOver),PropertyConditions=a=>(bool)a,TargetRelation=Relation.Me.Find(a => a is Polyline), Setters = { {nameof(Polyline.StrokeFill),"28,151,234" } } } } } },
                                { new Track
                                    {
                                        Name="PART_Track",
                                        Height ="100%",
                                        Width ="100%",
                                        Thumb=new Thumb{
                                            Width ="100%",
                                            Height="100%",
                                            Triggers={
                                                new Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a, Setters = { { "Background", "190,230,253" } } },
                                                new Trigger { Property = nameof(Thumb.IsDragging), PropertyConditions = a => (bool)a, Setters = { { "Background", "186,209,226" } } }
                                            }
                                        },
                                        IncreaseRepeatButton=new RepeatButton{Width ="100%", Height="100%",Commands = { { nameof(RepeatButton.Click), nameof(CommandIncreaseLargeChanged), this } } },
                                        DecreaseRepeatButton=new RepeatButton{Width ="100%", Height="100%",Commands = { { nameof(RepeatButton.Click), nameof(CommandDecreaseLargeChanged), this } } },
                                        Bindings ={
                                            { nameof(Track.Orientation), nameof(Orientation),3 },
                                            { nameof(Track.Value),nameof(Value),3,BindingMode.TwoWay},
                                            { nameof(Track.Maximum),nameof(Maximum),3},
                                            { nameof(Track.Minimum),nameof(Minimum),3},
                                            { nameof(Track.ViewportSize),nameof(ViewportSize),3},
                                        },

                                    },1,0 },
                                {new RepeatButton{
                            Name="PART_LineDownButton",
                        Width =20,
                        Height ="100%",
                        Content =new Polyline{ Points ={ { 0, 0 },{ 5, 5 },{ 0, 10 } },IsAntiAlias=true },
                        Commands ={ {nameof(RepeatButton.Click),nameof(CommandIncreaseSmallChange), this } },
                            Triggers ={ new Trigger { Property=nameof(IsMouseOver),PropertyConditions=a=>(bool)a,TargetRelation=Relation.Me.Find(a => a is Polyline), Setters = { {nameof(Polyline.StrokeFill),"28,151,234" } } } } } ,2,0 }
                            }
                    };
                }
                else
                {
                    //默认是垂直的
                    return new Grid
                    {
                        Width = "100%",
                        Height = "100%",
                        //Background = "white",
                        ColumnDefinitions = { new ColumnDefinition { } },
                        RowDefinitions =
                        {
                            new RowDefinition { Height = "auto" },
                            new RowDefinition { Height = "*" },
                            new RowDefinition { Height = "auto" }
                        },
                        Children =
                        {
                            {
                                new RepeatButton
                                {
                                    Name="PART_LineUpButton",
                                    Width ="100%",
                                    Height = 20,
                                    Content =new Polyline { Points = { { 0, 5 }, { 5, 0 }, { 10, 5 } },IsAntiAlias=true },
                                    Commands = { { nameof(RepeatButton.Click), nameof(CommandDecreaseSmallChange), this } },
                                    Triggers={ new Trigger { Property=nameof(IsMouseOver),PropertyConditions=a=>(bool)a,TargetRelation=Relation.Me.Find(a => a is Polyline), Setters = { {nameof(Polyline.StrokeFill),"28,151,234" } } } }
                                }
                            },
                            {
                                new Track
                                {
                                    Name="PART_Track",
                                    Height ="100%",
                                    Width ="100%",
                                    IsDirectionReversed=true,
                                    Thumb=new Thumb{
                                        Width ="100%",
                                        Height ="100%",
                                        Triggers={
                                            new Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a, Setters = { { "Background", "190,230,253" } } },
                                            new Trigger { Property = nameof(Thumb.IsDragging), PropertyConditions = a => (bool)a, Setters = { { "Background", "186,209,226" } } }
                                        }
                                    },
                                    IncreaseRepeatButton=new RepeatButton{Width ="100%", Height="100%",Commands = { { nameof(RepeatButton.Click), nameof(CommandIncreaseLargeChanged), this } } },
                                    DecreaseRepeatButton=new RepeatButton{Width ="100%", Height="100%",Commands = { { nameof(RepeatButton.Click), nameof(CommandDecreaseLargeChanged), this } } },
                                    Bindings ={
                                        { nameof(Track.Orientation), nameof(Orientation),3 },
                                        { nameof(Track.Value),nameof(Value),3,BindingMode.TwoWay},
                                        { nameof(Track.Maximum),nameof(Maximum),3},
                                        { nameof(Track.Minimum),nameof(Minimum),3},
                                        { nameof(Track.ViewportSize),nameof(ViewportSize),3},
                                    },
                                },
                                0,
                                1
                            },
                            {
                                new RepeatButton
                                {
                                    Name="PART_LineDownButton",
                                    Width ="100%",
                                    Height = 20,
                                    Content =new Polyline
                                    {
                                        Points ={ { 0, 0 },{ 5, 5 },{ 10, 0 } },IsAntiAlias=true
                                    },
                                    Commands ={ {nameof(RepeatButton.Click),nameof(CommandIncreaseSmallChange), this } },
                                    Triggers ={ new Trigger { Property=nameof(IsMouseOver),PropertyConditions=a=>(bool)a,TargetRelation=Relation.Me.Find(a=>a is Polyline), Setters = { {nameof(Polyline.StrokeFill),"28,151,234" } } } }
                                },0,2
                            }
                        }
                    };
                }
            });

        }
        /// <summary>
        /// 视图大小，用来计算Thumb占用比例
        /// </summary>
        [PropertyMetadata(0f)]
        public float ViewportSize
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
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

        //protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseDown(e);

        //    System.Diagnostics.Debug.WriteLine(Root.InputManager.MouseDevice.Location + "  " + PointToClient(Root.InputManager.MouseDevice.Location));
        //}
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"240,240,240", UIPropertyOptions.AffectsRender));
        }
    }

    public enum Orientation : byte
    {
        /// <summary>
        /// Control/Layout should be horizontally oriented.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Control/Layout should be vertically oriented.
        /// </summary>
        Vertical,
    }
}
