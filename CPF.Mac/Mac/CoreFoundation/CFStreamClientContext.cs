using CPF.Mac.Foundation;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreFoundation
{
	public struct CFStreamClientContext
	{
		[MonoNativeFunctionWrapper]
		private delegate IntPtr RetainDelegate(IntPtr info);

		[MonoNativeFunctionWrapper]
		private delegate void ReleaseDelegate(IntPtr info);

		[MonoNativeFunctionWrapper]
		private delegate IntPtr CopyDescriptionDelegate(IntPtr info);

		[MonoNativeFunctionWrapper]
		private delegate void CallbackDelegate(IntPtr stream, CFStreamEventType eventType, IntPtr info);

		public int Version;

		public IntPtr Info;

		private IntPtr retain;

		private IntPtr release;

		private IntPtr copyDescription;

		public void Retain()
		{
			if (!(retain == IntPtr.Zero) && !(Info == IntPtr.Zero))
			{
				CFReadStreamRef_InvokeRetain(retain, Info);
			}
		}

		public void Release()
		{
			if (!(release == IntPtr.Zero) && !(Info == IntPtr.Zero))
			{
				CFReadStreamRef_InvokeRelease(release, Info);
			}
		}

		public override string ToString()
		{
			if (copyDescription == IntPtr.Zero)
			{
				return ((ValueType)this).ToString();
			}
			IntPtr intPtr = CFReadStreamRef_InvokeCopyDescription(copyDescription, Info);
			if (!(intPtr == IntPtr.Zero))
			{
				return new NSString(intPtr).ToString();
			}
			return ((ValueType)this).ToString();
		}

		internal void Invoke(IntPtr callback, IntPtr stream, CFStreamEventType eventType)
		{
			if (!(callback == IntPtr.Zero))
			{
				CFReadStreamRef_InvokeCallback(callback, stream, eventType, Info);
			}
		}

		private static IntPtr CFReadStreamRef_InvokeRetain(IntPtr retain, IntPtr info)
		{
			return ((RetainDelegate)Marshal.GetDelegateForFunctionPointer(retain, typeof(RetainDelegate)))(info);
		}

		private static void CFReadStreamRef_InvokeRelease(IntPtr release, IntPtr info)
		{
			((ReleaseDelegate)Marshal.GetDelegateForFunctionPointer(release, typeof(ReleaseDelegate)))(info);
		}

		private static IntPtr CFReadStreamRef_InvokeCopyDescription(IntPtr copyDescription, IntPtr info)
		{
			return ((CopyDescriptionDelegate)Marshal.GetDelegateForFunctionPointer(copyDescription, typeof(CopyDescriptionDelegate)))(info);
		}

		private static void CFReadStreamRef_InvokeCallback(IntPtr callback, IntPtr stream, CFStreamEventType eventType, IntPtr info)
		{
			((CallbackDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(CallbackDelegate)))(stream, eventType, info);
		}
	}
}
