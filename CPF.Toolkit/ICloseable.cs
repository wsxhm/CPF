using CPF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Toolkit
{
    public interface ICloseable
    {
        event EventHandler<ClosingEventArgs> Closable;
        void OnClosable(ClosingEventArgs e);
    }
}
