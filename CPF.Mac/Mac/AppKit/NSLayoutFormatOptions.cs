namespace CPF.Mac.AppKit
{
	public enum NSLayoutFormatOptions : ulong
	{
		None = 0uL,
		AlignAllLeft = 2uL,
		AlignAllRight = 4uL,
		AlignAllTop = 8uL,
		AlignAllBottom = 0x10,
		AlignAllLeading = 0x20,
		AlignAllTrailing = 0x40,
		AlignAllCenterX = 0x200,
		AlignAllCenterY = 0x400,
		AlignAllBaseline = 0x800,
		AlignmentMask = 0xFFFF,
		DirectionLeadingToTrailing = 0uL,
		DirectionLeftToRight = 0x10000,
		DirectionRightToLeft = 0x20000,
		DirectionMask = 196608uL
	}
}
