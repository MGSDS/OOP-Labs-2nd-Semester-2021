using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.CompressionAlgorithms;

namespace BackupsExtra.CompressionAlgorithms
{
    public class ExtraZipCompressor : ZipCompressor, IExtraCompressor
    {
        public void Merge(Stream destination, Stream source)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (source == null) throw new ArgumentNullException(nameof(source));
            using var archive = new ZipArchive(destination, ZipArchiveMode.Update, leaveOpen: true);
            using var sourceArchive = new ZipArchive(source, ZipArchiveMode.Read, leaveOpen: true);
            foreach (ZipArchiveEntry entry in sourceArchive.Entries)
            {
                if (archive.Entries.Any(e => e.FullName == entry.FullName)) continue;

                ZipArchiveEntry file = archive.CreateEntry(entry.FullName);
                using Stream fileStream = file.Open();
                using var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(entry.Open());
            }
        }
    }
}