using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSFileCoordinatorWritingOptions : ulong
	{
		ForDeleting = 0x1,
		ForMoving = 0x2,
		ForMerging = 0x4,
		ForReplacing = 0x8
	}
}
