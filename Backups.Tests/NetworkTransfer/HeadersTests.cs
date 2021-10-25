using Backups.NetworkTransfer.Headers;
using NUnit.Framework;

namespace Backups.Tests.NetworkTransfer
{
    public class Headers
    {
        [Test]
        [TestCase("RandomName", 1024)]
        [TestCase("AnotherRandomName", 99999)]
        public void CreateFileHeaderEncodeDecodeRead_ValidNameSizeReturned(string name, int size)
        {
            var header = new FileHeader(name, size);
            byte[] array = header.GetByteHeader();
            var readHeader = new FileHeader(array);
            Assert.AreEqual(header.GetName(), readHeader.GetName());
            Assert.AreEqual(header.GetSize(), readHeader.GetSize());
        }
        
        [Test]
        [TestCase("RandomName", 1024)]
        [TestCase("AnotherRandomName", 99999)]
        public void CreateFileHeaderRead_ValidNameSizeReturned(string name, int size)
        {
            var header = new FileHeader(name, size);
            Assert.AreEqual(name, header.GetName());
            Assert.AreEqual(size, header.GetSize());
        }
        
        [Test]
        [TestCase("RandomName", 1024)]
        [TestCase("AnotherRandomName", 99999)]
        public void CreateFolderHeaderEncodeDecodeRead_ValidNameCountReturned(string name, int count)
        {
            var header = new FolderHeader(name, count);
            byte[] array = header.GetByteHeader();
            var readHeader = new FolderHeader(array);
            Assert.AreEqual(header.GetFolderName(), readHeader.GetFolderName());
            Assert.AreEqual(header.GetFilesCount(), readHeader.GetFilesCount());
        }
        
        [Test]
        [TestCase("RandomName", 1024)]
        [TestCase("AnotherRandomName", 99999)]
        public void CreateFolderHeaderRead_ValidNameCountReturned(string name, int size)
        {
            var header = new FileHeader(name, size);
            Assert.AreEqual(name, header.GetName());
            Assert.AreEqual(size, header.GetSize());
        }
    }
}