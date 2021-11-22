using System;
using System.Collections.Generic;
using Backups.Entities;
using BackupsExtra.Logging;
using Newtonsoft.Json;

namespace BackupsExtra.Entities
{
    public class ExtraBackupJob : BackupJob
    {
        public ExtraBackupJob(ExtraBackup backup)
            : base(backup)
        {
            if (!LoggerSingletone.IsInitialized)
                throw new InvalidOperationException("LoggerSingletone is required to be initializer");
            Id = Guid.NewGuid();
            LoggerSingletone.GetInstance().Write(new LoggerMessage($"Job {Id} created"));
        }

        [JsonConstructor]
        private ExtraBackupJob(ExtraBackup backup, List<JobObject> jobObjects, Guid id)
            : base(backup, jobObjects)
        {
            Id = id;
        }

        public Guid Id { get; }

        public override void AddObject(JobObject jobObject)
        {
            base.AddObject(jobObject);
            LoggerSingletone.GetInstance().Write(new LoggerMessage($"Added object {jobObject.FullPath} to job {Id}"));
        }

        public override void RemoveObject(JobObject jobObject)
        {
            try
            {
                base.RemoveObject(jobObject);
            }
            catch (Exception e)
            {
                LoggerSingletone.GetInstance().Write(new LoggerMessage(e));
                throw;
            }

            LoggerSingletone.GetInstance().Write(new LoggerMessage($"Removed object {jobObject.FullPath} from job {Id}"));
            base.RemoveObject(jobObject);
        }

        public override void CreateRestorePoint()
        {
            LoggerSingletone.GetInstance().Write(new LoggerMessage($"Started restore point creation {Id}"));
            try
            {
                base.CreateRestorePoint();
                LoggerSingletone.GetInstance().Write(new LoggerMessage($"Created restore point for job {Id}"));
            }
            catch (Exception e)
            {
                LoggerSingletone.GetInstance().Write(new LoggerMessage(e));
                throw;
            }

            LoggerSingletone.GetInstance().Write(new LoggerMessage($"Restore point creation stopped {Id}"));
            (Backup as ExtraBackup)?.Clear();
        }
    }
}