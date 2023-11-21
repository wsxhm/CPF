using System;

namespace CPF.Mac.CoreImage
{
	public class CIHueBlendMode : CIBlendFilter
	{
		public CIHueBlendMode()
			: base("CIHueBlendMode")
		{
		}

		public CIHueBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
