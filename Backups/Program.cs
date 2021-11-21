using System.Net;
using Backups.CompressionAlgorithms;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups
{
    internal static class Program
    {
        private static void Main()
        {
            var repository = new TcpRepository(IPAddress.Parse("127.0.0.1"), 1234);
            var algo = new SplitStorageRestorePointCreationalAlgorithm();
            var backup = new Backup(algo, new ZipCompressor(), repository);
            var joba = new BackupJob(backup);
            string path = "path";
            joba.AddObject(new JobObject(path, "file1"));
            joba.AddObject(new JobObject(path, "file2"));
            joba.AddObject(new JobObject(path, "file3"));
            joba.CreateRestorePoint();
        }
    }
}
