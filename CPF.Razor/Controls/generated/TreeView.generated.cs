// CPF自动生成.
            
using CPF;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Razor;
using CPF.Shapes;
using Microsoft.AspNetCore.Components;
using System.Collections;

namespace CPF.Razor.Controls
{
    /// <summary>
    /// 表示一个控件，该控件在树结构（其中的项可以展开和折叠）中显示分层数据。
    /// </summary>
    public partial class TreeView : Element<CPF.Controls.TreeView>
    {
        
        /// <summary>
        /// 获取或设置 ItemsControl 中的交替项容器的数目，该控件可使交替容器具有唯一外观，通过附加数据AttachedExtenstions.AlternationIndex 读取循环的ID
        /// <summary>
        [Parameter] public uint? AlternationCount { get; set; }
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
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public CornerRadius? CornerRadius { get; set; }
        /// <summary>
        /// 显示的数据字段或属性
        /// <summary>
        [Parameter] public string DisplayMemberPath { get; set; }
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
        /// 返回CPF.Controls.ItemCollection类型，可以直接将数据源设置过来
        /// <summary>
        [Parameter] public IList Items { get; set; }
        [Parameter] public string ItemsMemberPath { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        [Parameter] public EventCallback SelectionChanged { get; set; }
        [Parameter] public EventCallback<CPF.Controls.TreeViewItemMouseEventArgs> ItemMouseDown { get; set; }
        [Parameter] public EventCallback<CPF.Controls.TreeViewItemMouseEventArgs> ItemMouseUp { get; set; }
        [Parameter] public EventCallback<CPF.Controls.TreeViewItemEventArgs> ItemDoubleClick { get; set; }
        [Parameter] public EventCallback Initialized { get; set; }

    }
}
