using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSFileWrapperReadingOptions : ulong
	{
		Immediate = 0x1,
		WithoutMapping = 0x2
	}
}
