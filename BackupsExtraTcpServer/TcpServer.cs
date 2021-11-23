using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

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
            byte[] sizeBuffer = new byte[4];
            ReadStream(stream, sizeBuffer);
            int size = BitConverter.ToInt32(sizeBuffer, 0);
            byte[] objBytes = new byte[size];
            ReadStream(stream, objBytes);
            string jsonString = System.Text.Encoding.ASCII.GetString(objBytes);
            Type? type = BackupsExtraTcpShared.JsonConverter.Deserialize<Type>(jsonString);
            if (type == null)
                throw new InvalidOperationException("Can not deserialize type");
            sizeBuffer = new byte[4];
            ReadStream(stream, sizeBuffer);
            size = BitConverter.ToInt32(sizeBuffer, 0);
            objBytes = new byte[size];
            ReadStream(stream, objBytes);
            stream.Dispose();
            client.Dispose();
            jsonString = System.Text.Encoding.ASCII.GetString(objBytes);
            MethodInfo? method = typeof (BackupsExtraTcpShared.JsonConverter).GetMethod("Deserialize");
            MethodInfo? genericMethod = method?.MakeGenericMethod(type);
            object? obj = genericMethod?.Invoke(this, new object[] {jsonString});
            return obj;
        }

        public void Send(object? obj)
        {
            TcpClient? client = _tcpListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            string jsonString = BackupsExtraTcpShared.JsonConverter.Serialize(obj);
            byte[] jsonBytes = System.Text.Encoding.ASCII.GetBytes(jsonString);
            string jsonType = BackupsExtraTcpShared.JsonConverter.Serialize(obj?.GetType());
            byte[] typeBytes = System.Text.Encoding.ASCII.GetBytes(jsonType);
            Send(typeBytes, stream);
            Send(jsonBytes, stream);
            stream.Dispose();
            client.Dispose();
        }

        private void Send(byte[] bytes, NetworkStream stream)
        {
            if (!_tcpListener.Server.IsBound)
                throw new InvalidOperationException("Server is not started");
            byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);
            stream.Write(lengthBytes, 0, lengthBytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }


        private void ReadStream(Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException($"End of stream reached with {remaining} bytes left to read");

                remaining -= read;
                offset += read;
            }
        }
    }
}