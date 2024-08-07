using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreImage
{
	[Since(6, 0)]
	public class CIDotScreen : CIScreenFilter
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

		public CIDotScreen()
			: base("CIDotScreen")
		{
		}

		public CIDotScreen(IntPtr handle)
			: base(handle)
		{
		}
	}
}
