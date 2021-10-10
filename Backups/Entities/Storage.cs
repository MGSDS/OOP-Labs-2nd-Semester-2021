using System;
using System.Collections.Generic;

namespace Backups.Entities
{
    public class Storage
    {
        public Storage(string name, string path, Guid id)
        {
            Name = name;
            Path = path;
            Id = id;
        }

        public string Name { get; }

        public string Path { get; }

        public Guid Id { get; }
    }
}