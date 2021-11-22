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
        void Merge(RestorePoint source, RestorePoint destination, IExtraRepository repository, IExtraCompressor compressor);
        void Delete(RestorePoint target, IExtraRepository repository);
        IReadOnlyList<RestoreItem> Restore(RestorePoint target, IExtraRepository repository, IExtraCompressor compressor);
    }
}
