using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSEventSwipeTrackingOptions : ulong
	{
		LockDirection = 0x1,
		ClampGestureAmount = 0x2
	}
}
