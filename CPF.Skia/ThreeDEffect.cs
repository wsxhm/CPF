using CPF.Drawing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Skia
{
    /// <summary>
    /// 3D变换只能支持Skia
    /// </summary>
    public class ThreeDEffect : CPF.Effects.Effect
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
