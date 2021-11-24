using System;
using System.Collections.Generic;
using Backups.CompressionAlgorithms;
using Backups.CreationalAlgorithms;
using Backups.Repositories;

namespace Backups.Entities
{
    public class Backup
    {
        private readonly List<RestorePoint> _restorePoints;

        public Backup(
            IRestorePointCreationalAlgorithm restorePointCreationalAlgorithm,
            ICompressor compressor,
            IBackupDestinationRepository backupDestinationRepository,
            ISourceRepository sourceRepository)
            : this(restorePointCreationalAlgorithm, compressor, backupDestinationRepository, new List<RestorePoint>(), sourceRepository)
        {
        }

        protected internal Backup(
            IRestorePointCreationalAlgorithm restorePointCreationalAlgorithm,
            ICompressor compressor,
            IBackupDestinationRepository backupDestinationRepository,
            IReadOnlyList<RestorePoint> restorePoints,
            ISourceRepository sourceRepository)
        {
            RestorePointCreationalAlgorithm = restorePointCreationalAlgorithm;
            Compressor = compressor;
            BackupDestinationRepository = backupDestinationRepository;
            _restorePoints = restorePoints as List<RestorePoint>;
            SourceRepository = sourceRepository;
        }

        public ISourceRepository SourceRepository { get; }
        public IReadOnlyList<RestorePoint> RestorePoints => _restorePoints;

        public IRestorePointCreationalAlgorithm RestorePointCreationalAlgorithm { get; }
        public ICompressor Compressor { get; }
        public IBackupDestinationRepository BackupDestinationRepository { get; }

        public void Add(RestorePoint restorePoint)
        {
            if (restorePoint == null) throw new ArgumentNullException(nameof(restorePoint));
            _restorePoints.Add(restorePoint);
        }

        protected internal void Remove(RestorePoint restorePoint)
        {
            if (restorePoint == null) throw new ArgumentNullException(nameof(restorePoint));
            _restorePoints.Remove(restorePoint);
        }

        protected internal List<RestorePoint> GetRestorePoints() => _restorePoints;
    }
}