using CPF.Controls;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit
{
    public class ViewModelBase : ObservableObject, IClosable, IDialog
    {
        event EventHandler<bool?> _close;

        public IDialogService Dialog { get; set; }

        event EventHandler<bool?> IClosable.Closable { add => this._close += value; remove => this._close -= value; }

        protected void Close(bool? dialogResult = null)
        {
            if (this._close == null)
            {
                throw new ArgumentNullException();
            }
            this._close.Invoke(this, dialogResult);
        }

        protected virtual void OnClose(ClosingEventArgs e) { }

        void IClosable.OnClosable(object sender, ClosingEventArgs e) => this.OnClose(e);
    }
}
