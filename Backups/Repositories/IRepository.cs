using System;
using File = Backups.Entities.File;

namespace Backups.Repositories
{
    public interface IRepository
    {
        File ReadFile(string path);
    }
}