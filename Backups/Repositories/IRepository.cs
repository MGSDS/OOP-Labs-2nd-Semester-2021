using System.Collections.Generic;
using Backups.Entities;

namespace Backups.Repositories
{
    public interface IRepository
    {
        Storage CreateStorage(IReadOnlyList<JobObject> jobObjects, string folderName = "");
    }
}