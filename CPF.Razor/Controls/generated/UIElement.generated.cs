//CPF自动生成.
            
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
        /// 应用到元素上的类名，多个类用,分割
        /// <summary>
        [Parameter] public string Classes { get; set; }
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
        [Parameter] public EventCallback<UIElementAddedEventArgs> UIElementAdded { get; set; }
        [Parameter] public EventCallback<UIElementRemovedEventArgs> UIElementRemoved { get; set; }
        [Parameter] public EventCallback DesiredSizeChanged { get; set; }
        [Parameter] public EventCallback<MouseButtonEventArgs> PreviewMouseDown { get; set; }
        [Parameter] public EventCallback<MouseButtonEventArgs> PreviewMouseUp { get; set; }
        [Parameter] public EventCallback<MouseButtonEventArgs> MouseDown { get; set; }
        [Parameter] public EventCallback<MouseButtonEventArgs> MouseUp { get; set; }
        [Parameter] public EventCallback<RoutedEventArgs> DoubleClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> MouseMove { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> MouseEnter { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> MouseLeave { get; set; }
        [Parameter] public EventCallback<TouchEventArgs> TouchUp { get; set; }
        [Parameter] public EventCallback<TouchEventArgs> TouchDown { get; set; }
        [Parameter] public EventCallback<TouchEventArgs> TouchMove { get; set; }
        [Parameter] public EventCallback<MouseWheelEventArgs> MouseWheel { get; set; }
        [Parameter] public EventCallback<KeyEventArgs> KeyDown { get; set; }
        [Parameter] public EventCallback<KeyEventArgs> KeyUp { get; set; }
        [Parameter] public EventCallback<TextInputEventArgs> TextInput { get; set; }
        [Parameter] public EventCallback<GotFocusEventArgs> GotFocus { get; set; }
        [Parameter] public EventCallback<RoutedEventArgs> LostFocus { get; set; }
        [Parameter] public EventCallback<RoutedEventArgs> LayoutUpdated { get; set; }
        [Parameter] public EventCallback Disposed { get; set; }
        [Parameter] public EventCallback<RoutedEventArgs> ToolTipOpening { get; set; }
        [Parameter] public EventCallback<DragEventArgs> DragEnter { get; set; }
        [Parameter] public EventCallback DragLeave { get; set; }
        [Parameter] public EventCallback<DragEventArgs> DragOver { get; set; }
        [Parameter] public EventCallback<DragEventArgs> Drop { get; set; }
        [Parameter] public EventCallback<CPFPropertyChangedEventArgs> PropertyChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示此元素能否用作拖放操作的目标。
        /// <summary>
        [Parameter] public EventCallback<bool> AllowDropChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否剪切此元素的内容(或来自此元素的子元素的内容)使其适合包含元素的大小。这是一个依赖项属性。
        /// <summary>
        [Parameter] public EventCallback<bool> ClipToBoundsChanged { get; set; }
        /// <summary>
        /// 绑定的命令上下文
        /// <summary>
        [Parameter] public EventCallback<object> CommandContextChanged { get; set; }
        /// <summary>
        /// 右键菜单
        /// <summary>
        [Parameter] public EventCallback<ContextMenu> ContextMenuChanged { get; set; }
        /// <summary>
        /// 光标
        /// <summary>
        [Parameter] public EventCallback<Cursor> CursorChanged { get; set; }
        /// <summary>
        /// 绑定的数据上下文
        /// <summary>
        [Parameter] public EventCallback<object> DataContextChanged { get; set; }
        /// <summary>
        /// 位图特效
        /// <summary>
        [Parameter] public EventCallback<Effect> EffectChanged { get; set; }
        /// <summary>
        /// 是否可以获取焦点
        /// <summary>
        [Parameter] public EventCallback<bool> FocusableChanged { get; set; }
        /// <summary>
        /// 按tab键切换焦点显示的聚焦框填充
        /// <summary>
        [Parameter] public EventCallback<ViewFill> FocusFrameFillChanged { get; set; }
        /// <summary>
        /// 聚焦框到元素边缘距离
        /// <summary>
        [Parameter] public EventCallback<Thickness> FocusFramePaddingChanged { get; set; }
        /// <summary>
        /// 按tab键切换焦点显示的聚焦框
        /// <summary>
        [Parameter] public EventCallback<Stroke> FocusFrameStrokeChanged { get; set; }
        [Parameter] public EventCallback<FloatField> HeightChanged { get; set; }
        /// <summary>
        /// 图形抗锯齿
        /// <summary>
        [Parameter] public EventCallback<bool> IsAntiAliasChanged { get; set; }
        /// <summary>
        /// 是否启用
        /// <summary>
        [Parameter] public EventCallback<bool> IsEnabledChanged { get; set; }
        /// <summary>
        /// 是否可以通过鼠标点击到
        /// <summary>
        [Parameter] public EventCallback<bool> IsHitTestVisibleChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MarginBottomChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MarginLeftChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MarginRightChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MarginTopChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MaxHeightChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MaxWidthChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MinHeightChanged { get; set; }
        [Parameter] public EventCallback<FloatField> MinWidthChanged { get; set; }
        /// <summary>
        /// 元素名称
        /// <summary>
        [Parameter] public EventCallback<string> NameChanged { get; set; }
        /// <summary>
        /// 当添加触发器时并且触发器有设置动画，如果满足条件是否播放动画
        /// <summary>
        [Parameter] public EventCallback<bool> PlayAnimationOnAddTriggerChanged { get; set; }
        /// <summary>
        /// 渲染变换
        /// <summary>
        [Parameter] public EventCallback<Transform> RenderTransformChanged { get; set; }
        /// <summary>
        /// 渲染原点
        /// <summary>
        [Parameter] public EventCallback<PointField> RenderTransformOriginChanged { get; set; }
        /// <summary>
        /// tab键切换元素焦点时候的顺序
        /// <summary>
        [Parameter] public EventCallback<int> TabIndexChanged { get; set; }
        /// <summary>
        /// 与控件关联的用户自定义数据
        /// <summary>
        [Parameter] public EventCallback<object> TagChanged { get; set; }
        /// <summary>
        /// 获取或设置在用户界面 (UI) 中为此元素显示的工具提示对象
        /// <summary>
        [Parameter] public EventCallback<object> ToolTipChanged { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应向此元素的大小和位置布局应用布局舍入。
        /// <summary>
        [Parameter] public EventCallback<bool> UseLayoutRoundingChanged { get; set; }
        /// <summary>
        /// UI元素可见性
        /// <summary>
        [Parameter] public EventCallback<Visibility> VisibilityChanged { get; set; }
        [Parameter] public EventCallback<FloatField> WidthChanged { get; set; }
        /// <summary>
        /// Z轴
        /// <summary>
        [Parameter] public EventCallback<int> ZIndexChanged { get; set; }

    }
}
