using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Foundation
{
	[Since(4, 0)]
	[Flags]
	public enum NSDirectoryEnumerationOptions : ulong
	{
		SkipsNone = 0x0,
		SkipsSubdirectoryDescendants = 0x1,
		SkipsPackageDescendants = 0x2,
		SkipsHiddenFiles = 0x4
	}
}
