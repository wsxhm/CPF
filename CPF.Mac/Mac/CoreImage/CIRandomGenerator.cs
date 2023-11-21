using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIRandomGenerator : CIFilter
	{
		public CIRandomGenerator()
			: base("CIRandomGenerator")
		{
		}

		public CIRandomGenerator(IntPtr handle)
			: base(handle)
		{
		}
	}
}
