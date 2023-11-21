using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using CPF.Mac.OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CPF.Mac.CoreVideo
{
	public class CVDisplayLink : INativeObject, IDisposable
	{
		// Token: 0x0601036C RID: 66412 RVA: 0x00436404 File Offset: 0x00434604
		public CVDisplayLink(IntPtr handle)
		{
			bool flag = handle == IntPtr.Zero;
			if (flag)
			{
				throw new Exception("Invalid parameters to display link creation");
			}
			CVDisplayLink.CVDisplayLinkRetain(handle);
			this.handle = handle;
		}

		// Token: 0x0601036D RID: 66413 RVA: 0x00436444 File Offset: 0x00434644
		[Preserve(Conditional = true)]
		internal CVDisplayLink(IntPtr handle, bool owns)
		{
			bool flag = !owns;
			if (flag)
			{
				CVDisplayLink.CVDisplayLinkRetain(handle);
			}
			this.handle = handle;
		}

		// Token: 0x0601036E RID: 66414 RVA: 0x00436470 File Offset: 0x00434670
		~CVDisplayLink()
		{
			this.Dispose(false);
		}

		// Token: 0x0601036F RID: 66415 RVA: 0x004364A4 File Offset: 0x004346A4
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x170051ED RID: 20973
		// (get) Token: 0x06010370 RID: 66416 RVA: 0x004364B8 File Offset: 0x004346B8
		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		// Token: 0x06010371 RID: 66417
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern void CVDisplayLinkRetain(IntPtr handle);

		// Token: 0x06010372 RID: 66418
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern void CVDisplayLinkRelease(IntPtr handle);

		// Token: 0x06010373 RID: 66419 RVA: 0x004364D0 File Offset: 0x004346D0
		protected virtual void Dispose(bool disposing)
		{
			bool isAllocated = this.callbackHandle.IsAllocated;
			if (isAllocated)
			{
				this.callbackHandle.Free();
			}
			bool flag = this.handle != IntPtr.Zero;
			if (flag)
			{
				CVDisplayLink.CVDisplayLinkRelease(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		// Token: 0x06010374 RID: 66420
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkCreateWithActiveCGDisplays(out IntPtr displayLinkOut);

		// Token: 0x06010375 RID: 66421 RVA: 0x00436528 File Offset: 0x00434728
		public CVDisplayLink()
		{
			IntPtr intPtr;
			CVReturn cvreturn = CVDisplayLink.CVDisplayLinkCreateWithActiveCGDisplays(out intPtr);
			bool flag = cvreturn > CVReturn.Success;
			if (flag)
			{
				throw new Exception("CVDisplayLink returned: " + cvreturn.ToString());
			}
			this.handle = intPtr;
		}

		// Token: 0x06010376 RID: 66422
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkSetCurrentCGDisplay(IntPtr displayLink, int displayId);

		// Token: 0x06010377 RID: 66423 RVA: 0x00436574 File Offset: 0x00434774
		public CVReturn SetCurrentDisplay(int displayId)
		{
			return CVDisplayLink.CVDisplayLinkSetCurrentCGDisplay(this.handle, displayId);
		}

		// Token: 0x06010378 RID: 66424
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext(IntPtr displayLink, IntPtr cglContext, IntPtr cglPixelFormat);

		// Token: 0x06010379 RID: 66425 RVA: 0x00436594 File Offset: 0x00434794
		public CVReturn SetCurrentDisplay(CGLContext cglContext, CGLPixelFormat cglPixelFormat)
		{
			return CVDisplayLink.CVDisplayLinkSetCurrentCGDisplayFromOpenGLContext(this.handle, cglContext.Handle, cglPixelFormat.Handle);
		}

		// Token: 0x0601037A RID: 66426
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern int CVDisplayLinkGetCurrentCGDisplay(IntPtr displayLink);

		// Token: 0x0601037B RID: 66427 RVA: 0x004365C0 File Offset: 0x004347C0
		public int GetCurrentDisplay()
		{
			return CVDisplayLink.CVDisplayLinkGetCurrentCGDisplay(this.handle);
		}

		// Token: 0x0601037C RID: 66428
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkStart(IntPtr displayLink);

		// Token: 0x0601037D RID: 66429 RVA: 0x004365E0 File Offset: 0x004347E0
		public CVReturn Start()
		{
			return CVDisplayLink.CVDisplayLinkStart(this.handle);
		}

		// Token: 0x0601037E RID: 66430
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkStop(IntPtr displayLink);

		// Token: 0x0601037F RID: 66431 RVA: 0x00436600 File Offset: 0x00434800
		public CVReturn Stop()
		{
			return CVDisplayLink.CVDisplayLinkStop(this.handle);
		}

		// Token: 0x06010380 RID: 66432
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVTime CVDisplayLinkGetNominalOutputVideoRefreshPeriod(IntPtr displayLink);

		// Token: 0x170051EE RID: 20974
		// (get) Token: 0x06010381 RID: 66433 RVA: 0x00436620 File Offset: 0x00434820
		public CVTime NominalOutputVideoRefreshPeriod
		{
			get
			{
				return CVDisplayLink.CVDisplayLinkGetNominalOutputVideoRefreshPeriod(this.handle);
			}
		}

		// Token: 0x06010382 RID: 66434
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVTime CVDisplayLinkGetOutputVideoLatency(IntPtr displayLink);

		// Token: 0x170051EF RID: 20975
		// (get) Token: 0x06010383 RID: 66435 RVA: 0x00436640 File Offset: 0x00434840
		public CVTime OutputVideoLatency
		{
			get
			{
				return CVDisplayLink.CVDisplayLinkGetOutputVideoLatency(this.handle);
			}
		}

		// Token: 0x06010384 RID: 66436
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern double CVDisplayLinkGetActualOutputVideoRefreshPeriod(IntPtr displayLink);

		// Token: 0x170051F0 RID: 20976
		// (get) Token: 0x06010385 RID: 66437 RVA: 0x00436660 File Offset: 0x00434860
		public double ActualOutputVideoRefreshPeriod
		{
			get
			{
				return CVDisplayLink.CVDisplayLinkGetActualOutputVideoRefreshPeriod(this.handle);
			}
		}

		// Token: 0x06010386 RID: 66438
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern bool CVDisplayLinkIsRunning(IntPtr displayLink);

		// Token: 0x170051F1 RID: 20977
		// (get) Token: 0x06010387 RID: 66439 RVA: 0x00436680 File Offset: 0x00434880
		public bool IsRunning
		{
			get
			{
				return CVDisplayLink.CVDisplayLinkIsRunning(this.handle);
			}
		}

		// Token: 0x06010388 RID: 66440
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkGetCurrentTime(IntPtr displayLink, out CVTimeStamp outTime);

		// Token: 0x06010389 RID: 66441 RVA: 0x004366A0 File Offset: 0x004348A0
		public CVReturn GetCurrentTime(out CVTimeStamp outTime)
		{
			return CVDisplayLink.CVDisplayLinkGetCurrentTime(this.Handle, out outTime);
		}

		// Token: 0x0601038A RID: 66442 RVA: 0x004366C0 File Offset: 0x004348C0
		private static CVReturn OutputCallback(IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr displayLinkContext)
		{
			CVDisplayLink.DisplayLinkOutputCallback displayLinkOutputCallback = (CVDisplayLink.DisplayLinkOutputCallback)GCHandle.FromIntPtr(displayLinkContext).Target;
			CVDisplayLink displayLink2 = new CVDisplayLink(displayLink, false);
			return displayLinkOutputCallback(displayLink2, ref inNow, ref inOutputTime, flagsIn, ref flagsOut);
		}

		// Token: 0x0601038B RID: 66443
		[DllImport("/System/Library/Frameworks/CoreVideo.framework/CoreVideo")]
		private static extern CVReturn CVDisplayLinkSetOutputCallback(IntPtr displayLink, CVDisplayLink.CVDisplayLinkOutputCallback function, IntPtr userInfo);

		// Token: 0x0601038C RID: 66444 RVA: 0x004366FC File Offset: 0x004348FC
		public CVReturn SetOutputCallback(CVDisplayLink.DisplayLinkOutputCallback callback)
		{
			this.callbackHandle = GCHandle.Alloc(callback);
			return CVDisplayLink.CVDisplayLinkSetOutputCallback(this.Handle, CVDisplayLink.static_OutputCallback, GCHandle.ToIntPtr(this.callbackHandle));
		}

		// Token: 0x0601038D RID: 66445 RVA: 0x00436737 File Offset: 0x00434937
		// Note: this type is marked as 'beforefieldinit'.
		static CVDisplayLink()
		{
		}

		// Token: 0x04011701 RID: 71425
		private IntPtr handle;

		// Token: 0x04011702 RID: 71426
		private GCHandle callbackHandle;

		// Token: 0x04011703 RID: 71427
		private static CVDisplayLink.CVDisplayLinkOutputCallback static_OutputCallback = new CVDisplayLink.CVDisplayLinkOutputCallback(CVDisplayLink.OutputCallback);

		// Token: 0x0200246C RID: 9324
		// (Invoke) Token: 0x060162E0 RID: 90848
		public delegate CVReturn DisplayLinkOutputCallback(CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut);

		// Token: 0x0200246D RID: 9325
		// (Invoke) Token: 0x060162E4 RID: 90852
		private delegate CVReturn CVDisplayLinkOutputCallback(IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr displayLinkContext);
	}
}
