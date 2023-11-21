using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Input
{
    public class InputEventArgs : RoutedEventArgs
    {
        public InputEventArgs(InputDevice inputDevice)
        {
            /* inputDevice parameter being null is valid*/
            /* timestamp parameter is valuetype, need not be checked */
            _inputDevice = inputDevice;
        }

        private InputDevice _inputDevice;
        /// <summary>
        ///     Read-only access to the input device that initiated this
        ///     event.
        /// </summary>
        public InputDevice Device
        {
            get { return _inputDevice; }
            internal set { _inputDevice = value; }
        }
    }
}
