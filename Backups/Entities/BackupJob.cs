using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Algorithms;
using Backups.Repositories;

namespace Backups.Entities
{
    public class BackupJob
    {
        private IRepository _repository;
        private List<JobObject> _jobObjects;
        private IRestorePointCreationalAlgorithm _restorePointCreationalAlgorithm;

        public BackupJob(Backup backup, IRepository repository, List<JobObject> jobObjects, IRestorePointCreationalAlgorithm restorePointCreationalAlgorithm)
        {
            Backup = backup;
            _repository = repository;
            _jobObjects = jobObjects;
            _restorePointCreationalAlgorithm = restorePointCreationalAlgorithm;
        }

        public BackupJob(Backup backup, IRepository repository, IRestorePointCreationalAlgorithm restorePointCreationalAlgorithm)
            : this(backup, repository, new List<JobObject>(), restorePointCreationalAlgorithm) { }

        public Backup Backup { get; }
        public IReadOnlyList<JobObject> JobObjects => _jobObjects;

        public void AddObject(JobObject jobObject)
        {
            if (JobObjects.Any(obj => obj.Name == jobObject.Name))
                throw new Exception("Backup not contain files with similar names");
            _jobObjects.Add(jobObject);
        }

        public void RemoveObject(JobObject jobObject) => _jobObjects.Remove(jobObject);

        public void CreateRestorePoint()
        {
            Backup.AddRestorePoint(_restorePointCreationalAlgorithm.Run(_jobObjects, _repository));
        }
    }
}