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

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://$safeprojectname$/Stylesheet1.css")]//用于设计的时候加载样式
    public class DragListBox : Control
    {
        //模板定义
        protected override void InitializeComponent()
        {
            Size = new SizeField(200, 180);
            Children.Add(new ListBox
            {
                Name = "List1",
                PresenterFor = this,
                Size = SizeField.Fill,
                Items = {
                    new ListBoxItem{ Content = "266666" },
                    new ListBoxItem{ Content = "366666" },
                    new ListBoxItem{ Content  = "466666" },
                    new ListBoxItem{ Content = "566666" },
                },
                Commands = {
                    { nameof(ListBox.MouseDown),(s,e1)=>{
                        var e = e1 as MouseEventArgs;
                        var list = s as ListBox;
                        if (e.LeftButton == MouseButtonState.Pressed){
                            var item = list.SelectedItems.FirstOrDefault() as ListBoxItem;
                            if (item!=null){
                                list.Items.Remove(item);
                                Tag = item;
                                this.Children.Add(item);
                                item.InvalidateMeasure();
                                item.InvalidateArrange();
                            }
                        }
                    } },
                    { nameof(ListBox.MouseMove),(s,e1)=>{
                        if (!(Tag is UIElement)){
                            return;
                        }
                        var e = e1 as MouseEventArgs;
                        var list = s as ListBox;
                        if (e.LeftButton == MouseButtonState.Pressed){
                            var item = Tag as UIElement;
                            item.MarginTop = e.Location.Y;
                        }
                    } }
                }
            });

        }

        protected override Size ArrangeOverride(in Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }

    public class TestTextBlock : TextBlock
    {
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
            if (propertyName == nameof(MarginTop))
            {

            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
