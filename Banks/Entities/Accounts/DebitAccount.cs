using System;
using Banks.Database;
using Banks.Entities.Transactions;
using Banks.Providers;
using TransactionStatus = Banks.Entities.Transactions.TransactionStatus;

namespace Banks.Entities.Accounts
{
    public class DebitAccount : AbstractAccount
    {
        private decimal _notAccruedInterest;

        public DebitAccount(Client client, UnverifiedLimitProvider unverifiedLimit, IDateTimeProvider dateTimeProvider, DebitInterestProvider interestProvider)
            : base(client, unverifiedLimit, dateTimeProvider)
        {
            InterestProvider = interestProvider;
            NotAccruedInterest = 0;
            LastInterestAccrue = dateTimeProvider.Now;
        }

        internal DebitAccount(BanksContext context)
            : base(context)
        {
        }

        public DebitInterestProvider InterestProvider { get; internal init; }

        public decimal NotAccruedInterest { get => _notAccruedInterest; internal init => _notAccruedInterest = value; }

        public DateTime LastInterestAccrue { get; internal init; }

        public override AbstractTransaction AccrueTransaction(decimal amount)
        {
            AbstractTransaction transaction = base.AccrueTransaction(amount);
            if (transaction.Status == TransactionStatus.Successful)
                CalculateInterest();
            return transaction;
        }

        public override AbstractTransaction WithdrawTransaction(decimal amount)
        {
            AbstractTransaction transaction = base.WithdrawTransaction(amount);
            if (transaction.Status == TransactionStatus.Successful)
                CalculateInterest();
            return transaction;
        }

        public override AbstractTransaction TransferTransaction(decimal amount, AbstractAccount account)
        {
            AbstractTransaction transaction = base.TransferTransaction(amount, account);
            if (transaction.Status == TransactionStatus.Successful)
                CalculateInterest();
            return transaction;
        }

        internal override AbstractTransaction ServiceTransaction()
        {
            CalculateInterest();
            var transaction = new InterestAccrueTransaction(_notAccruedInterest, this, LastInterestAccrue.Date);
            if (transaction.Status == TransactionStatus.Successful)
                _notAccruedInterest = 0;
            return transaction;
        }

        internal override void DecreaseBalance(decimal amount)
        {
            if (Balance - amount < 0)
                throw new InvalidOperationException("Debit account balance cant go below zero");
            base.DecreaseBalance(amount);
        }

        private void CalculateInterest()
        {
            decimal days = (DateTimeProvider.Now - LastInterestAccrue).Days;
            if (days < 1) return;
            _notAccruedInterest += Balance * days * InterestProvider.Multiplier;
        }
    }
}