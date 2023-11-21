using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace CPF.Controls
{
    /// <summary>
    /// DataGrid内部ScrollViewer
    /// </summary>
    [Description("DataGridColumn模板"), Browsable(false)]
    public class DataGridScrollViewer : ScrollViewer
    {
        protected override void InitializeComponent()
        {
            var col = new ColumnDefinition { };
            var row = new RowDefinition { };
            Children.Add(new Grid
            {
                Width = "100%",
                Height = "100%",
                ColumnDefinitions = { new ColumnDefinition { }, col },
                RowDefinitions = { new RowDefinition { Height = "auto" }, new RowDefinition { }, row },
                Children = {
                    {new StackPanel{ Name="PART_ColumnHeadersPresenter",PresenterFor=this,Orientation= Orientation.Horizontal,MarginLeft=0, } },
                    {new Border{Name="contentPresenter",Width="100%",Height="100%", BorderStroke="0", PresenterFor=this },0,1 },
                    {new ScrollBar{ Width=20, Height="100%",Cursor=Cursors.Arrow, IncreaseLargeChanged=PageDown, DecreaseLargeChanged=PageUp, IncreaseSmallChange=LineDown, DecreaseSmallChange=LineUp,Bindings={
                            { nameof(ScrollBar.Maximum),nameof(VerticalMaximum),2,BindingMode.TwoWay },
                            { nameof(ScrollBar.Value),nameof(VerticalOffset),2,BindingMode.TwoWay},
                            { nameof(ScrollBar.ViewportSize),nameof(VerticalViewportSize),2 },
                            { nameof(Visibility),nameof(ComputedVerticalScrollBarVisibility),2},
                            { nameof(IsEnabled),nameof(HorizontalScrollBarVisibility),2,BindingMode.OneWay,a=>((ScrollBarVisibility)a)!=ScrollBarVisibility.Disabled },
                            { nameof(Visibility),nameof(Width),col,BindingMode.OneWayToSource,null,a=>((Visibility)a)==Visibility.Visible?(GridLength)((FloatField)Binding.Current.Owner.GetValue(nameof(ScrollBar.Width))).Value:(GridLength)0}
                        } },1,1 },
                    {new ScrollBar{ Orientation=Orientation.Horizontal,Width="100%", Height=20,Cursor=Cursors.Arrow, IncreaseLargeChanged=PageRight, DecreaseLargeChanged=PageLeft, IncreaseSmallChange=LineRight, DecreaseSmallChange=LineLeft,Bindings={
                            { nameof(ScrollBar.Maximum),nameof(HorizontalMaximum),2,BindingMode.TwoWay },
                            { nameof(ScrollBar.Value),nameof(HorizontalOffset),2,BindingMode.TwoWay},
                            { nameof(ScrollBar.ViewportSize),nameof(HorizontalViewportSize),2 },
                            { nameof(Visibility),nameof(ComputedHorizontalScrollBarVisibility),2},
                            { nameof(IsEnabled),nameof(VerticalScrollBarVisibility),2,BindingMode.OneWay,a=>((ScrollBarVisibility)a)!=ScrollBarVisibility.Disabled },
                            { nameof(Visibility),nameof(Height),row,BindingMode.OneWayToSource,null,a=>((Visibility)a)==Visibility.Visible?(GridLength)((FloatField)Binding.Current.Owner.GetValue(nameof(ScrollBar.Height))).Value:(GridLength)0  }
                        } },0,2 },
                }
            });
        }

        internal Panel PART_ColumnHeadersPresenter;
        internal UIElement contentPresenter;
        protected override void OnInitialized()
        {
            PART_ColumnHeadersPresenter = FindPresenter<Panel>().FirstOrDefault(a => a.Name == "PART_ColumnHeadersPresenter");
            if (!PART_ColumnHeadersPresenter)
            {
                throw new Exception("需要Name为PART_ColumnHeadersPresenter的元素");
            }
            base.OnInitialized();
            contentPresenter = FindPresenter().FirstOrDefault(a => a.Name == "contentPresenter");
        }

        [PropertyChanged(nameof(HorizontalOffset))]
        void RegisterHorizontalOffset(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            PART_ColumnHeadersPresenter.MarginLeft = -(float)newValue;
        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(HorizontalOffset))
        //    {
        //        PART_ColumnHeadersPresenter.MarginLeft = -(float)newValue;
        //    }
        //}
        protected override void OnOverrideMetadata(OverrideMetadata overridePropertys)
        {
            base.OnOverrideMetadata(overridePropertys);
            overridePropertys.Override(nameof(ClipToBounds), new UIPropertyMetadataAttribute(true, UIPropertyOptions.AffectsRender));
        }
    }
}
