using System;

namespace CPF.Mac.CoreImage
{
	public class CILightenBlendMode : CIBlendFilter
	{
		public CILightenBlendMode()
			: base("CILightenBlendMode")
		{
		}

		public CILightenBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
