using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Input;
using System.Linq;
using CPF.Animation;
using System.Collections;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// 表示带有下拉列表的选择控件，通过单击控件上的箭头可显示或隐藏下拉列表
    /// </summary>
    [Description("表示带有下拉列表的选择控件，通过单击控件上的箭头可显示或隐藏下拉列表")]
    public class ComboBox : ListBox
    {
        static Popup popup;
        public ComboBox()
        {
            //DropDownPanel.LayoutUpdated += DropDownPanel_LayoutUpdated;
        }

        //private void DropDownPanel_LayoutUpdated(object sender, RoutedEventArgs e)
        //{
        //    if (IsDropDownOpen && popup != null)
        //    {
        //        var s = DropDownPanel.ActualSize;
        //        BeginInvoke(() =>
        //        {
        //            popup.Width = s.Width + DropDownPanel.MarginLeft.GetActualValue(s.Width) + DropDownPanel.MarginRight.GetActualValue(s.Width) + 1;
        //        });
        //        //popup.Height = s.Height + DropDownPanel.MarginTop.GetActualValue(s.Height) + DropDownPanel.MarginBottom.GetActualValue(s.Height);
        //        //System.Diagnostics.Debug.WriteLine(popup.Width);
        //    }
        //}

        /// <summary>
        /// 下拉框容器 #DropDownPanel
        /// </summary>
        [NotCpfProperty]
        public Panel DropDownPanel { get; private set; } = new Panel { Name = "DropDownPanel" };
        /// <summary>
        /// 获取或设置一个值，该值指示组合框的下拉部分当前是否打开
        /// </summary>
        public bool IsDropDownOpen { get { return GetValue<bool>(); } set { SetValue(value); } }
        ///// <summary>
        ///// 获取选择框内容的项模板
        ///// </summary>
        //public UIElementTemplate<SelectionItem> SelectionItemTemplate
        //{
        //    get { return GetValue<UIElementTemplate<SelectionItem>>(); }
        //    set { SetValue(value); }
        //}
        //SelectionItem selectionItem;

        protected override void InitializeComponent()
        {
            Children.Add(new TextBlock
            {
                MarginLeft = 3,
                MarginRight = 15,
                VerticalAlignment = VerticalAlignment.Center,
                MaxHeight = "100%",
                TextTrimming = TextTrimming.CharacterEllipsis,
                Bindings =
                {
                    {nameof(Visibility),nameof(IsEditable),this,BindingMode.OneWay, (bool e)=>e?Visibility.Collapsed: Visibility.Visible },
                    {nameof(TextBlock.Text),nameof(SelectedItems),this,BindingMode.OneWay,SelectedValueConvert}
                }
            });
            Children.Add(new TextBox
            {
                MarginLeft = 3,
                MarginRight = 15,
                AcceptsReturn = false,
                HScrollBarVisibility = ScrollBarVisibility.Hidden,
                VScrollBarVisibility = ScrollBarVisibility.Hidden,
                Bindings =
                {
                    {nameof(Visibility),nameof(IsEditable),this,BindingMode.OneWay, (bool e)=>e?Visibility.Visible: Visibility.Collapsed },
                    {nameof(TextBlock.Text),nameof(SelectedItems),this,BindingMode.OneWay,SelectedItemsToText }
                },
                Commands =
                {
                    {nameof(TextBox.Text),TextBoxTextToSelectValue }
                }
            });


            Children.Add(new Shapes.Polyline { MarginRight = 5, IsAntiAlias = true, Points = { new Point(), new Point(4, 4), new Point(8, 0) } });
            //DropDownPanel.MarginTop = 1;
            DropDownPanel.MarginBottom = 1;
            //DropDownPanel.MarginLeft = 1;
            //DropDownPanel.MarginRight = 1;
            DropDownPanel.MinHeight = 20;
            //DropDownPanel.MaxHeight = 300;
            //DropDownPanel.Background = "#fff";
            //DropDownPanel.MaxWidth = 500;
            DropDownPanel[nameof(Width)] = (this, nameof(ActualSize), a => (FloatField)((Size)a).Width);
            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.Width = "100%";
            panel.MarginTop = 0;
            //panel.Height = "100%";
            if (ShowClear)
            {
                if (IsVirtualizing)
                {
                    DropDownPanel.Children.Add(new Border
                    {
                        Name = "dropDownBorder",
                        MarginTop = 0,
                        Width = "100%",
                        Height = "100%",
                        BorderFill = "172,172,172",
                        Background = "#fff",
                        Children =
                        {
                           new ScrollViewer { MarginTop = 0, MarginBottom = 30, Width = "100%", MaxHeight = 300, Content = new VirtualizationPresenter<ListBoxItem> { Width = "100%", MarginTop = 0, Child = panel, PresenterFor = this }, } ,
                           new Panel
                           {
                               MarginBottom = 0,
                               Width = "100%",
                                Height = 30,
                               Children =
                               {
                                  new TextBlock {
                                  Classes = "lblgreen",
                                  Foreground = "#05AA69",
                                  Cursor = Cursors.Hand, Name = "清除",Text = "清除",
                                  Commands = {
                                     { nameof(TextBlock.MouseUp),
                                       nameof(ClearSelectedVale),
                                       this
                                     }
                                 } }
                               }
                           }
                           
                       }
                    });

                }
                else
                {
                    DropDownPanel.Children.Add(new Border
                    {
                        Name = "dropDownBorder",
                        MarginTop = 0,
                        Width = "100%",
                        Height = "100%",
                        BorderFill = "172,172,172",
                        Background = "#fff",
                        Children = {
                             new ScrollViewer { MarginTop = 0, Width = "100%", MarginBottom = 30, Content = panel, MaxHeight = 300 },
                              new Panel
                           {
                               MarginBottom = 0,
                               Width = "100%",
                                Height = 30,
                               Children =
                               {
                                  new TextBlock {
                                  Classes = "lblgreen",
                                  Foreground = "#05AA69",
                                  Cursor = Cursors.Hand, Name = "清除",Text = "清除",
                                  Commands = {
                                     { nameof(TextBlock.MouseUp),
                                       nameof(ClearSelectedVale),
                                       this
                                     }
                                 } }
                               }
                           }
                        }
                    });
                }
            }
            else
            {
                if (IsVirtualizing)
                {
                    DropDownPanel.Children.Add(new Border { Name = "dropDownBorder", MarginTop = 0, Width = "100%", Height = "100%", BorderFill = "172,172,172", Background = "#fff", Child = new ScrollViewer { MarginTop = 0, Height = "100%", Width = "100%", MaxHeight = 300, Content = new VirtualizationPresenter<ListBoxItem> { Width = "100%", MarginTop = 0, Child = panel, PresenterFor = this }, } });
                }
                else
                {
                    DropDownPanel.Children.Add(new Border { Name = "dropDownBorder", MarginTop = 0, Width = "100%", Height = "100%", BorderFill = "172,172,172", Background = "#fff", Child = new ScrollViewer { MarginTop = 0, Width = "100%", Content = panel, MaxHeight = 300 } });
                }
            }
        }

        private void ClearSelectedVale()
        {
            SelectedValue = null;
            Popup.Hide();
            IsDropDownOpen = false;
        }

        protected virtual string SelectedValueConvert(object data)
        {
            var display = DisplayMemberPath;
            StringBuilder sb = new StringBuilder();
            if (data is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    sb.Append(ConvertStr(item, display));
                    sb.Append(',');
                }
                if (sb.Length > 0 && sb[sb.Length - 1] == ',')
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            else if (data != null)
            {
                sb.Append(ConvertStr(data, display));
            }
            return sb.ToString();
        }

        string ConvertStr(object a, string display)
        {
            var value = a;
            if (a is ContentControl contentControl)
            {
                value = contentControl.Content;
            }
            else if (a is UIElement element)
            {
                value = element.DataContext;
            }
            if (value == null)
            {
                return "";
            }
            if (!string.IsNullOrWhiteSpace(display))
            {
                var v = value.GetPropretyValue(display);
                if (v != null)
                {
                    return v.ToString();
                }
                return "";
            }
            return value.ToString();
        }

        protected virtual string SelectedItemsToText(object data)
        {
            //if (data == null || data is IEnumerable<object> enumer && enumer.Count() == 0)
            //{
            //    Binding.Current.Cancel();
            //    return null;
            //}
            if (isSetSelectValue)
            {
                Binding.Current.Cancel();
                return null;
            }
            var display = DisplayMemberPath;
            StringBuilder sb = new StringBuilder();
            if (data is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    sb.Append(ConvertStr(item, display));
                    sb.Append(',');
                }
                if (sb.Length > 0 && sb[sb.Length - 1] == ',')
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            else if (data != null)
            {
                sb.Append(ConvertStr(data, display));
            }
            return sb.ToString();
        }

        bool isSetSelectValue = false;
        protected virtual void TextBoxTextToSelectValue(CpfObject @object, object e)
        {
            if (!IsEditable || (Binding.Current != null && Binding.Current.Convert == SelectedItemsToText))
            {
                return;
            }
            isSetSelectValue = true;
            string txt = (@object as TextBox).Text;
            var display = DisplayMemberPath;
            //var sv = SelectedValuePath;
            //object selectValue = null;
            SelectedIndexs.Clear();
            var temp = txt.Split(',');
            foreach (var t in temp)
            {
                if (!string.IsNullOrWhiteSpace(display))
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        var item = Items[i];
                        if (item != null)
                        {
                            var value = item.GetPropretyValue(display);
                            if (value != null && value.ToString() == t)
                            {
                                //if (!string.IsNullOrWhiteSpace(sv))
                                //{
                                //selectValue = item.GetPropretyValue(sv);
                                //    break;
                                //}
                                //selectValue = item;
                                SelectedIndexs.Add(i);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        var value = Items[i];
                        if (value != null && value.ToString() == t)
                        {
                            //if (!string.IsNullOrWhiteSpace(sv))
                            //{
                            //    selectValue = value.GetPropretyValue(sv);
                            //    break;
                            //}
                            //selectValue = value;
                            SelectedIndexs.Add(i);
                            break;
                        }
                    }
                }
            }

            //SelectedValue = selectValue;
            //由于SelectedItems是异步更新的，只能异步延迟设置
            BeginInvoke(() =>
            {
                isSetSelectValue = false;
            });
        }

        //protected override Panel GetItemsPanel()
        //{
        //    var panel = (Panel)Find().FirstOrDefault(a => a.Name == "itemsPanel");
        //    if (panel == null)
        //    {
        //        var sv = DropDownPanel.Find<ScrollViewer>().FirstOrDefault(a => (a.Content is Panel) && ((Panel)a.Content).Name == "itemsPanel");
        //        if (sv != null)
        //        {
        //            panel = sv.Content as Panel;
        //        }
        //    }
        //    return panel;
        //}
        //Panel panel;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            //panel = GetItemsPanel();
            //if (panel == null)
            //{
            //    throw new Exception("未定义ItemsHostPanel");
            //}
            ItemsHost.LayoutUpdated += Panel_LayoutUpdated;
            ItemsHost.MouseDown += ItemsHost_MouseDown;
            ItemsHost.MouseUp += ItemsHost_MouseUp;
            ItemsHost.PreviewMouseDown += ItemsHost_PreviewMouseDown;
        }

        bool isMouseDownHost;

        private void ItemsHost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDownHost = true;
        }

        Point? touch;
        private void ItemsHost_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.IsTouch && SelectionMode == SelectionMode.Single)
            {
                if (touch == e.Location)
                {
                    IsDropDownOpen = false;
                }
                touch = null;
            }
        }

        private void ItemsHost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.IsTouch)
            {
                touch = e.Location;
            }
        }

        private void Panel_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            ItemsPanelHeight = ItemsHost.ActualSize.Height;
        }
        //protected override void OnLayoutUpdated()
        //{
        //    base.OnLayoutUpdated();
        //    ComboBoxWidth = ActualSize.Width;
        //}

        /// <summary>
        /// 下拉列表高度
        /// </summary>
        protected float ItemsPanelHeight
        {
            get { return GetValue<float>(); }
            private set { SetValue(value); }
        }
        ///// <summary>
        ///// 下拉框宽度
        ///// </summary>
        //protected float ComboBoxWidth
        //{
        //    get { return GetValue<float>(); }
        //    set { SetValue(value); }
        //}

        private static Popup Popup
        {
            get
            {
                if (popup == null)
                {
                    popup = new Popup { Name = "DropDownPopup" };
                    popup.Background = null;
                    popup.CanActivate = false;
                    popup.LayoutUpdated += Popup_LayoutUpdated;
                }
                return popup;
            }
        }

        private static void Popup_LayoutUpdated(object sender, RoutedEventArgs e)
        {
            popup.Height = "auto";
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            Root.LostFocus += Popup_LostFocus;
            Root.PreviewMouseDown += Root_PreviewMouseDown;
        }
        bool mouseDownMe = false;
        private void Root_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BeginInvoke(() =>
            {
                if (!mouseDownMe)
                {
                    IsDropDownOpen = false;
                }
                mouseDownMe = false;
            });
        }

        protected override void OnItemElementAdded(UIElement element)
        {
            base.OnItemElementAdded(element);
            element.MouseUp += Element_MouseUp;
        }

        private void Element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectionMode == SelectionMode.Single)
            {
                if (!e.IsTouch && isMouseDownHost)
                {
                    IsDropDownOpen = false;
                }
            }
        }

        protected override void OnItemElementRemoved(UIElement element)
        {
            base.OnItemElementRemoved(element);
            element.MouseUp -= Element_MouseUp;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            mouseDownMe = true;
            base.OnPreviewMouseDown(e);
        }


        private void Popup_LostFocus(object sender, RoutedEventArgs e)
        {
            BeginInvoke(() =>
            {
                //if (IsDisposed)
                //{
                //    return;
                //}
                IsDropDownOpen = false;
            });
        }

        protected override void OnDetachedFromVisualTree()
        {
            IsDropDownOpen = false;
            base.OnDetachedFromVisualTree();
            Root.LostFocus -= Popup_LostFocus;
            Root.PreviewMouseDown -= Root_PreviewMouseDown;
        }

        [PropertyChanged(nameof(Visibility))]
        void OnHide(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if ((Visibility)newValue != Visibility.Visible)
            {
                IsDropDownOpen = false;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled)
            {
                if (!IsKeyboardFocusWithin)
                {
                    Focus(NavigationMethod.Click);
                }
                //if (!(e.OriginalSource as UIElement).IsAncestors(DropDownPanel) || SelectionMode == SelectionMode.Single)
                //{
                //if (!IsDropDownOpen)
                //{
                IsDropDownOpen = !IsDropDownOpen;
                //}
                //}
            }
        }
        /// <summary>
        /// 编辑模式，就是可以文本框输入，自动选中下拉列表内容
        /// </summary>
        public bool IsEditable
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 是否展示清除按钮
        /// </summary>
        [PropertyMetadata(false)]
        public bool ShowClear
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
        //protected override void OnSelectionChanged(EventArgs e)
        //{
        //    base.OnSelectionChanged(e);
        //    //if (SelectedIndexs.Count > 0 && SelectionItemTemplate != null)
        //    //{
        //    //    if (selectionItem == null)
        //    //    {
        //    //        selectionItem = SelectionItemTemplate.CreateElement();
        //    //        Children.Add(selectionItem);
        //    //    }
        //    //    selectionItem.DataContext = SelectedItems;
        //    //    var dis = DisplayMemberPath;
        //    //    if (!string.IsNullOrWhiteSpace(dis))
        //    //    {
        //    //        selectionItem.Content = SelectedItems.Select(a => a.GetPropretyValue(dis));
        //    //    }
        //    //    else
        //    //    {
        //    //        selectionItem.Content = SelectedItems;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    if (selectionItem != null)
        //    //    {
        //    //        Children.Remove(selectionItem);
        //    //        selectionItem.Dispose();
        //    //        selectionItem = null;
        //    //    }
        //    //}
        //}

        //[PropertyChanged(nameof(SelectionItemTemplate))]
        //void RegisterSelectionItemTemplate(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        //{
        //    if (selectionItem != null)
        //    {
        //        Children.Remove(selectionItem);
        //        selectionItem.Dispose();
        //        selectionItem = null;
        //    }
        //    var n = newValue as UIElementTemplate<SelectionItem>;
        //    if (n != null && SelectedIndexs.Count > 0)
        //    {
        //        selectionItem = n.CreateElement();
        //        Children.Add(selectionItem);
        //        selectionItem.DataContext = SelectedItems;
        //        selectionItem.Content = SelectedItems;
        //    }
        //}

        [PropertyChanged(nameof(IsDropDownOpen))]
        void RegisterIsDropDownOpen(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var n = (bool)newValue;
            if (n)
            {
                //var p = PointToScreen(new Point(0, ActualSize.Height));
                //Popup.MarginTop = p.Y / Root.Scaling;
                //Popup.MarginLeft = p.X / Root.Scaling;
                //Popup.styleSheet = Root.StyleSheet;
                if (Popup.PlacementTarget is ComboBox comboBox && popup.PlacementTarget != this)
                {
                    comboBox.IsDropDownOpen = false;
                }
                popup.Placement = PlacementMode.Padding;
                popup.MarginLeft = 0;
                popup.MarginTop = "100%";
                Popup.Children.Add(DropDownPanel);
                popup.PlacementTarget = this;
                Popup.LoadStyle(Root);
                popup.StaysOpen = true;
                //popup.LayoutManager.ExecuteLayoutPass();
                //popup.InvalidateMeasure();
                popup.Width = "auto";
                Popup.Visibility = Visibility.Visible;
                //popup.InvalidateMeasure();
                //popup.LayoutManager.ExecuteLayoutPass();
            }
            else
            {
                isMouseDownHost = false;
                if (Popup.PlacementTarget == this)
                {
                    Popup.Visibility = Visibility.Collapsed;
                    popup.PlacementTarget = null;
                }
                if (CPF.Platform.Application.DisablePopupClose)
                {
                    return;
                }
                Popup.Children.Remove(DropDownPanel);
            }
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(SelectionItemTemplate))
        //    {
        //        if (selectionItem != null)
        //        {
        //            Children.Remove(selectionItem);
        //            selectionItem.Dispose();
        //            selectionItem = null;
        //        }
        //        var n = newValue as UIElementTemplate<SelectionItem>;
        //        if (n != null && SelectedIndexs.Count > 0)
        //        {
        //            selectionItem = n.CreateElement();
        //            Children.Add(selectionItem);
        //            selectionItem.DataContext = SelectedItems;
        //            selectionItem.Content = SelectedItems;
        //        }
        //    }
        //    else if (propertyName == nameof(IsDropDownOpen))
        //    {
        //        var n = (bool)newValue;
        //        if (n)
        //        {
        //            //var p = PointToScreen(new Point(0, ActualSize.Height));
        //            //Popup.MarginTop = p.Y / Root.Scaling;
        //            //Popup.MarginLeft = p.X / Root.Scaling;
        //            Popup.styleSheet = Root.StyleSheet;
        //            popup.Placement = PlacementMode.Padding;
        //            popup.MarginLeft = 0;
        //            popup.MarginTop = "100%";
        //            popup.Height = "auto";
        //            Popup.Children.Add(DropDownPanel);
        //            popup.PlacementTarget = this;
        //            popup.StaysOpen = true;
        //            Popup.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            Popup.Visibility = Visibility.Collapsed;
        //            popup.PlacementTarget = null;
        //            Popup.Children.Remove(DropDownPanel);
        //        }
        //    }
        //}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (e.Key == Keys.Enter)
                {
                    IsDropDownOpen = !IsDropDownOpen;
                }
                else if (e.Key == Keys.Down)
                {
                    IsDropDownOpen = true;
                }
            }
        }

        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    base.OnLostFocus(e);
        //    if (!e.Handled)
        //    {
        //        IsDropDownOpen = false;
        //    }
        //}

        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(Background), new UIPropertyMetadataAttribute((ViewFill)"234,234,234", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(BorderFill), new UIPropertyMetadataAttribute((ViewFill)"172,172,172", UIPropertyOptions.AffectsRender));
            overridePropertys.Override(nameof(Focusable), new PropertyMetadataAttribute(true));
            //overridePropertys.Override(nameof(ZIndex), new UIPropertyMetadataAttribute(1, UIPropertyOptions.AffectsArrange));
            overridePropertys.Override(nameof(Width), new UIPropertyMetadataAttribute((FloatField)100, UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(Height), new UIPropertyMetadataAttribute((FloatField)20, UIPropertyOptions.AffectsMeasure));
            overridePropertys.Override(nameof(ItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<ListBoxItem>)new ListBoxItem { Width = "100%" }));
            overridePropertys.Override(nameof(SelectionMethod), new PropertyMetadataAttribute(SelectionMethod.MouseUp));
            //overridePropertys.Override(nameof(SelectionItemTemplate), new PropertyMetadataAttribute((UIElementTemplate<SelectionItem>)new SelectionItem()));
        }
    }
}
