using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSDataReadingOptions : ulong
	{
		Mapped = 0x1,
		Uncached = 0x2,
		[Since(5, 0)]
		Coordinated = 0x4,
		[Since(5, 0)]
		MappedAlways = 0x8
	}
}
