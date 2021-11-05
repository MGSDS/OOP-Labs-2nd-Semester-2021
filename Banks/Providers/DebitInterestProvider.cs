using System;

namespace Banks.Providers
{
    public class DebitInterestProvider
    {
        public DebitInterestProvider(decimal percentage)
        {
            Percentage = percentage;
            Id = Guid.NewGuid();
        }

        internal DebitInterestProvider()
        {
        }

        public Guid Id { get; init; }
        public decimal Percentage { get; internal set; }
        public decimal Multiplier => Percentage / 100 / 365;
    }
}