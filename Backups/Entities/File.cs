using System;
using System.IO;
using Newtonsoft.Json;

namespace Backups.Entities
{
    public class File : IDisposable
    {
        public File(Stream stream, string name)
        {
            Stream = new MemoryStream();
            stream.Position = 0;
            stream.CopyTo(Stream);
            Name = name;
        }

        [JsonConstructor]
        public File(byte[] data, string name)
        {
            Stream = new MemoryStream();
            Stream.Write(data, 0, data.Length);
            Name = name;
        }

        public string Name { get; }
        public byte[] Data => Stream.ToArray();
        [JsonIgnore]
        public MemoryStream Stream { get; }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}