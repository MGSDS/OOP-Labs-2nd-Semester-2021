using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.Algorithms
{
    public class MultipleFileRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IRepository repository)
        {
            var id = Guid.NewGuid();
            IRepository repo = repository.CreateRepository(id);
            var storages = new List<Storage>(objects.Count);
            storages.AddRange(objects.Select(repo.CreateStorage));
            return new RestorePoint(storages, DateTime.Now, id);
        }
    }
}