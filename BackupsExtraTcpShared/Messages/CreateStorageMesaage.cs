using System.Collections.Generic;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace BackupsExtraTcpShared.Messages
{
    public record CreateStorageMesaage(List<JobObjectWithData> JobObjects, ICompressor Compressor, string FolderName);
}