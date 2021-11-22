using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Logging;
using BackupsExtra.Repository;

namespace BackupsExtra.RestoreAlgorithm
{
    public class CustomPathRestoreAlgorithm : IRestoreAlgorithm
    {
        public CustomPathRestoreAlgorithm(DirectoryInfo directory)
        {
            Directory = directory;
        }

        public DirectoryInfo Directory { get; }

        public void Restore(
            RestorePoint restorePoint,
            IRestorePointManageAlgorithm restorePointManageAlgorithm,
            IExtraRepository repository,
            IExtraCompressor compressor)
        {
            IReadOnlyList<RestoreItem> files = restorePointManageAlgorithm.Restore(restorePoint, repository, compressor);

            foreach (RestoreItem restoreItem in files)
            {
                string path = Path.Combine(Directory.FullName, restoreItem.JobObject.Name);
                if (!System.IO.File.Exists(path))
                {
                    using FileStream stream = System.IO.File.Create(path);
                    using MemoryStream fileStream = restoreItem.File.Stream;
                    fileStream.Position = 0;
                    fileStream.CopyTo(stream);
                    LoggerSingletone.GetInstance().Write(new LoggerMessage($"File {path} successfully restored"));
                }

                restoreItem.Dispose();
                LoggerSingletone.GetInstance().Write(new LoggerMessage($"File {path} already exists, File {restoreItem.JobObject.FullPath} wont be restores", messageType: MessageType.Warning));
            }
        }
    }
}