using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureContextualAlternates : CTFontFeatureSelectors
	{
		public enum Selector
		{
			ContextualAlternatesOn,
			ContextualAlternatesOff,
			SwashAlternatesOn,
			SwashAlternatesOff,
			ContextualSwashAlternatesOn,
			ContextualSwashAlternatesOff
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureContextualAlternates(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
