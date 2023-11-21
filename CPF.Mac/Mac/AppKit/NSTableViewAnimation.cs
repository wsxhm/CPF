using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSTableViewAnimation : ulong
	{
		None = 0x0,
		Fade = 0x1,
		Gap = 0x2,
		SlideUp = 0x10,
		SlideDown = 0x20,
		SlideLeft = 0x30,
		SlideRight = 0x40
	}
}
