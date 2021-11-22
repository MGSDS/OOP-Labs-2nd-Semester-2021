using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Backups.CreationalAlgorithms;
using Backups.Entities;
using BackupsExtra.CompressionAlgorithms;
using BackupsExtra.Entities;
using BackupsExtra.Repository;
using File = BackupsExtra.Entities.File;

namespace BackupsExtra.CreationalAlgorithms
{
    public class SingleStorageRestorePointManageAlgorithm : SingleStorageRestorePointCreationalAlgorithm, IRestorePointManageAlgorithm
    {
        public void Merge(RestorePoint source, RestorePoint destination, IExtraRepository repository, IExtraCompressor compressor)
        {
            if (source.Storages.Count != 1 || destination.Storages.Count != 1)
                throw new InvalidOperationException("Merge is only supported for single storage restore points");
            using Stream sourceStream = new MemoryStream();
            repository.Read(destination.Storages[0], sourceStream);
            using Stream destinationStream = new MemoryStream();
            repository.Read(source.Storages[0], destinationStream);
            compressor.Merge(destinationStream, sourceStream);
            repository.Write(destination.Storages[0], destinationStream);
            destination.Storages[0].JobObjects.AddRange(source.Storages[0].JobObjects.Where(x
                => !destination.Storages[0].JobObjects.Contains(x)));
            repository.Delete(source.Storages[0]);
        }

        public void Delete(RestorePoint target, IExtraRepository repository)
        {
            var deletedStorages = new List<Storage>();
            foreach (Storage targetStorage in target.Storages)
            {
                try
                {
                    repository.Delete(targetStorage);
                }
                catch (Exception)
                {
                    foreach (Storage deletedStorage in deletedStorages)
                    {
                        target.Storages.Remove(deletedStorage);
                    }

                    throw;
                }

                deletedStorages.Add(targetStorage);
            }

            foreach (Storage deletedStorage in deletedStorages)
            {
                target.Storages.Remove(deletedStorage);
            }
        }

        public IReadOnlyList<RestoreItem> Restore(RestorePoint target, IExtraRepository repository, IExtraCompressor compressor) // TODO: LINQ
        {
            var restoreItems = new List<RestoreItem>();
            foreach (Storage targetStorage in target.Storages)
            {
                IReadOnlyList<File> files = compressor.Decompress(targetStorage, repository);
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