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
    /// 支持可视化设计的控件基类
    /// </summary>
    public partial class CodeTextBox : Element<CPF.Controls.CodeTextBox>
    {
        
        /// <summary>
        /// 如果按 Tab 键会在当前光标位置插入一个制表符，则为 true；如果按 Tab 键会将焦点移动到标记为制表位的下一个控件且不插入制表符，则为 false
        /// <summary>
        [Parameter] public bool? AcceptsTab { get; set; }
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
        /// 获取或设置用于绘制文本框的插入符号的画笔
        /// <summary>
        [Parameter] public string CaretFill { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
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
        /// 获取或设置一个值，该值指示是否应显示水平 ScrollBar
        /// <summary>
        [Parameter] public ScrollBarVisibility? HScrollBarVisibility { get; set; }
        /// <summary>
        /// 是否启用输入法，主要描述的是中文这类输入法
        /// <summary>
        [Parameter] public bool? IsInputMethodEnabled { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示文本编辑控件对于与该控件交互的用户是否是只读的
        /// <summary>
        [Parameter] public bool? IsReadOnly { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示文本编辑控件是否支持撤消功能
        /// <summary>
        [Parameter] public bool? IsUndoEnabled { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        /// <summary>
        /// 获取或设置会突出显示选定文本的画笔。
        /// <summary>
        [Parameter] public string SelectionFill { get; set; }
        /// <summary>
        /// 获取或设置一个值，此值定义用于所选文本的画笔。
        /// <summary>
        [Parameter] public string SelectionTextFill { get; set; }
        /// <summary>
        /// 获取或设置一个值，是否显示行号
        /// <summary>
        [Parameter] public bool? ShowLineNumber { get; set; }
        [Parameter] public string Text { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        /// <summary>
        /// 获取或设置存储在撤消队列中的操作的数目。 默认值为-1, 表示撤消队列限制为可用的内存。
        /// <summary>
        [Parameter] public int? UndoLimit { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应显示垂直 ScrollBar。
        /// <summary>
        [Parameter] public ScrollBarVisibility? VScrollBarVisibility { get; set; }
        [Parameter] public EventCallback Initialized { get; set; }
        /// <summary>
        /// 如果按 Tab 键会在当前光标位置插入一个制表符，则为 true；如果按 Tab 键会将焦点移动到标记为制表位的下一个控件且不插入制表符，则为 false
        /// <summary>
        [Parameter] public EventCallback<bool> AcceptsTabChanged { get; set; }
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
        /// 获取或设置用于绘制文本框的插入符号的画笔
        /// <summary>
        [Parameter] public EventCallback<ViewFill> CaretFillChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public EventCallback<CornerRadius> CornerRadiusChanged { get; set; }
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
        /// 获取或设置一个值，该值指示是否应显示水平 ScrollBar
        /// <summary>
        [Parameter] public EventCallback<ScrollBarVisibility> HScrollBarVisibilityChanged { get; set; }
        /// <summary>
        /// 是否启用输入法，主要描述的是中文这类输入法
        /// <summary>
        [Parameter] public EventCallback<bool> IsInputMethodEnabledChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示文本编辑控件对于与该控件交互的用户是否是只读的
        /// <summary>
        [Parameter] public EventCallback<bool> IsReadOnlyChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示文本编辑控件是否支持撤消功能
        /// <summary>
        [Parameter] public EventCallback<bool> IsUndoEnabledChanged { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public EventCallback<Thickness> PaddingChanged { get; set; }
        /// <summary>
        /// 获取或设置会突出显示选定文本的画笔。
        /// <summary>
        [Parameter] public EventCallback<ViewFill> SelectionFillChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，此值定义用于所选文本的画笔。
        /// <summary>
        [Parameter] public EventCallback<ViewFill> SelectionTextFillChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，是否显示行号
        /// <summary>
        [Parameter] public EventCallback<bool> ShowLineNumberChanged { get; set; }
        [Parameter] public EventCallback<string> TextChanged { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }
        /// <summary>
        /// 获取或设置存储在撤消队列中的操作的数目。 默认值为-1, 表示撤消队列限制为可用的内存。
        /// <summary>
        [Parameter] public EventCallback<int> UndoLimitChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应显示垂直 ScrollBar。
        /// <summary>
        [Parameter] public EventCallback<ScrollBarVisibility> VScrollBarVisibilityChanged { get; set; }

    }
}
