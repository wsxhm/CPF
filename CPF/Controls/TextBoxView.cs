using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Documents;
using CPF.Input;
using CPF.Shapes;
using CPF.Threading;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// TextBox内部容器
    /// </summary>
    [Description("TextBox内部容器")]
    public class TextBoxView : ITextBoxView
    {
        public TextBoxView(TextBox textBox)
        {
            textbox = textBox;
            Children.Add(line);
            var b = line[nameof(Line.StrokeFill)] <= textBox[nameof(TextBox.CaretFill)];
            textbox.GotFocus += Textbox_GotFocus;
            textbox.LostFocus += Textbox_LostFocus;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (line.Visibility == Visibility.Collapsed)
            {
                var point = GetPostion(textbox.CaretIndex, out float length);
                line.StartPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y);
                line.EndPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y + length);
                line.Visibility = Visibility.Visible;

                var p = this.PointToView(point);
                Root?.ViewImpl.SetIMEPosition(p);
            }
            else
            {
                line.Visibility = Visibility.Collapsed;
            }
        }

        bool updateCaretPosition = false;
        public override void UpdateCaretPosition()
        {
            updateCaretPosition = true;
        }

        public override void _ShowCaret()
        {
            line.Visibility = Visibility.Collapsed;
        }
        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            if (updateCaretPosition)
            {
                updateCaretPosition = false;
                var point = GetPostion(textbox.CaretIndex, out float length);
                //line.StartPoint = point;
                //line.EndPoint = new Point(point.X, point.Y + length);
                line.StartPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y);
                line.EndPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y + length);
            }
        }
        /// <summary>
        /// 获取索引处坐标
        /// </summary>
        /// <param name="index"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public override Point GetPostion(IList<uint> index, out float height)
        {
            if (index.Count == 0)
            {
                index.Add(0);
            }
            if (index.Count == 1 && index[0] == 0 && Document.Lines.Count > 0)
            {
                height = document.Lines[0].Height;
                return new Point(document.Lines[0].X + 1, document.Lines[0].Y);
            }
            IDocumentContainer idoc = Document;
            Point point = new Point();
            height = textbox.FontSize * 1.2f;
            foreach (var i in index)
            {
                for (int r = 0; r < idoc.Lines.Count; r++)
                {
                    var line = idoc.Lines[r];
                    if (line.Start <= i && line.Start + line.Count > i)
                    {
                        var ii = Math.Min((int)i, idoc.Children.Count - 1);
                        if (ii < 0)
                        {
                            return point;
                        }
                        point.Offset(line.X, line.Y);
                        var item = idoc.Children[ii];
                        var idc = item as IDocumentContainer;
                        point.Offset(item.Left, item.Top);
                        if (idc != null)
                        {
                            idoc = idc;
                            break;
                        }
                        else
                        {
                            if (item is DocumentChar c && c.Char == '\n' && i != 0)
                            {//如果是换行符，就返回上一行最后
                                point.Offset(-line.X, -line.Y);
                                point.Offset(-item.Left, -item.Top);
                                var l = idoc.Lines[r - 1];
                                point.Offset(l.X, l.Y);
                                var e = idoc.Children[l.Start + l.Count - 1];
                                point.Offset(e.Left + e.Width, e.Top);
                                height = e.Height;
                                return point;
                            }
                            else
                            {
                                height = item.Height;
                                return point;
                            }
                        }
                    }
                }
            }
            if (document.Children.Count > 0)
            {
                point = new Point();
                idoc = Document;
                var w = 0f;
                for (int i = 0; i < index.Count; i++)
                {
                    foreach (var line in idoc.Lines)
                    {
                        if (line.Start <= index[i] && ((index.Count - 1 == i && line.Start + line.Count >= index[i]) || (line.Start + line.Count > index[i])))
                        {
                            var ix = index[i];
                            if (ix >= idoc.Children.Count)
                            {
                                ix = (uint)(idoc.Children.Count - 1);
                            }
                            var item = idoc.Children[(int)ix];
                            point.Offset(line.X, line.Y);
                            point.Offset(item.Left, item.Top);
                            var idc = item as IDocumentContainer;
                            height = item.Height;
                            w = item.Width;
                            if (idc != null)
                            {
                                idoc = idc;
                                break;
                            }
                            else
                            {
                                point.Offset(w, 0);
                                return point;
                            }
                        }
                    }
                }
                //point.Offset(w, 0);
                return point;
            }
            else
            {
                if (textbox.TextAlignment == TextAlignment.Left)
                {
                    return new Point(Document.Margin.Left + Document.Padding.Left + 1, Document.Margin.Top + Document.Padding.Top);
                }
                else if (textbox.TextAlignment == TextAlignment.Center)
                {
                    return new Point(((IDocumentElement)Document).Width / 2, Document.Margin.Top + Document.Padding.Top);
                }
                else
                {
                    return new Point(((IDocumentElement)Document).Width - 1, Document.Margin.Top + Document.Padding.Top);
                }
            }
        }

        private void Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            line.Visibility = Visibility.Collapsed;
        }

        private void Textbox_GotFocus(object sender, GotFocusEventArgs e)
        {
            if (!textbox.IsReadOnly && !DesignMode)
            {
                timer.Start();
            }
            else
            {
                line.Visibility = Visibility.Collapsed;
            }
        }

        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

        Line line = new Line() { ZIndex = 100, Visibility = Visibility.Collapsed, Name = "Caret", UseLayoutRounding = true };

        TextBox textbox;

        Document document;
        public override Document Document
        {
            get
            {
                if (document == null)
                {
                    document = new Document(this);
                    document.Padding = new Thickness(1);
                    (document.Children as Collection<IDocumentElement>).CollectionChanged += TextBoxView_CollectionChanged;
                }
                return document;
            }
        }

        internal static object GetPostion(uint caretIndex, out float height)
        {
            throw new NotImplementedException();
        }

        private void TextBoxView_CollectionChanged(object sender, CollectionChangedEventArgs<IDocumentElement> e)
        {
            if (e.NewItem is IDocumentContainer && !textbox.IsReadOnly)
            {
                throw new Exception("编辑模式下不能加元素容器，IsReadOnly改成true");
            }
        }

        public override string FontFamily
        {
            get
            {
                return textbox.FontFamily;
            }
            set
            {
                textbox.FontFamily = value;
            }
        }

        public override float FontSize
        {
            get
            {
                return textbox.FontSize;
            }
            set
            {
                textbox.FontSize = value;
            }
        }

        public override FontStyles FontStyle
        {
            get
            {
                return textbox.FontStyle;
            }
            set
            {
                textbox.FontStyle = value;
            }
        }

        public override ViewFill Foreground
        {
            get
            {
                return textbox.Foreground;
            }
            set
            {
                textbox.Foreground = value;
            }
        }

        public override ViewFill Background
        {
            get
            {
                return textbox.Background;
            }
            set
            {
                textbox.Background = value;
            }
        }
        public override TextDecoration TextDecoration
        {
            get
            {
                return textbox.TextDecoration;
            }
            set
            {
                textbox.TextDecoration = value;
            }
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            if (document != null)
            {
                if ((textbox.TextAlignment != TextAlignment.Left || document.WordWarp) && !float.IsPositiveInfinity(availableSize.Width))
                {
                    document.MaxWidth = availableSize.Width;
                }
                using (var font = new Font(textbox.FontFamily, textbox.FontSize, textbox.FontStyle))
                {
                    document.Arrange(font, availableSize);
                    line.Measure(availableSize);
                    if (document.Children.Count == 0)
                    {
                        return new Size(1, font.LineHeight);
                    }
                    return new Size(((IDocumentElement)document).Width, document.Height);
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            if (document != null)
            {
                using (var font = new Font(textbox.FontFamily, textbox.FontSize, textbox.FontStyle))
                {
                    if (textbox.TextAlignment != TextAlignment.Left)
                    {
                        document.Width = finalSize.Width;
                    }
                    document.Arrange(font, finalSize);
                    foreach (var child in document.UIContainers)
                    {
                        child.UIElement.Arrange(new Rect(child.ActualPositon, new Size(child.Width, child.Height)));
                    }
                    line.Arrange(new Rect(finalSize));
                    //return finalSize;
                    if (document.Children.Count == 0)
                    {
                        return new Size(1, font.LineHeight);
                    }
                    return new Size(((IDocumentElement)document).Width, document.Height);
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        bool isMouseLeftDown;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            textbox.Focus(NavigationMethod.Click);
            base.OnMouseDown(e);
            if (document != null && e.LeftButton == MouseButtonState.Pressed)
            {
                isMouseLeftDown = true;
                var list = new List<uint>();
                HitTest(e.Location, list);
                if (!e.Device.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Shift))
                {
                    textbox.SelectionEnd.Clear();
                }
                else if (textbox.SelectionEnd.Count == 0)
                {
                    foreach (var item in textbox.CaretIndex)
                    {
                        textbox.SelectionEnd.Add(item);
                    }
                }
                textbox.CaretIndex.Clear();

                foreach (var item in list)
                {
                    textbox.CaretIndex.Add(item);
                    if (!e.Device.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Shift))
                    {
                        textbox.SelectionEnd.Add(item);
                    }
                }
                //Invalidate();
                this.CaptureMouse();
                _ShowCaret();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isMouseLeftDown)
            {
                HitTest(e.Location, textbox.CaretIndex);
                var point = GetPostion(textbox.CaretIndex, out float length);
                //line.StartPoint = point;
                //line.EndPoint = new Point(point.X, point.Y + length);
                line.StartPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y);
                line.EndPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y + length);
                Invalidate();
                textbox.ScrollToCaret();
            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.MouseButton == MouseButton.Left)
            {
                isMouseLeftDown = false;
            }
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.IsTouch)
            {
                e.Handled = true;
            }
            base.OnMouseWheel(e);
        }
        public override void HitTest(Point mosPos, IList<uint> index)
        {
            index.Clear();
            HitTest(document, mosPos, index, out bool left);
            if (index.Count == 0)
            {
                index.Add(0);
            }
            if (!left)
            {
                index[index.Count - 1] = index[index.Count - 1] + 1;
            }
        }

        public override IDocumentElement HitTestElement(Point mosPos)
        {
            var list = new List<uint>();
            HitTest(mosPos, list);
            if (list.Count == 0)
            {
                return null;
            }
            IDocumentContainer container = Document;
            for (int i = 0; i < list.Count; i++)
            {
                var index = (int)list[i];
                if (index < 0 || index > container.Children.Count - 1)
                {
                    return null;
                }
                if (i == list.Count - 1)
                {
                    return container.Children[index];
                }
                var item = container.Children[index];
                if (item is IDocumentContainer)
                {
                    container = item as IDocumentContainer;
                }
                else
                {
                    return item;
                }
            }
            return null;
        }

        private static bool HitTest(IDocumentContainer doc, Point mosPos, IList<uint> index, out bool isLeft)
        {
            isLeft = true;
            for (int i = doc.Lines.Count - 1; i >= 0; i--)
            {
                var item = doc.Lines[i];
                if (mosPos.Y > item.Y)//&& mosPos.Y < item.Position.Y + item.Size.Height
                {
                    for (int j = item.Count + item.Start - 1; j >= item.Start; j--)
                    {
                        if (j >= doc.Children.Count)
                        {
                            return false;
                        }
                        var de = doc.Children[j];
                        var pos = new Point(de.Left + item.X, de.Top + item.Y);
                        if (pos.X < mosPos.X)// && pos.X + de.Size.Width > mosPos.X
                        {
                            if (de is IDocumentContainer dc)
                            {
                                index.Add((uint)j);
                                if (HitTest(dc, new Point(mosPos.X - pos.X, mosPos.Y - pos.Y), index, out isLeft))
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                //if (!(de is DocumentChar c && c.Char == '\n'))
                                {
                                    //System.Diagnostics.Debug.WriteLine(j);
                                    if (pos.X + de.Width / 2 < mosPos.X)
                                    {
                                        isLeft = false;
                                    }
                                    index.Add((uint)j);
                                    return true;
                                }
                            }
                        }
                    }

                    if (!(doc.Children[item.Start] is DocumentChar c && c.Char == '\n') || (item.Count == 0 && i == 0))
                    {
                        index.Add((uint)item.Start);
                    }
                    else
                    {
                        index.Add((uint)item.Start + 1);
                    }
                    return true;
                    //return false;
                }
            }
            return false;
        }

        Brush selectionFill;
        Brush selectionTextFill;
        List<int> renderPosition = new List<int>();
        int selectionComparer;
        //List<KeyValuePair<Point, IDocumentElement>> selectElements = new List<KeyValuePair<Point, IDocumentElement>>();

        bool InSelectRange(int index)
        {
            if (selectionComparer > 0)
            {
                if (Comparer(index, textbox.CaretIndex) < 0 && Comparer(index, textbox.SelectionEnd) >= 0)
                {
                    return true;
                }
            }
            else if (selectionComparer < 0)
            {
                if (Comparer(index, textbox.CaretIndex) >= 0 && Comparer(index, textbox.SelectionEnd) < 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var size = textbox.ActualSize;
            var rect = new Rect(new Point(-ActualOffset.X, -ActualOffset.Y), size);
            if (document != null)
            {
                selectionComparer = Comparer(textbox.CaretIndex, textbox.SelectionEnd);
                selectionFill = textbox.SelectionFill?.CreateBrush(rect, Root.RenderScaling);
                selectionTextFill = textbox.SelectionTextFill?.CreateBrush(rect, Root.RenderScaling);

                //selectElements.Clear();
                HybridDictionary<int, Cache> cache = new HybridDictionary<int, Cache>();
                //var rect = new Rect(new Point(), document.Size);
                var v = new Cache
                {
                    //Background = owner.Background?.CreateBrush(rect),
                    Foreground = textbox.Foreground?.CreateBrush(rect, Root.RenderScaling),
                    Font = new Font(textbox.FontFamily, textbox.FontSize, textbox.FontStyle),
                    TextDecoration = textbox.TextDecoration,
                };
                renderPosition.Clear();
                Render(dc, rect, new Point(), cache, document, v);

                //selectElements.Clear();
                v.Dispose();
                foreach (var item in cache)
                {
                    item.Value.Dispose();
                }
                if (selectionFill != null)
                {
                    selectionFill.Dispose();
                    selectionFill = null;
                }
                if (selectionTextFill != null)
                {
                    selectionTextFill.Dispose();
                    selectionTextFill = null;
                }
            }
        }


        void Render(DrawingContext dc, Rect source, Point location, HybridDictionary<int, Cache> cache, IDocumentContainer documentContainer, Cache parent)
        {
            float lastOffset = 0;
            float lastTop = 0;
            float lastHeight = 0;
            foreach (var line in documentContainer.Lines)
            {
                var p = new Point(location.X + line.X, location.Y + line.Y);
                if (p.Y < source.Bottom && p.Y + line.Height > source.Y)
                {
                    Rect backRect = new Rect();
                    Brush backBrush = null;
                    for (int i = line.Start; i < line.Start + line.Count; i++)
                    {
                        var item = documentContainer.Children[i];
                        var point = new Point(p.X + item.Left, p.Y + item.Top);
                        var cc = Cache.GetCache(item.StyleId, cache, source, dc, document, parent);
                        if (!(item is IDocumentContainer))
                        {
                            bool isSelect = false;
                            if (InSelectRange(i))
                            {
                                isSelect = true;
                                //selectElements.Add(new KeyValuePair<Point, IDocumentElement>(point, item));
                            }
                            //if (cc.Background != null || isSelect)
                            //{
                            if (backBrush != null && backRect.Width > 0 && backRect.Height > 0 && ((backBrush != cc.Background && !isSelect) || (isSelect && backBrush != selectionFill)))
                            {
                                dc.FillRectangle(backBrush, backRect);
                                backRect = new Rect();
                            }
                            backBrush = cc.Background;
                            if (isSelect)
                            {
                                backBrush = selectionFill;
                            }
                            if (item is DocumentChar c && c.Char == '\n')
                            {
                                if (isSelect && backBrush != null)
                                {
                                    if (lastHeight == 0)
                                    {
                                        lastHeight = item.Height;
                                    }
                                    dc.FillRectangle(backBrush, new Rect(new Point(lastOffset, lastTop), new Size(3, lastHeight)));
                                }
                                continue;
                            }
                            if ((isSelect && selectionFill != null) || (cc.Background != null))
                            {
                                document.Union(ref backRect, new Rect(point, new Size(item.Width, item.Height)));
                            }
                            //}
                        }
                        lastHeight = item.Height;
                    }

                    if (backBrush != null && backRect.Width > 0 && backRect.Height > 0)
                    {
                        dc.FillRectangle(backBrush, backRect);
                    }
                }
                lastOffset = document.Left + line.X + line.Width;
                lastTop = document.Top + line.Y;
            }
            foreach (var line in documentContainer.Lines)
            {
                var p = new Point(location.X + line.X, location.Y + line.Y);
                if (p.Y < source.Bottom && p.Y + line.Height > source.Y)
                {
                    for (int i = line.Start; i < line.Start + line.Count; i++)
                    {
                        var item = documentContainer.Children[i];
                        var point = new Point(p.X + item.Left, p.Y + item.Top);
                        var cc = Cache.GetCache(item.StyleId, cache, source, dc, document, parent);
                        var con = item as IDocumentContainer;
                        if (con != null)
                        {
                            var rect = new Rect(point, new Size(item.Width, item.Height));
                            if (con.Background != null)
                            {
                                using (var brush = con.Background.CreateBrush(rect, Root.RenderScaling))
                                {
                                    dc.FillRectangle(brush, rect);
                                }
                            }
                            rect.Intersect(source);
                            renderPosition.Add(i);
                            Render(dc, rect, point, cache, con, cc);
                            renderPosition.RemoveAt(renderPosition.Count - 1);
                        }
                        else
                        {
                            var c = item as DocumentChar;
                            var str = "";
                            if (c != null)
                            {
                                str = c.Char.ToString();
                            }
                            else
                            {
                                var u = item as UTF32Text;
                                if (u != null)
                                {
                                    str = u.Text;
                                }
                            }
                            var brush = cc.Foreground;
                            if (InSelectRange(i) && selectionTextFill != null)
                            {
                                brush = selectionTextFill;
                            }
                            if (!string.IsNullOrEmpty(str) && brush != null && str != "\n" && str != "\t")
                            {
                                dc.DrawString(point, brush, str, cc.Font, TextAlignment.Left, float.MaxValue, cc.TextDecoration);
                            }
                        }
                    }

                }

            }
        }


        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ClipToBounds), new PropertyMetadataAttribute(true)); overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(MinHeight), new UIPropertyMetadataAttribute(typeof(FloatField), "100%", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MinWidth), new UIPropertyMetadataAttribute(typeof(FloatField), "100%", UIPropertyOptions.AffectsMeasure));
            //overridePropertys.Override(nameof(MarginBottom), new UIPropertyMetadataAttribute(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginLeft), new UIPropertyMetadataAttribute(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure));
            //overridePropertys.Override(nameof(MarginRight), new UIPropertyMetadataAttribute(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(MarginTop), new UIPropertyMetadataAttribute(typeof(FloatField), "0", UIPropertyOptions.AffectsMeasure));
        }
        /// <summary>
        /// 对比，大于0，第一个参数在第二个参数后面
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public override int Comparer(IList<uint> index1, IList<uint> index2)
        {
            if (index1.Count > 0 && index2.Count > 0)
            {
                for (int i = 0; i < Math.Min(index1.Count, index2.Count); i++)
                {
                    if (index1[i] > index2[i])
                    {
                        return 1;
                    }
                    else if (index1[i] < index2[i])
                    {
                        return -1;
                    }
                }
                return index1.Count - index2.Count;
            }
            return 0;
        }

        int Comparer(int last, IList<uint> index)
        {
            if (index.Count > 0)
            {
                for (int i = 0; i < Math.Min(index.Count, renderPosition.Count + 1); i++)
                {
                    if (i < renderPosition.Count)
                    {
                        if (renderPosition[i] > index[i])
                        {
                            return 1;
                        }
                        else if (renderPosition[i] < index[i])
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (last > index[i])
                        {
                            return 1;
                        }
                        else if (last < index[i])
                        {
                            return -1;
                        }
                    }
                }
                return renderPosition.Count + 1 - index.Count;
            }
            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            timer.Dispose();
        }


    }
}
