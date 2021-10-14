using System;
using System.Collections.Generic;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.Algorithms
{
    public class SingleFileRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IRepository repository)
        {
            Storage storage = repository.CreateStorage(objects);
            return new RestorePoint(new List<Storage> { storage }, DateTime.Now, storage.Id);
        }
    }
}