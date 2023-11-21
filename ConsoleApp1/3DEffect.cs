using CPF.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
#if !Net4
using SkiaSharp;
#endif

namespace ConsoleApp1
{
    /// <summary>
    /// 3D变换只能支持Skia
    /// </summary>
    public class ThreeDEffect1 : CPF.Effects.Effect
    {
        /// <summary>
        /// 0到1
        /// </summary>
        [CPF.PropertyMetadata(0.5f)]
        public float Value
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 翻转方向
        /// </summary>
        [CPF.PropertyMetadata(TaperSide.Left)]
        public TaperSide TaperSide
        {
            get { return GetValue<TaperSide>(); }
            set { SetValue(value); }
        }
        [CPF.PropertyMetadata(TaperCorner.Both)]
        public TaperCorner TaperCorner
        {
            get { return GetValue<TaperCorner>(); }
            set { SetValue(value); }
        }

        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
#if !Net4
            float taperFraction = Value;
            TaperSide side = TaperSide;
            var size = new SKSize(bitmap.Width, bitmap.Height);
            SKMatrix taperMatrix =
                TaperTransform.Make(size,
                                    TaperSide, TaperCorner, taperFraction);

            //// Display the matrix in the lower-right corner
            //SKSize matrixSize = matrixDisplay.Measure(taperMatrix);

            //matrixDisplay.Paint(canvas, taperMatrix,
            //    new SKPoint(info.Width - matrixSize.Width,
            //                info.Height - matrixSize.Height));

            // Center bitmap on canvas
            float x = 0;
            float y = 0;

            SKMatrix matrix = SKMatrix.MakeTranslation(-x, -y);
            SKMatrix.PostConcat(ref matrix, taperMatrix);
            SKMatrix.PostConcat(ref matrix, SKMatrix.MakeTranslation(x, y));

            var canvas = (dc as CPF.Skia.SkiaDrawingContext).SKCanvas;
            var mat = canvas.TotalMatrix;

            dc.AntialiasMode = AntialiasMode.AntiAlias;
            canvas.SetMatrix(mat.PreConcat(matrix));
            canvas.DrawBitmap((bitmap.BitmapImpl as CPF.Skia.SkiaBitmap).Bitmap, x, y);
#endif
        }
    }
    public enum TaperSide { Left, Top, Right, Bottom }

    public enum TaperCorner { LeftOrTop, RightOrBottom, Both }
#if !Net4

    static class TaperTransform
    {
        public static SKMatrix Make(SKSize size, TaperSide taperSide, TaperCorner taperCorner, float taperFraction)
        {
            SKMatrix matrix = SKMatrix.MakeIdentity();

            switch (taperSide)
            {
                case TaperSide.Left:
                    matrix.ScaleX = taperFraction;
                    matrix.ScaleY = taperFraction;
                    matrix.Persp0 = (taperFraction - 1) / size.Width;

                    switch (taperCorner)
                    {
                        case TaperCorner.RightOrBottom:
                            break;

                        case TaperCorner.LeftOrTop:
                            matrix.SkewY = size.Height * matrix.Persp0;
                            matrix.TransY = size.Height * (1 - taperFraction);
                            break;

                        case TaperCorner.Both:
                            matrix.SkewY = (size.Height / 2) * matrix.Persp0;
                            matrix.TransY = size.Height * (1 - taperFraction) / 2;
                            break;
                    }
                    break;

                case TaperSide.Top:
                    matrix.ScaleX = taperFraction;
                    matrix.ScaleY = taperFraction;
                    matrix.Persp1 = (taperFraction - 1) / size.Height;

                    switch (taperCorner)
                    {
                        case TaperCorner.RightOrBottom:
                            break;

                        case TaperCorner.LeftOrTop:
                            matrix.SkewX = size.Width * matrix.Persp1;
                            matrix.TransX = size.Width * (1 - taperFraction);
                            break;

                        case TaperCorner.Both:
                            matrix.SkewX = (size.Width / 2) * matrix.Persp1;
                            matrix.TransX = size.Width * (1 - taperFraction) / 2;
                            break;
                    }
                    break;

                case TaperSide.Right:
                    matrix.ScaleX = 1 / taperFraction;
                    matrix.Persp0 = (1 - taperFraction) / (size.Width * taperFraction);

                    switch (taperCorner)
                    {
                        case TaperCorner.RightOrBottom:
                            break;

                        case TaperCorner.LeftOrTop:
                            matrix.SkewY = size.Height * matrix.Persp0;
                            break;

                        case TaperCorner.Both:
                            matrix.SkewY = (size.Height / 2) * matrix.Persp0;
                            break;
                    }
                    break;

                case TaperSide.Bottom:
                    matrix.ScaleY = 1 / taperFraction;
                    matrix.Persp1 = (1 - taperFraction) / (size.Height * taperFraction);

                    switch (taperCorner)
                    {
                        case TaperCorner.RightOrBottom:
                            break;

                        case TaperCorner.LeftOrTop:
                            matrix.SkewX = size.Width * matrix.Persp1;
                            break;

                        case TaperCorner.Both:
                            matrix.SkewX = (size.Width / 2) * matrix.Persp1;
                            break;
                    }
                    break;
            }
            return matrix;
        }
    }
#endif

    /// <summary>
    /// 3D变换只能支持Skia
    /// </summary>
    public class ThreeDEffect2 : CPF.Effects.Effect
    {
        /// <summary>
        /// -90到90
        /// </summary>
        [CPF.PropertyMetadata(0f)]
        public float X
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// -90到90
        /// </summary>
        [CPF.PropertyMetadata(0f)]
        public float Y
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// -90到90
        /// </summary>
        [CPF.PropertyMetadata(0f)]
        public float Z
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 默认250
        /// </summary>
        [CPF.PropertyMetadata(250f)]
        public float Depth
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 缩放值
        /// </summary>
        [CPF.PropertyMetadata(1f)]
        public float Scale
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
        public override void DoEffect(DrawingContext dc, Bitmap bitmap)
        {
#if !Net4
            // Find center of canvas
            float xCenter = bitmap.Width / 2;
            float yCenter = bitmap.Height / 2;

            // Translate center to origin
            SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);

            // Use 3D matrix for 3D rotations and perspective
            SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, X));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, Y));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, Z));

            SKMatrix44 perspectiveMatrix = SKMatrix44.CreateScale(Scale, Scale, Scale);
            perspectiveMatrix[3, 2] = -1 / Depth;
            matrix44.PostConcat(perspectiveMatrix);

            // Concatenate with 2D matrix
            SKMatrix.PostConcat(ref matrix, matrix44.Matrix);

            // Translate back to center
            SKMatrix.PostConcat(ref matrix,
                SKMatrix.MakeTranslation(xCenter, yCenter));

            //var tr= dc.Transform;
            //tr.ScalePrepend(0.9f, 0.9f);
            //dc.Transform = tr;
            var canvas = (dc as CPF.Skia.SkiaDrawingContext).SKCanvas;
            var mat = canvas.TotalMatrix;
            dc.AntialiasMode = AntialiasMode.AntiAlias;
            canvas.SetMatrix(mat.PreConcat(matrix));

            // Set the matrix and display the bitmap
            //canvas.SetMatrix(matrix);
            float xBitmap = xCenter - bitmap.Width / 2;
            float yBitmap = yCenter - bitmap.Height / 2;

            canvas.DrawBitmap((bitmap.BitmapImpl as CPF.Skia.SkiaBitmap).Bitmap, xBitmap, yBitmap);
#endif
        }
    }

}
