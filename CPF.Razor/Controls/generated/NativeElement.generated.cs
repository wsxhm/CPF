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
    /// 用于内嵌原生控件，一般来说原生控件无法使用渲染变换，无法调整ZIndex，只能在最前端，可能无法透明。一般尽可能少用该控件
    /// </summary>
    public partial class NativeElement : Element<CPF.Controls.NativeElement>
    {
        
        /// <summary>
        /// 背景色，有些平台可能无法透明
        /// <summary>
        [Parameter] public string BackColor { get; set; }
        /// <summary>
        /// 设置对应平台的控件句柄
        /// <summary>
        [Parameter] public object Content { get; set; }

    }
}
