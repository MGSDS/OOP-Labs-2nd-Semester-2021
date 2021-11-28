using System.IO;
using File = Backups.Entities.File;

namespace Backups.Repositories
{
    public class LocalFileSystemSourceRepository : ISourceRepository
    {
        public File ReadFile(string path)
        {
            var fileInfo = new FileInfo(path);
            using FileStream fs = System.IO.File.OpenRead(fileInfo.FullName);
            return new File(fs, fileInfo.Name);
        }
    }
}