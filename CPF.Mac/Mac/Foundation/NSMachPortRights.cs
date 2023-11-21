using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSMachPortRights : ulong
	{
		None = 0x0,
		SendRight = 0x1,
		ReceiveRight = 0x2
	}
}
