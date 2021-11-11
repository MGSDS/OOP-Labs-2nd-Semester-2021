using System;

namespace Banks.Providers
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider()
        {
            Now = DateTime.Now;
        }

        public DateTime Now { get; set; }
    }
}