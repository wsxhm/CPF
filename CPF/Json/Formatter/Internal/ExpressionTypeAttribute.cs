using System;
using CPF.Json.Deserialize;
using CPF.Json.Serializer;

namespace CPF.Json
{
    internal class ExpressionBuildTypeAttribute : Attribute
    {
        internal DeserializeBuildTypeEnum _deserializeBuildType;

        internal SerializerBuildTypeEnum _serializerBuildTypeEnum;

        internal ExpressionBuildTypeAttribute(DeserializeBuildTypeEnum buildType)
        {
            _deserializeBuildType = buildType;
        }

        internal ExpressionBuildTypeAttribute(SerializerBuildTypeEnum buildType)
        {
            _serializerBuildTypeEnum = buildType;
        }
    }
}
