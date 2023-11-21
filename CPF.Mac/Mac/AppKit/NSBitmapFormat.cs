using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSBitmapFormat : ulong
	{
		AlphaFirst = 0x1,
		AlphaNonpremultiplied = 0x2,
		FloatingPointSamples = 0x4,
		LittleEndian16Bit = 0x100,
		LittleEndian32Bit = 0x200,
		BigEndian16Bit = 0x400,
		BigEndian32Bit = 0x800
	}
}
