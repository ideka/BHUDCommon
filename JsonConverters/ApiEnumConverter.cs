using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;
using System;

namespace Ideka.BHUDCommon;

public class ApiEnumConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ApiEnum<>);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            serializer.Serialize(writer, null);
            return;
        }

        var valueProperty = value.GetType().GetProperty("Value");
        var actualValue = valueProperty.GetValue(value);
        serializer.Serialize(writer, actualValue);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var valueType = objectType.GetGenericArguments()[0];

        var actualValue = serializer.Deserialize(reader, valueType);
        var constructorInfo = objectType.GetConstructor(new[] { valueType })
            ?? throw new JsonSerializationException($"No matching constructor found for type {objectType}");

        return constructorInfo.Invoke(new[] { actualValue });
    }
}
