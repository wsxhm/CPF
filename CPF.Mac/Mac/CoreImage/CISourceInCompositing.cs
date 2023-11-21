using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CISourceInCompositing : CICompositingFilter
	{
		public CISourceInCompositing()
			: base("CISourceInCompositing")
		{
		}

		public CISourceInCompositing(IntPtr handle)
			: base(handle)
		{
		}
	}
}
