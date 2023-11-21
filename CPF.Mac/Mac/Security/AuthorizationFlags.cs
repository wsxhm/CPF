using System;

namespace CPF.Mac.Security
{
	[Flags]
	public enum AuthorizationFlags
	{
		Defaults = 0x0,
		InteractionAllowed = 0x1,
		ExtendRights = 0x2,
		PartialRights = 0x4,
		DestroyRights = 0x8,
		PreAuthorize = 0x10
	}
}
