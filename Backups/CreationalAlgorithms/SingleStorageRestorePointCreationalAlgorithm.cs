using System;
using System.Collections.Generic;
using System.Linq;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.CreationalAlgorithms
{
    public class SingleStorageRestorePointCreationalAlgorithm : IRestorePointCreationalAlgorithm
    {
        public RestorePoint Run(List<JobObject> objects, IBackupDestinationRepository backupDestinationRepository, ISourceRepository sourceRepository, ICompressor compressor)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            if (backupDestinationRepository == null) throw new ArgumentNullException(nameof(backupDestinationRepository));
            var jobObjectsWithData = objects.Select(x => new JobObjectWithData(x, sourceRepository)).ToList();
            Storage storage = backupDestinationRepository.CreateStorage(jobObjectsWithData, compressor);
            return new RestorePoint(new List<Storage> { storage }, DateTime.Now, storage.Id, string.Empty);
        }
    }
}