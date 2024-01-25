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

        private void Window_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DataContext):
                    {
                        if (e.NewValue is ICloseable closeable)
                        {
                            closeable.Closable -= Closeable_Closable;
                            closeable.Closable += Closeable_Closable;
                        }
                    }
                    break;
            }

            void Closeable_Closable(object _, ClosingEventArgs ee)
            {
                if (!ee.Cancel)
                {
                    var window = (Window)sender;
                    window.Close();
                }
            }
        }
    }
}
