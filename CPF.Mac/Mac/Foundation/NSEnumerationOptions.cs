using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSEnumerationOptions : ulong
	{
		SortConcurrent = 0x1,
		Reverse = 0x2
	}
}
