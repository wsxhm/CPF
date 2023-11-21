using System.Reflection;

namespace CPF.Json
{
    internal class MethodInfoOrder
    {
        internal MethodInfo MethodInfo;
        internal int Priority;
        public MethodInfoOrder(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }
        public MethodInfoOrder(MethodInfo methodInfo,int priority)
        {
            MethodInfo = methodInfo;
            Priority = priority;
        }
    }
}
