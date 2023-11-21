using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSByteCountFormatterUnits : ulong
	{
		UseDefault = 0x0,
		UseBytes = 0x1,
		UseKB = 0x2,
		UseMB = 0x4,
		UseGB = 0x8,
		UseTB = 0x10,
		UsePB = 0x20,
		UseEB = 0x40,
		UseZB = 0x80,
		UseYBOrHigher = 0xFF00,
		UseAll = 0xFFFF
	}
}
