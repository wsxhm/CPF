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
    /// 表示重新分布 Grid 控件的列间距或行间距的控件。
    /// </summary>
    public partial class GridSplitter : Element<CPF.Controls.GridSplitter>
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
        [Parameter] public UIElement Child { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
        [Parameter] public float? DragIncrement { get; set; }
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
        [Parameter] public float? KeyboardIncrement { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        [Parameter] public string PreviewColor { get; set; }
        [Parameter] public GridResizeBehavior? ResizeBehavior { get; set; }
        [Parameter] public GridResizeDirection? ResizeDirection { get; set; }
        [Parameter] public bool? ShowsPreview { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        [Parameter] public EventCallback<DragStartedEventArgs> DragStarted { get; set; }
        [Parameter] public EventCallback<DragDeltaEventArgs> DragDelta { get; set; }
        [Parameter] public EventCallback<DragCompletedEventArgs> DragCompleted { get; set; }
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
        [Parameter] public EventCallback<UIElement> ChildChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public EventCallback<CornerRadius> CornerRadiusChanged { get; set; }
        [Parameter] public EventCallback<float> DragIncrementChanged { get; set; }
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
        [Parameter] public EventCallback<float> KeyboardIncrementChanged { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public EventCallback<Thickness> PaddingChanged { get; set; }
        [Parameter] public EventCallback<Color> PreviewColorChanged { get; set; }
        [Parameter] public EventCallback<GridResizeBehavior> ResizeBehaviorChanged { get; set; }
        [Parameter] public EventCallback<GridResizeDirection> ResizeDirectionChanged { get; set; }
        [Parameter] public EventCallback<bool> ShowsPreviewChanged { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }

    }
}
