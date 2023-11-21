using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSUrlBookmarkResolutionOptions : ulong
	{
		WithoutUI = 0x100,
		WithoutMounting = 0x200,
		WithSecurityScope = 0x400
	}
}
