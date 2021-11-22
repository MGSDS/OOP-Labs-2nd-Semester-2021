using System.Collections.Generic;
using Backups.Entities;
using Newtonsoft.Json;

namespace BackupsExtra.Entities
{
    public class ExtraBackupJob : BackupJob
    {
        [JsonConstructor]
        public ExtraBackupJob(ExtraBackup backup, List<JobObject> jobObjects)
            : base(backup, jobObjects)
        {
        }

        public ExtraBackupJob(ExtraBackup backup)
            : base(backup)
        {
        }

        public override void CreateRestorePoint()
        {
            base.CreateRestorePoint();
            (Backup as ExtraBackup)?.Clear();
        }
    }
}