using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSTableViewGridStyle : ulong
	{
		None = 0x0,
		SolidVerticalLine = 0x1,
		SolidHorizontalLine = 0x2,
		DashedHorizontalGridLine = 0x8
	}
}
