using Backups.Server;
using Backups.Server.Repositories;

var server = new TcpServer(1234, new LocalFSRepository("path"));
server.Start();
while (true)
{
    server.Read();
}
