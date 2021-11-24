using System;
using System.Net;
using System.Net.Sockets;
using BackupsExtraTcpShared;

namespace BackupsExtraTcpServer
{
    public class TcpServer
    {
        private TcpListener _tcpListener;

        public TcpServer(ushort port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }

        public object? Receive()
        {
            if (!_tcpListener.Server.IsBound)
                throw new InvalidOperationException("Server is not started");
            TcpClient? client = _tcpListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            object? obj = NetworkStreamReaderWriter.Read(stream);
            stream.Dispose();
            client.Dispose();
            return obj;
        }

        public void Send(object? obj)
        {
            TcpClient? client = _tcpListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            NetworkStreamReaderWriter.Write(stream, obj);
            stream.Dispose();
            client.Dispose();
        }
    }
}