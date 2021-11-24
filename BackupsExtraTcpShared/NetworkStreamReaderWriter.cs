using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace BackupsExtraTcpShared
{
    public static class NetworkStreamReaderWriter
    {
        public static object? Read(NetworkStream stream)
        {
            string jsonString = ReadString(stream);
            Type? type = BackupsExtraTcpShared.JsonConverter.Deserialize<Type>(jsonString);
            if (type == null)
                throw new InvalidOperationException("Can not deserialize type");
            jsonString = ReadString(stream);
            object? obj = JsonConverter.Deserialize(jsonString, type);
            if (obj == null)
                throw new InvalidOperationException("Can not deserialize object");
            return obj;
        }

        public static void Write(NetworkStream stream, object? obj)
        {
            string jsonString = JsonConverter.Serialize(obj);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(jsonString);
            string jsonType = JsonConverter.Serialize(obj?.GetType());
            byte[] typeBytes = Encoding.ASCII.GetBytes(jsonType);
            Write(typeBytes, stream);
            Write(jsonBytes, stream);
        }

        private static void Write(byte[] bytes, NetworkStream stream)
        {
            byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);
            stream.Write(lengthBytes, 0, lengthBytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string ReadString(NetworkStream stream)
        {
            byte[] sizeBuffer = new byte[4];
            ReadStream(stream, sizeBuffer);
            int size = BitConverter.ToInt32(sizeBuffer, 0);
            byte[] objBytes = new byte[size];
            ReadStream(stream, objBytes);
            return System.Text.Encoding.ASCII.GetString(objBytes);
        }

        private static void ReadStream(Stream stream, byte[] data)
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