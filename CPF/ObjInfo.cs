using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    class ObjInfo: Dictionary<string, PropertyMetadataAttribute>
    {
        public List<ComputeProtertyInfo> Computed;
    }
}
