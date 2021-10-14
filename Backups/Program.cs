using System.Net;
using Backups.Algorithms;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups
{
    internal class Program
    {
        private static void Main()
        {
            var backup = new Backup();
            var repo = new TcpRepository(IPAddress.Parse("127.0.0.1"), 1234, new ZipCompressor());
            var algo = new MultipleFileRestorePointCreationalAlgorithm();
            var joba = new BackupJob(backup, repo, algo);
            string path = "/Users/bill/Documents/LINQPad 6.14.10";
            joba.AddObject(new JobObject(path, "LINQPad6-x86.exe"));
            joba.AddObject(new JobObject(path, "LPRun6-net5.dll"));
            joba.AddObject(new JobObject(path, "LPRun6.exe"));
            joba.CreateRestorePoint();
        }
    }
}
