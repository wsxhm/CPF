using System;

namespace CPF.Mac.AppKit
{
	[Flags]
	public enum NSTypesetterControlCharacterAction : ulong
	{
		ZeroAdvancement = 0x1,
		Whitespace = 0x2,
		HorizontalTab = 0x4,
		LineBreak = 0x8,
		ParagraphBreak = 0x10,
		ContainerBreak = 0x20
	}
}
