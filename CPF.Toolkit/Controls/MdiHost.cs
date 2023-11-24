using CPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CPF.Toolkit.Controls
{
    public class MdiHost : Panel
    {
        public MdiHost()
        {
            Size = SizeField.Fill;
            Background = "204,204,204";
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue, PropertyMetadataAttribute propertyMetadata)
        {
            if (propertyName == nameof(ActualSize))
            {
                foreach (MdiWindow item in this.Children)
                {
                    if (item.WindowState == WindowState.Maximized) continue;

                    if (item.MarginLeft.Value + item.ActualSize.Width > this.ActualSize.Width)
                    {
                        item.MarginLeft = this.ActualSize.Width - item.ActualSize.Width;
                    }

                    if (item.MarginTop.Value + item.ActualSize.Height > this.ActualSize.Height)
                    {
                        item.MarginTop = this.ActualSize.Height - item.ActualSize.Height;
                    }
                }
            }
            base.OnPropertyChanged(propertyName, oldValue, newValue, propertyMetadata);
        }

        protected override void OnUIElementAdded(UIElementAddedEventArgs e)
        {
            base.OnUIElementAdded(e);
        }
    }
}
