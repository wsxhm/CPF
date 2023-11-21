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
#if !Net4
using CPF.Skia;
using SkiaSharp;
#endif

namespace ConsoleApp1
{
    [CPF.Design.DesignerLoadStyle("res://ConsoleApp1/Stylesheet1.css")]//用于设计的时候加载样式
    public class Page1 : Control
    {
        //模板定义
        protected override void InitializeComponent()
        {
            Height = 320;
            Width = 599;
            IsAntiAlias = true;
            Children.Add(new TreeView
            {
                MarginLeft = 25,
                MarginTop = 28,
                Width = 160,
                Height = 166,
                Items =
                {
                    new TreeViewItem
                    {
                //Visibility = Visibility.Collapsed,
                        Header="342sdtest",
                        IsExpanded=true,
                        Items =
                        {
                            new TreeViewItem
                            {
                                Header="dgssaa"
                            },
                            new TreeViewItem
                            {
                                Header="fgfddgssaa"
                            },
                        }
                    },
                    new TreeViewItem
                    {
                        Header="34fgdgd2sd"
                    },
                    new TreeViewItem
                    {
                        Header="342sd"
                    },
                    new TreeViewItem
                    {
                        Header="34fgdgd2sd"
                    },
                    new TreeViewItem
                    {
                        Header="34fgd2sd"
                    },
                    new TreeViewItem
                    {
                        Header="34sfgdgd2sd"
                    },
                }
            });
            Children.Add(new TreeView
            {
                PresenterFor = this,
                Name = nameof(treeView),
                MarginLeft = 326,
                MarginTop = 41,
                Height = 244,
                Width = 224,
                DisplayMemberPath = "Text",
                ItemsMemberPath = "Nodes",
                ItemTemplate = typeof(TreeViewItemTemplate),
                Bindings =
                {
                    {
                        nameof(TreeView.Items),
                        nameof(MainModel.Nodes)
                    }
                }
            });
            Children.Add(new TextBlock
            {
                MarginLeft = 114,
                MarginTop = 6,
                Text = "如果页面多，设计成独立页面，可以提高界面打开速度，切换到该页面之后才会加载内部组件",
            });
        }
        TreeView treeView;
#if !Net4
        protected override void OnInitialized()
        {
            base.OnInitialized();
            treeView = FindPresenterByName<TreeView>(nameof(treeView));
            //float xCenter = 244 / 2;
            //float yCenter = 224 / 2;

            //// Translate center to origin
            //SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);

            //// Use 3D matrix for 3D rotations and perspective
            //SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
            //matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, 150));
            //matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, 0));
            //matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, 0));

            //SKMatrix44 perspectiveMatrix = SKMatrix44.CreateIdentity();
            //perspectiveMatrix[3, 2] = -1 / (float)250;
            //matrix44.PostConcat(perspectiveMatrix);

            //// Concatenate with 2D matrix
            //SKMatrix.PostConcat(ref matrix, matrix44.Matrix);

            //// Translate back to center
            //SKMatrix.PostConcat(ref matrix,
            //    SKMatrix.MakeTranslation(xCenter, yCenter));
            //treeView.RenderTransform = new MatrixTransform(matrix.ToMatrix());
        }
        Image bitmap = "res://ConsoleApp1/icon.png";
        //protected override void OnRender(DrawingContext dc)
        //{
        //    base.OnRender(dc);

        //    float xCenter = ActualSize.Width / 2;
        //    float yCenter = ActualSize.Height / 1;

        //    // Translate center to origin
        //    //SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);
        //    var m = dc.Transform;
        //    m.TranslatePrepend(-xCenter, -yCenter);
        //    SKMatrix matrix = m.ToMatrix();

        //    // Use 3D matrix for 3D rotations and perspective
        //    SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
        //    matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, 150));
        //    matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, 0));
        //    matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, 0));

        //    SKMatrix44 perspectiveMatrix = SKMatrix44.CreateIdentity();
        //    perspectiveMatrix[3, 2] = -1 / (float)250;
        //    matrix44.PostConcat(perspectiveMatrix);

        //    // Concatenate with 2D matrix
        //    SKMatrix.PostConcat(ref matrix, matrix44.Matrix);

        //    // Translate back to center
        //    SKMatrix.PostConcat(ref matrix,
        //        SKMatrix.MakeTranslation(xCenter, yCenter));
        //    //dc.Transform = matrix.ToMatrix();
        //    var SKCanvas = (SKCanvas)dc.GetPropretyValue("SKCanvas");
        //    SKCanvas.SetMatrix(matrix);

        //    float xBitmap = xCenter - bitmap.Width / 2;
        //    float yBitmap = yCenter - bitmap.Height / 2;
        //    dc.DrawImage(bitmap, new Rect(xBitmap, yBitmap, bitmap.Width, bitmap.Height), new Rect(0, 0, bitmap.Width, bitmap.Height));
        //}

#endif
    }
}
