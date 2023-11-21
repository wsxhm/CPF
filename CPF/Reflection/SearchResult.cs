using System;
using System.Collections.Generic;

namespace CPF.Reflection
{
    public class SearchResult
    {
        public int Length => Items?.Length ?? 0;
        public int ConsumptionWeight { get; set; }
        public int LostWeight { get; set; }
        public KeyValuePair<Type, ConvertItem>[] Items { get; set; }
    }
}
