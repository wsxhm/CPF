//CPF自动生成.
            
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
    /// 包含可选项列表
    /// </summary>
    public partial class ListBox : Element<CPF.Controls.ListBox>
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
        /// 是否虚拟化UI，只支持StackPanel的虚拟化数据显示。初始化之前设置
        /// <summary>
        [Parameter] public bool? IsVirtualizing { get; set; }
        /// <summary>
        /// 返回CPF.Controls.ItemCollection类型，可以直接将数据源设置过来
        /// <summary>
        [Parameter] public IList Items { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public Thickness? Padding { get; set; }
        /// <summary>
        /// 获取或者设置当前选定的项的第一个索引
        /// <summary>
        [Parameter] public int? SelectedIndex { get; set; }
        /// <summary>
        /// 获取或设置通过使用 SelectedItem 而获取的 SelectedValuePath 的值。如果数据量大不建议用这个来设置，如果是多选的时候，类型是IEnumerable数据，可以遍历获取
        /// <summary>
        [Parameter] public object SelectedValue { get; set; }
        /// <summary>
        /// 获取或设置用于从 SelectedValue 获取 SelectedItem 的路径。
        /// <summary>
        [Parameter] public string SelectedValuePath { get; set; }
        /// <summary>
        /// 鼠标选中方式
        /// <summary>
        [Parameter] public SelectionMethod? SelectionMethod { get; set; }
        /// <summary>
        /// 选择行为，单选，多选方式
        /// <summary>
        [Parameter] public SelectionMode? SelectionMode { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public TextDecoration? TextDecoration { get; set; }
        /// <summary>
        /// 虚拟模式下元素使用方式
        /// <summary>
        [Parameter] public VirtualizationMode? VirtualizationMode { get; set; }
        [Parameter] public EventCallback<ListBoxItemMouseEventArgs> ItemMouseDown { get; set; }
        [Parameter] public EventCallback<ListBoxItemMouseEventArgs> ItemMouseUp { get; set; }
        [Parameter] public EventCallback<ListBoxItemEventArgs> ItemDoubleClick { get; set; }
        [Parameter] public EventCallback SelectionChanged { get; set; }
        [Parameter] public EventCallback Initialized { get; set; }
        /// <summary>
        /// 获取或设置 ItemsControl 中的交替项容器的数目，该控件可使交替容器具有唯一外观，通过附加数据AttachedExtenstions.AlternationIndex 读取循环的ID
        /// <summary>
        [Parameter] public EventCallback<uint> AlternationCountChanged { get; set; }
        /// <summary>
        /// 背景填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> BackgroundChanged { get; set; }
        /// <summary>
        /// 边框线条填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> BorderFillChanged { get; set; }
        /// <summary>
        /// 获取或设置线条类型
        /// <summary>
        [Parameter] public EventCallback<Stroke> BorderStrokeChanged { get; set; }
        /// <summary>
        /// 四周边框粗细
        /// <summary>
        [Parameter] public EventCallback<Thickness> BorderThicknessChanged { get; set; }
        /// <summary>
        /// 边框类型，BorderStroke和BorderThickness
        /// <summary>
        [Parameter] public EventCallback<BorderType> BorderTypeChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值表示将 Border 的角倒圆的程度。格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft
        /// <summary>
        [Parameter] public EventCallback<CornerRadius> CornerRadiusChanged { get; set; }
        /// <summary>
        /// 显示的数据字段或属性
        /// <summary>
        [Parameter] public EventCallback<string> DisplayMemberPathChanged { get; set; }
        /// <summary>
        /// 字体名
        /// <summary>
        [Parameter] public EventCallback<string> FontFamilyChanged { get; set; }
        /// <summary>
        /// 字体尺寸，点
        /// <summary>
        [Parameter] public EventCallback<float> FontSizeChanged { get; set; }
        /// <summary>
        /// 字体样式
        /// <summary>
        [Parameter] public EventCallback<FontStyles> FontStyleChanged { get; set; }
        /// <summary>
        /// 控件文字的填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> ForegroundChanged { get; set; }
        /// <summary>
        /// 是否虚拟化UI，只支持StackPanel的虚拟化数据显示。初始化之前设置
        /// <summary>
        [Parameter] public EventCallback<bool> IsVirtualizingChanged { get; set; }
        /// <summary>
        /// 返回CPF.Controls.ItemCollection类型，可以直接将数据源设置过来
        /// <summary>
        [Parameter] public EventCallback<IList> ItemsChanged { get; set; }
        /// <summary>
        /// 获取或设置描述 Thickness 及其子元素之间的空间量的 Border 值。格式：all或者left,top,right,bottom
        /// <summary>
        [Parameter] public EventCallback<Thickness> PaddingChanged { get; set; }
        /// <summary>
        /// 获取或者设置当前选定的项的第一个索引
        /// <summary>
        [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }
        /// <summary>
        /// 获取或设置通过使用 SelectedItem 而获取的 SelectedValuePath 的值。如果数据量大不建议用这个来设置，如果是多选的时候，类型是IEnumerable数据，可以遍历获取
        /// <summary>
        [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }
        /// <summary>
        /// 获取或设置用于从 SelectedValue 获取 SelectedItem 的路径。
        /// <summary>
        [Parameter] public EventCallback<string> SelectedValuePathChanged { get; set; }
        /// <summary>
        /// 鼠标选中方式
        /// <summary>
        [Parameter] public EventCallback<SelectionMethod> SelectionMethodChanged { get; set; }
        /// <summary>
        /// 选择行为，单选，多选方式
        /// <summary>
        [Parameter] public EventCallback<SelectionMode> SelectionModeChanged { get; set; }
        /// <summary>
        /// 表示一个文本修饰，它是可添加到文本的视觉装饰（如下划线）。字符串格式： overline/Underline/Strikethrough/none [width[,Solid/Dash/Dot/DashDot/DashDotDot]] [color]
        /// <summary>
        [Parameter] public EventCallback<TextDecoration> TextDecorationChanged { get; set; }
        /// <summary>
        /// 虚拟模式下元素使用方式
        /// <summary>
        [Parameter] public EventCallback<VirtualizationMode> VirtualizationModeChanged { get; set; }

    }
}
