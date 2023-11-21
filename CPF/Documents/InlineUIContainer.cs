using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Documents
{
    /// <summary>
    /// 将UIElement作为文档里的元素布局，元素控件不能使用百分比尺寸
    /// </summary>
    public class InlineUIContainer : IDocumentElement, ICanSelectElement, IFlowElement
    {
        /// <summary>
        /// 将UIElement作为文档里的元素布局，元素控件不能使用百分比尺寸
        /// </summary>
        public InlineUIContainer()
        {

        }
        public float Left
        {
            get;
            set;
        }

        public float Top
        {
            get;
            set;
        }
        /// <summary>
        /// 获取获取相对于文档的位置
        /// </summary>
        /// <returns></returns>
        public Point ActualPositon
        {
            get
            {
                var p = new Point(Left, Top);
                if (Line != null)
                {
                    p.X += Line.X;
                    p.Y += Line.Y;
                }
                var parent = Parent;
                while (parent != null)
                {
                    p = new Point(parent.Left + p.X, parent.Top + p.Y);
                    var inBlock = parent as InlineBlock;
                    if (inBlock != null)
                    {
                        p.X += inBlock.Line.X;
                        p.Y += inBlock.Line.Y;
                    }
                    else if (!(parent is Document) && parent is Block block)
                    {
                        p.X += block.Line.X;
                        p.Y += block.Line.Y;
                    }
                    parent = parent.Parent as Block;
                }
                return p;
            }
        }

        internal TextLine Line;

        public bool IsMeasureValid
        {
            get;
            set;
        }

        public Block Parent { get; set; }

        public bool CanSelect { get; set; }

        Size size;
        public void Arrange(in Font font, in Size availableSize)
        {
            size = new Size();
            if (UIElement != null)
            {
                var e = UIElement;
                //if (!e.IsArrangeValid)
                //{
                //var s = e.ActualSize;
                //var width = availableSize.Width;
                //s.Width += e.MarginLeft.GetActualValue(width);
                //s.Width += e.MarginRight.GetActualValue(width);
                //s.Height += e.MarginTop.GetActualValue(width);
                //s.Height += e.MarginBottom.GetActualValue(width);
                //size = s;
                e.Measure(availableSize);
                //}
                size = e.DesiredSize;
            }
        }

        /// <summary>
        /// 布局之后的尺寸，包含margin
        /// </summary>
        public float Width { get { return size.Width; } }
        /// <summary>
        /// 布局之后的尺寸，包含margin
        /// </summary>
        public float Height { get { return size.Height; } }

        UIElement elementTemplate;
        public UIElement UIElement
        {
            get { return elementTemplate; }
            set
            {
                if (elementTemplate != value)
                {
                    if (document != null)
                    {
                        var e = document.Owner as UIElement;
                        if (value != null)
                        {
                            value.DesiredSizeChanged += Value_DesiredSizeChanged;
                            e.Children.Add(value);
                        }
                        if (elementTemplate != null)
                        {
                            elementTemplate.DesiredSizeChanged -= Value_DesiredSizeChanged;
                            e.Children.Remove(elementTemplate);
                        }
                    }
                    elementTemplate = value;
                }

            }
        }

        private void Value_DesiredSizeChanged(object sender, EventArgs e)
        {
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

        public Document Document
        {
            get
            {
                return document;
            }
            internal set
            {
                if (document != value)
                {
                    Line = null;
                    if (elementTemplate != null)
                    {
                        if (value != null)
                        {
                            var e = value.Owner as UIElement;
                            e.Children.Add(elementTemplate);
                            value.UIContainers.Add(this);
                            elementTemplate.DesiredSizeChanged += Value_DesiredSizeChanged;
                        }
                        if (document != null)
                        {
                            var e = document.Owner as UIElement;
                            e.Children.Remove(elementTemplate);
                            document.UIContainers.Remove(this);
                            elementTemplate.DesiredSizeChanged -= Value_DesiredSizeChanged;
                            //elementTemplate.Dispose();
                        }
                    }
                    document = value;
                }
            }
        }

        public short StyleId
        {
            get;
            set;
        } = -1;

        public FlowDirection FlowDirection
        {
            get; set;
        }

        public object Tag { get; set; }

        public float Right => 0;

        public float Bottom => 0;

        Document document;
    }
}
