using System.Collections.Generic;
using System.IO;
using Backups.Entities;

namespace Backups.CompressionAlgorithms
{
    public interface ICompressor
    {
        void Compress(IReadOnlyList<JobObject> objects, Stream stream);
    }
}