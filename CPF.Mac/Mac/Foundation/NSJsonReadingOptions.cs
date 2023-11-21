using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSJsonReadingOptions : ulong
	{
		MutableContainers = 0x1,
		MutableLeaves = 0x2,
		AllowFragments = 0x4
	}
}
