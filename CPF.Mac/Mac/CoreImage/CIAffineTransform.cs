using System;

namespace CPF.Mac.CoreImage
{
	public class CIAffineTransform : CIAffineFilter
	{
		public CIAffineTransform()
			: base("CIAffineTransform")
		{
		}

		public CIAffineTransform(IntPtr handle)
			: base(handle)
		{
		}
	}
}
