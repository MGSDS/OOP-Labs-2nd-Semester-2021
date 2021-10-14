using System;
using System.IO;

namespace Backups.NetworkTransfer.Entities
{
    public class TransferFile
    {
        public TransferFile(string name, MemoryStream stream)
        {
            Name = name;
            Stream = stream;
        }

        public string Name { get; }

        public MemoryStream Stream { get; }
    }
}