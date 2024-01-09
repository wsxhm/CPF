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
    /// 文本显示
    /// </summary>
    public partial class TextBlock : Element<CPF.Controls.TextBlock>
    {
        
        [Parameter] public string Background { get; set; }
        [Parameter] public string FontFamily { get; set; }
        [Parameter] public float? FontSize { get; set; }
        [Parameter] public FontStyles? FontStyle { get; set; }
        [Parameter] public string Foreground { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public TextAlignment? TextAlignment { get; set; }
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        /// <summary>
        /// 文字描边
        /// <summary>
        [Parameter] public Stroke? TextStroke { get; set; }
        /// <summary>
        /// 文字描边
        /// <summary>
        [Parameter] public string TextStrokeFill { get; set; }
        /// <summary>
        /// 文本裁剪
        /// <summary>
        [Parameter] public TextTrimming? TextTrimming { get; set; }
        /// <summary>
        /// 文本在垂直方向的对齐方式
        /// <summary>
        [Parameter] public VerticalAlignment? VerticalAlignment { get; set; }
        [Parameter] public EventCallback<ViewFill> BackgroundChanged { get; set; }
        [Parameter] public EventCallback<string> FontFamilyChanged { get; set; }
        [Parameter] public EventCallback<float> FontSizeChanged { get; set; }
        [Parameter] public EventCallback<FontStyles> FontStyleChanged { get; set; }
        [Parameter] public EventCallback<ViewFill> ForegroundChanged { get; set; }
        [Parameter] public EventCallback<string> TextChanged { get; set; }
        [Parameter] public EventCallback<TextAlignment> TextAlignmentChanged { get; set; }
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }
        /// <summary>
        /// 文字描边
        /// <summary>
        [Parameter] public EventCallback<Stroke> TextStrokeChanged { get; set; }
        /// <summary>
        /// 文字描边
        /// <summary>
        [Parameter] public EventCallback<ViewFill> TextStrokeFillChanged { get; set; }
        /// <summary>
        /// 文本裁剪
        /// <summary>
        [Parameter] public EventCallback<TextTrimming> TextTrimmingChanged { get; set; }
        /// <summary>
        /// 文本在垂直方向的对齐方式
        /// <summary>
        [Parameter] public EventCallback<VerticalAlignment> VerticalAlignmentChanged { get; set; }

    }
}
