using System;
using System.Collections.Generic;

namespace Raindrops.Shared.InvokeST.ConvertMap
{
    public class SearchResult
    {
        public int Length => Items?.Length ?? 0;
        public int ConsumptionWeight { get; set; }
        public int LostWeight { get; set; }
        public KeyValuePair<Type, ConvertItem>[] Items { get; set; }
    }
}
