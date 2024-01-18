using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CPF.Drawing;

namespace CPF.Svg
{
    class PathShape : SvgShape
    {
        //public class CommandSplitter
        //{
        //	// http://www.w3.org/TR/SVGTiny12/paths.html
        //	// command is from one non numeric character to the next (-.,space   is part of the numeric value since it defines a point)
        //	string m_value;
        //	int m_curPos = -1;
        //	char[] m_commands = new char[] {'m', 'M', 'z', 'Z', 'A', 'a', 'L', 'l', 'h', 'H', 'v', 'V', 'c', 'C', 's', 'S' };
        //	public CommandSplitter(string value)
        //	{
        //		m_value = value;
        //	}
        //	public string ReadNext()
        //	{
        //		int startpos = m_curPos;
        //		if (startpos < 0)
        //			startpos = 0;
        //		if (startpos >= m_value.Length)
        //			return string.Empty;
        //		int cmdstart = m_value.IndexOfAny(m_commands, startpos);
        //		int cmdend = cmdstart;
        //		if (cmdstart >= 0)
        //			cmdend = m_value.IndexOfAny(m_commands, cmdstart+1);
        //		if (cmdend < 0)
        //		{
        //			int len = m_value.Length - startpos;
        //			m_curPos = m_value.Length;
        //			return m_value.Substring(startpos, len).Trim();
        //		}
        //		else
        //		{
        //			int len = cmdend - startpos;
        //			m_curPos = cmdend;
        //			return m_value.Substring(startpos, len).Trim();
        //		}
        //	}

        //	ShapeUtil.StringSplitter m_splitter = new ShapeUtil.StringSplitter(string.Empty);
        //	public ShapeUtil.StringSplitter SplitCommand(string command, out char cmd)
        //	{
        //		cmd = command[0];
        //		m_splitter.SetString(command, 1);
        //		return m_splitter;
        //	}
        //}
        //public class PathElement
        //{
        //	public char Command { get; protected set; }
        //	public bool IsRelative
        //	{
        //		get
        //		{
        //			return char.IsLower(Command);
        //		}
        //	}
        //	protected PathElement(char command)
        //	{
        //		Command = command;
        //	}
        //}
        //public class MoveTo : PathElement
        //{
        //	public Point Point { get; private set; }
        //	public MoveTo(char command, ShapeUtil.StringSplitter value) : base(command)
        //	{
        //		Point = value.ReadNextPoint();
        //	}
        //}
        //public class LineTo : PathElement
        //{
        //	public enum eType
        //	{
        //		Point,
        //		Horizontal,
        //		Vertical,
        //	}
        //	public eType PositionType { get; private set; }
        //	public Point[] Points { get; private set; }
        //	public LineTo(char command, ShapeUtil.StringSplitter value) : base(command)
        //	{
        //		if (char.ToLower(command) == 'h')
        //		{
        //			PositionType = eType.Horizontal;
        //			double v = value.ReadNextValue();
        //			Points = new Point[] { new Point((float)v, 0) };
        //			return;
        //		}
        //		if (char.ToLower(command) == 'v')
        //		{
        //			PositionType = eType.Vertical;
        //			double v = value.ReadNextValue();
        //			Points = new Point[] { new Point(0, (float)v) };
        //			return;
        //		}

        //		PositionType = eType.Point;
        //		List<Point> list = new List<Point>();
        //		while (value.More)
        //		{
        //			Point p = value.ReadNextPoint();
        //			list.Add(p);
        //		}
        //		Points = list.ToArray();
        //	}
        //}

