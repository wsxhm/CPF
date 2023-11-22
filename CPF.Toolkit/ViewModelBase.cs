using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit
{
    public class ViewModelBase : ObservableObject
    {
        public IDialogService Dialog { get; set; }
    }
}
