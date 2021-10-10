using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Entities;

namespace Backups.Repositories
{
    public class LocalFSRepository : IRepository
    {
        public LocalFSRepository(string repositoryPath)
        {
            RepositoryPath = repositoryPath;
            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }
        }

        private string RepositoryPath { get;  }

        public Storage CreateStorage(JobObject jobObject)
        {
            var id = Guid.NewGuid();
            string oldPath = Path.Combine(jobObject.Path, jobObject.Name);
            var file = new FileInfo(oldPath);
            string newFileName = $"{id.ToString()}.{file.Extension}";
            string newPath = Path.Combine(RepositoryPath, newFileName);
            File.Copy(oldPath, newPath);
            return new Storage(newFileName, RepositoryPath, Guid.NewGuid());
        }

        public Storage CreateStorage(List<JobObject> jobObjects)
        {
            Guid id = Guid.NewGuid();
            var tempRepository = CreateRepository(id) as LocalFSRepository;
            tempRepository.ClearRepository();
            foreach (JobObject jobObject in jobObjects)
            {
                tempRepository.CreateStorage(jobObject);
            }

            ZipFile.CreateFromDirectory(tempRepository.RepositoryPath, $"{RepositoryPath}/{id}.zip");
            tempRepository.RemoveRepository();
            return new Storage($"{id.ToString()}.zip", RepositoryPath, id);
        }

        public IRepository CreateRepository(Guid id)
        {
            return new LocalFSRepository(Path.Combine(RepositoryPath, id.ToString()));
        }

        private void ClearRepository()
        {
            var dir = new DirectoryInfo(RepositoryPath);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void RemoveRepository()
        {
            var dir = new DirectoryInfo(RepositoryPath);
            dir.Delete(true);
        }
    }
}