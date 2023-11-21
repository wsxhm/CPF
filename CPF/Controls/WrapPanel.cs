using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Drawing;

namespace CPF.Controls
{
    /// <summary>
    /// 按从左到右的顺序位置定位子元素，在包含框的边缘处将内容切换到下一行。 后续排序按照从上至下或从右至左的顺序进行，具体取决于 Orientation 属性的值。
    /// </summary>
    [Description("按从左到右的顺序位置定位子元素，在包含框的边缘处将内容切换到下一行。 后续排序按照从上至下或从右至左的顺序进行，具体取决于 Orientation 属性的值。")]
    public class WrapPanel : Panel
    {
        #region Public Properties
        /// <summary>
        /// 默认值float.NaN
        /// </summary>
        [UIPropertyMetadata(float.NaN, UIPropertyOptions.AffectsArrange)]
        public float ItemWidth
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 默认值float.NaN
        /// </summary>
        [UIPropertyMetadata(float.NaN, UIPropertyOptions.AffectsArrange)]
        public float ItemHeight
        {
            get { return (float)GetValue(); }
            set { SetValue(value); }
        }

        [UIPropertyMetadata(Orientation.Horizontal, UIPropertyOptions.AffectsArrange)]
        public Orientation Orientation
        {
            get { return GetValue<Orientation>(); }
            set { SetValue(value); }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        private struct UVSize
        {
            internal UVSize(Orientation orientation, float width, float height)
            {
                U = V = 0f;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0f;
                _orientation = orientation;
            }

            internal float U;
            internal float V;
            private Orientation _orientation;

            internal float Width
            {
                get { return (_orientation == Orientation.Horizontal ? U : V); }
                set { if (_orientation == Orientation.Horizontal) U = value; else V = value; }
            }
            internal float Height
            {
                get { return (_orientation == Orientation.Horizontal ? V : U); }
                set { if (_orientation == Orientation.Horizontal) V = value; else U = value; }
            }
        }


        /// <summary>
        /// <see cref="UIElement.MeasureOverride"/>
        /// </summary>
        protected override Size MeasureOverride(in Size constraint)
        {
            var or = Orientation;
            UVSize curLineSize = new UVSize(or);
            UVSize panelSize = new UVSize(or);
            UVSize uvConstraint = new UVSize(or, constraint.Width, constraint.Height);
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            bool itemWidthSet = !FloatUtil.IsNaN(itemWidth);
            bool itemHeightSet = !FloatUtil.IsNaN(itemHeight);

            Size childConstraint = new Size(
                (itemWidthSet ? itemWidth : constraint.Width),
                (itemHeightSet ? itemHeight : constraint.Height));

            UIElementCollection children = Children;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                //Flow passes its own constrint to children
                child.Measure(childConstraint);

                //this is the size of the child in UV space
                UVSize sz = new UVSize(
                    or,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (FloatUtil.GreaterThan(curLineSize.U + sz.U, uvConstraint.U)) //need to switch to another line
                {
                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = sz;

                    if (FloatUtil.GreaterThan(sz.U, uvConstraint.U)) //the element is wider then the constrint - give it a separate line                    
                    {
                        panelSize.U = Math.Max(sz.U, panelSize.U);
                        panelSize.V += sz.V;
                        curLineSize = new UVSize(or);
                    }
                }
                else //continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            //the last line size, if any should be added
            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            //go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        /// <summary>
        /// <see cref="UIElement.ArrangeOverride"/>
        /// </summary>
        protected override Size ArrangeOverride(in Size finalSize)
        {
            var or = Orientation;
            int firstInLine = 0;
            float itemWidth = ItemWidth;
            float itemHeight = ItemHeight;
            float accumulatedV = 0;
            float itemU = (or == Orientation.Horizontal ? itemWidth : itemHeight);
            UVSize curLineSize = new UVSize(or);
            UVSize uvFinalSize = new UVSize(or, finalSize.Width, finalSize.Height);
            bool itemWidthSet = !FloatUtil.IsNaN(itemWidth);
            bool itemHeightSet = !FloatUtil.IsNaN(itemHeight);
            bool useItemU = (or == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            UIElementCollection children = Children;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                UVSize sz = new UVSize(
                    or,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (FloatUtil.GreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) //need to switch to another line
                {
                    arrangeLine(accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU, or);

                    accumulatedV += curLineSize.V;
                    curLineSize = sz;

                    if (FloatUtil.GreaterThan(sz.U, uvFinalSize.U)) //the element is wider then the constraint - give it a separate line                    
                    {
                        //switch to next line which only contain one element
                        arrangeLine(accumulatedV, sz.V, i, ++i, useItemU, itemU, or);

                        accumulatedV += sz.V;
                        curLineSize = new UVSize(or);


                        if (i < children.Count)
                        {
                            child = children[i];
                            sz = new UVSize(
                    or,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));
                            curLineSize.U = sz.U;
                            curLineSize.V = sz.V;
                        }
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            //arrange the last line, if any
            if (firstInLine < children.Count)
            {
                Size sizeWH = arrangeLine(accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU, or);
                if (sizeWH.Width > finalSize.Width || sizeWH.Height > finalSize.Height)
                {
                    float maxW = sizeWH.Width;
                    float maxH = sizeWH.Height;
                    maxW = maxW > finalSize.Width ? maxW : finalSize.Width;
                    maxH = maxH > finalSize.Height ? maxH : finalSize.Height;
                    return new Size(maxW, maxH);
                }
            }

            return finalSize;
        }

        private Size arrangeLine(float v, float lineV, int start, int end, bool useItemU, float itemU, Orientation or)
        {
            float u = 0;
            float maxW = 0;
            float maxH = 0;
            bool isHorizontal = (or == Orientation.Horizontal);
            UIElementCollection children = Children;
            for (int i = start; i < end; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child != null)
                {
                    UVSize childSize = new UVSize(or, child.DesiredSize.Width, child.DesiredSize.Height);
                    float layoutSlotU = (useItemU ? itemU : childSize.U);
                    Rect rect = new Rect(
                        (isHorizontal ? u : v),
                        (isHorizontal ? v : u),
                        (isHorizontal ? layoutSlotU : lineV),
                        (isHorizontal ? lineV : layoutSlotU));
                    child.Arrange(rect);
                    u += layoutSlotU;
                    maxW = maxW > (rect.X + rect.Width) ? maxW : (rect.X + rect.Width);
                    maxH = maxH > (rect.Y + rect.Height) ? maxH : (rect.Y + rect.Height);
                }
            }
            return new Size(maxW, maxH);
        }

        #endregion Protected Methods
    }
}
