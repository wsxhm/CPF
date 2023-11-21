using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSFontSymbolicTraits
	{
		ItalicTrait = 0x1,
		BoldTrait = 0x2,
		ExpandedTrait = 0x20,
		CondensedTrait = 0x40,
		MonoSpaceTrait = 0x400,
		VerticalTrait = 0x800,
		UIOptimizedTrait = 0x1000,
		UnknownClass = 0x0,
		OldStyleSerifsClass = 0x10000000,
		TransitionalSerifsClass = 0x20000000,
		ModernSerifsClass = 0x30000000,
		ClarendonSerifsClass = 0x40000000,
		SlabSerifsClass = 0x50000000,
		FreeformSerifsClass = 0x70000000,
		SansSerifClass = int.MinValue,
		OrnamentalsClass = -1879048192,
		ScriptsClass = -1610612736,
		SymbolicClass = -1073741824,
		FamilyClassMask = -268435456
	}
}
