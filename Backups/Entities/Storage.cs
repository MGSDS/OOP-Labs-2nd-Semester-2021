using System;
using System.Collections.Generic;

namespace Backups.Entities
{
    public class Storage
    {
        public Storage(string name, string path, Guid id, IReadOnlyList<JobObject> jobObjects)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Id = id;
            JobObjects = jobObjects ?? throw new ArgumentNullException(nameof(jobObjects));
        }

        public string Name { get; }

        public string Path { get; }

        public IReadOnlyList<JobObject> JobObjects { get; }

        public Guid Id { get; }
    }
}