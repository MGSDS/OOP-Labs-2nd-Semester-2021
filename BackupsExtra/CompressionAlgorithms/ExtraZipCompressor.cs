using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using BackupsExtra.Repository;
using File = Backups.Entities.File;

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
                using Stream destinationStream = file.Open();
                using Stream sourceStream = entry.Open();
                using var buffer = new MemoryStream();
                sourceStream.CopyTo(buffer);
                destinationStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
            }
        }

        public IReadOnlyList<File> Decompress(Storage storage, IExtraBackupDestinationRepository backupDestinationRepository)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));
            if (backupDestinationRepository == null) throw new ArgumentNullException(nameof(backupDestinationRepository));
            using var stream = new MemoryStream();
            backupDestinationRepository.Read(storage, stream);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
            var files = new List<File>();
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                using var entryStream = new MemoryStream();
                entry.Open().CopyTo(entryStream);
                var file = new File(entryStream, entry.Name);
                files.Add(file);
            }

            return files;
        }
    }
}