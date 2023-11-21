using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureIdeographicSpacing : CTFontFeatureSelectors
	{
		public enum Selector
		{
			FullWidthIdeographs,
			ProportionalIdeographs,
			HalfWidthIdeographs
		}

		public Selector Feature => (Selector)base.FeatureWeak;

		public CTFontFeatureIdeographicSpacing(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
