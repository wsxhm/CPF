using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raindrops.Shared.InvokeST.ConvertMap
{
    public struct SearchNode
    {
        public int Deep;
        public int ParentIndex;
        public int LostWeight;
        public int ConsumptionWeight;
        public Type Inherit;
        public Type Target;
        public ConvertItem ConvertItem;
    }
}
