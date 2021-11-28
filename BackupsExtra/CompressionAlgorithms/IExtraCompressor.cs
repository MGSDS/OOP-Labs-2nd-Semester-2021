using System.Collections.Generic;
using System.IO;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using BackupsExtra.Repository;
using File = Backups.Entities.File;

namespace BackupsExtra.CompressionAlgorithms
{
    public interface IExtraCompressor : ICompressor
    {
        public void Merge(Stream destination, Stream source);
        public IReadOnlyList<File> Decompress(Storage storage, IExtraBackupDestinationRepository backupDestinationRepository);
    }
}