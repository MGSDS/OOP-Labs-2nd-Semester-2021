using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Backups.Server.Entities;
using Backups.Server.Headers;
using Backups.Server.Repositories;

namespace Backups.Server
{
    public class TcpServer
    {
        private TcpListener _listener;
        private IServerRepository _repo;
        private NetworkStream _stream;
        private TcpClient _client;

        public TcpServer(ushort port, IServerRepository repo)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _repo = repo;
        }

        public void Start()
        {
            _listener.Start();
            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void Read()
        {
            byte[] singleFileByte = new byte[1];
            ReadStream(singleFileByte);
            bool singleFile = BitConverter.ToBoolean(singleFileByte);
            if (singleFile)
                SaveFile();
            else
                SaveFiles();
        }

        private TransferFile ReceiveSingleFile()
        {

            byte[] headerBytes = GetHeader();
            var header = new FileHeader(headerBytes);
            var stream = new MemoryStream();
            byte[] fileBytes = new byte[header.GetSize()];
            ReadStream(fileBytes);
            stream.Write(fileBytes, 0, header.GetSize());
            return new TransferFile(header.GetName(), stream);
        }

        private void SaveFile()
        {
            _repo.Save(ReceiveSingleFile());
        }

        private void SaveFiles()
        {
            byte[] headerBytes = GetHeader();
            var header = new FolderHeader(headerBytes);
            IServerRepository newRepository = _repo.CreateInnerRepository(header.GetFolderName());
            var files = new List<TransferFile>();
            for (int i = 0; i < header.GetFilesCount(); i++)
                newRepository.Save(ReceiveSingleFile());
        }

        private byte[] GetHeader()
        {
            byte[] headerSizeBytes = new byte[4];
            ReadStream(headerSizeBytes);
            int headerSize = BitConverter.ToInt32(headerSizeBytes);
            byte[] headerBytes = new byte[headerSize];
            ReadStream(headerBytes);
            return headerBytes;
        }

        private void ReadStream(byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = _stream.Read(data, offset, remaining);
                if (read <= 0)
                {
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                }

                remaining -= read;
                offset += read;
            }
        }
    }
}