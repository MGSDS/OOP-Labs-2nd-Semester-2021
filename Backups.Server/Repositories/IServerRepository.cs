using System.Collections.Generic;
using Backups.Server.Entities;

namespace Backups.Server.Repositories
{
    public interface IServerRepository
    {
        void Save(TransferFile transferFile);
        void Save(IReadOnlyList<TransferFile> transferFile);

        IServerRepository CreateInnerRepository(string name);
    }
}