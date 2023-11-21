using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace CPF.Skia
{
    public class SkiaPdf
    {
        /// <summary>
        /// 从指定控件生成PDF
        /// </summary>
        /// <param name="control">要生成的控件</param>
        /// <param name="Path">保存路径</param>
        public static void CreatePdf(UIElement control, string Path)
        {
            using (var document = SkiaSharp.SKDocument.CreatePdf(Path))
            {
                using (var canvas = document.BeginPage(control.ActualSize.Width, control.ActualSize.Height))
                {
                    var context = new SkiaDrawingContext(canvas, (SkiaDrawingFactory)DrawingFactory.Default);
                    if (control.Root.LayoutManager.VisibleUIElements.Element == control)
                    {
                        control.Root.RenderUIElement(context, control.Root.LayoutManager.VisibleUIElements, control.RenderBounds);
                    }
                    else
                    {
                        List<VisibleUIElement> vEles = new List<VisibleUIElement>();
                        Action<VisibleUIElement> gwl = null;
                        gwl = (Children) =>
                        {
                            if (Children.Element == control)
                            {
                                vEles.Add(Children);
                            }
                            Children.Children.ForEach(gwl);
                        };
                        gwl(control.Root.LayoutManager.VisibleUIElements);

                        //找到就全部使用pdf context渲染一边
                        foreach (var item in vEles)
                        {
                            control.Root.RenderUIElement(context, item, control.RenderBounds);
                        }
                    }
                    document.EndPage();
                }
                document.Close();
            }
        }
    }
}
