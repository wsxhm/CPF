using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Input;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApp1
{
    public class BrowseFileWindow : Window
    {
        protected override void InitializeComponent()
        {
            LoadStyleFile("res://ConsoleApp1/Stylesheet1.css");
            LoadStyleFile("res://ConsoleApp1/BrowserFile.css", true);
            //加载样式文件，文件需要设置为内嵌资源

            //Title = "文件选择";
            Width = 700;
            Height = 430;
            CanResize = true;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children = //内容元素放这里
                {
                    new ListBox
                    {
                        Commands =
                        {
                            {
                                nameof(ListBox.ItemMouseDown),
                                nameof(DriveMouseDown),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        PresenterFor = this,
                        Name = nameof(driveList),
                        MarginBottom = 0,
                        MarginTop = 0,
                        MarginLeft = 0,
                        Width = 130,
                        ItemTemplate=typeof(DriveTemplate)
                    },
                    new TextBox
                    {
                        Commands =
                        {
                            {
                                nameof(TextBox.KeyDown),
                                nameof(addressKeyDown),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        PresenterFor = this,
                        Name = nameof(addressTextBox),
                        Classes = "singleLine",
                        MarginRight = 1,
                        MarginLeft = 183,
                        MarginTop = 0,
                        Height = 27,
                    },
                    new DataGrid
                    {
                        Commands =
                        {
                            {
                                nameof(DataGrid.CellDoubleClick),
                                nameof(itemDoubleClick),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        PresenterFor = this,
                        Name = nameof(dataGrid),
                        Columns =
                        {
                            new DataGridTemplateColumn
                            {
                                Header="名称",
                                Width=300,
                                Binding=nameof(ItemInfo.Name),
                                CellTemplate=typeof(FileTemplate)
                            },
                            new DataGridTextColumn
                            {
                                Header="大小",
                                Width=100,
                                Binding=nameof(ItemInfo.Size)
                            },
                            new DataGridTextColumn
                            {
                                Header="修改日期",
                                Width=130,
                                Binding=nameof(ItemInfo.DateTime)
                            },
                        },
                        MarginLeft = 130,
                        MarginTop = 27,
                        MarginBottom = 36,
                        MarginRight = 0,
                        SelectionUnit= DataGridSelectionUnit.FullRow,
                        SelectionMode= DataGridSelectionMode.Single,
                    },
                    new ComboBox
                    {
                        PresenterFor = this,
                        Name = nameof(ffCombobox),
                        Height = 25,
                        MarginLeft = 141,
                        MarginBottom = 7,
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(Ok),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Height = 27,
                        Width = 78,
                        MarginRight = 126,
                        MarginBottom = 5,
                        Content = "确认",
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(Cancel),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        MarginBottom = 5,
                        MarginRight = 22,
                        Height = 27,
                        Width = 80,
                        Content = "取消",
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(parentDir),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Width = 54,
                        Height = 27,
                        MarginLeft = 130,
                        MarginTop = 0,
                        Content = "上一级",
                    },
                }
            })
            {
                MaximizeBox = true
            });
            if (!DesignMode)//设计模式下不执行，也可以用#if !DesignMode
            {
                
            }
        }
        ComboBox ffCombobox;
        TextBox addressTextBox;
        ListBox driveList;
        DataGrid dataGrid;
        string currentPath;
#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

            dataGrid = FindPresenterByName<DataGrid>(nameof(dataGrid));
            driveList = FindPresenterByName<ListBox>(nameof(driveList));
            addressTextBox = FindPresenterByName<TextBox>(nameof(addressTextBox));
            ffCombobox = FindPresenterByName<ComboBox>(nameof(ffCombobox));

            if (CPF.Platform.Application.OperatingSystem == CPF.Platform.OperatingSystemType.Windows)
            {
                var ds = DriveInfo.GetDrives();
                if (ds != null)
                {
                    foreach (var item in ds)
                    {
                        driveList.Items.Add((item.VolumeLabel + " (" + item.Name + ")", item.Name));
                    }
                }
            }
            else
            {
                driveList.Items.Add(("Home", "/home"));
                driveList.Items.Add(("其他位置", "/"));
            }
            if (FileFilter == null)
            {
                ffCombobox.Items.Add("全部文件");
            }
            else
            {
                ffCombobox.Items.Add($"{FileFilter.Name} ({FileFilter.Extensions})");
            }
            ffCombobox.SelectedIndex = 0;
        }
        //用户代码

#endif
        void addressKeyDown(CpfObject obj, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Keys.Enter)
            {
                OpenDirectory();
            }
        }
        void DriveMouseDown(CpfObject obj, ListBoxItemMouseEventArgs eventArgs)
        {
            addressTextBox.Text = (((string, string))eventArgs.Item.Content).Item2;
            OpenDirectory();
        }

        void OpenDirectory()
        {
            var path = addressTextBox.Text;
            try
            {
                var ds = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);
                dataGrid.Items.Clear();
                dataGrid.SelectedIndexs.Clear();
                currentPath = path;
                if (ds != null)
                {
                    foreach (var item in ds)
                    {
                        var dirInfo = new DirectoryInfo(item);
                        if (dirInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            continue;
                        }
                        dataGrid.Items.Add(new ItemInfo { DateTime = dirInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), Name = dirInfo.Name, Path = dirInfo.FullName });
                    }
                }
                if (files != null)
                {
                    string[] exs = null;
                    if (FileFilter != null)
                    {
                        exs = FileFilter.Extensions.ToLower().Split(',');
                    }
                    foreach (var item in files)
                    {
                        var file = new FileInfo(item);
                        if (file.Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            continue;
                        }
                        if (exs != null)
                        {
                            if (string.IsNullOrEmpty(file.Extension))
                            {
                                continue;
                            }
                            var ex = file.Extension.ToLower().TrimStart('.');
                            if (!exs.Any(a => a == ex))
                            {
                                continue;
                            }
                        }
                        dataGrid.Items.Add(new ItemInfo { DateTime = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), Name = file.Name, Path = file.FullName, Size = GetFileSize(file.Length), IsFile = true });
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// 格式化文件大小的C#方法
        /// </summary>
        /// <param name="filesize">文件的大小,传入的是一个bytes为单位的参数</param>
        /// <returns>格式化后的值</returns>
        private static string GetFileSize(long filesize)
        {
            if (filesize < 0)
            {
                return "0";
            }
            else if (filesize >= 1024 * 1024 * 1024) //文件大小大于或等于1024MB
            {
                return string.Format("{0:0.00} GB", (double)filesize / (1024 * 1024 * 1024));
            }
            else if (filesize >= 1024 * 1024) //文件大小大于或等于1024KB
            {
                return string.Format("{0:0.00} MB", (double)filesize / (1024 * 1024));
            }
            else if (filesize >= 1024) //文件大小大于等于1024bytes
            {
                return string.Format("{0:0.00} KB", (double)filesize / 1024);
            }
            else
            {
                return string.Format("{0:0.00} bytes", filesize);
            }
        }
        void itemDoubleClick(CpfObject obj, DataGridCellEventArgs eventArgs)
        {
            var info = eventArgs.Cell.DataContext as ItemInfo;
            if (info.IsFile)
            {
                DialogResult = info.Path;
            }
            else
            {
                addressTextBox.Text = info.Path;
                OpenDirectory();
            }
        }
        void Cancel(CpfObject obj, RoutedEventArgs eventArgs)
        {
            this.Close();
        }
        void Ok(CpfObject obj, RoutedEventArgs eventArgs)
        {
            var info = dataGrid.SelectedValue as ItemInfo;
            if (info != null && info.IsFile)
            {
                DialogResult = info.Path;
            }
        }
        void parentDir(CpfObject obj, RoutedEventArgs eventArgs)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currentPath))
                {
                    return;
                }
                var dic = new DirectoryInfo(currentPath);
                if (dic.Parent == null)
                {
                    return;
                }
                addressTextBox.Text = dic.Parent.FullName;
                OpenDirectory();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// 文件过滤
        /// </summary>
        [NotCpfProperty]
        public FileDialogFilter FileFilter { get; set; }// = new FileDialogFilter { Extensions = "dll", Name = "dll" };
    }
}
