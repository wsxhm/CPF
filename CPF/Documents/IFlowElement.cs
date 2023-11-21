using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Documents
{
    public interface IFlowElement : IDocumentElement
    {
        /// <summary>
        /// 流动方向
        /// </summary>
        FlowDirection FlowDirection { get; }
    }
}
