using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;
using CPF.Controls;

namespace CPF
{
    /// <summary>
    /// 纹理图片填充
    /// </summary>
    public class TextureFill : ViewFill, Design.ISerializerCode
    {
        public virtual Image Image
        {
            get { return GetValue<Image>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 纹理图片填充
        /// </summary>
        /// <param name="image"></param>
        public TextureFill(Image image)
        {
            this.Image = image;
        }

        public TextureFill()
        {
        }

        string path;
        /// <summary>
        /// 纹理图片路径，可以是URL或者文件路径或者res://内嵌资源路径  格式：url(img.gif) [no-repeat/repeat(clamp/tile)] [none/fill/uniform/UniformToFill] [x,y,w,h]  需要按顺序
        /// </summary>
        /// <param name="path"></param>
        public TextureFill(string path)
        {
            try
            {
                if (path.StartsWith("url("))
                {
                    var index = path.LastIndexOf(')');
                    Styling.ResourceManager.GetImage(path.Substring(4, index - 4), a =>
                    {
                        Threading.Dispatcher.MainThread.Invoke(() =>
                        {
                            if (a == null)
                            {
                                Image = Styling.ResourceManager.ErrorImage;
                            }
                            else
                            {
                                Image = a;
                            }
                        });

                    });
                    if (path.Length > index + 2)
                    {
                        var temp = path.Substring(index + 2).Split(' ');
                        if (temp.Length > 0 && !string.IsNullOrWhiteSpace(temp[0]))
                        {
                            var t = temp[0].ToLower().Trim();
                            if (t == "no-repeat" || t == "clamp")
                            {
                                WrapMode = WrapMode.Clamp;
                            }
                            else
                            {
                                WrapMode = WrapMode.Tile;
                            }
                        }
                        if (temp.Length > 1 && !string.IsNullOrWhiteSpace(temp[1]))
                        {
                            Stretch = (Stretch)Enum.Parse(typeof(Stretch), temp[1], true);
                        }
                        if (temp.Length > 2 && !string.IsNullOrWhiteSpace(temp[2]))
                        {
                            ImageClip = temp[2];
                        }
                    }
                }
                else
                {
                    Styling.ResourceManager.GetImage(path, a =>
                    {
                        Threading.Dispatcher.MainThread.Invoke(() =>
                        {
                            if (a == null)
                            {
                                Image = Styling.ResourceManager.ErrorImage;
                            }
                            else
                            {
                                Image = a;
                            }
                        });
                    });
                }
            }
            catch (Exception e)
            {
                throw new Exception("图片路径格式不对：" + path + e.Message);
            }
            this.path = path;
        }

        /// <summary>
        /// 图片的裁剪区域
        /// </summary>
        public Rect ImageClip
        {
            get { return GetValue<Rect>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 图片填充模式
        /// </summary>
        [PropertyMetadata(Stretch.None)]
        public Stretch Stretch
        {
            get { return GetValue<Stretch>(); }
            set { SetValue(value); }
        }
        /// <summary>
        /// 平铺模式
        /// </summary>
        public WrapMode WrapMode
        {
            get { return GetValue<WrapMode>(); }
            set { SetValue(value); }
        }

        Bitmap cacheImage;

        [PropertyChanged(nameof(ImageClip))]
        void RegisterImageClip(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            var rect = (Rect)newValue;
            if (rect.X < 0 || rect.Y < 0)
            {
                throw new Exception("x,y不能小于0");
            }
            var img = Image;
            if (cacheImage != null)
            {
                cacheImage.Dispose();
                cacheImage = null;
            }
            if (img != null && rect.Width > 0 && rect.Height > 0)
            {
                cacheImage = new Bitmap((int)rect.Width, (int)rect.Height);
                using (DrawingContext dc = DrawingContext.FromBitmap(cacheImage))
                {
                    dc.Clear(Color.Transparent);
                    dc.DrawImage(img, new Rect(0, 0, rect.Width, rect.Height), rect);
                }
            }
        }

        [PropertyChanged(nameof(Image))]
        void RegisterImage(object newValue, object oldValue, PropertyMetadataAttribute attribute)
        {
            if (cacheImage != null)
            {
                cacheImage.Dispose();
                cacheImage = null;
            }
            Rect rect = ImageClip;
            var img = newValue as Image;
            if (img != null && rect.Width > 0 && rect.Height > 0)
            {
                cacheImage = new Bitmap((int)rect.Width, (int)rect.Height);
                using (DrawingContext dc = DrawingContext.FromBitmap(cacheImage))
                {
                    dc.Clear(Color.Transparent);
                    dc.DrawImage(img, new Rect(0, 0, rect.Width, rect.Height), rect);
                }
            }

        }

        //protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        //{
        //    base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        //    if (propertyName == nameof(ImageClip))
        //    {
        //        var rect = (Rect)newValue;
        //        if (rect.X < 0 || rect.Y < 0)
        //        {
        //            throw new Exception("x,y不能小于0");
        //        }
        //        var img = Image;
        //        if (cacheImage != null)
        //        {
        //            cacheImage.Dispose();
        //            cacheImage = null;
        //        }
        //        if (img != null && rect.Width > 0 && rect.Height > 0)
        //        {
        //            cacheImage = new Bitmap((int)rect.Width, (int)rect.Height);
        //            using (DrawingContext dc = DrawingContext.FromBitmap(cacheImage))
        //            {
        //                dc.Clear(Color.Transparent);
        //                dc.DrawImage(img, new Rect(0, 0, rect.Width, rect.Height), rect);
        //            }
        //        }
        //    }
        //    else if (propertyName == nameof(Image))
        //    {
        //        if (newValue == null)
        //        {
        //            throw new Exception("Image不能为null");
        //        }
        //        else
        //        {
        //            if (cacheImage != null)
        //            {
        //                cacheImage.Dispose();
        //                cacheImage = null;
        //            }
        //            Rect rect = ImageClip;
        //            var img = newValue as Image;
        //            if (img != null && rect.Width > 0 && rect.Height > 0)
        //            {
        //                cacheImage = new Bitmap((int)rect.Width, (int)rect.Height);
        //                using (DrawingContext dc = DrawingContext.FromBitmap(cacheImage))
        //                {
        //                    dc.Clear(Color.Transparent);
        //                    dc.DrawImage(img, new Rect(0, 0, rect.Width, rect.Height), rect);
        //                }
        //            }
        //        }
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (cacheImage != null)
            {
                cacheImage.Dispose();
                cacheImage = null;
            }
        }

        public override Brush CreateBrush(in Rect rect, in float renderScaling)
        {
            var image = Image;
            if (image == null)
            {
                return new SolidColorBrush(Color.Transparent);
            }
            Image img = cacheImage == null ? image : cacheImage;
            var m = Matrix.Identity;
            var w = img.Width;
            var h = img.Height;
            var ww = rect.Width;
            var hh = rect.Height;
            OverrideMatrix(ref m, renderScaling);
            switch (Stretch)
            {
                case Stretch.None:
                    break;
                case Stretch.Fill:
                    m.Scale(rect.Width / img.Width, rect.Height / img.Height);
                    break;
                case Stretch.Uniform:
                    if (w / rect.Width > h / rect.Height)
                    {
                        hh = rect.Width * h / w;
                    }
                    else
                    {
                        ww = rect.Height * w / h;
                    }
                    m.Scale(ww / img.Width, hh / img.Height);
                    m.Translate((rect.Width - ww) / 2, (rect.Height - hh) / 2);
                    break;
                case Stretch.UniformToFill:
                    if (w / rect.Width < h / rect.Height)
                    {
                        hh = rect.Width * h / w;
                    }
                    else
                    {
                        ww = rect.Height * w / h;
                    }
                    m.Scale(ww / img.Width, hh / img.Height);
                    m.Translate((rect.Width - ww) / 2, (rect.Height - hh) / 2);
                    break;
            }
            m.Translate(rect.X, rect.Y);
            return new TextureBrush(img, WrapMode, m);
        }

        protected virtual void OverrideMatrix(ref Matrix matrix, in float renderScaling)
        {

        }

        public override string GetCreationCode()
        {
            if (!string.IsNullOrEmpty(path))
            {
                return "\"" + path + "\"";
            }
            else
            {
                return base.GetCreationCode();
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
            return base.ToString();
        }
    }
}
