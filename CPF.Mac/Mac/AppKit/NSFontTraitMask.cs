using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSFontTraitMask : ulong
	{
		Italic = 0x1,
		Bold = 0x2,
		Unbold = 0x4,
		NonStandardCharacterSet = 0x8,
		Narrow = 0x10,
		Expanded = 0x20,
		Condensed = 0x40,
		SmallCaps = 0x80,
		Poster = 0x100,
		Compressed = 0x200,
		FixedPitch = 0x400,
		Unitalic = 0x1000000
	}
}
