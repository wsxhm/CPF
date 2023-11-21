using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSRemoteNotificationType : ulong
	{
		None = 0x0,
		Badge = 0x1,
		Sound = 0x2,
		Alert = 0x4
	}
}
