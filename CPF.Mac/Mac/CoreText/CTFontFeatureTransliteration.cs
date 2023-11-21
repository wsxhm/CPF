using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureTransliteration : CTFontFeatureSelectors
	{
		public enum Selector
		{
			NoTransliteration,
			HanjaToHangul,
			HiraganaToKatakana,
			KatakanaToHiragana,
			KanaToRomanization,
			RomanizationToHiragana,
			RomanizationToKatakana,
			HanjaToHangulAltOne,
			HanjaToHangulAltTwo,
			HanjaToHangulAltThree
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureTransliteration(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
