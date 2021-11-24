using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using Backups.Repositories;
using BackupsExtra.ClearAlgorithms;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Logging;
using BackupsExtra.Repository;
using BackupsExtra.RestoreAlgorithm;
using Newtonsoft.Json;

namespace BackupsExtra.Entities
{
    public class ExtraBackup : Backup
    {
        public ExtraBackup(
            IRestorePointManageAlgorithm restorePointCreationalAlgorithm,
            IExtraCompressor compressor,
            IExtraBackupDestinationRepository backupDestinationRepository,
            IClearAlgorithm clearAlgorithm,
            ISourceRepository sourceRepository)
            : base(restorePointCreationalAlgorithm, compressor, backupDestinationRepository, sourceRepository)
        {
            if (!LoggerSingletone.IsInitialized)
                throw new InvalidOperationException("LoggerSingletone is required to be initializer");
            ClearAlgorithm = clearAlgorithm;
            Id = Guid.NewGuid();
            LoggerSingletone.GetInstance().Write(new LoggerMessage($"Extra Backup created with id {Id}"));
        }

        [JsonConstructor]
        private ExtraBackup(
            IRestorePointManageAlgorithm restorePointCreationalAlgorithm,
            IExtraCompressor compressor,
            IExtraBackupDestinationRepository backupDestinationRepository,
            List<RestorePoint> restorePoints,
            IClearAlgorithm clearAlgorithm,
            ISourceRepository sourceRepository,
            Guid id)
            : base(restorePointCreationalAlgorithm, compressor, backupDestinationRepository, restorePoints, sourceRepository)
        {
            if (!LoggerSingletone.IsInitialized)
                throw new InvalidOperationException("LoggerSingleton is required to be initializer");
            Id = id;
            ClearAlgorithm = clearAlgorithm;
        }

        public IClearAlgorithm ClearAlgorithm { get; }
        public Guid Id { get; }

        public void MergeRestorePoints(Guid first, Guid second)
        {
            try
            {
                RestorePoint firstRestorePoint = RestorePoints.FirstOrDefault(x => x.Id == first)
                                                 ?? throw new ArgumentException($"Restore point with id {first} not found");
                RestorePoint secondRestorePoint = RestorePoints.FirstOrDefault(x => x.Id == second)
                                                  ?? throw new ArgumentException($"Restore point with id {second} not found");
                if ((secondRestorePoint.BackupTime - firstRestorePoint.BackupTime).TotalMilliseconds > 0)
                {
                    (secondRestorePoint, firstRestorePoint) = (firstRestorePoint, secondRestorePoint);
                }

                (RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm)?.Merge(
                    secondRestorePoint,
                    firstRestorePoint,
                    BackupDestinationRepository as IExtraBackupDestinationRepository,
                    Compressor as IExtraCompressor);
                LoggerSingletone.GetInstance().Write(new LoggerMessage($"RestorePoints {first} {second} successfully merged"));
                Remove(secondRestorePoint);
            }
            catch (Exception e)
            {
                LoggerSingletone.GetInstance().Write(new LoggerMessage(e));
                throw;
            }
        }

        public void DeleteRestorePoint(Guid id)
        {
            try
            {
                RestorePoint restorePoint = RestorePoints.FirstOrDefault(x => x.Id == id)
                                            ?? throw new ArgumentException($"Restore point with id {id} not found");
                (RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm)?.Delete(restorePoint, BackupDestinationRepository as IExtraBackupDestinationRepository);
                LoggerSingletone.GetInstance().Write(new LoggerMessage($"RestorePoint {id} successfully deleted"));
                Remove(restorePoint);
            }
            catch (Exception e)
            {
                LoggerSingletone.GetInstance().Write(new LoggerMessage(e));
                throw;
            }
        }

        public void Restore(Guid pointId, IRestoreAlgorithm restoreAlgorithm)
        {
            try
            {
                RestorePoint restorePoint = RestorePoints.FirstOrDefault(x => x.Id == pointId)
                                            ?? throw new ArgumentException(
                                                $"Restore point with id {pointId} not found");
                restoreAlgorithm.Restore(
                    restorePoint,
                    RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm,
                    BackupDestinationRepository as IExtraBackupDestinationRepository,
                    Compressor as IExtraCompressor);
                LoggerSingletone.GetInstance().Write(
                    new LoggerMessage($"Restore point {pointId} successfully restored using {restoreAlgorithm.GetType().Name}"));
            }
            catch (Exception e)
            {
                LoggerSingletone.GetInstance().Write(new LoggerMessage(e));
                throw;
            }
        }

        public void Clear()
        {
            try
            {
                ClearAlgorithm.Clear(
                    RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm,
                    BackupDestinationRepository as IExtraBackupDestinationRepository,
                    GetRestorePoints());
            }
            catch (Exception e)
            {
                LoggerSingletone.GetInstance().Write(new LoggerMessage(e));
                throw;
            }
        }
    }
}