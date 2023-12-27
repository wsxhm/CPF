using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using System.Globalization;
using System.Collections;
using System.Linq;

namespace CPF.Documents
{
    /// <summary>
    /// 块级元素，类似于网页的 DIV
    /// </summary>
    public class Block : ContentElement, IDocumentElement, IDocumentContainer, IEnumerable
    {
        public Block()
        {
            children.CollectionChanged += Children_CollectionChanged;
        }
        public Block(string text, short style = -1) : this()
        {
            Add(text, style);
        }
        public Block(params IDocumentElement[] elements) : this()
        {
            Add(elements);
        }

        int blockCount;
        private void Children_CollectionChanged(object sender, CollectionChangedEventArgs<IDocumentElement> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    var c = e.NewItem as ContentElement;
                    if (c != null)
                    {
                        Block block = c as Block;
                        if (block != null)
                        {
                            blockCount++;
                            block.Document = Document;
                        }
                        c.Parent = this;
                    }
                    else
                    {
                        var ui = e.NewItem as InlineUIContainer;
                        if (ui != null)
                        {
                            ui.Document = Document;
                            containers.Add(ui);
                            ui.Parent = this;
                        }
                    }
                    break;
                case CollectionChangedAction.Remove:
                    noAppend = true;
                    var cc = e.OldItem as ContentElement;
                    if (cc != null)
                    {
                        Block block = cc as Block;
                        if (block != null)
                        {
                            blockCount--;
                            block.Document = null;
                        }
                        cc.Parent = null;
                    }
                    else
                    {
                        var ui = e.OldItem as InlineUIContainer;
                        if (ui != null)
                        {
                            ui.Document = null;
                            containers.Remove(ui);
                            ui.Parent = null;
                        }
                    }
                    break;
                case CollectionChangedAction.Replace:
                    noAppend = true;
                    var ccc = e.NewItem as ContentElement;
                    if (ccc != null)
                    {
                        Block block = ccc as Block;
                        if (block != null)
                        {
                            blockCount++;
                            block.Document = Document;
                        }
                        ccc.Parent = this;
                    }
                    else
                    {
                        var ui = e.NewItem as InlineUIContainer;
                        if (ui != null)
                        {
                            ui.Document = Document;
                            containers.Add(ui);
                            ui.Parent = this;
                        }
                    }

                    var cccc = e.OldItem as ContentElement;
                    if (cccc != null)
                    {
                        Block block = cccc as Block;
                        if (block != null)
                        {
                            blockCount--;
                            block.Document = null;
                        }
                        cccc.Parent = null;
                    }
                    else
                    {
                        var ui = e.OldItem as InlineUIContainer;
                        if (ui != null)
                        {
                            ui.Document = null;
                            containers.Remove(ui);
                            ui.Parent = null;
                        }
                    }
                    break;
            }
            //IsMeasureValid = false;
            //if (document != null)
            //{
            //    var p = Parent as Block;
            //    while (p != null)
            //    {
            //        p.IsMeasureValid = false;
            //        p = p.Parent as Block;
            //    }
            //    document.InvalidateArrange();
            //}
            InvalidateArrange();
        }

        public void InvalidateArrange()
        {
            IsMeasureValid = false;
            if (document != null)
            {
                var p = Parent as Block;
                while (p != null)
                {
                    p.IsMeasureValid = false;
                    p = p.Parent as Block;
                }
                document.InvalidateArrange();
            }
        }

        [NotCpfProperty]
        public float Left
        {
            get;
            set;
        }

        [NotCpfProperty]
        public float Top
        {
            get;
            set;
        }

