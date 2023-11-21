using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 定义一些常用附加属性
    /// </summary>
    public class AttachedExtenstions
    {
        /// <summary>
        /// 是否错误
        /// </summary>
        public static Attached<bool> IsError
        {
            get
            {
                return CpfObject.RegisterAttached(false, typeof(AttachedExtenstions));
            }
        }
        /// <summary>
        /// 是否有效
        /// </summary>
        public static Attached<bool> IsValid
        {
            get
            {
                return CpfObject.RegisterAttached(false, typeof(AttachedExtenstions));
            }
        }


        /// <summary>
        /// 是否为空
        /// </summary>
        public static Attached<bool> IsEmpty
        {
            get
            {
                return CpfObject.RegisterAttached(false, typeof(AttachedExtenstions));
            }
        }


        /// <summary>
        /// 当使用交替项目容器时，获取项目容器的分配值。
        /// </summary>
        public static Attached<int> AlternationIndex
        {
            get
            {
                return CpfObject.RegisterAttached(0, typeof(AttachedExtenstions));
            }
        }

        /// <summary>
        /// int值
        /// </summary>
        public static Attached<int> Value
        {
            get
            {
                return CpfObject.RegisterAttached(0, typeof(AttachedExtenstions));
            }
        }
    }
}
