using CPF.Controls;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Toolkit
{
    public class ViewModelBase : ObservableObject, IClosable, IDialog, ILoading
    {
        event EventHandler<object> _close;
        event Func<string, Task, Task<object>> _showLoadingFunc;
        event Func<string, Task, Task> _showLading;
        event EventHandler<object> IClosable.Closable { add => this._close += value; remove => this._close -= value; }
        event Func<string, Task, Task<object>> ILoading.ShowLoadingFunc { add => this._showLoadingFunc += value; remove => this._showLoadingFunc -= value; }
        event Func<string, Task, Task> ILoading.ShowLoading { add => this._showLading += value; remove => this._showLading -= value; }

        void IClosable.OnClosable(object sender, ClosingEventArgs e) => this.OnClose(e);

        public IDialogService Dialog { get; set; }

        protected void Close(object dialogResult = null)
        {
            if (this._close == null) throw new ArgumentNullException();
            this._close.Invoke(this, dialogResult);
        }

        protected virtual void OnClose(ClosingEventArgs e) { }

        protected async Task ShowLoading(Task task)
        {
            if (this._showLading == null) throw new ArgumentNullException();
            await this._showLading.Invoke("加载中……", task);
        }

        protected async Task ShowLoading(Func<Task> task)
        {
            if (this._showLoadingFunc == null) throw new ArgumentNullException();
            await this._showLading.Invoke("加载中……", task.Invoke());
        }

        protected async Task<T> ShowLoading<T>(Func<Task<T>> task)
        {
            if (this._showLoadingFunc == null) throw new ArgumentNullException();
            var result = await this._showLoadingFunc.Invoke("加载中……", task.Invoke());
            return (T)result;
        }
    }
}
