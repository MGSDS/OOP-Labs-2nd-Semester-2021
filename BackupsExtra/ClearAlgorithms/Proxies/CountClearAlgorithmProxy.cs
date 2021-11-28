using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.ClearAlgorithms.Proxies
{
    public class CountClearAlgorithmProxy : IClearAlgorithm
    {
        public CountClearAlgorithmProxy(IClearAlgorithm algorithm, ushort maxCount)
        {
            if (maxCount < 1)
                throw new ArgumentException("MaxCount must be greater than 0");
            MaxCount = maxCount;
            Algorithm = algorithm;
        }

        public IClearAlgorithm Algorithm { get; }
        public ushort MaxCount { get; }
        public void Clear(IRestorePointManageAlgorithm restorePointManageAlgorithm, IExtraBackupDestinationRepository backupDestinationRepository, List<RestorePoint> restorePoints)
        {
            if (restorePoints.Count() <= MaxCount)
            {
                Algorithm.Clear(restorePointManageAlgorithm, backupDestinationRepository, restorePoints);
                return;
            }

            restorePoints.Sort((x, y) => x.BackupTime.CompareTo(y.BackupTime));
            int toBeDeleted = restorePoints.Count - MaxCount;
            for (int i = 0; i < toBeDeleted; i++)
            {
                restorePointManageAlgorithm.Delete(restorePoints.First(), backupDestinationRepository);
                restorePoints.RemoveAt(0);
            }

            Algorithm.Clear(restorePointManageAlgorithm, backupDestinationRepository, restorePoints);
        }
    }
}