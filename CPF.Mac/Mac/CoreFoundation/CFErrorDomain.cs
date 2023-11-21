using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.CoreFoundation
{
	public static class CFErrorDomain
	{
		public static readonly NSString Cocoa;

		public static readonly NSString Mach;

		public static readonly NSString OSStatus;

		public static readonly NSString Posix;

		static CFErrorDomain()
		{
			IntPtr intPtr = Dlfcn.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
			if (!(intPtr == IntPtr.Zero))
			{
				try
				{
					Cocoa = Dlfcn.GetStringConstant(intPtr, "kCFErrorDomainCocoa");
					Mach = Dlfcn.GetStringConstant(intPtr, "kCFErrorDomainMach");
					OSStatus = Dlfcn.GetStringConstant(intPtr, "kCFErrorDomainOSStatus");
					Posix = Dlfcn.GetStringConstant(intPtr, "kCFErrorDomainPosix");
				}
				finally
				{
					Dlfcn.dlclose(intPtr);
				}
			}
		}
	}
}
