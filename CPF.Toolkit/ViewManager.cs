using CPF.Controls;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                if (view.DataContext is ILoading loading)
                {
                    loading.ShowLoadingFunc += async (message, task) =>
                    {
                        var loadingBox = new LoadingBox { Message = message };
                        var layer = new LayerDialog
                        {
                            Name = "loadingDialog",
                            Content = loadingBox,
                            ShowCloseButton = false,
                            Background = null,
                        };
                        layer.ShowDialog(view);
                        dynamic t = task;
                        var result = await t;
                        loadingBox.Invoke(layer.CloseDialog);
                        return (object)result;
                    };
                    loading.ShowLoading += async (message, task) =>
                    {
                        var loadingBox = new LoadingBox { Message = message };
                        var layer = new LayerDialog
                        {
                            Name = "loadingDialog",
                            Content = loadingBox,
                            ShowCloseButton = false,
                            Background = null,
                        };
                        layer.ShowDialog(view);
                        await task;
                        loadingBox.Invoke(layer.CloseDialog);
                    };
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
