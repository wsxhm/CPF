using System;

namespace CPF.Mac.CoreGraphics
{
	[Flags]
	public enum CGBitmapFlags
	{
		None = 0x0,
		PremultipliedLast = 0x1,
		PremultipliedFirst = 0x2,
		Last = 0x3,
		First = 0x4,
		NoneSkipLast = 0x5,
		NoneSkipFirst = 0x6,
		Only = 0x7,
		AlphaInfoMask = 0x1F,
		FloatComponents = 0x100,
		ByteOrderMask = 0x7000,
		ByteOrderDefault = 0x0,
		ByteOrder16Little = 0x1000,
		ByteOrder32Little = 0x2000,
		ByteOrder16Big = 0x3000,
		ByteOrder32Big = 0x4000
	}
}
