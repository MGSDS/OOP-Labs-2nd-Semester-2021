using System;
using System.Collections.Generic;
using System.IO;
using Backups.NetworkTransfer.Entities;
using Directory = System.IO.Directory;

namespace Backups.Server.Repositories
{
    public class LocalFSRepository : IServerRepository
    {
        public LocalFSRepository(string repositoryPath)
        {
            RepositoryPath = repositoryPath ?? throw new ArgumentNullException(nameof(repositoryPath));
            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }
        }

        public string RepositoryPath { get; }

        public void Save(IReadOnlyList<TransferFile> transferFiles, string folderName)
        {
            if (transferFiles == null) throw new ArgumentNullException(nameof(transferFiles));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = OpenDirectory(folderName);
            foreach (TransferFile transferFile in transferFiles)
            {
                FileStream file = File.OpenWrite(Path.Combine(path, transferFile.Name));
                transferFile.Stream.Position = 0;
                transferFile.Stream.CopyTo(file);
                file.Flush();
                file.Close();
            }
        }

        private string OpenDirectory(string dirName)
        {
            if (dirName == null) throw new ArgumentNullException(nameof(dirName));
            string path = Path.Combine(RepositoryPath, dirName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}