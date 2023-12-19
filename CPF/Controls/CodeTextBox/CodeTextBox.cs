using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Platform;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CPF.Documents;

namespace CPF.Controls
{
    /// <summary>
    /// 代码编辑控件，可以支持大规模文本
    /// </summary>
    public class CodeTextBox : Control, IEditor
    {
        public CodeTextBox()
        {
            codeTextView = CreateTextBoxView();
            styles.CollectionChanged += Styles_CollectionChanged;
        }

        private void Styles_CollectionChanged(object sender, CollectionChangedEventArgs<CodeStyle> e)
        {
            if (e.Action == CollectionChangedAction.Add || e.Action == CollectionChangedAction.Replace)
            {
                e.NewItem.Owner = this;
                if (e.OldItem != null)
                {
                    e.OldItem.Owner = null;
                }
            }
            else if (e.Action == CollectionChangedAction.Replace)
            {
                e.OldItem.Owner = null;
            }
            RenderKeywords();
        }

        /// <summary>
        /// 是否启用输入法，主要描述的是中文这类输入法
        /// </summary>
        [Description("是否启用输入法，主要描述的是中文这类输入法")]
        [PropertyMetadata(true)]
        public bool IsInputMethodEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示文本编辑控件对于与该控件交互的用户是否是只读的
        /// </summary>
        [Description("获取或设置一个值，该值指示文本编辑控件对于与该控件交互的用户是否是只读的")]
        public bool IsReadOnly
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        ///// <summary>
        ///// 自动换行
        ///// </summary>
        //[UIPropertyMetadata(false, UIPropertyOptions.AffectsMeasure)]
        //[Description("自动换行")]
        //public bool WordWarp { get { return GetValue<bool>(); } set { SetValue(value); } }

