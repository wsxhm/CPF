using CPF.Drawing;
using CPF.Input;
using CPF.Mac.AppKit;
using CPF.Mac.CoreGraphics;
using CPF.Mac.Foundation;
using CPF.Platform;
using CPF.Mac.ObjCRuntime;
using System;
using System.Collections.Generic;
using System.Text;
using CPF.OpenGL;
using CPF.Mac.OpenGL;
using CPF.Mac.CoreVideo;

namespace CPF.Mac
{
    class OpenGLView : NSOpenGLView, NotMonoMac, NSTextInputClient
    {
        public OpenGLView(CGRect rect, WindowImpl window) : base(rect, new NSOpenGLPixelFormat(
            new object[]
            {
                NSOpenGLPixelFormatAttribute.Accelerated,
                NSOpenGLPixelFormatAttribute.MinimumPolicy,
                //NSOpenGLPixelFormatAttribute.ColorSize, 48,
                //NSOpenGLPixelFormatAttribute.AlphaSize, 16,
                //NSOpenGLPixelFormatAttribute.DepthSize, 24,
                //NSOpenGLPixelFormatAttribute.StencilSize, 8,
            }
            ))
        {
            WantsBestResolutionOpenGLSurface = true;
            this.window = window;
            RegisterForDraggedTypes(new string[] { NSPasteboard.NSPictType, NSPasteboard.NSStringType, NSPasteboard.NSFilenamesType, NSPasteboard.NSHtmlType });
            context = new OpenGL.CglContext(this);
            OpenGLContext.SurfaceOpaque = false;
        }
        Mac.OpenGL.CglContext context;

