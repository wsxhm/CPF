using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIPixellate : CIFilter
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

		public CIVector Center
		{
			get
			{
				return GetVector("inputCenter");
			}
			set
			{
				SetValue("inputCenter", value);
			}
		}

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

		public CIPixellate()
			: base("CIPixellate")
		{
		}

		public CIPixellate(IntPtr handle)
			: base(handle)
		{
		}
	}
}
