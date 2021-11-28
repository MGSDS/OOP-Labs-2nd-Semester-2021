using System;

namespace BackupsExtra.Logging
{
    public static class LoggerSingletone
    {
        private static ILogger _logger;

        public static bool IsInitialized { get; private set; }

        public static void Initialize(ILogger logger)
        {
            if (IsInitialized)
                return;

            _logger = logger;
            IsInitialized = true;
        }

        public static ILogger GetInstance()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Logger is not initialized");
            return _logger;
        }
    }
}