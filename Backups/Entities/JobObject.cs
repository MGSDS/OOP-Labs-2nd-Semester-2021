using System;
using Newtonsoft.Json;

namespace Backups.Entities
{
    public class JobObject
    {
        public JobObject(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            Name = System.IO.Path.GetFileName(path);
            Path = System.IO.Path.GetDirectoryName(path);
        }

        [JsonConstructor]
        public JobObject(string path, string name)
            : this(path: System.IO.Path.Combine(path, name))
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public string Path { get; }

        [JsonIgnore]
        public string FullPath => System.IO.Path.Combine(Path, Name);
    }
}