using System.Threading.Tasks;
using BackupsExtraTcpClient;
using BackupsExtraTcpServer;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class ServerTests
    {
        [Test]
        public void ObjectTransfer_objectReceived()
        {
            var server = new TcpServer(1234);
            var client = new TcpObjectTransferClient("localhost", 1234);
            server.Start();
            Task.Run(() =>
            {
                object obj = server.Receive();
                server.Send(obj);
                server.Stop();
            });
            string str = "Awesome string";
            client.Send(str);
            string received =  client.Receive() as string;
            Assert.AreEqual(str, received);

        }
    }
}