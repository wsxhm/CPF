using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreFoundation
{
	[Since(3, 2)]
	internal class CFBoolean : INativeObject, IDisposable
	{
		private IntPtr handle;

		public static readonly CFBoolean True;

		public static readonly CFBoolean False;

		public IntPtr Handle => handle;

		public bool Value => CFBooleanGetValue(handle);

		static CFBoolean()
		{
			IntPtr value = Dlfcn.dlopen("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", 0);
			if (!(value == IntPtr.Zero))
			{
				try
				{
					True = new CFBoolean(Dlfcn.GetIntPtr(value, "kCFBooleanTrue"), owns: false);
					False = new CFBoolean(Dlfcn.GetIntPtr(value, "kCFBooleanFalse"), owns: false);
				}
				finally
				{
					Dlfcn.dlclose(value);
				}
			}
		}

		[Preserve(Conditional = true)]
		internal CFBoolean(IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		~CFBoolean()
		{
			Dispose(disposing: false);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", EntryPoint = "CFBooleanGetTypeID")]
		public static extern int GetTypeID();

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (handle != IntPtr.Zero)
			{
				CFObject.CFRelease(handle);
				handle = IntPtr.Zero;
			}
		}

		public static implicit operator bool(CFBoolean value)
		{
			return value.Value;
		}

		public static explicit operator CFBoolean(bool value)
		{
			return FromBoolean(value);
		}

		public static CFBoolean FromBoolean(bool value)
		{
			if (!value)
			{
				return False;
			}
			return True;
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", CharSet = CharSet.Unicode)]
		private static extern bool CFBooleanGetValue(IntPtr boolean);

		public static bool GetValue(IntPtr boolean)
		{
			return CFBooleanGetValue(boolean);
		}
	}
}
