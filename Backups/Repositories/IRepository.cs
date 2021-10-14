using System;
using System.Collections.Generic;
using Backups.Entities;

namespace Backups.Repositories
{
    public interface IRepository
    {
        IReadOnlyList<Storage> CreateStorages(IReadOnlyList<JobObject> jobObjects, string folderName = "");
        Storage CreateStorage(IReadOnlyList<JobObject> jobObjects, string folderName = "");
    }
}