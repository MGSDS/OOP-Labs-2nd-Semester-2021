using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace Backups.Repositories
{
    public class LocalFsBackupDestinationRepository : IBackupDestinationRepository
    {
        public LocalFsBackupDestinationRepository(string repositoryPath)
        {
            RepositoryPath = repositoryPath ?? throw new ArgumentNullException(nameof(repositoryPath));
            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }
        }

        public string RepositoryPath { get; }

        public Storage CreateStorage(List<JobObjectWithData> jobObjects, ICompressor compressor, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = OpenDirectory(folderName);
            var id = Guid.NewGuid();
            string newFileName = $"{id.ToString()}.zip";
            FileStream stream = System.IO.File.OpenWrite(Path.Combine(path, newFileName));
            compressor.Compress(jobObjects.Select(x => x.File).ToList(), stream);
            stream.Close();
            return new Storage(newFileName, folderName, id, jobObjects.Select(x => x.JobObject).ToList());
        }

        private string OpenDirectory(string folderName)
        {
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = Path.Combine(RepositoryPath, folderName);
            if (!Directory.Exists(Path.Combine(RepositoryPath, folderName)))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}