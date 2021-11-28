using System.Collections.Generic;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Repository;

namespace BackupsExtra.CreationalAlgorithms
{
    public interface IRestorePointManageAlgorithm : IRestorePointCreationalAlgorithm
    {
        void Merge(RestorePoint source, RestorePoint destination, IExtraBackupDestinationRepository backupDestinationRepository, IExtraCompressor compressor);
        void Delete(RestorePoint target, IExtraBackupDestinationRepository backupDestinationRepository);
        IReadOnlyList<RestoreItem> Restore(RestorePoint target, IExtraBackupDestinationRepository backupDestinationRepository, IExtraCompressor compressor);
    }
}
