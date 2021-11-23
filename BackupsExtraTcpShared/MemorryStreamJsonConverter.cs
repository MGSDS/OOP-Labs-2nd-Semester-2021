using System;
using System.IO;
using Newtonsoft.Json;

namespace BackupsExtraTcpShared
{
    public class MemoryStreamJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MemoryStream).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var ms = new MemoryStream();
            byte[]? bytes = serializer.Deserialize<byte[]>(reader);
            if (bytes != null)
                ms.Write(bytes, 0, bytes.Length);
            return ms;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            byte[]? bytes = (value as MemoryStream).ToArray();
            serializer.Serialize(writer, bytes);
        }
    }
}