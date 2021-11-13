using System;

namespace Banks.Providers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}