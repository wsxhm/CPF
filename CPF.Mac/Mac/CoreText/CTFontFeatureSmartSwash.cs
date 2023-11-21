using CPF.Mac.Foundation;

namespace CPF.Mac.CoreText
{
	public class CTFontFeatureSmartSwash : CTFontFeatureSelectors
	{
		public enum Selector
		{
			WordInitialSwashesOn,
			WordInitialSwashesOff,
			WordFinalSwashesOn,
			WordFinalSwashesOff,
			LineInitialSwashesOn,
			LineInitialSwashesOff,
			LineFinalSwashesOn,
			LineFinalSwashesOff,
			NonFinalSwashesOn,
			NonFinalSwashesOff
		}

		public Selector Feature => Feature;

		public CTFontFeatureSmartSwash(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
