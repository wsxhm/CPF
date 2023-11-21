using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Controls;
using CPF.Shapes;
using System.Diagnostics;
using System.Threading;
using CPF.Cef;
using static CPF.Cef.Wrapper.Helpers;

namespace ConsoleApp1
{
    public class Window3 : Window
    {
        protected override void InitializeComponent()
        {
            CanResize = true;
            Title = "标题";
            Width = 630;
            Height = 417;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Background = "#00000000",
                Children =
                {
                    //内容元素放这里
                    new Button
                    {
                        MarginLeft = 38f,
                        MarginTop = 21.9f,
                        Height = 29.5f,
                        Width = 81.6f,
                        Content="访问",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (s,e)=>
                                {
                                    FindPresenterByName<WebBrowser>("webBrowser").Url=FindPresenterByName<TextBox>("textBox").Text;
                                }
                            }
                        }
                    },
                    new TextBox
                    {
                        Bindings =
                        {
                            {
                                nameof(TextBox.Text),
                                "Url",
                                FindPresenterByName("webBrowser")
                            },
                        },
                        Height = 27,
                        BorderFill = "#000000",
                        BorderStroke = "1,Solid",
                        MarginLeft = 153,
                        MarginTop = 25,
                        Width = 145,
                        PresenterFor=this,
                        Classes="Single",
                        Name="textBox"
                    },
                    new WebBrowser
                    {
                        Bindings =
                        {
                            {
                                nameof(WebBrowser.Title),
                                "Title",
                                this,
                                BindingMode.OneWayToSource
                            },
                        },
                        PresenterFor=this,
                        Name=nameof(webBrowser),
                        MarginBottom=0,
                        MarginLeft=0,
                        MarginRight=0,
                        MarginTop=60,
                        Url="about:blank",
                        CommandContext=this,
                    },
                    new Button
                    {
                        Width = 71.7f,
                        MarginLeft = 337.1f,
                        MarginTop = 25f,
                        Content = "开发者工具",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (s,e)=>
                                {
                                    webBrowser.ShowDev();
                                }
                            }
                        }
                    },
                    new Button
                    {
                        Width = 71.7f,
                        MarginLeft = 427f,
                        MarginTop = 25f,
                        Content = "调用JS",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                async (s,e)=>
                                {
                                    //var test=await FindPresenterByName<WebBrowser>("webBrowser").ExecuteJavaScript("prompt('test测试','test')");
                                    //Debug.WriteLine(test);
                                    //Console.WriteLine(test);
                                    var test = await webBrowser.ExecuteJavaScript("test('test测试弹窗')");
                                }
                            }
                        }
                    },
                    new Button
                    {
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                nameof(NewWindow),
                                this,
                                CommandParameter.EventSender,
                                CommandParameter.EventArgs
                            },
                        },
                        Height = 26,
                        Width = 75,
                        MarginLeft = 524,
                        MarginTop = 21,
                        Content = "新窗体",
                    },
                }
            })
            {
                MaximizeBox = true
            });
            LoadStyleFile("res://ConsoleApp1/Stylesheet.css");
            //加载样式文件，文件需要设置为内嵌资源

            if (!DesignMode)//设计模式下不执行
            {

            }
        }
        WebBrowser webBrowser;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            webBrowser = FindPresenterByName<WebBrowser>(nameof(webBrowser));
            //Thread.Sleep(15000);
            //WindowState = WindowState.Maximized;
            webBrowser.RegisterJavascriptObject(new TestJSClass(), "testObject");
        }
        [JSFunction]
        public string test(string p)
        {
            //return "test测试" + p + p1 + dateTime;
            //Debug.WriteLine("test测试" + p + p1 + dateTime);
            //return dateTime;
            MessageBox.Show(p);
            return p;
        }

        void NewWindow(CpfObject obj, RoutedEventArgs eventArgs)
        {
            new Window3().Show();

            //CefRuntime.PostTask(CefThreadId.UI, new ActionTask(() =>
            //{

            //    var Address = "http://127.0.0.1:1080";
            //    var rc = webBrowser.BrowserHost.GetRequestContext();
            //    CefValue vv = CefValue.Create();
            //    var dic = CefDictionaryValue.Create();
            //    dic.SetString("mode", "fixed_servers");
            //    dic.SetString("server", Address);
            //    var r = vv.SetDictionary(dic);
            //    string error;
            //    bool success = rc.SetPreference("proxy", vv, out error);
            //}));

        }
    }

    public class TestJSClass
    {
        public string Name { get; set; }

        public void Test(string test, int a)
        {
            MessageBox.Show(test + a);
        }
    }
}
