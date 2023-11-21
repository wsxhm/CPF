using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSDataSearchOptions : ulong
	{
		SearchBackwards = 0x1,
		SearchAnchored = 0x2
	}
}
