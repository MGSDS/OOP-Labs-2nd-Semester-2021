using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.CreationalAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Repository;

namespace BackupsExtra.RestoreAlgorithm
{
    public class OriginalLocationRestoreAlgorithm : IRestoreAlgorithm
    {
        public void Restore(
            RestorePoint restorePoint,
            IRestorePointManageAlgorithm restorePointManageAlgorithm,
            IExtraRepository repository,
            IExtraCompressor compressor)
        {
            IReadOnlyList<RestoreItem> files = restorePointManageAlgorithm.Restore(restorePoint, repository, compressor);
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
                // TODO: file rewrite logger
                using FileStream stream = System.IO.File.OpenWrite(restoreItem.JobObject.FullPath);
                stream.SetLength(0);
                stream.Position = 0;
                using MemoryStream fileStream = restoreItem.File.Stream;
                fileStream.Position = 0;
                fileStream.CopyTo(stream);
                restoreItem.Dispose();
            }
        }
    }
}