using System;

namespace Banks.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        private DateTime _now;

        public DateTimeProvider()
        {
            _now = DateTime.Now;
        }

        public DateTime Now
        {
            get
            {
                DateTime res = _now;
                _now = _now.AddSeconds(1);
                return res;
            }
            set => _now = value;
        }
    }
}