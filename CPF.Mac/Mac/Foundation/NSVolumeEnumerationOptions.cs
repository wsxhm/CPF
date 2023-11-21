using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Foundation
{
	[Since(4, 0)]
	[Flags]
	public enum NSVolumeEnumerationOptions : ulong
	{
		None = 0x0,
		SkipHiddenVolumes = 0x2,
		ProduceFileReferenceUrls = 0x4
	}
}
