using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Backups.Server
{
    public class Server
    {
        private TcpListener _listener;

        public Server(ushort port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                NetworkStream ns = client.GetStream();
                byte[] hello = new byte[100];
                hello = Encoding.Default.GetBytes("hello world");

                ns.Write(hello, 0, hello.Length);
                while (client.Connected)
                {
                    byte[] msg = new byte[1024];
                    int count = ns.Read(msg, 0, msg.Length);
                    Console.Write(Encoding.Default.GetString(msg, 0, count));
                }
            }
        }
    }
}