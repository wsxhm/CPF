
namespace CPF.Json.Serializer
{
    internal enum SerializerBuildTypeEnum
    {
        Nullable,
        IDictionaryGeneric,
        IEnumerableGeneric,
        IListGeneric,
        WrongGenericKey,
        KeyValueObject,
        KeyValuePair,
        Lazy
    }
}
