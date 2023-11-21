using System;
using System.Collections.Generic;
using System.Text;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using CPF;

namespace ConsoleApp1
{
    public class TreeViewItemTemplate : TreeViewItem
    {
        protected override void InitializeComponent()
        {//模板定义
            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var b1 = this[nameof(Header)] <= DisplayMemberPath;
            }
            if (!string.IsNullOrWhiteSpace(ItemsMemberPath))
            {
                var b2 = this[nameof(Items)] <= ItemsMemberPath;
            }


            var panel = ItemsPanel.CreateElement();
            panel.Name = "itemsPanel";
            panel.PresenterFor = this;
            panel.MarginLeft = 20;
            //var bb = panel[nameof(Visibility)] <= this[nameof(IsExpanded), a => (bool)a ? Visibility.Visible : Visibility.Collapsed];
            panel[nameof(Visibility)] = (this, nameof(IsExpanded), a => (bool)a ? Visibility.Visible : Visibility.Collapsed);
            Children.Add(new StackPanel
            {
                MarginLeft = 0,
                Orientation = Orientation.Vertical,
                PresenterFor = this,
                Name = "panel",
                Children =
                {
                   new StackPanel
                   {
                       Name="treeViewItem",
                       PresenterFor=this,
                       MarginLeft=0,
                       Orientation= Orientation.Horizontal,
                       Children={
                            new Polygon
                            {
                                IsAntiAlias=true,
                                MarginLeft=3,
                                MarginTop=5,
                                Width=12,
                                RenderTransformOrigin=new PointField("30%","70%"),
                                Points={new Point(2,2),new Point(2,10),new Point(6,6), },
                                Bindings={
                                    { nameof(Polygon.RenderTransform),nameof(IsExpanded),3,BindingMode.OneWay,a=>(bool)a?new RotateTransform(45):Transform.Identity},
                                    { nameof(Visibility),nameof(HasItems),3,BindingMode.OneWay,a=>(bool)a?Visibility.Visible:Visibility.Collapsed }
                                },
                                Commands={ {nameof(MouseDown),(s,e)=> { ((RoutedEventArgs)e).Handled = true; IsExpanded = !IsExpanded; } } },
                                Triggers={
                                    new Trigger{ Property=nameof(IsMouseOver), Setters = { {nameof(Shape.StrokeFill),"4,124,205" } } }

                                }
                            },
                            new ContentControl{MarginLeft=3 ,Bindings={ {nameof(ContentControl.Content),nameof(TreeViewItem.Header),3 }, {nameof(ContentControl.ContentTemplate),nameof(TreeViewItem.HeaderTemplate),3 } } }
                       },
                       Triggers={
                           new Trigger { Property = nameof(IsMouseOver), PropertyConditions = a => (bool)a && !IsSelected, Setters = { { nameof(Background), "232,242,252" } } },

                       },
                       Commands={ {nameof(MouseDown),(s,e)=> { SingleSelect(); } } },
                   },
                   IsExpanded? panel:null,
                },
            });
            Children.Add(new Line
            {//竖线
                IsAntiAlias = true,
                UseLayoutRounding = true,
                StrokeStyle = new Stroke(1, DashStyles.Dot),
                IsHitTestVisible = false,
                StartPoint = new Point(1, 5),
                EndPoint = new Point(1, 50),
                MarginLeft = 7,
                MarginTop = 12,
                Bindings =
                {
                    { nameof(Visibility), nameof(LineVisibility), this },
                    { nameof(EndPoint), nameof(EndPoint), this }
                },
            });
            Children.Add(
            new Line
            {
                StartPoint = new Point(0, 10),
                EndPoint = new Point(15, 10),
                MarginLeft = -11,
                IsHitTestVisible = false,
                //IsAntiAlias = true,
                UseLayoutRounding = true,
                StrokeStyle = new Stroke(1, DashStyles.Dot),
                Bindings = { { nameof(Visibility), nameof(HLineVisibility), this } }
            });

            this.Triggers.Add(new Trigger { Property = nameof(IsSelected), PropertyConditions = a => (bool)a, TargetRelation = Relation.Me.Find(a => a.Name == "treeViewItem" && a.PresenterFor == this), Setters = { { nameof(Background), "255,255,255" } } });
            Commands.Add(nameof(HasItems), (s, e) => SetLineVisibility());
            Commands.Add(nameof(IsExpanded), (s, e) => SetLineVisibility());
        }
        [PropertyChanged(nameof(IsExpanded))]
        void OnIsExpanded(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var itemsPanel = FindPresenterByName<Panel>("itemsPanel");
            if ((bool)newValue && itemsPanel.Root == null)
            {
                FindPresenterByName<Panel>("panel").Children.Add(itemsPanel);
            }
        }

        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();
            if (Items.Count > 0)
            {
                EndPoint = new Point(1, (ItemsHost.Children[Items.Count - 1]).ActualOffset.Y + 20);
            }
            else
            {
                EndPoint = new Point(1, ActualSize.Height - 23);
            }
            HLineVisibility = (ParentItem is TreeViewItem) ? Visibility.Visible : Visibility.Collapsed;
        }

        public Point EndPoint
        {
            get { return GetValue<Point>(); }
            set { SetValue(value); }
        }

        [PropertyMetadata(Visibility.Collapsed)]
        public Visibility LineVisibility
        {
            get { return GetValue<Visibility>(); }
            set { SetValue(value); }
        }
        [PropertyMetadata(Visibility.Collapsed)]
        public Visibility HLineVisibility
        {
            get { return GetValue<Visibility>(); }
            set { SetValue(value); }
        }

        void SetLineVisibility()
        {
            if (HasItems && IsExpanded)
            {
                LineVisibility = Visibility.Visible;
            }
            else
            {
                LineVisibility = Visibility.Collapsed;
            }
        }
    }
}
