using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureCharacterAlternatives : CTFontFeatureSelectors
	{
		public enum Selector
		{
			NoAlternates
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureCharacterAlternatives(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
