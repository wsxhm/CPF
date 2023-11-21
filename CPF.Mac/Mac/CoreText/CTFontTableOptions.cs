using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	[Flags]
	public enum CTFontTableOptions : uint
	{
		None = 0x0,
		ExcludeSynthetic = 0x1
	}
}
