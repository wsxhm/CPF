using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSNotificationCoalescing : ulong
	{
		NoCoalescing = 0x0,
		CoalescingOnName = 0x1,
		CoalescingOnSender = 0x2
	}
}
