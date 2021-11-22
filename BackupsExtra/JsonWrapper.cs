using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BackupsExtra
{
    public static class JsonWrapper
    {
        public static void WriteJson<T>(string filePath, List<T> backups)
        {
            string jsonString = JsonConvert.SerializeObject(backups, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
            File.WriteAllText(filePath, jsonString);
        }

        public static List<T> ReadJson<T>(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }
    }
}