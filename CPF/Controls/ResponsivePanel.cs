using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CPF.Controls
{
    /// <summary>
    /// 响应式布局容器，模仿bootstrap 
    /// XS(&lt;768px), SM(&lt;992px), MD(&lt;1200px), LG(1200px&gt;=)
    /// 整个页面有12列。(可使用MaxDivision、BreakPoints属性自定义)
    /// </summary>
    [Description("响应式布局容器，模仿bootstrap")]
    public class ResponsivePanel : Panel
    {
        /// <summary>
        /// 响应式布局容器，模仿bootstrap 
        /// XS(&lt;768px), SM(&lt;992px), MD(&lt;1200px), LG(1200px&gt;=)
        /// 整个页面有12列。(可使用MaxDivision、BreakPoints属性自定义)
        /// </summary>
        public ResponsivePanel()
        {

        }
        /// <summary>
        /// 定义列数
        /// </summary>
        [Description("定义列数")]
        [UIPropertyMetadata(12, UIPropertyOptions.AffectsMeasure)]
        public int MaxDivision
        {
            get { return (int)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 定义响应布局的尺寸
        /// </summary>
        [Description("定义响应布局的尺寸")]
        public BreakPoints BreakPoints
        {
            get { return (BreakPoints)GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 显示分割列线条
        /// </summary>
        [Description("显示分割列线条")]
        public bool ShowGridLines
        {
            get { return (bool)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 尺寸状态 XS, SM, MD, LG
        /// </summary>
        [Description("尺寸状态 XS, SM, MD, LG")]
        public ResponsiveState State
        {
            get { return (ResponsiveState)GetValue(); }
            private set { SetValue(value); }
        }

        public static Attached<int> XS
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> SM
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> MD
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> LG
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> XS_Offset
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }

        public static Attached<int> SM_Offset
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }

        public static Attached<int> MD_Offset
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }

        public static Attached<int> LG_Offset
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> XS_Push
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }

        public static Attached<int> SM_Push
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> MD_Push
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> LG_Push
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> XS_Pull
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> SM_Pull
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> MD_Pull
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }


        public static Attached<int> LG_Pull
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }
        [Browsable(false)]
        public static Attached<int> ActualColumn
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel));
            }
        }


        [Browsable(false)]
        public static Attached<int> ActualRow
        {
            get
            {
                return RegisterAttached(0, typeof(ResponsivePanel));
            }
        }

        public static Attached<RVisible> Visible
        {
            get
            {
                return RegisterAttached(RVisible.Visible, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
                {
                    if (obj is UIElement element && element.Parent != null)
                    {
                        element.Parent.InvalidateMeasure();
                    }
                });
            }
        }
        //public static Attached<RHidden> Hidden
        //{
        //    get
        //    {
        //        return RegisterAttached(RHidden.None, typeof(ResponsivePanel), (CpfObject obj, string propertyName, object defaultValue, object oldValue, ref object newValue) =>
        //        {
        //            if (obj is UIElement element && element.Parent != null)
        //            {
        //                element.Parent.InvalidateMeasure();
        //            }
        //        });
        //    }
        //}

        protected int GetSpan(in BreakPoints breakPoints, UIElement element, double width)
        {
            var span = 0;

            var getXS = new Func<UIElement, int>((o) => { var x = XS(o); return x != 0 ? x : this.MaxDivision; });
            var getSM = new Func<UIElement, int>((o) => { var x = SM(o); return x != 0 ? x : getXS(o); });
            var getMD = new Func<UIElement, int>((o) => { var x = MD(o); return x != 0 ? x : getSM(o); });
            var getLG = new Func<UIElement, int>((o) => { var x = LG(o); return x != 0 ? x : getMD(o); });

            if (width < breakPoints.XS_SM)
            {
                span = getXS(element);
            }
            else if (width < breakPoints.SM_MD)
            {
                span = getSM(element);
            }
            else if (width < breakPoints.MD_LG)
            {
                span = getMD(element);
            }
            else
            {
                span = getLG(element);
            }

            return Math.Min(Math.Max(0, span), this.MaxDivision); ;
        }

        protected int GetOffset(in BreakPoints breakPoints, UIElement element, double width)
        {
            var span = 0;

            var getXS = new Func<UIElement, int>((o) => { var x = XS_Offset(o); return x != 0 ? x : 0; });
            var getSM = new Func<UIElement, int>((o) => { var x = SM_Offset(o); return x != 0 ? x : getXS(o); });
            var getMD = new Func<UIElement, int>((o) => { var x = MD_Offset(o); return x != 0 ? x : getSM(o); });
            var getLG = new Func<UIElement, int>((o) => { var x = LG_Offset(o); return x != 0 ? x : getMD(o); });

            if (width < breakPoints.XS_SM)
            {
                span = getXS(element);
            }
            else if (width < breakPoints.SM_MD)
            {
                span = getSM(element);
            }
            else if (width < breakPoints.MD_LG)
            {
                span = getMD(element);
            }
            else
            {
                span = getLG(element);
            }

            return Math.Min(Math.Max(0, span), this.MaxDivision); ;
        }

        protected int GetPush(in BreakPoints breakPoints, UIElement element, double width)
        {
            var span = 0;

            var getXS = new Func<UIElement, int>((o) => { var x = XS_Push(o); return x != 0 ? x : 0; });
            var getSM = new Func<UIElement, int>((o) => { var x = SM_Push(o); return x != 0 ? x : getXS(o); });
            var getMD = new Func<UIElement, int>((o) => { var x = MD_Push(o); return x != 0 ? x : getSM(o); });
            var getLG = new Func<UIElement, int>((o) => { var x = LG_Push(o); return x != 0 ? x : getMD(o); });

            if (width < breakPoints.XS_SM)
            {
                span = getXS(element);
            }
            else if (width < breakPoints.SM_MD)
            {
                span = getSM(element);
            }
            else if (width < breakPoints.MD_LG)
            {
                span = getMD(element);
            }
            else
            {
                span = getLG(element);
            }

            return Math.Min(Math.Max(0, span), this.MaxDivision); ;
        }

        protected int GetPull(in BreakPoints breakPoints, UIElement element, double width)
        {
            var span = 0;

            var getXS = new Func<UIElement, int>((o) => { var x = XS_Pull(o); return x != 0 ? x : 0; });
            var getSM = new Func<UIElement, int>((o) => { var x = SM_Pull(o); return x != 0 ? x : getXS(o); });
            var getMD = new Func<UIElement, int>((o) => { var x = MD_Pull(o); return x != 0 ? x : getSM(o); });
            var getLG = new Func<UIElement, int>((o) => { var x = LG_Pull(o); return x != 0 ? x : getMD(o); });

            if (width < breakPoints.XS_SM)
            {
                span = getXS(element);
            }
            else if (width < breakPoints.SM_MD)
            {
                span = getSM(element);
            }
            else if (width < breakPoints.MD_LG)
            {
                span = getMD(element);
            }
            else
            {
                span = getLG(element);
            }

            return Math.Min(Math.Max(0, span), this.MaxDivision); ;
        }

        Visibility GetVisibility(in BreakPoints breakPoints, UIElement element, float width)
        {
            Visibility visibility = Visibility.Collapsed;
            var v = Visible(element);
            if (width < breakPoints.XS_SM && v.HasFlag(RVisible.XS))
            {
                visibility = Visibility.Visible;
            }
            else if (width >= breakPoints.XS_SM && width < breakPoints.SM_MD && v.HasFlag(RVisible.SM))
            {
                visibility = Visibility.Visible;
            }
            else if (width >= breakPoints.SM_MD && width < breakPoints.MD_LG && v.HasFlag(RVisible.MD))
            {
                visibility = Visibility.Visible;
            }
            else if (width >= breakPoints.MD_LG && v.HasFlag(RVisible.LG))
            {
                visibility = Visibility.Visible;
            }
            return visibility;
        }

        float availableWidth;
        protected override Size MeasureOverride(in Size availableSize)
        {
            var count = 0;
            var currentRow = 0;

            availableWidth = float.IsPositiveInfinity(availableSize.Width) ? float.PositiveInfinity : availableSize.Width / this.MaxDivision;

            var breakPoints = BreakPoints;

            foreach (UIElement child in this.Children)
            {
                if (child != null)
                {
                    var v = GetVisibility(breakPoints, child, availableSize.Width);
                    //child.Visibility = v;
                    if (v == Visibility.Collapsed) { continue; }

                    var span = this.GetSpan(breakPoints, child, availableSize.Width);
                    var offset = this.GetOffset(breakPoints, child, availableSize.Width);
                    var push = this.GetPush(breakPoints, child, availableSize.Width);
                    var pull = this.GetPull(breakPoints, child, availableSize.Width);

                    if (count + span + offset > this.MaxDivision)
                    {
                        currentRow++;
                        count = 0;
                    }

                    ActualColumn(child, count + offset + push - pull);
                    ActualRow(child, currentRow);

                    count += (span + offset);

                    var size = new Size(availableWidth * span, float.PositiveInfinity);
                    child.Measure(size);
                }
            }


            var group = this.Children.GroupBy(x => ActualRow(x));

            var totalSize = new Size();
            if (group.Count() != 0)
            {
                totalSize.Width = group.Max(rows => rows.Sum(o => o.DesiredSize.Width));
                totalSize.Height = group.Sum(rows => rows.Max(o => o.DesiredSize.Height));
            }

            return totalSize;
        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            var breakPoints = BreakPoints;
            if (finalSize.Width != availableWidth)
            {//对于测量时给的尺寸不一致时候需要重新测量一遍
                var count = 0;
                var currentRow = 0;
                foreach (UIElement child in this.Children)
                {
                    var v = GetVisibility(breakPoints, child, finalSize.Width);
                    child.Visibility = v;
                    if (v == Visibility.Collapsed) { continue; }

                    var span = this.GetSpan(breakPoints, child, finalSize.Width);
                    var offset = this.GetOffset(breakPoints, child, finalSize.Width);
                    var push = this.GetPush(breakPoints, child, finalSize.Width);
                    var pull = this.GetPull(breakPoints, child, finalSize.Width);

                    if (count + span + offset > this.MaxDivision)
                    {
                        currentRow++;
                        count = 0;
                    }

                    ActualColumn(child, count + offset + push - pull);
                    ActualRow(child, currentRow);

                    count += (span + offset);
                }
            }

            var columnWidth = finalSize.Width / this.MaxDivision;

            var group = this.Children.GroupBy(x => ActualRow(x));

            float temp = 0;
            foreach (var rows in group)
            {
                float max = 0;

                var columnHeight = rows.Max(o => o.DesiredSize.Height);
                foreach (var element in rows)
                {
                    var column = ActualColumn(element);
                    var row = ActualRow(element);
                    var columnSpan = this.GetSpan(breakPoints, element, finalSize.Width);

                    var rect = new Rect(columnWidth * column, temp, columnWidth * columnSpan, columnHeight);

                    element.Arrange(rect);

                    max = Math.Max(element.DesiredSize.Height, max);
                }

                temp += max;
            }

            if (finalSize.Width < breakPoints.XS_SM)
            {
                State = ResponsiveState.XS;
            }
            else if (finalSize.Width >= breakPoints.XS_SM && finalSize.Width < breakPoints.SM_MD)
            {
                State = ResponsiveState.SM;
            }
            else if (finalSize.Width >= breakPoints.SM_MD && finalSize.Width < breakPoints.MD_LG)
            {
                State = ResponsiveState.MD;
            }
            else if (finalSize.Width >= breakPoints.MD_LG)
            {
                State = ResponsiveState.LG;
            }
            return finalSize;
        }



        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.ShowGridLines)
            {
                using (var brush = new SolidColorBrush("Yellow"))
                {
                    using (var brush1 = new SolidColorBrush("Blue"))
                    {
                        var ds = new Stroke(1, DashStyles.Custom, 0, new float[] { 4, 4 }, CapStyles.Round);
                        var ds1 = new Stroke(1);
                        var size = ActualSize;
                        var gridNum = this.MaxDivision;
                        var unit = size.Width / gridNum;
                        for (var i = 0; i <= gridNum; i++)
                        {
                            var x = (int)(unit * i);
                            dc.DrawLine(ds1, brush, new Point(x, 0), new Point(x, size.Height));
                            dc.DrawLine(ds, brush1, new Point(x, 0), new Point(x, size.Height));
                        }
                    }
                }
            }
        }

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(BreakPoints), new UIPropertyMetadataAttribute(BreakPoints.Default, UIPropertyOptions.AffectsMeasure));
        }
    }

    /// <summary>
    /// 定义响应布局的尺寸
    /// </summary>
    public struct BreakPoints
    {
        public float XS_SM { get; set; }

        public float SM_MD { get; set; }

        public float MD_LG { get; set; }

        public static BreakPoints Default
        {
            get { return new BreakPoints { XS_SM = 768f, SM_MD = 992f, MD_LG = 1200 }; }
        }
        public override string ToString()
        {
            return $"XS_SM = {XS_SM}, SM_MD = {SM_MD}, MD_LG = {MD_LG}";
        }

        public static implicit operator BreakPoints(string str)
        {
            try
            {
                var t = str.Split(',');
                var bp = new BreakPoints();
                bp.XS_SM = float.Parse(t[0].Split('=')[1]);
                bp.SM_MD = float.Parse(t[1].Split('=')[1]);
                bp.MD_LG = float.Parse(t[2].Split('=')[1]);
                return bp;
            }
            catch (Exception e)
            {
                throw new Exception("BreakPoints字符串格式不对", e);
            }
        }
    }
    /// <summary>
    /// 响应布局元素的可见性
    /// </summary>
    public enum RVisible : byte
    {
        Hidden = 0,
        XS = 1,
        SM = 2,
        MD = 4,
        LG = 8,
        Hidden_XS = SM | MD | LG,
        Hidden_SM = XS | MD | LG,
        Hidden_MD = XS | SM | LG,
        Hidden_LG = XS | SM | MD,
        Visible = XS | SM | MD | LG,
    }

    public enum ResponsiveState
    {
        XS,
        SM,
        MD,
        LG,
    }

    //public enum RHidden : byte
    //{
    //    None = 0,
    //    XS = 1,
    //    SM = 2,
    //    MD = 4,
    //    LG = 8
    //}
}
