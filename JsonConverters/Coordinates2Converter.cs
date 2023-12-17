using Gw2Sharp.Models;
using Newtonsoft.Json;
using System;

namespace Ideka.BHUDCommon;

public class Coordinates2Converter : JsonConverter<Coordinates2>
{
    private class Intermediate
    {
        public double X;
        public double Y;
    }

    public override Coordinates2 ReadJson(JsonReader reader, Type objectType, Coordinates2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        => serializer.Deserialize<Intermediate>(reader) is Intermediate i
            ? new Coordinates2(i.X, i.Y)
            : throw new JsonSerializationException();

    public override void WriteJson(JsonWriter writer, Coordinates2 value, JsonSerializer serializer)
        => serializer.Serialize(writer, new Intermediate()
        {
            X = value.X,
            Y = value.Y,
        });
}
