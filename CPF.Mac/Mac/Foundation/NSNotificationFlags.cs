using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSNotificationFlags : ulong
	{
		DeliverImmediately = 0x1,
		PostToAllSessions = 0x2
	}
}
