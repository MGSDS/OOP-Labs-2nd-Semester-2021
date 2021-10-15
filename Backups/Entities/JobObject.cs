namespace Backups.Entities
{
    public class JobObject
    {
        public JobObject(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = System.IO.Path.GetDirectoryName(path);
        }

        public JobObject(string path, string name)
            : this(path: System.IO.Path.Combine(path, name)) { }

        public string Name { get; }

        public string Path { get; }
    }
}