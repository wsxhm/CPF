using System;

namespace CPF.Mac.CoreImage
{
	public class CIMultiplyBlendMode : CIBlendFilter
	{
		public CIMultiplyBlendMode()
			: base("CIMultiplyBlendMode")
		{
		}

		public CIMultiplyBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
