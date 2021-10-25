using System;
using System.IO;

namespace Backups.NetworkTransfer.Entities
{
    public class TransferFile
    {
        public TransferFile(string name, MemoryStream stream)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public string Name { get; }

        public MemoryStream Stream { get; }
    }
}