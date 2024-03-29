using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreFoundation
{
	public class CFString : INativeObject, IDisposable
	{
		internal IntPtr handle;

		internal string str;

		public IntPtr Handle => handle;

		public int Length
		{
			get
			{
				if (str != null)
				{
					return str.Length;
				}
				return CFStringGetLength(handle);
			}
		}

		public char this[int p]
		{
			get
			{
				if (str != null)
				{
					return str[p];
				}
				return CFStringGetCharacterAtIndex(handle, p);
			}
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringCreateWithCharacters(IntPtr allocator, string str, int count);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern int CFStringGetLength(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringGetCharactersPtr(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern IntPtr CFStringGetCharacters(IntPtr handle, CFRange range, IntPtr buffer);

		public CFString(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			handle = CFStringCreateWithCharacters(IntPtr.Zero, str, str.Length);
			this.str = str;
		}

		~CFString()
		{
			Dispose(disposing: false);
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText", EntryPoint = "CFStringGetTypeID")]
		public static extern int GetTypeID();

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			str = null;
			if (handle != IntPtr.Zero)
			{
				CFObject.CFRelease(handle);
				handle = IntPtr.Zero;
			}
		}

		public CFString(IntPtr handle)
			: this(handle, owns: false)
		{
		}

		[Preserve(Conditional = true)]
		internal CFString(IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		internal unsafe static string FetchString(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return null;
			}
			int num = CFStringGetLength(handle);
			IntPtr intPtr = CFStringGetCharactersPtr(handle);
			IntPtr intPtr2 = IntPtr.Zero;
			if (intPtr == IntPtr.Zero)
			{
				CFRange range = new CFRange(0, num);
				intPtr2 = Marshal.AllocCoTaskMem(num * 2);
				CFStringGetCharacters(handle, range, intPtr2);
				intPtr = intPtr2;
			}
			string result = new string((char*)(void*)intPtr, 0, num);
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr2);
			}
			return result;
		}

		public static implicit operator string(CFString x)
		{
			if (x.str == null)
			{
				x.str = FetchString(x.handle);
			}
			return x.str;
		}

		public static implicit operator CFString(string s)
		{
			return new CFString(s);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern char CFStringGetCharacterAtIndex(IntPtr handle, int p);

		public override string ToString()
		{
			if (str != null)
			{
				return str;
			}
			return FetchString(handle);
		}
	}
}
