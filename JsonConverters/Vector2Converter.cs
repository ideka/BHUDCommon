using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;

namespace Ideka.BHUDCommon;

public class Vector2Converter : JsonConverter<Vector2>
{
    private struct Intermediate
    {
        public float X;
        public float Y;
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var i = serializer.Deserialize<Intermediate>(reader);
        return new Vector2(i.X, i.Y);
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        => serializer.Serialize(writer, new Intermediate() { X = value.X, Y = value.Y });
}
