using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Backups.Client;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.NetworkTransfer.Entities;

namespace Backups.Repositories
{
    public class TcpBackupDestinationRepository : IBackupDestinationRepository, IDisposable
    {
        private readonly TcpFileTransferClient _client;

        public TcpBackupDestinationRepository(IPAddress address, ushort port)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            _client = new TcpFileTransferClient(address.ToString(), port);
        }

        public Storage CreateStorage(List<JobObjectWithData> jobObjects, ICompressor compressor, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            var id = Guid.NewGuid();
            string name = $"{id}.zip";
            var mem = new MemoryStream();
            compressor.Compress(jobObjects.Select(x => x.File).ToList(), mem);
            var files = new List<TransferFile> { new TransferFile(name, mem) };
            _client.SendFiles(files, folderName);
            return new Storage(name, folderName, id, jobObjects.Select(x => x.JobObject).ToList());
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}