using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSWindowCollectionBehavior : ulong
	{
		Default = 0x0,
		CanJoinAllSpaces = 0x1,
		MoveToActiveSpace = 0x2,
		Managed = 0x4,
		Transient = 0x8,
		Stationary = 0x10,
		ParticipatesInCycle = 0x20,
		IgnoresCycle = 0x40,
		FullScreenPrimary = 0x80,
		FullScreenAuxiliary = 0x100,
		[ElCapitan]
		FullScreenAllowsTiling = 0x800,
		[ElCapitan]
		FullScreenDisallowsTiling = 0x1000
	}
}
