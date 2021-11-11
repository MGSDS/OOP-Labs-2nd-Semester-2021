using System;

namespace Banks.Providers
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}