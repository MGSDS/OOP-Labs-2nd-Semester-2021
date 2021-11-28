using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Logging;
using BackupsExtra.Repository;
using BackupsExtra.RestoreAlgorithm;

namespace BackupsExtra.RestoreAlgorithms
{
    public class OriginalLocationRestoreAlgorithm : IRestoreAlgorithm
    {
        public void Restore(
            RestorePoint restorePoint,
            IRestorePointManageAlgorithm restorePointManageAlgorithm,
            IExtraBackupDestinationRepository backupDestinationRepository,
            IExtraCompressor compressor)
        {
            IReadOnlyList<RestoreItem> files = restorePointManageAlgorithm.Restore(restorePoint, backupDestinationRepository, compressor);
            foreach (RestoreItem restoreItem in files)
            {
                try
                {
                    var directoryInfo = new DirectoryInfo(restoreItem.JobObject.Path);
                }
                catch (Exception)
                {
                    throw new DataException($"folder {restoreItem.JobObject.Path} can not be accessed or created");
                }
            }

            foreach (RestoreItem restoreItem in files)
            {
                if (System.IO.File.Exists(restoreItem.JobObject.FullPath))
                    LoggerSingletone.GetInstance().Write(new LoggerMessage($"File {restoreItem.JobObject.FullPath} will be rewrited", messageType: MessageType.Warning));
                using FileStream stream = System.IO.File.OpenWrite(restoreItem.JobObject.FullPath);
                stream.SetLength(0);
                stream.Position = 0;
                using MemoryStream fileStream = restoreItem.File.Stream;
                fileStream.Position = 0;
                fileStream.CopyTo(stream);
                LoggerSingletone.GetInstance().Write(new LoggerMessage($"File {restoreItem.JobObject.FullPath} successfully restored"));
                restoreItem.Dispose();
            }
        }
    }
}