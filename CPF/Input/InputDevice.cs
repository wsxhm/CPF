using System;
using System.Collections.Generic;
using System.Text;
using CPF;

namespace CPF.Input
{
    /// <summary>
    ///     Provides the base class for all input devices.
    /// </summary>
    public abstract class InputDevice
    {
        /// <summary>
        ///  Constructs an instance of the InputDevice class.
        /// </summary>
        public InputDevice(InputManager inputManager)
        {
            // Only we can create these.
            // 
            this.inputManager = inputManager;
        }

        InputManager inputManager;

        public InputManager InputManager
        {
            get
            {
                return inputManager;
            }
        }


        ///// <summary>
        /////     Returns the PresentationSource that is reporting input for this device.
        ///// </summary>
        //public abstract PresentationSource ActiveSource { get; }

        //public abstract bool ProcessEvent(InputEventArgs args);
    }
}
