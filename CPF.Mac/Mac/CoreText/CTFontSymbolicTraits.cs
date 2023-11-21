using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	[Flags]
	public enum CTFontSymbolicTraits : uint
	{
		None = 0x0,
		Italic = 0x1,
		Bold = 0x2,
		Expanded = 0x20,
		Condensed = 0x40,
		MonoSpace = 0x400,
		Vertical = 0x800,
		UIOptimized = 0x1000,
		ColorGlyphs = 0x2000,
		Composite = 0x4000,
		Mask = 0xF0000000
	}
}
