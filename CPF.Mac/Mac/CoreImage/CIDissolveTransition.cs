using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIDissolveTransition : CITransitionFilter
	{
		public CIDissolveTransition()
			: base("CIDissolveTransition")
		{
		}

		public CIDissolveTransition(IntPtr handle)
			: base(handle)
		{
		}
	}
}
