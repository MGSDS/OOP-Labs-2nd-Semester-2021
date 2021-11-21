using System.IO;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace BackupsExtra.Repository
{
    public class LocalFsExtraRepository : LocalFsRepository, IExtraRepository
    {
        public LocalFsExtraRepository(string repositoryPath, ICompressor compressionAlg)
            : base(repositoryPath, compressionAlg)
        {
        }

        public void Read(Storage storage, Stream stream)
        {
            string filePath = Path.Combine(RepositoryPath, storage.Path, storage.Name);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            using FileStream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.Position = 0;
            stream.SetLength(0);
            stream.Position = 0;
            fileStream.CopyTo(stream);
        }

        public void Write(Storage storage, Stream stream)
        {
            string filePath = Path.Combine(RepositoryPath, storage.Path, storage.Name);
            using FileStream fileStream = File.OpenWrite(filePath);
            fileStream.SetLength(0);
            fileStream.Position = 0;
            stream.Position = 0;
            stream.CopyTo(fileStream);
        }

        public void Delete(Storage storage)
        {
            string filePath = Path.Combine(RepositoryPath, storage.Path, storage.Name);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            File.Delete(filePath);
        }
    }
}