using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Documents
{
    /// <summary>
    /// 分块
    /// </summary>
    public class InlineBlock : Block, IFlowElement
    {
        //public InlineBlock() { }

        //public InlineBlock(string text)
        //{
        //    Add(text);
        //}
        //internal TextLine Line;
        /// <summary>
        /// 流动方向
        /// </summary>
        [PropertyMetadata(FlowDirection.LeftToRight)]
        public FlowDirection FlowDirection
        {
            get { return GetValue<FlowDirection>(); }
            set { SetValue(value); }
        }
    }
}
