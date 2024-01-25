using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit
{
    public class ViewBehavior : Behavior<Window>
    {
        protected override void OnBehaviorTo(Window window)
        {
            this.DataContextChanged(window);
            window.PropertyChanged += Window_PropertyChanged;
            window.Loaded += Window_Loaded;
            window.Closing += Window_Closing;
            window.Closed += Window_Closed;
            base.OnBehaviorTo(window);
        }

        protected override void OnDetachingFrom(Window window)
        {
            window.PropertyChanged -= Window_PropertyChanged;
            window.Loaded -= Window_Loaded;
            window.Closing -= Window_Closing;
            window.Closed -= Window_Closed;
            base.OnDetachingFrom(window);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var window = (Window)sender;
            if (window.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void Window_Closing(object sender, ClosingEventArgs e)
        {
            var window = (Window)sender;
            if (window.DataContext is ICloseable closeable)
            {
                closeable.OnClosable(e);
            }
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            var window = (Window)sender;
            if (window.DataContext is ILoaded loaded)
            {
                loaded.OnLoaded();
            }
        }

        void DataContextChanged(Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            if (window.DataContext is ICloseable closeable)
            {
                closeable.Closable -= Closeable_Closable;
                closeable.Closable += Closeable_Closable;
            }

            if (window.DataContext is IDialog dialog)
            {
                dialog.Dialog = new DialogService(window);
            }

            void Closeable_Closable(object _, ClosingEventArgs ee)
            {
                if (!ee.Cancel)
                {
                    window.Close();
                }
            }
        }

        private void Window_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DataContext):
                    {
                        DataContextChanged(sender as Window);
                    }
                    break;
            }
        }
    }
}
