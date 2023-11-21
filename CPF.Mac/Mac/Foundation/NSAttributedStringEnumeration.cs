using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSAttributedStringEnumeration : ulong
	{
		None = 0x0,
		Reverse = 0x2,
		LongestEffectiveRangeNotRequired = 0x100000
	}
}
