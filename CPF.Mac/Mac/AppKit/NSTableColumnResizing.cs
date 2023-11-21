using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSTableColumnResizing : ulong
	{
		None = 0x0,
		Autoresizing = 0x1,
		UserResizingMask = 0x2
	}
}
