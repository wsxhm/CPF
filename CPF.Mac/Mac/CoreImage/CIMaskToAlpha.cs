using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIMaskToAlpha : CIFilter
	{
		public CIImage Image
		{
			get
			{
				return GetInputImage();
			}
			set
			{
				SetInputImage(value);
			}
		}

		public CIMaskToAlpha()
			: base("CIMaskToAlpha")
		{
		}

		public CIMaskToAlpha(IntPtr handle)
			: base(handle)
		{
		}
	}
}
