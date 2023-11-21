using System;
using System.Runtime.CompilerServices;

namespace CPF.Windows.Json.Serializer
{
    internal class FormattingProvider<T>
    {
        internal static Action<T, JsonSerializerHandler> Get = FormatterFind<T>.Find();

#if !Net4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static void Convert(T type, JsonSerializerHandler handler) => Get(type, handler);
    }
}
