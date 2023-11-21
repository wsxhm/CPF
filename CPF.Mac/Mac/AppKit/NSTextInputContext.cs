using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.ComponentModel;

namespace CPF.Mac.AppKit
{
    [Register("NSTextInputContext", true)]
    public class NSTextInputContext : NSObject
    {

        private const string selAcceptsGlyphInfo = "acceptsGlyphInfo";

        private static readonly IntPtr selAcceptsGlyphInfoHandle = Selector.GetHandle("acceptsGlyphInfo");

        private const string selActivate = "activate";

        private static readonly IntPtr selActivateHandle = Selector.GetHandle("activate");

        private const string selAllowedInputSourceLocales = "allowedInputSourceLocales";

        private static readonly IntPtr selAllowedInputSourceLocalesHandle = Selector.GetHandle("allowedInputSourceLocales");

        private const string selClient = "client";

        private static readonly IntPtr selClientHandle = Selector.GetHandle("client");

        private const string selCurrentInputContext = "currentInputContext";

        private static readonly IntPtr selCurrentInputContextHandle = Selector.GetHandle("currentInputContext");

        private const string selDeactivate = "deactivate";

        private static readonly IntPtr selDeactivateHandle = Selector.GetHandle("deactivate");

        private const string selDiscardMarkedText = "discardMarkedText";

        private static readonly IntPtr selDiscardMarkedTextHandle = Selector.GetHandle("discardMarkedText");

        private const string selHandleEvent_ = "handleEvent:";

        private static readonly IntPtr selHandleEvent_Handle = Selector.GetHandle("handleEvent:");

        private const string selInitWithClient_ = "initWithClient:";

        private static readonly IntPtr selInitWithClient_Handle = Selector.GetHandle("initWithClient:");

        private const string selInvalidateCharacterCoordinates = "invalidateCharacterCoordinates";

        private static readonly IntPtr selInvalidateCharacterCoordinatesHandle = Selector.GetHandle("invalidateCharacterCoordinates");

        private const string selKeyboardInputSources = "keyboardInputSources";

        private static readonly IntPtr selKeyboardInputSourcesHandle = Selector.GetHandle("keyboardInputSources");

        private const string selLocalizedNameForInputSource_ = "localizedNameForInputSource:";

        private static readonly IntPtr selLocalizedNameForInputSource_Handle = Selector.GetHandle("localizedNameForInputSource:");

        private const string selSelectedKeyboardInputSource = "selectedKeyboardInputSource";

        private static readonly IntPtr selSelectedKeyboardInputSourceHandle = Selector.GetHandle("selectedKeyboardInputSource");

        private const string selSetAcceptsGlyphInfo_ = "setAcceptsGlyphInfo:";

        private static readonly IntPtr selSetAcceptsGlyphInfo_Handle = Selector.GetHandle("setAcceptsGlyphInfo:");

        private const string selSetAllowedInputSourceLocales_ = "setAllowedInputSourceLocales:";

        private static readonly IntPtr selSetAllowedInputSourceLocales_Handle = Selector.GetHandle("setAllowedInputSourceLocales:");

        private const string selSetSelectedKeyboardInputSource_ = "setSelectedKeyboardInputSource:";

        private static readonly IntPtr selSetSelectedKeyboardInputSource_Handle = Selector.GetHandle("setSelectedKeyboardInputSource:");

        private static readonly IntPtr class_ptr = Class.GetHandle("NSTextInputContext");

        private static NSString _KeyboardSelectionDidChangeNotification;

        public override IntPtr ClassHandle => class_ptr;

        public virtual bool AcceptsGlyphInfo
        {
            [Export("acceptsGlyphInfo")]
            get
            {
                NSApplication.EnsureUIThread();
                if (base.IsDirectBinding)
                {
                    return Messaging.bool_objc_msgSend(base.Handle, selAcceptsGlyphInfoHandle);
                }
                return Messaging.bool_objc_msgSendSuper(base.SuperHandle, selAcceptsGlyphInfoHandle);
            }
            [Export("setAcceptsGlyphInfo:")]
            set
            {
                NSApplication.EnsureUIThread();
                if (base.IsDirectBinding)
                {
                    Messaging.void_objc_msgSend_bool(base.Handle, selSetAcceptsGlyphInfo_Handle, value);
                }
                else
                {
                    Messaging.void_objc_msgSendSuper_bool(base.SuperHandle, selSetAcceptsGlyphInfo_Handle, value);
                }
            }
        }

