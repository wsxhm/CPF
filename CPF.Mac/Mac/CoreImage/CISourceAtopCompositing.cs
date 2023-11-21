using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CISourceAtopCompositing : CICompositingFilter
	{
		public CISourceAtopCompositing()
			: base("CISourceAtopCompositing")
		{
		}

		public CISourceAtopCompositing(IntPtr handle)
			: base(handle)
		{
		}
	}
}
