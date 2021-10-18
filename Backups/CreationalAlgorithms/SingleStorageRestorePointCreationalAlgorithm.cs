using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.CreationalAlgorithms
{
    public class SingleStorageRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IRepository repository)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            Storage storage = repository.CreateStorage(objects);
            return new RestorePoint(new List<Storage> { storage }, DateTime.Now, storage.Id);
        }
    }
}