using System;
using File = Backups.Entities.File;

namespace Backups.Repositories
{
    public interface ISourceRepository
    {
        File ReadFile(string path);
    }
}