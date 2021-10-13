using System.Collections.Generic;
using System.IO;
using Backups.Server.Entities;
using Directory = System.IO.Directory;

namespace Backups.Server.Repositories
{
    public class LocalFSRepository : IServerRepository
    {
        public LocalFSRepository(string repositoryPath)
        {
            RepositoryPath = repositoryPath;
            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }
        }

        public string RepositoryPath { get; }

        public void Save(TransferFile transferFile)
        {
            FileStream fileStream = File.Create(Path.Combine(RepositoryPath, transferFile.Name));
            transferFile.Stream.Position = 0;
            transferFile.Stream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
        }

        public void Save(IReadOnlyList<TransferFile> transferFile)
        {
            foreach (TransferFile file in transferFile)
            {
                Save(file);
            }
        }

        public IServerRepository CreateInnerRepository(string name)
        {
            return new LocalFSRepository(Path.Combine(RepositoryPath, name));
        }
    }
}