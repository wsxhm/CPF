using CPF.Mac.CoreFoundation;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreServices
{
	public class CFHTTPMessage : CFType, INativeObject, IDisposable
	{
		private struct CFStreamError
		{
			public int domain;

			public int code;
		}

		private enum ErrorHTTPAuthentication
		{
			TypeUnsupported = -1000,
			BadUserName = -1001,
			BadPassword = -1002
		}

		public enum AuthenticationScheme
		{
			Default,
			Basic,
			Negotiate,
			NTLM,
			Digest
		}

		internal IntPtr handle;

		private static readonly NSString _HTTPVersion1_0;

		private static readonly NSString _HTTPVersion1_1;

		private static readonly NSString _AuthenticationSchemeBasic;

		private static readonly NSString _AuthenticationSchemeNegotiate;

		private static readonly NSString _AuthenticationSchemeNTLM;

		private static readonly NSString _AuthenticationSchemeDigest;

		private static readonly NSString _AuthenticationUsername;

		private static readonly NSString _AuthenticationPassword;

		private static readonly NSString _AuthenticationAccountDomain;

		public IntPtr Handle
		{
			get
			{
				CheckHandle();
				return handle;
			}
		}

		public bool IsRequest
		{
			get
			{
				CheckHandle();
				return CFHTTPMessageIsRequest(Handle);
			}
		}

		public CFIndex ResponseStatusCode
		{
			get
			{
				if (IsRequest)
				{
					throw new InvalidOperationException();
				}
				return CFHTTPMessageGetResponseStatusCode(Handle);
			}
		}

		public string ResponseStatusLine
		{
			get
			{
				if (IsRequest)
				{
					throw new InvalidOperationException();
				}
				IntPtr value = CFHTTPMessageCopyResponseStatusLine(Handle);
				if (value == IntPtr.Zero)
				{
					return null;
				}
				using (NSString nSString = new NSString(value))
				{
					return nSString.ToString();
				}
			}
		}

		public Version Version
		{
			get
			{
				CheckHandle();
				IntPtr intPtr = CFHTTPMessageCopyVersion(handle);
				try
				{
					if (intPtr.Equals(_HTTPVersion1_0.Handle))
					{
						return HttpVersion.Version10;
					}
					return HttpVersion.Version11;
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						CFObject.CFRelease(intPtr);
					}
				}
			}
		}

		public bool IsHeaderComplete
		{
			get
			{
				CheckHandle();
				return CFHTTPMessageIsHeaderComplete(Handle);
			}
		}

		internal CFHTTPMessage(IntPtr handle)
			: this(handle, owns: false)
		{
		}

		internal CFHTTPMessage(IntPtr handle, bool owns)
		{
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
			this.handle = handle;
		}

		static CFHTTPMessage()
		{
			IntPtr value = Dlfcn.dlopen("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork", 0);
			if (value == IntPtr.Zero)
			{
				throw new InvalidOperationException();
			}
			try
			{
				_HTTPVersion1_0 = GetStringConstant(value, "kCFHTTPVersion1_0");
				_HTTPVersion1_1 = GetStringConstant(value, "kCFHTTPVersion1_1");
				_AuthenticationSchemeBasic = GetStringConstant(value, "kCFHTTPAuthenticationSchemeBasic");
				_AuthenticationSchemeNegotiate = GetStringConstant(value, "kCFHTTPAuthenticationSchemeNegotiate");
				_AuthenticationSchemeNTLM = GetStringConstant(value, "kCFHTTPAuthenticationSchemeNTLM");
				_AuthenticationSchemeDigest = GetStringConstant(value, "kCFHTTPAuthenticationSchemeDigest");
				_AuthenticationUsername = GetStringConstant(value, "kCFHTTPAuthenticationUsername");
				_AuthenticationPassword = GetStringConstant(value, "kCFHTTPAuthenticationPassword");
				_AuthenticationAccountDomain = GetStringConstant(value, "kCFHTTPAuthenticationAccountDomain");
			}
			finally
			{
				Dlfcn.dlclose(value);
			}
		}

		private static NSString GetStringConstant(IntPtr handle, string name)
		{
			NSString stringConstant = Dlfcn.GetStringConstant(handle, name);
			if (stringConstant == null)
			{
				throw new InvalidOperationException($"Cannot get '{name}' property.");
			}
			return stringConstant;
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork", EntryPoint = "CFHTTPMessageGetTypeID")]
		public static extern int GetTypeID();

		~CFHTTPMessage()
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

		private static IntPtr GetVersion(Version version)
		{
			if (version == null || version.Equals(HttpVersion.Version11))
			{
				return _HTTPVersion1_1.Handle;
			}
			if (version.Equals(HttpVersion.Version10))
			{
				return _HTTPVersion1_0.Handle;
			}
			throw new ArgumentException();
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPMessageCreateEmpty(IntPtr allocator, bool isRequest);

		public static CFHTTPMessage CreateEmpty(bool request)
		{
			IntPtr value = CFHTTPMessageCreateEmpty(IntPtr.Zero, request);
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new CFHTTPMessage(value);
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPMessageCreateRequest(IntPtr allocator, IntPtr requestMethod, IntPtr url, IntPtr httpVersion);

		public static CFHTTPMessage CreateRequest(CFUrl url, NSString method, Version version)
		{
			IntPtr value = CFHTTPMessageCreateRequest(IntPtr.Zero, method.Handle, url.Handle, GetVersion(version));
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new CFHTTPMessage(value);
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPMessageIsRequest(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern CFIndex CFHTTPMessageGetResponseStatusCode(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPMessageCopyResponseStatusLine(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPMessageCopyVersion(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPMessageIsHeaderComplete(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPMessageAppendBytes(IntPtr message, ref byte[] newBytes, CFIndex numBytes);

		public bool AppendBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return AppendBytes(bytes, bytes.Length);
		}

		public bool AppendBytes(byte[] bytes, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return CFHTTPMessageAppendBytes(Handle, ref bytes, count);
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern IntPtr CFHTTPMessageCopyAllHeaderFields(IntPtr handle);

		public NSDictionary GetAllHeaderFields()
		{
			CheckHandle();
			IntPtr value = CFHTTPMessageCopyAllHeaderFields(handle);
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new NSDictionary(value);
		}

		private InvalidOperationException GetException(ErrorHTTPAuthentication code)
		{
			switch (code)
			{
			case ErrorHTTPAuthentication.BadUserName:
				throw new InvalidOperationException("Bad username.");
			case ErrorHTTPAuthentication.BadPassword:
				throw new InvalidOperationException("Bad password.");
			case ErrorHTTPAuthentication.TypeUnsupported:
				throw new InvalidOperationException("Authentication type not supported.");
			default:
				throw new InvalidOperationException("Unknown error.");
			}
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPMessageApplyCredentials(IntPtr request, IntPtr auth, IntPtr user, IntPtr pass, out CFStreamError error);

		public void ApplyCredentials(CFHTTPAuthentication auth, string userName, string password, string domain = null)
		{
			if (auth.RequiresAccountDomain)
			{
				ApplyCredentialDictionary(auth, userName, password, domain);
				return;
			}
			CFString cFString = new CFString(userName);
			CFString cFString2 = new CFString(password);
			try
			{
				if (!CFHTTPMessageApplyCredentials(Handle, auth.Handle, cFString.Handle, cFString2.Handle, out CFStreamError error))
				{
					throw GetException((ErrorHTTPAuthentication)error.code);
				}
			}
			finally
			{
				cFString.Dispose();
				cFString2.Dispose();
			}
		}

		internal static IntPtr GetAuthScheme(AuthenticationScheme scheme)
		{
			switch (scheme)
			{
			case AuthenticationScheme.Default:
				return IntPtr.Zero;
			case AuthenticationScheme.Basic:
				return _AuthenticationSchemeBasic.Handle;
			case AuthenticationScheme.Negotiate:
				return _AuthenticationSchemeNegotiate.Handle;
			case AuthenticationScheme.NTLM:
				return _AuthenticationSchemeNTLM.Handle;
			case AuthenticationScheme.Digest:
				return _AuthenticationSchemeDigest.Handle;
			default:
				throw new ArgumentException();
			}
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPMessageAddAuthentication(IntPtr request, IntPtr response, IntPtr username, IntPtr password, IntPtr scheme, bool forProxy);

		public bool AddAuthentication(CFHTTPMessage failureResponse, NSString username, NSString password, AuthenticationScheme scheme, bool forProxy)
		{
			return CFHTTPMessageAddAuthentication(Handle, failureResponse.Handle, username.Handle, password.Handle, GetAuthScheme(scheme), forProxy);
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern bool CFHTTPMessageApplyCredentialDictionary(IntPtr request, IntPtr auth, IntPtr dict, out CFStreamError error);

		public void ApplyCredentialDictionary(CFHTTPAuthentication auth, string userName, string password, string domain = null)
		{
			NSString[] array = new NSString[3];
			CFString[] array2 = new CFString[3];
			array[0] = _AuthenticationUsername;
			array[1] = _AuthenticationPassword;
			array[2] = _AuthenticationAccountDomain;
			array2[0] = userName;
			array2[1] = password;
			array2[2] = ((domain != null) ? ((CFString)domain) : null);
			CFDictionary cFDictionary = CFDictionary.FromObjectsAndKeys(array2, array);
			try
			{
				if (!CFHTTPMessageApplyCredentialDictionary(Handle, auth.Handle, cFDictionary.Handle, out CFStreamError error))
				{
					throw GetException((ErrorHTTPAuthentication)error.code);
				}
			}
			finally
			{
				cFDictionary.Dispose();
				array2[0].Dispose();
				array2[1].Dispose();
				if (array2[2] != null)
				{
					array2[2].Dispose();
				}
			}
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern void CFHTTPMessageSetHeaderFieldValue(IntPtr message, IntPtr headerField, IntPtr value);

		public void SetHeaderFieldValue(string name, string value)
		{
			NSString nSString = (NSString)name;
			NSString nSString2 = (value != null) ? ((NSString)value) : null;
			IntPtr value2 = (nSString2 != null) ? nSString2.Handle : IntPtr.Zero;
			CFHTTPMessageSetHeaderFieldValue(Handle, nSString.Handle, value2);
			nSString.Dispose();
			if (nSString2 != null)
			{
				nSString2.Dispose();
			}
		}

		[DllImport("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork")]
		private static extern void CFHTTPMessageSetBody(IntPtr message, IntPtr data);

		internal void SetBody(CFData data)
		{
			CFHTTPMessageSetBody(Handle, data.Handle);
		}

		public void SetBody(byte[] buffer)
		{
			using (CFDataBuffer cFDataBuffer = new CFDataBuffer(buffer))
			{
				CFHTTPMessageSetBody(Handle, cFDataBuffer.Handle);
			}
		}
	}
}
