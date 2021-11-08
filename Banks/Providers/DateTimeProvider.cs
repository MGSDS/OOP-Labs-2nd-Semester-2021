using System;

namespace Banks.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeProvider()
        {
            Now = DateTime.Now;
        }

        public DateTime Now { get; set; }
    }
}