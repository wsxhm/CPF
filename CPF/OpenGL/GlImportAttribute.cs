using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.OpenGL
{
    /// <summary>
    /// OpenGL导入函数名
    /// </summary>
    public class GlImportAttribute : Attribute
    {
        public string Name { get; set; }
        /// <summary>
        /// OpenGL导入函数名
        /// </summary>
        /// <param name="name"></param>
        public GlImportAttribute(string name)
        {
            Name = name;
        }
    }
}
