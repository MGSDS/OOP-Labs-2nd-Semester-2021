using System;
using System.Linq;
using System.Net;
using Backups.CompressionAlgorithms;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using Backups.Repositories;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Repository;

namespace BackupsExtra
{
    internal class Program
    {
        private static void Main()
        {
            var algo = new SplitStorageRestorePointManageAlgorithm();
            var compressionAlgo = new ExtraZipCompressor();
            var repository = new LocalFsExtraRepository("/Users/bill/Desktop/backup", compressionAlgo);
            var backup = new ExtraBackup(algo, compressionAlgo, repository);
            var joba = new BackupJob(backup);
            string path = "/Users/bill/Desktop";
            joba.AddObject(new JobObject(path, "Screenshot 2021-11-21 at 5.10.05 PM.png"));
            joba.CreateRestorePoint();
            Guid first = backup.RestorePoints.Last().Id;
            joba.RemoveObject(joba.JobObjects.First());
            joba.AddObject(new JobObject(path, "Screenshot 2021-11-21 at 5.10.00 PM.png"));
            joba.CreateRestorePoint();
            Guid second = backup.RestorePoints.Last().Id;
            backup.MergeRestorePoints(first, second);
        }
    }
}
