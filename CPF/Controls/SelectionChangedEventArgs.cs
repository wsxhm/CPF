using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public class SelectionChangedEventArgs : EventArgs
    {
        public IList AddedItems { get; internal set; }

        public IList RemovedItems { get; internal set; }
    }
}
