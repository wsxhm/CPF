using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CPF.Toolkit
{
    public class ViewModelBase : INotifyPropertyChanged, ILoaded, IDisposable, ICloseable, IDialog
    {
        WeakEventHandlerList events = new WeakEventHandlerList();

        IDialogService IDialog.Dialog { get; set; }

        event EventHandler<ClosingEventArgs> ICloseable.Closable { add => AddHandler(value); remove => RemoveHandler(value); }

        public virtual void Dispose() { }

        void ICloseable.OnClosable(ClosingEventArgs e) => this.OnClosing(e);

        void ILoaded.OnLoaded() => this.OnLoaded();

        protected IDialogService Dialog => (this as IDialog).Dialog;

        protected virtual void OnLoaded() { }
        protected void Close() => this.RaiseEvent(new ClosingEventArgs(), nameof(ICloseable.Closable));

        protected virtual void OnClosing(ClosingEventArgs e) { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void AddHandler(Delegate handler, [CallerMemberName] string eventName = null) => events.AddHandler(handler, eventName);
        protected void RemoveHandler(Delegate handler, [CallerMemberName] string eventName = null) => events.RemoveHandler(handler, eventName);
        protected void RaiseEvent<TEventArgs>(in TEventArgs eventArgs, string eventName) => events[eventName]?.Invoke(this, eventArgs);
    }
}
