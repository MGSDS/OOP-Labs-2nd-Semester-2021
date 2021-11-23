using System;
using System.Collections.Generic;
using System.IO;
using Backups.CompressionAlgorithms;
using Backups.Entities;
using BackupsExtraTcpClient;
using BackupsExtraTcpShared.Messages;

namespace BackupsExtra.Repository
{
    public class ServerExtraBackupDestinationRepository : IExtraBackupDestinationRepository
    {
        public ServerExtraBackupDestinationRepository(TcpObjectTransferClient client)
        {
            Client = client;
        }

        public TcpObjectTransferClient Client { get; }
        public Storage CreateStorage(List<JobObjectWithData> jobObjects, ICompressor compressor, string folderName = "")
        {
            Client.Send(new CreateStorageMesaage(jobObjects, compressor, folderName));
            return Client.Receive() as Storage;
        }

        public void Read(Storage storage, Stream stream)
        {
            Client.Send(new ReadMessage(storage));
            byte[] bytes = Client.Receive() as byte[];
            if (bytes is null)
                throw new NullReferenceException("Received stream is null or not a stream");
            stream.Write(bytes, 0, bytes.Length);
            }

        public void Write(Storage storage, Stream stream)
        {
            Client.Send(new WriteMessage(storage, stream));
        }

        public void Delete(Storage storage)
        {
            Client.Send(new DeleteMessage(storage));
        }
    }
}