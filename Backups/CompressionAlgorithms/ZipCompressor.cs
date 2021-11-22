using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Entities;

namespace Backups.CompressionAlgorithms
{
    public class ZipCompressor : ICompressor
    {
        public void Compress(IReadOnlyList<JobObject> objects, Stream stream)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);
            foreach (JobObject obj in objects)
            {
                if (!File.Exists(obj.FullPath))
                    throw new InvalidOperationException($"File {obj.FullPath} does not exists");
                archive.CreateEntryFromFile(Path.Combine(obj.Path, obj.Name), obj.Name, CompressionLevel.Optimal);
            }
        }
    }
}