using System.Collections.Generic;
using Backups.Entities;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.ClearAlgorithms
{
    public interface IClearAlgorithm
    {
        void Clear(IRestorePointManageAlgorithm restorePointManageAlgorithm, IExtraBackupDestinationRepository backupDestinationRepository, List<RestorePoint> restorePoints);
    }
}