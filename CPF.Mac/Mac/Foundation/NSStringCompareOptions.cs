namespace CPF.Mac.Foundation
{
	public enum NSStringCompareOptions : uint
	{
		CaseInsensitiveSearch = 1u,
		LiteralSearch = 2u,
		BackwardsSearch = 4u,
		AnchoredSearch = 8u,
		NumericSearch = 0x40,
		DiacriticInsensitiveSearch = 0x80,
		WidthInsensitiveSearch = 0x100,
		ForcedOrderingSearch = 0x200,
		RegularExpressionSearch = 0x400
	}
}
