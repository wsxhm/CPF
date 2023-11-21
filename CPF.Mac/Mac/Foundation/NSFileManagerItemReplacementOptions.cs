using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.Foundation
{
	[Since(4, 0)]
	[Flags]
	public enum NSFileManagerItemReplacementOptions : ulong
	{
		None = 0x0,
		UsingNewMetadataOnly = 0x1,
		WithoutDeletingBackupItem = 0x2
	}
}
