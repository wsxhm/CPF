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
    /// 简单的文档控件，支持图片字符控件等元素布局，支持每个字符设置样式
    /// </summary>
    public partial class DocumentBlock : Element<CPF.Controls.DocumentBlock>
    {
        
        [Parameter] public string Background { get; set; }
        [Parameter] public string FontFamily { get; set; }
        [Parameter] public float? FontSize { get; set; }
        [Parameter] public FontStyles? FontStyle { get; set; }
        [Parameter] public string Foreground { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        [Parameter] public EventCallback<ViewFill> BackgroundChanged { get; set; }
        [Parameter] public EventCallback<string> FontFamilyChanged { get; set; }
        [Parameter] public EventCallback<float> FontSizeChanged { get; set; }
        [Parameter] public EventCallback<FontStyles> FontStyleChanged { get; set; }
        [Parameter] public EventCallback<ViewFill> ForegroundChanged { get; set; }
        [Parameter] public EventCallback<string> TextChanged { get; set; }
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }

    }
}
