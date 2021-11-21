using System.Collections.Generic;
using Backups.CompressionAlgorithms;
using Backups.Entities;

namespace Backups.Repositories
{
    public interface IRepository
    {
        Storage CreateStorage(List<JobObject> jobObjects, ICompressor compressor, string folderName = "");
    }
}