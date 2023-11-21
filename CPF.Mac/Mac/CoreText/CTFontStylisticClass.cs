using CPF.Mac.ObjCRuntime;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public enum CTFontStylisticClass : uint
	{
		None = 0u,
		Unknown = 0u,
		OldStyleSerifs = 0x10000000,
		TransitionalSerifs = 0x20000000,
		ModernSerifs = 805306368u,
		ClarendonSerifs = 0x40000000,
		SlabSerifs = 1342177280u,
		FreeformSerifs = 1879048192u,
		SansSerif = 0x80000000,
		Ornamentals = 2415919104u,
		Scripts = 2684354560u,
		Symbolic = 3221225472u
	}
}
