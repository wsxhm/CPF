using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class TestWindow : Window
    {
        protected override void InitializeComponent()
        {
            BaseCoinDatas = new Collection<(string, string, CoinTypeState, string)>();
            BaseCoinDatas.Add(("BTC", "Bitcoin", CoinTypeState.Selected, "0@0"));
            BaseCoinDatas.Add(("BTC", "Bitcoin", CoinTypeState.Default, "0@0"));
            LoadStyleFile("res://CPFApplication1/Stylesheet1.css");//加载样式文件，文件需要设置为内嵌资源

            Title = "标题";
            Width = 500;
            Height = 400;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children =
                {
                    new Button{ Content="按钮" ,MarginBottom = 10,
                    Commands = { {
                            nameof(Button.Click),
                            nameof(Click1),
                            this,
                            CommandParameter.EventSender,
                            CommandParameter.EventArgs
                            } }
                    },
                    new ListBox
                    {
                        //IsVirtualizing=true,
                        MarginBottom = 73,
                        MarginTop = 38,
                        Width = "100%",
                        ItemTemplate=typeof(CoinTypeTemplate),
                        SelectionMode= SelectionMode.Multiple,
                        //Items =
                        //{
                        //    ("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //    ("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //    //("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //    //("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //    //("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //    //("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //    //("BTC","Bitcoin",CoinTypeState.Selected,"0@0"),
                        //},
                        Bindings =
                        {
                            {
                                nameof(ListBox.Items),
                                nameof(BaseCoinDatas),
                                this,
                                BindingMode.TwoWay
                            }
                        }
                    },
                }
            }));

            if (!DesignMode)//设计模式下不执行，也可以用#if !DesignMode
            {

            }
        }

        public IList<(string, string, CoinTypeState, string)> BaseCoinDatas
        {
            get { return GetValue<IList<(string, string, CoinTypeState, string)>>(); }
            set { SetValue(value); }
        }

#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码
        public void Click1(CpfObject obj, RoutedEventArgs e)
        {
            IList<(string, string, CoinTypeState, string)> tempBaseCoinDatas = new Collection<(string, string, CoinTypeState, string)>();
            tempBaseCoinDatas.Add(("BTC", "Bitcoin", CoinTypeState.Selected, "0@0"));
            tempBaseCoinDatas.Add(("BTC", "Bitcoin", CoinTypeState.Selected, "0@0"));
            BaseCoinDatas = tempBaseCoinDatas;
        }
#endif
    }

    [CPF.Design.DesignerLoadStyle("res://UdunWallet/Stylesheet1.css")]//用于设计的时候加载样式
    public class CoinTypeTemplate : ListBoxItem
    {
        //模板定义
        protected override void InitializeComponent()
        {
            CornerRadius = "8";
            Height = 56;
            Width = 192;
            Children.Add(new TextBlock
            {
                Classes = "main,foreColor10",
                MarginLeft = 48,
                MarginTop = 10,
                Text = "CPF控件",
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        "Item1"
                    }
                }
            });
            Children.Add(new SVG
            {
                MarginTop = 10,
                MarginLeft = 16,
                IsAntiAlias = true,
                Height = 18,
                Stretch = Stretch.Uniform,
                Source = "res://UdunWallet/Resources/checkbox-round-no-with-basis.svg",
            });
            Children.Add(new TextBlock
            {
                Classes = "weaken,foreColor6",
                MarginLeft = 48,
                MarginTop = 33,
                Text = "CPF控件",
                Bindings =
                {
                    {
                        nameof(TextBlock.Text),
                        "Item2"
                    }
                }
            });
            Bindings.Add(nameof(State), "Item3");
        }

        public CoinTypeState State
        {
            get { return GetValue<CoinTypeState>(); }
            set { SetValue(value); }
        }
        bool isSetState;
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
            if (propertyName == nameof(IsSelected))
            {
                this.Background = IsSelected ? "#EEEEEE" : "#FFFFFF";
            }
            if (propertyName == nameof(IsSelected) && !isSetState)
            {
                if ((bool)newValue && IsEnabled)
                {
                    State = CoinTypeState.Selected;
                }
                else if ((bool)newValue && !IsEnabled)
                {
                    State = CoinTypeState.Disable;
                }
                else if (!(bool)newValue)
                {
                    State = CoinTypeState.Default;
                }
            }
            else if (propertyName == nameof(IsEnabled) && !isSetState)
            {
                if ((bool)newValue && IsSelected)
                {
                    State = CoinTypeState.Selected;
                }
                else if (!(bool)newValue && IsSelected)
                {
                    State = CoinTypeState.Disable;
                }
                else
                {
                    State = CoinTypeState.Default;
                }
            }
            else if (propertyName == nameof(State))
            {
                isSetState = true;
                switch ((CoinTypeState)newValue)
                {
                    case CoinTypeState.Default:
                        IsEnabled = true;
                        IsSelected = false;
                        break;
                    case CoinTypeState.Selected:
                        IsEnabled = true;
                        IsSelected = true;
                        break;
                    case CoinTypeState.Disable:
                        IsEnabled = false;
                        IsSelected = true;
                        break;
                    default:
                        break;
                }
                isSetState = false;
            }
        }

#if !DesignMode //用户代码写到这里，设计器下不执行，防止设计器出错
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }
        //用户代码

#endif
    }

    public enum CoinTypeState
    {
        Default,
        Selected,
        Disable,
    }
}
