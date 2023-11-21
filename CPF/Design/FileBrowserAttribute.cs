using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Design
{
    /// <summary>
    /// 用于定义设计器中属性的文件浏览功能
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileBrowserAttribute : Attribute
    {
        /// <summary>
        /// 设置文件类型，后缀名  .jpg;.png;
        /// </summary>
        /// <param name="fileTypes">.jpg;.png;</param>
        public FileBrowserAttribute(string fileTypes)
        {
            this.FileTypes = fileTypes;
        }
        /// <summary>
        /// 后缀名  .jpg;.png;
        /// </summary>
        public string FileTypes { get; set; }
    }
}