        //public class CurveTo : PathElement
        //{
        //	public Point CtrlPoint1 { get; private set; }
        //	public Point CtrlPoint2 { get; private set; }
        //	public Point Point { get; private set; }
        //	public CurveTo(char command, ShapeUtil.StringSplitter value) : base(command)
        //	{
        //		CtrlPoint1 = value.ReadNextPoint();
        //		CtrlPoint2 = value.ReadNextPoint();
        //		Point = value.ReadNextPoint();
        //	}
        //	public CurveTo(char command, ShapeUtil.StringSplitter value, Point ctrlPoint1) : base(command)
        //	{
        //		CtrlPoint1 = ctrlPoint1;
        //		CtrlPoint2 = value.ReadNextPoint();
        //		Point = value.ReadNextPoint();
        //	}
        //}
        //public class EllipticalArcTo : PathElement
        //{
        //	public double RX { get; private set; }
        //	public double RY { get; private set; }
        //	public double AxisRotation { get; private set; }
        //	public double X { get; private set; }
        //	public double Y { get; private set; }
        //	public bool Clockwise  { get; private set; }
        //	public bool LargeArc { get; private set; }
        //	public EllipticalArcTo(char command, ShapeUtil.StringSplitter value) : base(command)
        //	{
        //		RX = value.ReadNextValue();
        //		RY = value.ReadNextValue();
        //		AxisRotation = value.ReadNextValue();
        //		double arcflag = value.ReadNextValue();
        //		LargeArc = (arcflag > 0);
        //		double sweepflag = value.ReadNextValue();
        //		Clockwise = (sweepflag > 0);
        //		X = value.ReadNextValue();
        //		Y = value.ReadNextValue();
        //	}
        //}
        //List<PathElement> m_elements = new List<PathElement>();

        //public IList<PathElement> Elements
        //{
        //	get 
        //	{ 
        //		return m_elements.AsReadOnly();
        //	}
        //}

        public string Data { get; set; }

        //public bool ClosePath { get; private set;}
        // http://apike.ca/prog_svg_paths.html
        public PathShape(XmlNode node, SvgShape parent) : base(node, parent)
        {
            //ClosePath = false;
            string path = XmlUtil.AttrValue(node, "d", string.Empty);
            Data = path;
            //CommandSplitter cmd = new CommandSplitter(path);
            //string commandstring;
            //char command;
            //List<PathElement> elements = m_elements;
            //while (true)
            //{
            //	commandstring = cmd.ReadNext();
            //	if (commandstring.Length == 0)
            //		break;
            //	ShapeUtil.StringSplitter split = cmd.SplitCommand(commandstring, out command);
            //	if (command == 'm' || command == 'M')
            //	{
            //		elements.Add(new MoveTo(command, split));
            //		if (split.More)
            //			elements.Add(new LineTo(command, split));
            //		continue;
            //	}
            //	if (command == 'l' || command == 'L' || command == 'H' || command == 'h' || command == 'V' || command == 'v')
            //	{
            //		elements.Add(new LineTo(command, split));
            //		continue;
            //	}
            //	if (command == 'c' || command == 'C')
            //	{
            //		while (split.More)
            //			elements.Add(new CurveTo(command, split));
            //		continue;
            //	}
            //	if (command == 's' || command == 'S')
            //	{
            //		while (split.More)
            //		{
            //			CurveTo lastshape = elements[elements.Count - 1] as CurveTo;
            //			System.Diagnostics.Debug.Assert(lastshape != null);
            //			elements.Add(new CurveTo(command, split, lastshape.CtrlPoint2));
            //		}
            //		continue;
            //	}
            //	if (command == 'a' || command == 'A')
            //	{
            //		elements.Add(new EllipticalArcTo(command, split));
            //		while (split.More)
            //			elements.Add(new EllipticalArcTo(command, split));
            //		continue;
            //	}
            //	if (command == 'z' || command == 'Z')
            //	{
            //		ClosePath = true;
            //		continue;
            //	}

            //	// extended format moveto or lineto can contain multiple points which should be translated into lineto
            //	PathElement lastitem = elements[elements.Count-1];
            //	if (lastitem is MoveTo || lastitem is LineTo || lastitem is CurveTo)
            //	{
            //		//Point p = Point.Parse(s);
            //		//elements.Add(new LineTo(p));
            //		continue;
            //	}


            //	System.Diagnostics.Debug.Assert(false, string.Format("type '{0}' not supported", commandstring));
            //}
        }

        public override PathGeometry CreateGeometry()
        {
            return Data;
        }
    }
}
