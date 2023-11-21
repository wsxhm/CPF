using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureNumberCase : CTFontFeatureSelectors
	{
		public enum Selector
		{
			LowerCaseNumbers,
			UpperCaseNumbers
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureNumberCase(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
