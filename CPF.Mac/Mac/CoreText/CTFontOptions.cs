using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	[Flags]
	public enum CTFontOptions
	{
		Default = 0x0,
		PreventAutoActivation = 0x1,
		PreferSystemFont = 0x4,
		IncludeDisabled = 0x80
	}
}
