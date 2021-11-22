using System;
using System.Globalization;
using System.Text;

namespace BackupsExtra.Logging
{
    public enum MessageType
    {
        Info,
        Warning,
        Error,
    }

    public class LoggerMessage
    {
        public LoggerMessage(string message, MessageType messageType = MessageType.Info)
        {
            Message = message;
            MessageType = messageType;
            Time = DateTime.Now;
        }

        public LoggerMessage(Exception exception, MessageType messageType = MessageType.Error)
        {
            Message = exception.Message;
            MessageType = messageType;
            Time = DateTime.Now;
        }

        public string Message { get; }
        public MessageType MessageType { get; }
        public DateTime Time { get; }
    }
}