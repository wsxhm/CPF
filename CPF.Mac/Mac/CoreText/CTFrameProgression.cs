using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	[Flags]
	public enum CTFrameProgression : uint
	{
		TopToBottom = 0x0,
		RightToLeft = 0x1
	}
}