        /// <summary>
        /// 如果按 Tab 键会在当前光标位置插入一个制表符，则为 true；如果按 Tab 键会将焦点移动到标记为制表位的下一个控件且不插入制表符，则为 false
        /// </summary>
        [PropertyMetadata(false)]
        [Description("如果按 Tab 键会在当前光标位置插入一个制表符，则为 true；如果按 Tab 键会将焦点移动到标记为制表位的下一个控件且不插入制表符，则为 false")]
        public bool AcceptsTab
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置用于绘制文本框的插入符号的画笔
        /// </summary>
        [Description("获取或设置用于绘制文本框的插入符号的画笔")]
        [PropertyMetadata(typeof(ViewFill), "#000")]
        public ViewFill CaretFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置会突出显示选定文本的画笔。
        /// </summary>
        [Description("获取或设置会突出显示选定文本的画笔。")]
        [UIPropertyMetadata(typeof(ViewFill), "153,201,239", UIPropertyOptions.AffectsRender)]
        public ViewFill SelectionFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，此值定义用于所选文本的画笔。
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        [Description("获取或设置一个值，此值定义用于所选文本的画笔。")]
        public ViewFill SelectionTextFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应显示水平 ScrollBar。
        /// </summary>
        [PropertyMetadata(ScrollBarVisibility.Auto)]
        [Description("获取或设置一个值，该值指示是否应显示水平 ScrollBar")]
        public ScrollBarVisibility HScrollBarVisibility
        {
            get { return GetValue<ScrollBarVisibility>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示是否应显示垂直 ScrollBar。
        /// </summary>
        [PropertyMetadata(ScrollBarVisibility.Auto)]
        [Description("获取或设置一个值，该值指示是否应显示垂直 ScrollBar。")]
        public ScrollBarVisibility VScrollBarVisibility
        {
            get { return GetValue<ScrollBarVisibility>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，是否显示行号
        /// </summary>
        [PropertyMetadata(true)]
        [Description("获取或设置一个值，是否显示行号")]
        public bool ShowLineNumber
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示文本编辑控件是否支持撤消功能
        /// </summary>
        [PropertyMetadata(false)]
        [Description("获取或设置一个值，该值指示文本编辑控件是否支持撤消功能")]
        public bool IsUndoEnabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置存储在撤消队列中的操作的数目。 默认值为-1, 表示撤消队列限制为可用的内存。
        /// </summary>
        [Description("获取或设置存储在撤消队列中的操作的数目。 默认值为-1, 表示撤消队列限制为可用的内存。")]
        [PropertyMetadata(-1)]
        public int UndoLimit
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        uint caretIndex;
        /// <summary>
        /// 光标位置，或者选中的开始位置
        /// </summary>
        [NotCpfProperty]
        public uint CaretIndex
        {
            get { return caretIndex; }
            set { caretIndex = value; }
        }
        uint selectionEnd;
        /// <summary>
        /// 选中的结束位置
        /// </summary>
        [NotCpfProperty]
        public uint SelectionEnd
        {
            get { return selectionEnd; }
            set { selectionEnd = value; }
        }
        /// <summary>
        /// 选中的文字
        /// </summary>
        [NotCpfProperty]
        public string SelectedText
        {
            get
            {
                if (selectionEnd != caretIndex)
                {
                    var start = (int)caretIndex;
                    var length = (int)(selectionEnd - caretIndex);

                    if (selectionEnd < caretIndex)
                    {
                        start = (int)selectionEnd;
                        length = (int)(caretIndex - selectionEnd);
                    }
                    if (length > 0 && start >= 0 && start < codeTextView.Text.Length && start + length <= codeTextView.Text.Length)
                    {
                        return codeTextView.Text.Substring(start, length);
                    }
                }
                return "";
            }
        }

        /// <summary>
        /// 文本内容
        /// </summary>
        [PropertyMetadata("")]
        public string Text
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 布局好的行
        /// </summary>
#if Net4
        public IList<TextLine> Lines
        {
            get { return codeTextView.Lines; }
        }
#else
        public IReadOnlyList<TextLine> Lines
        {
            get { return codeTextView.Lines; }
        }
#endif

        Collection<KeywordsStyle> keywordsStyles;
        /// <summary>
        /// 关键词样式集合
        /// </summary>
        [NotCpfProperty]
        public Collection<KeywordsStyle> KeywordsStyles
        {
            get
            {
                if (keywordsStyles == null)
                {
                    keywordsStyles = new Collection<KeywordsStyle>();
                    keywordsStyles.CollectionChanged += delegate
                    {
                        RenderKeywords();
                    };
                }
                return keywordsStyles;
            }
        }

        CancellationTokenSource Cancellation;
        private async void RenderKeywords()
        {
            if (Root == null)
            {
                render = true;
                return;
            }
            if (render)
            {
                render = false;
#if Net4
                await TaskEx.Delay(30);
#else
                await Task.Delay(30);
#endif
                render = true;
                if (Cancellation == null)
                {
                    Cancellation = new CancellationTokenSource();
                }
                else
                {
                    Cancellation.Cancel();
                    Cancellation = new CancellationTokenSource();
                }
                var ks = KeywordsStyles.ToArray();
#if Net4
                var styles = await TaskEx.Run(() => CodeTextView.RenderKeywords(Cancellation.Token, ks, codeTextView.Text));
#else
                var styles = await Task.Run(() => CodeTextView.RenderKeywords(Cancellation.Token, ks, codeTextView.Text));
#endif
                if (styles != null)
                {
                    codeTextView.styles = styles;
                    codeTextView.Invalidate();
                }
            }
        }

        bool render = true;

        Collection<CodeStyle> styles = new Collection<CodeStyle>();
        /// <summary>
        /// 样式
        /// </summary>
        [NotCpfProperty]
        public Collection<CodeStyle> Styles
        {
            get { return styles; }
        }

        /// <summary>
        /// 在文本元素中的内容改变时发生.
        /// </summary>
        public event EventHandler TextChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


        //模板定义
        protected override void InitializeComponent()
        {
            Children.Add(new Grid
            {
                Width = "100%",
                Height = "100%",
                ColumnDefinitions = { new ColumnDefinition { Width = "Auto" }, new ColumnDefinition { Width = "*" } },
                Children =
                {
                    new LineNumber{
                        Name="lineNumber",
                        PresenterFor=this,
                        Height="100%",
                        MarginRight=2,
                        Bindings =
                        {
                            {nameof(Visibility),nameof(ShowLineNumber),this,BindingMode.OneWay,(bool v)=>v? Visibility.Visible: Visibility.Collapsed },
                        }
                    },
                    {
                        new ScrollViewer
                        {
                            Width = "100%",
                            Height = "100%",
                            Name = "scrollViewer",
                            PresenterFor = this,
                            Content = TextView,
                            Bindings = { { nameof(ScrollViewer.HorizontalScrollBarVisibility), nameof(HScrollBarVisibility), this }, { nameof(ScrollViewer.VerticalScrollBarVisibility), nameof(VScrollBarVisibility), this } }
                        },1
                    }
                }
            });
        }

        ScrollViewer scrollViewer;
        LineNumber lineNumber;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            scrollViewer = FindPresenterByName<ScrollViewer>("scrollViewer");
            lineNumber = FindPresenterByName<LineNumber>("lineNumber");
            var contentPresenter = scrollViewer.FindPresenterByName<Border>("contentPresenter");
            if (contentPresenter != null)
            {
                contentPresenter.Bindings.Add(nameof(Padding), nameof(Padding), this, BindingMode.TwoWay);
            }
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();

            if (keywordsStyles != null && keywordsStyles.Count > 0)
            {
                RenderKeywords();
            }
        }

        [NotCpfProperty]
        public LineNumber LineNumber
        {
            get { return lineNumber; }
        }

        bool scroll = false;
        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            if (scroll && IsInitialized)
            {
                scroll = false;
                BeginInvoke(() =>
                {
                    var point = codeTextView.GetPostion(caretIndex, out float height);
                    //var scrollViewer = Find<ScrollViewer>().FirstOrDefault();
                    if (scrollViewer == null)
                    {
                        throw new Exception("ScrollViewer不存在");
                    }
                    if (scrollViewer.VerticalOffset >= point.Y + scrollViewer.VerticalOffset)
                    {
                        scrollViewer.VerticalOffset = point.Y + scrollViewer.VerticalOffset;
                        codeTextView.InvalidateArrange();
                    }
                    else if (scrollViewer.VerticalOffset < point.Y + scrollViewer.VerticalOffset + height - scrollViewer.ViewportHeight)
                    {
                        scrollViewer.VerticalOffset = point.Y + scrollViewer.VerticalOffset + height - scrollViewer.ViewportHeight;
                        codeTextView.InvalidateArrange();
                    }
                    if (scrollViewer.HorizontalOffset > point.X + scrollViewer.HorizontalOffset)
                    {
                        if (caretIndex == 0)
                        {
                            scrollViewer.HorizontalOffset = 0;
                        }
                        else
                        {
                            scrollViewer.HorizontalOffset = point.X + scrollViewer.HorizontalOffset;
                        }
                        codeTextView.InvalidateArrange();
                    }
                    else if (scrollViewer.HorizontalOffset < point.X + scrollViewer.HorizontalOffset - scrollViewer.ViewportWidth)
                    {
                        scrollViewer.HorizontalOffset = point.X + scrollViewer.HorizontalOffset - scrollViewer.ViewportWidth;
                        codeTextView.InvalidateArrange();
                    }
                });
            }
        }

        public void ScrollToCaret()
        {
            scroll = true;
            InvalidateArrange();
        }

        CodeTextView codeTextView;
        protected virtual CodeTextView TextView
        {
            get { return codeTextView; }
        }

        protected virtual CodeTextView CreateTextBoxView()
        {
            return new CodeTextView(this);
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        [PropertyChanged(nameof(FontSize))]
        void OnFontSize(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            codeTextView.InvalidateMeasure();
        }
        [PropertyChanged(nameof(Text))]
        void OnText(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            //if (newValue is string text)
            //{
            //    codeTextView.Text = text.Replace("\r\n", "\n");
            //}
            //else
            //{
            //    codeTextView.Text = "";
            //}
            codeTextView.Text = (string)newValue;
            codeTextView.InvalidateMeasure();
            RenderKeywords();
            if (IsUndoEnabled)
            {
                if (!isRedo)
                {
                    if (undoIndex + 1 < undoRedoStates.Count)
                    {
                        undoRedoStates.RemoveRange(undoIndex + 1, undoRedoStates.Count - undoIndex - 1);
                    }
                    var ul = UndoLimit;
                    if (ul > 0 && undoRedoStates.Count >= ul)
                    {
                        undoRedoStates.RemoveRange(0, undoRedoStates.Count - ul + 1);
                    }
                    undoRedoStates.Add(new UndoRedoState(codeTextView.Text, caretIndex));
                    undoIndex = undoRedoStates.Count - 1;
                }
            }
            //else
            //{
            //    undoRedoStates.Clear();
            //}
            isRedo = false;
            this.RaiseEvent(EventArgs.Empty, nameof(TextChanged));
        }

        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Text))
            {
                if (value is string text)
                {
                    value = text.Replace("\r\n", "\n");
                }
                else
                {
                    value = "";
                }
            }
            return base.OnSetValue(propertyName, ref value);
        }

