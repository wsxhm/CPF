using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CITwirlDistortion : CIDistortionFilter
	{
		public CITwirlDistortion()
			: base("CITwirlDistortion")
		{
		}

		public CITwirlDistortion(IntPtr handle)
			: base(handle)
		{
		}
	}
}
