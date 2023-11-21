using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public class TransformGroup : Transform
    {
        TransformCollection children = new TransformCollection();
        public TransformCollection Children
        {
            get { return children; }
        }

        public override Matrix Value
        {
            get
            {
                Matrix m = Matrix.Identity;
                foreach (Transform item in children)
                {
                    m.Append(item.Value);
                }
                return m;
            }
        }
    }
}
