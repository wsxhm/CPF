using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSSearchPathDomain : ulong
	{
		None = 0x0,
		User = 0x1,
		Local = 0x2,
		Network = 0x4,
		System = 0x8,
		All = 0xFFFF
	}
}
