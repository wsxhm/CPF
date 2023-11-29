﻿using CPF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CPF.Toolkit.Dialogs
{
    internal interface IClosable
    {
        event EventHandler<ClosingEventArgs> Closable;
        void OnClosable(object sender, ClosingEventArgs e);
    }
}
