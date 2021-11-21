using System;
using System.Collections.Generic;
using Backups.Repositories;

namespace Backups.Entities
{
    public class RestorePoint
    {
        public RestorePoint(List<Storage> storages, DateTime backupTime, Guid id, string path)
        {
            Storages = storages;
            BackupTime = backupTime;
            Path = path;
            Id = id;
        }

        public string Path { get; }
        public List<Storage> Storages { get; }
        public Guid Id { get; }
        public DateTime BackupTime { get; }
    }
}