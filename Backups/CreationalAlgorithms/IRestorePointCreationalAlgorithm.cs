using System.Collections.Generic;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.CreationalAlgorithms
{
    public interface IRestorePointCreationalAlgorithm
    {
        RestorePoint Run(List<JobObject> objects, IRepository repository, ICompressor compressor);
    }
}