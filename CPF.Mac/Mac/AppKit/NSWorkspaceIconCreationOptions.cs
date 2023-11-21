using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSWorkspaceIconCreationOptions : ulong
	{
		NSExcludeQuickDrawElements = 0x2,
		NSExclude10_4Elements = 0x4
	}
}
