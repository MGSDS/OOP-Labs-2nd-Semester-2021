using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Backups.Client;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using Backups.NetworkTransfer.Entities;

namespace Backups.Repositories
{
    public class TcpRepository : IRepository, IDisposable
    {
        private readonly TcpFileTransferClient _client;

        public TcpRepository(IPAddress address, ushort port)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            _client = new TcpFileTransferClient(address.ToString(), port);
        }

        public Storage CreateStorage(List<JobObject> jobObjects, ICompressor compressor, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            var id = Guid.NewGuid();
            string name = $"{id}.zip";
            var mem = new MemoryStream();
            compressor.Compress(jobObjects, mem);
            var files = new List<TransferFile> { new TransferFile(name, mem) };
            _client.SendFiles(files, folderName);
            return new Storage(name, folderName, id, jobObjects);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}