using System;
using System.Collections.Generic;
using Backups.Entities;

namespace Backups.Repositories
{
    public interface IRepository
    {
        Storage CreateStorage(JobObject jobObject);
        Storage CreateStorage(List<JobObject> jobObjects);
        IRepository CreateRepository(Guid id);
    }
}