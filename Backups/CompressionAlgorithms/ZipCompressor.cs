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
            var archive = new ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true);
            foreach (JobObject obj in objects)
                archive.CreateEntryFromFile(Path.Combine(obj.Path, obj.Name), obj.Name, CompressionLevel.Optimal);
        }
    }
}