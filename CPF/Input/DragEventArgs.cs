using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF.Input
{
    public class DragEventArgs : RoutedEventArgs
    {

        public DragDropEffects DragEffects { get; set; }

        public IDataObject Data { get; private set; }

        public Point Location { get; internal set; }

        public DragEventArgs(IDataObject data, Point location,  UIElement source)
            : base(source)
        {
            this.Data = data;
            this.Location = location;
        }

    }
}
