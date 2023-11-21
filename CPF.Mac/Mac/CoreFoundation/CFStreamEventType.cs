using System;

namespace CPF.Mac.CoreFoundation
{
	[Flags]
	public enum CFStreamEventType
	{
		None = 0x0,
		OpenCompleted = 0x1,
		HasBytesAvailable = 0x2,
		CanAcceptBytes = 0x4,
		ErrorOccurred = 0x8,
		EndEncountered = 0x10
	}
}
