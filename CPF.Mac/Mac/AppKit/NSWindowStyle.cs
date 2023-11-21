using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSWindowStyle : ulong
	{
		Borderless = 0x0,
		Titled = 0x1,
		Closable = 0x2,
		Miniaturizable = 0x4,
		Resizable = 0x8,
		Utility = 0x10,
		DocModal = 0x40,
		NonactivatingPanel = 0x80,
		TexturedBackground = 0x100,
		Unscaled = 0x800,
		UnifiedTitleAndToolbar = 0x1000,
		Hud = 0x2000,
		FullScreenWindow = 0x4000
	}
}
