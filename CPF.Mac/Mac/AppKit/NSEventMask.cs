using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSEventMask : ulong
	{
		LeftMouseDown = 0x2,
		LeftMouseUp = 0x4,
		RightMouseDown = 0x8,
		RightMouseUp = 0x10,
		MouseMoved = 0x20,
		LeftMouseDragged = 0x40,
		RightMouseDragged = 0x80,
		MouseEntered = 0x100,
		MouseExited = 0x200,
		KeyDown = 0x400,
		KeyUp = 0x800,
		FlagsChanged = 0x1000,
		AppKitDefined = 0x2000,
		SystemDefined = 0x4000,
		ApplicationDefined = 0x8000,
		Periodic = 0x10000,
		CursorUpdate = 0x20000,
		ScrollWheel = 0x400000,
		TabletPoint = 0x800000,
		TabletProximity = 0x1000000,
		OtherMouseDown = 0x2000000,
		OtherMouseUp = 0x4000000,
		OtherMouseDragged = 0x8000000,
		EventGesture = 0x20000000,
		EventMagnify = 0x40000000,
		EventSwipe = 0x80000000,
		EventRotate = 0x40000,
		EventBeginGesture = 0x80000,
		EventEndGesture = 0x100000,
		AnyEvent = 0xFFFFFFFF
	}
}
