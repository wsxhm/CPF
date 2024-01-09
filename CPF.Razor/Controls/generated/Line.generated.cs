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
    /// 在两个点之间绘制直线。
    /// </summary>
    public partial class Line : Element<CPF.Shapes.Line>
    {
        
        [Parameter] public Point? EndPoint { get; set; }
        [Parameter] public string Fill { get; set; }
        /// <summary>
        /// 事件响应范围是路径的线条上还是路径围成的范围内，true就是在线条上
        /// <summary>
        [Parameter] public bool? IsHitTestOnPath { get; set; }
        [Parameter] public Point? StartPoint { get; set; }
        [Parameter] public string StrokeFill { get; set; }
        [Parameter] public Stroke? StrokeStyle { get; set; }
        [Parameter] public EventCallback<Point> EndPointChanged { get; set; }
        [Parameter] public EventCallback<ViewFill> FillChanged { get; set; }
        /// <summary>
        /// 事件响应范围是路径的线条上还是路径围成的范围内，true就是在线条上
        /// <summary>
        [Parameter] public EventCallback<bool> IsHitTestOnPathChanged { get; set; }
        [Parameter] public EventCallback<Point> StartPointChanged { get; set; }
        [Parameter] public EventCallback<ViewFill> StrokeFillChanged { get; set; }
        [Parameter] public EventCallback<Stroke> StrokeStyleChanged { get; set; }

    }
}
