//CPF自动生成.
            
using CPF;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Razor;
using CPF.Shapes;
using Microsoft.AspNetCore.Components;

namespace CPF.Razor.Controls
{
    /// <summary>
    /// 表示一个控件，该控件可让用户通过沿 Thumb 移动 Track 控件从一个值范围中进行选择。
    /// </summary>
    public partial class Slider : Element<CPF.Controls.Slider>
    {
        
        /// <summary>
        /// 背景填充
        /// <summary>
        [Parameter] public string Background { get; set; }
        /// <summary>
        /// 边框线条填充
        /// <summary>
        [Parameter] public string BorderFill { get; set; }
        /// <summary>
        /// 获取或设置线条类型
        /// <summary>
        [Parameter] public Stroke? BorderStroke { get; set; }
        /// <summary>
        /// 四周边框粗细
        /// <summary>
        [Parameter] public Thickness? BorderThickness { get; set; }
        /// <summary>
        /// 边框类型，BorderStroke和BorderThickness
        /// <summary>
        [Parameter] public BorderType? BorderType { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
        /// <summary>
        /// 获取或设置在按下 RepeatButton 之后等待执行用于移动 Thumb 的命令（如 DecreaseLarge 命令）的时间（以毫秒为单位）。
        /// <summary>
        [Parameter] public int? Delay { get; set; }
        /// <summary>
        /// 字体名
        /// <summary>
        [Parameter] public string FontFamily { get; set; }
        /// <summary>
        /// 字体尺寸，点
        /// <summary>
        [Parameter] public float? FontSize { get; set; }
        /// <summary>
        /// 字体样式
        /// <summary>
        [Parameter] public FontStyles? FontStyle { get; set; }
        /// <summary>
        /// 控件文字的填充
        /// <summary>
        [Parameter] public string Foreground { get; set; }
        /// <summary>
        /// 获取或设置当用户单击 RepeatButton 的 Slider 时增加或减少命令之间的时间量（以毫秒为单位）
        /// <summary>
        [Parameter] public int? Interval { get; set; }
        /// <summary>
        /// 如果增加值的方向向左（对于水平滑块）或向下（对于垂直滑块），则为 true；否则为 false。 默认值为 false。
        /// <summary>
        [Parameter] public bool? IsDirectionReversed { get; set; }
        /// <summary>
        ///  获取或设置一个值，该值指示是否立即将 Slider 的 Thumb 移动到在鼠标指针悬停在 Slider 轨道的上方时鼠标单击的位置。
        /// <summary>
        [Parameter] public bool? IsMoveToPointEnabled { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示 Slider 是否自动将 Thumb 移动到最近的刻度线
        /// <summary>
        [Parameter] public bool? IsSnapToTickEnabled { get; set; }
        [Parameter] public double? LargeChange { get; set; }
        /// <summary>
        /// 最大值
        /// <summary>
        [Parameter] public double? Maximum { get; set; }
        /// <summary>
        /// 最小值
        /// <summary>
        [Parameter] public double? Minimum { get; set; }
        /// <summary>
        /// 布局方向
        /// <summary>
        [Parameter] public Orientation? Orientation { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        [Parameter] public double? SmallChange { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        /// <summary>
        ///  刻度线之间的距离。 默认值为 (1.0)。
        /// <summary>
        [Parameter] public float? TickFrequency { get; set; }
        /// <summary>
        /// 获取或设置与 Track 的 Slider 相关的刻度线的位置。
        /// <summary>
        [Parameter] public TickPlacement? TickPlacement { get; set; }
        /// <summary>
        /// 获取或设置刻度线的位置。
        /// <summary>
        [Parameter] public Collection<float> Ticks { get; set; }
        /// <summary>
        /// 当前值
        /// <summary>
        [Parameter] public double? Value { get; set; }
        [Parameter] public EventCallback Initialized { get; set; }
        /// <summary>
        /// 背景填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> BackgroundChanged { get; set; }
        /// <summary>
        /// 边框线条填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> BorderFillChanged { get; set; }
        /// <summary>
        /// 获取或设置线条类型
        /// <summary>
        [Parameter] public EventCallback<Stroke> BorderStrokeChanged { get; set; }
        /// <summary>
        /// 四周边框粗细
        /// <summary>
        [Parameter] public EventCallback<Thickness> BorderThicknessChanged { get; set; }
        /// <summary>
        /// 边框类型，BorderStroke和BorderThickness
        /// <summary>
        [Parameter] public EventCallback<BorderType> BorderTypeChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public EventCallback<CornerRadius> CornerRadiusChanged { get; set; }
        /// <summary>
        /// 获取或设置在按下 RepeatButton 之后等待执行用于移动 Thumb 的命令（如 DecreaseLarge 命令）的时间（以毫秒为单位）。
        /// <summary>
        [Parameter] public EventCallback<int> DelayChanged { get; set; }
        /// <summary>
        /// 字体名
        /// <summary>
        [Parameter] public EventCallback<string> FontFamilyChanged { get; set; }
        /// <summary>
        /// 字体尺寸，点
        /// <summary>
        [Parameter] public EventCallback<float> FontSizeChanged { get; set; }
        /// <summary>
        /// 字体样式
        /// <summary>
        [Parameter] public EventCallback<FontStyles> FontStyleChanged { get; set; }
        /// <summary>
        /// 控件文字的填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> ForegroundChanged { get; set; }
        /// <summary>
        /// 获取或设置当用户单击 RepeatButton 的 Slider 时增加或减少命令之间的时间量（以毫秒为单位）
        /// <summary>
        [Parameter] public EventCallback<int> IntervalChanged { get; set; }
        /// <summary>
        /// 如果增加值的方向向左（对于水平滑块）或向下（对于垂直滑块），则为 true；否则为 false。 默认值为 false。
        /// <summary>
        [Parameter] public EventCallback<bool> IsDirectionReversedChanged { get; set; }
        /// <summary>
        ///  获取或设置一个值，该值指示是否立即将 Slider 的 Thumb 移动到在鼠标指针悬停在 Slider 轨道的上方时鼠标单击的位置。
        /// <summary>
        [Parameter] public EventCallback<bool> IsMoveToPointEnabledChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示 Slider 是否自动将 Thumb 移动到最近的刻度线
        /// <summary>
        [Parameter] public EventCallback<bool> IsSnapToTickEnabledChanged { get; set; }
        [Parameter] public EventCallback<double> LargeChangeChanged { get; set; }
        /// <summary>
        /// 最大值
        /// <summary>
        [Parameter] public EventCallback<double> MaximumChanged { get; set; }
        /// <summary>
        /// 最小值
        /// <summary>
        [Parameter] public EventCallback<double> MinimumChanged { get; set; }
        /// <summary>
        /// 布局方向
        /// <summary>
        [Parameter] public EventCallback<Orientation> OrientationChanged { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public EventCallback<Thickness> PaddingChanged { get; set; }
        [Parameter] public EventCallback<double> SmallChangeChanged { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }
        /// <summary>
        ///  刻度线之间的距离。 默认值为 (1.0)。
        /// <summary>
        [Parameter] public EventCallback<float> TickFrequencyChanged { get; set; }
        /// <summary>
        /// 获取或设置与 Track 的 Slider 相关的刻度线的位置。
        /// <summary>
        [Parameter] public EventCallback<TickPlacement> TickPlacementChanged { get; set; }
        /// <summary>
        /// 获取或设置刻度线的位置。
        /// <summary>
        [Parameter] public EventCallback<Collection<float>> TicksChanged { get; set; }
        /// <summary>
        /// 当前值
        /// <summary>
        [Parameter] public EventCallback<double> ValueChanged { get; set; }

    }
}
