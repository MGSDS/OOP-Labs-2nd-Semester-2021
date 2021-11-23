using System.Collections.Generic;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace Backups.Repositories
{
    public interface IBackupDestinationRepository
    {
        Storage CreateStorage(List<JobObjectWithData> jobObjects, ICompressor compressor, string folderName = "");
    }
}