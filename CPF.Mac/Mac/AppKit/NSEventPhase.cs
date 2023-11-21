using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSEventPhase : ulong
	{
		None = 0x0,
		Began = 0x1,
		Stationary = 0x2,
		Changed = 0x4,
		Ended = 0x8,
		Cancelled = 0x10,
		MayBegin = 0x20
	}
}
