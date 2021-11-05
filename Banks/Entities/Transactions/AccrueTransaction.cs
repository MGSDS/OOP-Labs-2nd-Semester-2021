using System;
using System.Security.Cryptography;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class AccrueTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public AccrueTransaction(decimal amount, AbstractAccount account, DateTime time)
            : base(amount, time)
        {
            Account = account;
            _errorMessage = string.Empty;
        }

        internal AccrueTransaction()
        {
        }

        public AbstractAccount Account { get; init; }

        public override string ErrorMessage { get => _errorMessage; internal init => _errorMessage = value; }

        public override void Execute()
        {
            if (Status is not TransactionStatus.Ready)
                throw new InvalidOperationException("Transaction is already done");
            Account.IncreaseBalance(Amount);
            Status = TransactionStatus.Successful;
        }

        public override void Cancel()
        {
            if (Status is not TransactionStatus.Successful)
                throw new InvalidOperationException("Transaction can not be canceled");
            Account.DecreaseBalanceWithoutLimit(Amount);
            Status = TransactionStatus.Canceled;
        }
    }
}