using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSPasteboardReadingOptions : ulong
	{
		AsData = 0x0,
		AsString = 0x1,
		AsPropertyList = 0x2,
		AsKeyedArchive = 0x4
	}
}
