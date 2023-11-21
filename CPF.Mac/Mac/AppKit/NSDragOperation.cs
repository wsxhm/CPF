using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSDragOperation : ulong
	{
		None = 0x0,
		Copy = 0x1,
		Link = 0x2,
		Generic = 0x4,
		Private = 0x8,
		AllObsolete = 0xF,
		Move = 0x10,
		Delete = 0x20,
		All = ulong.MaxValue
	}
}
