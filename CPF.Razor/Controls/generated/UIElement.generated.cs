// CPF自动生成.
            
using CPF;
using CPF.Controls;
using CPF.Drawing;
using CPF.Effects;
using CPF.Input;
using CPF.Razor;
using CPF.Shapes;
using Microsoft.AspNetCore.Components;

namespace CPF.Razor.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Element<T> 
    {
        
        /// <summary>
        /// 获取或设置一个值，该值指示此元素能否用作拖放操作的目标。
        /// <summary>
        [Parameter] public bool? AllowDrop { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否剪切此元素的内容(或来自此元素的子元素的内容)使其适合包含元素的大小。这是一个依赖项属性。
        /// <summary>
        [Parameter] public bool? ClipToBounds { get; set; }
        /// <summary>
        /// 绑定的命令上下文
        /// <summary>
        [Parameter] public object CommandContext { get; set; }
        /// <summary>
        /// 右键菜单
        /// <summary>
        [Parameter] public ContextMenu ContextMenu { get; set; }
        /// <summary>
        /// 光标
        /// <summary>
        [Parameter] public Cursor Cursor { get; set; }
        /// <summary>
        /// 绑定的数据上下文
        /// <summary>
        [Parameter] public object DataContext { get; set; }
        /// <summary>
        /// 位图特效
        /// <summary>
        [Parameter] public Effect Effect { get; set; }
        /// <summary>
        /// 是否可以获取焦点
        /// <summary>
        [Parameter] public bool? Focusable { get; set; }
        /// <summary>
        /// 按tab键切换焦点显示的聚焦框填充
        /// <summary>
        [Parameter] public string FocusFrameFill { get; set; }
        /// <summary>
        /// 聚焦框到元素边缘距离
        /// <summary>
        [Parameter] public Thickness? FocusFramePadding { get; set; }
        /// <summary>
        /// 按tab键切换焦点显示的聚焦框
        /// <summary>
        [Parameter] public Stroke? FocusFrameStroke { get; set; }
        [Parameter] public FloatField? Height { get; set; }
        /// <summary>
        /// 图形抗锯齿
        /// <summary>
        [Parameter] public bool? IsAntiAlias { get; set; }
        /// <summary>
        /// 是否启用
        /// <summary>
        [Parameter] public bool? IsEnabled { get; set; }
        /// <summary>
        /// 是否可以通过鼠标点击到
        /// <summary>
        [Parameter] public bool? IsHitTestVisible { get; set; }
        [Parameter] public FloatField? MarginBottom { get; set; }
        [Parameter] public FloatField? MarginLeft { get; set; }
        [Parameter] public FloatField? MarginRight { get; set; }
        [Parameter] public FloatField? MarginTop { get; set; }
        [Parameter] public FloatField? MaxHeight { get; set; }
        [Parameter] public FloatField? MaxWidth { get; set; }
        [Parameter] public FloatField? MinHeight { get; set; }
        [Parameter] public FloatField? MinWidth { get; set; }
        /// <summary>
        /// 元素名称
        /// <summary>
        [Parameter] public string Name { get; set; }
        /// <summary>
        /// 当添加触发器时并且触发器有设置动画，如果满足条件是否播放动画
        /// <summary>
        [Parameter] public bool? PlayAnimationOnAddTrigger { get; set; }
        /// <summary>
        /// 渲染变换
        /// <summary>
        [Parameter] public Transform RenderTransform { get; set; }
        /// <summary>
        /// 渲染原点
        /// <summary>
        [Parameter] public PointField? RenderTransformOrigin { get; set; }
        /// <summary>
        /// tab键切换元素焦点时候的顺序
        /// <summary>
        [Parameter] public int? TabIndex { get; set; }
        /// <summary>
        /// 与控件关联的用户自定义数据
        /// <summary>
        [Parameter] public object Tag { get; set; }
        /// <summary>
        /// 获取或设置在用户界面 (UI) 中为此元素显示的工具提示对象
        /// <summary>
        [Parameter] public object ToolTip { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应向此元素的大小和位置布局应用布局舍入。
        /// <summary>
        [Parameter] public bool? UseLayoutRounding { get; set; }
        /// <summary>
        /// UI元素可见性
        /// <summary>
        [Parameter] public Visibility? Visibility { get; set; }
        [Parameter] public FloatField? Width { get; set; }
        /// <summary>
        /// Z轴
        /// <summary>
        [Parameter] public int? ZIndex { get; set; }
        [Parameter] public EventCallback<CPF.UIElementAddedEventArgs> UIElementAdded { get; set; }
        [Parameter] public EventCallback<CPF.UIElementRemovedEventArgs> UIElementRemoved { get; set; }
        [Parameter] public EventCallback DesiredSizeChanged { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseButtonEventArgs> PreviewMouseDown { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseButtonEventArgs> PreviewMouseUp { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseButtonEventArgs> MouseDown { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseButtonEventArgs> MouseUp { get; set; }
        [Parameter] public EventCallback<CPF.RoutedEventArgs> DoubleClick { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseEventArgs> MouseMove { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseEventArgs> MouseEnter { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseEventArgs> MouseLeave { get; set; }
        [Parameter] public EventCallback<CPF.Input.TouchEventArgs> TouchUp { get; set; }
        [Parameter] public EventCallback<CPF.Input.TouchEventArgs> TouchDown { get; set; }
        [Parameter] public EventCallback<CPF.Input.TouchEventArgs> TouchMove { get; set; }
        [Parameter] public EventCallback<CPF.Input.MouseWheelEventArgs> MouseWheel { get; set; }
        [Parameter] public EventCallback<CPF.Input.KeyEventArgs> KeyDown { get; set; }
        [Parameter] public EventCallback<CPF.Input.KeyEventArgs> KeyUp { get; set; }
        [Parameter] public EventCallback<CPF.Input.TextInputEventArgs> TextInput { get; set; }
        [Parameter] public EventCallback<CPF.Input.GotFocusEventArgs> GotFocus { get; set; }
        [Parameter] public EventCallback<CPF.RoutedEventArgs> LostFocus { get; set; }
        [Parameter] public EventCallback<CPF.RoutedEventArgs> LayoutUpdated { get; set; }
        [Parameter] public EventCallback Disposed { get; set; }
        [Parameter] public EventCallback<CPF.RoutedEventArgs> ToolTipOpening { get; set; }
        [Parameter] public EventCallback<CPF.Input.DragEventArgs> DragEnter { get; set; }
        [Parameter] public EventCallback DragLeave { get; set; }
        [Parameter] public EventCallback<CPF.Input.DragEventArgs> DragOver { get; set; }
        [Parameter] public EventCallback<CPF.Input.DragEventArgs> Drop { get; set; }
        [Parameter] public EventCallback<CPF.CPFPropertyChangedEventArgs> PropertyChanged { get; set; }

    }
}
