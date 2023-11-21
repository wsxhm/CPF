using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSAlignmentOptions : ulong
	{
		MinXInward = 0x1,
		MinYInward = 0x2,
		MaxXInward = 0x4,
		MaxYInward = 0x8,
		WidthInward = 0x10,
		HeightInward = 0x20,
		MinXOutward = 0x100,
		MinYOutward = 0x200,
		MaxXOutward = 0x400,
		MaxYOutward = 0x800,
		WidthOutward = 0x1000,
		HeightOutward = 0x2000,
		MinXNearest = 0x10000,
		MinYNearest = 0x20000,
		MaxXNearest = 0x40000,
		MaxYNearest = 0x80000,
		WidthNearest = 0x100000,
		HeightNearest = 0x200000,
		RectFlipped = 18446744071562067968uL,
		AllEdgesInward = 0xF,
		AllEdgesOutward = 0xF00,
		AllEdgesNearest = 0xF0000
	}
}
