using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	[Flags]
	public enum CTWritingDirection : sbyte
	{
		Natural = -1,
		LeftToRight = 0x0,
		RightToLeft = 0x1,
		Embedding = 0x0,
		Override = 0x2
	}
}
