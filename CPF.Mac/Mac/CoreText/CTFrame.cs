using CPF.Mac.CoreFoundation;
using CPF.Mac.CoreGraphics;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public class CTFrame : INativeObject, IDisposable
	{
		internal IntPtr handle;

		public IntPtr Handle => handle;

		internal CTFrame(IntPtr handle, bool owns)
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

		~CTFrame()
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
		private static extern NSRange CTFrameGetStringRange(IntPtr handle);

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern NSRange CTFrameGetVisibleStringRange(IntPtr handle);

		public NSRange GetStringRange()
		{
			return CTFrameGetStringRange(handle);
		}

		public NSRange GetVisibleStringRange()
		{
			return CTFrameGetVisibleStringRange(handle);
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFrameGetPath(IntPtr handle);

		public CGPath GetPath()
		{
			IntPtr value = CTFrameGetPath(handle);
			if (!(value == IntPtr.Zero))
			{
				return new CGPath(value, owns: false);
			}
			return null;
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFrameGetFrameAttributes(IntPtr handle);

		public CTFrameAttributes GetFrameAttributes()
		{
			NSDictionary nSDictionary = (NSDictionary)Runtime.GetNSObject(CTFrameGetFrameAttributes(handle));
			if (nSDictionary != null)
			{
				return new CTFrameAttributes(nSDictionary);
			}
			return null;
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFrameGetLines(IntPtr handle);

		public CTLine[] GetLines()
		{
			IntPtr value = CTFrameGetLines(handle);
			if (value == IntPtr.Zero)
			{
				return new CTLine[0];
			}
			return NSArray.ArrayFromHandleFunc(value, (IntPtr p) => new CTLine(p, owns: false));
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern void CTFrameGetLineOrigins(IntPtr handle, NSRange range, [Out] CGPoint[] origins);

		public void GetLineOrigins(NSRange range, CGPoint[] origins)
		{
			if (origins == null)
			{
				throw new ArgumentNullException("origins");
			}
			if (range.Length != 0L && (ulong)origins.Length < range.Length)
			{
				throw new ArgumentException("origins must contain at least range.Length elements.", "origins");
			}
			if (origins.Length < CFArray.GetCount(CTFrameGetLines(handle)))
			{
				throw new ArgumentException("origins must contain at least GetLines().Length elements.", "origins");
			}
			CTFrameGetLineOrigins(handle, range, origins);
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern void CTFrameDraw(IntPtr handle, IntPtr context);

		public void Draw(CGContext ctx)
		{
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			CTFrameDraw(handle, ctx.Handle);
		}
	}
}
