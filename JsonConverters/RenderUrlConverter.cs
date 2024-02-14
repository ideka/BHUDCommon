using Gw2Sharp;
using Gw2Sharp.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Ideka.BHUDCommon;

public class RenderUrlConverter : JsonConverter<RenderUrl>
{
    private class Intermediate
    {
        public string? Url;
    }

    public override RenderUrl ReadJson(JsonReader reader, Type objectType, RenderUrl existingValue, bool hasExistingValue, JsonSerializer serializer)
    { 
        if (serializer.Deserialize<Intermediate>(reader) is not Intermediate i)
            throw new JsonSerializationException();

        var constructorInfo = objectType.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null,
            [typeof(IGw2Client), typeof(string), typeof(string)], null)
            ?? throw new JsonSerializationException($"No matching constructor found for type {objectType}");

        return i.Url == null 
            ? new RenderUrl()
            : (RenderUrl)constructorInfo.Invoke(new[] { null, i.Url ?? "", null });
    }

    public override void WriteJson(JsonWriter writer, RenderUrl value, JsonSerializer serializer)
        => serializer.Serialize(writer, new Intermediate()
        {
            Url = value.Url?.AbsoluteUri,
        });
}
