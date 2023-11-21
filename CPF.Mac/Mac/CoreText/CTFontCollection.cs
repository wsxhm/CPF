using CPF.Mac.CoreFoundation;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public class CTFontCollection : INativeObject, IDisposable
	{
		private delegate int CTFontCollectionSortDescriptorsCallback(IntPtr first, IntPtr second, IntPtr refCon);

		internal IntPtr handle;

		public IntPtr Handle => handle;

		internal CTFontCollection(IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
			{
				throw ConstructorError.ArgumentNull(this, "handle");
			}
			this.handle = handle;
			if (!owns)
			{
				CFObject.CFRetain(handle);
			}
		}

		~CTFontCollection()
		{
			Dispose(disposing: false);
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

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFontCollectionCreateFromAvailableFonts(IntPtr options);

		public CTFontCollection(CTFontCollectionOptions options)
		{
			handle = CTFontCollectionCreateFromAvailableFonts(options?.Dictionary.Handle ?? IntPtr.Zero);
			if (handle == IntPtr.Zero)
			{
				throw ConstructorError.Unknown(this);
			}
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFontCollectionCreateWithFontDescriptors(IntPtr queryDescriptors, IntPtr options);

		public CTFontCollection(CTFontDescriptor[] queryDescriptors, CTFontCollectionOptions options)
		{
			using (CFArray cFArray = (queryDescriptors == null) ? null : CFArray.FromNativeObjects(queryDescriptors))
			{
				handle = CTFontCollectionCreateWithFontDescriptors(cFArray?.Handle ?? IntPtr.Zero, options?.Dictionary.Handle ?? IntPtr.Zero);
			}
			if (handle == IntPtr.Zero)
			{
				throw ConstructorError.Unknown(this);
			}
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFontCollectionCreateCopyWithFontDescriptors(IntPtr original, IntPtr queryDescriptors, IntPtr options);

		public CTFontCollection WithFontDescriptors(CTFontDescriptor[] queryDescriptors, CTFontCollectionOptions options)
		{
			IntPtr value;
			using (CFArray cFArray = (queryDescriptors == null) ? null : CFArray.FromNativeObjects(queryDescriptors))
			{
				value = CTFontCollectionCreateCopyWithFontDescriptors(handle, cFArray?.Handle ?? IntPtr.Zero, options?.Dictionary.Handle ?? IntPtr.Zero);
			}
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new CTFontCollection(value, owns: true);
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFontCollectionCreateMatchingFontDescriptors(IntPtr collection);

		public CTFontDescriptor[] GetMatchingFontDescriptors()
		{
			IntPtr intPtr = CTFontCollectionCreateMatchingFontDescriptors(handle);
			if (intPtr == IntPtr.Zero)
			{
				return new CTFontDescriptor[0];
			}
			CTFontDescriptor[] result = NSArray.ArrayFromHandle(intPtr, (IntPtr fd) => new CTFontDescriptor(fd, owns: false));
			CFObject.CFRelease(intPtr);
			return result;
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback(IntPtr collection, CTFontCollectionSortDescriptorsCallback sortCallback, IntPtr refCon);

		[MonoPInvokeCallback(typeof(CTFontCollectionSortDescriptorsCallback))]
		private static int CompareDescriptors(IntPtr first, IntPtr second, IntPtr context)
		{
			return (GCHandle.FromIntPtr(context).Target as Comparison<CTFontDescriptor>)?.Invoke(new CTFontDescriptor(first, owns: false), new CTFontDescriptor(second, owns: false)) ?? 0;
		}

		public CTFontDescriptor[] GetMatchingFontDescriptors(Comparison<CTFontDescriptor> comparer)
		{
			GCHandle value = GCHandle.Alloc(comparer);
			try
			{
				IntPtr cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback(handle, CompareDescriptors, GCHandle.ToIntPtr(value));
				if (cfArrayRef == IntPtr.Zero)
				{
					return new CTFontDescriptor[0];
				}
				CTFontDescriptor[] result = NSArray.ArrayFromHandle(cfArrayRef, (IntPtr fd) => new CTFontDescriptor(cfArrayRef, owns: false));
				CFObject.CFRelease(cfArrayRef);
				return result;
			}
			finally
			{
				value.Free();
			}
		}
	}
}
