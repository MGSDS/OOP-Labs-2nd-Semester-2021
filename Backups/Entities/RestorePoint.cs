using System;
using System.Collections.Generic;
using Backups.Repositories;

namespace Backups.Entities
{
    public class RestorePoint
    {
        public RestorePoint(IReadOnlyList<Storage> storages, DateTime backupTime, Guid id, string path)
        {
            Storages = (List<Storage>)(storages ?? throw new ArgumentNullException(nameof(storages)));
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