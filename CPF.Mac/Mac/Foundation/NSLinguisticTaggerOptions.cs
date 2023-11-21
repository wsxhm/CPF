using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSLinguisticTaggerOptions : ulong
	{
		OmitWords = 0x1,
		OmitPunctuation = 0x2,
		OmitWhitespace = 0x4,
		OmitOther = 0x8,
		JoinNames = 0x10
	}
}
