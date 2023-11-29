using CPF.Controls;
using CPF.Toolkit.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Toolkit
{
    public static class ToolkitHelper
    {

        internal static void CreateLoading(this ILoading loading,UIElement uIElement)
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
                layer.ShowDialog(uIElement);
                dynamic t = task;
                var result = await t;
                loadingBox.Invoke(layer.CloseDialog);
                return result;
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
                layer.ShowDialog(uIElement);
                await task;
                loadingBox.Invoke(layer.CloseDialog);
            };
        }
    }
}
