using System;
using System.Collections.Generic;
using System.Text;
using CPF.Drawing;

namespace CPF
{
    public class MatrixTransform : Transform
    {
        public MatrixTransform(Matrix m)
        {
            matrix = m;
        }
        Matrix matrix;

        public override Matrix Value
        {
            get
            {
                return matrix;
            }
        }
    }
}
