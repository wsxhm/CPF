using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSDraggingItemEnumerationOptions : ulong
	{
		Concurrent = 0x1,
		ClearNonenumeratedImages = 0x10000
	}
}
