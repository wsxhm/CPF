using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSEventModifierMask : ulong
	{
		AlphaShiftKeyMask = 0x10000,
		ShiftKeyMask = 0x20000,
		ControlKeyMask = 0x40000,
		AlternateKeyMask = 0x80000,
		CommandKeyMask = 0x100000,
		NumericPadKeyMask = 0x200000,
		HelpKeyMask = 0x400000,
		FunctionKeyMask = 0x800000,
		DeviceIndependentModifierFlagsMask = 0xFFFF0000
	}
}
