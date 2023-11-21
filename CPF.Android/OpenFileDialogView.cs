using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Text;
using Java.IO;
using CPF.Input;

namespace CPF.Android
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
                Width = "90%",
                Background = "#0f0",
                Bindings =
                {
                    {nameof(TextBlock.Text),nameof(CurrentDirectory),this }
                }
            });
            Children.Add(new ListBox
            {
                MarginBottom = 90f,
                MarginTop = 80,
                Width = "90%",
                ItemTemplate = typeof(ListBoxTemplate),
                Background = "#f00",
                Bindings =
                {
                    {nameof(ListBox.Items),nameof(Files),this }
                },
                Commands =
                {
                    {nameof(ListBox.MouseDown),ListBoxMouseDown }
                }
            });
            Children.Add(new Button
            {
                Name = "ok",
                MarginRight = 30,
                MarginBottom = 29f,
                Height = 31.1f,
                Width = 88.8f,
                Content = "确定",
                Commands =
                {
                    {nameof(Button.Click),(s,e)=>{ } }
                }
            });
            Children.Add(new Button
            {
                Name = "cancel",
                MarginBottom = 29f,
                MarginLeft = 30f,
                Height = 31.1f,
                Width = 88.8f,
                Content = "取消",
                Commands =
                {
                    {nameof(Button.Click),(s,e)=>{ this.Dispose(); } }
                }
            });
        }

        public OpenFileDialogView()
        {
            CurrentDirectory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            SelectedFiles = new Collection<string>();
            Files = new Collection<(string, Image, bool, bool, string)>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RefreshList();
        }

        private void RefreshList()
        {
            var dir = new File(CurrentDirectory);
            //var list = System.IO.Directory.GetFiles(CurrentDirectory);
            //var dirs = System.IO.Directory.GetDirectories(CurrentDirectory);
            File[] files = dir.ListFiles();
            var fs = Files;
            fs.Clear();
            if (dir.Parent != null)
            {
                fs.Add(("<", null, true, true, dir.Parent));
            }
            if (files != null)
            {
                foreach (var item in files)
                {
                    fs.Add((item.Name, null, false, item.IsDirectory, item.AbsolutePath));
                }
            }
        }

        public Collection<string> SelectedFiles
        {
            get { return GetValue<Collection<string>>(); }
            set { SetValue(value); }
        }
        public string CurrentDirectory
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public Collection<(string, Image, bool, bool, string)> Files
        {
            get { return GetValue<Collection<(string, Image, bool, bool, string)>>(); }
            set { SetValue(value); }
        }

        void ListBoxMouseDown(CpfObject sender, object e)
        {
            var listbox = sender as ListBox;
            var listItem = listbox.IsInItem((e as RoutedEventArgs).OriginalSource as UIElement);
            if (listItem == null)
            {
                return;
            }
            var item = ((string, Image, bool, bool, string))listItem.DataContext;
            if (item.Item1 != null)
            {
                if (item.Item3)
                {
                    var dir = new File(CurrentDirectory);
                    if (dir.Parent != null && dir.Parent != "/")
                    {
                        CurrentDirectory = dir.Parent;
                    }
                    RefreshList();
                }
                else
                {
                    if (item.Item4)
                    {
                        CurrentDirectory = item.Item5;
                        RefreshList();
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
