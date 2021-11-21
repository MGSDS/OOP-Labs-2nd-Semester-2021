using System;
using System.Linq;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.Entities
{
    public class ExtraBackup : Backup
    {
        public ExtraBackup(IRestorePointManageAlgorithm restorePointCreationalAlgorithm, IExtraCompressor compressor, IExtraRepository repository)
            : base(restorePointCreationalAlgorithm, compressor, repository)
        {
        }

        public void MergeRestorePoints(Guid first, Guid second)
        {
            RestorePoint firstRestorePoint = RestorePoints.FirstOrDefault(x => x.Id == first)
                                             ?? throw new ArgumentException($"Restore point with id {first} not found");
            RestorePoint secondRestorePoint = RestorePoints.FirstOrDefault(x => x.Id == second)
                                              ?? throw new ArgumentException($"Restore point with id {second} not found");
            if ((secondRestorePoint.BackupTime - firstRestorePoint.BackupTime).TotalMilliseconds > 0)
            {
                (secondRestorePoint, firstRestorePoint) = (firstRestorePoint, secondRestorePoint);
            }

            (RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm)?.Merge(
                secondRestorePoint,
                firstRestorePoint,
                Repository as IExtraRepository,
                Compressor as IExtraCompressor);
            Remove(secondRestorePoint);
        }

        public void DeleteRestorePoint(Guid id)
        {
            RestorePoint restorePoint = RestorePoints.FirstOrDefault(x => x.Id == id)
                                          ?? throw new ArgumentException($"Restore point with id {id} not found");
            (RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm)?.Delete(restorePoint, Repository as IExtraRepository);
            Remove(restorePoint);
        }
    }
}