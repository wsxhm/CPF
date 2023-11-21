using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSFontPanelMode
	{
		FaceMask = 0x1,
		SizeMask = 0x2,
		CollectionMask = 0x4,
		UnderlineEffectMask = 0x100,
		StrikethroughEffectMask = 0x200,
		TextColorEffectMask = 0x400,
		DocumentColorEffectMask = 0x800,
		ShadowEffectMask = 0x1000,
		AllEffectsMask = 0xFFF00,
		StandardMask = 0xFFFF,
		AllModesMask = -1
	}
}
