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
    /// 按从左到右的顺序位置定位子元素，在包含框的边缘处将内容切换到下一行。 后续排序按照从上至下或从右至左的顺序进行，具体取决于 Orientation 属性的值。
    /// </summary>
    public partial class WrapPanel : Element<CPF.Controls.WrapPanel>
    {
        
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
        [Parameter] public BorderType? BorderType { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
        [Parameter] public string FontFamily { get; set; }
        [Parameter] public float? FontSize { get; set; }
        [Parameter] public FontStyles? FontStyle { get; set; }
        [Parameter] public string Foreground { get; set; }
        /// <summary>
        /// 定义一个控件组，一般由多个元素组成，在设计器中，子元素和该控件为一个控件组，点击子元素拖动时，将作为整体拖动整个控件组。如果该控件被子元素盖住，按Alt+Ctrl键加鼠标左键可以选中该控件。按住Alt键可以移动子元素。
        /// <summary>
        [Parameter] public bool? IsGroup { get; set; }
        [Parameter] public float? ItemHeight { get; set; }
        [Parameter] public float? ItemWidth { get; set; }
        [Parameter] public Orientation? Orientation { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
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
        [Parameter] public EventCallback<BorderType> BorderTypeChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。
        /// <summary>
        [Parameter] public EventCallback<CornerRadius> CornerRadiusChanged { get; set; }
        [Parameter] public EventCallback<string> FontFamilyChanged { get; set; }
        [Parameter] public EventCallback<float> FontSizeChanged { get; set; }
        [Parameter] public EventCallback<FontStyles> FontStyleChanged { get; set; }
        [Parameter] public EventCallback<ViewFill> ForegroundChanged { get; set; }
        /// <summary>
        /// 定义一个控件组，一般由多个元素组成，在设计器中，子元素和该控件为一个控件组，点击子元素拖动时，将作为整体拖动整个控件组。如果该控件被子元素盖住，按Alt+Ctrl键加鼠标左键可以选中该控件。按住Alt键可以移动子元素。
        /// <summary>
        [Parameter] public EventCallback<bool> IsGroupChanged { get; set; }
        [Parameter] public EventCallback<float> ItemHeightChanged { get; set; }
        [Parameter] public EventCallback<float> ItemWidthChanged { get; set; }
        [Parameter] public EventCallback<Orientation> OrientationChanged { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }

    }
}
