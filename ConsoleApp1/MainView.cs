using CPF;
using CPF.Controls;
using System;

namespace ClassLibrary1
{
    public class MainView : Control
    {
        protected override void InitializeComponent()
        {
            //Root.LoadStyleFile("res://ClassLibrary1/Stylesheet1.css");
            Background = "#FFFFFF";
            Width = 387;
            Height = 464;
            Children.Add(new Button
            {
                MarginTop = 42,
                Commands =
                {
                    {
                        nameof(Button.Click),
                        nameof(ClickTest),
                        this,
                        CommandParameter.EventSender,
                        CommandParameter.EventArgs
                    },
                },
                Height = 31,
                Width = 72,
                Content = "Button",
            });
            //Children.Add(new ListBox
            //{
            //    Height = 189,
            //    Width = 148,
            //    Items =
            //    {
            //        1,
            //        2,
            //        3,
            //        4,
            //        5,
            //        6,
            //        7,
            //        8,
            //        9,
            //        10,
            //        11,
            //        12,
            //        13,
            //        14,
            //        15,
            //        16,
            //        17,
            //        18,
            //        19,
            //        20
            //    }
            //});
        }
        void ClickTest(CpfObject obj, RoutedEventArgs eventArgs)
        {
            //new Window1().Show();
        }
    }
}
