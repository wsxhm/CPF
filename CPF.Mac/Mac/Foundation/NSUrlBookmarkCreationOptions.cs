using System;

namespace CPF.Mac.Foundation
{
	[Flags]
	public enum NSUrlBookmarkCreationOptions : ulong
	{
		PreferFileIDResolution = 0x100,
		MinimalBookmark = 0x200,
		SuitableForBookmarkFile = 0x400,
		WithSecurityScope = 0x800,
		SecurityScopeAllowOnlyReadAccess = 0x1000
	}
}
