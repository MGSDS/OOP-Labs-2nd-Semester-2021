using System;
using System.IO;

namespace Backups.Server.Entities
{
    public class TransferFile
    {
        public TransferFile(String name, MemoryStream stream)
        {
            Name = name;
            Stream = stream;
        }

        public string Name { get; }
        public MemoryStream Stream { get; }
    }
}