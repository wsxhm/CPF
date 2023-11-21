using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSWritingDirection : long
	{
		Natural = -1L,
		LeftToRight = 0x0,
		RightToLeft = 0x1,
		Embedding = 0x0,
		Override = 0x2
	}
}
