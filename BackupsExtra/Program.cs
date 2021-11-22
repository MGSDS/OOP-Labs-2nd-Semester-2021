using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using BackupsExtra.ClearAlgorithms;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Repository;
using BackupsExtra.RestoreAlgorithm;

namespace BackupsExtra
{
    internal static class Program
    {
        private static void Main()
        {
            // var algo = new SplitStorageRestorePointManageAlgorithm();
            // var compressionAlgo = new ExtraZipCompressor();
            // var repository = new LocalFsExtraRepository("/Users/bill/Desktop/backup");
            // var backup = new ExtraBackup(algo, compressionAlgo, repository);
            // var joba = new BackupJob(backup);
            // string path = "/Users/bill/Desktop";
            // joba.AddObject(new JobObject(path, "Screenshot 2021-11-21 at 5.10.05 PM.png"));
            // joba.CreateRestorePoint();
            // Guid first = backup.RestorePoints.Last().Id;
            // joba.RemoveObject(joba.JobObjects.First());
            // joba.AddObject(new JobObject(path, "Screenshot 2021-11-21 at 5.10.00 PM.png"));
            // joba.CreateRestorePoint();
            // Guid second = backup.RestorePoints.Last().Id;
            //
            // // // backup.MergeRestorePoints(first, second);
            // JsonWrapper.WriteJson("/Users/bill/Desktop/backup/settings2.json", new List<BackupJob> { joba });
             // Backup backup = JsonWrapper.ReadJson<BackupJob>("/Users/bill/Desktop/backup/settings2.json").First().Backup;
             // Guid first = backup.RestorePoints.First().Id;
             // (backup as ExtraBackup)?.Restore(first, new CustomPathRestoreAlgorithm(new DirectoryInfo("/Users/bill/Desktop/a")));

             // var a = (joba2.RestorePointCreationalAlgorithm as IRestorePointManageAlgorithm)?.Restore(joba2.RestorePoints.Last(), joba2.Repository as IExtraRepository, joba2.Compressor as IExtraCompressor);

            // ExtraBackup backup = JsonWrapper.ReadJsonBackups("/Users/bill/Desktop/backup/settings.json").First();
            // Guid first = backup.RestorePoints.First().Id;
            // Guid second = backup.RestorePoints.Last().Id;
            // backup.MergeRestorePoints(first, second);
            // JsonWrapper.WriteJson("/Users/bill/Desktop/backup/settings.json", new List<ExtraBackup> { backup });
            // var builder = new ClearAlgorithmBuilder();
            // builder.AddMaxCount(1);
            // var algo = new SingleStorageRestorePointManageAlgorithm();
            // var compressionAlgo = new ExtraZipCompressor();
            // var repository = new LocalFsExtraRepository("/Users/bill/Desktop/backup");
            // var backup = new ExtraBackup(algo, compressionAlgo, repository, builder.Build());
            // var joba = new ExtraBackupJob(backup);
            // string path = "/Users/bill/Desktop";
            // joba.AddObject(new JobObject(path, "Screenshot 2021-11-21 at 5.10.05 PM.png"));
            // joba.CreateRestorePoint();
            // joba.AddObject(new JobObject(path, "Screenshot 2021-11-22 at 5.07.26 PM.png"));
            // joba.CreateRestorePoint();
            // JsonWrapper.WriteJson("/Users/bill/Desktop/backup/settings.json", new List<ExtraBackupJob> { joba });
            List<ExtraBackupJob> backup = JsonWrapper.ReadJson<ExtraBackupJob>("/Users/bill/Desktop/backup/settings.json");
            Guid id = backup.First().Backup.RestorePoints.First().Id;
            (backup.First().Backup as ExtraBackup)?.Restore(id, new OriginalLocationRestoreAlgorithm());
        }
    }
}
