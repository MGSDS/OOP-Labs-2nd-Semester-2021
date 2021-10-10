using System.IO;

namespace Backups.Entities
{
    public class JobObject
    {
        public JobObject(string path)
        {
            var fileInfo = new FileInfo(path);
            Name = fileInfo.Name;
            Path = fileInfo.Name;
        }

        public JobObject(string path, string name)
            : this(path: System.IO.Path.Combine(path, name)) { }

        public string Name { get; }

        public string Path { get; }
    }
}