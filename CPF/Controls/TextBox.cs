using System;
using System.Collections.Generic;
using System.Text;
using CPF;
using CPF.Documents;
using CPF.Drawing;
using System.Linq;
using System.Globalization;
using CPF.Input;
using System.IO;
using System.Text.RegularExpressions;
using CPF.Platform;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示一个控件，该控件可用于显示或编辑无格式文本。
    /// </summary>
    [Description("表示一个控件，该控件可用于显示或编辑无格式文本。")]
    [DefaultProperty(nameof(TextBox.Text))]
    public class TextBox : Control, IDocumentStyle, IEditor
    {
        public TextBox()
        {
            selectionEnd.CollectionChanged += SelectionEnd_CollectionChanged;
            caretIndex.CollectionChanged += CaretIndex_CollectionChanged;

            view = CreateTextBoxView();
            view.Cursor = Cursors.Ibeam;

            document = TextBoxView.Document;
            _ = this[nameof(TextAlignment)] == document[nameof(TextAlignment)];
            _ = this[nameof(WordWarp)] == document[nameof(WordWarp)];
            view[nameof(Width)] = (this, nameof(WordWarp), a => (bool)a ? (FloatField)"100%" : (FloatField)"auto");
            document.Children.CollectionChanged += Children_CollectionChanged;
        }


        private void CaretIndex_CollectionChanged(object sender, CollectionChangedEventArgs<uint> e)
        {
            Invalidate();
        }

        private void SelectionEnd_CollectionChanged(object sender, CollectionChangedEventArgs<uint> e)
        {
            Invalidate();
        }

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        [Description("文本对齐方式")]
        [PropertyMetadata(TextAlignment.Left)]
        public TextAlignment TextAlignment
        {
            get { return GetValue<TextAlignment>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示在用户按 ENTER 键时文本编辑控件如何响应。如果按 Enter 键会在当前光标位置插入一个新行，则为 true；否则将忽略 Enter 键
        /// </summary>
        [Description("获取或设置一个值，该值指示在用户按 ENTER 键时文本编辑控件如何响应。如果按 Enter 键会在当前光标位置插入一个新行，则为 true；否则将忽略 Enter 键")]
        [PropertyMetadata(true)]
        public bool AcceptsReturn
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
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
        [PropertyMetadata(typeof(ViewFill), "153,201,239")]
        public ViewFill SelectionFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 获取或设置一个值，此值定义用于所选文本的画笔。
        /// </summary>
        //[PropertyMetadata(typeof(ViewFill), "#fff")]
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
        /// 密码模式的代替字符
        /// </summary>
        [Description("密码模式的代替字符")]
        public char PasswordChar
        {
            get { return GetValue<char>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 最大长度，为0的时候不限
        /// </summary>
        [Description("最大长度，为0的时候不限")]
        public uint MaxLength
        {
            get { return GetValue<uint>(); }
            set { SetValue(value); }
        }

        Collection<uint> selectionEnd = new Collection<uint>();
        /// <summary>
        /// 选中的结束位置
        /// </summary>
        [NotCpfProperty]
        public IList<uint> SelectionEnd
        {
            get { return selectionEnd; }
        }

        Collection<uint> caretIndex = new Collection<uint>();
        /// <summary>
        /// 光标位置，或者选中的开始位置
        /// </summary>
        [NotCpfProperty]
        public IList<uint> CaretIndex
        {
            get { return caretIndex; }
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

        /// <summary>
        /// 是否允许粘贴图片
        /// </summary>
        [Description("是否允许粘贴图片")]
        [PropertyMetadata(false)]
        public bool IsAllowPasteImage
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
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
        /// 自动换行
        /// </summary>
        [PropertyMetadata(false)]
        [Description("自动换行")]
        public bool WordWarp { get { return GetValue<bool>(); } set { SetValue(value); } }

        bool render = true;
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
                        if (render)
                        {
                            render = false;
                            this.BeginInvoke(() =>
                            {
                                RenderKeywords();
                                render = true;
                            });
                        }
                    };
                }
                return keywordsStyles;
            }
        }
        /// <summary>
        /// 样式
        /// </summary>
        [NotCpfProperty]
        public Collection<DocumentStyle> Styles
        {
            get { return Document.Styles; }
        }

        /// <summary>
        /// 在文本元素中的内容改变时发生.
        /// </summary>
        public event EventHandler TextChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        Document document;
        /// <summary>
        /// 文档对象，用于描述复杂内容
        /// </summary>
        [NotCpfProperty]
        public Document Document
        {
            get
            {
                return document;
            }
        }
        /// <summary>
        /// 末尾加一段文字
        /// </summary>
        /// <param name="text"></param>
        public void AppentText(string text)
        {
            Document.Add(text);
        }
        /// <summary>
        /// 插入文字，index小于0则是末尾加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="styleId"></param>
        public void InsertText(string text, int index = -1, short styleId = -1)
        {
            if (index < 0)
            {
                Document.Add(text, styleId);
            }
            else
            {
                Document.InsertText(index, text, styleId);
            }
        }
        /// <summary>
        /// 插入图片，index小于0则是末尾加
        /// </summary>
        /// <param name="img"></param>
        /// <param name="index"></param>
        public void InsertImage(object img, int index = -1)
        {
            if (index < 0)
            {
                Document.Children.Add(CreatePictureBox(img));
            }
            else
            {
                Document.Children.Insert(index, CreatePictureBox(img));
            }
        }

        protected virtual ITextBoxView CreateTextBoxView()
        {
            return new TextBoxView(this);
        }
        /// <summary>
        /// 内部TextBoxView
        /// </summary>
        public ITextBoxView TextBoxView
        {
            get
            {
                return view;
            }
        }
        List<UndoRedoState> undoRedoStates = new List<UndoRedoState>();
        int undoIndex = 0;
        bool isRedo;

        bool ownerSet;
        bool textChanged = true;
        private void Children_CollectionChanged(object sender, CollectionChangedEventArgs<IDocumentElement> e)
        {
            if (textChanged)
            {
                textChanged = false;
                this.BeginInvoke(() =>
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in document.Children)
                    {
                        if (item is DocumentChar c)
                        {
                            sb.Append(c.Char);
                        }
                        else if (item is UTF32Text u)
                        {
                            sb.Append(u.Text);
                        }
                    }
                    ownerSet = true;
                    if (PasswordChar == 0)
                    {
                        Text = sb.ToString();
                    }
                    ownerSet = false;
                    OnTextChanged(EventArgs.Empty);
                    textChanged = true;
                    RenderKeywords();
                    if (!isRedo && IsUndoEnabled)
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
                        undoRedoStates.Add(new UndoRedoState(document.Children.ToArray(), caretIndex.Count == 0 ? 0 : caretIndex[0]));
                        undoIndex = undoRedoStates.Count - 1;
                    }
                    isRedo = false;
                });
            }
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            this.RaiseEvent(e, nameof(TextChanged));
        }

        protected override void InitializeComponent()
        {
            this.Children.Add(new ScrollViewer
            {
                Width = "100%",
                Height = "100%",
                Name = "scrollViewer",
                PresenterFor = this,
                Content = TextBoxView,
                Bindings = { { nameof(ScrollViewer.HorizontalScrollBarVisibility), nameof(TextBox.HScrollBarVisibility), 1 }, { nameof(ScrollViewer.VerticalScrollBarVisibility), nameof(TextBox.VScrollBarVisibility), 1 } }
            });
        }

        ScrollViewer scrollViewer;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (view == null)
            {
                view = TextBoxView;
            }
            scrollViewer = FindPresenterByName<ScrollViewer>("scrollViewer");
            var contentPresenter = scrollViewer.FindPresenterByName<Border>("contentPresenter");
            if (contentPresenter != null)
            {
                //Bindings.Add(nameof(Padding), nameof(Padding), contentPresenter, BindingMode.TwoWay);
                contentPresenter.Bindings.Add(nameof(Padding), nameof(Padding), this, BindingMode.TwoWay);
            }
        }

        ITextBoxView view;
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
            overridePropertys.Override(nameof(Cursor), new UIPropertyMetadataAttribute(typeof(Cursor), nameof(Cursors.Ibeam), true));
        }

        protected override object OnGetDefaultValue(PropertyMetadataAttribute pm)
        {
            if (pm.PropertyName == nameof(IsInputMethodEnabled))
            {
                return PasswordChar == 0;
            }
            return base.OnGetDefaultValue(pm);
        }

        [PropertyChanged(nameof(IsReadOnly))]
        void RegisterIsReadOnly(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!(bool)newValue && document != null)
            {
                if (document.Children.Any(a => a is IDocumentContainer))
                {
                    throw new Exception("编辑模式下不能加元素容器");
                }
            }
        }
        [PropertyChanged(nameof(IsKeyboardFocusWithin))]
        void OnFocus(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if ((bool)newValue && !IsInputMethodEnabled)
            {
                Root.ViewImpl.SetIMEEnable(false);
            }
        }
        [PropertyChanged(nameof(IsInputMethodEnabled))]
        void OnIsInputMethodEnabled(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (Root && IsKeyboardFocusWithin)
            {
                Root.ViewImpl.SetIMEEnable((bool)newValue);
            }
        }

        [PropertyChanged(nameof(Text))]
        void RegisterText(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (!ownerSet)
            {
                if (newValue == null)
                {
                    throw new Exception("Text不能为null");
                }
                Document.Children.Clear();
                if (PasswordChar == 0)
                {
                    Document.Add((string)newValue);
                }
                else
                {
                    var cha = PasswordChar.ToString();
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < ((string)newValue).Length; i++)
                    {
                        stringBuilder.Append(cha);
                    }
                    Document.Add(stringBuilder.ToString());
                }
            }
        }

        [PropertyChanged(nameof(PasswordChar))]
        void RegisterPasswordChar(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var newv = (char)newValue;
            if (!string.IsNullOrEmpty(Text))
            {
                Document.Children.Clear();
                if (newv != 0)
                {
                    for (int i = 0; i < Text.Length; i++)
                    {
                        Document.Children.Add(new DocumentChar(newv));
                    }
                }
                else
                {
                    Document.Add(Text);
                }
            }
        }

        [PropertyChanged(nameof(IsUndoEnabled))]
        void RegisterIsUndoEnabled(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if ((bool)newValue)
            {
                undoRedoStates.Clear();
                undoRedoStates.Add(new UndoRedoState(document.Children.ToArray(), caretIndex.Count == 0 ? 0 : caretIndex[0]));
                undoIndex = undoRedoStates.Count - 1;
            }
            else
            {
                undoIndex = 0;
                undoRedoStates.Clear();
            }
        }
        [PropertyChanged(nameof(FontSize))]
        void OnFontSize(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            view.Document.IsMeasureValid = false;
            view.InvalidateMeasure();
        }

        //[PropertyChanged(nameof(WordWarp))]
        //void OnWordWarp(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        //{
        //    if ((bool)newValue && scrollViewer)
        //    {
        //        //document.MaxWidth = scrollViewer.ViewportWidth;
        //        TextBoxView.Width = "100%";
        //    }
        //    else
        //    {
        //        //document.MaxWidth = "auto";
        //        TextBoxView.Width = "auto";
        //    }
        //    //document.InvalidateArrange();
        //    //InvalidateArrange();

        //}

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(IsReadOnly))
        //    {
        //        if (!(bool)newValue && document != null)
        //        {
        //            if (document.Children.Any(a => a is IDocumentContainer))
        //            {
        //                throw new Exception("编辑模式下不能加元素容器");
        //            }
        //        }
        //    }
        //    else if (propertyName == nameof(Text))
        //    {
        //        if (!ownerSet)
        //        {
        //            if (newValue == null)
        //            {
        //                throw new Exception("Text不能为null");
        //            }
        //            if (PasswordChar == 0)
        //            {
        //                Document.Children.Clear();
        //                Document.Add((string)newValue);
        //            }
        //        }
        //    }
        //    else if (propertyName == nameof(PasswordChar))
        //    {
        //        var newv = (char)newValue;
        //        if (!string.IsNullOrEmpty(Text))
        //        {
        //            Document.Children.Clear();
        //            if (newv != 0)
        //            {
        //                for (int i = 0; i < Text.Length; i++)
        //                {
        //                    Document.Children.Add(new DocumentChar(newv));
        //                }
        //            }
        //            else
        //            {
        //                Document.Add(Text);
        //            }
        //        }
        //    }
        //    else if (propertyName == nameof(IsUndoEnabled))
        //    {
        //        if ((bool)newValue)
        //        {
        //            undoRedoStates.Clear();
        //            undoRedoStates.Add(new UndoRedoState(document.Children.ToArray(), caretIndex.Count == 0 ? 0 : caretIndex[0]));
        //            undoIndex = undoRedoStates.Count - 1;
        //        }
        //        else
        //        {
        //            undoIndex = 0;
        //            undoRedoStates.Clear();
        //        }
        //    }
        //}

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                rect.Location = new Point(b.Width, b.Width);
                rect.Size = new Size(Math.Max(0, rect.Width - b.Width - b.Width), Math.Max(0, rect.Height - b.Width - b.Width));
            }
            else
            {
                var b = BorderThickness;
                rect.Location = new Point(b.Left, b.Top);
                rect.Size = new Size(Math.Max(0, rect.Width - b.Left - b.Right), Math.Max(0, rect.Height - b.Top - b.Bottom));
            }
            foreach (UIElement child in Children)
            {
                child.Arrange(rect);
            }
            return finalSize;
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            var ava = availableSize;
            Size size;
            if (BorderType == BorderType.BorderStroke)
            {
                var b = BorderStroke;
                ava.Width = Math.Max(0, ava.Width - b.Width - b.Width);
                ava.Height = Math.Max(0, ava.Height - b.Width - b.Width);
                size = BaseMeasureOverride(ava);
                size.Width = size.Width + b.Width + b.Width;
                size.Height = size.Height + b.Width + b.Width;
            }
            else
            {
                var b = BorderThickness;
                ava.Width = Math.Max(0, ava.Width - b.Left - b.Right);
                ava.Height = Math.Max(0, ava.Height - b.Top - b.Bottom);
                size = BaseMeasureOverride(ava);
                size.Width = size.Width + b.Left + b.Right;
                size.Height = size.Height + b.Top + b.Bottom;
            }
            return size;
        }
        Size BaseMeasureOverride(in Size availableSize)
        {
            Size contentDesiredSize = new Size();
            foreach (UIElement item in Children)
            {
                item.Measure(availableSize);
                contentDesiredSize.Width = Math.Max(contentDesiredSize.Width, item.DesiredSize.Width);
                contentDesiredSize.Height = Math.Max(contentDesiredSize.Height, item.DesiredSize.Height);
            }
            return contentDesiredSize;
        }


        protected override bool OnSetValue(string propertyName, ref object value)
        {
            if (propertyName == nameof(Text) && value == null)
            {
                value = "";
            }
            return base.OnSetValue(propertyName, ref value);
        }

        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            //if (IsReadOnly)
            //{
            //    return;
            //}
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

        void SelectWord()
        {
            var items = Document.Children;
            IDocumentElement E = null;
            int index = -1;
            if (CaretIndex.Count == 0)
            {
                CaretIndex.Add(0);
            }
            var caretIndex = (int)CaretIndex[0];
            if (caretIndex < items.Count)
            {
                E = items[caretIndex];
                index = caretIndex;
            }
            else if (items.Count > 0)
            {
                E = items[items.Count - 1];
                index = items.Count - 1;
            }
            if (E != null)
            {
                int start = index;
                int lenght = 1;
                DocumentChar c = E as DocumentChar;
                int type = 3;//字符类型0：数字，1：字母，2：文字，3：其他
                if (c != null)
                {
                    if (IsDigit(c.Char))
                    {
                        type = 0;
                    }
                    else if (IsLetter(c.Char))
                    {
                        type = 1;
                    }
                    else if (IsAsianCharecter(c.Char))
                    {
                        type = 2;
                    }
                    if (index > 0)
                    {
                        for (int i = index - 1; i >= 0; i--)//往前找
                        {
                            var ch = items[i] as DocumentChar;
                            if (ch != null && CheckChar(type, ch.Char))
                            {
                                lenght++;
                                start = i;
                                continue;
                            }
                            break;
                        }
                    }
                    if (index < items.Count)//往后找
                    {
                        for (int i = index + 1; i < items.Count; i++)
                        {
                            var ch = items[i] as DocumentChar;
                            if (ch != null && CheckChar(type, ch.Char))
                            {
                                lenght++;
                                continue;
                            }
                            break;
                        }
                    }

                    //双击选中之后光标一定是在选中块的后面
                    if (selectionEnd.Count == 0)
                    {
                        CaretIndex.Add((uint)(start + lenght));
                        selectionEnd.Add((uint)start);
                    }
                    else
                    {
                        CaretIndex[0] = (uint)(start + lenght);
                    }
                    //this.CaretIndex[0] = (uint)start;
                    this.selectionEnd[0] = (uint)start;
                }

            }
        }

        private bool CheckChar(int type, char c)
        {
            if (type == 0 && IsDigit(c))
            {
                return true;
            }
            if (type == 1 && IsLetter(c))
            {
                return true;
            }
            if (type == 2 && IsAsianCharecter(c))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            selectionEnd.Clear();
            selectionEnd.Add(0);
            caretIndex.Clear();
            caretIndex.Add((uint)Document.Children.Count);

            ScrollToCaret();
            TextBoxView.UpdateCaretPosition();
        }

        public void Undo()
        {
            if (!IsReadOnly && IsUndoEnabled)
            {
                if (undoRedoStates.Count > 1 && undoIndex > 0)
                {
                    isRedo = true;
                    undoIndex--;
                    Document.Children.Clear();
                    document.Children.AddRange(undoRedoStates[undoIndex].Items);
                    caretIndex.Clear();
                    caretIndex.Add(undoRedoStates[undoIndex].CaretPosition);
                    selectionEnd.Clear();
                    //isRedo = false;
                }
            }
        }

        public void Redo()
        {
            if (!IsReadOnly && IsUndoEnabled)
            {
                if (undoIndex < undoRedoStates.Count - 1)
                {
                    isRedo = true;
                    undoIndex++;
                    Document.Children.Clear();
                    document.Children.AddRange(undoRedoStates[undoIndex].Items);
                    caretIndex.Clear();
                    caretIndex.Add(undoRedoStates[undoIndex].CaretPosition);
                    selectionEnd.Clear();
                    //isRedo = false;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (caretIndex.Count == 0)
                {
                    caretIndex.Add(0);
                }
                TextBoxView._ShowCaret();
                Point point;
                float height;
                int index;
                switch (e.Key)
                {
                    case Keys.Up:
                        if (selectionEnd.Count == 0 && Root.InputManager.KeyboardDevice.Modifiers == InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                            selectionEnd.AddRange(caretIndex);
                        }
                        point = TextBoxView.GetPostion(caretIndex, out height);
                        point.Offset(0, -height / 2);
                        TextBoxView.HitTest(point, caretIndex);
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                        }
                        break;
                    case Keys.Down:
                        if (selectionEnd.Count == 0 && Root.InputManager.KeyboardDevice.Modifiers == InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                            selectionEnd.AddRange(caretIndex);
                        }
                        point = TextBoxView.GetPostion(caretIndex, out height);
                        point.Offset(0, height / 2 + height);
                        TextBoxView.HitTest(point, caretIndex);
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                        }
                        break;
                    case Keys.Left:
                        if (selectionEnd.Count == 0 && Root.InputManager.KeyboardDevice.Modifiers == InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                            selectionEnd.AddRange(caretIndex);
                        }
                        index = (int)(caretIndex[0] - 1);
                        if (index >= 0)
                        {
                            caretIndex[0] = (uint)index;
                        }
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                        }
                        break;
                    case Keys.Right:
                        if (selectionEnd.Count == 0 && Root.InputManager.KeyboardDevice.Modifiers == InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                            selectionEnd.AddRange(caretIndex);
                        }
                        index = (int)(caretIndex[0] + 1);
                        if (index <= Document.Children.Count)
                        {
                            caretIndex[0] = (uint)index;
                        }
                        if (Root.InputManager.KeyboardDevice.Modifiers != InputModifiers.Shift)
                        {
                            selectionEnd.Clear();
                        }
                        break;
                    case Keys.Home:
                        point = TextBoxView.GetPostion(caretIndex, out height);
                        point = new Point(1, point.Y + 2);
                        TextBoxView.HitTest(point, caretIndex);
                        selectionEnd.Clear();
                        break;
                    case Keys.End:
                        point = TextBoxView.GetPostion(caretIndex, out height);
                        point = new Point(TextBoxView.ActualSize.Width, point.Y + 2);
                        TextBoxView.HitTest(point, caretIndex);
                        selectionEnd.Clear();
                        break;
                    case Keys.PageDown:
                        point = TextBoxView.GetPostion(caretIndex, out height);
                        point = new Point(point.X, point.Y + 2 + ActualSize.Height);
                        TextBoxView.HitTest(point, caretIndex);
                        selectionEnd.Clear();
                        break;
                    case Keys.PageUp:
                        point = TextBoxView.GetPostion(caretIndex, out height);
                        point = new Point(point.X, point.Y + 2 - ActualSize.Height);
                        TextBoxView.HitTest(point, caretIndex);
                        selectionEnd.Clear();
                        break;
                    //case Keys.C:
                    //    if (e.Modifiers == InputModifiers.Control && PasswordChar == 0)
                    //    {
                    //        Copy();
                    //    }
                    //    break;
                    //case Keys.A:
                    //    if (e.Modifiers == InputModifiers.Control)
                    //    {
                    //        selectionEnd.Clear();
                    //        selectionEnd.Add(0);
                    //        caretIndex.Clear();
                    //        caretIndex.Add((uint)Document.Children.Count);
                    //    }
                    //    break;
                    default:
                        break;
                }
                var hotkey = Application.GetRuntimePlatform().Hotkey(new KeyGesture(e.Key, e.Modifiers));
                switch (hotkey)
                {
                    case PlatformHotkey.None:
                        break;
                    case PlatformHotkey.SelectAll:
                        selectionEnd.Clear();
                        selectionEnd.Add(0);
                        caretIndex.Clear();
                        caretIndex.Add((uint)Document.Children.Count);
                        break;
                    case PlatformHotkey.Copy:
                        if (PasswordChar == 0)
                        {
                            Copy();
                        }
                        break;
                    case PlatformHotkey.Cut:
                        if (PasswordChar == 0)
                        {
                            Copy();
                        }
                        if (!IsReadOnly)
                        {
                            RemoveSelect();
                        }
                        break;
                    case PlatformHotkey.Paste:
                        if (!IsReadOnly)
                        {
                            Paste();
                        }
                        break;
                    case PlatformHotkey.Undo:
                        Undo();
                        break;
                    case PlatformHotkey.Redo:
                        Redo();
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
                    //case Keys.Z:

                    //    break;
                    //case Keys.Y:

                    //    break;
                    case Keys.Delete:
                        if (!RemoveSelect())
                        {
                            index = (int)(caretIndex[0]);
                            if (index < Document.Children.Count)
                            {
                                Document.Children.RemoveAt(index);
                                if (PasswordChar != 0)
                                {
                                    Text = Text.Remove(index, 1);
                                }
                            }
                            selectionEnd.Clear();
                        }
                        break;
                    case Keys.Back:
                        if (!RemoveSelect())
                        {
                            index = (int)(caretIndex[0] - 1);
                            if (index >= 0 && index < Document.Children.Count)
                            {
                                Document.Children.RemoveAt(index);
                                if (PasswordChar != 0)
                                {
                                    Text = Text.Remove(index, 1);
                                }
                                caretIndex[0] = (uint)index;
                            }
                            selectionEnd.Clear();
                        }
                        break;
                    //case Keys.X:
                    //    if (e.Modifiers == InputModifiers.Control)
                    //    {
                    //        if (PasswordChar == 0)
                    //        {
                    //            Copy();
                    //        }
                    //        RemoveSelect();
                    //    }
                    //    break;
                    //case Keys.V:
                    //    if (e.Modifiers == InputModifiers.Control)
                    //    {
                    //        Paste();
                    //    }
                    //    break;
                    case Keys.Enter:
                        if (AcceptsReturn && PasswordChar == 0)
                        {
                            if (MaxLength > 0 && Document.Children.Count >= MaxLength && TextBoxView.Comparer(SelectionEnd, CaretIndex) == 0)
                            {
                                return;
                            }
                            RemoveSelect();
                            Document.Children.Insert((int)caretIndex[0], new DocumentChar('\n'));
                            caretIndex[0] = caretIndex[0] + 1;
                            selectionEnd.Clear();
                        }
                        break;
                    case Keys.Tab:
                        if (AcceptsTab)
                        {
                            if (MaxLength > 0 && Document.Children.Count >= MaxLength && TextBoxView.Comparer(SelectionEnd, CaretIndex) == 0)
                            {
                                return;
                            }
                            RemoveSelect();
                            Document.Children.Insert((int)caretIndex[0], new DocumentChar('\t'));
                            caretIndex[0] = caretIndex[0] + 1;
                            selectionEnd.Clear();

                            e.Handled = true;
                        }
                        break;
                }
                ScrollToCaret();
                TextBoxView.UpdateCaretPosition();
            }

        }

        void GetText(StringBuilder sb, StringBuilder textSb, int i, IDocumentContainer documentContainer, IList<uint> big, IList<uint> small)
        {
            for (int j = (int)small[i]; j <= big[i]; j++)
            {
                if (j < documentContainer.Children.Count)
                {
                    var cc = documentContainer.Children[j];
                    if (cc is IDocumentContainer ccc)
                    {
                        if (j == (int)small[i])
                        {
                            if (i + 1 == small.Count)
                            {
                                foreach (var item in ccc.Children)
                                {
                                    AddText(sb, textSb, item);
                                }
                            }
                            else
                            {
                                var dc = ccc;
                                List<KeyValuePair<int, IDocumentContainer>> dcs = new List<KeyValuePair<int, IDocumentContainer>>();
                                for (int z = i + 1; z < small.Count; z++)
                                {
                                    var ci = (int)small[z];
                                    IDocumentContainer dcc = null;
                                    if (ci < dc.Children.Count)
                                    {
                                        dcc = dc.Children[ci] as IDocumentContainer;
                                    }
                                    if (dcc == null)
                                    {
                                        for (int x = (int)small[z]; x < dc.Children.Count; x++)
                                        {//最顶上的一层
                                            AddText(sb, textSb, dc.Children[x]);
                                        }
                                        break;
                                    }
                                    dc = dcc;
                                    dcs.Add(new KeyValuePair<int, IDocumentContainer>(z, dc));
                                }
                                for (int z = dcs.Count - 1; z > 0; z--)
                                {
                                    var p = dcs[z - 1];
                                    for (int x = dcs[z].Key; x < p.Value.Children.Count; x++)
                                    {
                                        AddText(sb, textSb, p.Value.Children[x]);
                                    }
                                }
                            }
                        }
                        else if (j == big[i])
                        {
                            var dc = ccc;
                            List<KeyValuePair<int, IDocumentContainer>> dcs = new List<KeyValuePair<int, IDocumentContainer>>();
                            for (int z = i + 1; z < big.Count; z++)
                            {
                                var ci = (int)big[z];
                                IDocumentContainer dcc = null;
                                if (ci < dc.Children.Count)
                                {
                                    dcc = dc.Children[ci] as IDocumentContainer;
                                }
                                if (dcc == null)
                                {
                                    for (int x = 0; x < big[z]; x++)
                                    {//最顶上的一层
                                        AddText(sb, textSb, dc.Children[x]);
                                    }
                                    break;
                                }
                                dc = dcc;
                                dcs.Add(new KeyValuePair<int, IDocumentContainer>(z, dc));
                            }
                            for (int z = dcs.Count - 1; z > 0; z--)
                            {
                                var p = dcs[z - 1];
                                for (int x = 0; x < dcs[z].Key; x++)
                                {
                                    AddText(sb, textSb, p.Value.Children[x]);
                                }
                            }
                        }
                        else
                        {
                            GetText(sb, textSb, ccc);
                        }
                        if (cc is Block && !(cc is InlineBlock))
                        {
                            sb.Append('\n');
                            textSb.Append('\n');
                        }
                    }
                    else if (j != big[i])
                    {
                        if (cc is DocumentChar c)
                        {
                            if (spacialChar.TryGetValue(c.Char, out string str))
                            {
                                sb.Append(str);
                            }
                            else
                            {
                                sb.Append(c.Char);
                            }
                            textSb.Append(c.Char);
                        }
                        else if (cc is UTF32Text u)
                        {
                            sb.Append(u.Text);
                            textSb.Append(u.Text);
                        }
                        else if (cc is InlineUIContainer ui && ui.UIElement is Picture pic)
                        {
                            var source = pic.Source;
                            if (source is byte[] b)
                            {
                                var base64 = Convert.ToBase64String(b);
                                sb.Append("<img src='data:image/png;base64," + base64 + "' />");
                            }
                            else if (source is Stream stream)
                            {
                                var bs = new byte[stream.Length];
                                stream.Position = 0;
                                stream.Read(bs, 0, bs.Length);
                                stream.Position = 0;
                                var base64 = Convert.ToBase64String(bs);
                                sb.Append("<img src='data:image/png;base64," + base64 + "' />");
                            }
                            else if (source is string str)
                            {
                                sb.Append("<img src='" + str + "' />");
                            }
                            else if (source is Image img)
                            {
                                using (var ms = img.SaveToStream(ImageFormat.Png))
                                {
                                    var bs = new byte[ms.Length];
                                    ms.Position = 0;
                                    ms.Read(bs, 0, bs.Length);
                                    var base64 = Convert.ToBase64String(bs);
                                    sb.Append("<img src='data:image/png;base64," + base64 + "' />");
                                }
                            }
                        }
                    }

                }
            }
        }

        void GetText(StringBuilder sb, StringBuilder textSb, IDocumentContainer container)
        {
            for (int i = 0; i < container.Children.Count; i++)
            {
                var item = container.Children[i];
                AddText(sb, textSb, item);
            }
        }

        private void AddText(StringBuilder sb, StringBuilder textSb, IDocumentElement item)
        {
            if (item is DocumentChar c)
            {
                if (spacialChar.TryGetValue(c.Char, out string str))
                {
                    sb.Append(str);
                }
                else
                {
                    sb.Append(c.Char);
                }
                textSb.Append(c.Char);
            }
            else if (item is UTF32Text u)
            {
                sb.Append(u.Text);
                textSb.Append(u.Text);
            }
            else if (item is IDocumentContainer dc)
            {
                GetText(sb, textSb, dc);
                if (item is Block && !(item is InlineBlock))
                {
                    sb.Append('\n');
                    textSb.Append('\n');
                }
            }
            else if (item is InlineUIContainer ui && ui.UIElement is Picture pic)
            {
                var source = pic.Source;
                if (source is byte[] b)
                {
                    var base64 = Convert.ToBase64String(b);
                    sb.Append("<img src='data:image/png;base64," + base64 + "' />");
                }
                else if (source is Stream stream)
                {
                    var bs = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bs, 0, bs.Length);
                    stream.Position = 0;
                    var base64 = Convert.ToBase64String(bs);
                    sb.Append("<img src='data:image/png;base64," + base64 + "' />");
                }
                else if (source is string str)
                {
                    sb.Append("<img src='" + str + "' />");
                }
                else if (source is Image img)
                {
                    using (var ms = img.SaveToStream(ImageFormat.Png))
                    {
                        var bs = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(bs, 0, bs.Length);
                        var base64 = Convert.ToBase64String(bs);
                        sb.Append("<img src='data:image/png;base64," + base64 + "' />");
                    }
                }
            }
        }

        static Dictionary<char, string> spacialChar = new Dictionary<char, string>() {
            { '<', "&lt;" },
            { '>', "&gt;" },
            { '&', "&amp;" },
            { '\'', "&apos;" },
            { '"', "&quot;" },
            { ' ', "&nbsp;" } ,
        };

        static Dictionary<string, char> spacialStr = new Dictionary<string, char>()
        {
            { "&lt;", '<' },
            { "&gt;", '>' },
            { "&amp;", '&' },
            { "&apos;", '\'' },
            { "&quot;", '"' },
            { "&nbsp;", ' ' },
            { "&#32;", ' ' },
            { "&excl;", '!' },
            { "&#33;", '!' },
            { "&#34;", '"' },
            { "&num;", '#' },
            { "&#35;", '#' },
            { "&dollar;", '$' },
            { "&#36;", '$' },
            { "&percnt;", '%' },
            { "&#37;", '%' },
            { "&#38;", '&' },
            { "&#39;", '\'' },
            { "&lpar;", '(' },
            { "&#40;", '(' },
            { "&rpar;", ')' },
            { "&#41;", ')' },
            { "&ast;", '*' },
            { "&#42;", '*' },
            { "&plus;", '+' },
            { "&#43;", '+' },
            { "&comma;", ',' },
            { "&#44;", ',' },
            { "&hyphen;", '-' },
            { "&#45;", '-' },
            { "&period;", '.' },
            { "&#46;", '.' },
            { "&sol;", '/' },
            { "&#47;", '/' },
            { "&colon;", ':' },
            { "&#58;", ':' },
            { "&semi;", ';' },
            { "&#59;", ';' },
            { "&#60;", '<' },
            { "&#62;", '>' },
            { "&equals;", '=' },
            { "&#61;", '=' },
            { "&quest;", '?' },
            { "&#63;", '?' },
            { "&commat;", '@' },
            { "&#64;", '@' },
            { "&lsqb;", '[' },
            { "&#91;", '[' },
            { "&bsol;", '\\' },
            { "&#92;", '\\' },
            { "&rsqb;", ']' },
            { "&#93;", ']' },
            { "&circ;", '^' },
            { "&#94;", '^' },
            { "&lowbar;", '_' },
            { "&#95;", '_' },
            { "&grave;", '`' },
            { "&#96;", '`' },
            { "&lcub;", '{' },
            { "&#123;", '{' },
            { "&verbar;", '|' },
            { "&#124;", '|' },
            { "&rcub;", '}' },
            { "&#125;", '}' },
            { "&tilde;", '~' },
            { "&#126;", '~' },
        };

        public virtual void Copy()
        {
            if (TextBoxView.Comparer(caretIndex, selectionEnd) != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<pre>");
                StringBuilder textSb = new StringBuilder();
                IDocumentContainer documentContainer = Document;
                var cs = caretIndex.ToList();
                var ss = selectionEnd.ToList();
                var l = Math.Max(cs.Count, ss.Count);
                if (cs.Count < l)
                {//假如两个索引层级不一样，要补充索引
                    for (int i = 0; i < l - cs.Count; i++)
                    {
                        cs.Add(0);
                    }
                }
                if (ss.Count < l)
                {
                    for (int i = 0; i < l - ss.Count; i++)
                    {
                        ss.Add(0);
                    }
                }
                for (int i = 0; i < l; i++)
                {
                    if (cs[i] > ss[i])
                    {
                        GetText(sb, textSb, i, documentContainer, cs, ss);
                        break;
                    }
                    else if (cs[i] < ss[i])
                    {
                        GetText(sb, textSb, i, documentContainer, ss, cs);
                        break;
                    }
                    else
                    {
                        if (i < l - 1)
                        {
                            documentContainer = documentContainer.Children[(int)cs[i]] as IDocumentContainer;
                            if (documentContainer == null)
                            {
                                break;
                            }
                        }
                    }
                }
                sb.Append("</pre>");
                Clipboard.SetData((DataFormat.Text, textSb), (DataFormat.Html, sb));
            }
        }

        /// <summary>
        /// 获取选中的内容
        /// </summary>
        /// <returns></returns>
        public (string text, string html) GetSelectedString()
        {
            return GetString(caretIndex, selectionEnd);
        }
        /// <summary>
        /// 获取所有Html和Text格式内容
        /// </summary>
        /// <returns></returns>
        public (string text, string html) GetString()
        {
            return GetString(new uint[] { 0 }, new uint[] { (uint)Document.Children.Count });
        }

        /// <summary>
        /// 获取Html和Text格式内容
        /// </summary>
        /// <returns></returns>
        public (string text, string html) GetString(IList<uint> start, IList<uint> end)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<pre>");
            StringBuilder textSb = new StringBuilder();
            IDocumentContainer documentContainer = Document;
            start = start.ToList();
            end = end.ToList();
            var l = Math.Max(start.Count, end.Count);
            if (start.Count < l)
            {//假如两个索引层级不一样，要补充索引
                for (int i = 0; i < l - start.Count; i++)
                {
                    start.Add(0);
                }
            }
            if (end.Count < l)
            {
                for (int i = 0; i < l - end.Count; i++)
                {
                    end.Add(0);
                }
            }
            for (int i = 0; i < l; i++)
            {
                if (start[i] > end[i])
                {
                    GetText(sb, textSb, i, documentContainer, start, end);
                    break;
                }
                else if (start[i] < end[i])
                {
                    GetText(sb, textSb, i, documentContainer, end, start);
                    break;
                }
                else
                {
                    if (i < l - 1)
                    {
                        documentContainer = documentContainer.Children[(int)start[i]] as IDocumentContainer;
                        if (documentContainer == null)
                        {
                            break;
                        }
                    }
                }
            }
            sb.Append("</pre>");
            return (textSb.ToString(), sb.ToString());
        }

        public virtual void Paste()
        {
            RemoveSelect();
            var psw = PasswordChar;
            var acceptsReturn = AcceptsReturn;
            if (Clipboard.Contains(DataFormat.Html) && psw == 0 && IsAllowPasteImage)
            {
                var startIndex = (int)caretIndex[0];
                var html = Clipboard.GetData(DataFormat.Html).ToString();
                //var start = html.IndexOf("<!--StartFragment");
                //var end = html.IndexOf("<!--EndFragment");
                //html = html.Substring(start + 17, end - start - 17);
                //html = html.TrimStart('-', ' ', '>');
                var styleStart = html.IndexOf("<style");
                if (styleStart >= 0)
                {
                    var styleEnd = html.IndexOf("</style>");
                    if (styleEnd > 0)
                    {
                        html = html.Remove(styleStart, styleEnd + 8 - styleStart);
                    }
                }

                bool hasLeftTag = false;
                var spacialStart = "";
                StringBuilder tag = new StringBuilder();
                var img = false;
                var sp = (char)0;
                var src = false;
                var maxLength = MaxLength;
                var Items = Document.Children;
                var Surrogate = "";//多字节字符

                foreach (var item in html)
                {
                    if (maxLength != 0 && Items.Count >= maxLength)
                    {
                        break;
                    }
                    if (item == '<')
                    {
                        hasLeftTag = true; img = false;
                        tag.Remove(0, tag.Length);
                    }
                    else if (item == '>' && hasLeftTag)
                    {
                        hasLeftTag = false; img = false;
                        var low = tag.ToString().ToLower();
                        if (acceptsReturn && (low == "br" || low == "/p" || low == "/div" || low == "/br" || low == "/h1" || low == "/h2" || low == "/h3" || low == "/h4" || low == "/h5"))
                        {
                            Items.Insert(startIndex, new DocumentChar('\n'));
                            startIndex++;
                        }
                        tag.Remove(0, tag.Length);
                    }
                    else if (hasLeftTag)
                    {
                        if (!img)
                        {
                            src = false;
                            if (item == ' ')
                            {
                                if (tag.ToString().ToLower() == "img")
                                {
                                    img = true;
                                }
                                tag.Remove(0, tag.Length);
                            }
                            else
                            {
                                //tag += item;
                                tag.Append(item);
                            }
                        }
                        else if (img)
                        {
                            if (!src)
                            {
                                if (item == '=')
                                {
                                    if (tag.ToString().ToLower() == "src")
                                    {
                                        src = true;
                                    }
                                    tag.Remove(0, tag.Length);
                                }
                                else
                                {
                                    if (item == ' ')
                                    {
                                        tag.Remove(0, tag.Length);
                                    }
                                    else
                                    {
                                        //tag += item;
                                        tag.Append(item);
                                    }
                                }
                            }
                            else
                            {
                                if (sp == 0 && (item == '"' || item == '\''))
                                {
                                    sp = item;
                                }
                                else if (sp != 0 && item != sp)
                                {
                                    tag.Append(item);
                                }
                                else if (item == sp)
                                {
                                    if (IsAllowPasteImage)
                                    {
                                        var value = tag.ToString();
                                        var tolow = "";
                                        if (value.Length > 12)
                                        {
                                            tolow = value.Substring(0, 12).ToLower();
                                        }
                                        else
                                        {
                                            tolow = value.ToLower();
                                        }
                                        if (tolow.StartsWith("http") || tolow.StartsWith("res://"))
                                        {
                                            try
                                            {
                                                //var request = HttpWebRequest.Create(value);
                                                //var response = request.GetResponse();
                                                //var sb = response.GetResponseStream();
                                                //using (sb)
                                                //{
                                                //var im = Image.FromStream(sb);
                                                Items.Insert(startIndex, CreatePictureBox(value));
                                                startIndex++;
                                                //}
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine("下载图片失败：" + e.Message);
                                            }
                                        }
                                        else if (tolow.StartsWith("data:image/"))
                                        {
                                            var data = value.Substring(value.IndexOf(',') + 1);
                                            var dataImg = Convert.FromBase64String(data);
                                            //using (MemoryStream ms = new MemoryStream(dataImg))
                                            //{
                                            //    var im = Image.FromStream(ms);
                                            Items.Insert(startIndex, CreatePictureBox(dataImg));
                                            startIndex++;
                                            //}
                                        }
                                        else if (tolow.StartsWith("file"))
                                        {
                                            //var fileName = value.Substring(5);
                                            //fileName = fileName.TrimStart('\\', '/');
                                            //if (!string.IsNullOrEmpty(value))
                                            //{
                                            try
                                            {
                                                Items.Insert(startIndex, CreatePictureBox(value));
                                                startIndex++;
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine("加载图片失败：" + e.Message);
                                            }
                                            //}

                                        }
                                    }
                                    tag.Remove(0, tag.Length);
                                }
                            }

                        }
                    }
                    else if (!hasLeftTag)
                    {
                        if (item == '&')
                        {
                            spacialStart = "&";
                            continue;
                        }
                        else if (spacialStart != "" && item != ';' && spacialStart.Length < 6)
                        {
                            spacialStart += item;
                        }
                        else
                        {
                            if (spacialStart != "")
                            {
                                spacialStart += item;
                                if (item == ';')
                                {
                                    char p;
                                    if (spacialStr.TryGetValue(spacialStart, out p))
                                    {
                                        Items.Insert(startIndex, new DocumentChar(p));
                                        startIndex++;
                                    }
                                    else
                                    {
                                        //foreach (var c in spacialStart)
                                        //{
                                        //    Items.Insert(startIndex, new DocumentChar(c));
                                        //    startIndex++;
                                        //}
                                        startIndex += Document.InsertText(startIndex, spacialStart);
                                    }
                                }
                                else
                                {
                                    //foreach (var c in spacialStart)
                                    //{
                                    //    Items.Insert(startIndex, new DocumentChar(c));
                                    //    startIndex++;
                                    //}
                                    startIndex += Document.InsertText(startIndex, spacialStart);
                                }
                                spacialStart = "";
                            }
                            else
                            {
                                if (item != '\r')
                                {
                                    var info = CharUnicodeInfo.GetUnicodeCategory(item);
                                    if (IsCombiningCategory(info))
                                    {
                                        Surrogate += item;
                                        if (Surrogate.Length > 1)
                                        {
                                            Items.Insert(startIndex, new UTF32Text(Surrogate));
                                            startIndex++;
                                            Surrogate = "";
                                        }
                                    }
                                    else
                                    {
                                        if (item == '\n' && !acceptsReturn)
                                        {
                                            continue;
                                        }
                                        Items.Insert(startIndex, new DocumentChar(item));
                                        startIndex++;
                                    }
                                }
                            }
                        }
                    }
                }
                CaretIndex[0] = (uint)startIndex;
            }
            else
            {
                if (Clipboard.Contains(DataFormat.Text))
                {
                    var text = Clipboard.GetData(DataFormat.Text).ToString();
                    if (!acceptsReturn)
                    {
                        text = text.Replace("\n", "");
                        text = text.Replace("\r", "");
                    }
                    if (MaxLength != 0 && text.Length + Document.Children.Count > MaxLength)
                    {
                        var l = MaxLength - Document.Children.Count;
                        if (l > 0)
                        {
                            text = text.Substring(0, (int)l);
                        }
                        else
                        {
                            return;
                        }
                    }
                    var index = Math.Min((int)caretIndex[0], Document.Children.Count);
                    if (psw == 0)
                    {
                        caretIndex[0] = (uint)(index + Document.InsertText(index, text));
                    }
                    else
                    {
                        Text = Text.Insert(index, text);
                        //for (int i = 0; i < text.Length; i++)
                        //{
                        //    Document.Children.Add(new DocumentChar(psw));
                        //}
                        caretIndex[0] = (uint)(index + text.Length);
                    }
                }
                if (Clipboard.Contains(DataFormat.Image) && psw == 0 && IsAllowPasteImage)
                {
                    Document.Children.Insert((int)caretIndex[0], CreatePictureBox(Clipboard.GetData(DataFormat.Image)));
                }
            }

            selectionEnd.Clear();
        }

        protected virtual InlineUIContainer CreatePictureBox(object source)
        {
            return new InlineUIContainer
            {
                UIElement = new Picture()
                {
                    MarginBottom = 2,
                    MarginLeft = 2,
                    MarginRight = 2,
                    MarginTop = 2,
                    Source = source,
                    MaxWidth = 200,
                    Stretch = Stretch.Uniform,
                    StretchDirection = StretchDirection.DownOnly
                }
            };
        }
        /// <summary>
        /// 根据鼠标位置计算索引位置
        /// </summary>
        /// <param name="mosPos"></param>
        /// <param name="index">返回结果</param>
        public void HitTestElement(Point mosPos, IList<uint> index)
        {
            TextBoxView.HitTest(mosPos, index);
        }
        /// <summary>
        /// 根据鼠标位置计算点击到的元素
        /// </summary>
        /// <param name="mosPos"></param>
        /// <returns></returns>
        public IDocumentElement HitTestElement(Point mosPos)
        {
            return TextBoxView.HitTestElement(mosPos);
        }

        bool scroll = false;
        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            //if (WordWarp)
            //{
            //    var mw = document.MaxWidth;
            //    if (scrollViewer)
            //    {
            //        if (!mw.IsAuto && mw.Unit == Unit.Default)
            //        {
            //            if (mw.Value != scrollViewer.ViewportWidth)
            //            {
            //                document.InvalidateArrange();
            //                InvalidateArrange();
            //            }
            //        }
            //        document.MaxWidth = scrollViewer.ViewportWidth;
            //    }
            //    else
            //    {
            //        document.MaxWidth = ActualSize.Width;
            //    }
            //}
            //else
            //{
            //    document.MaxWidth = FloatField.Auto;
            //}
            if (scroll && IsInitialized)
            {
                scroll = false;
                BeginInvoke(() =>
                {
                    var point = TextBoxView.GetPostion(caretIndex, out float height);
                    //var scrollViewer = Find<ScrollViewer>().FirstOrDefault();
                    if (scrollViewer == null)
                    {
                        throw new Exception("ScrollViewer不存在");
                    }
                    if (scrollViewer.VerticalOffset >= point.Y)
                    {
                        scrollViewer.VerticalOffset = point.Y;
                        InvalidateArrange();
                    }
                    else if (scrollViewer.VerticalOffset < point.Y + height - scrollViewer.ViewportHeight)
                    {
                        scrollViewer.VerticalOffset = point.Y + height - scrollViewer.ViewportHeight;
                        InvalidateArrange();
                    }
                    if (scrollViewer.HorizontalOffset > point.X)
                    {
                        if (caretIndex.Count == 0 || caretIndex[0] == 0)
                        {
                            scrollViewer.HorizontalOffset = 0;
                        }
                        else
                        {
                            scrollViewer.HorizontalOffset = point.X;
                        }
                        InvalidateArrange();
                    }
                    else if (scrollViewer.HorizontalOffset < point.X - scrollViewer.ViewportWidth)
                    {
                        scrollViewer.HorizontalOffset = point.X - scrollViewer.ViewportWidth;
                        InvalidateArrange();
                    }
                });
            }
        }

        public void ScrollToCaret()
        {
            scroll = true;
            InvalidateArrange();
        }
        /// <summary>
        /// 滚动到最底下
        /// </summary>
        public void ScrollToEnd()
        {
            //if (CaretIndex.Count == 0)
            //{
            CaretIndex.Clear();
            CaretIndex.Add((uint)Document.Children.Count);
            //}
            //else
            //{
            //    CaretIndex[0] = (uint)Document.Children.Count;
            //}
            SelectionEnd.Clear();
            ScrollToCaret();
        }

        private void RenderKeywords()
        {
            if (KeywordsStyles.Count == 0)
            {
                return;
            }
            var items = Document.Children;
            if (items.FirstOrDefault(a => a is DocumentChar) != null)//是否包含字符
            {
                Dictionary<int, string> texts = new Dictionary<int, string>();
                StringBuilder sb = new StringBuilder();
                int startIndex = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] is DocumentChar c)
                    {
                        c.StyleId = -1;
                        sb.Append(c.Char);
                    }
                    //else if (items[i] is UTF32Text u)
                    //{
                    //    u.StyleId = -1;
                    //    sb.Append(u.Text);
                    //}
                    else
                    {
                        if (sb.Length > 0)
                        {
                            texts.Add(startIndex, sb.ToString());
                            sb.Remove(0, sb.Length);
                        }
                        startIndex = i + 1;
                    }
                }
                if (sb.Length > 0)
                {
                    texts.Add(startIndex, sb.ToString());
                }
                var invalidateArrange = false;
                foreach (var textItem in texts)
                {
                    string text = textItem.Value;
                    foreach (KeywordsStyle k in KeywordsStyles)
                    {
                        int index = 0;
                        if (!string.IsNullOrEmpty(k.Keywords))
                        {
                            if (k.IsRegex)
                            {
                                RegexOptions ro = k.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
                                MatchCollection mc = Regex.Matches(text, k.Keywords, ro);
                                foreach (Match item in mc)
                                {
                                    for (int i = item.Index + textItem.Key; i < item.Index + textItem.Key + item.Length; i++)
                                    {
                                        if (i < items.Count)
                                        {
                                            var c = items[i];
                                            if (c is DocumentChar)
                                            {
                                                //if (c.StyleId != k.StyleId)
                                                //{
                                                c.StyleId = k.StyleId;
                                                if (c.StyleId < document.Styles.Count)
                                                {
                                                    var style = document.Styles[c.StyleId];
                                                    if (style.HasLocalValue(nameof(DocumentStyle.FontSize)))
                                                    {
                                                        invalidateArrange = true;
                                                        c.IsMeasureValid = false;
                                                    }
                                                }
                                                //}
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                StringComparison sc = k.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                                while ((index = text.IndexOf(k.Keywords, index, sc)) > -1)
                                {
                                    for (int i = index + textItem.Key; i < index + textItem.Key + k.Keywords.Length; i++)
                                    {
                                        if (i < items.Count)
                                        {
                                            var c = items[i];
                                            if (c is DocumentChar)
                                            {
                                                if (c.StyleId != k.StyleId)
                                                {
                                                    c.StyleId = k.StyleId;
                                                    if (c.StyleId < document.Styles.Count)
                                                    {
                                                        var style = document.Styles[c.StyleId];
                                                        if (style.HasLocalValue(nameof(DocumentStyle.FontSize)))
                                                        {
                                                            invalidateArrange = true;
                                                            c.IsMeasureValid = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                }
                if (invalidateArrange)
                {
                    (document as Block).InvalidateArrange();
                }
            }
            Invalidate();
        }

        /// <summary>
        /// 是否是多字节字符
        /// </summary>
        /// <param name="uc"></param>
        /// <returns></returns>
        internal static bool IsCombiningCategory(UnicodeCategory uc)
        {
            return (
                //(uc == UnicodeCategory.NonSpacingMark) ||
                //(uc == UnicodeCategory.SpacingCombiningMark) ||
                //(uc == UnicodeCategory.EnclosingMark) ||
                //(uc == UnicodeCategory.Format) ||
                //(uc == UnicodeCategory.Control) ||
                //(uc == UnicodeCategory.OtherNotAssigned) ||
                (uc == UnicodeCategory.Surrogate)
            );
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            if (IsReadOnly || e.Handled)
            {
                return;
            }
            RemoveSelect();
            if (caretIndex.Count == 0)
            {
                caretIndex.Add(0);
            }

            if (MaxLength > 0 && Document.Children.Count >= MaxLength && TextBoxView.Comparer(SelectionEnd, CaretIndex) == 0)
            {
                return;
            }
            if (PasswordChar != 0)
            {
                var text = "";
                for (int i = 0; i < e.Text.Length; i++)
                {
                    text += PasswordChar;
                }
                Document.InsertText((int)caretIndex[0], text);
                Text = Text.Insert((int)caretIndex[0], e.Text);
                caretIndex[0] = (uint)(caretIndex[0] + text.Length);
            }
            else
            {
                var text = e.Text;
                Document.InsertText((int)caretIndex[0], text);
                caretIndex[0] = (uint)(caretIndex[0] + text.Length);
            }
            selectionEnd.Clear();
            ScrollToCaret();
        }
        /// <summary>
        /// 移除选中的内容
        /// </summary>
        /// <returns></returns>
        public bool RemoveSelect()
        {
            int index;
            if (caretIndex.Count == 0)
            {
                caretIndex.Add(0);
            }
            if (caretIndex[0] > Document.Children.Count)
            {
                caretIndex[0] = (uint)document.Children.Count;
            }
            if (selectionEnd.Count > 0)
            {
                var end = (int)selectionEnd[0];
                var start = (int)caretIndex[0];
                int length = start - end;
                if (length != 0)
                {
                    index = Math.Min(start, end);
                    length = Math.Abs(length);
                    if (index + length > document.Children.Count)
                    {
                        length = document.Children.Count - index;
                    }
                    Document.Children.RemoveRange(index, length);
                    caretIndex[0] = (uint)index;
                    if (PasswordChar != 0)
                    {
                        Text = Text.Remove(index, Math.Abs(length));
                    }
                    selectionEnd.Clear();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the given char is of Asian range.检查出的字符是亚洲范围。
        /// </summary>
        /// <param name="ch">the character to check</param>
        /// <returns>true - Asian char, false - otherwise</returns>
        public static bool IsAsianCharecter(char ch)
        {
            return ch >= 0x4e00 && ch <= 0xFA2D;
        }

        /// <summary>
        /// Check if the given char is a digit character (0-9) and (0-9, a-f for HEX)
        /// </summary>
        /// <param name="ch">the character to check</param>
        /// <param name="hex">optional: is hex digit check</param>
        /// <returns>true - is digit, false - not a digit</returns>
        public static bool IsDigit(char ch, bool hex = false)
        {
            return (ch >= '0' && ch <= '9') || (hex && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F')));
        }
        /// <summary>
        /// 判断字符是否为字母
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsLetter(char ch)
        {
            return ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'));
        }

        /// <summary>
        /// Convert the given char to digit.
        /// </summary>
        /// <param name="ch">the character to check</param>
        /// <param name="hex">optional: is hex digit check</param>
        /// <returns>true - is digit, false - not a digit</returns>
        public static int ToDigit(char ch, bool hex = false)
        {
            if (ch >= '0' && ch <= '9')
                return ch - '0';
            else if (hex)
            {
                if (ch >= 'a' && ch <= 'f')
                    return ch - 'a' + 10;
                else if (ch >= 'A' && ch <= 'F')
                    return ch - 'A' + 10;
            }

            return 0;
        }
        class UndoRedoState
        {
            public IDocumentElement[] Items { get; }
            public uint CaretPosition { get; }

            public UndoRedoState(IDocumentElement[] items, uint caretPosition)
            {
                Items = items;
                CaretPosition = caretPosition;
            }

        }
    }

}
