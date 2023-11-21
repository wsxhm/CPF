using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSGradientDrawingOptions : ulong
	{
		None = 0x0,
		BeforeStartingLocation = 0x1,
		AfterEndingLocation = 0x2
	}
}
