using System;

namespace Banks.Entities
{
    public class Interest
    {
        public Interest(decimal percentage, decimal minMoney)
        {
            Id = Guid.NewGuid();
            Percentage = percentage;
            MinMoney = minMoney;
        }

        internal Interest()
        {
        }

        public Guid Id { get; init; }
        public decimal Percentage { get; internal init; }
        public decimal MinMoney { get; internal init; }
    }
}