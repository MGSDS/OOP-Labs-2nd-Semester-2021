using System.Net;
using Backups.CompressionAlgorithms;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups
{
    internal class Program
    {
        private static void Main()
        {
            var backup = new Backup();
            var repository = new TcpRepository(IPAddress.Parse("127.0.0.1"), 1234, new ZipCompressor());
            var algo = new SplitStorageRestorePointCreationalAlgorithm();
            var joba = new BackupJob(backup, repository, algo);
            string path = "path";
            joba.AddObject(new JobObject(path, "file1"));
            joba.AddObject(new JobObject(path, "file2"));
            joba.AddObject(new JobObject(path, "file3"));
            joba.CreateRestorePoint();
        }
    }
}
