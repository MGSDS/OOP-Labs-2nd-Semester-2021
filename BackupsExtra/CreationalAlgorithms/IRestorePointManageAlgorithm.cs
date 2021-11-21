using Backups.CreationalAlgorithms;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.CreationalAlgorithms
{
    public interface IRestorePointManageAlgorithm : IRestorePointCreationalAlgorithm
    {
        void Merge(RestorePoint source, RestorePoint destination, IExtraRepository repository, IExtraCompressor compressor);
        void Delete(RestorePoint target, IExtraRepository repository);
    }
}
