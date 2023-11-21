using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CPF.Platform;

namespace CPF
{
    /// <summary>
    /// 用Cursors.****来设置
    /// </summary>
    [Description("光标类型，格式：Arrow, Ibeam,Wait,Cross,UpArrow,SizeWestEast,SizeNorthSouth,SizeAll,No,Hand,AppStarting,Help,TopSide,BottomSide,LeftSide,RightSide,TopLeftCorner,TopRightCorner,BottomLeftCorner,BottomRightCorner,DragMove,DragCopy,DragLink, ")]
    public class Cursor : CPF.Design.ISerializerCode
    {
        //public static readonly Cursor Default = new Cursor(StandardCursorType.Arrow);

        public Cursor(object platformCursor, Cursors? cursors)
        {
            PlatformCursor = platformCursor;
            cursorType = cursors;
        }

        public Cursor(Cursors cursorType)
            : this(GetCursor(cursorType), cursorType)
        {
        }

        Cursors? cursorType;

        public object PlatformCursor { get; }
        /// <summary>
        /// 是否是标准的光标样式
        /// </summary>
        public Cursors? CursorType { get => cursorType; }

        public static Cursor Parse(string s)
        {
            return Enum.TryParse<Cursors>(s, true, out var t) ?
                new Cursor(t) :
                throw new ArgumentException($"Unrecognized cursor type '{s}'.");
        }

        public static implicit operator Cursor(string n)
        {
            return Parse(n);
        }
        public static implicit operator Cursor(Cursors n)
        {
            return new Cursor(n);
        }

        private static object GetCursor(Cursors type)
        {
            return Application.GetRuntimePlatform().GetCursor(type);
        }

        public override string ToString()
        {
            if (cursorType != null)
            {
                return cursorType.ToString();
            }
            return base.ToString();
        }

        public string GetCreationCode()
        {
            if (cursorType != null)
            {
                return "Cursors." + cursorType.ToString();
            }
            return "";
        }
    }
    /// <summary>
    /// 标准光标样式
    /// </summary>
    public enum Cursors : byte
    {
        Arrow,
        Ibeam,
        Wait,
        Cross,
        UpArrow,
        SizeWestEast,
        SizeNorthSouth,
        SizeAll,
        No,
        Hand,
        AppStarting,
        Help,
        TopSide,
        BottomSide,
        LeftSide,
        RightSide,
        TopLeftCorner,
        TopRightCorner,
        BottomLeftCorner,
        BottomRightCorner,
        DragMove,
        DragCopy,
        DragLink,

        // Not available in GTK directly, see http://www.pixelbeat.org/programming/x_cursors/ 
        // We might enable them later, preferably, by loading pixmax direclty from theme with fallback image
        // SizeNorthWestSouthEast,
        // SizeNorthEastSouthWest,
    }
}
