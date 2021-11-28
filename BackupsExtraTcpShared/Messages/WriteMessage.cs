using System.IO;
using Backups.Entities;
using Newtonsoft.Json;

namespace BackupsExtraTcpShared.Messages
{
    public class WriteMessage
    {
        public WriteMessage(Storage storage, Stream stream)
        {
            Stream = new MemoryStream();
            stream.Position = 0;
            stream.CopyTo(Stream);
            Storage = storage;
        }
        
        [JsonConstructor]
        public WriteMessage(Storage storage, byte[] data)
        {
            Stream = new MemoryStream();
            Stream.Write(data, 0, data.Length);
            Storage = storage;
        }
        
        
        public Storage Storage { get; }
        public byte[] Data => Stream.ToArray();

        [JsonIgnore]
        public MemoryStream Stream { get; }
        
        
    }
}