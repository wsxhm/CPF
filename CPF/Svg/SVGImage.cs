using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF.Drawing;

namespace CPF.Svg
{
	public class SVGImage : Control
	{
		public enum eSizeType
		{
			None,
			ContentToSizeNoStretch,
			ContentToSizeStretch,
			SizeToContent,
		}

		public static DependencyProperty SizeTypeProperty = DependencyProperty.Register("SizeType",
			typeof(eSizeType), 
			typeof(SVGImage), 
			new FrameworkPropertyMetadata(eSizeType.ContentToSizeNoStretch, 
				FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnSizeTypeChanged)));

		static void OnSizeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SVGImage ctrl = d as SVGImage;
			ctrl.RecalcImage();
		}

		public eSizeType SizeType
		{
			get { return (eSizeType)GetValue(SizeTypeProperty); }
			set { SetValue(SizeTypeProperty, value); }
		}
		public static DependencyProperty ImageSourcePoperty = DependencyProperty.Register("ImageSource",
			typeof(Drawing),
			typeof(SVGImage),
			new FrameworkPropertyMetadata(null, 
				FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, 
				new PropertyChangedCallback(OnImageSourceChanged)));
		
		static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((SVGImage)d).SetImage(e.NewValue as Drawing);
		}
		public Drawing ImageSource
		{
			get { return (Drawing)GetValue(ImageSourcePoperty); }
			set { SetValue(ImageSourcePoperty, value); }
		}

        public static DependencyProperty ImageSourceFilePoperty = DependencyProperty.Register("ImageSourceFile",
            typeof(string),
            typeof(SVGImage),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnImageSourceFileChanged)));

        static void OnImageSourceFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue as string;
            if(newValue != null)
            {
                ((SVGImage)d).SetImage(e.NewValue as string);
            }
        }
        public string ImageSourceFile
        {
            get { return (string)GetValue(ImageSourceFilePoperty); }
            set { SetValue(ImageSourceFilePoperty, value); }
        }

        public static DependencyProperty FillBrushPoperty = DependencyProperty.Register("FillBrush",
            typeof(Brush),
            typeof(SVGImage),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnFillBrushChanged)));

        static void OnFillBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SVGImage)d).SetBrush(e.NewValue as Brush);
        }
        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushPoperty); }
            set { SetValue(FillBrushPoperty, value); }
        }

        Drawing m_drawing;
		TranslateTransform m_offsetTransform = new TranslateTransform();
		ScaleTransform m_scaleTransform = new ScaleTransform();

		static SVGImage()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SVGImage), new FrameworkPropertyMetadata(typeof(SVGImage)));
			ClipToBoundsProperty.OverrideMetadata(typeof(SVGImage), new FrameworkPropertyMetadata(true));
			SnapsToDevicePixelsProperty.OverrideMetadata(typeof(SVGImage), new FrameworkPropertyMetadata(true));
		}
		public SVGImage()
		{
			ClipToBounds = true;
			SnapsToDevicePixels = true;
		}


        SVGRender _render;
        SVGRender render
        {
            get
            {
                if (_render == null)
                    _render = new SVGRender();
                return _render;
            }
        }
        public void SetImage(string filePath)
		{
			SetImage(render.LoadDrawing(filePath));
		}
        private void SetBrush(Brush brush)
        {
            render.SetBrush(brush);
            SetImage(render.LoadDrawing());
        }
		public void SetImage(Drawing drawing)
		{
            if (drawing == null) return;

			m_drawing = drawing;
			InvalidateVisual();
			if (m_drawing != null && SizeType == eSizeType.SizeToContent)
				InvalidateMeasure();
			RecalcImage();
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			RecalcImage();
			InvalidateVisual();
		}
		
		// Notice TemplateBinding BackGround must be removed from the Border in the default template (or remove Border from the template)
		// Border renders the background AFTER the child render has been called
		// http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/1575d2af-8e86-4085-81b8-a8bf24268e51/?prof=required
		protected override void OnRender(DrawingContext dc)
		{
			//base.OnRender(dc);
			if (Background != null)
				dc.DrawRectangle(Background, null, new Rect(0,0, ActualWidth, ActualHeight));
			if (m_drawing == null)
				return;
			dc.PushTransform(m_offsetTransform);
			dc.PushTransform(m_scaleTransform);
			dc.DrawDrawing(m_drawing);
			dc.Pop();
			dc.Pop();
		}
		void RecalcImage()
		{
			if (m_drawing == null)
				return;
			
			Rect r = m_drawing.Bounds;
			if (SizeType == eSizeType.None)
			{
				m_scaleTransform.ScaleX = 1;
				m_scaleTransform.ScaleY = 1;
				switch (HorizontalContentAlignment)
				{
					case System.Windows.HorizontalAlignment.Center:
						m_offsetTransform.X = ActualWidth / 2 - r.Width / 2 - r.Left;
						break;
					case System.Windows.HorizontalAlignment.Right:
						m_offsetTransform.X = ActualWidth - r.Right;
						break;
					default:
						m_offsetTransform.X = -r.Left; // move to left by default
						break;
				}
				switch (VerticalContentAlignment)
				{
					case System.Windows.VerticalAlignment.Center:
						m_offsetTransform.Y = ActualHeight / 2 - r.Height / 2;
						break;
					case System.Windows.VerticalAlignment.Bottom:
						m_offsetTransform.Y = ActualHeight - r.Height - r.Top;
						break;
					default:
						m_offsetTransform.Y = -r.Top; // move to top by default
						break;
				}
				return;
			}
			if (SizeType == eSizeType.ContentToSizeNoStretch)
			{
				SizeToContentNoStretch(HorizontalContentAlignment, VerticalContentAlignment);
				return;
			}
			if (SizeType == eSizeType.ContentToSizeStretch)
			{
				double xscale = this.ActualWidth / r.Width;
				double yscale = this.ActualHeight / r.Height;
				m_scaleTransform.CenterX = r.Left;
				m_scaleTransform.CenterY = r.Top;
				m_scaleTransform.ScaleX = xscale;
				m_scaleTransform.ScaleY = yscale;

				m_offsetTransform.X = -r.Left;
				m_offsetTransform.Y = -r.Top;
				return;
			}
			if (SizeType == eSizeType.SizeToContent)
			{
				if (r.Width > ActualWidth || r.Height > ActualHeight)
					SizeToContentNoStretch(HorizontalAlignment.Left, VerticalAlignment.Top);
				else
				{
					m_scaleTransform.CenterX = r.Left;
					m_scaleTransform.CenterY = r.Top;
					m_scaleTransform.ScaleX = 1;
					m_scaleTransform.ScaleY = 1;

					m_offsetTransform.X = -r.Left; // move to left by default
					m_offsetTransform.Y = -r.Top; // move to top by default
				}
				return;
			}
		}
		void SizeToContentNoStretch(HorizontalAlignment hAlignment, VerticalAlignment vAlignment)
		{
			Rect r = m_drawing.Bounds;
			double xscale = this.ActualWidth / r.Width;
			double yscale = this.ActualHeight / r.Height;
			double scale = xscale;
			if (scale > yscale)
				scale = yscale;

			m_scaleTransform.CenterX = r.Left;
			m_scaleTransform.CenterY = r.Top;
			m_scaleTransform.ScaleX = scale;
			m_scaleTransform.ScaleY = scale;

			m_offsetTransform.X = -r.Left;
			if (scale < xscale)
			{
				switch (HorizontalContentAlignment)
				{
					case System.Windows.HorizontalAlignment.Center:
						double width = r.Width*scale;
						m_offsetTransform.X = ActualWidth / 2 - width / 2 - r.Left;
						break;
					case System.Windows.HorizontalAlignment.Right:
						m_offsetTransform.X = ActualWidth - r.Right * scale;
						break;
				}
			}
			m_offsetTransform.Y = -r.Top;
			if (scale < yscale)
			{
				switch (VerticalContentAlignment)
				{
					case System.Windows.VerticalAlignment.Center:
						double height = r.Height*scale;
						m_offsetTransform.Y = ActualHeight / 2 - height / 2 - r.Top;
						break;
					case System.Windows.VerticalAlignment.Bottom:
						m_offsetTransform.Y = ActualHeight - r.Height * scale - r.Top;
						break;
				}
			}
		}
		protected override Size MeasureOverride(Size constraint)
		{	
			Size result = base.MeasureOverride(constraint);
			if (SizeType == eSizeType.SizeToContent)
			{
				if (m_drawing != null && !m_drawing.Bounds.Size.IsEmpty)
					result = m_drawing.Bounds.Size;
			}			
			if (constraint.Width > 0 && constraint.Width < result.Width)
				result.Width = constraint.Width;
			if (constraint.Height > 0 && constraint.Height < result.Height)
				result.Height = constraint.Height;
			//Console.WriteLine("MasureOverride({0}), SizeType = {1}, result {2}", constraint.ToString(), SizeType.ToString(), result.ToString());
			return result;
		}
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			Size result = base.ArrangeOverride(arrangeBounds);
			if (SizeType == eSizeType.SizeToContent)
			{
				if (m_drawing != null && !m_drawing.Bounds.Size.IsEmpty)
					result = m_drawing.Bounds.Size;
			}						 
			if (arrangeBounds.Width > 0 && arrangeBounds.Width < result.Width)
				result.Width = arrangeBounds.Width;
			if (arrangeBounds.Height > 0 && arrangeBounds.Height < result.Height)
				result.Height = arrangeBounds.Height;
			//Console.WriteLine("ArrangeOverride({0}), SizeType = {1}, result {2}", arrangeBounds.ToString(), SizeType.ToString(), result.ToString());
			return result;
		}

	}
}
