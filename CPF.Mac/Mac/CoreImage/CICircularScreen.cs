using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CICircularScreen : CIScreenFilter
	{
		public CICircularScreen()
			: base("CICircularScreen")
		{
		}

		public CICircularScreen(IntPtr handle)
			: base(handle)
		{
		}
	}
}
