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
    /// 绘制一系列相互连接的直线和曲线。
    /// </summary>
    public partial class Path : Element<CPF.Shapes.Path>
    {
        
        [Parameter] public PathGeometry Data { get; set; }
        [Parameter] public string Fill { get; set; }
        /// <summary>
        /// 事件响应范围是路径的线条上还是路径围成的范围内，true就是在线条上
        /// <summary>
        [Parameter] public bool? IsHitTestOnPath { get; set; }
        /// <summary>
        /// 获取或设置 Stretch 模式，该模式确定内容适应可用空间的方式。
        /// <summary>
        [Parameter] public Stretch? Stretch { get; set; }
        [Parameter] public string StrokeFill { get; set; }
        [Parameter] public Stroke? StrokeStyle { get; set; }

    }
}
