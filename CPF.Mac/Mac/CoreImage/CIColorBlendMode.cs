using System;

namespace CPF.Mac.CoreImage
{
	public class CIColorBlendMode : CIBlendFilter
	{
		public CIColorBlendMode()
			: base("CIColorBlendMode")
		{
		}

		public CIColorBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
