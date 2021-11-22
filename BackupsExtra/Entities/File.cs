using System;
using System.IO;

namespace BackupsExtra.Entities
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

        public string Name { get; }
        public MemoryStream Stream { get; }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}