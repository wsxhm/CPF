using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Documents
{
    public interface ICanSelectElement
    {
        /// <summary>
        /// 是否可以被选中
        /// </summary>
        /// <returns></returns>
        bool CanSelect { get; }
    }
}
