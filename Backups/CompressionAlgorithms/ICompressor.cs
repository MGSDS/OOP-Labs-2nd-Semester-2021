using System.Collections.Generic;
using System.IO;
using File = Backups.Entities.File;

namespace Backups.CompressionAlgorithms
{
    public interface ICompressor
    {
        void Compress(IReadOnlyList<File> files, Stream stream);
    }
}