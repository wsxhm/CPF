using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;

namespace CPF.Svg
{
	class TextShape : Shape
	{
		public double X { get; set; }
		public double Y { get; set; }
		public string Text { get; set; }
		public TSpan.Element TextSpan {get; private set;}

		public TextShape(XmlNode node, Shape parent) : base(node, parent)
		{
			X = XmlUtil.AttrValue(node, "x", 0);
			Y = XmlUtil.AttrValue(node, "y", 0);
			Text = node.InnerText;
			GetTextStyle();
			// check for tSpan tag
			if (node.InnerXml.IndexOf("<") >= 0)
				TextSpan = ParseTSpan(node.InnerXml);
		}
		TSpan.Element ParseTSpan(string tspanText)
		{
			try
			{
				return TSpan.Parse(tspanText, this);
			}
			catch
			{
				return null;
			}
		}
		public class TSpan
		{
			public class Element : Shape
			{
				public enum eElementType
				{
					Tag,
					Text,
				}
				public override System.Windows.Media.Transform Transform 
				{ 
					get {return Parent.Transform; }
				}
				public eElementType ElementType {get; private set;}
				public List<ShapeUtil.Attribute> Attributes {get; set;}
				public List<Element> Children {get; private set;}
				public int StartIndex {get; set;}
				public string Text {get; set;}
				public Element End {get; set;}
				public Element(Shape parent, string text) : base((XmlNode)null, parent)
				{
					ElementType = eElementType.Text;
					Text = text;
				}
				public Element(Shape parent, eElementType eType, List<ShapeUtil.Attribute> attrs) : base(attrs, parent)
				{
					ElementType = eType;
					Text = string.Empty;
					Children = new List<Element>();
				}
				public override string ToString()
				{
					return Text;
				}
			}

			static Element NextTag(Element parent, string text, ref int curPos)
			{
				int start = text.IndexOf("<", curPos);
				if (start < 0)
					return null;
				int end = text.IndexOf(">", start+1);
				if (end < 0)
					throw new Exception("Start '<' with no end '>'");

				end++;

				string tagtext = text.Substring(start, end - start);
				if (tagtext.IndexOf("<", 1) > 0)
					throw new Exception(string.Format("Start '<' within tag 'tag'"));

				List<ShapeUtil.Attribute> attrs = new List<ShapeUtil.Attribute>();
				int attrstart = tagtext.IndexOf("tspan");
				if (attrstart > 0)
				{
					attrstart += 5;
					while (attrstart < tagtext.Length-1)
						attrs.Add(ShapeUtil.ReadNextAttr(tagtext, ref attrstart));
				}
	
				Element tag = new Element(parent, Element.eElementType.Tag, attrs);
				tag.StartIndex = start;
				tag.Text = text.Substring(start, end - start);
				if (tag.Text.IndexOf("<", 1) > 0)
					throw new Exception(string.Format("Start '<' within tag 'tag'"));

				curPos = end;
				return tag;
			}
			static Element Parse(string text, ref int curPos, Element parent, Element curTag)
			{
				Element tag = curTag;
				if (tag == null)
					tag = NextTag(parent, text, ref curPos);
				while (curPos < text.Length)
				{
					int prevPos = curPos;
					Element next = NextTag(tag, text, ref curPos);
					if (next == null && curPos < text.Length)
					{
						// remaining pure text 
						string s = text.Substring(curPos, text.Length - curPos);
						tag.Children.Add(new Element(tag, s));
						return tag;
					}
					if (next != null && next.StartIndex-prevPos > 0)
					{
						// pure text between tspan elements
						int diff = next.StartIndex-prevPos;
						string s = text.Substring(prevPos, diff);
						tag.Children.Add(new Element(tag, s));
					}
					if (next.Text.StartsWith("<tspan"))
					{
						// new nested element
						next = Parse(text, ref curPos, tag, next);
						tag.Children.Add(next);
						continue;
					}
					if (next.Text.StartsWith("</tspan"))
					{
						// end of cur element
						tag.End = next;
						return tag;
					}
					if (next.Text.StartsWith("<textPath"))
					{
						continue;
					}
					if (next.Text.StartsWith("</textPath"))
					{
						continue;
					}
					throw new Exception(string.Format("unexpected tag '{0}'", next.Text));
				}
				return tag;
			}
			public static Element Parse(string text, TextShape owner)
			{
				int curpos = 0;
				Element root = new Element(owner, Element.eElementType.Tag, null);
				root.Text = "<root>";
				root.StartIndex = 0;
				return Parse(text, ref curpos, null, root);
			}
			public void Print(Element tag, string indent)
			{
				if (tag.ElementType == Element.eElementType.Text)
					Console.WriteLine("{0} '{1}'", indent, tag.Text);
				indent += "   ";
				foreach (Element c in tag.Children)
					Print(c, indent);
			}
		}
	}
}
