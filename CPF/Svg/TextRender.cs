﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;
using System.Windows;

namespace CPF.Svg
{
    class TextRender
    {
        static public GeometryGroup BuildTextGeometry(TextShape shape)
        {
            return BuildGlyphRun(shape, 0, 0);
        }

        // Use GlyphRun to build the text. This allows us to define letter and word spacing
        // http://books.google.com/books?id=558i6t1dKEAC&pg=PA485&source=gbs_toc_r&cad=4#v=onepage&q&f=false
        static GeometryGroup BuildGlyphRun(TextShape shape, double xoffset, double yoffset)
        {
            GeometryGroup gp = new GeometryGroup();
            double totalwidth = 0;
            if (shape.TextSpan == null)
            {
                string txt = shape.Text;
                gp.Children.Add(BuildGlyphRun(shape.TextStyle, txt, shape.X, shape.Y, ref totalwidth));
                return gp;
            }
            return BuildTextSpan(shape);
        }

        static GeometryGroup BuildTextSpan(TextShape shape)
        {
            double x = shape.X;
            double y = shape.Y;
            GeometryGroup gp = new GeometryGroup();
            BuildTextSpan(gp, shape.TextStyle, shape.TextSpan, ref x, ref y);
            return gp;
        }

        public static DependencyProperty TSpanElementProperty = DependencyProperty.RegisterAttached("TSpanElement", typeof(TextShape.TSpan.Element), typeof(DependencyObject));
        public static void SetElement(DependencyObject obj, TextShape.TSpan.Element value)
        {
            obj.SetValue(TSpanElementProperty, value);
        }
        public static TextShape.TSpan.Element GetElement(DependencyObject obj)
        {
            return (TextShape.TSpan.Element)obj.GetValue(TSpanElementProperty);
        }

        static void BuildTextSpan(GeometryGroup gp, TextStyle textStyle, TextShape.TSpan.Element tspan, ref double x, ref double y)
        {
            foreach (TextShape.TSpan.Element child in tspan.Children)
            {
                if (child.ElementType == TextShape.TSpan.Element.eElementType.Text)
                {
                    string txt = child.Text;
                    double totalwidth = 0;
                    double baseline = y;

                    if (child.TextStyle.BaseLineShift == "sub")
                        baseline += child.TextStyle.FontSize * 0.5; /* * cap height ? fontSize*/;
                    if (child.TextStyle.BaseLineShift == "super")
                        baseline -= tspan.TextStyle.FontSize + (child.TextStyle.FontSize * 0.25)/*font.CapsHeight * fontSize*/;

                    Geometry gm = BuildGlyphRun(child.TextStyle, txt, x, baseline, ref totalwidth);
                    TextRender.SetElement(gm, child);
                    gp.Children.Add(gm);
                    x += totalwidth;
                    continue;
                }
                if (child.ElementType == TextShape.TSpan.Element.eElementType.Tag)
                    BuildTextSpan(gp, textStyle, child, ref x, ref y);
            }

        }



        static Geometry BuildGlyphRun(TextStyle textStyle, string text, double x, double y, ref double totalwidth)
        {
            double fontSize = textStyle.FontSize;
            GlyphRun glyphs = null;
            Typeface font = new Typeface(new FontFamily(textStyle.FontFamily),
                textStyle.Fontstyle,
                textStyle.Fontweight,
                FontStretch.FromOpenTypeStretch(9),
                new FontFamily("MetricHPE Unicode MS"));
            GlyphTypeface glyphFace;
            double baseline = y;
            if (font.TryGetGlyphTypeface(out glyphFace))
            {
                glyphs = new GlyphRun();
                ((System.ComponentModel.ISupportInitialize)glyphs).BeginInit();
                glyphs.GlyphTypeface = glyphFace;
                glyphs.FontRenderingEmSize = fontSize;
                List<char> textChars = new List<char>();
                List<ushort> glyphIndices = new List<ushort>();
                List<double> advanceWidths = new List<double>();
                totalwidth = 0;
                char[] charsToSkip = new char[] { '\t', '\r', '\n' };
                for (int i = 0; i < text.Length; ++i)
                {
                    char textchar = text[i];
                    int codepoint = textchar;
                    //if (charsToSkip.Any<char>(item => item == codepoint))
                    //	continue;
                    ushort glyphIndex;
                    if (glyphFace.CharacterToGlyphMap.TryGetValue(codepoint, out glyphIndex) == false)
                        continue;
                    textChars.Add(textchar);
                    double glyphWidth = glyphFace.AdvanceWidths[glyphIndex];
                    glyphIndices.Add(glyphIndex);
                    advanceWidths.Add(glyphWidth * fontSize + textStyle.LetterSpacing);
                    if (char.IsWhiteSpace(textchar))
                        advanceWidths[advanceWidths.Count - 1] += textStyle.WordSpacing;
                    totalwidth += advanceWidths[advanceWidths.Count - 1];
                }
                glyphs.Characters = textChars.ToArray();
                glyphs.GlyphIndices = glyphIndices.ToArray();
                glyphs.AdvanceWidths = advanceWidths.ToArray();

                // calculate text alignment
                double alignmentoffset = 0;
                if (textStyle.TextAlignment == TextAlignment.Center)
                    alignmentoffset = totalwidth / 2;
                if (textStyle.TextAlignment == TextAlignment.Right)
                    alignmentoffset = totalwidth;

                baseline = y;
                glyphs.BaselineOrigin = new Point((float)(x - alignmentoffset), (float)baseline);
                ((System.ComponentModel.ISupportInitialize)glyphs).EndInit();
            }
            else
                return new GeometryGroup();

            // add text decoration to geometry
            GeometryGroup gp = new GeometryGroup();
            gp.Children.Add(glyphs.BuildGeometry());
            if (textStyle.TextDecoration.Brush != null)
            {
                double decorationPos = 0;
                double decorationThickness = 0;
                if (textStyle.TextDecoration.Location == TextDecorationLocation.Strikethrough)
                {
                    decorationPos = baseline - (font.StrikethroughPosition * fontSize);
                    decorationThickness = font.StrikethroughThickness * fontSize;
                }
                if (textStyle.TextDecoration.Location == TextDecorationLocation.Underline)
                {
                    decorationPos = baseline - (font.UnderlinePosition * fontSize);
                    decorationThickness = font.UnderlineThickness * fontSize;
                }
                if (textStyle.TextDecoration.Location == TextDecorationLocation.OverLine)
                {
                    decorationPos = baseline - fontSize;
                    decorationThickness = font.StrikethroughThickness * fontSize;
                }
                Rect bounds = new Rect(gp.Bounds.Left, (float)decorationPos, gp.Bounds.Width, (float)decorationThickness);
                gp.Children.Add(new RectangleGeometry(bounds));

            }
            return gp;
        }
    }
}
