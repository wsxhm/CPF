using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIPinchDistortion : CIDistortionFilter
	{
		public float Scale
		{
			get
			{
				return GetFloat("inputScale");
			}
			set
			{
				SetFloat("inputScale", value);
			}
		}

		public CIPinchDistortion()
			: base("CIPinchDistortion")
		{
		}

		public CIPinchDistortion(IntPtr handle)
			: base(handle)
		{
		}
	}
}
