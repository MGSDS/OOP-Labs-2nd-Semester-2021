using System;
using Banks.Database;
using Banks.Entities.Transactions;
using Banks.Providers;

namespace Banks.Entities.Accounts
{
    public class CreditAccount : AbstractAccount
    {
        public CreditAccount(Client client, UnverifiedLimitProvider unverifiedLimit, IDateTimeProvider dateTimeProvider, CreditInfoProvider creditInfo)
            : base(client, unverifiedLimit, dateTimeProvider)
        {
            CreditInfo = creditInfo;
        }

        internal CreditAccount(BanksContext context)
            : base(context)
        {
        }

        public CreditInfoProvider CreditInfo { get; init; }

        internal override AbstractTransaction ServiceTransaction()
        {
            decimal commission = 0;
            if (Balance < 0)
                commission = CreditInfo.Commission;
            var transaction = new CommissionTransaction(commission,  this, DateTimeProvider.Now);
            transaction.Execute();
            return transaction;
        }

        internal override void DecreaseBalance(decimal amount)
        {
            if (-CreditInfo.Limit > Balance - amount)
                throw new InvalidOperationException("Credit Account can not go below limit");
            base.DecreaseBalance(amount);
        }
    }
}