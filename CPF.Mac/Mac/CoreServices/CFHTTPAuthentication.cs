using CPF.Mac.CoreFoundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreServices
{
	public class CFHTTPAuthentication : CFType, INativeObject, IDisposable
	{
		internal IntPtr handle;

		public IntPtr Handle
		{
			get
			{
				CheckHandle();
				return handle;
			}
		}

		public bool IsValid => CFHTTPAuthenticationIsValid(Handle, IntPtr.Zero);

		public bool RequiresAccountDomain => CFHTTPAuthenticationRequiresAccountDomain(Handle);

		public bool RequiresOrderedRequests => CFHTTPAuthenticationRequiresOrderedRequests(Handle);

		public bool RequiresUserNameAndPassword => CFHTTPAuthenticationRequiresUserNameAndPassword(Handle);

		internal CFHTTPAuthentication(IntPtr handle)
			: this(handle, owns: false)
		{
		}

		internal CFHTTPAuthentication(IntPtr handle, bool owns)
		{
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
			this.handle = handle;
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork", EntryPoint = "CFHTTPAuthenticationGetTypeID")]
		public static extern int GetTypeID();

		~CFHTTPAuthentication()
		{
			Dispose(disposing: false);
		}

		protected void CheckHandle()
		{
			if (handle == IntPtr.Zero)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (handle != IntPtr.Zero)
			{
				CFObject.CFRelease(handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPAuthenticationCreateFromResponse(IntPtr allocator, IntPtr response);

		public static CFHTTPAuthentication CreateFromResponse(CFHTTPMessage response)
		{
			if (response.IsRequest)
			{
				throw new InvalidOperationException();
			}
			IntPtr value = CFHTTPAuthenticationCreateFromResponse(IntPtr.Zero, response.Handle);
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new CFHTTPAuthentication(value);
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPAuthenticationIsValid(IntPtr handle, IntPtr error);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPAuthenticationAppliesToRequest(IntPtr handle, IntPtr request);

		public bool AppliesToRequest(CFHTTPMessage request)
		{
			if (!request.IsRequest)
			{
				throw new InvalidOperationException();
			}
			return CFHTTPAuthenticationAppliesToRequest(Handle, request.Handle);
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPAuthenticationRequiresAccountDomain(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPAuthenticationRequiresOrderedRequests(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPAuthenticationRequiresUserNameAndPassword(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPAuthenticationCopyMethod(IntPtr handle);

		public string GetMethod()
		{
			IntPtr value = CFHTTPAuthenticationCopyMethod(Handle);
			if (value == IntPtr.Zero)
			{
				return null;
			}
			using (CFString x = new CFString(value))
			{
				return x;
			}
		}
	}
}
