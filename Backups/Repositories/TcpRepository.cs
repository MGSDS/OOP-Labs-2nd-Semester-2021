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
        private readonly ICompressor _compressor;

        public TcpRepository(IPAddress address, ushort port, ICompressor compressor)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            _client = new TcpFileTransferClient(address.ToString(), port);
            _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        }

        public Storage CreateStorage(IReadOnlyList<JobObject> jobObjects, string folderName = "")
        {
            if (jobObjects == null) throw new ArgumentNullException(nameof(jobObjects));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            var id = Guid.NewGuid();
            string name = $"{id}.zip";
            var mem = new MemoryStream();
            _compressor.Compress(jobObjects, mem);
            var files = new List<TransferFile> { new TransferFile(name, mem) };
            _client.SendFiles(files, folderName);
            return new Storage(name, Path.Combine(":SERVER:", folderName), id, jobObjects);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}