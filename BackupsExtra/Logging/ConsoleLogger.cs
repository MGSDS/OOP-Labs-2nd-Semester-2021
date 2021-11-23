using System;
using System.Globalization;
using System.Text;

namespace BackupsExtra.Logging
{
    public class ConsoleLogger : ILogger
    {
        private LoggerSettings _settings;

        public ConsoleLogger(LoggerSettings settings)
        {
            _settings = settings;
        }

        public void Write(LoggerMessage message)
        {
            if (message.MessageType == MessageType.Error)
                Console.Error.WriteLine(ConvertToString(message));
            else
                Console.WriteLine(ConvertToString(message));
        }

        private string ConvertToString(LoggerMessage message)
        {
            var builder = new StringBuilder();
            if (message.MessageType != MessageType.Error)
                builder.Append($"[{message.MessageType.ToString()}] ");
            if (_settings.ShowTime)
                builder.Append($" {message.Time.ToString(CultureInfo.CurrentCulture)} ");
            builder.Append(message.Message);
            string result = builder.ToString();
            return result;
        }
    }
}