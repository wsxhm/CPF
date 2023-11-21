using System;

namespace CPF.Mac.CoreData
{
	[Flags]
	public enum NSFetchRequestResultType : ulong
	{
		ManagedObject = 0x0,
		ManagedObjectID = 0x1,
		DictionaryResultType = 0x2,
		NSCountResultType = 0x4
	}
}
