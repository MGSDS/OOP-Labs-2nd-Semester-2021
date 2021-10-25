using System.Collections.Generic;
using Backups.NetworkTransfer.Entities;

namespace Backups.Server.Repositories
{
    public interface IServerRepository
    {
        void Save(IReadOnlyList<TransferFile> transferFiles, string folderName);
    }
}