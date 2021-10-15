using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Backups.Client;
using Backups.NetworkTransfer.Entities;
using Backups.Server;
using Backups.Server.Repositories;
using Moq;
using NUnit.Framework;

namespace Backups.Tests
{
    public class ClientServerTests : IDisposable
    {
        private Mock<IServerRepository> _serverRepo;
        private List<TransferFile> _files;
        private string _folderName;
        private TcpServer _server;
        private TcpFileTransferClient _client;

        [SetUp]
        public void SetUp()
        {
            _serverRepo = new Mock<IServerRepository>();
            _serverRepo.Setup(a => a.Save(It.IsAny<IReadOnlyList<TransferFile>>(), It.IsAny<string>()))
                .Callback((IReadOnlyList<TransferFile> x, string y) =>
                {
                    _files = (List<TransferFile>) x;
                    _folderName = y;
                });
            _server = new TcpServer(1234, _serverRepo.Object);
            _client = new TcpFileTransferClient("127.0.0.1", 1234);
        }

        [Test]
        public void SendFile_FileReceived()
        {
            _server.Start();
            var server = Task.Run(() => _server.Read());
            string testFolderName = "awesome folder";
            var rand = new Random();
            var testFile = new TransferFile("file", new MemoryStream(BitConverter.GetBytes(rand.NextInt64())));
            _client.SendFiles(new List<TransferFile>{ testFile }, testFolderName);
            server.Wait();
            _server.Stop();
            Assert.AreEqual(testFolderName ,_folderName);
            Assert.AreEqual(testFile.Stream, _files[0].Stream);
            Assert.AreEqual(testFile.Name, _files[0].Name);
        }
        
        [Test]
        public void SendMultipleFile_FilesReceived()
        {
            _server.Start();
            var server = Task.Run(() => _server.Read());
            string testFolderName = "awesome folder";
            var rand = new Random();
            var testFile1 = new TransferFile("file1", new MemoryStream(BitConverter.GetBytes(rand.NextInt64())));
            var testFile2 = new TransferFile("file2", new MemoryStream(BitConverter.GetBytes(rand.NextInt64())));
            _client.SendFiles(new List<TransferFile>{ testFile1, testFile2 }, testFolderName);
            server.Wait();
            _server.Stop();
            Assert.AreEqual(testFolderName ,_folderName);
            Assert.AreEqual(testFile1.Stream, _files[0].Stream);
            Assert.AreEqual(testFile1.Name, _files[0].Name);
            Assert.AreEqual(testFile2.Stream, _files[1].Stream);
            Assert.AreEqual(testFile2.Name, _files[1].Name);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}