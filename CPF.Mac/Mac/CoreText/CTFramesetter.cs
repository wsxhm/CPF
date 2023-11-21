using CPF.Mac.CoreFoundation;
using CPF.Mac.CoreGraphics;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Runtime.InteropServices;

namespace CPF.Mac.CoreText
{
	[Since(3, 2)]
	public class CTFramesetter : INativeObject, IDisposable
	{
		internal IntPtr handle;

		public IntPtr Handle => handle;

		internal CTFramesetter(IntPtr handle, bool owns)
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

		~CTFramesetter()
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
		private static extern IntPtr CTFramesetterCreateWithAttributedString(IntPtr @string);

		public CTFramesetter(NSAttributedString value)
		{
			if (value == null)
			{
				throw ConstructorError.ArgumentNull(this, "value");
			}
			handle = CTFramesetterCreateWithAttributedString(value.Handle);
			if (handle == IntPtr.Zero)
			{
				throw ConstructorError.Unknown(this);
			}
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFramesetterCreateFrame(IntPtr framesetter, NSRange stringRange, IntPtr path, IntPtr frameAttributes);

		public CTFrame GetFrame(NSRange stringRange, CGPath path, CTFrameAttributes frameAttributes)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			IntPtr value = CTFramesetterCreateFrame(handle, stringRange, path.Handle, frameAttributes?.Dictionary.Handle ?? IntPtr.Zero);
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new CTFrame(value, owns: true);
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern IntPtr CTFramesetterGetTypesetter(IntPtr framesetter);

		public CTTypesetter GetTypesetter()
		{
			IntPtr value = CTFramesetterGetTypesetter(handle);
			if (value == IntPtr.Zero)
			{
				return null;
			}
			return new CTTypesetter(value, owns: false);
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		private static extern CGSize CTFramesetterSuggestFrameSizeWithConstraints(IntPtr framesetter, NSRange stringRange, IntPtr frameAttributes, CGSize constraints, out NSRange fitRange);

		public CGSize SuggestFrameSize(NSRange stringRange, CTFrameAttributes frameAttributes, CGSize constraints, out NSRange fitRange)
		{
			return CTFramesetterSuggestFrameSizeWithConstraints(handle, stringRange, frameAttributes?.Dictionary.Handle ?? IntPtr.Zero, constraints, out fitRange);
		}
	}
}
