#if Net4||Net2
namespace System
{
    /// <summary>
    /// 值元组
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [Serializable]
    public struct ValueTuple<T1, T2>
    {
        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1;

        public T2 Item2;
    }
}

#endif