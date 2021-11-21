using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace Backups.Repositories
{
    public class LocalFsRepository : IRepository
    {
        public LocalFsRepository(string repositoryPath, ICompressor compressionAlg)
        {
            if (compressionAlg == null) throw new ArgumentNullException(nameof(compressionAlg));
            RepositoryPath = repositoryPath ?? throw new ArgumentNullException(nameof(repositoryPath));
            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }

            Compressor = compressionAlg;
        }

        public ICompressor Compressor { get; }
        public string RepositoryPath { get; }

        public Storage CreateStorage(IReadOnlyList<JobObject> jobObjects, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = OpenDirectory(folderName);
            var id = Guid.NewGuid();
            string newFileName = $"{id.ToString()}.zip";
            FileStream stream = OpenFile(Path.Combine(path, newFileName));
            Compressor.Compress(jobObjects, stream);
            stream.Close();
            return new Storage(newFileName, folderName, id, jobObjects);
        }

        private string OpenDirectory(string folderName)
        {
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = Path.Combine(RepositoryPath, folderName);
            if (!Directory.Exists(Path.Combine(RepositoryPath, folderName)))
                Directory.CreateDirectory(path);
            return path;
        }

        private FileStream OpenFile(string path)
        {
            if (File.Exists(path)) throw new DataException("File already exists");
            return File.OpenWrite(path);
        }
    }
}