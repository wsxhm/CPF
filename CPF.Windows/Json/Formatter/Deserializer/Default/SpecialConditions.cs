using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CPF.Windows.Json.Deserialize
{
    internal class SpecialConditionsResolve : DefaultJsonResolve
    {
        #region pregenerated metas
        internal static MethodInfo _ReadAvoidNull = typeof(SpecialConditionsResolve).GetMethod(nameof(ReadAvoidNull), BindingFlags.NonPublic | BindingFlags.Static);
        internal static MethodInfo _ReadNullable = typeof(SpecialConditionsResolve).GetMethod(nameof(ReadNullable), BindingFlags.NonPublic | BindingFlags.Static);
        internal static MethodInfo _ReadEnum = typeof(SpecialConditionsResolve).GetMethod(nameof(ReadEnum), BindingFlags.NonPublic | BindingFlags.Static);
        #endregion

        //---------Avoid
#if !Net4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static void ReadAvoidNull(JsonReader reader)
        {
            char c = reader.BeforAnnotation();
            if (c == 'n' && reader.StrCompair("ull"))
                return;
            if (c == '{' && reader.GetChar() == '}')
                return;
            throw new JsonWrongCharacterException(reader);
        }

        //---------T?
#if !Net4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static T? ReadNullable<T>(JsonReader reader, JsonDeserializeHandler handler) where T : struct
        {
            var c = reader.BeforAnnotation();
            if (c == 'n')
            {
                if (reader.StrCompair("ull"))
                    return null;
                throw new JsonDeserializationTypeResolutionException(typeof(T?));
            }
            else
            {
                reader.RollbackChar();
                return (T?)ResolveProvider<T>.InvokeGet(reader, handler);
            }
        }

        //---------Enum
#if !Net4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object ReadEnum(JsonReader reader, JsonDeserializeHandler handler)
        {
            var c = reader.BeforAnnotation();
            var t = handler.Types.Dequeue();
            if (c == '"')
            {
                reader.RollbackChar();
                var str = reader.ReadString();
                return Enum.Parse(t, str);
            }

            if (c >= '0' && c <= '9')// byte、sbyte、short、ushort、int、uint、long 、ulong  default ->int
            {
                reader.RollbackChar();
                var basicType = Enum.GetUnderlyingType(t);
                if (basicType == typeof(long))
                    return Enum.ToObject(t, PrimitiveResolve.ReadLong(reader, handler));
                else if (basicType == typeof(ulong))
                    return Enum.ToObject(t, PrimitiveResolve.ReadULong(reader, handler));
                else if (basicType == typeof(uint))
                    return Enum.ToObject(t, PrimitiveResolve.ReadUInt(reader, handler));
                else
                    return Enum.ToObject(t, PrimitiveResolve.ReadInt(reader, handler));
            }
            throw new JsonDeserializationTypeResolutionException(reader, t);
        }
    }
}
