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
    /// 多边形
    /// </summary>
    public partial class Polygon : Element<CPF.Shapes.Polygon>
    {
        
        [Parameter] public string Fill { get; set; }
        /// <summary>
        /// 事件响应范围是路径的线条上还是路径围成的范围内，true就是在线条上
        /// <summary>
        [Parameter] public bool? IsHitTestOnPath { get; set; }
        [Parameter] public string StrokeFill { get; set; }
        [Parameter] public Stroke? StrokeStyle { get; set; }

    }
}
