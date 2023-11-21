using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSGlyphStorageOptions : ulong
	{
		ShowControlGlyphs = 0x1,
		ShowInvisibleGlyphs = 0x2,
		WantsBidiLevels = 0x4
	}
}
