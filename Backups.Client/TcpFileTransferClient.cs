#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Backups.NetworkTransfer.Entities;
using Backups.NetworkTransfer.Headers;

namespace Backups.Client
{
    public class TcpFileTransferClient : IDisposable
    {
        private readonly TcpClient _client;
        private readonly string _address;
        private readonly ushort _port;
        private Stream? _stm;

        public TcpFileTransferClient(string address, ushort port)
        {
            _client = new TcpClient();
            _address = address;
            _port = port;
            _stm = null;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void SendFiles(IReadOnlyList<TransferFile> files, string folderName = "")
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (folderName == null) throw new ArgumentNullException(nameof(folderName));
            var header = new FolderHeader(folderName, files.Count);
            byte[] headerBytes = header.GetByteHeader();
            int headerSize = headerBytes.Length;
            byte[] headerSizeBytes = BitConverter.GetBytes(headerSize);
            Start();
            _stm?.Write(headerSizeBytes, 0, headerSizeBytes.Length);
            _stm?.Write(headerBytes, 0, headerBytes.Length);
            foreach (TransferFile transferFile in files)
                SendSingleFile(transferFile);
            Close();
        }

        private void SendSingleFile(TransferFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            byte[] bytes = file.Stream.ToArray();
            var header = new FileHeader(file.Name, bytes.Length);
            byte[] headerBytes = header.GetByteHeader();
            int headerSize = headerBytes.Length;
            byte[] headerSizeBytes = BitConverter.GetBytes(headerSize);
            _stm?.Write(headerSizeBytes, 0, headerSizeBytes.Length);
            _stm?.Write(headerBytes, 0, headerBytes.Length);
            _stm?.Write(bytes, 0, bytes.Length);
        }

        private void Start()
        {
            try
            {
                _client.Connect(_address, _port);
            }
            catch (SocketException)
            {
                throw new InvalidOperationException("Can not connect to server");
            }

            _stm = _client.GetStream();
        }

        private void Close()
        {
            _client.Close();
        }
    }
}