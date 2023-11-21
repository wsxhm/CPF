using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSMenuProperty : ulong
	{
		Title = 0x1,
		AttributedTitle = 0x2,
		KeyEquivalent = 0x4,
		Image = 0x8,
		Enabled = 0x10,
		AccessibilityDescription = 0x20
	}
}
