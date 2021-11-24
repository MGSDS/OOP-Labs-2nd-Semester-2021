using System;
using System.Collections.Generic;
using System.Linq;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.CreationalAlgorithms
{
    public class SplitStorageRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IBackupDestinationRepository backupDestinationRepository, ISourceRepository sourceRepository, ICompressor compressor)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            if (backupDestinationRepository == null) throw new ArgumentNullException(nameof(backupDestinationRepository));
            var id = Guid.NewGuid();
            var jobObjectsWithData = objects.Select(x => new JobObjectWithData(x, sourceRepository)).ToList();
            List<Storage> storages = jobObjectsWithData.ConvertAll(jobObject =>
                backupDestinationRepository.CreateStorage(new List<JobObjectWithData> { jobObject }, compressor, id.ToString()));
            return new RestorePoint(storages, DateTime.Now, id, id.ToString());
        }
    }
}