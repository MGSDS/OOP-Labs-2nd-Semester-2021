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
        public Backup(IRestorePointCreationalAlgorithm restorePointCreationalAlgorithm, ICompressor compressor, IRepository repository)
        : this(restorePointCreationalAlgorithm, compressor, repository, new List<RestorePoint>())
        {
        }

        protected internal Backup(IRestorePointCreationalAlgorithm restorePointCreationalAlgorithm, ICompressor compressor, IRepository repository, IReadOnlyList<RestorePoint> restorePoints)
        {
            RestorePointCreationalAlgorithm = restorePointCreationalAlgorithm;
            Compressor = compressor;
            Repository = repository;
            _restorePoints = restorePoints as List<RestorePoint>;
        }

        public IReadOnlyList<RestorePoint> RestorePoints => _restorePoints;

        public IRestorePointCreationalAlgorithm RestorePointCreationalAlgorithm { get; }
        public ICompressor Compressor { get; }
        public IRepository Repository { get; }

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
    }
}