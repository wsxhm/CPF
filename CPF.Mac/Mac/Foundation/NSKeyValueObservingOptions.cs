using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSKeyValueObservingOptions : ulong
	{
		New = 0x1,
		Old = 0x2,
		OldNew = 0x3,
		Initial = 0x4,
		Prior = 0x8
	}
}
