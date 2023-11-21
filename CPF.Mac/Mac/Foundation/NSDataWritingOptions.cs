using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSDataWritingOptions : ulong
	{
		Atomic = 0x1,
		WithoutOverwriting = 0x2,
		[Obsolete("No longer available")]
		Coordinated = 0x4,
		[Since(4, 0)]
		FileProtectionNone = 0x10000000,
		[Since(4, 0)]
		FileProtectionComplete = 0x20000000,
		[Since(4, 0)]
		FileProtectionMask = 0xF0000000,
		[Since(5, 0)]
		FileProtectionCompleteUnlessOpen = 0x30000000,
		[Since(5, 0)]
		FileProtectionCompleteUntilFirstUserAuthentication = 0x40000000
	}
}