        public override void MouseDown(NSEvent theEvent)
        {
            InputContext.HandleEvent(theEvent);
        }

        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            //Keys k;
            //Key.s_KeyMap.TryGetValue(theEvent.KeyCode, out k);
            //if (k == Keys.Back || k == Keys.Tab || k == Keys.Enter)
            //{
            //    return;
            //}
            var r = InputContext.HandleEvent(theEvent);
            //Debug.WriteLine(r);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            InputContext.HandleEvent(theEvent);
        }
        public override void MouseUp(NSEvent theEvent)
        {
            InputContext.HandleEvent(theEvent);
        }
        WindowImpl window;
        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            window.UpdateCursor();
            if (window.Resized != null)
            {
                window.Resized(new Size((float)newSize.Width, (float)newSize.Height) / window.LayoutScaling);
                //Console.WriteLine(newSize);
            }
        }


        //static CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
        //Size oldSize;
        //Bitmap bitmap;
        public override void DrawRect(CGRect dirtyRect)
        {
            if (window.root.IsDisposed)
            {
                return;
            }
            var scaling = (float)window.BackingScaleFactor;
            window.root.LayoutManager.ExecuteLayoutPass();
            var size = new Size((float)Bounds.Width, (float)Bounds.Height) * scaling;

            context.MakeCurrent();

            //OpenGL.Cgl.glClearColor(0, 0, 0, 0);
            //OpenGL.Cgl.glClear(0x00004400);
            if (window.root.LayoutManager.VisibleUIElements != null)
            {
                context.GetFramebufferInfo(out var fb, out var sam, out var sten);
                var rt = new OpenGlRenderTarget(context, (int)size.Width, (int)size.Height, fb, sam, sten);
                using (DrawingContext dc = DrawingContext.FromRenderTarget(rt))
                {
                    //var t = dc.Transform;
                    //t.ScalePrepend(1 / scaling, 1 / scaling);
                    //dc.Transform = t;
                    window.root.RenderView(dc, new Rect((float)dirtyRect.X * scaling, ((float)Bounds.Height - (float)dirtyRect.Height - (float)dirtyRect.Y) * scaling, (float)dirtyRect.Width * scaling, (float)dirtyRect.Height * scaling));
                }
            }

            ////context.Enable(GlConsts.GL_MULTISAMPLE);
            //context.Color4f(1.0f, 0.85f, 0.35f, 1);
            //context.Begin(GlConsts.GL_TRIANGLES);

            //context.Vertex3f(0.0f, 0.6f, 0.0f);
            //context.Vertex3f(-0.2f, -0.3f, 0.0f);
            //context.Vertex3f(0.2f, -0.3f, 0.0f);

            //context.End();

            context.SwapBuffers();
        }
        public override bool CanBecomeKeyView
        {
            get
            {
                return true;
            }
        }


        public override bool AcceptsFirstResponder()
        {
            return true;
        }

        public override bool BecomeFirstResponder()
        {
            return true;
        }
        public override bool ResignFirstResponder()
        {
            return true;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (bitmap != null)
            //{
            //    bitmap.Dispose();
            //    bitmap = null;
            //}
        }
        public static NSDragOperation ConvertDropEffect(DragDropEffects operation)
        {
            NSDragOperation result = NSDragOperation.None;
            if (operation.HasFlag(DragDropEffects.Copy))
                result |= NSDragOperation.Copy;
            if (operation.HasFlag(DragDropEffects.Move))
                result |= NSDragOperation.Move;
            if (operation.HasFlag(DragDropEffects.Link))
                result |= NSDragOperation.Link;
            return result;
        }

        public static DragDropEffects ConvertDropEffect(NSDragOperation effect)
        {
            DragDropEffects result = DragDropEffects.None;
            if (effect.HasFlag(NSDragOperation.Copy))
                result |= DragDropEffects.Copy;
            if (effect.HasFlag(NSDragOperation.Move))
                result |= DragDropEffects.Move;
            if (effect.HasFlag(NSDragOperation.Link))
                result |= DragDropEffects.Link;
            return result;
        }
        private Point GetDragLocation(CGPoint dragPoint)
        {
            return new Point((float)dragPoint.X, (float)(Frame.Height - dragPoint.Y));
        }
        IDataObject _currentDrag;
        public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
        {
            _currentDrag = new DataObject(sender.DraggingPasteboard);
            //_currentDrag.Get(DataFormat.FileNames);
            var effect = window.root.InputManager.DragDropDevice.DragEnter(new DragEventArgs(_currentDrag, GetDragLocation(sender.DraggingLocation), window.root) { DragEffects = ConvertDropEffect(sender.DraggingSourceOperationMask) }, window.root.LayoutManager.VisibleUIElements);
            return ConvertDropEffect(effect);
        }
        public override NSDragOperation DraggingUpdated(NSDraggingInfo sender)
        {
            _currentDrag = new DataObject(sender.DraggingPasteboard);
            var effect = window.root.InputManager.DragDropDevice.DragOver(new DragEventArgs(_currentDrag, GetDragLocation(sender.DraggingLocation), window.root) { DragEffects = ConvertDropEffect(sender.DraggingSourceOperationMask) }, window.root.LayoutManager.VisibleUIElements);
            return ConvertDropEffect(effect);
        }
        public override void DraggingExited(NSDraggingInfo sender)
        {
            window.root.InputManager.DragDropDevice.DragLeave(window.root.LayoutManager.VisibleUIElements);
            _currentDrag = null;
        }

        public override bool PerformDragOperation(NSDraggingInfo sender)
        {
            //Debug.WriteLine("PerformDragOperation");
            window.root.InputManager.DragDropDevice.Drop(new DragEventArgs(_currentDrag, GetDragLocation(sender.DraggingLocation), window.root) { DragEffects = ConvertDropEffect(sender.DraggingSourceOperationMask) }, window.root.LayoutManager.VisibleUIElements);
            return true;
            //return base.PerformDragOperation(sender);
        }

        [Export("doCommandBySelector:")]
        public virtual void DoCommandBySelector(Selector selector)
        {
            //Debug.WriteLine(selector.Name);
            if (selector.Name == "deleteBackward:")
            {
                window.SetModifier();
                window.root.InputManager.KeyboardDevice.ProcessEvent(new KeyEventArgs(window.root, Keys.Back, Key.kVK_Delete, window.root.InputManager.KeyboardDevice.Modifiers, window.root.InputManager.KeyboardDevice), KeyEventType.KeyDown);
            }
        }


        [Export("baselineDeltaForCharacterAtIndex:")]
        public virtual float GetBaselineDelta(uint charIndex)
        {
            throw new You_Should_Not_Call_base_In_This_Method();
        }

        [Export("characterIndexForPoint:")]
        public virtual uint GetCharacterIndex(CGPoint point)
        {
            //throw new You_Should_Not_Call_base_In_This_Method();
            return 0;
        }

        [Export("firstRectForCharacterRange:actualRange:")]
        public virtual CGRect GetFirstRect(NSRange characterRange, out NSRange actualRange)
        {
            actualRange = new NSRange(0, 0);
            return new CGRect(window.Frame.X + window.imePoint.X, window.Frame.Y + (window.Frame.Height - window.imePoint.Y - 15), 1, 10);
        }

        [Export("fractionOfDistanceThroughGlyphForPoint:")]
        public virtual float GetFractionOfDistanceThroughGlyph(CGPoint point)
        {
            return 0;
        }

        [Export("insertText:replacementRange:")]
        public virtual void InsertText(NSObject text, NSRange replacementRange)
        {
            //throw new You_Should_Not_Call_base_In_This_Method();
            //Console.WriteLine("输入法输出：" + text);
            window.root.InputManager.KeyboardDevice.ProcessEvent(new TextInputEventArgs(window.root, window.root.InputManager.KeyboardDevice, text.ToString()), KeyEventType.TextInput);
            InputContext.InvalidateCharacterCoordinates();
        }

        [Export("setMarkedText:selectedRange:replacementRange:")]
        public virtual void SetMarkedText(NSObject text, NSRange selectedRange, NSRange replacementRange)
        {
            InputContext.InvalidateCharacterCoordinates();
        }

        [Export("unmarkText")]
        public virtual void UnmarkText()
        {
            InputContext.DiscardMarkedText();
        }

        [Export("attributedSubstringForProposedRange:actualRange:")]
        public NSAttributedString GetAttributedSubstring(NSRange proposedRange, out NSRange actualRange)
        {
            actualRange = new NSRange(0, 0);
            return null;
        }

        public virtual bool HasMarkedText
        {
            [Export("hasMarkedText")]
            get
            {
                return false;
            }
        }

        public virtual NSRange MarkedRange
        {
            [Export("markedRange")]
            get
            {
                return new NSRange(0, 0);
            }
        }

        public virtual NSRange SelectedRange
        {
            [Export("selectedRange")]
            get
            {
                return new NSRange(0, 0);
            }
        }

        public virtual NSString[] ValidAttributesForMarkedText
        {
            [Export("validAttributesForMarkedText")]
            get
            {
                return new NSString[] { NSAttributedString.MarkedClauseSegmentAttributeName, NSAttributedString.GlyphInfo, };
            }
        }

        public virtual NSWindowLevel WindowLevel
        {
            [Export("windowLevel")]
            get
            {
                return window.Level;
            }
        }

        //[Export("attributedString")]
        //public NSAttributedString AttributedString => null;
    }

    //public class GLView : NSOpenGLView
    //{
    //    public GLView(CGRect rect, NSOpenGLPixelFormat format) : base(rect, format)
    //    {
    //        WantsBestResolutionOpenGLSurface = true;
    //        context = new OpenGL.CglContext(this);
    //    }
    //    Mac.OpenGL.CglContext context;

    //    public override void DrawRect(CGRect dirtyRect)
    //    {
    //        OpenGLContext.MakeCurrentContext();

    //        context.Viewport(0, 0, 100, 100);
    //        context.ClearColor(0, 0, 0, 0);
    //        context.Clear(GlConsts.GL_COLOR_BUFFER_BIT);

    //        context.Color4f(1.0f, 0.85f, 0.35f, 1);
    //        context.Begin(GlConsts.GL_TRIANGLES);

    //        context.Vertex3f(0.0f, 0.6f, 0.0f);
    //        context.Vertex3f(-0.2f, -0.3f, 0.0f);
    //        context.Vertex3f(0.2f, -0.3f, 0.0f);

    //        context.End();

    //        context.Flush();
    //    }
    //}

    //public partial class MyOpenGLView : AppKit.NSView
    //{

    //    NSOpenGLContext openGLContext;
    //    NSOpenGLPixelFormat pixelFormat;

    //    MainWindowController controller;

    //    CVDisplayLink displayLink;

    //    NSObject notificationProxy;

    //    [Export("initWithFrame:")]
    //    public MyOpenGLView(CGRect frame) : this(frame, null)
    //    {
    //    }

    //    public MyOpenGLView(CGRect frame, NSOpenGLContext context) : base(frame)
    //    {
    //        var attribs = new object[] {
    //            NSOpenGLPixelFormatAttribute.Accelerated,
    //            NSOpenGLPixelFormatAttribute.NoRecovery,
    //            NSOpenGLPixelFormatAttribute.DoubleBuffer,
    //            NSOpenGLPixelFormatAttribute.ColorSize, 24,
    //            NSOpenGLPixelFormatAttribute.DepthSize, 16 };

    //        pixelFormat = new NSOpenGLPixelFormat(attribs);

    //        if (pixelFormat == null)
    //            Console.WriteLine("No OpenGL pixel format");

    //        // NSOpenGLView does not handle context sharing, so we draw to a custom NSView instead
    //        openGLContext = new NSOpenGLContext(pixelFormat, context);

    //        openGLContext.MakeCurrentContext();

    //        // Synchronize buffer swaps with vertical refresh rate
    //        openGLContext.SwapInterval = true;

    //        // Initialize our newly created view.
    //        InitGL();

    //        SetupDisplayLink();

    //        // Look for changes in view size
    //        // Note, -reshape will not be called automatically on size changes because NSView does not export it to override 
    //        notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver(NSView.GlobalFrameChangedNotification, HandleReshape);
    //    }

    //    public override void DrawRect(CGRect dirtyRect)
    //    {
    //        // Ignore if the display link is still running
    //        if (!displayLink.IsRunning && controller != null)
    //            DrawView();
    //    }

    //    public override bool AcceptsFirstResponder()
    //    {
    //        // We want this view to be able to receive key events
    //        return true;
    //    }

    //    public override void LockFocus()
    //    {
    //        base.LockFocus();
    //        if (openGLContext.View != this)
    //            openGLContext.View = this;
    //    }

    //    public override void KeyDown(NSEvent theEvent)
    //    {
    //        controller.KeyDown(theEvent);
    //    }

    //    public override void MouseDown(NSEvent theEvent)
    //    {
    //        controller.MouseDown(theEvent);
    //    }

    //    // All Setup For OpenGL Goes Here
    //    public bool InitGL()
    //    {
    //        // Enables Smooth Shading  
    //        GL.ShadeModel(ShadingModel.Smooth);
    //        // Set background color to black     
    //        GL.ClearColor(Color.Black);

    //        // Setup Depth Testing

    //        // Depth Buffer setup
    //        GL.ClearDepth(1.0);
    //        // Enables Depth testing
    //        GL.Enable(EnableCap.DepthTest);
    //        // The type of depth testing to do
    //        GL.DepthFunc(DepthFunction.Lequal);

    //        // Really Nice Perspective Calculations
    //        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

    //        return true;
    //    }

    //    private void DrawView()
    //    {
    //        // This method will be called on both the main thread (through -drawRect:) and a secondary thread (through the display link rendering loop)
    //        // Also, when resizing the view, -reshape is called on the main thread, but we may be drawing on a secondary thread
    //        // Add a mutex around to avoid the threads accessing the context simultaneously 
    //        openGLContext.CGLContext.Lock();

    //        // Make sure we draw to the right context
    //        openGLContext.MakeCurrentContext();

    //        // Delegate to the scene object for rendering
    //        controller.Scene.DrawGLScene();

    //        openGLContext.FlushBuffer();

    //        openGLContext.CGLContext.Unlock();
    //    }


    //    private void SetupDisplayLink()
    //    {
    //        // Create a display link capable of being used with all active displays
    //        displayLink = new CVDisplayLink();

    //        // Set the renderer output callback function
    //        displayLink.SetOutputCallback(MyDisplayLinkOutputCallback);

    //        // Set the display link for the current renderer
    //        CGLContext cglContext = openGLContext.CGLContext;
    //        CGLPixelFormat cglPixelFormat = PixelFormat.CGLPixelFormat;
    //        displayLink.SetCurrentDisplay(cglContext, cglPixelFormat);

    //    }

    //    public CVReturn MyDisplayLinkOutputCallback(CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut)
    //    {
    //        CVReturn result = GetFrameForTime(inOutputTime);

    //        return result;
    //    }


    //    private CVReturn GetFrameForTime(CVTimeStamp outputTime)
    //    {
    //        // There is no autorelease pool when this method is called because it will be called from a background thread
    //        // It's important to create one or you will leak objects
    //        using (NSAutoreleasePool pool = new NSAutoreleasePool())
    //        {

    //            // Update the animation
    //            BeginInvokeOnMainThread(DrawView);
    //        }

    //        return CVReturn.Success;

    //    }

    //    public NSOpenGLContext OpenGLContext
    //    {
    //        get { return openGLContext; }
    //    }

    //    public NSOpenGLPixelFormat PixelFormat
    //    {
    //        get { return pixelFormat; }
    //    }

    //    public MainWindowController MainController
    //    {
    //        set { controller = value; }
    //    }

    //    public void UpdateView()
    //    {
    //        // This method will be called on the main thread when resizing, but we may be drawing on a secondary thread through the display link
    //        // Add a mutex around to avoid the threads accessing the context simultaneously
    //        openGLContext.CGLContext.Lock();

    //        // Delegate to the scene object to update for a change in the view size
    //        controller.Scene.ResizeGLScene(Bounds);
    //        openGLContext.Update();

    //        openGLContext.CGLContext.Unlock();
    //    }

    //    private void HandleReshape(NSNotification note)
    //    {
    //        UpdateView();
    //    }

    //    public void StartAnimation()
    //    {
    //        if (displayLink != null && !displayLink.IsRunning)
    //            displayLink.Start();
    //    }

    //    public void StopAnimation()
    //    {
    //        if (displayLink != null && displayLink.IsRunning)
    //            displayLink.Stop();
    //    }

    //    // Clean up the notifications
    //    public void DeAllocate()
    //    {
    //        displayLink.Stop();
    //        displayLink.SetOutputCallback(null);

    //        NSNotificationCenter.DefaultCenter.RemoveObserver(notificationProxy);
    //    }

    //    [Export("toggleFullScreen:")]
    //    public void toggleFullScreen(NSObject sender)
    //    {
    //        controller.toggleFullScreen(sender);
    //    }
    //}
}
