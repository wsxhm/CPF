using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public class LineControl : UIElement
    {
        public LineControl()
        {
            /*
                textbox.FontFamily, textbox.FontSize, textbox.FontStyle
             */
        }
        public float FontSize
        {
            get { return (float)GetValue(); }
            set
            {
                SetValue(value);
            }
        }
        public string FontFamily
        {
            get { return (string)GetValue(); }
            set { SetValue(value); }
        }
        public FontStyles FontStyle
        {
            get { return (FontStyles)GetValue(); }
            set { SetValue(value); }
        }
        public List<LineStruct> XLine
        {
            get { return (List<LineStruct>)GetValue(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 用来绘制折叠标识。折叠功能没写
        /// </summary>
        public List<CodeTextBox.Keystruct> IDocumentElementList
        {
            get { return (List<CodeTextBox.Keystruct>)GetValue(); }
            set { SetValue(value); }
        }
        public Font Font
        {
            get
            {
                return new Font(FontFamily, FontSize, FontStyle);
            }
        }

        [PropertyMetadata(typeof(ViewFill), "255,255,255")]
        public ViewFill LineBackground
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(typeof(ViewFill), "43,145,175")]
        public ViewFill LineNumFill
        {
            get { return GetValue<ViewFill>(); }
            set { SetValue(value); }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Size size = ActualSize;
            if (size.Width > 0 && size.Height > 0)
            {
                //填充背景
                var back = LineBackground;
                var rect = new Rect(size);
                if (back != null)
                {
                    using (var brush = back.CreateBrush(rect, Root.RenderScaling))
                    {
                        dc.FillRectangle(brush, rect);
                    }
                }

                var xoffset = size.Width - 5;
                //绘制线条
                dc.DrawLine(new Stroke(1), "#aaa", new Point(xoffset, 0), new Point(xoffset, size.Height));

                //绘制行号
                var fill = LineNumFill;
                if (fill != null)
                {
                    var antialias = dc.AntialiasMode;
                    dc.AntialiasMode = AntialiasMode.AntiAlias;
                    var font = Font;
                    var line = XLine;
                    using (var brush = fill.CreateBrush(rect, Root.RenderScaling))
                    {
                        for (int i = 0; i < line.Count; i++)
                        {
                            dc.DrawString(new Point(5, line[i].YOffet), brush, $"{i + 1}", font);
                            //if (line[i].IsFold)//还不支持折叠，暂时注释掉折叠
                            //{
                            //    if (i + 1 <= line.Count-1)
                            //    {
                            //        var yheight = (line[i + 1].YOffet - line[i].YOffet) / 2;
                            //        var y = line[i].YOffet + yheight;
                            //        var recty = (line[i + 1].YOffet - line[i].YOffet) / 3;
                            //        dc.DrawLine("2", "#aaa", new Point(xoffset - 5, y), new Point(xoffset + 5, y));
                            //        dc.DrawRectangle("#aaa","1",new Rect(new Point(xoffset - 5, recty + line[i].YOffet-0.7f), new Size(10, 10)));
                            //    }
                            //}
                        }
                    }
                    dc.AntialiasMode = antialias;
                }
            }

        }
    }
}
