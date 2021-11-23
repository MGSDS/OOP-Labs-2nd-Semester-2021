using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Repository;
using File = Backups.Entities.File;

namespace BackupsExtra.CreationalAlgorithms
{
    public class SplitStorageRestorePointManageAlgorithm : SplitStorageRestorePointCreationalAlgorithm, IRestorePointManageAlgorithm
    {
        public void Merge(RestorePoint source, RestorePoint destination, IExtraBackupDestinationRepository backupDestinationRepository, IExtraCompressor compressor)
        {
            foreach (Storage sourceStorage in source.Storages.Where(sourceStorage
                => destination.Storages.All(x => x.JobObjects.First<JobObject>().FullPath != sourceStorage.JobObjects.First().FullPath)))
            {
                using var memoryStream = new MemoryStream();
                backupDestinationRepository.Read(sourceStorage, memoryStream);
                var storage = new Storage(sourceStorage.Name, destination.Path, sourceStorage.Id, sourceStorage.JobObjects);
                backupDestinationRepository.Write(storage, memoryStream);
                destination.Storages.Add(storage);
                backupDestinationRepository.Delete(sourceStorage);
            }
        }

        public void Delete(RestorePoint target, IExtraBackupDestinationRepository backupDestinationRepository)
        {
            foreach (Storage targetStorage in target.Storages)
            {
                backupDestinationRepository.Delete(targetStorage);
            }
        }

        public IReadOnlyList<RestoreItem> Restore(RestorePoint target, IExtraBackupDestinationRepository backupDestinationRepository, IExtraCompressor compressor)
        {
            var restoreItems = new List<RestoreItem>();
            foreach (Storage targetStorage in target.Storages)
            {
                IReadOnlyList<File> files = compressor.Decompress(targetStorage, backupDestinationRepository);
                foreach (JobObject targetStorageJobObject in targetStorage.JobObjects)
                {
                    File file = files.FirstOrDefault(x => x.Name == targetStorageJobObject.Name)
                                ?? throw new DataException($"File with name {targetStorageJobObject.Name} in storage");
                    restoreItems.Add(new RestoreItem(file, targetStorageJobObject));
                }
            }

            return restoreItems;
        }
    }
}