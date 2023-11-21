using System;

namespace CPF.Mac.CoreImage
{
	public class CIDifferenceBlendMode : CIBlendFilter
	{
		public CIDifferenceBlendMode()
			: base("CIDifferenceBlendMode")
		{
		}

		public CIDifferenceBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
