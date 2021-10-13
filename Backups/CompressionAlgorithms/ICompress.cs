using System.Collections.Generic;
using System.IO;
using Backups.Entities;

namespace Backups.CompressionAlgorithms
{
    public interface ICompress
    {
        void Compress(IReadOnlyList<JobObject> objects, Stream stream);
    }
}