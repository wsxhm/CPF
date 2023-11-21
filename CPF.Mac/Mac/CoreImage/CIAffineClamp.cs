using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIAffineClamp : CIAffineFilter
	{
		public CIAffineClamp()
			: base("CIAffineClamp")
		{
		}

		public CIAffineClamp(IntPtr handle)
			: base(handle)
		{
		}
	}
}
