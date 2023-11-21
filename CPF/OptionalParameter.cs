using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    /// <summary>
    /// 定义一个可选参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct OptionalParameter<T>
    {
        public OptionalParameter(T value)
        {
            Value = value;
            HasValue = true;
        }
        /// <summary>
        /// 参数值
        /// </summary>
        public T Value { get; }
        /// <summary>
        /// 是否有参数值
        /// </summary>
        public bool HasValue { get; }

        public static implicit operator OptionalParameter<T>(T value)
        {
            return new OptionalParameter<T>(value);
        }
    }
}
