using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSWorkspaceLaunchOptions : ulong
	{
		Print = 0x2,
		InhibitingBackgroundOnly = 0x80,
		WithoutAddingToRecents = 0x100,
		WithoutActivation = 0x200,
		Async = 0x10000,
		AllowingClassicStartup = 0x20000,
		PreferringClassic = 0x40000,
		NewInstance = 0x80000,
		Hide = 0x100000,
		HideOthers = 0x200000,
		Default = 0x30000
	}
}
