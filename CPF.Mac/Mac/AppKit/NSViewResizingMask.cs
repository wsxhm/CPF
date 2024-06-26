using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSViewResizingMask : ulong
	{
		NotSizable = 0x0,
		MinXMargin = 0x1,
		WidthSizable = 0x2,
		MaxXMargin = 0x4,
		MinYMargin = 0x8,
		HeightSizable = 0x10,
		MaxYMargin = 0x20
	}
}
