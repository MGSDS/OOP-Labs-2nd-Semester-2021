using System;
using System.Collections.Generic;
using Backups.Repositories;

namespace Backups.Entities
{
    public class RestorePoint
    {
        public RestorePoint(IReadOnlyList<Storage> storages, DateTime backupTime, Guid id)
        {
            Storages = storages ?? throw new ArgumentNullException(nameof(storages));
            BackupTime = backupTime;
            Id = id;
        }

        public IReadOnlyList<Storage> Storages { get; }
        public Guid Id { get; }
        public DateTime BackupTime { get; }
    }
}