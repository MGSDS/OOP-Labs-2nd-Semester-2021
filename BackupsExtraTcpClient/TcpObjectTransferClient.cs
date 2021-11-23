using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;

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

            byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);
            stream.Write(lengthBytes, 0, lengthBytes.Length);
            stream.Write(bytes, 0, bytes.Length);
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