using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.ClearAlgorithms.Proxies
{
    public class DateClearAlgorithmProxy : IClearAlgorithm
    {
        public DateClearAlgorithmProxy(IClearAlgorithm algorithm, TimeSpan maxAge)
        {
            Algorithm = algorithm;
            MaxAge = maxAge;
        }

        public IClearAlgorithm Algorithm { get; }
        public TimeSpan MaxAge { get; }

        public void Clear(IRestorePointManageAlgorithm restorePointManageAlgorithm, IExtraRepository repository, List<RestorePoint> restorePoints)
        {
            if (restorePoints.Count <=
                restorePoints.Count(restorePoint => DateTime.Now - restorePoint.BackupTime > MaxAge))
            {
                Algorithm.Clear(restorePointManageAlgorithm, repository, restorePoints);
                throw new InvalidOperationException($"Can not run DateClearAlgorithmProxy with MaxAge {MaxAge}. RestorePoints can not be empty");
            }

            foreach (RestorePoint restorePoint in restorePoints.Where(restorePoint =>
                    DateTime.Now - restorePoint.BackupTime > MaxAge))
            {
                restorePointManageAlgorithm.Delete(restorePoint, repository);
            }

            restorePoints.RemoveAll(restorePoint =>
                DateTime.Now - restorePoint.BackupTime > MaxAge);
            Algorithm.Clear(restorePointManageAlgorithm, repository, restorePoints);
        }
    }
}