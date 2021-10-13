using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Backups.Client
{
    public class Client : System.IDisposable
    {
        private TcpClient _client;
        private string _address;
        private ushort _port;

        public Client(string address, ushort port)
        {
            _client = new TcpClient();
            _address = address;
            _port = port;
        }

        public void Connect()
        {
            _client.Connect(_address, _port);
        }

        public void Close()
        {
            _client.Close();
        }

        public void Test()
        {
            Console.WriteLine("Connected");
            Console.Write("Enter the string to be transmitted : ");
            
            String str=Console.ReadLine();
            Stream stm = _client.GetStream();
                        
            ASCIIEncoding asen= new ASCIIEncoding();
            byte[] ba=asen.GetBytes(str);
            Console.WriteLine("Transmitting.....");
            
            stm.Write(ba,0,ba.Length);
            
            byte[] bb=new byte[100];
            int k=stm.Read(bb,0,100);
            
            for (int i=0;i<k;i++)
                Console.Write(Convert.ToChar(bb[i]));
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}