using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Backups.Entities
{
    public class BackupJob
    {
        private List<JobObject> _jobObjects;

        [JsonConstructor]
        public BackupJob(Backup backup, List<JobObject> jobObjects)
        {
            Backup = backup ?? throw new ArgumentNullException(nameof(backup));
            _jobObjects = jobObjects;
        }

        public BackupJob(Backup backup)
            : this(backup, new List<JobObject>()) { }

        public Backup Backup { get; }
        public IReadOnlyList<JobObject> JobObjects => _jobObjects.AsReadOnly();

        public virtual void AddObject(JobObject jobObject)
        {
            if (JobObjects.Any(obj => obj.Name == jobObject.Name))
                throw new ArgumentException("Backup not contain files with similar names");
            _jobObjects.Add(jobObject);
        }

        public virtual void RemoveObject(JobObject jobObject) => _jobObjects.Remove(jobObject);

        public virtual void CreateRestorePoint()
        {
            Backup.Add(Backup.RestorePointCreationalAlgorithm.Run(_jobObjects, Backup.Repository, Backup.Compressor));
        }
    }
}