        [PropertyChanged(nameof(IsUndoEnabled))]
        void OnIsUndoEnabled(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if ((bool)newValue)
            {
                undoRedoStates.Clear();
                undoRedoStates.Add(new UndoRedoState(codeTextView.Text, caretIndex));
                undoIndex = undoRedoStates.Count - 1;
            }
            else
            {
                undoIndex = 0;
                undoRedoStates.Clear();
            }
        }
        bool isRedo;
        List<UndoRedoState> undoRedoStates = new List<UndoRedoState>();
        int undoIndex;

        public void SelectAll()
        {
            selectionEnd = 0;
            caretIndex = (uint)codeTextView.Text.Length;
            Invalidate();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            if (IsReadOnly || e.Handled)
            {
                return;
            }
            if (caretIndex != selectionEnd)
            {
                RemoveRange();
            }
            if (codeTextView.Text.Length >= caretIndex)
            {
                //Text = codeTextView.Text.Insert((int)caretIndex, e.Text);
                var caretIndex_old = caretIndex;
                caretIndex = caretIndex + (uint)e.Text.Length;
                codeTextView.InsertText(caretIndex_old, e.Text);
                selectionEnd = caretIndex;
            }
            else
            {
                Text = codeTextView.Text + e.Text;
            }
            ScrollToCaret();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Keys.Up:
                        for (int i = 0; i < codeTextView.Lines.Count; i++)
                        {
                            var item = codeTextView.Lines[i];
                            if (item.Start <= caretIndex && item.Start + item.Count > caretIndex || i == codeTextView.Lines.Count - 1)
                            {
                                if (i > 0)
                                {
                                    var line = codeTextView.Lines[i - 1];
                                    caretIndex = (uint)Math.Min(line.Start + line.Count, caretIndex - item.Start + line.Start);
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd = caretIndex;
                        }
                        ScrollToCaret();
                        break;
                    case Keys.Down:
                        for (int i = 0; i < codeTextView.Lines.Count; i++)
                        {
                            var item = codeTextView.Lines[i];
                            if (item.Start <= caretIndex && item.Start + item.Count > caretIndex)
                            {
                                if (i < codeTextView.Lines.Count - 1)
                                {
                                    var line = codeTextView.Lines[i + 1];
                                    caretIndex = (uint)Math.Min(line.Start + line.Count, caretIndex - item.Start + line.Start);
                                    break;
                                }
                            }
                        }
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd = caretIndex;
                        }
                        ScrollToCaret();
                        break;
                    case Keys.Left:
                        if (caretIndex > 0)
                        {
                            CaretIndex--;
                        }
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd = caretIndex;
                        }
                        ScrollToCaret();
                        break;
                    case Keys.Right:
                        CaretIndex++;
                        if (caretIndex > codeTextView.Text.Length)
                        {
                            caretIndex = (uint)codeTextView.Text.Length;
                        }
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd = caretIndex;
                        }
                        ScrollToCaret();
                        break;
                    case Keys.Home:
                        for (int i = 0; i < codeTextView.Lines.Count; i++)
                        {
                            var item = codeTextView.Lines[i];
                            if (item.Start <= caretIndex && item.Start + item.Count >= caretIndex)
                            {
                                if (item.Start == 0)
                                {
                                    caretIndex = 0;
                                }
                                else
                                {
                                    caretIndex = (uint)item.Start + 1;
                                }
                                break;
                            }
                        }
                        selectionEnd = caretIndex;
                        ScrollToCaret();
                        break;
                    case Keys.End:
                        for (int i = 0; i < codeTextView.Lines.Count; i++)
                        {
                            var item = codeTextView.Lines[i];
                            if (item.Start <= caretIndex && item.Start + item.Count >= caretIndex)
                            {
                                caretIndex = (uint)(item.Start + item.Count);
                                break;
                            }
                        }
                        selectionEnd = caretIndex;
                        ScrollToCaret();
                        break;
                    case Keys.PageDown:
                        {
                            var size = codeTextView.ActualSize;
                            using (var font = new Font(FontFamily, FontSize, FontStyle))
                            {
                                var lineHeight = (float)Math.Round(font.LineHeight, 2);
                                var len = (int)Math.Ceiling(size.Height / lineHeight);

                                scrollViewer.PageDown();
                                for (int i = 0; i < codeTextView.Lines.Count; i++)
                                {
                                    var item = codeTextView.Lines[i];
                                    if (item.Start <= caretIndex && item.Start + item.Count >= caretIndex)
                                    {
                                        var line = codeTextView.Lines[Math.Min(i + len, codeTextView.Lines.Count - 1)];
                                        caretIndex = (uint)Math.Min(line.Start + line.Count, caretIndex - item.Start + line.Start);
                                        break;
                                    }
                                }
                                selectionEnd = caretIndex;
                            }
                        }
                        break;
                    case Keys.PageUp:
                        {
                            var size = codeTextView.ActualSize;
                            using (var font = new Font(FontFamily, FontSize, FontStyle))
                            {
                                var lineHeight = (float)Math.Round(font.LineHeight, 2);
                                var len = (int)Math.Ceiling(size.Height / lineHeight);

                                scrollViewer.PageUp();
                                for (int i = 0; i < codeTextView.Lines.Count; i++)
                                {
                                    var item = codeTextView.Lines[i];
                                    if (item.Start <= caretIndex && item.Start + item.Count >= caretIndex)
                                    {
                                        var line = codeTextView.Lines[Math.Max(i - len, 0)];
                                        caretIndex = (uint)Math.Min(line.Start + line.Count, caretIndex - item.Start + line.Start);
                                        break;
                                    }
                                }
                                selectionEnd = caretIndex;
                            }
                        }
                        break;
                    default:
                        break;
                }
                var hotkey = Application.GetRuntimePlatform().Hotkey(new KeyGesture(e.Key, e.Modifiers));
                switch (hotkey)
                {
                    case PlatformHotkey.None:
                        break;
                    case PlatformHotkey.SelectAll:
                        selectionEnd = 0;
                        caretIndex = (uint)codeTextView.Text.Length;
                        ScrollToCaret();
                        break;
                    case PlatformHotkey.Copy:
                        Copy();
                        break;
                    case PlatformHotkey.Cut:
                        var text = SelectedText;
                        if (!IsReadOnly)
                        {
                            RemoveRange();
                        }
                        if (!string.IsNullOrEmpty(text))
                        {
                            Clipboard.SetData((DataFormat.Text, text));
                        }
                        break;
                    case PlatformHotkey.Paste:
                        if (!IsReadOnly)
                        {
                            Paste();
                        }
                        break;
                    case PlatformHotkey.Undo:
                        if (!IsReadOnly && IsUndoEnabled)
                        {
                            if (undoRedoStates.Count > 1 && undoIndex > 0)
                            {
                                isRedo = true;
                                undoIndex--;
                                Text = undoRedoStates[undoIndex].Text;
                                caretIndex = undoRedoStates[undoIndex].CaretPosition;
                                selectionEnd = caretIndex;
                                //isRedo = false;
                            }
                        }
                        break;
                    case PlatformHotkey.Redo:
                        if (!IsReadOnly && IsUndoEnabled)
                        {
                            if (undoIndex < undoRedoStates.Count - 1)
                            {
                                isRedo = true;
                                undoIndex++;
                                Text = undoRedoStates[undoIndex].Text;
                                caretIndex = undoRedoStates[undoIndex].CaretPosition;
                                selectionEnd = caretIndex;
                                //isRedo = false;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (IsReadOnly)
                {
                    return;
                }
                switch (e.Key)
                {
                    case Keys.Delete:
                        if (caretIndex != selectionEnd)
                        {
                            RemoveRange();
                        }
                        else
                        {
                            if (caretIndex != codeTextView.Text.Length)
                            {
                                //Text = codeTextView.Text.Remove((int)(caretIndex), 1);
                                codeTextView.RemoveText(caretIndex, 1);
                            }
                        }

                        selectionEnd = caretIndex;
                        ScrollToCaret();
                        break;
                    case Keys.Back:
                        if (caretIndex != selectionEnd)
                        {
                            RemoveRange();
                        }
                        else
                        {
                            if (caretIndex != 0)
                            {
                                //Text = codeTextView.Text.Remove((int)(caretIndex - 1), 1);
                                codeTextView.RemoveText(caretIndex - 1, 1);
                                caretIndex = caretIndex - 1;
                            }
                        }

                        selectionEnd = caretIndex;
                        ScrollToCaret();
                        break;
                    case Keys.Enter:
                        if (caretIndex != selectionEnd)
                        {
                            RemoveRange();
                        }
                        //Text = codeTextView.Text.Insert((int)caretIndex, "\n");
                        codeTextView.InsertText(caretIndex, "\n");
                        caretIndex++;
                        selectionEnd = caretIndex;
                        ScrollToCaret();
                        break;
                    case Keys.Tab:
                        if (AcceptsTab)
                        {
                            if (caretIndex != selectionEnd)
                            {
                                RemoveRange();
                            }
                            //Text = codeTextView.Text.Insert((int)caretIndex, "\t");
                            codeTextView.InsertText(caretIndex, "\t");
                            caretIndex++;
                            selectionEnd = caretIndex;
                            e.Handled = true;
                            ScrollToCaret();
                        }
                        break;
                }

            }

        }

        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            if (e.Handled)
            {
                return;
            }
            SelectWord();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Handled)
            {
                return;
            }
            if (e.MouseButton == MouseButton.Left && Root.InputManager.KeyboardDevice.Modifiers == (InputModifiers.Control | InputModifiers.LeftMouseButton))
            {
                SelectWord();
            }
        }

        private void SelectWord()
        {
            var items = codeTextView.Text;
            char c = (char)0;
            int index = -1;

            if (caretIndex < items.Length)
            {
                c = items[(int)caretIndex];
                index = (int)caretIndex;
            }
            else if (items.Length > 0)
            {
                c = items[items.Length - 1];
                index = items.Length - 1;
            }
            if (c != 0)
            {
                int start = index;
                int lenght = 1;
                int type = 3;//字符类型0：数字，1：字母，2：文字，3：其他
                {
                    if (TextBox.IsDigit(c))
                    {
                        type = 0;
                    }
                    else if (TextBox.IsLetter(c))
                    {
                        type = 1;
                    }
                    else if (TextBox.IsAsianCharecter(c))
                    {
                        type = 2;
                    }
                    if (index > 0)
                    {
                        for (int i = index - 1; i >= 0; i--)//往前找
                        {
                            var ch = items[i];
                            if (CheckChar(type, ch))
                            {
                                lenght++;
                                start = i;
                                continue;
                            }
                            break;
                        }
                    }
                    if (index < items.Length)//往后找
                    {
                        for (int i = index + 1; i < items.Length; i++)
                        {
                            var ch = items[i];
                            if (CheckChar(type, ch))
                            {
                                lenght++;
                                continue;
                            }
                            break;
                        }
                    }

                    CaretIndex = (uint)(start + lenght);
                    //this.CaretIndex[0] = (uint)start;
                    this.selectionEnd = (uint)start;
                }

            }
        }

        private bool CheckChar(int type, char c)
        {
            if (type == 0 && TextBox.IsDigit(c))
            {
                return true;
            }
            if (type == 1 && TextBox.IsLetter(c))
            {
                return true;
            }
            if (type == 2 && TextBox.IsAsianCharecter(c))
            {
                return true;
            }
            return false;
        }

        private void RemoveRange()
        {
            if (caretIndex > selectionEnd)
            {
                //Text = codeTextView.Text.Remove((int)(selectionEnd), (int)(caretIndex - selectionEnd));
                codeTextView.RemoveText((selectionEnd), (int)(caretIndex - selectionEnd));
                caretIndex = selectionEnd;
            }
            else
            {
                //Text = codeTextView.Text.Remove((int)(caretIndex), (int)(selectionEnd - caretIndex));
                codeTextView.RemoveText((caretIndex), (int)(selectionEnd - caretIndex));
                selectionEnd = caretIndex;
            }
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        public virtual void Paste()
        {
            if (Clipboard.Contains(DataFormat.Text))
            {
                if (caretIndex != selectionEnd)
                {
                    RemoveRange();
                }
                var text = Clipboard.GetData(DataFormat.Text).ToString();
                if (codeTextView.Text.Length >= caretIndex)
                {
                    //Text = codeTextView.Text.Insert((int)caretIndex, text);
                    codeTextView.InsertText(caretIndex, text);
                    caretIndex = caretIndex + (uint)text.Length;
                    selectionEnd = caretIndex;
                }
                else
                {
                    Text = codeTextView.Text + text;
                }
                ScrollToCaret();
            }
        }

        public virtual void Copy()
        {
            if (selectionEnd != caretIndex)
            {
                Clipboard.SetData((DataFormat.Text, SelectedText));
            }
        }
        /// <summary>
        /// 通过鼠标坐标获取光标索引位置
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public uint GetCareIndex(Point point)
        {
            point.Offset(-Padding.Left, -Padding.Top);
            return codeTextView.GetCareIndex(point);
        }

        /// <summary>
        /// 获取索引处坐标，如果位置不在可视范围内，那X值将不是精确值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="lineHeight"></param>
        /// <returns></returns>
        public Point GetPostion(uint index, out float lineHeight)
        {
            var point = codeTextView.GetPostion(index, out lineHeight);
            point.Offset(Padding.Left, Padding.Top);
            return point;
        }


        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
            //overridePropertys.Override(nameof(Cursor), new UIPropertyMetadataAttribute(typeof(Cursor), nameof(Cursors.Ibeam), true));
            overridePropertys.Override(nameof(Padding), new UIPropertyMetadataAttribute(new Thickness(1), UIPropertyOptions.AffectsMeasure));
        }

        class UndoRedoState
        {
            public string Text { get; }
            public uint CaretPosition { get; }

            public UndoRedoState(string text, uint caretPosition)
            {
                Text = text;
                CaretPosition = caretPosition;
            }

        }
    }
}
