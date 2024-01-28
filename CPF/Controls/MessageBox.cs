using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPF.Controls
{
    public class MessageBox
    {
        /// <summary>
        /// 弹窗提示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="owner">如果为null，则使用当前激活的窗体</param>
        public static Task<object> Show(string message, string title = null, Window owner = null)
        {
            Task<object> task = null;
            CPF.Threading.Dispatcher.MainThread.Invoke(() =>
            {
                Window window = CreateWindow(message, title, owner);
                task = window.ShowDialog();
            });
            return task;
        }
        /// <summary>
        /// 弹窗提示，可阻塞当前方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="owner">如果为null，则使用当前激活的窗体</param>
        /// <returns></returns>
        public static object ShowSync(string message, string title = null, Window owner = null)
        {
            object result = null;
            CPF.Threading.Dispatcher.MainThread.Invoke(() =>
            {
                Window window = CreateWindow(message, title, owner);
                result = window.ShowDialogSync();
            });
            return result;
        }

        private static Window CreateWindow(string message, string title, Window owner)
        {
            var main = owner;
            if (main == null)
            {
                main = Window.Windows.FirstOrDefault(a => a.IsKeyboardFocusWithin);
                if (main == null)
                {
                    main = Window.Windows.FirstOrDefault(a => a.IsMain);
                }
                var os = CPF.Platform.Application.OperatingSystem;
                if (main == null && (os == Platform.OperatingSystemType.Windows || os == Platform.OperatingSystemType.Linux || os == Platform.OperatingSystemType.OSX))
                {
                    throw new Exception("需要有主窗体");
                }
            }
            Window window = new Window { CanResize = false, Background = null, Title = title == null ? "" : title, Icon = main.Icon, MinWidth = 200, Name = "messageBox", TopMost = main.TopMost };
            window.LoadStyle(main);
            window.Children.Add(new WindowFrame(window, new Panel
            {
                Children =
                    {
                        new Panel
                        {
                            Name="messagePanel",
                            Children =
                            {
                                new TextBlock{ Name="message", Text = message == null ? "" : message}
                            },
                            MarginBottom=50,
                            MarginTop=15,
                            MarginLeft=10,
                            MarginRight=10,
                        },
                        new Button{ Content="OK", Width=60,MarginBottom=15,Commands={ {nameof(Button.Click),(s,e)=> { window.DialogResult = true; } } } }
                    }
            })
            { MinimizeBox = false, MaximizeBox = false, });
            return window;
        }
    }
}
