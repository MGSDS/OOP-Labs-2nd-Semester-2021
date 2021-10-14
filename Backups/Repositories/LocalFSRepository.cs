using System;
using System.Collections.Generic;
using System.IO;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace Backups.Repositories
{
    public class LocalFsRepository : IRepository
    {
        private ICompressor _compressor;
        private string _repositoryPath;
        public LocalFsRepository(string repositoryPath, ICompressor compressionAlg)
        {
            _repositoryPath = repositoryPath;
            if (!Directory.Exists(_repositoryPath))
            {
                Directory.CreateDirectory(_repositoryPath);
            }

            _compressor = compressionAlg;
        }

        public IReadOnlyList<Storage> CreateStorages(IReadOnlyList<JobObject> jobObjects, string folderName)
        {
            string path = OpenDirectory(folderName);
            var storages = new List<Storage>(jobObjects.Count);

            foreach (JobObject jobObject in jobObjects)
            {
                var id = Guid.NewGuid();
                string newFileName = $"{id.ToString()}.zip";
                FileStream stream = File.OpenWrite(Path.Combine(path, newFileName));
                _compressor.Compress(new List<JobObject> { jobObject }, stream);
                storages.Add(new Storage(newFileName, path, id, jobObjects));
                stream.Close();
            }

            return storages;
        }

        public Storage CreateStorage(IReadOnlyList<JobObject> jobObjects, string folderName = "")
        {
            string path = OpenDirectory(folderName);
            var id = Guid.NewGuid();
            string newFileName = $"{id.ToString()}.zip";
            FileStream stream = File.OpenWrite(Path.Combine(Path.Combine(path, folderName), newFileName));
            _compressor.Compress(jobObjects, stream);
            stream.Close();
            return new Storage(newFileName, path, id, jobObjects);
        }

        private string OpenDirectory(string folderName)
        {
            string path = Path.Combine(_repositoryPath, folderName);
            if (!Directory.Exists(Path.Combine(_repositoryPath, folderName)))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}