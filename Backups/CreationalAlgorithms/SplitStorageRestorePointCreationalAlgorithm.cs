using System;
using System.Collections.Generic;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.CreationalAlgorithms
{
    public class SplitStorageRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IRepository repository, ICompressor compressor)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            var id = Guid.NewGuid();
            List<Storage> storages = objects.ConvertAll(jobObject => repository.CreateStorage(new List<JobObject> { jobObject }, compressor, id.ToString()));
            return new RestorePoint(storages, DateTime.Now, id, id.ToString());
        }
    }
}