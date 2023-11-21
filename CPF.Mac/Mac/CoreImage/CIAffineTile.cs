using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIAffineTile : CIAffineFilter
	{
		public CIAffineTile()
			: base("CIAffineTile")
		{
		}

		public CIAffineTile(IntPtr handle)
			: base(handle)
		{
		}
	}
}
