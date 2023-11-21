using CPF.Mac.ObjCRuntime;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public enum CTFontFormat : uint
	{
		Unrecognized,
		OpenTypePostScript,
		OpenTypeTrueType,
		TrueType,
		PostScript,
		Bitmap
	}
}
