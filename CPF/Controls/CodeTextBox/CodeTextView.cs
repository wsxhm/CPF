using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Documents;
using CPF.Drawing;
using CPF.Input;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace CPF.Controls
{
    public class CodeTextView : UIElement, IScrollInfo
    {
        CodeTextBox textBox;
        public CodeTextView(CodeTextBox textBox)
        {
            this.textBox = textBox;
            Width = "100%";
            Height = "100%";
            Cursor = Cursors.Ibeam;
            timer.Tick += Timer_Tick;
            textBox.GotFocus += Textbox_GotFocus;
            textBox.LostFocus += Textbox_LostFocus;
        }
        bool showCaret;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!showCaret)
            {
                showCaret = true;
                var point = GetPostion(textBox.CaretIndex, out float length);
                //line.StartPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y);
                //line.EndPoint = new Point(point.X - line.StrokeStyle.Width / 2, point.Y + length);
                //line.Visibility = Visibility.Visible;

                var p = this.PointToView(point);
                Root?.ViewImpl.SetIMEPosition(p);
            }
            else
            {
                showCaret = false;
            }
            Invalidate();
        }
        private void Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            showCaret = false;
            Invalidate();
        }

        private void Textbox_GotFocus(object sender, GotFocusEventArgs e)
        {
            if (!textBox.IsReadOnly && !DesignMode)
            {
                timer.Start();
            }
            else
            {
                //line.Visibility = Visibility.Collapsed;
                showCaret = false;
            }
        }
        public event EventHandler ScrollChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }


        internal string Text = "";
        internal List<short> styles = new List<short>();
        List<List<float>> lineLayoutWidth = new List<List<float>>();
        CPF.Threading.DispatcherTimer timer = new Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

        /// <summary>
        /// 用于同步样式
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        internal void InsertText(uint index, string text)
        {
            textBox.Text = Text.Insert((int)index, text);
            if (index < styles.Count)
            {
                styles.InsertRange((int)index, text.Select(a => (short)-1));
            }
        }
        /// <summary>
        /// 用于同步样式
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        internal void RemoveText(uint index, int count)
        {
            textBox.Text = Text.Remove((int)index, count);
            if (index < styles.Count)
            {
                styles.RemoveRange((int)index, (int)Math.Min(count, styles.Count - index));
            }
        }


        [NotCpfProperty]
        public bool CanVerticallyScroll
        {
            get;
            set;
        }

        [NotCpfProperty]
        public bool CanHorizontallyScroll
        {
            get;
            set;
        }

        [NotCpfProperty]
        public float ExtentWidth
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ExtentHeight
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ViewportWidth
        {
            get; private set;
        }

        [NotCpfProperty]
        public float ViewportHeight
        {
            get; private set;
        }

        [NotCpfProperty]
        public float HorizontalOffset
        {
            get; private set;
        }

        [NotCpfProperty]
        public float VerticalOffset
        {
            get;
            private set;
        }

        [NotCpfProperty]
        public ScrollViewer ScrollOwner
        {
            get;
            set;
        }

        [PropertyMetadata(16f)]
        public float ScrollLineDelta
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(48f)]
        public float MouseWheelDelta
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        protected override Size ArrangeOverride(in Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(in Size availableSize)
        {
            using (var font = new Font(textBox.FontFamily, textBox.FontSize, textBox.FontStyle))
            {
                lines.Clear();
                var lineHeight = (float)Math.Round(font.LineHeight, 2);
                var top = 0d;//换成双精度减少误差
                var h = 0d;
                float left = 0;
                //if (textBox.WordWarp)
                //{

                //}
                //else
                //{
                Size size = new Size();
                if (!string.IsNullOrEmpty(Text))
                {
                    TextLine line = new TextLine { Width = 0, Height = lineHeight, };
                    for (int i = 0; i < Text.Length; i++)
                    {
                        var item = Text[i];
                        line.Count++;
                        if (item == '\n')
                        {
                            line.Count--;
                            h += line.Height;
                            //size.Width = Math.Max(line.Width, size.Width);
                            lines.Add(line);
                            top += line.Height;

                            line = new TextLine { X = left, Y = (float)top, Start = i, Count = 1, Width = 0, Height = lineHeight };
                        }
                        else
                        {

                        }
                    }
                    if (lines.Count == 0 || line != lines[lines.Count - 1])
                    {
                        h += line.Height;
                        lines.Add(line);
                    }
                }
                size.Height = (float)h;
                ExtentHeight = size.Height;
                ExtentWidth = size.Width;

                //}
            }
            return base.MeasureOverride(availableSize);
        }

        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            var size = ActualSize;
            LayoutLines();

            bool changed = false;
            //if (!extentHeight.Equal(ExtentHeight))
            //{
            //    ExtentHeight = extentHeight;
            //    changed = true;
            //}
            //if (!extentWidth.Equal(ExtentWidth))
            //{
            //    ExtentWidth = extentWidth;
            //    changed = true;
            //}
            if (!ViewportHeight.Equal(size.Height))
            {
                ViewportHeight = size.Height;
                changed = true;
            }
            if (!ViewportWidth.Equal(size.Width))
            {
                ViewportWidth = size.Width;
                changed = true;
            }
            if (CanVerticallyScroll != (!ExtentHeight.Equal(ViewportHeight) && ExtentHeight > ViewportHeight))
            {
                CanVerticallyScroll = (!ExtentHeight.Equal(ViewportHeight) && ExtentHeight > ViewportHeight);
                changed = true;
            }
            if (CanHorizontallyScroll != (!ExtentWidth.Equal(ViewportWidth) && ExtentWidth > ViewportWidth))
            {
                CanHorizontallyScroll = (!ExtentWidth.Equal(ViewportWidth) && ExtentWidth > ViewportWidth);
                changed = true;
            }
            if (changed)
            {
                if (scrollChanged)
                {
                    scrollChanged = false;
                    this.BeginInvoke(() =>
                    {
                        RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
                        scrollChanged = true;
                    });
                }
            }
        }

        void LayoutLines()
        {
            var size = ActualSize;
            using (var font = new Font(textBox.FontFamily, textBox.FontSize, textBox.FontStyle))
            {
                if (HorizontalOffset < 0 || ExtentWidth < ViewportWidth)
                {
                    HorizontalOffset = 0;
                }
                if (VerticalOffset < 0 || ExtentHeight < ViewportHeight)
                {
                    VerticalOffset = 0;
                }
                lineLayoutWidth.Clear();
                var lineHeight = (float)Math.Round(font.LineHeight, 2);
                var len = Math.Ceiling(size.Height / lineHeight);
                var start = (int)Math.Floor(VerticalOffset / lineHeight);
                textBox.LineNumber.Start = start;
                textBox.LineNumber.Count = Math.Max(1, Math.Min((int)len, lines.Count - start));
                textBox.LineNumber.LineHeight = lineHeight;
                if (start < lines.Count)
                {
                    textBox.LineNumber.Offset = lines[start].Y - VerticalOffset + textBox.Padding.Top;
                }
                else
                {
                    textBox.LineNumber.Offset = textBox.Padding.Top;
                }
                //System.Diagnostics.Debug.WriteLine($"VerticalOffset：{ VerticalOffset}，Start：{start},Len:{len}");
                for (int i = start; i < Math.Min(start + len, lines.Count); i++)
                {
                    var line = lines[i];

                    //ExtentWidth = Math.Max(DrawingFactory.Default.MeasureString(Text.Substring(line.Start, line.Count).Trim('\n'), font).Width, ExtentWidth);
                    var ws = new List<float>();
                    var ww = 0d;
                    for (int j = line.Start; j < line.Count + line.Start; j++)
                    {
                        if (Text[j] == '\n')
                        {
                            ws.Add(0);
                        }
                        else
                        {
                            var w = DocumentChar.GetCharSize(font, Text[j]).Width;
                            ww += w;
                            ws.Add(w);
                        }
                    }
                    //foreach (var item in Text.Substring(line.Start, line.Count))
                    //{
                    //    var w = DocumentChar.GetCharSize(font, item).Width;
                    //    ww += w;
                    //    ws.Add(w);
                    //}
                    lineLayoutWidth.Add(ws);
                    line.Width = (float)ww;
                    ExtentWidth = Math.Max((float)ww, ExtentWidth);
                }
                //if (HorizontalOffset > ExtentWidth - ViewportWidth && ExtentWidth > ViewportWidth)
                //{
                //    HorizontalOffset = ExtentWidth - ViewportWidth;
                //}
                //if (VerticalOffset > ExtentHeight - ViewportHeight && ExtentHeight > ViewportHeight)
                //{
                //    VerticalOffset = ExtentHeight - ViewportHeight;
                //}
            }
        }

        bool scrollChanged = true;

        protected override void OnRender(DrawingContext dc)
        {
            var fore = textBox.Foreground;
            if (fore == null)
            {
                return;
            }
            if (!IsArrangeValid)
            {
                LayoutLines();
            }
            var size = ActualSize;
            var rect = new Rect(new Point(), size);
            var ho = HorizontalOffset;
            var vo = VerticalOffset;
            using (var font = new Font(textBox.FontFamily, textBox.FontSize, textBox.FontStyle))
            {
                HybridDictionary<int, Cache> caches = new HybridDictionary<int, Cache>();
                var lineHeight = (float)Math.Round(font.LineHeight, 2);
                var len = Math.Ceiling(size.Height / lineHeight);
                var start = (int)Math.Floor(vo / lineHeight);

                var index = 0;
                Brush selectionFill = null;
                if (textBox.SelectionFill != null)
                {
                    selectionFill = textBox.SelectionFill.CreateBrush(rect, Root.RenderScaling);
                }
                Brush selectionTextFill = null;
                if (textBox.SelectionTextFill != null)
                {
                    selectionTextFill = textBox.SelectionTextFill.CreateBrush(rect, Root.RenderScaling);
                }

                float lastOffset = -ho;
                float lastTop = -vo;
                for (int i = start; i < Math.Min(start + len, lines.Count); i++)
                {
                    if (lineLayoutWidth.Count - 1 < index)
                    {
                        break;
                    }
                    var line = lines[i];
                    //var l = Text.Substring(line.Start, line.Count);
                    var left = 0f;
                    var lineWidth = lineLayoutWidth[index];
                    Rect backRect = new Rect();
                    Brush backBrush = null;
                    for (int j = 0; j < line.Count; j++)
                    {
                        var c = Text[line.Start + j];
                        var w = lineWidth[j];
                        if (!(left - ho > size.Width || left + w < ho) || c == '\n')
                        {
                            short styleId = -1;
                            if (styles.Count > line.Start + j)
                            {
                                styleId = styles[line.Start + j];
                            }
                            var cache = Cache.GetCache(styleId, caches, rect, textBox);
                            var point = new Point(-ho + line.X + left, -vo + line.Y);

                            bool isSelect = false;
                            if (IsInSelectRange(line.Start + j))
                            {
                                isSelect = true;
                            }
                            if (backBrush != null && backRect.Width > 0 && backRect.Height > 0 && ((backBrush != cache.Background && !isSelect) || (isSelect && backBrush != selectionFill)))
                            {
                                dc.FillRectangle(backBrush, backRect);
                                backRect = new Rect();
                            }
                            backBrush = cache.Background;
                            if (isSelect)
                            {
                                backBrush = selectionFill;
                            }
                            if (c == '\n')//绘制换行符选中背景
                            {
                                if (isSelect && backBrush != null)
                                {
                                    dc.FillRectangle(backBrush, new Rect(new Point(lastOffset, lastTop), new Size(3, lineHeight)));
                                }
                                continue;
                            }
                            if ((isSelect && selectionFill != null) || (cache.Background != null))
                            {
                                Union(ref backRect, new Rect(point, new Size(w, lineHeight)));
                            }

                        }
                        left += w;
                    }
                    if (backBrush != null && backRect.Width > 0 && backRect.Height > 0)
                    {
                        dc.FillRectangle(backBrush, backRect);
                    }
                    lastOffset = -ho + line.X + left;
                    lastTop = -vo + line.Y;
                    index++;
                }
                index = 0;
                for (int i = start; i < Math.Min(start + len, lines.Count); i++)
                {
                    if (lineLayoutWidth.Count - 1 < index)
                    {
                        break;
                    }
                    var line = lines[i];
                    //var l = Text.Substring(line.Start, line.Count);
                    var left = 1d;
                    var lineWidth = lineLayoutWidth[index];
                    for (int j = 0; j < line.Count; j++)
                    {
                        var c = Text[j + line.Start];
                        var w = lineWidth[j];
                        if (c != '\n' && c != '\r' && !(left - ho > size.Width || left + w < ho))
                        {
                            short styleId = -1;
                            if (styles.Count > line.Start + j)
                            {
                                styleId = styles[line.Start + j];
                            }
                            var cache = Cache.GetCache(styleId, caches, rect, textBox);
                            var foreBrush = cache.Foreground;
                            if (selectionTextFill != null && IsInSelectRange(line.Start + j))
                            {
                                foreBrush = selectionTextFill;
                            }
                            dc.DrawString(new Point(-ho + line.X + (float)left - 1.5f, -vo + line.Y), foreBrush, c.ToString(), font, decoration: cache.TextDecoration);
                        }
                        left += w;
                    }
                    index++;
                }
                foreach (var item in caches)
                {
                    item.Value.Dispose();
                }
                selectionTextFill?.Dispose();
                selectionFill?.Dispose();

                if (showCaret)
                {
                    var caretBrush = textBox.CaretFill;
                    if (caretBrush != null)
                    {
                        using (var b = caretBrush.CreateBrush(rect, Root.RenderScaling))
                        {
                            var point = GetPostion(textBox.CaretIndex, out _);
                            if (point.X == 0)
                            {
                                point.X = 1;
                            }
                            dc.DrawLine(new Drawing.Stroke { Width = 1 }, b, point, new Point(point.X, point.Y + lineHeight));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 光标位置是否在选中的文字里
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsInSelectRange(int index)
        {
            if (textBox.SelectionEnd != textBox.CaretIndex && ((index >= textBox.CaretIndex && index < textBox.SelectionEnd) || (index >= textBox.SelectionEnd && index < textBox.CaretIndex)))
            {
                return true;
            }
            return false;
        }
        void Union(ref Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty || rect2.Contains(rect1) || rect2 == rect1)//如果更新区域大于原有区域，则当前区域设为更新区域
            {
                rect1 = rect2;
            }
            else if (rect1.Contains(rect2))//如果更新区域小于原来的区域，则区域不变
            {

            }
            else if (rect1.IsEmpty)//如果原来的区域为空
            {
                rect1 = rect2;
            }
            else
            {//如果两个区域没有关联或者相交
                var minX = rect1.X < rect2.X ? rect1.X : rect2.X;//确定包含这两个矩形的最小矩形
                var minY = rect1.Y < rect2.Y ? rect1.Y : rect2.Y;
                var maxW = (rect1.Width + rect1.X - minX) > (rect2.Width + rect2.X - minX) ? (rect1.Width + rect1.X - minX) : (rect2.Width + rect2.X - minX);
                var maxH = (rect1.Height + rect1.Y - minY) > (rect2.Height + rect2.Y - minY) ? (rect1.Height + rect1.Y - minY) : (rect2.Height + rect2.Y - minY);
                Rect min = new Rect(minX, minY, maxW, maxH);

                rect1 = min;
            }
        }

        internal static List<short> RenderKeywords(CancellationToken cancellation, IList<KeywordsStyle> keywordsStyles, string text)
        {
            List<short> styles = new List<short>();
            if (keywordsStyles.Count > 0)
            {
                //styles.Clear();
                for (int i = 0; i < text.Length; i++)
                {
                    styles.Add(-1);
                }
            }
            foreach (KeywordsStyle k in keywordsStyles)
            {
                if (cancellation.IsCancellationRequested)
                {
                    return null;
                }
                int index = 0;
                if (!string.IsNullOrEmpty(k.Keywords))
                {
                    if (k.IsRegex)
                    {
                        RegexOptions ro = k.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
                        MatchCollection mc = Regex.Matches(text, k.Keywords, ro);
                        foreach (Match item in mc)
                        {
                            for (int i = item.Index; i < item.Index + item.Length; i++)
                            {
                                if (cancellation.IsCancellationRequested)
                                {
                                    return null;
                                }
                                if (i < text.Length)
                                {
                                    styles[i] = k.StyleId;
                                }
                            }
                        }
                    }
                    else
                    {
                        StringComparison sc = k.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                        while ((index = text.IndexOf(k.Keywords, index, sc)) > -1)
                        {
                            for (int i = index; i < index + k.Keywords.Length; i++)
                            {
                                if (cancellation.IsCancellationRequested)
                                {
                                    return null;
                                }
                                if (i < text.Length)
                                {
                                    styles[i] = k.StyleId;
                                }
                            }
                            index++;
                        }
                    }
                }
            }
            return styles;
        }

        /// <summary>
        /// 获取索引处坐标，如果位置不在可视范围内，那X值将不是精确值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="lineHeight"></param>
        /// <returns></returns>
        public Point GetPostion(uint index, out float lineHeight)
        {
            using (var font = new Font(textBox.FontFamily, textBox.FontSize, textBox.FontStyle))
            {
                lineHeight = (float)Math.Round(font.LineHeight, 2);
                var ho = HorizontalOffset;
                var vo = VerticalOffset;
                var start = (int)Math.Floor(vo / lineHeight);
                if (lines.Count == 0)
                {
                    return new Point(-ho, -vo);
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    var item = lines[i];
                    if (item.Start <= index && index <= item.Start + item.Count)
                    {
                        if (index == item.Start + item.Count)
                        {
                            return new Point(item.X - ho + item.Width, item.Y - vo);
                        }
                        else if (start <= i && i < start + lineLayoutWidth.Count)
                        {
                            return new Point(item.X - ho + (index == item.Start ? 0 : lineLayoutWidth[i - start].Take((int)index - item.Start).Sum()), item.Y - vo);
                        }
                        else
                        {
                            return new Point(item.X - ho, item.Y - vo);
                        }
                    }
                }
                var line = lines[lines.Count - 1];
                //if (line.Start <= index && index <= line.Start + line.Count)
                //{
                //    return new Point(line.X - ho + (index == line.Start ? 0 : lineLayoutWidth[lineLayoutWidth.Count - 1].Take((int)index - line.Start).Sum()), line.Y - vo);
                //}
                //else
                //{
                return new Point(line.X - ho, line.Y - vo);
                //}
            }
        }

        /// <summary>
        /// 通过鼠标坐标获取光标索引位置
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public uint GetCareIndex(Point point)
        {
            if (lines.Count == 0)
            {
                return 0;
            }
            //var size = new Size(ViewportWidth, ViewportHeight);
            using (var font = new Font(textBox.FontFamily, textBox.FontSize, textBox.FontStyle))
            {
                var lineHeight = Math.Round(font.LineHeight, 2);
                var ho = (double)HorizontalOffset + point.X;
                var vo = (double)VerticalOffset;
                var start = (int)Math.Floor(vo / lineHeight);

                vo = vo + point.Y;
                var lineIndex = Math.Max(0, Math.Min((int)(vo / lineHeight), lines.Count - 1));
                if (lineIndex >= start && lineIndex < start + lineLayoutWidth.Count)
                {
                    var ws = lineLayoutWidth[lineIndex - start];
                    var left = 0f;
                    var lastW = 0f;
                    for (int i = 0; i < ws.Count; i++)
                    {
                        if (i != 0)
                        {
                            lastW = ws[i - 1];
                        }
                        if (ho > left - lastW / 2 && ho < ws[i] / 2 + left)
                        {
                            return (uint)(lines[lineIndex].Start + i);
                        }
                        else if (i == ws.Count - 1)
                        {
                            if (i == 0 && ho < left)
                            {
                                return 0;
                            }
                            return (uint)(lines[lineIndex].Start + i + 1);
                        }
                        else if (ho < left && i == 0 && lineIndex != 0)
                        {
                            return (uint)lines[lineIndex].Start + 1;
                        }
                        else if (ho < left && i == 0 && lineIndex == 0)
                        {
                            return 0;
                        }
                        left += ws[i];
                    }
                    return (uint)lines[lineIndex].Start;
                }
                else
                {
                    return (uint)lines[lineIndex].Start;
                }
            }
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


        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ScrollLineDelta);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ScrollLineDelta);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ScrollLineDelta);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + ScrollLineDelta);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset + MouseWheelDelta);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset - MouseWheelDelta);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - MouseWheelDelta);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + MouseWheelDelta);
        }

        public void SetHorizontalOffset(float offset)
        {
            HorizontalOffset = offset;
            if (HorizontalOffset < 0 || ExtentWidth < ViewportWidth)
            {
                HorizontalOffset = 0;
            }
            else if (HorizontalOffset > ExtentWidth - ViewportWidth)
            {
                HorizontalOffset = ExtentWidth - ViewportWidth;
            }
            RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            InvalidateArrange();
        }

        public void SetVerticalOffset(float offset)
        {
            VerticalOffset = offset;
            if (VerticalOffset < 0 || ExtentHeight < ViewportHeight)
            {
                VerticalOffset = 0;
            }
            else if (VerticalOffset > ExtentHeight - ViewportHeight)
            {
                VerticalOffset = ExtentHeight - ViewportHeight;
            }
            RaiseEvent(EventArgs.Empty, nameof(ScrollChanged));
            InvalidateArrange();
        }

        bool isMouseLeftDown;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                isMouseLeftDown = true;
                textBox.Focus();
                this.CaptureMouse();
                if (e.Device.InputManager.KeyboardDevice.Modifiers.HasFlag(InputModifiers.Shift))
                {
                    //textBox.SelectionEnd = textBox.CaretIndex;
                    textBox.CaretIndex = GetCareIndex(e.Location);
                }
                else
                {
                    textBox.CaretIndex = GetCareIndex(e.Location);
                    textBox.SelectionEnd = textBox.CaretIndex;
                }
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isMouseLeftDown)
            {
                textBox.CaretIndex = GetCareIndex(e.Location);
                Invalidate();
                textBox.ScrollToCaret();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                isMouseLeftDown = false;
                this.ReleaseMouseCapture();
            }

            base.OnMouseUp(e);
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(IsAntiAlias), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender | UIPropertyOptions.Inherits));
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName==nameof(ClipToBounds))
        //    {

        //    }
        //}


        class Cache
        {
            public static Cache GetCache(short styleId, HybridDictionary<int, Cache> cache, Rect rect, CodeTextBox codeTextBox)
            {
                if (!cache.TryGetValue(styleId, out Cache v))
                {
                    if (styleId < 0 || styleId >= codeTextBox.Styles.Count)
                    {
                        v = new Cache
                        {
                            Foreground = codeTextBox.Foreground?.CreateBrush(rect, 1),
                            //Font = new Font(documentStyle.FontFamily, documentStyle.FontSize, documentStyle.FontStyle),
                            TextDecoration = codeTextBox.TextDecoration
                        };
                        cache.Add(styleId, v);
                    }
                    else
                    {
                        CodeStyle documentStyle = codeTextBox.Styles[styleId];
                        v = new Cache
                        {
                            Background = documentStyle.Background?.CreateBrush(rect, 1),
                            Foreground = documentStyle.Foreground?.CreateBrush(rect, 1),
                            //Font = new Font(documentStyle.FontFamily, documentStyle.FontSize, documentStyle.FontStyle),
                            TextDecoration = documentStyle.TextDecoration
                        };
                        cache.Add(styleId, v);
                    }
                }
                return v;
            }


            //public Font Font;
            public Brush Foreground;
            public Brush Background;
            public TextDecoration TextDecoration;

            public void Dispose()
            {
                //if (Font != null)
                //{
                //Font.Dispose();
                //Font = null;
                //}
                if (Foreground != null)
                {
                    Foreground.Dispose();
                    Foreground = null;
                }
                if (Background != null)
                {
                    Background.Dispose();
                    Background = null;
                }
            }
        }
    }

}
