using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Backups.Server.Entities;
using Backups.Server.Headers;

namespace Backups.Client
{
    public class Client : System.IDisposable
    {
        private TcpClient _client;
        private string _address;
        private ushort _port;
        private Stream _stm;

        public Client(string address, ushort port)
        {
            _client = new TcpClient();
            _address = address;
            _port = port;
        }

        public void Start()
        {
            _client.Connect(_address, _port);
            _stm = _client.GetStream();
        }

        public void Close()
        {
            _client.Close();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void SendFile(TransferFile file)
        {
            byte[] singleFile = BitConverter.GetBytes(true);
            byte[] bytes = file.Stream.ToArray();
            var header = new FileHeader(file.Name, bytes.Length);
            byte[] headerBytes = header.GetByteHeader();
            int headerSize = headerBytes.Length;
            byte[] headerSizeBytes = BitConverter.GetBytes(headerSize);
            _stm.Write(singleFile, 0, singleFile.Length);
            _stm.Write(headerSizeBytes, 0, headerSizeBytes.Length);
            _stm.Write(headerBytes, 0, headerBytes.Length);
            _stm.Write(bytes, 0, bytes.Length);
        }

        public void SendFiles(IReadOnlyList<TransferFile> files, string folderName)
        {
            byte[] singleFile = BitConverter.GetBytes(false);
            var header = new FolderHeader(folderName, files.Count);
            byte[] headerBytes = header.GetByteHeader();
            int headerSize = headerBytes.Length;
            byte[] message = singleFile.Concat(BitConverter.GetBytes(headerSize)).Concat(headerBytes).ToArray();
            _stm.Write(message, 0, message.Length);
            foreach (TransferFile transferFile in files)
                SendFile(transferFile);
        }
    }
}