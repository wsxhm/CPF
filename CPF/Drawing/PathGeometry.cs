using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CPF.Platform;

namespace CPF.Drawing
{
    /// <summary>
    /// 支持WPF里的字符串格式数据，隐式转换
    /// </summary>
    public class PathGeometry : IDisposable, ICloneable
    {
        public PathGeometry(IPathImpl path)
        {
            this.path = path;
        }

        public PathGeometry()
        { }
        public PathGeometry(in Font font, string text)
        {
            path = Application.GetDrawingFactory().CreatePath(font, text);
        }

        IPathImpl path;

        public IPathImpl PathIml
        {
            get
            {
                if (path == null)
                {
                    path = Application.GetDrawingFactory().CreatePath();
                }
                return path;
            }
        }

        /// <summary>
        /// 开始新的图形操作，设置起始点。和EndFigure匹配
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void BeginFigure(float x, float y)
        {
            PathIml.BeginFigure(x, y);
        }
        /// <summary>
        /// 添加一个圆角矩形路径
        /// </summary>
        /// <param name="CornerRadius">表示矩形的角的半径，格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft</param>
        /// <param name="rect">矩形的位置和尺寸</param>
        /// <param name="width">padding</param>
        public void RectRoundedTo(String CornerRadius,Rect rect,float width = 0)
        {
            RectRoundedTo(CornerRadius, rect, width);
        }
        /// <summary>
        /// 添加一个圆角矩形路径
        /// </summary>
        /// <param name="CornerRadius">表示矩形的角的半径，格式 一个数字或者四个数字 比如10或者 10,10,10,10  topLeft,topRight,bottomRight,bottomLeft</param>
        /// <param name="rect">矩形的位置和尺寸</param>
        /// <param name="width">padding</param>
        public void RectRoundedTo(Controls.CornerRadius CornerRadius, Rect rect, float width = 0)
        {
            Controls.CornerRadius cr = CornerRadius;
            var w2 = width;
            var r = rect;
            PathIml.BeginFigure(w2 + cr.TopLeft + r.Left, w2 + r.Top);
            PathIml.LineTo(r.Right - w2 - cr.TopRight, w2 + r.Top);
            PathIml.ArcTo(new Point(r.Right - w2, w2 + cr.TopRight + r.Top), new Size(cr.TopRight, cr.TopRight), 90, true, false);
            PathIml.LineTo(r.Right - w2, r.Bottom - cr.BottomRight - w2);
            PathIml.ArcTo(new Point(r.Right - w2 - cr.BottomRight, r.Bottom - w2), new Size(cr.BottomRight, cr.BottomRight), 90, true, false);
            PathIml.LineTo(w2 + cr.BottomLeft + r.Left, r.Bottom - w2);
            PathIml.ArcTo(new Point(w2 + r.Left, r.Bottom - w2 - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, true, false);
            PathIml.LineTo(w2 + r.Left, w2 + cr.TopLeft + r.Top);
            PathIml.ArcTo(new Point(w2 + cr.TopLeft + r.Left, w2 + r.Top), new Size(cr.TopLeft, cr.TopLeft), 90, true, false);
            PathIml.EndFigure(true);
        }
        /// <summary>
        /// 添加一条线
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LineTo(float x, float y)
        {
            PathIml.LineTo(x, y);
        }
        /// <summary>
        /// 添加一个圆弧
        /// </summary>
        /// <param name="point">弧线末尾的目标点</param>
        /// <param name="size">椭圆的半径</param>
        /// <param name="rotationAngle">角度</param>
        /// <param name="isClockwise">是否为顺时针</param>
        /// <param name="isLargeArc">是否是大弧线</param>
        public void ArcTo(Point point, Size size, float rotationAngle, bool isClockwise, bool isLargeArc)
        {
            PathIml.ArcTo(point, size, rotationAngle, isClockwise, isLargeArc);
        }
        /// <summary>
        /// 结束当前图形操作，是否闭合路径。和BeginFigure匹配
        /// </summary>
        /// <param name="closeFigure"></param>
        public void EndFigure(bool closeFigure)
        {
            PathIml.EndFigure(closeFigure);
        }
        /// <summary>
        /// 三次方程式贝塞尔曲线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public void CubicTo(Point p1, Point p2, Point p3)
        {
            PathIml.CubicTo(p1, p2, p3);
        }
        /// <summary>
        /// 二次方程式贝塞尔曲线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void QuadTo(Point p1, Point p2)
        {
            PathIml.QuadTo(p1, p2);
        }

        public void Transform(Matrix matrix)
        {
            PathIml.Transform(matrix);
        }

        public Rect GetBounds()
        {
            return PathIml.GetBounds();
        }

        string dataString;

        public static implicit operator PathGeometry(string pathString)
        {
            try
            {
                var path = new PathGeometry();
                //new PathData(path, pathString);
                FillRule fillRule;
                if (pathString != null)
                {
                    int curIndex = 0;

                    // skip any leading space
                    while ((curIndex < pathString.Length) && Char.IsWhiteSpace(pathString, curIndex))
                    {
                        curIndex++;
                    }

                    // Is there anything to look at?
                    if (curIndex < pathString.Length)
                    {
                        // If so, we only care if the first non-WhiteSpace char encountered is 'F'
                        if (pathString[curIndex] == 'F')
                        {
                            curIndex++;

                            // Since we found 'F' the next non-WhiteSpace char must be 0 or 1 - look for it.
                            while ((curIndex < pathString.Length) && Char.IsWhiteSpace(pathString, curIndex))
                            {
                                curIndex++;
                            }

                            // If we ran out of text, this is an error, because 'F' cannot be specified without 0 or 1
                            // Also, if the next token isn't 0 or 1, this too is illegal
                            if ((curIndex == pathString.Length) ||
                                ((pathString[curIndex] != '0') &&
                                 (pathString[curIndex] != '1')))
                            {
                                throw new FormatException("path格式不对");
                            }

                            //#if PRESENTATION_CORE
                            fillRule = pathString[curIndex] == '0' ? FillRule.EvenOdd : FillRule.NonZero;
                            //#else
                            //                            fillRule = pathString[curIndex] != '0';

                            //#endif

                            // Increment curIndex to point to the next char
                            curIndex++;
                        }
                    }

                    AbbreviatedGeometryParser parser = new AbbreviatedGeometryParser();

                    parser.ParseToGeometryContext(path, pathString, curIndex);
                    path.dataString = pathString;
                }

                return path;
            }
            catch (Exception e)
            {
                throw new Exception("Path数据字符串格式不对:" + pathString, e);
            }
        }

        public FillRule FillRule
        {
            get { return PathIml.FillRule; }
            set { PathIml.FillRule = value; }
        }
        object ICloneable.Clone()
        {
            return new PathGeometry(PathIml.Clone() as IPathImpl);
        }

        public PathGeometry Clone()
        {
            return new PathGeometry(PathIml.Clone() as IPathImpl);
        }

        public void AddPath(PathGeometry path, bool connect)
        {
            PathIml.AddPath(path, connect);
        }

        public bool Contains(float x, float y)
        {
            return PathIml.Contains(x, y);
        }
        /// <summary>
        /// 创建一个用于描边当前路径的路径
        /// </summary>
        /// <returns></returns>
        public PathGeometry CreateStrokePath(float strokeWidth = 1)
        {
            return new PathGeometry(PathIml.CreateStrokePath(strokeWidth));
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(dataString))
            {
                return dataString;
            }
            return base.ToString();
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (path != null)
            {
                path.Dispose();
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~PathGeometry()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion

    }

    public enum FillRule : byte
    {
        NonZero,
        EvenOdd
    }

    /// <summary>
    /// Parser for XAML abbreviated geometry.
    /// SVG path spec is closely followed http://www.w3.org/TR/SVG11/paths.html
    /// 3/23/2006, new parser for performance (fyuan)
    /// </summary>
    sealed internal class AbbreviatedGeometryParser
    {
        const bool AllowSign = true;
        const bool AllowComma = true;
        const bool IsFilled = true;
        const bool IsClosed = true;
        const bool IsStroked = true;
        const bool IsSmoothJoin = true;

        IFormatProvider _formatProvider;

        string _pathString;        // Input string to be parsed
        int _pathLength;
        int _curIndex;          // Location to read next character from 
        bool _figureStarted;     // StartFigure is effective

        Point _lastStart;         // Last figure starting point
        Point _lastPoint;         // Last point
        Point _secondLastPoint;   // The point before last point

        char _token;             // Non whitespace character returned by ReadToken

        PathGeometry _context;

        /// <summary>
        /// Throw unexpected token exception
        /// </summary>
        private void ThrowBadToken()
        {
            throw new System.FormatException($"path格式不对{_pathString},{_curIndex - 1}");
        }

        bool More()
        {
            return _curIndex < _pathLength;
        }

        // Skip white space, one comma if allowed
        private bool SkipWhiteSpace(bool allowComma)
        {
            bool commaMet = false;

            while (More())
            {
                char ch = _pathString[_curIndex];

                switch (ch)
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t': // SVG whitespace
                        break;

                    case ',':
                        if (allowComma)
                        {
                            commaMet = true;
                            allowComma = false; // one comma only
                        }
                        else
                        {
                            ThrowBadToken();
                        }
                        break;

                    default:
                        // Avoid calling IsWhiteSpace for ch in (' ' .. 'z']
                        if (((ch > ' ') && (ch <= 'z')) || !Char.IsWhiteSpace(ch))
                        {
                            return commaMet;
                        }
                        break;
                }

                _curIndex++;
            }

            return commaMet;
        }

        /// <summary>
        /// Read the next non whitespace character
        /// </summary>
        /// <returns>True if not end of string</returns>
        private bool ReadToken()
        {
            SkipWhiteSpace(!AllowComma);

            // Check for end of string
            if (More())
            {
                _token = _pathString[_curIndex++];

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsNumber(bool allowComma)
        {
            bool commaMet = SkipWhiteSpace(allowComma);

            if (More())
            {
                _token = _pathString[_curIndex];

                // Valid start of a number
                if ((_token == '.') || (_token == '-') || (_token == '+') || ((_token >= '0') && (_token <= '9'))
                    || (_token == 'I')  // Infinity
                    || (_token == 'N')) // NaN
                {
                    return true;
                }
            }

            if (commaMet) // Only allowed between numbers
            {
                ThrowBadToken();
            }

            return false;
        }

        void SkipDigits(bool signAllowed)
        {
            // Allow for a sign
            if (signAllowed && More() && ((_pathString[_curIndex] == '-') || _pathString[_curIndex] == '+'))
            {
                _curIndex++;
            }

            while (More() && (_pathString[_curIndex] >= '0') && (_pathString[_curIndex] <= '9'))
            {
                _curIndex++;
            }
        }

        //       
        //         /// <summary>
        //         /// See if the current token matches the string s. If so, advance and
        //         /// return true. Else, return false.
        //         /// </summary>
        //         bool TryAdvance(string s)
        //         {
        //             Debug.Assert(s.Length != 0);
        // 
        //             bool match = false;
        //             if (More() && _pathString[_currentIndex] == s[0])
        //             {
        //                 //
        //                 // Don't bother reading subsequent characters, as the CLR parser will
        //                 // do this for us later.
        //                 //
        //                 _currentIndex = Math.Min(_currentIndex + s.Length, _pathLength);
        // 
        //                 match = true;
        //             }
        // 
        //             return match;
        //         }
        // 

        /// <summary>
        /// Read a floating point number
        /// </summary>
        /// <returns></returns>
        double ReadNumber(bool allowComma)
        {
            if (!IsNumber(allowComma))
            {
                ThrowBadToken();
            }

            bool simple = true;
            int start = _curIndex;

            //
            // Allow for a sign
            // 
            // There are numbers that cannot be preceded with a sign, for instance, -NaN, but it's
            // fine to ignore that at this point, since the CLR parser will catch this later.
            //
            if (More() && ((_pathString[_curIndex] == '-') || _pathString[_curIndex] == '+'))
            {
                _curIndex++;
            }

            // Check for Infinity (or -Infinity).
            if (More() && (_pathString[_curIndex] == 'I'))
            {
                //
                // Don't bother reading the characters, as the CLR parser will
                // do this for us later.
                //
                _curIndex = Math.Min(_curIndex + 8, _pathLength); // "Infinity" has 8 characters
                simple = false;
            }
            // Check for NaN
            else if (More() && (_pathString[_curIndex] == 'N'))
            {
                //
                // Don't bother reading the characters, as the CLR parser will
                // do this for us later.
                //
                _curIndex = Math.Min(_curIndex + 3, _pathLength); // "NaN" has 3 characters
                simple = false;
            }
            else
            {
                SkipDigits(!AllowSign);

                // Optional period, followed by more digits
                if (More() && (_pathString[_curIndex] == '.'))
                {
                    simple = false;
                    _curIndex++;
                    SkipDigits(!AllowSign);
                }

                // Exponent
                if (More() && ((_pathString[_curIndex] == 'E') || (_pathString[_curIndex] == 'e')))
                {
                    simple = false;
                    _curIndex++;
                    SkipDigits(AllowSign);
                }
            }

            if (simple && (_curIndex <= (start + 8))) // 32-bit integer
            {
                int sign = 1;

                if (_pathString[start] == '+')
                {
                    start++;
                }
                else if (_pathString[start] == '-')
                {
                    start++;
                    sign = -1;
                }

                int value = 0;

                while (start < _curIndex)
                {
                    value = value * 10 + (_pathString[start] - '0');
                    start++;
                }

                return value * sign;
            }
            else
            {
                string subString = _pathString.Substring(start, _curIndex - start);

                try
                {
                    return System.Convert.ToDouble(subString, _formatProvider);
                }
                catch (FormatException except)
                {
                    throw new System.FormatException($"path格式不对{_pathString},{start}", except);
                }
            }
        }

        /// <summary>
        /// Read a bool: 1 or 0
        /// </summary>
        /// <returns></returns>
        bool ReadBool()
        {
            SkipWhiteSpace(AllowComma);

            if (More())
            {
                _token = _pathString[_curIndex++];

                if (_token == '0')
                {
                    return false;
                }
                else if (_token == '1')
                {
                    return true;
                }
            }

            ThrowBadToken();

            return false;
        }

        /// <summary>
        /// Read a relative point
        /// </summary>
        /// <returns></returns>
        private Point ReadPoint(char cmd, bool allowcomma)
        {
            double x = ReadNumber(allowcomma);
            double y = ReadNumber(AllowComma);

            if (cmd >= 'a') // 'A' < 'a'. lower case for relative
            {
                x += _lastPoint.X;
                y += _lastPoint.Y;
            }

            return new Point((float)x, (float)y);
        }

        /// <summary>
        /// Reflect _secondLastPoint over _lastPoint to get a new point for smooth curve
        /// </summary>
        /// <returns></returns>
        private Point Reflect()
        {
            return new Point(2 * _lastPoint.X - _secondLastPoint.X,
                             2 * _lastPoint.Y - _secondLastPoint.Y);
        }

        private void EnsureFigure()
        {
            if (!_figureStarted)
            {
                //_context.BeginFigure(_lastStart, IsFilled, !IsClosed);
                _context.BeginFigure(_lastStart.X, _lastPoint.Y);
                _figureStarted = true;
            }
        }

        /// <summary>
        /// Parse a PathFigureCollection string
        /// </summary>
        internal void ParseToGeometryContext(
            PathGeometry context,
            string pathString,
            int startIndex)
        {
            // [BreakingChange] Dev10 Bug #453199
            // We really should throw an ArgumentNullException here for context and pathString.

            // From original code
            // This is only used in call to Double.Parse
            _formatProvider = System.Globalization.CultureInfo.InvariantCulture;

            _context = context;
            _pathString = pathString;
            _pathLength = pathString.Length;
            _curIndex = startIndex;

            _secondLastPoint = new Point(0, 0);
            _lastPoint = new Point(0, 0);
            _lastStart = new Point(0, 0);

            _figureStarted = false;

            bool first = true;

            char last_cmd = ' ';

            while (ReadToken()) // Empty path is allowed in XAML
            {
                char cmd = _token;

                if (first)
                {
                    if ((cmd != 'M') && (cmd != 'm'))  // Path starts with M|m
                    {
                        ThrowBadToken();
                    }

                    first = false;
                }

                switch (cmd)
                {
                    case 'm':
                    case 'M':
                        // XAML allows multiple points after M/m
                        _lastPoint = ReadPoint(cmd, !AllowComma);

                        //context.BeginFigure(_lastPoint, IsFilled, !IsClosed);
                        context.BeginFigure(_lastPoint.X, _lastPoint.Y);
                        _figureStarted = true;
                        _lastStart = _lastPoint;
                        last_cmd = 'M';

                        while (IsNumber(AllowComma))
                        {
                            _lastPoint = ReadPoint(cmd, !AllowComma);

                            //context.LineTo(_lastPoint, IsStroked, !IsSmoothJoin);
                            context.LineTo(_lastPoint.X, _lastPoint.Y);
                            last_cmd = 'L';
                        }
                        break;

                    case 'l':
                    case 'L':
                    case 'h':
                    case 'H':
                    case 'v':
                    case 'V':
                        EnsureFigure();

                        do
                        {
                            switch (cmd)
                            {
                                case 'l': _lastPoint = ReadPoint(cmd, !AllowComma); break;
                                case 'L': _lastPoint = ReadPoint(cmd, !AllowComma); break;
                                case 'h': _lastPoint.X += (float)ReadNumber(!AllowComma); break;
                                case 'H': _lastPoint.X = (float)ReadNumber(!AllowComma); break;
                                case 'v': _lastPoint.Y += (float)ReadNumber(!AllowComma); break;
                                case 'V': _lastPoint.Y = (float)ReadNumber(!AllowComma); break;
                            }

                            //context.LineTo(_lastPoint, IsStroked, !IsSmoothJoin);
                            context.LineTo(_lastPoint.X, _lastPoint.Y);
                        }
                        while (IsNumber(AllowComma));

                        last_cmd = 'L';
                        break;

                    case 'c':
                    case 'C': // cubic Bezier
                    case 's':
                    case 'S': // smooth cublic Bezier
                        EnsureFigure();

                        do
                        {
                            Point p;

                            if ((cmd == 's') || (cmd == 'S'))
                            {
                                if (last_cmd == 'C')
                                {
                                    p = Reflect();
                                }
                                else
                                {
                                    p = _lastPoint;
                                }

                                _secondLastPoint = ReadPoint(cmd, !AllowComma);
                            }
                            else
                            {
                                p = ReadPoint(cmd, !AllowComma);

                                _secondLastPoint = ReadPoint(cmd, AllowComma);
                            }

                            _lastPoint = ReadPoint(cmd, AllowComma);

                            //context.BezierTo(p, _secondLastPoint, _lastPoint, IsStroked, !IsSmoothJoin);
                            context.CubicTo(p, _secondLastPoint, _lastPoint);

                            last_cmd = 'C';
                        }
                        while (IsNumber(AllowComma));

                        break;

                    case 'q':
                    case 'Q': // quadratic Bezier
                    case 't':
                    case 'T': // smooth quadratic Bezier
                        EnsureFigure();

                        do
                        {
                            if ((cmd == 't') || (cmd == 'T'))
                            {
                                if (last_cmd == 'Q')
                                {
                                    _secondLastPoint = Reflect();
                                }
                                else
                                {
                                    _secondLastPoint = _lastPoint;
                                }

                                _lastPoint = ReadPoint(cmd, !AllowComma);
                            }
                            else
                            {
                                _secondLastPoint = ReadPoint(cmd, !AllowComma);
                                _lastPoint = ReadPoint(cmd, AllowComma);
                            }

                            //context.QuadraticBezierTo(_secondLastPoint, _lastPoint, IsStroked, !IsSmoothJoin);
                            context.QuadTo(_secondLastPoint, _lastPoint);

                            last_cmd = 'Q';
                        }
                        while (IsNumber(AllowComma));

                        break;

                    case 'a':
                    case 'A':
                        EnsureFigure();

                        do
                        {
                            // A 3,4 5, 0, 0, 6,7
                            double w = ReadNumber(!AllowComma);
                            double h = ReadNumber(AllowComma);
                            double rotation = ReadNumber(AllowComma);
                            bool large = ReadBool();
                            bool sweep = ReadBool();

                            _lastPoint = ReadPoint(cmd, AllowComma);

                            context.ArcTo(
                                _lastPoint,
                                new Size((float)w, (float)h),
                                (float)rotation,
                            //#if PBTCOMPILER
                            sweep,
                                //#else
                                //                            sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                                //#endif
                                large
                                );
                        }
                        while (IsNumber(AllowComma));

                        last_cmd = 'A';
                        break;

                    case 'z':
                    case 'Z':
                        EnsureFigure();
                        context.EndFigure(IsClosed);

                        _figureStarted = false;
                        last_cmd = 'Z';

                        _lastPoint = _lastStart; // Set reference point to be first point of current figure
                        break;

                    default:
                        ThrowBadToken();
                        break;
                }
            }
        }
    }
}
