using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSTextStorageEditedFlags : ulong
	{
		EditedAttributed = 0x1,
		EditedCharacters = 0x2
	}
}
