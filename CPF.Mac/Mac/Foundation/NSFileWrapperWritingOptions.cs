using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSFileWrapperWritingOptions : ulong
	{
		Atomic = 0x1,
		WithNameUpdating = 0x2
	}
}
