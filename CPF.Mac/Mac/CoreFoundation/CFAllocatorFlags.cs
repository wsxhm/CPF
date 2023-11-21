using System;

namespace CPF.Mac.CoreFoundation
{
	[Flags]
	public enum CFAllocatorFlags : ulong
	{
		GCScannedMemory = 0x200,
		GCObjectMemory = 0x400
	}
}
