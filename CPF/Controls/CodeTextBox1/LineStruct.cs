using System;
using System.Collections.Generic;
using System.Text;

namespace CPF.Controls
{
    public class LineStruct
    {
        /// <summary>
        /// 行号的y轴偏移
        /// </summary>
        public float YOffet { set; get; }
        /// <summary>
        /// 此行是否绘制折叠
        /// </summary>
        public bool IsFold { set; get; }

    }
}
