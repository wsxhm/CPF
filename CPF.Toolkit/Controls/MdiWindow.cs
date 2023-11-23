using CPF;
using CPF.Animation;
using CPF.Charts;
using CPF.Controls;
using CPF.Drawing;
using CPF.Platform;
using CPF.Shapes;
using CPF.Styling;
using CPF.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class MdiWindow : Control, IWindow
    {
        public WindowState WindowState { get => GetValue<WindowState>(); set => SetValue(value); }
        public Image Icon { get => GetValue<Image>(); set => SetValue(value); }
        public string Title { get => GetValue<string>(); set => SetValue(value); }
        public UIElement Content { get => GetValue<UIElement>(); set => SetValue(value); }
        //protected override UIElementCollection Children => throw new NotImplementedException();
        WindowFrame frame;
        SizeField normalSize = new SizeField(500, 500);
        public void Close()
        {

        }

        public void DragMove()
        {
        }

        protected override void InitializeComponent()
        {
            this.Size = this.normalSize;
            this.frame = base.Children.Add(new WindowFrame(this, this.Content));
            frame.MaximizeBox = true;

            var caption = frame.Find<Panel>().FirstOrDefault(x => x.Name == "caption");
            caption.Background = "white";
            caption.BorderType = BorderType.BorderThickness;
            caption.BorderThickness = new Thickness(0, 0, 0, 1);
            caption.BorderFill = "235,235,235";

            var title = frame.Find<TextBlock>().FirstOrDefault(x => x.Name == "title");
            title.Foreground = "black";

            frame.ControlBoxStroke = "black";
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            switch (propertyName)
            {
                case nameof(WindowState):
                    switch (this.WindowState)
                    {
                        case WindowState.Normal:
                            {
                                this.Size = this.normalSize;
                            }
                            break;
                        case WindowState.Minimized:
                            break;
                        case WindowState.Maximized:
                            {
                                this.Size = SizeField.Fill;
                            }
                            break;
                        case WindowState.FullScreen:
                            break;
                    }
                    break;

                case nameof(Width):
                case nameof(Height):
                case nameof(Size):
                    {
                        if (WindowState == WindowState.Normal)
                        {
                            this.Size = this.normalSize;
                        }
                    }
                    break;
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        }
    }
}
