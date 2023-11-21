using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Design
{
    /// <summary>
    /// 用于给控件模板设计预览的时候加载样式
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class DesignerLoadStyleAttribute : Attribute
    {
        /// <summary>
        /// 设计时项目路径
        /// </summary>
        public static string ProjectPath { get; set; }

        /// <summary>
        /// 用于给控件模板设计预览的时候加载样式
        /// </summary>
        /// <param name="stylesheet">样式路径</param>
        public DesignerLoadStyleAttribute(string stylesheet)
        {
            this.Stylesheet = stylesheet;
        }

        /// <summary>
        /// 样式路径
        /// </summary>
        public string Stylesheet { get; set; }
    }
}
