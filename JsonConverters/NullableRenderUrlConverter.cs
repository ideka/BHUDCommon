using Gw2Sharp;
using Gw2Sharp.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Ideka.BHUDCommon;

public class NullableRenderUrlConverter : JsonConverter<RenderUrl?>
{
    private struct Intermediate
    {
        public string? Url;
    }

    public override RenderUrl? ReadJson(JsonReader reader, Type objectType, RenderUrl? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var i = serializer.Deserialize<Intermediate?>(reader);
        if (i?.Url is not string url)
            return null;

        var constructorInfo = typeof(RenderUrl).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null,
            [typeof(IGw2Client), typeof(string), typeof(string)], null)
            ?? throw new JsonSerializationException($"No matching constructor found for type {objectType}");

        var result = (RenderUrl)constructorInfo.Invoke(new[] { null, url, null });

        return result;
    }

    public override void WriteJson(JsonWriter writer, RenderUrl? value, JsonSerializer serializer)
        => serializer.Serialize(writer, value == null ? null : new Intermediate()
        {
            Url = value?.Url?.AbsoluteUri,
        });
}
