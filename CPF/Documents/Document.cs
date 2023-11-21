using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CPF.Drawing;
using System.Globalization;

namespace CPF.Documents
{
    public class Document : Block
    {
        IDocumentStyle owner;
        Collection<DocumentStyle> styles;
        public Document(IDocumentStyle owner)
        {
            this.owner = owner;
            Document = this;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override short StyleId
        {
            get
            {
                return base.StyleId;
            }

            set
            {
                base.StyleId = value;
            }
        }
        [PropertyMetadata(typeof(FloatField), "Auto")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override FloatField Width
        {
            get
            {
                return base.Width;
            }

            set
            {
                base.Width = value;
            }
        }

        [NotCpfProperty]
        public IDocumentStyle Owner
        {
            get
            {
                return owner;
            }
        }
        /// <summary>
        /// 文档样式
        /// </summary>
        [NotCpfProperty]
        public Collection<DocumentStyle> Styles
        {
            get
            {
                if (styles == null)
                {
                    styles = new Collection<DocumentStyle>();
                    styles.CollectionChanged += Styles_CollectionChanged;
                }
                return styles;
            }
        }

        private void Styles_CollectionChanged(object sender, CollectionChangedEventArgs<DocumentStyle> e)
        {
            switch (e.Action)
            {
                case CollectionChangedAction.Add:
                    e.NewItem.Parent = owner;
                    break;
                case CollectionChangedAction.Remove:
                    e.OldItem.Parent = null;
                    break;
                case CollectionChangedAction.Replace:
                    e.NewItem.Parent = owner;
                    e.OldItem.Parent = null;
                    break;
            }
            IsMeasureValid = false;
            ((UIElement)owner).InvalidateMeasure();
        }


        internal List<InlineUIContainer> UIContainers = new List<InlineUIContainer>();

        //protected override void OnItemAdded(Paragraph e)
        //{
        //    e.Document = this;
        //    if (e.elements != null)
        //    {
        //        var ele = owner as UIElement;
        //        foreach (var item in e.elements)
        //        {
        //            item.Document = this;
        //        }
        //    }
        //    base.OnItemAdded(e);
        //}
        //protected override void OnItemRemoved(Paragraph e)
        //{
        //    base.OnItemRemoved(e);
        //    e.Document = null;
        //    if (e.elements != null)
        //    {
        //        var ele = owner as UIElement;
        //        foreach (var item in e.elements)
        //        {
        //            item.Document = null;
        //        }
        //    }
        //}

        //public Size Arrange(float width)
        //{
        //    var size = new Size();
        //    foreach (Paragraph item in this)
        //    {
        //        var s = item.Arrange(this, width);
        //        item.Postion = new Point(0, size.Height);
        //        size.Width = Math.Max(s.Width, size.Width);
        //        size.Height += s.Height;
        //    }
        //    return size;
        //}
        public new void InvalidateArrange()
        {
            ((UIElement)owner).InvalidateMeasure();
        }

        public void Render(DrawingContext dc, Rect source)
        {
            HybridDictionary<int, Cache> cache = new HybridDictionary<int, Cache>();
            var rect = new Rect(new Point(), new Size((this as IDocumentElement).Width, Height));
            var v = new Cache
            {
                //Background = owner.Background?.CreateBrush(rect, dc),
                Foreground = owner.Foreground?.CreateBrush(rect, 1),
                Font = new Font(owner.FontFamily, owner.FontSize, owner.FontStyle),
                TextDecoration = owner.TextDecoration,
            };

            Render(dc, source, new Point(), cache, this, v);

            v.Dispose();
            foreach (var item in cache)
            {
                item.Value.Dispose();
            }
        }

        internal void Union(ref Rect rect1, Rect rect2)
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

        void Render(DrawingContext dc, Rect source, Point location, HybridDictionary<int, Cache> cache, IDocumentContainer documentContainer, Cache parent)
        {
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
                        var cc = Cache.GetCache(item.StyleId, cache, source, dc, this, parent);
                        if (!(item is IDocumentContainer))
                        {
                            if (cc.Background != null)
                            {
                                if (backBrush != null && backBrush != cc.Background && backRect.Width > 0 && backRect.Height > 0)
                                {
                                    dc.FillRectangle(backBrush, backRect);
                                    backRect = new Rect();
                                }
                                backBrush = cc.Background;
                                if (item is DocumentChar c && c.Char == '\n')
                                {
                                    continue;
                                }
                                Union(ref backRect, new Rect(point, new Size(item.Width, item.Height)));
                            }
                        }
                    }

                    if (backBrush != null && backRect.Width > 0 && backRect.Height > 0)
                    {
                        dc.FillRectangle(backBrush, backRect);
                    }
                }

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
                        var cc = Cache.GetCache(item.StyleId, cache, source, dc, this, parent);
                        var con = item as IDocumentContainer;
                        if (con != null)
                        {
                            var rect = new Rect(point, new Size(item.Width, item.Height));
                            if (con.Background != null)
                            {
                                using (var brush = con.Background.CreateBrush(rect, 1))
                                {
                                    dc.FillRectangle(brush, rect);
                                }
                            }
                            rect.Intersect(source);
                            Render(dc, rect, point, cache, con, cc);
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
                            if (!string.IsNullOrEmpty(str) && cc.Foreground != null && str != "\n" && str != "\t")
                            {
                                dc.DrawString(point, cc.Foreground, str, cc.Font,TextAlignment.Left, float.MaxValue, cc.TextDecoration);
                            }
                        }
                    }

                }

            }
        }
    }

    class Cache
    {
        public static Cache GetCache(short styleId, HybridDictionary<int, Cache> cache, Rect rect, DrawingContext dc, Document document, Cache parent)
        {
            if (styleId < 0 || styleId >= document.Styles.Count)
            {
                return parent;
            }
            if (!cache.TryGetValue(styleId, out Cache v))
            {
                IDocumentStyle documentStyle = document.Styles[styleId];
                v = new Cache
                {
                    Background = documentStyle.Background?.CreateBrush(rect, 1),
                    Foreground = documentStyle.Foreground?.CreateBrush(rect, 1),
                    Font = new Font(documentStyle.FontFamily, documentStyle.FontSize, documentStyle.FontStyle),
                    TextDecoration = documentStyle.TextDecoration
                };
                cache.Add(styleId, v);
            }
            return v;
        }
        /// <summary>
        /// 只缓存字体
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="cache"></param>
        /// <param name="document"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Cache GetCache(short styleId, HybridDictionary<int, Cache> cache, Document document, Cache parent)
        {
            if (styleId < 0)
            {
                //styleId = -1;
                return parent;
            }
            if (!cache.TryGetValue(styleId, out Cache v))
            {
                IDocumentStyle documentStyle;
                if (styleId > -1 && styleId < document.Styles.Count)
                {
                    documentStyle = document.Styles[styleId];
                }
                else
                {
                    documentStyle = document.Owner;
                }
                v = new Cache
                {
                    //Background = documentStyle.Background?.CreateBrush(rect, dc),
                    //Foreground = documentStyle.Foreground?.CreateBrush(rect, dc),
                    Font = new Font(documentStyle.FontFamily, documentStyle.FontSize, documentStyle.FontStyle)
                };
                cache.Add(styleId, v);
            }
            return v;
        }

        public Font Font;
        public Brush Foreground;
        public Brush Background;
        public TextDecoration TextDecoration;

        public void Dispose()
        {
            //if (Font != null)
            //{
            Font.Dispose();
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
