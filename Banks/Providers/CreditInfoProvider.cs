using System;

namespace Banks.Providers
{
    public class CreditInfoProvider
    {
        public CreditInfoProvider(decimal limit, decimal commission)
        {
            Limit = limit;
            Commission = commission;
            Id = Guid.NewGuid();
        }

        internal CreditInfoProvider()
        {
        }

        public Guid Id { get; init; }
        public decimal Limit { get; internal set; }
        public decimal Commission { get; internal set; }
    }
}