using System;

namespace CPF.Mac.CoreGraphics
{
	[Flags]
	public enum CGWindowImageOption
	{
		Default = 0x0,
		BoundsIgnoreFraming = 0x1,
		ShouldBeOpaque = 0x2,
		OnlyShadows = 0x4
	}
}
