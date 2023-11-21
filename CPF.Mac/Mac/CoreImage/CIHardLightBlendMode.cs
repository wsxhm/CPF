using System;

namespace CPF.Mac.CoreImage
{
	public class CIHardLightBlendMode : CIBlendFilter
	{
		public CIHardLightBlendMode()
			: base("CIHardLightBlendMode")
		{
		}

		public CIHardLightBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
