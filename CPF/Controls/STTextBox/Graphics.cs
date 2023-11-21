using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Drawing
{
    public class Graphics
    {
        DrawingContext drawingContext;
        public Graphics(DrawingContext dc) {
            drawingContext = dc;
        }
        public void FillRectangle(Brush brush,float x,float y, float width,float height) {
            drawingContext.FillRectangle(brush, new Rect(x,y,width,height));
        }
        public void DrawLine(Brush brush,float x,float y,float x1,float y1,int stroke = 1)
        {
            drawingContext.DrawLine(new Stroke(stroke), brush,new Point(x,y),new Point(x1,y1));
        }
        public void DrawRectangle(Brush brush,Rect rect)
        {
            drawingContext.DrawRectangle(brush, new Stroke(1), rect);
        }
        public void DrawPath(Brush brush,PathGeometry path)
        {
            drawingContext.DrawPath(brush,new Stroke(1), path);
        }

        public void FillPath(Brush brush, PathGeometry path)
        {
            drawingContext.FillPath(brush, path);
        }
        public void FillRectangle(Brush brush,Rect rect)
        {
            drawingContext.FillRectangle(brush, rect);
        }
        public void FillRectangle(Brush brush,float x,float y,int width,int height)
        {
            drawingContext.FillRectangle(brush,new Rect(x,y,width,height));
        }
        public void DrawString(string str,Font font, Brush brush,float x, float y, StringFormat stringFormat)
        {
            drawingContext.DrawString(new Point(x,y),brush,str,font, stringFormat.Alignment);
        }
        public void DrawString(string str, Font font, Brush brush, float x, float y)
        {
            drawingContext.DrawString(new Point(x, y), brush, str, font);
        }
        public void DrawString(string str, Font font, Brush brush, Rect rect, StringFormat stringFormat)
        {
            drawingContext.DrawString(new Point(rect.X, rect.Y), brush, str, font,stringFormat.Alignment, rect.Width,
                default,rect.Height);
        }

        public static Graphics FromImage(Bitmap bmp)
        {
            return new Graphics(DrawingContext.FromBitmap(bmp));
        }

        public Size MeasureString(string strText, Font ft, int v, StringFormat m_sf)
        {
            return Platform.Application.GetDrawingFactory().MeasureString(strText, ft,v);
        }

        public void DrawImage(Image image, Rect rect)
        {
            drawingContext.DrawImage(image, rect, rect);
        }

        internal void SetClip(Rect rect)
        {
        }
    }
}
