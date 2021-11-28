using System;
using System.IO;
using BackupsExtra.Repository;
using BackupsExtraTcpShared.Messages;

namespace BackupsExtraTcpServer
{
    public class ObjectHandler
    {
        private readonly IExtraBackupDestinationRepository _repository;

        public ObjectHandler(IExtraBackupDestinationRepository repository)
        {
            _repository = repository;
        }

        public object? Handle(object? obj) => obj.GetType() switch
        {
            _ when obj.GetType() == typeof(DeleteMessage) => Delete(obj as DeleteMessage),
            _ when obj.GetType() == typeof(ReadMessage) => Read(obj as ReadMessage),
            _ when obj.GetType() == typeof(WriteMessage) => Write(obj as WriteMessage),
            _ when obj.GetType() == typeof(CreateStorageMesaage) => CreateStorage(obj as CreateStorageMesaage),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        private object? Delete(DeleteMessage? message)
        {
            _repository.Delete(message.Storage);
            return null;
        }
        
        private object? Read(ReadMessage? message)
        {
            MemoryStream stream = new MemoryStream();
            _repository.Read(message.Storage, stream);
            return stream.ToArray();
        }
        
        private object? Write(WriteMessage? message)
        {
            _repository.Write(message.Storage, message.Stream);
            return null;
        }

        private object? CreateStorage(CreateStorageMesaage? message)
        {
            return _repository.CreateStorage(message.JobObjects, message.Compressor, message.FolderName);
        }
    }
}