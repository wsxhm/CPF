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
    /// 
    /// </summary>
    public partial class SVG : Element<CPF.Svg.SVG>
    {
        
        [Parameter] public string Fill { get; set; }
        /// <summary>
        /// SVG源，可以是路径、Url、或者svg文档字符串
        /// <summary>
        [Parameter] public string Source { get; set; }
        /// <summary>
        /// 图片缩放模式
        /// <summary>
        [Parameter] public Stretch? Stretch { get; set; }
        /// <summary>
        /// 描述如何对内容应用缩放，并限制对已命名像素类型的缩放。
        /// <summary>
        [Parameter] public StretchDirection? StretchDirection { get; set; }
        [Parameter] public EventCallback<ViewFill> FillChanged { get; set; }
        /// <summary>
        /// SVG源，可以是路径、Url、或者svg文档字符串
        /// <summary>
        [Parameter] public EventCallback<string> SourceChanged { get; set; }
        /// <summary>
        /// 图片缩放模式
        /// <summary>
        [Parameter] public EventCallback<Stretch> StretchChanged { get; set; }
        /// <summary>
        /// 描述如何对内容应用缩放，并限制对已命名像素类型的缩放。
        /// <summary>
        [Parameter] public EventCallback<StretchDirection> StretchDirectionChanged { get; set; }

    }
}
