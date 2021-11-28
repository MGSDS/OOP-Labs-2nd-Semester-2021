using System;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.Repositories;
using BackupsExtra.ClearAlgorithms;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Logging;
using BackupsExtra.Repository;
using BackupsExtra.RestoreAlgorithms;
using BackupsExtraTcpClient;

namespace BackupsExtra
{
    internal static class Program
    {
        private static void Main()
        {
            LoggerSingletone.Initialize(new FileLogger("logPath", new LoggerSettings()
            {
                ShowTime = true,
            }));
            var algo = new SingleStorageRestorePointManageAlgorithm();
            var compressionAlgo = new ExtraZipCompressor();

            var backupDestinationRepository = new ServerExtraBackupDestinationRepository(new TcpObjectTransferClient("localhost", 1234));
            var backup = new ExtraBackup(algo, compressionAlgo, backupDestinationRepository, new BaseClearAlgorithm(), new LocalFileSystemSourceRepository());
            var joba = new ExtraBackupJob(backup);
            string path = "path";
            joba.AddObject(new JobObject(path, "object"));
            joba.CreateRestorePoint();
            Guid first = backup.RestorePoints.Last().Id;
            joba.RemoveObject(joba.JobObjects.First());
            joba.AddObject(new JobObject(path, "object"));
            joba.CreateRestorePoint();
            Guid second = backup.RestorePoints.Last().Id;
            backup.MergeRestorePoints(first, second);
            backup.Restore(backup.RestorePoints.Last().Id, new OriginalLocationRestoreAlgorithm());
            JsonWrapper.WriteJson("path", joba);
        }
    }
}
