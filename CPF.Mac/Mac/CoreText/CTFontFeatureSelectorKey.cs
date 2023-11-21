using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public class CTFontFeatureSelectorKey
	{
		public static readonly NSString Identifier;

		public static readonly NSString Name;

		public static readonly NSString Default;

		public static readonly NSString Setting;

		static CTFontFeatureSelectorKey()
		{
			IntPtr intPtr = Dlfcn.dlopen("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText", 0);
			if (!(intPtr == IntPtr.Zero))
			{
				try
				{
					Identifier = Dlfcn.GetStringConstant(intPtr, "kCTFontFeatureSelectorIdentifierKey");
					Name = Dlfcn.GetStringConstant(intPtr, "kCTFontFeatureSelectorNameKey");
					Default = Dlfcn.GetStringConstant(intPtr, "kCTFontFeatureSelectorDefaultKey");
					Setting = Dlfcn.GetStringConstant(intPtr, "kCTFontFeatureSelectorSettingKey");
				}
				finally
				{
					Dlfcn.dlclose(intPtr);
				}
			}
		}
	}
}
