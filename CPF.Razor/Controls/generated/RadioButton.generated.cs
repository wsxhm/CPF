// CPF自动生成.
            
using CPF;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Razor;
using CPF.Shapes;
using Microsoft.AspNetCore.Components;
using System;

namespace CPF.Razor.Controls
{
    /// <summary>
    /// 表示可由用户选择但不能清除的按钮。 可以通过单击来设置 IsChecked 的 RadioButton 属性，但只能以编程方式清除该属性。
    /// </summary>
    public partial class RadioButton : Element<CPF.Controls.RadioButton> ,IHandleChildContentText
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
        [Parameter] public ClickMode? ClickMode { get; set; }
        /// <summary>
        /// 内容可以是字符串，UI元素等等
        /// <summary>
        [Parameter] public object Content { get; set; }
        [Parameter] public string ContentStringFormat { get; set; }
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
        /// 通过该属性对RadioButton分组，通过Root.GetRadioButtonValue()获取分组的选中值
        /// <summary>
        [Parameter] public string GroupName { get; set; }
        [Parameter] public Nullable<bool> IsChecked { get; set; }
        [Parameter] public bool? IsThreeState { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        [Parameter] public EventCallback Checked { get; set; }
        [Parameter] public EventCallback Indeterminate { get; set; }
        [Parameter] public EventCallback Unchecked { get; set; }
        [Parameter] public EventCallback<CPF.RoutedEventArgs> Click { get; set; }
        [Parameter] public EventCallback Initialized { get; set; }

    }
}
