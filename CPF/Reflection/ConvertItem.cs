using System;
using System.Reflection.Emit;

namespace CPF.Reflection
{
    public class ConvertItem
    {
        public ConvertItem(Action<ILGenerator> action, int lostWeight = 0, int consumptionWeight = 0, int order = 0)
        {
            Order = order;
            Action = action;
            LostWeight = lostWeight;
            ConsumptionWeight = consumptionWeight;
        }
        public ConvertItem(OpCode opCode, int lostWeight = 0, int consumptionWeight = 0, int order = 0)
        {
            Order = order;
            OpCode = opCode;
            LostWeight = lostWeight;
            ConsumptionWeight = consumptionWeight;
        }
        internal int Order { get; set; }
        public int ConsumptionWeight { get; }
        public int LostWeight { get; }
        public Action<ILGenerator> Action { get; }
        public OpCode? OpCode { get; }
    }
}
