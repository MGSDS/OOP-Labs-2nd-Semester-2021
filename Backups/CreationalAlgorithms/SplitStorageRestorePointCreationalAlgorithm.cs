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
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            var id = Guid.NewGuid();
            IReadOnlyList<Storage> storages = repository.CreateStorages(objects, id.ToString());
            return new RestorePoint(storages, DateTime.Now, id);
        }
    }
}