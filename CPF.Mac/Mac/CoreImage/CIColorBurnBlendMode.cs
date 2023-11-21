using System;

namespace CPF.Mac.CoreImage
{
	public class CIColorBurnBlendMode : CIBlendFilter
	{
		public CIColorBurnBlendMode()
			: base("CIColorBurnBlendMode")
		{
		}

		public CIColorBurnBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
