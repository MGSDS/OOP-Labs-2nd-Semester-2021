using System.IO;
using Backups.Client;
using Backups.Server.Entities;

var client = new Client("127.0.0.1", 1234);
MemoryStream mem = new MemoryStream();
FileStream file = File.Open("/Users/bill/Downloads/Vivaldi.4.3.2439.44.universal.dmg", FileMode.Open);
file.CopyTo(mem);
client.Start();
client.SendFile(new TransferFile("test.dmg", mem));
client.Close();