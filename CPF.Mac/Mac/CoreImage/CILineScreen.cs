using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CILineScreen : CIScreenFilter
	{
		public float Angle
		{
			get
			{
				return GetFloat("inputAngle");
			}
			set
			{
				SetFloat("inputAngle", value);
			}
		}

		public CILineScreen()
			: base("CILineScreen")
		{
		}

		public CILineScreen(IntPtr handle)
			: base(handle)
		{
		}
	}
}
