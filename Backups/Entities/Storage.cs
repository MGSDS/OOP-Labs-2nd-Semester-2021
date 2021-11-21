using System;
using System.Collections.Generic;

namespace Backups.Entities
{
    public class Storage
    {
        public Storage(string name, string path, Guid id, List<JobObject> jobObjects)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Id = id;
            JobObjects = jobObjects;
        }

        public string Name { get; }
        public string Path { get; }
        public List<JobObject> JobObjects { get; }

        public Guid Id { get; }
    }
}