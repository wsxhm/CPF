namespace CPF.Mac.Foundation
{
	public enum NSTextCheckingType : ulong
	{
		Orthography = 1uL,
		Spelling = 2uL,
		Grammar = 4uL,
		Date = 8uL,
		Address = 0x10,
		Link = 0x20,
		Quote = 0x40,
		Dash = 0x80,
		Replacement = 0x100,
		Correction = 0x200
	}
}
