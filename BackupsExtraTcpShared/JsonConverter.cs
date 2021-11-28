using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackupsExtraTcpShared
{
    public static class JsonConverter
    {
        public static string Serialize(object? obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<Newtonsoft.Json.JsonConverter> { new MemoryStreamJsonConverter() },
            });
        }

        public static T? Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<Newtonsoft.Json.JsonConverter> { new MemoryStreamJsonConverter() },
            });
        }

        public static object? Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<Newtonsoft.Json.JsonConverter> { new MemoryStreamJsonConverter() },
            });
        }
    }
}