using Backups.Server;
using Backups.Server.Repositories;

var server = new TcpServer(1234, new LocalFsRepository("path"));
server.Start();
while (true)
{
    server.Read();
}
