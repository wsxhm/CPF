using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;

namespace CPF.Mac.AppKit
{
	public class NSWorkspaceApplicationEventArgs : NSNotificationEventArgs
	{
		private static IntPtr k0;

		public NSRunningApplication Application
		{
			get
			{
				if (k0 == IntPtr.Zero)
				{
					k0 = Dlfcn.GetIntPtr(Libraries.AppKit.Handle, "NSWorkspaceApplicationKey");
				}
				IntPtr intPtr = base.Notification.UserInfo.LowlevelObjectForKey(k0);
				if (intPtr == IntPtr.Zero)
				{
					return null;
				}
				return (NSRunningApplication)Runtime.GetNSObject(intPtr);
			}
		}

		public NSWorkspaceApplicationEventArgs(NSNotification notification)
			: base(notification)
		{
		}
	}
}
