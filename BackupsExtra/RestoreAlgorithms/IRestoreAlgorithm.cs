using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.RestoreAlgorithm
{
    public interface IRestoreAlgorithm
    {
        void Restore(RestorePoint restorePoint, IRestorePointManageAlgorithm restorePointManageAlgorithm, IExtraRepository repository, IExtraCompressor compressor);
    }
}