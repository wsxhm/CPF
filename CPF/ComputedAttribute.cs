using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 计算属性，设置需要关联的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ComputedAttribute : Attribute
    {
        string[] propertyNames;
        /// <summary>
        /// 计算属性，设置需要关联的属性
        /// </summary>
        /// <param name="propertyName"></param>
        public ComputedAttribute(params string[] propertyName)
        {
            propertyNames = propertyName;
        }
        /// <summary>
        /// 关联的属性
        /// </summary>
        public string[] Properties
        {
            get { return propertyNames; }
        }
    }
}
