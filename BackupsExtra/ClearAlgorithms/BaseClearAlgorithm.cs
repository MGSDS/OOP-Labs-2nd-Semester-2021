using System.Collections.Generic;
using Backups.Entities;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Repository;

namespace BackupsExtra.ClearAlgorithms
{
    public class BaseClearAlgorithm : IClearAlgorithm
    {
        public void Clear(IRestorePointManageAlgorithm restorePointManageAlgorithm, IExtraRepository repository, List<RestorePoint> restorePoints)
        {
        }
    }
}