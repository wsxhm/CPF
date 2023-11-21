using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Design
{
    /// <summary>
    /// 定义类型序列化为C#代码
    /// </summary>
    public interface ISerializerCode
    {
        /// <summary>
        /// 获取对象构造的C#代码
        /// </summary>
        /// <returns></returns>
        string GetCreationCode();
    }
}
