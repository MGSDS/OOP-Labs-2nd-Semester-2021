using System;
using Banks.Database;
using Banks.Entities.Transactions;
using Banks.Providers;
using TransactionStatus = Banks.Entities.Transactions.TransactionStatus;

namespace Banks.Entities.Accounts
{
    public class DepositAccount : AbstractAccount
    {
        private decimal _notAccruedInterest;

        public DepositAccount(
            Client client,
            UnverifiedLimitProvider unverifiedLimit,
            IDateTimeProvider dateTimeProvider,
            DepositInterestProvider interestProvider,
            DateTime endDate)
            : base(client, unverifiedLimit, dateTimeProvider)
        {
            InterestProvider = interestProvider;
            EndDate = endDate;
            NotAccruedInterest = 0;
            LastInterestAccrue = dateTimeProvider.Now;
        }

        internal DepositAccount(BanksContext context)
            : base(context)
        {
        }

        public DepositInterestProvider InterestProvider { get; internal init; }
        public DateTime EndDate { get; internal init; }

        public decimal NotAccruedInterest { get => _notAccruedInterest; internal init => _notAccruedInterest = value; }

        public DateTime LastInterestAccrue { get; internal init; }

        public override AbstractTransaction CreateAccrueTransaction(decimal amount)
        {
            AbstractTransaction transaction = base.CreateAccrueTransaction(amount);
            if (transaction.Status == TransactionStatus.Successful)
                CalculateInterest();
            return transaction;
        }

        public override AbstractTransaction CreateWithdrawTransaction(decimal amount)
        {
            AbstractTransaction transaction = base.CreateWithdrawTransaction(amount);
            if (transaction.Status == TransactionStatus.Successful)
                CalculateInterest();
            return transaction;
        }

        public override AbstractTransaction CreateTransferTransaction(decimal amount, AbstractAccount account)
        {
            AbstractTransaction transaction = base.CreateTransferTransaction(amount, account);
            if (transaction.Status == TransactionStatus.Successful)
                CalculateInterest();
            return transaction;
        }

        internal override AbstractTransaction CreateServiceTransaction()
        {
            CalculateInterest();
            var transaction =
                new InterestAccrueTransaction(_notAccruedInterest, this, LastInterestAccrue.Date);
            if (transaction.Status == TransactionStatus.Successful)
                _notAccruedInterest = 0;
            return transaction;
        }

        internal override void DecreaseBalance(decimal amount)
        {
            if (Balance - amount < 0)
                throw new InvalidOperationException("Deposit account balance cant go below zero");
            if (DateTimeProvider.Now < EndDate)
                throw new InvalidOperationException("Deposit account can not decrease balance before end date");
            base.DecreaseBalance(amount);
        }

        private void CalculateInterest()
        {
            decimal days = (DateTimeProvider.Now - LastInterestAccrue).Days;
            if (days < 1) return;
            for (int i = 0; i < days; ++i)
                IncreaseBalance(InterestProvider.GetMultiplier(Balance) * Balance);
        }
    }
}