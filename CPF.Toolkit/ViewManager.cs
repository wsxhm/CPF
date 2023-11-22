using CPF.Controls;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit
{
    public static class ViewManager
    {
        public static T View<T>(params object[] arges) where T : Window
        {
            var view = Activator.CreateInstance(typeof(T), arges) as T;
            view.Closing += View_Closing;
            view.PropertyChanged += View_PropertyChanged;
            return view;
        }

        private static void View_PropertyChanged(object sender, CPFPropertyChangedEventArgs e)
        {
            var view = sender as Window;
            if (e.PropertyName == nameof(Window.DataContext))
            {
                if (view.DataContext is IClosable closable)
                {
                    closable.Closable += (ss, dialogResult) =>
                    {
                        if (view.IsDialogMode == true)
                        {
                            view.DialogResult = dialogResult;
                        }
                        view.Close();
                    };
                }
                if (view.DataContext is IDialog dialog)
                {
                    dialog.Dialog = new DialogService(view);
                }
            }
        }

        private static void View_Closing(object sender, ClosingEventArgs e)
        {
            var view = sender as Window;
            if (view.DataContext is IClosable closable)
            {
                closable.OnClosable(sender, e);
            }
        }
    }
}
