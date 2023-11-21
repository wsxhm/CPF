
using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{

    public class Window6 : Window
    {
        protected override void InitializeComponent()
        {
            Background = null;
            Width = 800;
            Height = 450;
            Children.Add(new WindowFrame(this, new Panel
            {
                UseLayoutRounding = true,
                Width = "100%",
                Height = "100%",
                Background = "240,240,240,255",
                Children =
                {
                    new Label
                    {
                        Text = "ChatListBox",
                        Background = "255,255,255,50",
                        MarginLeft = 58,
                        MarginTop = 24,
                        Width = 150,
                        Height = 250,
                        Name = "chatListBox1",
                        PresenterFor = this,
                        BorderFill = "#DCDFE6",
                        BorderStroke = "1",
                    },
                    new CheckBox
                    {
                        Content = "skinCheckBox1",
                        MarginLeft = 493,
                        MarginTop = 85,
                        Width = 114,
                        Height = 21,
                        Name = "skinCheckBox1",
                        PresenterFor = this,
                    },
                    new Button
                    {
                        Content = "skinButton1",
                        MarginLeft = 495,
                        MarginTop = 166,
                        Width = 75,
                        Height = 23,
                        Name = "skinButton1",
                        PresenterFor = this,
                    },
                    new CheckBox
                    {
                        Content = "dSkinCheckBox1",
                        MarginLeft = 524,
                        MarginTop = 257,
                        Width = 112,
                        Height = 18,
                        Name = "dSkinCheckBox1",
                        PresenterFor = this,
                    },
                    new Label
                    {
                        Text = "DSkinCode",
                        MarginLeft = 320,
                        MarginTop = 150,
                        Width = 100,
                        Height = 100,
                        Name = "dSkinCode1",
                        PresenterFor = this,
                        BorderFill = "#DCDFE6",
                        BorderStroke = "1",
                    },
                }
            })
            {MaximizeBox=true });
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        void btnDoubleClick(CpfObject obj, RoutedEventArgs eventArgs)
        {
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            string osk = null;

            if (osk == null)
            {
                osk = System.IO.Path.Combine(System.IO.Path.Combine(windir, "sysnative"), "osk.exe");
                if (!System.IO.File.Exists(osk))
                    osk = null;
            }

            if (osk == null)
            {
                osk = System.IO.Path.Combine(System.IO.Path.Combine(windir, "system32"), "osk.exe");
                if (!System.IO.File.Exists(osk))
                {
                    osk = null;
                }
            }

            if (osk == null)
                osk = "osk.exe";
            Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = osk });
            //Process.Start(osk);
            eventArgs.Handled = true;
        }
        void panelDoubleClick(CpfObject obj, RoutedEventArgs eventArgs)
        {

        }
        void borderDoubleClick(CpfObject obj, RoutedEventArgs eventArgs)
        {
            eventArgs.Handled = true;
        }
    }

}
