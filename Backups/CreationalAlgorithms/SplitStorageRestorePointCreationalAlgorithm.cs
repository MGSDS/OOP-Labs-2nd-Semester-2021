using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.CreationalAlgorithms
{
    public class SplitStorageRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IRepository repository)
        {
            var id = Guid.NewGuid();
            IReadOnlyList<Storage> storages = repository.CreateStorages(objects, id.ToString());
            return new RestorePoint(storages, DateTime.Now, id);
        }
    }
}