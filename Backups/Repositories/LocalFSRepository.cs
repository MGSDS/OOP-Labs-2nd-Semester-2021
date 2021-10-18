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
        private readonly ICompressor _compressor;
        private readonly string _repositoryPath;
        public LocalFsRepository(string repositoryPath, ICompressor compressionAlg)
        {
            if (compressionAlg == null) throw new ArgumentNullException(nameof(compressionAlg));
            _repositoryPath = repositoryPath ?? throw new ArgumentNullException(nameof(repositoryPath));
            if (!Directory.Exists(_repositoryPath))
            {
                Directory.CreateDirectory(_repositoryPath);
            }

            _compressor = compressionAlg;
        }

        public IReadOnlyList<Storage> CreateStorages(IReadOnlyList<JobObject> jobObjects, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = OpenDirectory(folderName);
            var storages = new List<Storage>(jobObjects.Count);

            foreach (JobObject jobObject in jobObjects)
            {
                var id = Guid.NewGuid();
                string newFileName = $"{id.ToString()}.zip";
                string filePath = Path.Combine(path, newFileName);
                FileStream stream = OpenFile(filePath);
                _compressor.Compress(new List<JobObject> { jobObject }, stream);
                storages.Add(new Storage(newFileName, path, id, jobObjects));
                stream.Close();
            }

            return storages;
        }

        public Storage CreateStorage(IReadOnlyList<JobObject> jobObjects, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = OpenDirectory(folderName);
            var id = Guid.NewGuid();
            string newFileName = $"{id.ToString()}.zip";
            FileStream stream = OpenFile(Path.Combine(Path.Combine(path, folderName), newFileName));
            _compressor.Compress(jobObjects, stream);
            stream.Close();
            return new Storage(newFileName, path, id, jobObjects);
        }

        private string OpenDirectory(string folderName)
        {
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            string path = Path.Combine(_repositoryPath, folderName);
            if (!Directory.Exists(Path.Combine(_repositoryPath, folderName)))
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