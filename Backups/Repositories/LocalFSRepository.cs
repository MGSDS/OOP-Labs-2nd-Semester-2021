using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace Backups.Repositories
{
    public class LocalFSRepository : IRepository
    {
        private ICompress _compress;
        public LocalFSRepository(string repositoryPath, ICompress compressionAlg)
        {
            RepositoryPath = repositoryPath;
            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }

            _compress = compressionAlg;
        }

        private string RepositoryPath { get;  }

        public Storage CreateStorage(JobObject jobObject)
        {
            return CreateStorage(new List<JobObject>() { jobObject });
        }

        public Storage CreateStorage(IReadOnlyList<JobObject> jobObjects)
        {
            var id = Guid.NewGuid();
            string newFileName = $"{id.ToString()}.zip";
            FileStream stream = File.OpenWrite(Path.Combine(RepositoryPath, newFileName));
            _compress.Compress(jobObjects, stream);
            stream.Close();
            return new Storage(newFileName, RepositoryPath, id, jobObjects);
        }

        public IRepository CreateRepository(Guid id)
        {
            return new LocalFSRepository(Path.Combine(RepositoryPath, id.ToString()), _compress);
        }
    }
}