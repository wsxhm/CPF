using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CPF.Windows
{
    public struct Message
    {
        IntPtr hWnd;
        int msg;
        IntPtr wparam;
        IntPtr lparam;
        IntPtr result;

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.HWnd"]/*' />
        /// <devdoc>
        ///    <para>Specifies the window handle of the message.</para>
        /// </devdoc>

        public IntPtr HWnd
        {
            get { return hWnd; }
            set { hWnd = value; }
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.Msg"]/*' />
        /// <devdoc>
        ///    <para>Specifies the ID number for the message.</para>
        /// </devdoc>
        public int Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.WParam"]/*' />
        /// <devdoc>
        /// <para>Specifies the <see cref='System.Windows.Forms.Message.wparam'/> of the message.</para>
        /// </devdoc>
        public IntPtr WParam
        {
            get { return wparam; }
            set { wparam = value; }
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.LParam"]/*' />
        /// <devdoc>
        /// <para>Specifies the <see cref='System.Windows.Forms.Message.lparam'/> of the message.</para>
        /// </devdoc>
        public IntPtr LParam
        {
            get { return lparam; }
            set { lparam = value; }
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.Result"]/*' />
        /// <devdoc>
        ///    <para>Specifies the return value of the message.</para>
        /// </devdoc>
        public IntPtr Result
        {
            get { return result; }
            set { result = value; }
        }

        ///// <include file='doc\Message.uex' path='docs/doc[@for="Message.GetLParam"]/*' />
        ///// <devdoc>
        ///// <para>Gets the <see cref='System.Windows.Forms.Message.lparam'/> value, and converts the value to an object.</para>
        ///// </devdoc>
        //public object GetLParam(Type cls)
        //{
        //    return Marshal.PtrToStructure(lparam, cls);
        //}

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.Create"]/*' />
        /// <devdoc>
        /// <para>Creates a new <see cref='System.Windows.Forms.Message'/> object.</para>
        /// </devdoc>
        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            Message m = new Message();
            m.hWnd = hWnd;
            m.msg = msg;
            m.wparam = wparam;
            m.lparam = lparam;
            m.result = IntPtr.Zero;

            return m;
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.Equals"]/*' />
        public override bool Equals(object o)
        {
            if (!(o is Message))
            {
                return false;
            }

            Message m = (Message)o;
            return hWnd == m.hWnd &&
                   msg == m.msg &&
                   wparam == m.wparam &&
                   lparam == m.lparam &&
                   result == m.result;
        }

        public static bool operator !=(Message a, Message b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Message a, Message b)
        {
            return a.Equals(b);
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.GetHashCode"]/*' />
        public override int GetHashCode()
        {
            return (int)hWnd << 4 | msg;
        }

        /// <include file='doc\Message.uex' path='docs/doc[@for="Message.ToString"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override string ToString()
        {
            return "msg=0x" + Convert.ToString(msg, 16)
             + " hwnd=0x" + Convert.ToString((long)hWnd, 16)
             + " wparam=0x" + Convert.ToString((long)wparam, 16)
             + " lparam=0x" + Convert.ToString((long)lparam, 16)
             + " result=0x" + Convert.ToString((long)result, 16);
        }
    }
}
