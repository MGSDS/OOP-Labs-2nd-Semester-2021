using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BackupsExtra
{
    public static class JsonWrapper
    {
        public static void WriteJson<T>(string filePath, T objects)
        {
            string jsonString = JsonConvert.SerializeObject(objects, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
            File.WriteAllText(filePath, jsonString);
        }

        public static T ReadJson<T>(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }
    }
}