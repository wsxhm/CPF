using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSFileWrapperReadingOptions : ulong
	{
		Immediate = 0x1,
		WithoutMapping = 0x2
	}
}
