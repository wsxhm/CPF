using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class OpenFileDialogView : Control
    {
        protected override void InitializeComponent()
        {
            Background = "#FFFFFF";
            Width = 400;
            Height = 600;
            if (!DesignMode)
            {
                Width = "90%";
                Height = "90%";
            }
            Children.Add(new TextBlock
            {
                FontSize = 20f,
                MarginTop = 19.3f,
                Text = "文件选择"
            });
            Children.Add(new TextBlock
            {
                Height = 20f,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MarginTop = 57.2f,
                Text = "",
                Width = "90%",
            });
            Children.Add(new ListBox
            {
                MarginBottom = 90f,
                MarginTop = 80f,
                Width = "90%",
            });
            Children.Add(new Button
            {
                Name="ok",
                MarginRight = 30,
                MarginBottom = 29f,
                Height = 31.1f,
                Width = 88.8f,
                Content = "确定",
                Commands =
                {
                    {
                        nameof(Button.Click),
                        (s,e)=>
                        {
                            
                        }
                    }
                }
            });
            Children.Add(new Button
            {
                Name="cancel",
                MarginBottom = 29f,
                MarginLeft = 30f,
                Height = 31.1f,
                Width = 88.8f,
                Content = "取消",
                Commands =
                {
                    {
                        nameof(Button.Click),
                        (s,e)=>
                        {
                            
                        }
                    }
                }
            });
        }

        public Collection<string> SelectedFiles
        {
            get { return GetValue<Collection<string>>(); }
            set { SetValue(value); }
        }
    }
}
