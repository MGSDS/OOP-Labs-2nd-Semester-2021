using System;
using System.Net.Sockets;
using BackupsExtraTcpShared;

namespace BackupsExtraTcpClient
{
    public class TcpObjectTransferClient
    {
        private readonly string _address;
        private readonly ushort _port;

        public TcpObjectTransferClient(string address, ushort port)
        {
            _address = address;
            _port = port;
        }

        public void Send(object? obj)
        {
            TcpClient client;
            try
            {
                client = new TcpClient();
                client.Connect(_address, _port);
            }
            catch (SocketException)
            {
                throw new InvalidOperationException("Can not connect to server");
            }

            NetworkStream stream = client.GetStream();
            NetworkStreamReaderWriter.Write(stream, obj);
            stream.Dispose();
            client.Dispose();
        }

        public object? Receive()
        {
            TcpClient client;
            try
            {
                client = new TcpClient();
                client.Connect(_address, _port);
            }
            catch (SocketException)
            {
                throw new InvalidOperationException("Can not connect to server");
            }

            using NetworkStream stream = client.GetStream();
            return NetworkStreamReaderWriter.Read(stream);
        }
    }
}