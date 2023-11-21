using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSTrackingAreaOptions : ulong
	{
		MouseEnteredAndExited = 0x1,
		MouseMoved = 0x2,
		CursorUpdate = 0x4,
		ActiveWhenFirstResponder = 0x10,
		ActiveInKeyWindow = 0x20,
		ActiveInActiveApp = 0x40,
		ActiveAlways = 0x80,
		AssumeInside = 0x100,
		InVisibleRect = 0x200,
		EnabledDuringMouseDrag = 0x400
	}
}
