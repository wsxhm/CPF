using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSFontCollectionVisibility : ulong
	{
		Process = 0x1,
		User = 0x2,
		Computer = 0x4
	}
}
