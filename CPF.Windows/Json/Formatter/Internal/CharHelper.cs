using System.Runtime.CompilerServices;

namespace CPF.Windows.Json
{
    internal static class CharHelper
    {
#if !Net4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static int CharToNumber(this char x)
        {
            if ('0' <= x && x <= '9')
                return x - '0';
            if ('a' <= x && x <= 'f')
                return x - 'a' + 10;
            if ('A' <= x && x <= 'F')
                return x - 'A' + 10;

            throw new JsonWrongCharacterException("The code unit format is incorrect");
        }
    }
}
