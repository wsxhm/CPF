using System;

namespace CPF.Mac.CoreImage
{
	public class CISoftLightBlendMode : CIBlendFilter
	{
		public CISoftLightBlendMode()
			: base("CISoftLightBlendMode")
		{
		}

		public CISoftLightBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
