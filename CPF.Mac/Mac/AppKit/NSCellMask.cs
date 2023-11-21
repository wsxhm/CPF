using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSCellMask : ulong
	{
		NoCell = 0x0,
		ContentsCell = 0x1,
		PushInCell = 0x2,
		ChangeGrayCell = 0x4,
		ChangeBackgroundCell = 0x8
	}
}
