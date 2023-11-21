using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureUnicodeDecomposition : CTFontFeatureSelectors
	{
		public enum Selector
		{
			CanonicalCompositionOn,
			CanonicalCompositionOff,
			CompatibilityCompositionOn,
			CompatibilityCompositionOff,
			TranscodingCompositionOn,
			TranscodingCompositionOff
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureUnicodeDecomposition(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
