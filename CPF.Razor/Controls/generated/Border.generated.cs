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
    /// 在另一个元素四周绘制边框和背景
    /// </summary>
    public partial class Border : Element<CPF.Controls.Border>
    {
        
        [Parameter] public string Background { get; set; }
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
        /// 边框类型
        /// <summary>
        [Parameter] public BorderType? BorderType { get; set; }
        [Parameter] public UIElement Child { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        /// <summary>
        /// 模糊宽度
        /// <summary>
        [Parameter] public byte? ShadowBlur { get; set; }
        /// <summary>
        /// 阴影颜色
        /// <summary>
        [Parameter] public string ShadowColor { get; set; }
        /// <summary>
        /// 阴影水平偏移
        /// <summary>
        [Parameter] public sbyte? ShadowHorizontal { get; set; }
        /// <summary>
        /// 阴影垂直偏移
        /// <summary>
        [Parameter] public sbyte? ShadowVertical { get; set; }
        [Parameter] public EventCallback<ViewFill> BackgroundChanged { get; set; }
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
        /// 边框类型
        /// <summary>
        [Parameter] public EventCallback<BorderType> BorderTypeChanged { get; set; }
        [Parameter] public EventCallback<UIElement> ChildChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。
        /// <summary>
        [Parameter] public EventCallback<CornerRadius> CornerRadiusChanged { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值
        /// <summary>
        [Parameter] public EventCallback<Thickness> PaddingChanged { get; set; }
        /// <summary>
        /// 模糊宽度
        /// <summary>
        [Parameter] public EventCallback<byte> ShadowBlurChanged { get; set; }
        /// <summary>
        /// 阴影颜色
        /// <summary>
        [Parameter] public EventCallback<Color> ShadowColorChanged { get; set; }
        /// <summary>
        /// 阴影水平偏移
        /// <summary>
        [Parameter] public EventCallback<sbyte> ShadowHorizontalChanged { get; set; }
        /// <summary>
        /// 阴影垂直偏移
        /// <summary>
        [Parameter] public EventCallback<sbyte> ShadowVerticalChanged { get; set; }

    }
}
