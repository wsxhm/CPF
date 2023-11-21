using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSComparisonPredicateOptions : ulong
	{
		None = 0x0,
		CaseInsensitive = 0x1,
		DiacriticInsensitive = 0x2,
		Normalized = 0x4
	}
}
