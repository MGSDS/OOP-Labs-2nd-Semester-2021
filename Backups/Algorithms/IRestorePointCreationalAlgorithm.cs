using System;
using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.Repositories;

namespace Backups.Algorithms
{
    public interface IRestorePointCreationalAlgorithm
    {
        RestorePoint Run(List<JobObject> objects, IRepository repository);
    }
}