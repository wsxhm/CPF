using System;

namespace CPF.Mac.CoreGraphics
{
	[Flags]
	public enum CGWindowListOption
	{
		All = 0x0,
		OnScreenOnly = 0x1,
		OnScreenAboveWindow = 0x2,
		OnScreenBelowWindow = 0x4,
		IncludingWindow = 0x8,
		ExcludeDesktopElements = 0x10
	}
}
