using CPF.Mac.AppKit;
using CPF.Mac.Foundation;
using CPF.Mac.ObjCRuntime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Mac
{
    [Register("NSApplication")]
    public class CPFNSApplication : NSApplication, CefAppProtocol, NotMonoMac
    {
        public CPFNSApplication(NSObjectFlag flag) : base(flag)
        {

        }
        [Export("initWithCoder:")]
        public CPFNSApplication(NSCoder coder) : base(coder)
        {

        }
        public CPFNSApplication(IntPtr handle) : base(handle)
        {
            //Console.WriteLine("CPFNSApplication");
        }

        bool handlingSendEvent;
        /// <summary>
        /// Cef需要用的属性
        /// </summary>
        [Export("isHandlingSendEvent")]
        public virtual bool IsHandlingSendEvent
        {
            get
            {
                //Console.WriteLine("isHandlingSendEvent");
                return handlingSendEvent;
            }
        }

        /// <summary>
        /// Cef需要用的属性
        /// </summary>
        [Export("setHandlingSendEvent")]
        public void SetHandlingSendEvent(bool handlingSendEvent)
        {
            this.handlingSendEvent = handlingSendEvent;
        }

        //public bool Test()
        //{
        //    var isHn = Selector.GetHandle("isHandlingSendEvent");
        //    Console.WriteLine("isHandlingSendEvent");
        //    if (IsDirectBinding)
        //    {
        //        return Messaging.bool_objc_msgSend(base.Handle, isHn);
        //    }
        //    return Messaging.bool_objc_msgSendSuper(base.SuperHandle, isHn);
        //}
    }

    [Protocol]
    public interface CefAppProtocol : CrAppControlProtocol
    {

    }

    [Protocol]
    public interface CrAppProtocol
    {
        [Export("isHandlingSendEvent")]
        bool IsHandlingSendEvent { get; }
    }
    [Protocol]
    public interface CrAppControlProtocol : CrAppProtocol
    {
        [Export("setHandlingSendEvent")]
        void SetHandlingSendEvent(bool handlingSendEvent);
    }
}
