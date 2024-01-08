// CPF自动生成.
            
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
    /// 显示图像，支持路径、Url、Image、Bitmap、Stream、byte[]、支持GIF播放
    /// </summary>
    public partial class Picture : Element<CPF.Controls.Picture>
    {
        
        /// <summary>
        /// 图片源，可以是路径、Url、Drawing.Image对象、Stream、byte[]
        /// <summary>
        [Parameter] public object Source { get; set; }
        /// <summary>
        /// 图片缩放模式
        /// <summary>
        [Parameter] public Stretch? Stretch { get; set; }
        /// <summary>
        /// 描述如何对内容应用缩放，并限制对已命名像素类型的缩放。
        /// <summary>
        [Parameter] public StretchDirection? StretchDirection { get; set; }
        [Parameter] public EventCallback ImageFailed { get; set; }

    }
}
