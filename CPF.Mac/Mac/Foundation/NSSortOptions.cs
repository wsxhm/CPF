using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSSortOptions : ulong
	{
		Concurrent = 0x1,
		Stable = 0x10
	}
}
