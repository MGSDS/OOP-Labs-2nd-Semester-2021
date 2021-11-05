using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;

namespace Banks.Providers
{
    public class DepositInterestProvider
    {
        private List<Interest> _interests;

        public DepositInterestProvider(IReadOnlyList<Interest> interests)
        {
            _interests = (List<Interest>)interests;
            Id = Guid.NewGuid();
        }

        internal DepositInterestProvider()
        {
        }

        public Guid Id { get; init; }
        public IReadOnlyList<Interest> Interests { get => _interests; internal set => _interests = (List<Interest>)value; }

        public decimal GetMultiplier(decimal amount)
        {
            var res = Interests.Where(x => x.MinMoney <= amount).ToList();
            if (!res.Any())
                return 0;
            return res.MaxBy(x => x.MinMoney).Percentage / 100 / 365;
        }
    }
}