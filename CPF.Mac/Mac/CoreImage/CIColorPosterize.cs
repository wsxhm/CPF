using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIColorPosterize : CIFilter
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

		public float Levels
		{
			get
			{
				return GetFloat("inputLevels");
			}
			set
			{
				SetFloat("inputLevels", value);
			}
		}

		public CIColorPosterize()
			: base("CIColorPosterize")
		{
		}

		public CIColorPosterize(IntPtr handle)
			: base(handle)
		{
		}
	}
}
