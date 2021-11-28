using System;
using System.IO;
using BackupsExtra.Repository;
using BackupsExtraTcpServer;

IExtraBackupDestinationRepository repository = new LocalFsExtraBackupDestinationRepository("path");

var server = new TcpServer(1234);
ObjectHandler handler = new ObjectHandler(new LocalFsExtraBackupDestinationRepository("/Users/bill/Desktop/backup"));
server.Start();
while (true)
{
    object? obj = handler.Handle(server.Receive());
    if (obj != null)
    {
        server.Send(obj);
    }
}

