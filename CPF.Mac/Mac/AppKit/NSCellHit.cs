using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSCellHit : ulong
	{
		None = 0x0,
		ContentArea = 0x1,
		EditableTextArea = 0x2,
		TrackableArae = 0x4
	}
}
