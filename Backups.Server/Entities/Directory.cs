using System.Collections.Generic;

namespace Backups.Server.Entities
{
    public class Directory
    {
        public Directory(string name, IReadOnlyList<TransferFile> files)
        {
            Name = name;
            Files = files;
        }

        public string Name { get; }
        public IReadOnlyList<TransferFile> Files { get; }
    }
}