// CPF自动生成.
            
using CPF;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Razor;
using CPF.Shapes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace CPF.Razor.Controls
{
    /// <summary>
    /// 提供折线图，曲线图，柱状图
    /// </summary>
    public partial class Chart : Element<CPF.Charts.Chart>
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
        /// <summary>
        /// 是否可以缩放滚动
        /// <summary>
        [Parameter] public bool? CanScroll { get; set; }
        /// <summary>
        /// 图表区域填充
        /// <summary>
        [Parameter] public string ChartFill { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
        [Parameter] public IList<IChartData> Data { get; set; }
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
        /// 网格填充
        /// <summary>
        [Parameter] public string GridFill { get; set; }
        /// <summary>
        /// 网格显示模式
        /// <summary>
        [Parameter] public GridShowMode? GridShowMode { get; set; }
        /// <summary>
        /// 水平缩放值 大于等于1
        /// <summary>
        [Parameter] public float? HorizontalScaling { get; set; }
        /// <summary>
        /// 鼠标移入选中的线条填充
        /// <summary>
        [Parameter] public string HoverSelectLineFill { get; set; }
        /// <summary>
        /// 鼠标移入选中的坐标轴提示背景填充
        /// <summary>
        [Parameter] public string HoverSelectTipBackFill { get; set; }
        /// <summary>
        /// 鼠标移入选中的坐标轴提示文字填充
        /// <summary>
        [Parameter] public string HoverSelectTipFill { get; set; }
        /// <summary>
        /// Y轴最大值
        /// <summary>
        [Parameter] public Nullable<double> MaxValue { get; set; }
        /// <summary>
        /// Y轴最小值
        /// <summary>
        [Parameter] public Nullable<double> MinValue { get; set; }
        /// <summary>
        /// 鼠标移入图表的时候显示信息
        /// <summary>
        [Parameter] public bool? MouseHoverShowTip { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        /// <summary>
        /// 显示滚动缩放值的线条填充
        /// <summary>
        [Parameter] public string ScrollLineFill { get; set; }
        [Parameter] public float? ScrollValue { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        /// <summary>
        /// X轴文字
        /// <summary>
        [Parameter] public string XAxis { get; set; }
        /// <summary>
        /// X轴颜色
        /// <summary>
        [Parameter] public string XAxisFill { get; set; }
        /// <summary>
        /// Y轴颜色
        /// <summary>
        [Parameter] public string YAxisFill { get; set; }
        /// <summary>
        /// Y轴刻度分割数量，大于等于1
        /// <summary>
        [Parameter] public uint? YAxisScaleCount { get; set; }
        [Parameter] public EventCallback Initialized { get; set; }

    }
}
