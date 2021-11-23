using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Entities;
using File = Backups.Entities.File;

namespace Backups.CompressionAlgorithms
{
    public class ZipCompressor : ICompressor
    {
        public void Compress(IReadOnlyList<File> files, Stream stream)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);
            foreach (File file in files)
            {
                ZipArchiveEntry entry = archive.CreateEntry(file.Name);
                using Stream fileStream = entry.Open();
                using var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(file.Stream);
            }
        }
    }
}