        public virtual string[] AllowedInputSourceLocales
        {
            [Export("allowedInputSourceLocales", ArgumentSemantic.Copy)]
            get
            {
                NSApplication.EnsureUIThread();
                if (base.IsDirectBinding)
                {
                    return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSend(base.Handle, selAllowedInputSourceLocalesHandle));
                }
                return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selAllowedInputSourceLocalesHandle));
            }
            [Export("setAllowedInputSourceLocales:", ArgumentSemantic.Copy)]
            set
            {
                NSApplication.EnsureUIThread();
                NSArray nSArray = (value == null) ? null : NSArray.FromStrings(value);
                if (base.IsDirectBinding)
                {
                    Messaging.void_objc_msgSend_IntPtr(base.Handle, selSetAllowedInputSourceLocales_Handle, nSArray?.Handle ?? IntPtr.Zero);
                }
                else
                {
                    Messaging.void_objc_msgSendSuper_IntPtr(base.SuperHandle, selSetAllowedInputSourceLocales_Handle, nSArray?.Handle ?? IntPtr.Zero);
                }
                nSArray?.Dispose();
            }
        }

        //public virtual NSTextInputClient Client
        //{
        //	[Export("client")]
        //	get
        //	{
        //		NSApplication.EnsureUIThread();
        //		if (base.IsDirectBinding)
        //		{
        //			return Runtime.GetINativeObject<NSTextInputClient>(Messaging.IntPtr_objc_msgSend(base.Handle, selClientHandle), owns: false);
        //		}
        //		return Runtime.GetINativeObject<NSTextInputClient>(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selClientHandle), owns: false);
        //	}
        //}

        private static object __mt_CurrentInputContext_var_static;

        public static NSTextInputContext CurrentInputContext
        {
            [Export("currentInputContext")]
            get
            {
                NSApplication.EnsureUIThread();
                //return Runtime.GetNSObject<NSTextInputContext>(Messaging.IntPtr_objc_msgSend(class_ptr, selCurrentInputContextHandle));
                return (NSTextInputContext)(__mt_CurrentInputContext_var_static = (NSTextInputContext)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend(class_ptr, selCurrentInputContextHandle)));
            }
        }

        public virtual string[] KeyboardInputSources
        {
            [Export("keyboardInputSources")]
            get
            {
                NSApplication.EnsureUIThread();
                if (base.IsDirectBinding)
                {
                    return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSend(base.Handle, selKeyboardInputSourcesHandle));
                }
                return NSArray.StringArrayFromHandle(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selKeyboardInputSourcesHandle));
            }
        }

        public virtual string SelectedKeyboardInputSource
        {
            [Export("selectedKeyboardInputSource")]
            get
            {
                NSApplication.EnsureUIThread();
                if (base.IsDirectBinding)
                {
                    return NSString.FromHandle(Messaging.IntPtr_objc_msgSend(base.Handle, selSelectedKeyboardInputSourceHandle));
                }
                return NSString.FromHandle(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, selSelectedKeyboardInputSourceHandle));
            }
            [Export("setSelectedKeyboardInputSource:")]
            set
            {
                NSApplication.EnsureUIThread();
                IntPtr intPtr = NSString.CreateNative(value);
                if (base.IsDirectBinding)
                {
                    Messaging.void_objc_msgSend_IntPtr(base.Handle, selSetSelectedKeyboardInputSource_Handle, intPtr);
                }
                else
                {
                    Messaging.void_objc_msgSendSuper_IntPtr(base.SuperHandle, selSetSelectedKeyboardInputSource_Handle, intPtr);
                }
                NSString.ReleaseNative(intPtr);
            }
        }

        [Field("NSTextInputContextKeyboardSelectionDidChangeNotification", "AppKit")]
        [Advice("Use NSTextInputContext.Notifications.ObserveKeyboardSelectionDidChange helper method instead.")]
        public static NSString KeyboardSelectionDidChangeNotification
        {
            get
            {
                if (_KeyboardSelectionDidChangeNotification == null)
                {
                    _KeyboardSelectionDidChangeNotification = Dlfcn.GetStringConstant(Libraries.AppKit.Handle, "NSTextInputContextKeyboardSelectionDidChangeNotification");
                }
                return _KeyboardSelectionDidChangeNotification;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Export("init")]
        public NSTextInputContext()
            : base(NSObjectFlag.Empty)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected NSTextInputContext(NSObjectFlag t)
            : base(t)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public NSTextInputContext(IntPtr handle)
            : base(handle)
        {
        }


        [Export("activate")]
        public virtual void Activate()
        {
            NSApplication.EnsureUIThread();
            if (base.IsDirectBinding)
            {
                Messaging.void_objc_msgSend(base.Handle, selActivateHandle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper(base.SuperHandle, selActivateHandle);
            }
        }

        [Export("deactivate")]
        public virtual void Deactivate()
        {
            NSApplication.EnsureUIThread();
            if (base.IsDirectBinding)
            {
                Messaging.void_objc_msgSend(base.Handle, selDeactivateHandle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper(base.SuperHandle, selDeactivateHandle);
            }
        }

        [Export("discardMarkedText")]
        public virtual void DiscardMarkedText()
        {
            NSApplication.EnsureUIThread();
            if (base.IsDirectBinding)
            {
                Messaging.void_objc_msgSend(base.Handle, selDiscardMarkedTextHandle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper(base.SuperHandle, selDiscardMarkedTextHandle);
            }
        }

        [Export("handleEvent:")]
        public virtual bool HandleEvent(NSEvent theEvent)
        {
            NSApplication.EnsureUIThread();
            if (theEvent == null)
            {
                throw new ArgumentNullException("theEvent");
            }
            if (base.IsDirectBinding)
            {
                return Messaging.bool_objc_msgSend_IntPtr(base.Handle, selHandleEvent_Handle, theEvent.Handle);
            }
            return Messaging.bool_objc_msgSendSuper_IntPtr(base.SuperHandle, selHandleEvent_Handle, theEvent.Handle);
        }

        [Export("invalidateCharacterCoordinates")]
        public virtual void InvalidateCharacterCoordinates()
        {
            NSApplication.EnsureUIThread();
            if (base.IsDirectBinding)
            {
                Messaging.void_objc_msgSend(base.Handle, selInvalidateCharacterCoordinatesHandle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper(base.SuperHandle, selInvalidateCharacterCoordinatesHandle);
            }
        }

        [Export("localizedNameForInputSource:")]
        public static string LocalizedNameForInputSource(string inputSourceIdentifier)
        {
            NSApplication.EnsureUIThread();
            if (inputSourceIdentifier == null)
            {
                throw new ArgumentNullException("inputSourceIdentifier");
            }
            IntPtr intPtr = NSString.CreateNative(inputSourceIdentifier);
            string result = NSString.FromHandle(Messaging.IntPtr_objc_msgSend_IntPtr(class_ptr, selLocalizedNameForInputSource_Handle, intPtr));
            NSString.ReleaseNative(intPtr);
            return result;
        }
    }
}
