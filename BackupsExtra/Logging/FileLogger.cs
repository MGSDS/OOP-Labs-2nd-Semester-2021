using System;
using System.Globalization;
using System.Text;

namespace BackupsExtra.Logging
{
    public class FileLogger : ILogger
    {
        private LoggerSettings _settings;

        public FileLogger(string logFilePath, LoggerSettings settings)
        {
            _settings = settings;
            LogFilePath = logFilePath;
        }

        public FileLogger(string logFilePath)
        {
            _settings = new LoggerSettings();
            LogFilePath = logFilePath;
        }

        public string LogFilePath { get; set; }
        public void Write(LoggerMessage message)
        {
            System.IO.File.AppendAllText(LogFilePath, ConvertToString(message) + Environment.NewLine);
        }

        private string ConvertToString(LoggerMessage message)
        {
            var builder = new StringBuilder();
            builder.Append($"[{message.MessageType.ToString()}]");
            if (_settings.ShowTime)
                builder.Append($" {message.Time.ToString(CultureInfo.CurrentCulture)} ");
            builder.Append(message.Message);
            string result = builder.ToString();
            return result;
        }
    }
}