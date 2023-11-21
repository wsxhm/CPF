using CPF.Mac.CoreGraphics;
using CPF.Mac.Foundation;

namespace CPF.Mac.CoreImage
{
	public class CIImageInitializationOptions : DictionaryContainer
	{
		public CGColorSpace ColorSpace
		{
			set
			{
				SetNativeValue(CIImage.CIImageColorSpaceKey, (value == null) ? null : value);
			}
		}

		public CIImageInitializationOptions()
			: base(new NSMutableDictionary())
		{
		}

		public CIImageInitializationOptions(NSDictionary dictionary)
			: base(dictionary)
		{
		}
	}
}
