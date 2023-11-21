using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CISourceOverCompositing : CICompositingFilter
	{
		public CISourceOverCompositing()
			: base("CISourceOverCompositing")
		{
		}

		public CISourceOverCompositing(IntPtr handle)
			: base(handle)
		{
		}
	}
}
