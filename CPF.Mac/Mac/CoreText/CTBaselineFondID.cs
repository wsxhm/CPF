using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	internal static class CTBaselineFondID
	{
		public static readonly NSString Reference;

		public static readonly NSString Original;

		static CTBaselineFondID()
		{
			IntPtr intPtr = Dlfcn.dlopen("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText", 0);
			if (!(intPtr == IntPtr.Zero))
			{
				try
				{
					Reference = Dlfcn.GetStringConstant(intPtr, "kCTBaselineReferenceFont");
					Original = Dlfcn.GetStringConstant(intPtr, "kCTBaselineOriginalFont");
				}
				finally
				{
					Dlfcn.dlclose(intPtr);
				}
			}
		}

		public static NSString ToNSString(CTBaselineFont key)
		{
			switch (key)
			{
			case CTBaselineFont.Reference:
				return Reference;
			case CTBaselineFont.Original:
				return Original;
			default:
				throw new ArgumentOutOfRangeException("key");
			}
		}
	}
}
