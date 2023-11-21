using System;
using CPF.Windows.Json.Deserialize;
using CPF.Windows.Json.Serializer;

namespace CPF.Windows.Json
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
