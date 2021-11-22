using System;
using Backups.Entities;

namespace BackupsExtra.Entities
{
    public class RestoreItem : IDisposable
    {
        public RestoreItem(BackupsExtra.Entities.File file, JobObject jobObject)
        {
            JobObject = jobObject;
            File = file;
        }

        public BackupsExtra.Entities.File File { get; }
        public JobObject JobObject { get; }

        public void Dispose()
        {
            File?.Dispose();
        }
    }
}