using Newtonsoft.Json;
using System;

namespace Ideka.BHUDCommon;

public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        => TimeSpan.FromTicks(serializer.Deserialize<long>(reader));

    public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        => serializer.Serialize(writer, value.Ticks);
}
