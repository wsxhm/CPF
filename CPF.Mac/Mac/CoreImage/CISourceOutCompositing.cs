using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CISourceOutCompositing : CICompositingFilter
	{
		public CISourceOutCompositing()
			: base("CISourceOutCompositing")
		{
		}

		public CISourceOutCompositing(IntPtr handle)
			: base(handle)
		{
		}
	}
}
