using System;

namespace CPF.Json.Deserialize
{
    internal class ExpressionJsonResolve : JsonResolveBase
    {
        internal static T ReturnFunc<T>(Func<T> f) => f();
    }
}
