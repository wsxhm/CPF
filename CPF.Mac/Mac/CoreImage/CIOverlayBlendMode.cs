using System;

namespace CPF.Mac.CoreImage
{
	public class CIOverlayBlendMode : CIBlendFilter
	{
		public CIOverlayBlendMode()
			: base("CIOverlayBlendMode")
		{
		}

		public CIOverlayBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
