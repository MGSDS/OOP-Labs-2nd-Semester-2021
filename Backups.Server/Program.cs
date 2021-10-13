using Backups.Server;
using Backups.Server.Repositories;

var server = new TcpServer(1234, new LocalFSRepository("/Users/bill/Desktop/Backups"));
server.Start();
server.Read();
server.Stop();