        [PropertyMetadata((short)-1)]
        public virtual short StyleId
        {
            get { return GetValue<short>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(true)]
        public bool ChildrenCanSelect
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        [NotCpfProperty]
        public bool IsMeasureValid
        {
            get;
            set;
        }
        Collection<IDocumentElement> children = new Collection<IDocumentElement>();
        /// <summary>
        /// 子元素
        /// </summary>
        [NotCpfProperty]
        public Collection<IDocumentElement> Children
        {
            get
            {
                return children;
            }
        }
        /// <summary>
        /// 背景填充
        /// </summary>
        [UIPropertyMetadata(null, UIPropertyOptions.AffectsRender)]
        public ViewFill Background
        {
            get { return (ViewFill)GetValue(); }
            set { SetValue(value); }
        }
        List<TextLine> lines = new List<TextLine>();
        /// <summary>
        /// 布局好的行
        /// </summary>
#if NET40
        public IList<TextLine> Lines
        {
            get { return lines; }
        }
#else
        public IReadOnlyList<TextLine> Lines
        {
            get { return lines; }
        }
#endif


        internal TextLine Line;

        List<InlineUIContainer> containers = new List<InlineUIContainer>();

        Document document;
        public virtual Document Document
        {
            get { return document; }
            set
            {
                if (document != value)
                {
                    document = value;
                    foreach (var item in children)
                    {
                        var block = item as Block;
                        if (block != null)
                        {
                            block.Document = value;
                        }
                    }
                    foreach (var item in containers)
                    {
                        item.Document = value;
                    }
                }
            }
        }

        public void Add(params IDocumentElement[] elements)
        {
            if (elements != null)
            {
                foreach (var item in elements)
                {
                    children.Add(item);
                }
            }
        }
        /// <summary>
        /// 添加一段文字
        /// </summary>
        /// <param name="text"></param>
        /// <param name="styleId"></param>
        /// <returns>字符数量</returns>
        public int Add(string text, short styleId = -1)
        {
            if (!string.IsNullOrEmpty(text))
            {
                append = true;
                var si = new StringInfo(text);
                int count = 0;
                for (int i = 0; i < si.LengthInTextElements; i++)
                {
                    var str = si.SubstringByTextElements(i, 1);
                    if (str.Length == 1)
                    {
                        if (str != "\r")
                        {
                            Children.Add(new DocumentChar(str[0]) { StyleId = styleId });
                            count++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (str == "\r\n")
                        {
                            Children.Add(new DocumentChar('\n') { StyleId = styleId });
                        }
                        else
                        {
                            Children.Add(new UTF32Text(str) { StyleId = styleId });
                        }
                        count++;
                    }
                }
                return count;
            }
            return 0;
        }/// <summary>
         /// 插入文字
         /// </summary>
         /// <param name="index"></param>
         /// <param name="text"></param>
         /// <param name="styleId"></param>
         /// <returns>字符数量</returns>
        public int InsertText(int index, string text, short styleId = -1)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (index > Children.Count)
                {
                    index = children.Count;
                }
                var si = new StringInfo(text);
                int count = 0;
                for (int i = 0; i < si.LengthInTextElements; i++)
                {
                    var str = si.SubstringByTextElements(i, 1);
                    if (str.Length == 1)
                    {
                        if (str != "\r")
                        {
                            Children.Insert(index, new DocumentChar(str[0]) { StyleId = styleId });
                            index++;
                            count++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (str == "\r\n")
                        {
                            Children.Insert(index, new DocumentChar('\n') { StyleId = styleId });
                        }
                        else
                        {
                            Children.Insert(index, new UTF32Text(str) { StyleId = styleId });
                        }
                        index++;
                        count++;
                    }
                }
                append = false;
                return count;
            }
            return 0;
        }

        Size size;
        Size oldAvailableSize;
        bool append;
        bool noAppend;
        Font oldFont;
        bool oldWrapMode;
        Size oldSize;
        int lastIndex;
        float lastCharLeft;
        TextLine lastLine;
        List<IFlowElement> lastRtl;
        public virtual void Arrange(in Font font, in Size availableSize)
        {
            var minw = MinWidth;
            var padding = Padding;
            if (!IsMeasureValid || (availableSize.Width != oldAvailableSize.Width && (WordWarp || containers.Count > 0 || TextAlignment != TextAlignment.Left || blockCount > 0)))
            {
                IsMeasureValid = true;
                //if (children.Count == 0)
                //{
                //    size = new Size(padding.Left + padding.Right, padding.Top + padding.Bottom);
                //    return;
                //}
                var ks = Platform.Application.GetDrawingFactory().MeasureString(" ", font);
                int startIndex = lastIndex;
                float top = padding.Top;
                float left = padding.Left;
                TextLine line = lastLine;
                List<IFlowElement> rtl = lastRtl;
                float charLeft = 0;
                if (append && !noAppend && oldFont.Equals(font) && (oldWrapMode == WordWarp && !oldWrapMode))
                {
                    size = oldSize;
                    charLeft = lastCharLeft;
                    if (line == null)
                    {
                        line = new TextLine { X = left, Y = top, Width = 0, Height = ks.Height };
                    }
                    else
                    {
                        size.Height = Math.Max(0, size.Height - line.Height);
                        lines.Remove(line);
                    }
                    top = line.Y;
                    if (rtl == null)
                    {
                        rtl = new List<IFlowElement>();
                    }
                }
                else
                {
                    lines.Clear();
                    size = new Size();
                    startIndex = 0;
                    line = new TextLine { X = left, Y = top, Width = 0, Height = ks.Height };
                    rtl = new List<IFlowElement>();
                }
                append = false;
                noAppend = false;
                oldWrapMode = WordWarp;
                var changedFont = !oldFont.Equals(font);
                oldFont = font;
                var margin = Margin;
                float width = availableSize.Width;
                var W = Width;
                if (!W.IsAuto)
                {
                    width = W.GetActualValue(width);
                }
                var maxW = MaxWidth;
                if (!maxW.IsAuto)
                {
                    width = Math.Min(maxW.GetActualValue(availableSize.Width), width);
                }
                if (!minw.IsAuto)
                {
                    width = Math.Max(minw.GetActualValue(availableSize.Width), width);
                }
                var parentCache = new Cache { Font = font };
                var document = Document;
                HybridDictionary<int, Cache> cache = new HybridDictionary<int, Cache>();
                width = width - padding.Left - padding.Right - margin.Right - margin.Left;
                var avs = new Size(Math.Max(0, width), Math.Max(0, availableSize.Height - padding.Bottom - padding.Top - margin.Top - margin.Bottom));
                for (int i = startIndex; i < children.Count; i++)
                {
                    line.Count++;
                    var item = children[i];
                    var cc = Cache.GetCache(item.StyleId, cache, document, parentCache);
                    if (changedFont)
                    {
                        item.IsMeasureValid = false;
                    }
                    item.Arrange(cc.Font, avs);

                    var flow = item as IFlowElement;
                    if (flow != null)
                    {
                        var run = item as DocumentChar;
                        if (run != null && run.Char == '\n')
                        {
                            //if (i == 0)//第一个如果是换行符
                            //{
                            //    line.Count = 0;
                            //    size.Height += line.Size.Height;
                            //    size.Width = Math.Max(line.Size.Width, size.Width);
                            //    lines.Add(line);
                            //    top += line.Size.Height;
                            //    l = 0;
                            //    line = new TextLine { Position = new Point(left, top), Start = i, Count = 1, Size = new Size(0, run.Size.Height) };
                            //}
                            line.Count--;
                            size.Height += line.Height;
                            size.Width = Math.Max(line.Width, size.Width);
                            lines.Add(line);
                            top += line.Height;
                            Rtl(line, rtl, charLeft);
                            SetY(line);

                            charLeft = 0;
                            line = new TextLine { X = left, Y = top, Start = i, Count = 1, Width = 0, Height = run.Height + run.Bottom };//cc.Font.FontSize
                        }
                        else
                        {
                            if (WordWarp && line.Width + flow.Width + flow.Right > width)
                            {
                                line.Count--;
                                size.Height += line.Height;
                                size.Width = Math.Max(line.Width, size.Width);
                                line.Line = false;
                                lines.Add(line);
                                top += line.Height;

                                Rtl(line, rtl, charLeft);
                                SetY(line);

                                charLeft = 0;
                                line = new TextLine { X = left, Y = top, Start = i, Count = 1 };

                            }

                            var ui = item as InlineUIContainer;
                            var offset = new Point();
                            if (ui != null)
                            {
                                ui.Line = line;
                            }
                            else
                            {
                                var inblock = item as InlineBlock;
                                if (inblock != null)
                                {
                                    inblock.Line = line;
                                    offset = new Point(inblock.Margin.Left, inblock.Margin.Top);
                                }
                            }
                            if (flow.FlowDirection == FlowDirection.RightToLeft || (rtl.Count > 0 && flow.FlowDirection == FlowDirection.Auto))
                            {
                                rtl.Add(flow);
                            }
                            else
                            {
                                charLeft = Rtl(line, rtl, charLeft);
                                flow.Left = charLeft + offset.X;
                                flow.Top = offset.Y;
                                var ass = flow.Width + flow.Right;
                                charLeft += ass;
                                line.Width += ass;
                                line.Height = Math.Max(line.Height, flow.Height + flow.Bottom);
                            }

                        }
                    }
                    else
                    {
                        if (line.Count != 1)
                        {
                            line.Count--;
                            size.Height += line.Height;
                            size.Width = Math.Max(line.Width, size.Width);
                            lines.Add(line);
                            top += line.Height;

                            Rtl(line, rtl, charLeft);
                            SetY(line);

                            charLeft = 0;
                            line = new TextLine { X = left, Y = top, Start = i, Count = 1 };
                        }
                        if (item is Block block)
                        {
                            item.Left = block.Margin.Left;
                            item.Top = block.Margin.Top;
                            block.Line = line;
                        }

                        line.Width = item.Width + item.Right;
                        line.Height = item.Height + item.Bottom;
                        size.Height += line.Height;
                        size.Width = Math.Max(line.Width, size.Width);
                        lines.Add(line);
                        top += line.Height;
                        line = new TextLine { X = left, Y = top, Start = i + 1 };
                    }
                }
                if (line.Count > 0)
                {
                    Rtl(line, rtl, charLeft);
                    //var s = line.Size;
                    size.Height += line.Height;
                    size.Width = Math.Max(line.Width, size.Width);
                    lines.Add(line);
                    SetY(line);
                }
                lastCharLeft = charLeft;
                lastLine = line;
                lastRtl = rtl;
                lastIndex = children.Count;
                oldSize = size;
                var textAlin = TextAlignment;
                if (!W.IsAuto && (!float.IsInfinity(availableSize.Width) || W.Unit == Unit.Default))
                {
                    var w = W.GetActualValue(availableSize.Width);
                    size.Width = w - padding.Left - padding.Right;
                }
                if (!maxW.IsAuto && (!float.IsInfinity(availableSize.Width) || maxW.Unit == Unit.Default))
                {
                    size.Width = Math.Min(maxW.GetActualValue(availableSize.Width) - padding.Left - padding.Right, size.Width);
                }
                if (!minw.IsAuto && (!float.IsInfinity(availableSize.Width) || minw.Unit == Unit.Default))
                {
                    size.Width = Math.Max(minw.GetActualValue(availableSize.Width) - padding.Left - padding.Right, size.Width);
                }
                if (textAlin != TextAlignment.Left)
                {
                    if (textAlin == TextAlignment.Right)
                    {
                        foreach (var item in lines)
                        {
                            item.X = size.Width - item.Width + padding.Left;
                        }
                    }
                    else
                    {
                        foreach (var item in lines)
                        {
                            item.X = (size.Width - item.Width) / 2;
                        }
                    }
                }
                size.Height += padding.Bottom + padding.Top;// + margin.Top + margin.Bottom;
                size.Width += padding.Left + padding.Right;// + margin.Left + margin.Right;
                foreach (var item in cache)
                {
                    item.Value.Dispose();
                }
            }
            if (!minw.IsAuto)
            {
                size.Width = Math.Max(minw.GetActualValue(availableSize.Width) - padding.Left - padding.Right, size.Width);
            }
            oldAvailableSize = availableSize;
        }

        public Size Size
        {
            get { return size; }
        }
        [NotCpfProperty]
        float IDocumentElement.Width
        {
            get { return size.Width; }
        }

        [NotCpfProperty]
        public float Height
        {
            get { return size.Height; }
        }


        private void SetY(TextLine line)
        {
            for (int j = line.Start; j < line.Start + line.Count; j++)
            {
                var c = children[j];
                var inblock = c as InlineBlock;
                if (inblock != null)
                {
                    //c.X = c.X;
                    c.Top = line.Height - c.Height + inblock.Margin.Top;
                }
                else
                {
                    c.Top = line.Height - c.Height;
                }
            }
        }

        private static float Rtl(TextLine line, List<IFlowElement> rtl, float l)
        {
            if (rtl.Count > 0)
            {
                for (int j = rtl.Count - 1; j > -1; j--)
                {
                    var r = rtl[j];
                    r.Left = l;
                    r.Top = 0;
                    var asss = r.Width;
                    l += asss;
                    line.Width += asss;
                    line.Height = Math.Max(line.Height, r.Height + r.Bottom);
                }
            }
            rtl.Clear();
            return l;
        }

        public IEnumerator GetEnumerator()
        {
            return children.GetEnumerator();
        }

        //protected virtual void OnItemAdded(IDocumentElement item)
        //{

        //}

        //protected virtual void OnItemRemoved(IDocumentElement item)
        //{

        //}

        /// <summary>
        /// 限定布局宽度，自动换行或者居中或者右靠的时候有用
        /// </summary>
        [PropertyMetadata(typeof(FloatField), "auto")]
        public virtual FloatField Width
        {
            get { return GetValue<FloatField>(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(typeof(FloatField), "auto")]
        public FloatField MaxWidth
        {
            get { return GetValue<FloatField>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(typeof(FloatField), "auto")]
        public FloatField MinWidth
        {
            get { return GetValue<FloatField>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 自动换行
        /// </summary>
        [PropertyMetadata(false)]
        public bool WordWarp { get { return GetValue<bool>(); } set { SetValue(value); } }

        /// <summary>
        /// 外间距
        /// </summary>
        public virtual Thickness Margin { get { return GetValue<Thickness>(); } set { SetValue(value); } }
        /// <summary>
        /// 内间距
        /// </summary>
        public Thickness Padding { get { return GetValue<Thickness>(); } set { SetValue(value); } }
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        [PropertyMetadata(TextAlignment.Left)]
        public TextAlignment TextAlignment { get { return GetValue<TextAlignment>(); } set { SetValue(value); } }

        public float Right => Margin.Right;

        public float Bottom => Margin.Bottom;

        [PropertyChanged(nameof(WordWarp))]
        void OnWordWarp(object newV, object old, PropertyMetadataAttribute attribute)
        {
            InvalidateArrange();
        }
        //[UIPropertyMetadata(TextOverflow.Clip, UIPropertyOptions.AffectsRender)]
        //public TextOverflow Overflow
        //{
        //    get { return GetValue<TextOverflow>(); }
        //    set { SetValue(value); }
        //}
    }

    /// <summary>
    /// 布局好的行
    /// </summary>
    public class TextLine
    {
        public float X;

        public float Y;

        public float Width;

        public float Height;

        public int Start;

        public int Count;
        /// <summary>
        /// 是否为新行也就是上一个以\n结尾的
        /// </summary>
        public bool Line = true;
    }

    public enum TextOverflow : byte
    {
        Clip,
        Ellipsis,
    }
}
