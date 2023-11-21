using System;

namespace CPF.Mac.CoreImage
{
	public class CILuminosityBlendMode : CIBlendFilter
	{
		public CILuminosityBlendMode()
			: base("CILuminosityBlendMode")
		{
		}

		public CILuminosityBlendMode(IntPtr handle)
			: base(handle)
		{
		}
	}
}
